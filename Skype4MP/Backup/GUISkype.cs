#define SHOW_AUDIO_DEVICES

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Microsoft.Win32;

using MediaPortal.Dialogs;
using MediaPortal.GUI.Library;
using MediaPortal.Util;

using SKYPE4COMLib;
using MediaPortal.Player;


namespace maniac.MediaPortal.Skype
{
  /// <summary>
  /// Description résumée de GUISkype.
  /// </summary>
  public class GUISkype : GUIWindow, ISetupForm, IComparer<GUIListItem>
  {
    #region Internal Classes
    /// <summary>
    /// Internal class to manage objects observation
    /// </summary>
    private class Observer
    {
      private GUISkype m_parent = null;
      private SKYPE4COMLib.Call m_currentCall = null;
      private SKYPE4COMLib.User m_currentUser = null;

      public Observer(GUISkype Parent)
      {
        if (Parent == null) throw new ArgumentNullException("Parent");
        m_parent = Parent;

        return;
      }

      ~Observer()
      {
        m_currentUser = null;
        m_currentCall = null;
        m_parent = null;
        return;
      }


      public SKYPE4COMLib.Call CurrentCall
      {
        get
        {
          return m_currentCall;
        }
        set
        {
          m_currentCall = value;
        }
      }

      public SKYPE4COMLib.User CurrentUser
      {
        get
        {
          return m_currentUser;
        }
        set
        {
          m_currentUser = value;
          m_parent.UpdateUserDisplay(m_currentUser);
        }
      }
    }

    #endregion

    #region CONST
    private const string TAG_USER_STATUS = "#Skype.User.Status";
    private const string TAG_CALL_STATUS = "#Skype.Call.Status";
    private const string TAG_CALL_DURATION = "#Skype.Call.Duration";
    private const string TAG_CONTACT_NAME = "#Skype.Contact.Name";
    private const string TAG_CONTACT_STATUS = "#Skype.Contact.Status";
    private const string TAG_CONTACT_COUNT = "#Skype.Contact.Count";
    private const string TAG_MISSED_CALLS = "#Skype.Call.Missed";
    private const string TAG_MISSED_CALLS2 = "#Skype.Call.2Missed";
    private const string TAG_SKYPE_CREDIT = "#Skype.Credit";
    private const string TAG_IMG_STATUS = "#Skype.Image.MyStatus";
    private const string TAG_IMG_AVATAR = "#Skype.Image.Avatar";
    private const string TAG_CONTACT_MOODTEXT = "#Skype.Contact.MoodText";

    enum SortMethod
    {
      SORT_NAME = 0,
      SORT_STATUS = 1,  // Keep this at the end, please...
    }

    string[] SortMethodName = {
            "By name",
            "By status"
        };


    private enum Controls
    {
      CONTROL_BTNSKYPEOUT = 2,
      CONTROL_BTNSTATUS = 3, 
      CONTROL_BTNHISTORY = 5,
      CONTROL_BTNCHATS = 10,
      CONTROL_BTNSILENTMODE = 11,

      CONTROL_IMG_STATUS = 8,
      CONTROL_START_SKYPE = 9,
      CONTROL_USER_STATUS = 12,
      CONTROL_MOOD_TEXT = 21,
      CONTROL_CONTACT_NAME = 22,

      CONTROL_BTNSORTORDER = 27,
      CONTROL_CALL_STATUS = 29,
      CONTROL_CALL_DURATION = 30,
      CONTROL_LSTCONTACTS = 40,
      CONTROL_LSTCALLS = 41,
      CONTROL_MISSEDCALLS = 42,
      CONTROL_MISSEDCALLS2 = 43,
      CONTROL_IMG_AVATAR = 44
    };

    #endregion

    #region API Calls
    [DllImport("user32.dll", SetLastError = true)]
    static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

    [DllImport("user32.dll")]
    public static extern int SendMessage(
          IntPtr hWnd,      // handle to destination window
          uint Msg,       // message
          long wParam,  // first message parameter
          long lParam   // second message parameter
    );

    private const int WM_CLOSE = 0x0010;

    #endregion

    #region Attributes
    private static SKYPE4COMLib.Skype m_skypeAccess = null;
    private static SKYPE4COMLib.UserCollection m_skypeFriends = null;

    private bool m_apiAttached = false;

    // Options
    private bool m_startSkype = true;
    private bool m_stopSkype = false;
    private bool m_HangupIfDND = true;
    private bool m_HandleChats = true;
    private bool m_HandleVoiceCalls = true;
    private bool m_AllowSkypeOutCalls = true;
    private bool m_AllowSMS = true;
    private bool m_PauseForCalls = true;
    private bool m_PauseForChats = true;
    private bool m_UseSilentMode = false;
    private bool m_IgnoreIncomingCalls = false;

    private OptionTypeList options = new OptionTypeList();

    //private bool m_DNDwhilePlaying = true;
    private static Observer m_observer = null;
    private GUIDialogYesNo m_acceptanceWindow = null;
    private Hashtable m_currentCalls = null;
    private Hashtable m_callsId = null;

    // Sorting stuff...
    SortMethod currentSortMethod = SortMethod.SORT_NAME;

    [SkinControlAttribute(2)]
    protected GUIToggleButtonControl btnSkypeOut = null;
    [SkinControlAttribute(3)]
    protected GUIButtonControl btnStatus = null;
    [SkinControlAttribute(5)]
    protected GUIButtonControl btnHistory = null;
    [SkinControlAttribute(10)]
    protected GUIButtonControl btnChats = null;
    [SkinControlAttribute(11)]
    protected GUIToggleButtonControl btnSilentMode = null;
    [SkinControlAttribute(9)]
    protected GUIButtonControl btnStartSkype = null;
    [SkinControlAttribute(21)]
    protected GUIFadeLabel lblMoodText = null;
    [SkinControlAttribute(27)]
    protected GUIButtonControl btnSortOrder = null;
    [SkinControlAttribute(40)]
    protected GUIListControl lstContacts = null;
    [SkinControlAttribute(41)]
    protected GUIListControl lstCalls = null;
    [SkinControlAttribute(44)]
    protected GUIImage imgAvatar = null;

    [SkinControlAttribute(13)]
    protected GUIButtonControl btnSettings = null;

#if SHOW_AUDIO_DEVICES
    [SkinControlAttribute(14)]
    protected GUIButtonControl btnAudioDevices = null;
#endif

    #endregion

    #region Properties
    public static SKYPE4COMLib.Skype SkypeAccess
    {
      get { return m_skypeAccess; }
    }

    public static User CurrentCallTarget
    {
      get
      {
        if (m_observer == null)
        {
          return null;
        }
        return m_observer.CurrentUser;
      }
    }

    #endregion

    #region Constructors
    public GUISkype()
    {

      RegistryKey skypeRK = null;

      LoadSettings();
      try
      {
        GetID = SkypeHelper.MAIN_WINDOW_ID;

        // check for skype installed
        /*
         To see if Skype is installed check the following registry key:
        HKCU\Software\Skype\Phone '?SkypePath' . The key will point to the location of
        skype.exe. In case that key is missing the application should also check for
        existance of HKLM\Software\Skype\Phone '?SkypePath' (if HKCU is missing, but
        HKLM is present, it indicates that skype has been installed from an administrator
        account, but not yet used from current account).
         */
        skypeRK = Registry.CurrentUser.OpenSubKey(@"Software\Skype\Phone");
        if (skypeRK.GetValue("SkypePath") == null)
        {
          skypeRK = Registry.LocalMachine.OpenSubKey(@"Software\Skype\Phone");
          if (skypeRK.GetValue("SkypePath") == null) throw new ApplicationException("Skype is not installed");
          else throw new ApplicationException("Skype is not installed for this account");
        }

        // check for skype running -- only if mediaportal.exe running, not configuration.exe
        if (System.Diagnostics.Process.GetCurrentProcess().ProcessName == "MediaPortal")
        {

          m_currentCalls = new Hashtable();
          m_callsId = new Hashtable();
          m_skypeAccess = new SKYPE4COMLib.Skype();

          // API events registering
          // These ones are always required
          ((_ISkypeEvents_Event)m_skypeAccess).AttachmentStatus += OnAPIStatusChanged;
          ((_ISkypeEvents_Event)m_skypeAccess).ConnectionStatus += new _ISkypeEvents_ConnectionStatusEventHandler(OnConnectionStatusChanged);
          ((_ISkypeEvents_Event)m_skypeAccess).Mute += OnMuteChanged;
          ((_ISkypeEvents_Event)m_skypeAccess).SilentModeStatusChanged += OnSilentModeStatusChanged;
          m_skypeAccess.Command += OnCommand;
          m_skypeAccess.Reply += OnResult;
          m_skypeAccess.Error += new _ISkypeEvents_ErrorEventHandler(OnError);
          m_skypeAccess.OnlineStatus += OnOnlineStatusChanged;
          m_skypeAccess.UserStatus += OnUserStatusChanged;

          ((_ISkypeEvents_Event)m_skypeAccess).CallVideoReceiveStatusChanged += OnVideoReceiveStatusChanged;

          // These are only for Calls
          if (m_HandleVoiceCalls)
          {
            m_skypeAccess.CallHistory += OnCallHistoryChanged;
            m_skypeAccess.CallStatus += OnCallStatusChanged;
          }

          // These are only for Chats and Chat messages
          if (m_HandleChats)
          {
            m_skypeAccess.MessageHistory += OnIMHistoryChanged;
            m_skypeAccess.MessageStatus += OnMessageStatus;
            //((_ISkypeEvents_Event)m_skypeAccess).MessageSent += new _IAccessEvents_MessageSentEventHandler(OnMessageSent);
          }
          if (m_startSkype == true)
          {
            Log.Debug("SP->Allowed to start Skype");
            if (!m_skypeAccess.Client.IsRunning)
            {
              Log.Debug("SP->Starting Skype client");
              m_skypeAccess.Client.Start(true, true);
              Log.Debug("SP->Started  Skype client");
            }
          }
          Log.Debug("SP->Attaching to Skype");
          m_skypeAccess.Attach(9999, false);     // 9999 will force the latest protocol to be used

        }
      }
      catch (Exception e)
      {
        Log.Warn("Unable to initialise GUISkype");
        Log.Warn(e.Message);
        Log.Error(e.StackTrace);
      }
      Log.Debug("SP->Skype attached");

      // Load the keymap for the Chat Window
      ActionTranslator.Load("keymap_skype.xml");
      return;
    }

    ~GUISkype()
    {
      // check for skype running -- only if mediaportal.exe running, not configuration.exe
      if (System.Diagnostics.Process.GetCurrentProcess().ProcessName == "MediaPortal")
      {
        // if asked to stop with MP
        if (m_stopSkype == true)
        {
          Process[] processes = System.Diagnostics.Process.GetProcessesByName("Skype");
          if (processes.Length == 1)
          {
            Process skypeProcess = processes[0];
            skypeProcess.Kill();
          }
        }
      }
    }
    #endregion

    #region Membres de ISetupForm
    public bool CanEnable()
    {
      return true;
    }

    public string Description()
    {
      return SkypeHelper.PLUGIN_DESCRIPTION;
    }

    public bool DefaultEnabled()
    {
      return false;
    }

    public int GetWindowId()
    {
      return SkypeHelper.MAIN_WINDOW_ID;
    }

    public bool GetHome(out string strButtonText, out string strButtonImage, out string strButtonImageFocus, out string strPictureImage)
    {
      strButtonText = "Skype";
      strButtonImage = string.Empty;
      strButtonImageFocus = string.Empty;
      strPictureImage = string.Empty;
      return true;
    }

    public string Author()
    {
      return SkypeHelper.PLUGIN_AUTHOR;
    }

    public string PluginName()
    {
      return SkypeHelper.PLUGIN_NAME;
    }

    public bool HasSetup()
    {
      return true;
    }

    public void ShowPlugin()
    {
      SkypeSetupForm ssf = new SkypeSetupForm();
      ssf.ShowDialog();
      return;
    }

    #endregion

    #region SKYPE event handlers
    private void OnAPIStatusChanged(TAttachmentStatus Status)
    {
      m_apiAttached = (Status == TAttachmentStatus.apiAttachSuccess);
      Log.Debug("SP->API status: " + Status.ToString() + " (" + m_apiAttached + ")");
      if (m_apiAttached || Status == TAttachmentStatus.apiAttachNotAvailable)
      {
        try
        {
          if (this != null)
          {
            ResetEntireWindow();
          }
        }
        catch (Exception e)
        {
          Log.Warn("Caught exception calling ResetEntireWindow");
          Log.Warn(e.Message);
          Log.Warn(e.StackTrace);
        }
        MinimizeSkypeClient();
        if (m_apiAttached)
        {
          if (m_UseSilentMode)
          {
            m_skypeAccess.SilentMode = true;
          }
          Log.Info("SP->Versions: Skype {0}, Skype4COM {1}, Protocol {2}, Skype4MP {3}",
              m_skypeAccess.Version,
              m_skypeAccess.ApiWrapperVersion,
              m_skypeAccess.Protocol,
              System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString()
          );

          Settings sets = m_skypeAccess.Settings;
          Log.Info("SP->AudioIn  '{0}'", sets.AudioIn);
          Log.Info("SP->AudioOut '{0}'", sets.AudioOut);
          Log.Info("SP->Ringer   '{0}'", sets.Ringer);
          Log.Info("SP->VideoIn  '{0}'", sets.VideoIn);
          Log.Info("SP->Language '{0}'", sets.Language);
        }
      }
      else if (Status == TAttachmentStatus.apiAttachAvailable)
      {
        try
        {
          m_skypeAccess.Attach(9999, true);
        }
        catch (Exception)
        {
        }
      }
    }

    private void OnCallHistoryChanged()
    {
    }

    private void OnCallStatusChanged(Call ChangedCall, TCallStatus CallProgress)
    {
      switch (CallProgress)
      {
        case TCallStatus.clsFinished:
        case TCallStatus.clsCancelled:
        case TCallStatus.clsRefused:
          Log.Debug("SP->CallStatus Finished/Refused/Cancelled");

          if (m_acceptanceWindow != null)
          {
            // acceptance windows is actually show
            Log.Debug("SP->CallStatus closing acceptance");
            Action a = new Action(Action.ActionType.ACTION_CLOSE_DIALOG, 0, 0);
            m_acceptanceWindow.OnAction(a);
            m_acceptanceWindow = null;
          }

          if (m_UseSilentMode)
          {
            m_skypeAccess.SilentMode = true;
            btnSilentMode.Selected = true;
          }

          try
          {
            RemoveCall(ChangedCall.PartnerDisplayName);
          }
          catch (Exception e)
          {
            Log.Debug("SP->" + e.Message);
          }

          if (m_observer.CurrentCall == null || m_observer.CurrentCall.Id == ChangedCall.Id)
          {
            m_observer.CurrentCall = null;

            UpdateCallDisplay(CallProgress);

          }
          break;

        case TCallStatus.clsFailed:
          Log.Debug("SP->CallStatus Failed");

          UpdateCallDisplay(CallProgress);
          RemoveCall(ChangedCall.PartnerDisplayName);
          m_observer.CurrentCall = null;
          break;

        case TCallStatus.clsRinging:
          if ((ChangedCall.Type == TCallType.cltIncomingP2P ||
               ChangedCall.Type == TCallType.cltIncomingPSTN) &&
            !m_IgnoreIncomingCalls)
          {
            MinimizeSkypeClient();
            if (m_HangupIfDND && m_skypeAccess.CurrentUserStatus == TUserStatus.cusDoNotDisturb)
            {
              ChangedCall.Finish();
              return;
            }

            bool wePausedIt = false;
            if (g_Player.Playing && !g_Player.Paused && m_PauseForCalls)
            {
              g_Player.Pause();
              // Only resume automatically if we actually paused it for this...
              wePausedIt = true;
            }

            if (m_observer.CurrentCall == null || m_observer.CurrentCall == ChangedCall)
            {
              UpdateCallDisplay(CallProgress);
            }

            // init question dialog
            try
            {
              m_acceptanceWindow = (GUIDialogYesNo)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_YES_NO);
              m_acceptanceWindow.SetHeading("Incoming Call");
              m_acceptanceWindow.SetLine(1, ChangedCall.PartnerDisplayName + " is calling you.");
              m_acceptanceWindow.SetLine(2, "Do you want to answer?");
              if (m_observer.CurrentCall != null)
              {
                m_acceptanceWindow.SetLine(3, "(The current call will be paused)");
              }
              else
              {
                m_observer.CurrentCall = ChangedCall;
              }
              m_acceptanceWindow.DoModal(GetID);

              if (m_acceptanceWindow.IsConfirmed)
              {
                ChangedCall.Answer();
                AcceptNewCall(ChangedCall);
              }
              else
              {
                // refuse the call
                ChangedCall.Finish();
                if (m_observer.CurrentCall == null || m_observer.CurrentCall == ChangedCall)
                {
                  UpdateCallDisplay(TCallStatus.clsRefused);
                }
                if (g_Player.Playing && g_Player.Paused && m_PauseForCalls && wePausedIt)
                {
                  g_Player.Pause();
                }
              }
              m_acceptanceWindow = null;
            }
            catch (Exception e)
            {
              Log.Debug("SP->{0}", e.Message);
            }
          }
          else
          {
            UpdateCallDisplay(CallProgress);
          }
          break;

        case TCallStatus.clsMissed:
          Log.Debug("SP->CallStatus Missed");

          if (m_acceptanceWindow != null)
          {
            Log.Debug("SP->CallStatus closing acceptance (missed)");
            // acceptance windows is actually show
            Action a = new Action(Action.ActionType.ACTION_CLOSE_DIALOG, 0, 0);
            m_acceptanceWindow.OnAction(a);
            m_acceptanceWindow = null;
          }
          try
          {
            RemoveCall(ChangedCall.PartnerDisplayName);
          }
          catch (Exception e)
          {
            Log.Debug("SP->" + e.Message);
          }

          UpdateCallDisplay(CallProgress);
          m_observer.CurrentCall = null;
          UpdateMissedCalls();
          break;

        case TCallStatus.clsOnHold:
        case TCallStatus.clsRemoteHold:
          Log.Debug("SP->CallStatus OnHold");

          if (m_observer.CurrentCall != null && m_observer.CurrentCall.Id == ChangedCall.Id)
          {
            UpdateCallDisplay(CallProgress);
          }
          break;

        case TCallStatus.clsInProgress:
          Log.Debug("SP->CallStatus InProgress");
          if (m_acceptanceWindow != null)
          {
            Log.Debug("SP->CallStatus closing acceptance (inprogress)");
            // acceptance windows is actually show
            Action a = new Action(Action.ActionType.ACTION_CLOSE_DIALOG, 0, 0);
            m_acceptanceWindow.OnAction(a);
            m_acceptanceWindow = null;
            AcceptNewCall(ChangedCall);
          }
          UpdateCallDisplay(CallProgress);

          break;

        default:
          Log.Debug("SP->CallStatus default {0}", CallProgress);
          UpdateCallDisplay(CallProgress);
          break;
      }
      UpdateCall(ChangedCall, CallProgress);
      // Try to keep the main Window from appearing when it shouldn't.
      // TODO: THis should probably be an option, to avoid problems for other people...
      if (m_observer.CurrentCall == null ||
            (m_observer.CurrentCall.VideoReceiveStatus != TCallVideoSendStatus.vssRunning &&
             m_observer.CurrentCall.VideoReceiveStatus != TCallVideoSendStatus.vssStarting)
        )
      {
        MinimizeSkypeClient();
      }

    }

    private void OnCommand(ICommand SkypeCommand)
    {
    }

    private void OnConnectionStatusChanged(TConnectionStatus Status)
    {
      Log.Debug("SP->Connection status : {0}", Status);
      if (Status == TConnectionStatus.conOffline)
      {
        ResetEntireWindow();
      }
    }

    private void OnError(ICommand command, int Number, string Description)
    {
      Log.Warn("SP->Error : number " + Number.ToString() + ", description : " + Description);
    }
    private void OnIMHistoryChanged(String user)
    {
      Log.Warn("SP->IMHistoryChanged: user " + user);
    }

    // Forward MessageStatusChanged events to the Chat window for processing (maybe)
    private void OnMessageStatus(SKYPE4COMLib.ChatMessage SkypeMessage, TChatMessageStatus messageStatus)
    {
      // Send the message to the chat window anyway...
      GUIWindow w = GUIWindowManager.GetWindow(SkypeHelper.CHAT_WINDOW_ID);
      ((GUISkypeChat)w).OnMessageStatus(SkypeMessage, messageStatus);
      if (GUIWindowManager.ActiveWindow != (int)SkypeHelper.CHAT_WINDOW_ID)
      {
        if (ConfirmSwitchToChat(SkypeMessage, messageStatus))
        {
          if (GUIGraphicsContext.IsFullScreenVideo)
          {
            GUIGraphicsContext.IsFullScreenVideo = false;
          }
          GUIWindowManager.ActivateWindow((int)SkypeHelper.CHAT_WINDOW_ID);
          ((GUISkypeChat)w).ChatWith(SkypeMessage.Sender);
        }
      }
    }

    private void OnMuteChanged(bool Mute)
    {
    }

    private GUIListItem FindContactItem(GUIListControl lst, User SkypeUser)
    {
      for (int i = 0; i < lst.Count; i++)
      {
        GUIListItem gli = GUIControl.GetListItem(GetID, (int)Controls.CONTROL_LSTCONTACTS, i);
        if (((User)(gli.AlbumInfoTag)).Handle.Equals(SkypeUser.Handle))
        {
          return gli;
        }
      }
      return null;
    }

    private void OnOnlineStatusChanged(SKYPE4COMLib.User SkypeUser, TOnlineStatus SkypeStatus)
    {
      GUIListItem item = null;
      string strThumb = string.Empty;

      item = FindContactItem(lstContacts, SkypeUser);

      if (item != null)
      {
        strThumb = SkypeHelper.GetStatusImage(SkypeUser.OnlineStatus); // Another User
        if (System.IO.File.Exists(strThumb))
        {
          item.ThumbnailImage = strThumb;
          item.IconImage = strThumb;
          item.IconImageBig = strThumb;
        }
      }
      GUIControl.RefreshControl(GetID, (int)Controls.CONTROL_LSTCONTACTS);
      lstContacts.Sort(this);
    }

    // This is a pseudo-event handler - Video Receive Status 
    // changes can now be received as OnResult events.
    // TODO: This should become a real event handler
    private void OnVideoReceiveStatusChanged_Old(int call, string sts)
    {
      Log.Debug("SP->OnResult: Video Reception for Call {0} {1}",
          call,
          sts
          );
      if (sts.Equals("STARTING") || sts.Equals("RUNNING"))
      {
        m_skypeAccess.Client.Focus();
      }
      else if (sts.Equals("STOPPING"))
      {
        MinimizeSkypeClient();
        GUIMessage msg = new GUIMessage(GUIMessage.MessageType.GUI_MSG_GOTO_WINDOW, GetID, 0, 0, 0, 0, null);
        OnMessage(msg);
      }
    }

    private void OnVideoReceiveStatusChanged(Call call, TCallVideoSendStatus sts)
    {
      Log.Debug("SP->OnVideoRcvStatus: Video Reception for Call {0} {1}",
        call.Id,
        sts
      );
      if (sts==TCallVideoSendStatus.vssStarting || sts == TCallVideoSendStatus.vssRunning)
      {
        m_skypeAccess.Client.Focus();
        btnSilentMode.Selected = false;
      }
      else if (sts == TCallVideoSendStatus.vssStopping)
      {
        if (m_UseSilentMode)
        {
          m_skypeAccess.SilentMode = true;
          btnSilentMode.Selected = true;
        }

        MinimizeSkypeClient();
        GUIMessage msg = new GUIMessage(GUIMessage.MessageType.GUI_MSG_GOTO_WINDOW, GetID, 0, 0, 0, 0, null);
        OnMessage(msg);
      }

    }

    private void OnSilentModeStatusChanged(Boolean sts)
    {
      // Set the checkbox
      btnSilentMode.Selected = sts; // (sts == "ON");
      Log.Debug("SP->OnSilent: Status {0}", sts);
    }

    private void OnResult(ICommand Description)
    {
      if (!m_apiAttached)
      {
        return;
      }

      if (Description.Reply.Contains("SILENT_MODE"))
      {
        //OnSilentModeStatusChanged(
        //    Description.Reply.Substring(Description.Reply.IndexOf("SILENT_MODE ") + "SILENT_MODE ".Length) == "ON"
        //);
        Log.Debug("SP->OnResult: SilentModeStatus {0}", Description.Reply); //.Substring(Description.Reply.IndexOf("SILENT_MODE ") + "SILENT_MODE ".Length));
        return;
      }

      // Check for a call duration message and update the display if appropriate
      if (Description.Command.Equals(string.Empty))
      {
        String[] words = Description.Reply.Split(' ');
        if (Description.Reply.Contains("DURATION"))
        {
          // Extract the call id and duration from the reply
          if (words[0].Equals("CALL") && words[2].Equals("DURATION"))
          {
            UpdateCallDuration(Convert.ToInt32(words[1]), Convert.ToInt32(words[3]));
          }
        }
        else if (Description.Reply.Contains("VIDEO_SEND_STATUS"))
        {
          Log.Debug("SP->OnResult: Command '{0}' Reply '{1}'",
              Description.Command,
              Description.Reply
              );
        }
        else if (Description.Reply.StartsWith("PROFILE PSTN_BALANCE"))
        {
          ShowSkypeCredits();
        }
        else if (Description.Reply.Contains("VIDEO_RECEIVE_STATUS"))
        {
          //OnVideoReceiveStatusChanged(Convert.ToInt32(words[1]), words[3]);
        }
#if false
                if (!Description.Reply.Contains("ONLINESTATUS") && 
                    !Description.Reply.Contains("DURATION") &&
                     !Description.Reply.Contains("CONNSTATUS") &&
                     !Description.Reply.Contains("SEARCH") &&
                    !Description.Reply.Contains("GET CHATMESSAGE"))
                {
                    Log.Info("SP->OnResult: Reply '{0}'",
                        Description.Reply
                        );
                }
#endif
      }
#if false
            else
            {
                if (!Description.Command.Contains("GET USER") &&
                    !Description.Command.Contains("GET GROUP") &&
                    !Description.Command.Contains("MINIMIZE") &&
                    !Description.Command.Contains("GET CHATMESSAGE") &&
                    !Description.Command.Contains("SEARCH RECENTCHATS") &&
                    !Description.Command.Contains("GET CHAT "))
                {
                    Log.Info("SP->OnResult: Command '{0}' Reply '{1}'",
                        Description.Command,
                        Description.Reply
                        );
                }
            }
#endif
    }

    private void OnUserStatusChanged(TUserStatus SkypeStatus)
    {
      if (m_apiAttached)
      {
        GUIPropertyManager.SetProperty(TAG_USER_STATUS, SkypeHelper.GetOnlineStatus(m_skypeAccess.CurrentUserStatus));
        GUIPropertyManager.SetProperty(TAG_IMG_STATUS, SkypeHelper.GetStatusImage(m_skypeAccess.CurrentUserStatus));
      }
      else
      {
        GUIPropertyManager.SetProperty(TAG_USER_STATUS, "Offline");
        GUIPropertyManager.SetProperty(TAG_IMG_STATUS, SkypeHelper.GetStatusImage(TUserStatus.cusOffline));
      }
      ShowSkypeCredits();
    }

    #endregion

    #region Methods
    private void ShowSkypeCredits()
    {
      // Update the Skype Credit as well, just for fun
      String creds;
      if (m_apiAttached)
      {
        if (m_skypeAccess.CurrentUserProfile.Balance == 0)
        {
          creds = "No Skype credit remaining";
        }
        else
        {
          creds = "Skype credit: " +
              m_skypeAccess.CurrentUserProfile.BalanceCurrency + " " +
              m_skypeAccess.CurrentUserProfile.BalanceToText;
        }
      }
      else
      {
        creds = "";
      }
      GUIPropertyManager.SetProperty(TAG_SKYPE_CREDIT, creds);
    }

    // A messgage has arrived but the Chat window isn;t active - see
    // if we should switch to it.
    private bool ConfirmSwitchToChat(ChatMessage msg, TChatMessageStatus messageStatus)
    {
      if (msg.Body != string.Empty)
      {
        bool wePausedIt = false;
        if (g_Player.Playing && !g_Player.Paused && m_PauseForCalls)
        {
          g_Player.Pause();
          // Only resume automatically if we actually paused it for this...
          wePausedIt = true;
        }

        // init question dialog
        m_acceptanceWindow = (GUIDialogYesNo)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_YES_NO);
        m_acceptanceWindow.SetHeading("Incoming Chat Message");
        m_acceptanceWindow.SetLine(1, msg.FromDisplayName + " says");
        m_acceptanceWindow.SetLine(2, msg.Body);
        m_acceptanceWindow.SetLine(3, "Do you want to reply?");
        m_acceptanceWindow.DoModal(GetID);

        bool res = m_acceptanceWindow.IsConfirmed;
        m_acceptanceWindow = null;

        // If we're not going to respond, and we did pause the video, restart it...
        if (!res && g_Player.Playing && g_Player.Paused && m_PauseForCalls && wePausedIt)
        {
          g_Player.Pause();
        }

        return res;
      }
      return false;
    }

    private void LoadSkypeFriends()
    {
      int i = 0;
      string friendName = string.Empty;
      string contactsCount = SkypeHelper.EMPTY_STRING;
      try
      {
        // load list
        if (!m_apiAttached)
        {
          m_skypeFriends = new UserCollection();
        }
        else //if( m_skypeFriends == null )
        {
          m_skypeFriends = m_skypeAccess.Friends;
        }

        GUIControl.ClearControl(GetID, (int)Controls.CONTROL_LSTCONTACTS);

        // Load friends list
        for (i = 1; i <= m_skypeFriends.Count; i++)
        {
          SKYPE4COMLib.User friend = null;
          GUIListItem item = new GUIListItem();
          string strThumb = string.Empty;

          friend = m_skypeFriends[i];
          if (friend == null) continue;

          // Check the SkypeOut button
          if (btnSkypeOut.Selected != friend.IsSkypeOutContact) continue;

          // whose name to use
          friendName = SkypeHelper.GetFriendNameToUse(friend);

          // create friend list item
          item.Label = friendName;
          item.AlbumInfoTag = friend;
          strThumb = SkypeHelper.GetStatusImage(friend.OnlineStatus); //Another User
          if (System.IO.File.Exists(strThumb))
          {
            item.ThumbnailImage = strThumb;
            item.IconImage = strThumb;
            item.IconImageBig = strThumb;
          }
          lstContacts.Add(item);
//          GUIControl.AddListItemControl(GetID, (int)Controls.CONTROL_LSTCONTACTS, item);
        }

        if (lstContacts.Count > 0)
        {
          contactsCount = lstContacts.Count.ToString() + " contacts";
          btnSortOrder.Disabled = false;
        }
        else
        {
          contactsCount = "No contacts";
          btnSortOrder.Disabled = true;
        }
        lstContacts.Sort(this);
      }
      catch (Exception e)
      {
        Log.Warn("Failed to load contacts list : {0}\n{1}", e.Message, e.StackTrace);
        contactsCount = "No contacts";
      }
      finally
      {
        GUIPropertyManager.SetProperty(TAG_CONTACT_COUNT, contactsCount);
      }
    }
    public void AddCall(string PartnerDisplayName, int CallID, TCallType CallType)
    {
      Log.Info("SP->AddCall({0},{1},{2})", PartnerDisplayName, CallID, CallType);
      try
      {
        // add name id pair
        m_currentCalls.Add(PartnerDisplayName, CallID);

        GUIListItem item = MakeCallListItem(CallID);
        AddCallStatusToLabel(item);
        lstCalls.Disabled = false;
        lstCalls.Add(item);
        lstCalls.Visible = true;
      }
      catch (Exception)
      {
        Log.Warn("SP->Unable to add the call to current calls list");
      }
    }

    private void AddCallStatusToLabel(GUIListItem item)
    {
      TCallStatus callProgress = m_skypeAccess.get_Call(item.ItemId).Status;
      String lbl = item.Label;
      int pos = lbl.IndexOf(" - ");
      if (pos < 0)
      {
        pos = lbl.Length;
      }
      if (callProgress == TCallStatus.clsLocalHold || callProgress == TCallStatus.clsOnHold || callProgress == TCallStatus.clsRemoteHold)
      {
        lbl = lbl.Substring(0, pos) + " - " + SkypeHelper.GetCallStatus(callProgress);
      }
      else
      {
        lbl = lbl.Substring(0, pos);
      }
      if (!lbl.Equals(item.Label))
      {
        item.Label = lbl;
      }
    }

    // Update the call status text in the list of calls
    public void UpdateCall(Call ChangedCall, TCallStatus callProgress)
    {
      if (callProgress == TCallStatus.clsUnplaced)
      {
        return;
      }
      GUIListItem item = null;
      string strThumb = string.Empty;
      int itemId = -1;

      foreach (DictionaryEntry entry in m_callsId)
      {
        if (((int)entry.Value) == ChangedCall.Id)
        {
          itemId = (int)entry.Key;
        }
      }

      //if (itemId == -1) return;
      if (itemId == -1)
      {
        if (callProgress == TCallStatus.clsCancelled ||
            callProgress == TCallStatus.clsFinished ||
            callProgress == TCallStatus.clsMissed ||
            callProgress == TCallStatus.clsRefused)
        {
          return;
        }
        AddCall(ChangedCall.PartnerDisplayName, ChangedCall.Id, ChangedCall.Type);
        return;
      }
      item = GUIControl.GetListItem(GetID, (int)Controls.CONTROL_LSTCALLS, itemId);

      if (item != null)
      {

        AddCallStatusToLabel(item);
        GUIControl.RefreshControl(GetID, (int)Controls.CONTROL_LSTCALLS);
      }
    }

    private GUIListItem MakeCallListItem(int callId)
    {
      string strThumb = string.Empty;

      GUIListItem item = new GUIListItem();
      item.ItemId = callId;

      SKYPE4COMLib.Call theCall = m_skypeAccess.get_Call(callId);

      m_callsId.Add(m_callsId.Count, callId);

      // create call list item
      switch (theCall.Type)
      {
        case TCallType.cltIncomingP2P:
          item.Label = "P2P with ";
          strThumb = SkypeHelper.GetCallTypeImage(theCall.Type);
          if (strThumb != string.Empty && System.IO.File.Exists(strThumb))
          {
            item.ThumbnailImage = strThumb;
            item.IconImage = strThumb;
            item.IconImageBig = strThumb;
          }
          break;

        case TCallType.cltIncomingPSTN:
          item.Label = "PSTN with ";
          strThumb = SkypeHelper.GetCallTypeImage(theCall.Type);
          if (strThumb != string.Empty && System.IO.File.Exists(strThumb))
          {
            item.ThumbnailImage = strThumb;
            item.IconImage = strThumb;
            item.IconImageBig = strThumb;
          }
          break;

        case TCallType.cltOutgoingP2P:
          item.Label = "P2P with ";
          strThumb = SkypeHelper.GetCallTypeImage(theCall.Type);
          if (strThumb != string.Empty && System.IO.File.Exists(strThumb))
          {
            item.ThumbnailImage = strThumb;
            item.IconImage = strThumb;
            item.IconImageBig = strThumb;
          }
          break;

        case TCallType.cltOutgoingPSTN:
          item.Label = "PSTN with ";
          strThumb = SkypeHelper.GetCallTypeImage(theCall.Type);
          if (strThumb != string.Empty && System.IO.File.Exists(strThumb))
          {
            item.ThumbnailImage = strThumb;
            item.IconImage = strThumb;
            item.IconImageBig = strThumb;
          }
          break;

        default:
          item.Label = "Unknown call.";
          break;
      }
      item.Label += theCall.PartnerDisplayName;

      return item;
    }

    public void RemoveCall(string PartnerDisplayName)
    {
      try
      {
        if (m_currentCalls.ContainsKey(PartnerDisplayName))
        {
          lstCalls.Clear();
          m_currentCalls.Remove(PartnerDisplayName);

          // reload calls
          m_callsId.Clear();
          foreach (int callId in m_currentCalls.Values)
          {
            GUIListItem item = MakeCallListItem(callId);
            AddCallStatusToLabel(item);
            lstCalls.Add(item);
          }
        }
        if (m_currentCalls.Count == 0)
        {
          lstCalls.Visible = false;
          lstCalls.Disabled = true;
        }
      }
      catch (Exception)
      {
        Log.Warn("Unable to remove a call from current calls list");
      }
    }

    public void MinimizeSkypeClient()
    {
      if (m_apiAttached)
      {
        m_skypeAccess.Client.Minimize();
      }
    }

    public bool ExistsInCalls(string PartnerDisplayName)
    {
      return m_currentCalls.ContainsKey(PartnerDisplayName);
    }
    
    public override bool Init()
    {
      return (Load(GUIGraphicsContext.Skin + @"\mySkype.xml"));
    }
    
    private void DisableAllControls()
    {
      btnStatus.Disabled = true;

      btnHistory.Disabled = true;

      btnSkypeOut.Disabled = true;
      btnSkypeOut.Selected = false;

      btnSilentMode.Disabled = true;
      btnSilentMode.Selected = false;

      btnSortOrder.Disabled = true;

    }

    private void UpdateUserDisplay(SKYPE4COMLib.User TheUser)
    {
      if (TheUser != null)
      {
        // Load infos from user
        GUIPropertyManager.SetProperty(TAG_CONTACT_NAME, SkypeHelper.GetFriendNameToUse(TheUser));
        GUIPropertyManager.SetProperty(TAG_CONTACT_STATUS, SkypeHelper.GetOnlineStatus(TheUser.OnlineStatus));
        string mdTxt = TheUser.MoodText;
        if (mdTxt.Equals(string.Empty))
        {
          mdTxt = TheUser.RichMoodText;
        }
        if (mdTxt.Equals(string.Empty))
        {
          mdTxt = TheUser.About;
        }
        GUIPropertyManager.SetProperty(TAG_CONTACT_MOODTEXT, mdTxt);
      }
      else
      {
        GUIPropertyManager.SetProperty(TAG_CONTACT_NAME, SkypeHelper.EMPTY_STRING);
        GUIPropertyManager.SetProperty(TAG_CONTACT_STATUS, SkypeHelper.EMPTY_STRING);
        GUIPropertyManager.SetProperty(TAG_CONTACT_MOODTEXT, SkypeHelper.EMPTY_STRING);
      }
    }

    private void AcceptNewCall(Call ChangedCall)
    {
      // pause current call if exists
      if (m_observer.CurrentCall != null && m_observer.CurrentCall.Id != ChangedCall.Id)
      {
        m_observer.CurrentCall.Status = TCallStatus.clsOnHold;
      }

      // accept the call 
      AddCall(ChangedCall.PartnerDisplayName, ChangedCall.Id, ChangedCall.Type);

      m_observer.CurrentUser = m_skypeAccess.get_User(ChangedCall.PartnerHandle);
      m_observer.CurrentCall = ChangedCall;

    }
    private void StartCall(string number)
    {
      if (number != "")
      {
        Log.Debug("SP->Calling " + (number.StartsWith("+") ? "" : "+") + number);
      }
      try
      {
        if (m_observer.CurrentUser != null || number != "")
        {
          SKYPE4COMLib.Call theCall = null;
          if (number == "")
          {
            theCall = m_skypeAccess.PlaceCall(m_observer.CurrentUser.Handle, string.Empty, string.Empty, string.Empty);
          }
          else
          {
            theCall = m_skypeAccess.PlaceCall((number.StartsWith("+") ? "" : "+") + number, string.Empty, string.Empty, string.Empty);
          }
          
          Log.Debug("SP->Call placed " + (theCall == null ? " (null)" : theCall.Status.ToString()));
          if (theCall != null)
          {
            // Very rarely gets the call immediately - handled by status messages later on
            m_observer.CurrentCall = theCall;
            if (theCall.Status != TCallStatus.clsUnplaced)
            {
              Log.Debug("SP->Adding call to list");
              if (number == "")
              {
                AddCall(SkypeHelper.GetFriendNameToUse(m_observer.CurrentUser), theCall.Id, theCall.Type);
              }
              else
              {
                AddCall((number.StartsWith("+") ? "" : "+") + number, theCall.Id, theCall.Type);
              }
              Log.Debug("SP->UpdateCallDisplay");
              UpdateCallDisplay(theCall.Status);
            }
          }
        }
        Log.Debug("SP->MinimizeSkypeClient");
        MinimizeSkypeClient();
      }
      catch (Exception callEx)
      {
        if (callEx.Message.Contains("CALL: Unrecognised identity"))
        {
          GUIPropertyManager.SetProperty(TAG_CALL_DURATION, "Failed: Invalid number");
        }
        else
        {
          GUIPropertyManager.SetProperty(TAG_CALL_DURATION, "Failed: Unknown error");
          Log.Warn("Unable to start a call : '{0}' \n {1}", callEx.Message, callEx.StackTrace);
        }
      }
    }

    private void HoldCall(bool HoldState)
    {
      try
      {
        if (m_observer.CurrentCall != null)
        {
          if (!HoldState)
          {
            m_observer.CurrentCall.Status = TCallStatus.clsOnHold;
            UpdateCallDisplay(TCallStatus.clsOnHold);
          }
          else
          {
            m_observer.CurrentCall.Status = TCallStatus.clsInProgress;
            UpdateCallDisplay(TCallStatus.clsInProgress);
          }
        }
      }
      catch (Exception callEx)
      {
        Log.Warn("Unable to hold a call : {0} \n {1}", callEx.Message, callEx.StackTrace);
      }
    }
    private void StopCall()
    {
      try
      {
        if (m_observer.CurrentCall != null)
        {
          try
          {
            m_observer.CurrentCall.Status = TCallStatus.clsFinished;
            // The rest is handled when the Call Status message arrives...
          }
          catch
          {
            //nothing to be done exception is caught
          }
        }
      }
      catch (Exception callEx)
      {
        Log.Warn("Unable to stop a call : {0} \n {1}", callEx.Message, callEx.StackTrace);
      }
    }

    // Change the user's online status - presents the options as a dialog...
    // Options are
    //cusOffline = 0,
    //cusOnline = 1,
    //cusAway = 2,
    //cusNotAvailable = 3,
    //cusDoNotDisturb = 4,
    //cusInvisible = 5,
    //cusLoggedOut = 6,
    //cusSkypeMe = 7,

    private void GetNewStatus()
    {
      GUIDialogMenu dlg = null;

      dlg = (GUIDialogMenu)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_MENU);
      if (dlg == null) return;

      dlg.Reset();
      dlg.SetHeading("Call actions");

      TUserStatus tus;
      for (tus = TUserStatus.cusOffline; tus <= TUserStatus.cusSkypeMe; tus++)
      {
        dlg.Add(m_skypeAccess.Convert.UserStatusToText(tus)); // Finish
      }

      dlg.DoModal(GetID);

      if (dlg.SelectedId < 0)
      {
        return;
      }
      tus = (TUserStatus)dlg.SelectedId - 1;
      if (m_skypeAccess.CurrentUserStatus != tus && tus != TUserStatus.cusUnknown)
      {
        m_skypeAccess.CurrentUserStatus = tus;
      }

    }

    private void ViewHistory()
    {
      GUIWindowManager.ActivateWindow((int)SkypeHelper.HISTORY_WINDOW_ID);
    }

    private void ViewSettings()
    {
      GUIWindowManager.ActivateWindow((int)SkypeHelper.SETTINGS_WINDOW_ID);
    }

    private void ShowChatWindow(User user)
    {
      if (g_Player.Playing)
      {
        if (g_Player.FullScreen)
        {
          g_Player.FullScreen = false;
        }
        //if (!g_Player.Paused)
        //{
        //    if (g_Player.IsVideo || g_Player.IsDVD) g_Player.Pause();
        //}
      }

      GUIWindowManager.ActivateWindow((int)SkypeHelper.CHAT_WINDOW_ID);
      if (user != null)
      {
        GUIWindow w = GUIWindowManager.GetWindow(SkypeHelper.CHAT_WINDOW_ID);
        ((GUISkypeChat)w).ChatWith(user);
      }
    }
    private void SendSMS()
    {
      GUIWindowManager.ActivateWindow((int)SkypeHelper.SMS_WINDOW_ID);
    }
    private void ToggleSkypeOut()
    {
      LoadSkypeFriends();
      //KillQualityFeedbackWindow();
    }
    private void SetSilentMode()
    {
      //m_skypeAccess.SilentMode = btnSilentMode.Selected;
      Command cmd = new Command();
      string s = "SILENT_MODE " + (btnSilentMode.Selected?"ON":"OFF");
      cmd.Command = "SET " + s;
      cmd.Blocking = true;
      cmd.Expected = s;
      m_skypeAccess.SendCommand(cmd);
    }
    private void ToggleSortOrder()
    {
      if (++currentSortMethod > SortMethod.SORT_STATUS)
      {
        currentSortMethod = SortMethod.SORT_NAME;
      }
      btnSortOrder.Label = SortMethodName[(int)currentSortMethod];
      LoadSkypeFriends();
    }
    private void ToggleVideo()
    {
      if (m_observer.CurrentCall == null) return;
      TCallVideoSendStatus vidStatus = m_observer.CurrentCall.VideoSendStatus;
      try
      {
        if (vidStatus == TCallVideoSendStatus.vssAvailable)
        {
          m_observer.CurrentCall.StartVideoSend();
        }
        else if (vidStatus == TCallVideoSendStatus.vssRunning ||
            vidStatus == TCallVideoSendStatus.vssPaused ||
            vidStatus == TCallVideoSendStatus.vssStarting)
        {
          m_observer.CurrentCall.StopVideoSend();
        }
      }
      catch (Exception e)
      {
        Log.Debug("SP->ToggleVideo failed: {0}", e.Message);
      }
    }

    // Show the options available for a selected Call.
    // For Internationalisation to work, this will need to be changed so that the
    // controls added to the list are controls whose Id is constant but whose text 
    // changes depending on the language selected.
    private void ShowContextMenuCall()
    {
      GUIListItem item = null;
      GUIDialogMenu dlg = null;
      int callId = -1;
      Call call = null;

      item = GUIControl.GetSelectedListItem(GetID, (int)Controls.CONTROL_LSTCALLS);
      if (item == null) return;

      dlg = (GUIDialogMenu)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_MENU);
      if (dlg == null) return;

      callId = item.ItemId;

      call = m_skypeAccess.get_Call(callId);

      dlg.Reset();
      dlg.SetHeading("Call actions");

      dlg.Add("Stop call"); // Finish

      if (call.Status == TCallStatus.clsOnHold || call.Status == TCallStatus.clsLocalHold || call.Status == TCallStatus.clsRemoteHold)
      {
        dlg.Add("Resume call");
      }
      else
      {
        dlg.Add("Hold call"); // Hold
      }
      if (call != null && m_observer.CurrentCall != null && call.Id == m_observer.CurrentCall.Id)
      {
        Log.Debug("SP->call==currentCall {0}", call.VideoSendStatus);
        // Can only toggle video for the current call
        if (call.VideoSendStatus == TCallVideoSendStatus.vssRunning ||
            call.VideoSendStatus == TCallVideoSendStatus.vssStarting ||
            call.VideoSendStatus == TCallVideoSendStatus.vssPaused)
        {
          dlg.Add("Stop video");
        }
        else if (
            call.VideoSendStatus != TCallVideoSendStatus.vssNotAvailable &&
            call.VideoSendStatus != TCallVideoSendStatus.vssRejected &&
            call.VideoSendStatus != TCallVideoSendStatus.vssStopping)
        {
          dlg.Add("Start video");
        }
      }
      else
      {
        Log.Debug("SP->call!=currentCall");
      }
      dlg.DoModal(GetID);

      switch (dlg.SelectedLabel)
      {
        case 0: // Finish
          try
          {
            call.Status = TCallStatus.clsFinished;
          }
          catch (Exception)
          { }
          break;

        case 1: // Stop Hold
          try
          {
            if (call.Status == TCallStatus.clsOnHold || call.Status == TCallStatus.clsLocalHold || call.Status == TCallStatus.clsRemoteHold)
            {
              // If the selected call is on hold, we need to restart it,
              // which means Holding the current call if any, and not already on hold
              if (m_observer.CurrentCall != null && call != m_observer.CurrentCall)
              {
                if (m_observer.CurrentCall.Status != TCallStatus.clsOnHold &&
                    m_observer.CurrentCall.Status != TCallStatus.clsLocalHold &&
                    m_observer.CurrentCall.Status != TCallStatus.clsRemoteHold)
                {
                  m_observer.CurrentCall.Status = TCallStatus.clsOnHold;
                }
              }
              call.Status = TCallStatus.clsInProgress;
              m_observer.CurrentCall = call;
              m_observer.CurrentUser = m_skypeAccess.get_User(call.PartnerHandle);
            }
            else
            {
              call.Status = TCallStatus.clsOnHold;
            }
          }
          catch (Exception)
          {
            Log.Warn("SP->Cannot hold/resume this call at the moment");
          }
          break;

        case 2: // Toggle video
          ToggleVideo();
          break;

        default:
          break;
      }
    }

    // Show the options available for a selected Contact.
    // For Internationalisation to work, this will need to be changed so that the
    // controls added to the list are controls whose Id is constant but whose text 
    // changes depending on the language selected.
    private void ShowContextMenuContact()
    {

      GUIListItem item = GUIControl.GetSelectedListItem(GetID, (int)Controls.CONTROL_LSTCONTACTS);
      if (item == null) return;

      GUIDialogMenu dlg = (GUIDialogMenu)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_MENU);
      if (dlg == null) return;

      User user = (User)item.AlbumInfoTag;
      if (user == null)
      {
        Log.Debug("SP->No Contact selected");
        return;
      }

      dlg.Reset();
      //dlg.SetHeading("Contact actions: " + SkypeHelper.GetFriendNameToUse(user));
      dlg.SetHeading(SkypeHelper.GetFriendNameToUse(user));

      // Can only place another call if there is not active call, or
      // it's on hold, and only if it's to a different user.
      if (m_HandleVoiceCalls)
      {
        if (m_observer.CurrentCall == null ||
            ((m_observer.CurrentCall.Status == TCallStatus.clsLocalHold ||
            m_observer.CurrentCall.Status == TCallStatus.clsOnHold ||
            m_observer.CurrentCall.Status == TCallStatus.clsRemoteHold) &&
            m_observer.CurrentCall.PartnerHandle != user.Handle
            ))
        {
          if (!user.IsSkypeOutContact || (m_AllowSkypeOutCalls && m_skypeAccess.CurrentUserProfile.Balance > 0))
          {
            dlg.Add("Call");
          }
        }

      }

      if ((!user.PhoneMobile.Equals(string.Empty) || (user.IsSkypeOutContact) && m_AllowSMS && m_skypeAccess.CurrentUserProfile.Balance > 0))
      {
          StringBuilder s = new StringBuilder("Send SMS (");
          if (!user.PhoneMobile.Equals(string.Empty))
          {
            s.Append(user.PhoneMobile + ")");
          }
          else
          {
            s.Append(user.Handle + ")");
          }
          dlg.Add(s.ToString());
      }

      if (user.IsSkypeOutContact)
      {
        if (m_AllowSkypeOutCalls && m_HandleVoiceCalls && m_skypeAccess.CurrentUserProfile.Balance > 0)
        {
          dlg.Add("Call number");
        }
      }
      else
      {
        if (m_HandleChats)
        {
          dlg.Add("Chat");
        }
        dlg.Add("Contact details");
      }

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
        case "Call": // Call
          // Again check that no call is active (it might have started while we were
          // waiting for the dialog to end?) before trying to start a new one.
          SelectAnotherUser(user);
          StartCall("");
          break;

        case "Call number":
          ShowSkypeOutWindow();
          break;

        case "Contact details":
          ShowContactWindow(user);
          break;

        case "Chat":
          ShowChatWindow(user);
          break;

        default:
          // These labels can have variable text after the start, so
          // we need to check only the initial text to see which button 
          // was pressed.
          // Note: Won't work for internationalisation!!!
          if (dlg.SelectedLabelText.StartsWith("Send SMS"))
          {
            SendSMS();
          }
          else
          {
            Log.Warn("SP->Unknown label selected '{0}'", dlg.SelectedLabelText);
          }
          break;
      }
    }

    private void ShowContextMenu()
    {
      if (lstContacts.IsFocused)
      {
        ShowContextMenuContact();
      }
      else if (lstCalls.IsFocused)
      {
        ShowContextMenuCall();
      }
      else
      {
        Log.Warn("SP->ShowContextMenu: No list focused");
      }
    }

    private void ShowUserAvatar(User user)
    {
      if (GUIGraphicsContext.IsPlayingVideo || user == null)
      {
        if (imgAvatar != null)
        {
          imgAvatar.Visible = false;
        }
      }
      else
      {
        if (imgAvatar != null)
        {
          imgAvatar.Visible = true;
        }
        String strAvatar = GUIGraphicsContext.Skin + "\\..\\..\\plugins\\windows\\SkypeAvatars\\" + user.Handle + ".jpg";
        if (!File.Exists(strAvatar))
        {
          // Get the avatar
          Command cmd = new Command();
          string s = "USER " + user.Handle + " AVATAR 1 " + strAvatar;
          cmd.Command = "GET " + s;
          cmd.Blocking = true;
          cmd.Expected = s;
          m_skypeAccess.SendCommand(cmd);
        }
        GUIPropertyManager.SetProperty(TAG_IMG_AVATAR, strAvatar);
      }
    }

    private void SelectAnotherUser(User user)
    {
      // do not change current user if a call is in progress
      if (m_observer.CurrentCall != null &&
          (m_observer.CurrentCall.Status == TCallStatus.clsInProgress ||
          m_observer.CurrentCall.Status == TCallStatus.clsRinging ||
          m_observer.CurrentCall.Status == TCallStatus.clsRouting))
      {
        return;
      }

      m_observer.CurrentUser = user;

      ShowUserAvatar(user);
    }

    /// <summary>
    /// Allow the user to type a number and call it using SkypeOut
    /// </summary>
    private void ShowSkypeOutWindow()
    {
      if (m_skypeAccess.CurrentUserProfile.Balance <= 0)
      {
        return;
      }

      // F3 on contact item
      GUISkypeOut skypeOut = (GUISkypeOut)GUIWindowManager.GetWindow((int)SkypeHelper.SKYPEOUT_WINDOW_ID);
      if (skypeOut == null) return;

      // show the contact window ! Modal
      skypeOut.DoModal(GetID);

      Log.Debug("SP->" + (skypeOut.Cancelled ? "Not c" : "C") + "alling " + skypeOut.Number + " via SkypeOut");
      if (!skypeOut.Cancelled)
      {
        StartCall(skypeOut.Number);
      }
    }

    private void ShowContactWindow(SKYPE4COMLib.User Friend)
    {
      // do not show info if no contact selected
      if (Friend == null) return;

      // F3 on contact item
      GUISkypeContact skypeContactGUI = (GUISkypeContact)GUIWindowManager.GetWindow((int)GUISkypeContact.WINDOW_ID);
      if (skypeContactGUI == null) return;

      // show the contact window ! Modal
      skypeContactGUI.DoModal(GetID, Friend);
    }
    public void UpdateCallDisplay(TCallStatus CallProgress)
    {
      GUIPropertyManager.SetProperty(TAG_CALL_STATUS, SkypeHelper.GetCallStatus(CallProgress));
      if (CallProgress == TCallStatus.clsFailed)
      {
        // Try to give more details about the failure
        GUIPropertyManager.SetProperty(TAG_CALL_DURATION, m_skypeAccess.Convert.CallFailureReasonToText(m_observer.CurrentCall.FailureReason));

        Log.Info("SP->Skype Call Failure text '{0}'",
            m_skypeAccess.Convert.CallFailureReasonToText(m_observer.CurrentCall.FailureReason));
      }
      else if (CallProgress != TCallStatus.clsFinished &&
               CallProgress != TCallStatus.clsInProgress &&
               CallProgress != TCallStatus.clsLocalHold &&
               CallProgress != TCallStatus.clsRemoteHold &&
               CallProgress != TCallStatus.clsOnHold)
      {
        // For anything other than finished etc, we leave the duration visible
        GUIPropertyManager.SetProperty(TAG_CALL_DURATION, "");
      }
    }

    public void UpdateCallDuration(int callId, int dur)
    {
      if (m_observer.CurrentCall != null &&
          m_observer.CurrentCall.Id == callId)
      {
        // update the call duration
        int hrs = dur / 3600;
        int mins = (dur - hrs * 3600) / 60;
        int secs = dur % 60;

        String drn = "";
        if (hrs > 0)
        {
          drn = hrs.ToString("##0") + ":";
        }
        drn += mins.ToString("00") + ":" + secs.ToString("00");
        GUIPropertyManager.SetProperty(TAG_CALL_DURATION, drn);
      }
      // Try to keep the main Window from appearing when it shouldn't.
      // TODO: THis should probably be an option, to avoid problems for other people...
      if (m_observer.CurrentCall == null ||
          (m_observer.CurrentCall.VideoReceiveStatus != TCallVideoSendStatus.vssRunning &&
           m_observer.CurrentCall.VideoReceiveStatus != TCallVideoSendStatus.vssStarting))
      {
        MinimizeSkypeClient();
      }
    }

    private void ReloadCallList()
    {
      if (m_apiAttached)
      {
        CallCollection calls = m_skypeAccess.ActiveCalls;
        foreach (Call c in calls)
        {
          AddCall(c.PartnerDisplayName, c.Id, c.Type);
          if (m_observer.CurrentCall == null ||   // First one always gets in
              c.Status == TCallStatus.clsInProgress || // Otherwise it has to be one of these
              c.Status == TCallStatus.clsEarlyMedia ||
              c.Status == TCallStatus.clsRouting ||
              c.Status == TCallStatus.clsRinging)
          {
            m_observer.CurrentCall = c;
            m_observer.CurrentUser = m_skypeAccess.get_User(c.PartnerHandle);
          }
        }
      }
    }

    private void ResetEntireWindow()
    {

      GUIPropertyManager.SetProperty(TAG_CONTACT_NAME, SkypeHelper.EMPTY_STRING);
      GUIPropertyManager.SetProperty(TAG_CONTACT_STATUS, SkypeHelper.EMPTY_STRING);
      GUIPropertyManager.SetProperty(TAG_CONTACT_COUNT, SkypeHelper.EMPTY_STRING);
      GUIPropertyManager.SetProperty(TAG_CALL_STATUS, SkypeHelper.EMPTY_STRING);
      GUIPropertyManager.SetProperty(TAG_CALL_DURATION, SkypeHelper.EMPTY_STRING);
      GUIPropertyManager.SetProperty(TAG_SKYPE_CREDIT, SkypeHelper.EMPTY_STRING);
      GUIPropertyManager.SetProperty(TAG_CONTACT_MOODTEXT, SkypeHelper.EMPTY_STRING);

      if (lstCalls != null)
      {
        lstCalls.Clear();
        lstCalls.Disabled = true;
        lstCalls.Visible = false;
      }

      if (m_currentCalls != null)
      {
        m_currentCalls.Clear();
      }
      m_callsId.Clear();
      m_observer.CurrentCall = null;

      ShowUserAvatar(null);

      // init
      try
      {
        LoadSkypeFriends();
        UpdateMissedCalls();
        if (m_apiAttached)
        {
          btnHistory.Disabled = false;

          btnSkypeOut.Disabled = false;
          //btnSkypeOut.Selected = false;
          
          btnSilentMode.Disabled = false;
          btnSilentMode.Selected = m_skypeAccess.SilentMode;

          btnStartSkype.Disabled = true;
          btnChats.Disabled = !m_HandleChats;

          btnAudioDevices.Disabled = false;
          
          OnUserStatusChanged(m_skypeAccess.CurrentUserStatus);
          btnStatus.Disabled = false;
        }
        else
        {
          btnStatus.Disabled = true;
          btnHistory.Disabled = true;

          btnSkypeOut.Disabled = true;
          btnSkypeOut.Selected = false;

          btnSilentMode.Disabled = true;
          btnSilentMode.Selected = false;

          btnStartSkype.Disabled = false;
          btnChats.Disabled = true;

          btnAudioDevices.Disabled = true;

          OnUserStatusChanged(TUserStatus.cusOffline);
          ShowSkypeCredits();
        }

        ReloadCallList();

        // Update the call status if there's a current call
        if (m_observer.CurrentCall != null)
        {
          UpdateCallDisplay(m_observer.CurrentCall.Status);
        }
      }
      catch (Exception e)
      {
        Log.Warn("Unable to initalise skype plugin. Exception :{0}", e.Message);
        DisableAllControls();
      }
    }

    public void UpdateMissedCalls()
    {
      string strMissedCalls = string.Empty;
      string strMissedCalls2 = string.Empty;
      CallCollection missedCalls = null;
      Hashtable oldCalls = new Hashtable();
      int index = 0;

      try
      {
        GUIControl.ClearControl(GetID, (int)Controls.CONTROL_MISSEDCALLS);

        if (m_apiAttached)
        {
          missedCalls = m_skypeAccess.MissedCalls;
        }
        if (missedCalls != null && missedCalls.Count > 0)
        {
          strMissedCalls = missedCalls.Count.ToString();
          strMissedCalls += " missed call" + ((missedCalls.Count == 1) ? "" : "s") + ":";

          for (index = 1; index <= missedCalls.Count; index++)
          {
            Call call = missedCalls[index];

            if (!oldCalls.ContainsKey(call.PartnerDisplayName))
            {
              oldCalls.Add(call.PartnerDisplayName, 1);
            }
            else
            {
              oldCalls[call.PartnerDisplayName] = ((int)oldCalls[call.PartnerDisplayName]) + 1;
            }
          }

          foreach (DictionaryEntry entry in oldCalls)
          {
            strMissedCalls2 += entry.Key.ToString();

            if (((int)entry.Value) > 1)
            {
              strMissedCalls2 += "(" + entry.Value.ToString() + ")";
            }

            strMissedCalls2 += ", ";
          }
          strMissedCalls2 = strMissedCalls2.Substring(0, strMissedCalls2.Length - 2);
        }
        else
        {
          strMissedCalls = SkypeHelper.EMPTY_STRING;
          strMissedCalls2 = SkypeHelper.EMPTY_STRING;
        }
      }
      catch (Exception e)
      {
        Log.Warn(e.Message);
        strMissedCalls = SkypeHelper.EMPTY_STRING;
        strMissedCalls2 = SkypeHelper.EMPTY_STRING;
      }
      finally
      {
        GUIPropertyManager.SetProperty(TAG_MISSED_CALLS, strMissedCalls);
        GUIPropertyManager.SetProperty(TAG_MISSED_CALLS2, strMissedCalls2);
      }
    }
    #endregion

    #region BaseWindow Members
    public override void OnAction(Action action)
    {
      switch (action.wID)
      {
        case Action.ActionType.ACTION_MOVE_RIGHT:
          Log.Debug("SP->Action:MoveRight ({0})", this.GetFocusControlId());
          if (GetFocusControlId() == (int)Controls.CONTROL_LSTCONTACTS)
          {
            Log.Debug("SP->Focusable {0} IsEnabled {1} IsVisible {2} OnRight={3}",
                lstCalls.Focusable,
                lstCalls.IsEnabled,
                lstCalls.IsVisible,
                lstContacts.NavigateRight);
            Log.Debug("SP->Status button Focusable {0} IsEnabled {1} IsVisible {2}",
                btnStatus.Focusable,
                btnStatus.IsEnabled,
                btnStatus.IsVisible);

          }
          break;

        case Action.ActionType.ACTION_PREVIOUS_MENU:
          GUIWindowManager.ShowPreviousWindow();
          break;

        case Action.ActionType.ACTION_KEY_PRESSED:
          Log.Debug("SP->Action: {0}", action.wID);
          /* TODO
          //space bar
          if(action.m_key.KeyChar == 32)
          {	
              //enable snooze timer
              _SnoozeCount = 0;
              _SnoozeTimer.Enabled = true;
          }
          */
          break;

        case Action.ActionType.ACTION_CONTEXT_MENU:
          ShowContextMenu();
          break;
      }

      base.OnAction(action);
    }

    public override bool OnMessage(GUIMessage message)
    {

      switch (message.Message)
      {
        case GUIMessage.MessageType.GUI_MSG_WINDOW_INIT:
          base.OnMessage(message);
          ResetEntireWindow();
          return true;

        case GUIMessage.MessageType.GUI_MSG_WINDOW_DEINIT:
          break;

        case GUIMessage.MessageType.GUI_MSG_SETFOCUS:
          break;

        case GUIMessage.MessageType.GUI_MSG_ITEM_SELECTED:
          GUIControl gc = GetControl(message.TargetControlId);
          if (gc != null)
          {
            if (message.TargetControlId == (int)Controls.CONTROL_LSTCONTACTS)
            {
            }

            if (message.TargetControlId == (int)Controls.CONTROL_LSTCALLS)
            {
              GUIListItem item = GUIControl.GetSelectedListItem(GetID, (int)Controls.CONTROL_LSTCALLS);
              if (item == null) break;

              Call call = null;
              call = m_skypeAccess.get_Call(item.ItemId);

              User u = m_skypeAccess.get_User(call.PartnerHandle);
              if (u != null)
              {
                UpdateUserDisplay(u);
              }

              UpdateCallDisplay(call.Status);

              m_observer.CurrentCall = call;

            }
          }
          break;

        case GUIMessage.MessageType.GUI_MSG_CLICKED:
          int iControl = message.SenderControlId;
          if (iControl == (int)Controls.CONTROL_BTNSTATUS)
          {
            GetNewStatus();
            break;
          }

          if (iControl == (int)Controls.CONTROL_START_SKYPE)
          {
            if (!m_skypeAccess.Client.IsRunning)
            {
              m_skypeAccess.Client.Start(true, true);
            }
            break;
          }

          if (iControl == (int)Controls.CONTROL_BTNCHATS)
          {
            ShowChatWindow(null);
            break;
          }

          if (iControl == (int)Controls.CONTROL_BTNHISTORY)
          {
            ViewHistory();
            break;
          }

          if (iControl == btnSettings.GetID)
          {
            ViewSettings();
            break;
          }

#if SHOW_AUDIO_DEVICES
          if (iControl == btnAudioDevices.GetID)
          {
            ViewAudioDevices();
            break;
          }
#endif

          if (iControl == (int)Controls.CONTROL_BTNSKYPEOUT)
          {
            ToggleSkypeOut();
            break;
          }

          if (iControl == (int)Controls.CONTROL_BTNSILENTMODE)
          {
            SetSilentMode();
            break;
          }

          if (iControl == (int)Controls.CONTROL_BTNSORTORDER)
          {
            ToggleSortOrder();
            break;
          }

          if (iControl == (int)Controls.CONTROL_LSTCONTACTS)
          {
            GUIMessage msg = new GUIMessage(GUIMessage.MessageType.GUI_MSG_ITEM_SELECTED, GetID, 0, iControl, 0, 0, null);
            OnMessage(msg);

            if (message.Param1 == (int)Action.ActionType.ACTION_SELECT_ITEM)
            {
              User user = null;

              GUIListItem selectedItem = GUIControl.GetSelectedListItem(GetID, (int)Controls.CONTROL_LSTCONTACTS);
              if (selectedItem == null)
              {
                break;
              }
              user = (User)selectedItem.AlbumInfoTag;

              SelectAnotherUser(user);

            }

            if (message.Param1 == (int)Action.ActionType.ACTION_SHOW_INFO)
            {
              GUIMessage tmpMsg = new GUIMessage(GUIMessage.MessageType.GUI_MSG_ITEM_SELECTED, GetID, 0, (int)Controls.CONTROL_LSTCONTACTS, 0, 0, null);
              OnMessage(tmpMsg);
              GUIListItem selectedItem = GUIControl.GetListItem(GetID, (int)Controls.CONTROL_LSTCONTACTS, (int)tmpMsg.Param1);
              if (selectedItem != null)
              {
                User user = (User)selectedItem.AlbumInfoTag;
                ShowContactWindow(user);
              }
            }
          }

          if (iControl == (int)Controls.CONTROL_LSTCALLS)
          {
            GUIMessage msg = new GUIMessage(GUIMessage.MessageType.GUI_MSG_ITEM_SELECTED, GetID, 0, iControl, 0, 0, null);
            OnMessage(msg);

            if (message.Param1 == (int)Action.ActionType.ACTION_SELECT_ITEM)
            {
              int callId = -1;

              if (m_observer.CurrentCall != null)
              {
                // unable to change current call, skip
                if (m_observer.CurrentCall.Status == TCallStatus.clsInProgress ||
                  m_observer.CurrentCall.Status == TCallStatus.clsRinging ||
                  m_observer.CurrentCall.Status == TCallStatus.clsRouting)
                {
                  break;
                }
              }

              GUIListItem selectedItem = GUIControl.GetSelectedListItem(GetID, (int)Controls.CONTROL_LSTCALLS);
              if (selectedItem != null)
              {
                callId = selectedItem.ItemId;
                if (callId == -1) break;

                if (m_observer.CurrentCall != null && callId != m_observer.CurrentCall.Id)
                {
                  m_observer.CurrentCall = m_skypeAccess.get_Call(callId);
                  UpdateUserDisplay(m_observer.CurrentUser);
                  UpdateCallDisplay(m_observer.CurrentCall.Status);
                }
              }
            }
          }
          break;
      }
      return base.OnMessage(message);
    }

#if SHOW_AUDIO_DEVICES
    private void ViewAudioDevices()
    {
      GUIWindowManager.ActivateWindow((int)SkypeHelper.AUDIO_DEVICES_WINDOW_ID);
    }
#endif

    #endregion

    #region IComparer<GUIListItem> Members

    public int Compare(GUIListItem item1, GUIListItem item2)
    {
      int[] StatusOrder = {
                 7,       //olsUnknown = -1,
                 5,       //olsOffline = 0,
                 0,       //olsOnline = 1,
                 4,       //olsAway = 2,
                 2,       //olsNotAvailable = 3,
                 3,       //olsDoNotDisturb = 4,
                 6,       //olsSkypeOut = 5,
                 1       //olsSkypeMe = 6,
            };
      if (item1 == item2) return 0;
      if (item1 == null) return -1;
      if (item2 == null) return -1;

      User contact1 = item1.AlbumInfoTag as User;
      User contact2 = item2.AlbumInfoTag as User;

      int cSts1 = StatusOrder[(int)contact1.OnlineStatus + 1];
      int cSts2 = StatusOrder[(int)contact2.OnlineStatus + 1];

      if (currentSortMethod == SortMethod.SORT_STATUS)
      {
        if (cSts1 < cSts2)
        {
          return -1;
        }
        else if (cSts2 < cSts1)
        {
          return 1;
        }
      }
      return String.Compare(item1.Label, item2.Label, true);
    }
    #endregion

    #region Persistance
    private bool LoadSettings()
    {
      // create the observer
      if (m_observer == null)
      {
        m_observer = new Observer(this);
      }

      if (options.Count == 0)
      {
        options.LoadList();
      }
      m_startSkype = options["StartWithMP"];
      m_stopSkype = options["StopWithMP"];
      m_HandleChats = options["HandleChats"];
      m_HandleVoiceCalls = options["HandleVoiceCalls"];
      m_AllowSkypeOutCalls = options["AllowSkypeOutCalls"];
      m_AllowSMS = options["AllowSMS"];
      m_PauseForCalls = options["PauseForIncomingCalls"];
      m_PauseForChats = options["PauseForIncomingChats"];
      m_UseSilentMode = options["UseSilentMode"];
      m_IgnoreIncomingCalls = options["IgnoreIncomingCalls"];
      m_HangupIfDND = options["HangupIfDND"];

      return true;
    }

    public void ReloadSettings()
    {
      options.Clear();
      LoadSettings();
    }
    #endregion

    void KillQualityFeedbackWindow()
    {
      IntPtr ptr = FindWindow(null, "Skype™ - Call Quality Feedback");
      //IntPtr ptr = FindWindow(null, "Skype™ - merton_a");
      if ((int)ptr != 0)
      {
        SendMessage(ptr, WM_CLOSE, 0, 0);
        Log.Debug("SP->Found and closed Skype QFB window: {0}", ptr);
      }
      else
      {
        Log.Debug("SP->Couldn't find Skype QFB window");
      }
    }
  }
}
