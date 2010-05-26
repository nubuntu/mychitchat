using System;
using System.Collections;
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


namespace maniac.MediaPortal.Skype
{
  /// <summary>
  /// Description résumée de GUISkype.
  /// </summary>
  public class GUISkypeHistory : GUIWindow
  {
    #region CONST
    private const string EMPTY_STRING = " ";
    private const string TAG_LISTNAME = "#Skype.History.ListName";
    private const string MISSED_CALLS = "Missed";
    private const string INCOMING_CALLS = "Incoming";
    private const string OUTGOING_CALLS = "Outgoing";

    private enum Controls
    {
      CONTROL_BTNHISTORYVIEW = 3,
      CONTROL_BTNPURGE = 2,
      CONTROL_LSTMISSEDCALLS = 12
    };

    [SkinControlAttribute(2)]
    protected GUIButtonControl btnPurge = null;
    [SkinControlAttribute(12)]
    protected GUIListControl lstCalls = null;

    #endregion

    #region Attributes
    private static SKYPE4COMLib.CallCollection m_callsHistory = null;
    private static SKYPE4COMLib.CallCollection m_missedCalls = null;
    #endregion

    #region Properties
    #endregion

    #region Constructors
    public GUISkypeHistory()
    {
      try
      {
        GetID = SkypeHelper.HISTORY_WINDOW_ID;
      }
      catch (Exception e)
      {
        Log.Warn("Unable to initialise GUISkypeHistory");
        Log.Warn(e.Message);
        Log.Warn(e.StackTrace);
      }
      return;
    }

    #endregion

    #region Methods
    private void LoadMissedCalls()
    {
      int index = 0;
      string strThumb = string.Empty;

      try
      {
        GUIControl.ClearControl(GetID, (int)Controls.CONTROL_LSTMISSEDCALLS);

        m_missedCalls = GUISkype.SkypeAccess.MissedCalls;

        if (m_missedCalls != null && m_missedCalls.Count > 0)
        {
          strThumb = GUIGraphicsContext.Skin + SkypeHelper.SKIN_MEDIA_DIR + SkypeHelper.IMG_MISSED_CALL;
          for (index = 1; index <= m_missedCalls.Count; index++)
          {
            Call call = m_missedCalls[index];
            if (call == null) continue;

            StringBuilder sb = new StringBuilder();
            GUIListItem item = new GUIListItem();

            sb.Append(call.PartnerDisplayName);
            sb.Append(" - ");
            sb.Append(call.Timestamp);
            item.Label = sb.ToString();
            item.ItemId = call.Id;

            if (System.IO.File.Exists(strThumb))
            {
              item.ThumbnailImage = strThumb;
              item.IconImage = strThumb;
              item.IconImageBig = strThumb;
            }
            GUIControl.AddListItemControl(GetID, (int)Controls.CONTROL_LSTMISSEDCALLS, item);
          }

          btnPurge.Disabled = false;
        }
        else
        {
          btnPurge.Disabled = true;
        }

        // Purging doesn't work - wait for Skype to fix it...
        // Might be working in 3.2 (Beta), so enable it
        if (GUISkype.SkypeAccess.Version.CompareTo("3.2.0.0") < 0)
        {
          btnPurge.Disabled = true;
        }

        GUIPropertyManager.SetProperty(TAG_LISTNAME, MISSED_CALLS + " calls");
      }
      catch (Exception e)
      {
        Log.Warn("SP->Failed to load missed calls list : {0}\n{1}", e.Message, e.StackTrace);
      }
    }
    private void LoadOutgoingCalls()
    {
      int index = 0;
      string strThumb = string.Empty;

      try
      {
        GUIControl.ClearControl(GetID, (int)Controls.CONTROL_LSTMISSEDCALLS);

        m_callsHistory = GUISkype.SkypeAccess.get_Calls("");

        if (m_callsHistory != null && m_callsHistory.Count > 0)
        {
          strThumb = GUIGraphicsContext.Skin + SkypeHelper.SKIN_MEDIA_DIR + SkypeHelper.IMG_OUTGOING_CALL;
          for (index = 1; index <= m_callsHistory.Count; index++)
          {
            Call call = m_callsHistory[index];
            if (call == null ||
              call.Type == TCallType.cltIncomingP2P ||
              call.Type == TCallType.cltIncomingPSTN)
            {
              continue;
            }

            StringBuilder sb = new StringBuilder();
            GUIListItem item = new GUIListItem();

            sb.Append(call.PartnerDisplayName);
            sb.Append(" - ");
            sb.Append(call.Timestamp);
            item.Label = sb.ToString();
            item.ItemId = call.Id;

            if (System.IO.File.Exists(strThumb))
            {
              item.ThumbnailImage = strThumb;
              item.IconImage = strThumb;
              item.IconImageBig = strThumb;
            }

            GUIControl.AddListItemControl(GetID, (int)Controls.CONTROL_LSTMISSEDCALLS, item);
          }
          btnPurge.Disabled = false;
        }
        else
        {
          btnPurge.Disabled = true;
        }

        // Purging doesn't work - wait for Skype to fix it...
        // Might be working in 3.2 (Beta), so enable it
        if (GUISkype.SkypeAccess.Version.CompareTo("3.2.0.0") < 0)
        {
          btnPurge.Disabled = true;
        }

        GUIPropertyManager.SetProperty(TAG_LISTNAME, OUTGOING_CALLS + " calls");
      }
      catch (Exception e)
      {
        Log.Warn("SP->Failed to load missed calls list : {0}\n{1}", e.Message, e.StackTrace);
      }
    }
    private void LoadIncomingCalls()
    {
      int index = 0;
      string strThumb = string.Empty;

      try
      {
        GUIControl.ClearControl(GetID, (int)Controls.CONTROL_LSTMISSEDCALLS);

        m_callsHistory = GUISkype.SkypeAccess.get_Calls("");

        if (m_callsHistory != null && m_callsHistory.Count > 0)
        {
          strThumb = GUIGraphicsContext.Skin + SkypeHelper.SKIN_MEDIA_DIR + SkypeHelper.IMG_INCOMING_CALL;
          for (index = 1; index <= m_callsHistory.Count; index++)
          {
            Call call = m_callsHistory[index];
            if (call == null ||
              call.Type == TCallType.cltOutgoingP2P ||
              call.Type == TCallType.cltOutgoingPSTN)
            {
              continue;
            }

            StringBuilder sb = new StringBuilder();
            GUIListItem item = new GUIListItem();

            sb.Append(call.PartnerDisplayName);
            sb.Append(" - ");
            sb.Append(call.Timestamp);
            item.Label = sb.ToString();
            item.ItemId = call.Id;

            if (System.IO.File.Exists(strThumb))
            {
              item.ThumbnailImage = strThumb;
              item.IconImage = strThumb;
              item.IconImageBig = strThumb;
            }

            GUIControl.AddListItemControl(GetID, (int)Controls.CONTROL_LSTMISSEDCALLS, item);
          }
          btnPurge.Disabled = false;
        }
        else
        {
          btnPurge.Disabled = true;
        }

        // Purging doesn't work - wait for Skype to fix it...
        // Might be working in 3.2 (Beta), so enable it
        if (GUISkype.SkypeAccess.Version.CompareTo("3.2.0.0") < 0)
        {
          btnPurge.Disabled = true;
        }

        GUIPropertyManager.SetProperty(TAG_LISTNAME, INCOMING_CALLS + " calls");
      }
      catch (Exception e)
      {
        Log.Warn("SP->Failed to load incoming calls list : {0}\n{1}", e.Message, e.StackTrace);
      }
    }
    public override bool Init()
    {
      return Load(GUIGraphicsContext.Skin + @"\mySkypeHistory.xml");
    }
    private void DisableAllControls()
    {
      btnPurge.Disabled = true;
      return;
    }

    private void PurgeCalls(TCallHistory callType)
    {
      try
      {
        // We purge the calls currently displayed in the list
        CallCollection calls;
        if (callType == TCallHistory.chsMissedCalls)
        {
          calls = m_missedCalls;
        }
        else
        {
          calls = m_callsHistory;
        }

        GUISkype.SkypeAccess.ClearCallHistory(String.Empty, callType);
        GUIControl.ClearControl(GetID, (int)Controls.CONTROL_LSTMISSEDCALLS);
        btnPurge.Disabled = true;

        m_missedCalls = null;
        m_callsHistory = null;
      }
      catch (Exception e)
      {
        Log.Warn("SP->Failed to purge calls({0},{1}): {2}\n{3}", "ALL", callType.ToString(), e.Message, e.StackTrace);
      }
    }
    #endregion

    #region BaseWindow Members
    public override void OnAction(Action action)
    {
      switch (action.wID)
      {
        case Action.ActionType.ACTION_PREVIOUS_MENU:
          GUIWindowManager.ReplaceWindow(SkypeHelper.MAIN_WINDOW_ID);
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

          btnPurge.Disabled = true;

          // init
          try
          {
            LoadMissedCalls();
          }
          catch (Exception e)
          {
            Log.Warn("SP->Unable to initalise skype plugin. Exception :{0}", e.Message);
            DisableAllControls();
          }
          return true;

        case GUIMessage.MessageType.GUI_MSG_WINDOW_DEINIT:
          break;

        case GUIMessage.MessageType.GUI_MSG_ITEM_SELECTED:
          GUIControl gc = GetControl(message.TargetControlId);
          if (gc != null)
          {
            // show other lists of calls (incoming or missed or outgoing)
            if (message.TargetControlId == (int)Controls.CONTROL_BTNHISTORYVIEW)
            {
              switch ((string)gc.GetSubItem(gc.SelectedItem))
              {
                case MISSED_CALLS:
                  LoadMissedCalls();
                  break;

                case OUTGOING_CALLS:
                  LoadOutgoingCalls();
                  break;

                case INCOMING_CALLS:
                  LoadIncomingCalls();
                  break;
              }
            }
          }
          break;

        case GUIMessage.MessageType.GUI_MSG_CLICKED:
          int iControl = message.SenderControlId;

          if (iControl == (int)Controls.CONTROL_BTNHISTORYVIEW)
          {
            GUIMessage msg = new GUIMessage(GUIMessage.MessageType.GUI_MSG_ITEM_SELECTED, GetID, 0, iControl, 0, 0, null);
            GUIGraphicsContext.SendMessage(msg);
            break;
          }

          if (iControl == (int)Controls.CONTROL_BTNPURGE)
          {
            TCallHistory callType = TCallHistory.chsAllCalls;
            GUIControl gcBtn = GetControl((int)Controls.CONTROL_BTNHISTORYVIEW);
            if (gcBtn != null)
            {
              switch ((string)gcBtn.GetSubItem(gcBtn.SelectedItem))
              {
                case MISSED_CALLS:
                  callType = TCallHistory.chsMissedCalls;
                  break;

                case OUTGOING_CALLS:
                  callType = TCallHistory.chsOutgoingCalls;
                  //GUIControl.DisableControl(GetID,(int)Controls.CONTROL_BTNPURGE);
                  break;

                case INCOMING_CALLS:
                  callType = TCallHistory.chsIncomingCalls;
                  //GUIControl.DisableControl(GetID,(int)Controls.CONTROL_BTNPURGE);
                  break;
              }
              if (callType != TCallHistory.chsAllCalls)
              {
                PurgeCalls(callType);
              }
            }
            break;
          }
          break;
      }
      return base.OnMessage(message);
    }
    #endregion
  }
}
