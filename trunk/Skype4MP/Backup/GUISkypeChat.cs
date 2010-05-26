#region Copyright (C) 2005-2007 Team MediaPortal

/* 
 *	Copyright (C) 2005-2007 Team MediaPortal
 *	http://www.team-mediaportal.com
 *
 *  This Program is free software; you can redistribute it and/or modify
 *  it under the terms of the GNU General Public License as published by
 *  the Free Software Foundation; either version 2, or (at your option)
 *  any later version.
 *   
 *  This Program is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
 *  GNU General Public License for more details.
 *   
 *  You should have received a copy of the GNU General Public License
 *  along with GNU Make; see the file COPYING.  If not, write to
 *  the Free Software Foundation, 675 Mass Ave, Cambridge, MA 02139, USA. 
 *  http://www.gnu.org/copyleft/gpl.html
 *
 */

#endregion

using System;
using System.IO;
using MediaPortal.GUI.Library;
using MediaPortal.Player;
using MediaPortal.Dialogs;


using SKYPE4COMLib;
using System.Threading;
using System.Text;


namespace maniac.MediaPortal.Skype
{
  /// <summary>
  /// Allows the use of Skype Chats within MediaPortal
  /// </summary>
  public class GUISkypeChat : GUIWindow
  {
    enum Controls : int
    {
      MessageList = 11,
      ChatList = 12,
      Input = 51
    }

    private const string TAG_IMG_AVATAR = "#Skype.Image.Avatar";

    #region Attributes

    [SkinControlAttribute(11)]
    protected GUIListControl lstMessages = null;
    [SkinControlAttribute(12)]
    protected GUIListControl lstChats = null;
    [SkinControlAttribute(13)]
    protected GUILabelControl lblChats = null;
    [SkinControlAttribute(14)]
    protected GUILabelControl lblMessages = null;
    [SkinControlAttribute(15)]
    protected GUIImage imgAvatar = null;


    #endregion

    #region locals

    Chat currentChat = null;

    #endregion

    /// <constructor>
    /// Default constructor - sets the window id
    /// </constructor>
    public GUISkypeChat()
    {
      GetID = (int)SkypeHelper.CHAT_WINDOW_ID;
    }

    /// <summary>
    /// Loads the XML for the window
    /// </summary>
    public override bool Init()
    {
      return Load(GUIGraphicsContext.Skin + @"\mySkypeChat.xml");
    }
#if false
        public override void OnAction(Action action)
        {
            if (action.wID != Action.ActionType.ACTION_MOUSE_MOVE)
                Log.Info("SC->OnAction: {0}:{1}",
                    action.wID, action.m_key == null ? "null" : action.m_key.KeyChar.ToString());
            if (action.wID == Action.ActionType.ACTION_PREVIOUS_MENU)
            {
                GUIWindowManager.ShowPreviousWindow();
            }
            if (GetFocusControlId() != (int)Controls.Input)
            {
                // set focus to the default control then
                GUIMessage msg = new GUIMessage(GUIMessage.MessageType.GUI_MSG_SETFOCUS, GetID, 0, (int)Controls.Input, 0, 0, null);
                OnMessage(msg);
            }
            base.OnAction(action);
        }
#endif
    /// <summary>
    /// Handles an Action message
    /// </summary>
    public override void OnAction(Action action)
    {
      if (action.wID == Action.ActionType.ACTION_PREVIOUS_MENU)
      {
        base.OnAction(action);
        return;
      }
      else if (action.wID == Action.ActionType.ACTION_CONTEXT_MENU)
      {
        ShowContextMenu();
        base.OnAction(action);
        return;
      }
      // translate all other actions from regular keypresses back to keypresses
      if (action.wID != Action.ActionType.ACTION_KEY_PRESSED &&
          (action.m_key != null && action.m_key.KeyChar >= 32))
      {
        action.wID = Action.ActionType.ACTION_KEY_PRESSED;
      }
      // Process the keystrokes

      if (action.wID == Action.ActionType.ACTION_KEY_PRESSED)
      {
        // Check focus on sms input control, only for visible characters
        if (action.m_key != null && action.m_key.KeyChar >= 32 && GetFocusControlId() != (int)Controls.Input)
        {
          // set focus to the default control then
          GUIMessage msg = new GUIMessage(GUIMessage.MessageType.GUI_MSG_SETFOCUS, GetID, 0, (int)Controls.Input, 0, 0, null);
          OnMessage(msg);
        }
      }

      base.OnAction(action);
    }


    public override bool OnMessage(GUIMessage message)
    {
      switch (message.Message)
      {
        case GUIMessage.MessageType.GUI_MSG_WINDOW_INIT:
          base.OnMessage(message);
          GUIWindowManager.RouteToWindow(GetID);
          RefreshChatList();
          return true;

        case GUIMessage.MessageType.GUI_MSG_WINDOW_DEINIT:
          GUIWindowManager.UnRoute();
          break;

        case GUIMessage.MessageType.GUI_MSG_CLICKED:
          break;

        case GUIMessage.MessageType.GUI_MSG_NEW_LINE_ENTERED:
          try
          {
            if (message.Label != null && currentChat != null)
            {
              currentChat.SendMessage(message.Label);
            }
          }
          catch (Exception e)
          {
            Log.Warn("SC->Error sending chat message:");
            Log.Warn(e.Message);
            Log.Warn(e.StackTrace);
          }
          break;
      }
      return base.OnMessage(message);
    }

    /// <summary>
    /// OnClicked event handler - only does specific stuff for the Chat List
    /// </summary>
    protected override void OnClicked(int controlId, GUIControl control, Action.ActionType actionType)
    {

      if (controlId == (int)Controls.ChatList)
      {
        currentChat = (Chat)(lstChats.SelectedListItem.AlbumInfoTag);
        DoSelectChat();
      }
      base.OnClicked(controlId, control, actionType);
    }

    #region Methods

    /// <summary>
    /// Add the new Chat message to the list, with basic formatting for
    /// chat control messages (e.g. Leave/Add/User Seen)
    /// </summary>
    private void AddToList(ChatMessage msg)
    {
      GUIListItem item = null;
      if (msg.Type == TChatMessageType.cmeSaid)
      {
        if (msg.Body == String.Empty)
        {
          // Don't add empty messages - they just take up space
          return;
        }
        item = new GUIListItem(String.Format("{0}: {1}", msg.FromHandle, msg.Body));
      }
      else
      {
        StringBuilder s = new StringBuilder(String.Format("{0}: {1} ",
            msg.FromHandle,
            GUISkype.SkypeAccess.Convert.ChatMessageTypeToText(msg.Type)));
        if (msg.Users.Count != 0)
        {
          foreach (User u in msg.Users)
          {
            s.AppendFormat("{0}, ", u.Handle);
          }
          s.Remove(s.Length - 2, 2);
        }
        item = new GUIListItem(s.ToString());
      }
      item.AlbumInfoTag = msg;
      lstMessages.Add(item);
      lstMessages.ScrollToEnd();
      // Set the message status
      if (msg.Status == TChatMessageStatus.cmsReceived)
      {
        msg.Seen = true;
      }
      if (msg.Type == TChatMessageType.cmeLeft)
      {
        if (msg.Chat.Members.Count == 1)
        {
          AddToList("All others have left the Chat");
        }
      }
    }

    /// <summary>
    /// Adds the message just sent from here to the list control.
    /// </summary>
    private void AddToList(string msg)
    {
      GUIListItem item = null;
      item = new GUIListItem(msg);
      item.AlbumInfoTag = null;
      lstMessages.Add(item);
      lstMessages.ScrollToEnd();
    }

    // 
    /// <summary>
    /// A different  Chat has been selected by the user, or on entry to the screen,
    /// so retrieves the chat messages and sets the message header.
    /// Note: Assumes that the chat has been set into currentChat
    /// </summary>
    private void DoSelectChat()
    {
      // Make the new label for the message list
      StringBuilder ss = new StringBuilder("Chat: " + currentChat.FriendlyName + " with ");
      foreach (User user in currentChat.Members)
      {
        if (user.Handle != GUISkype.SkypeAccess.CurrentUserHandle)
        {
          ss.Append(user.Handle + ",");
        }
      }
      ss = ss.Remove(ss.Length - 1, 1);
      lblMessages.Label = ss.ToString();

      // Display any previous messages
      lstMessages.Clear();
      //foreach (ChatMessage msg in currentChat.Messages)  // RecentMessages?
      //Disable the cache while we get the list of messsages
      GUISkype.SkypeAccess.Cache = false;
      ChatMessageCollection msgs = currentChat.Messages;
      for (int i = msgs.Count; i > 0; i--)  // RecentMessages?
      {
        //AddToList(msg);
        AddToList(msgs[i]);
      }
      GUISkype.SkypeAccess.Cache = true;

      if (currentChat.Members.Count > 0)
      {
        ShowUserAvatar(currentChat.Members[1]);
      }
    }

    /// <summary>
    /// ChatMessage Status Change event handler
    /// </summary>
    public void OnMessageStatus(ChatMessage msg, TChatMessageStatus messageStatus)
    {
      // Attempt to make sure that the chat appears in the list if it's a new one
      if (FindChatItem(lstChats, msg.Chat) == null)
      {
        RefreshChatList();
      }
      if (currentChat != null && msg.Chat.Name == currentChat.Name)
      {
        if (messageStatus == TChatMessageStatus.cmsSending)
        {
          AddToList(msg);
        }
        else if (messageStatus == TChatMessageStatus.cmsReceived)
        {
          AddToList(msg);
        }
        else if (messageStatus == TChatMessageStatus.cmsRead)
        {
          GUIListItem item = FindMessageItem(lstMessages, msg);
          if (item == null)
          {
            AddToList(msg);
          }
          else
          {
            // Change status to Read (doesn't do anything at present)
          }
        }
      }
    }

    /// <summary>
    /// Find the specified message in the given list control.
    /// </summary>
    /// <param name="lst">A list control with Chat Messages in its AlbumInfoTags</param>
    /// <param name="msg">The message to find (matches on Id)</param>
    private GUIListItem FindMessageItem(GUIListControl lst, ChatMessage msg)
    {
      for (int ii = 0; ii < lst.Count; ii++)
      {
        GUIListItem item = lst[ii];
        ChatMessage m = item.AlbumInfoTag as ChatMessage;
        if (m != null && m.Id == msg.Id)
        {
          return item;
        }
      }

      return null;
    }

    /// <summary>
    /// Find the given CHat in the given list
    /// </summary>
    /// <param name="lst">List control with Chats as AlbumInfoTags</param>
    /// <param name="chat">Chat to be found</param>
    private GUIListItem FindChatItem(GUIListControl lst, Chat chat)
    {
      for (int ii = 0; ii < lst.Count; ii++)
      {
        GUIListItem item = lst[ii];
        Chat c = item.AlbumInfoTag as Chat;
        if (c != null && c.Name == chat.Name)
        {
          return item;
        }
      }

      return null;
    }

    /// <summary>
    /// Reloads the Chats from Skype's Recent Chats list.
    /// </summary>
    /// <remarks>
    /// Note that Skype.ActiveChats appears to be those Chats that have open Chat windows
    /// in the Skype Client, which is why we use RecentChats...
    /// </remarks>
    private void RefreshChatList()
    {
      if (lstChats == null)
      {
        return;
      }
      // Get the chats that are most likely to be interesting.
      lblChats.Label = "Recent chats:";
      lstChats.Clear();
      GUISkype.SkypeAccess.Cache = false;
      foreach (Chat chat in GUISkype.SkypeAccess.RecentChats)
      {
        if (FindChatItem(lstChats, chat) == null)
        {
          GUIListItem item = new GUIListItem(chat.FriendlyName);
          item.AlbumInfoTag = chat;
          lstChats.Add(item);
        }
      }
      GUISkype.SkypeAccess.Cache = true;
      lblMessages.Label = String.Empty;
    }

    /// <summary>
    /// Display the context menu for the Chat list
    /// </summary>
    private void ShowContextMenu()
    {
      if (lstChats.IsFocused)
      {
        GUIListItem item = GUIControl.GetSelectedListItem(GetID, (int)Controls.ChatList);
        if (item == null) return;

        GUIDialogMenu dlg = (GUIDialogMenu)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_MENU);
        if (dlg == null) return;

        Chat chat = (Chat)item.AlbumInfoTag;
        if (chat == null)
        {
          return;
        }

        dlg.Reset();
        dlg.SetHeading("Chat actions: " + chat.FriendlyName);

        dlg.Add("Leave chat");
        //dlg.Add("Clear chat history"); Can't be done per chat?

        dlg.DoModal(GetID);
        if (dlg.SelectedLabel < 0)
        {
          return;
        }
        // Using the label text allows buttons to be present or not,
        // but complicates the ones where we add or change the text depending on 
        // context (e.g. Send SMS)
        switch (dlg.SelectedLabelText)
        {
          case "Leave chat":
            try
            {
              chat.Leave();
              RefreshChatList();
            }
            catch (Exception)
            { }
            break;

          case "Clear chat history":
            // Probably doesn't work...
            chat.Messages.RemoveAll();
            break;

          default:
            break;
        }
      }
    }


    // Our guy is asking to Chat with a specific user.
    // Search the list to see if any of the Recent chats involved the target,
    // and auto-select it if so.  Otherwise, we need to start a new chat with
    // the sucker...
    /// <summary>
    /// Search the chat lsit control for a chat with the given user, and select
    /// it if found. Otherwise, starts a new chat with the User
    /// </summary>
    /// <param name="user">User with whom to chat</param>
    public void ChatWith(User user)
    {

      for (int ii = 0; ii < lstChats.Count; ii++)
      {
        Chat chat = lstChats[ii].AlbumInfoTag as Chat;
        if (chat != null)
        {
          foreach (User m in chat.Members)
          {
            if (m.Handle == user.Handle)
            {
              // Found one, so use it (we don't look for any more)
              currentChat = chat;
              DoSelectChat();
              return;
            }
          }
        }
      }

      // OK - if we get here there's no existng chat to use for this guy.
      // Make a new one, refresh the list, and then activate it.
      currentChat = GUISkype.SkypeAccess.CreateChatWith(user.Handle);
      RefreshChatList();
      DoSelectChat();
    }

    /// <summary>
    /// Retrieve the user's Avatar (if necessary) and display it in the image control
    /// </summary>
    private void ShowUserAvatar(User user)
    {
      if (GUIGraphicsContext.IsPlayingVideo || user == null)
      {
        imgAvatar.Visible = false;
      }
      else
      {
        imgAvatar.Visible = true;
        String strAvatar = GUIGraphicsContext.Skin + "\\..\\..\\plugins\\windows\\SkypeAvatars\\" + user.Handle + ".jpg";
        if (!File.Exists(strAvatar))
        {
          // Get the avatar
          Command cmd = new Command();
          string s = "USER " + user.Handle + " AVATAR 1 " + strAvatar;
          cmd.Command = "GET " + s;
          cmd.Blocking = true;
          cmd.Expected = s;
          GUISkype.SkypeAccess.SendCommand(cmd);
        }
        GUIPropertyManager.SetProperty(TAG_IMG_AVATAR, strAvatar);
      }
    }


    #endregion

  }
}
