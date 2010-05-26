using System;
using System.Collections.Generic;
using System.Text;
using MediaPortal.Dialogs;
using MediaPortal.GUI.Library;
using SKYPE4COMLib;

namespace maniac.MediaPortal.Skype
{
  public class GUISkypeAcceptance :  GUIDialogWindow
  {
    [SkinControlAttribute(10)]    protected GUIButtonControl btnNo = null;
    [SkinControlAttribute(11)]    protected GUIButtonControl btnYes = null;

    bool m_bConfirmed = false;
    bool m_DefaultYes = false;
    int iYesKey = -1;
    int iNoKey = -1;
    //bool needRefresh = false;
    DateTime vmr7UpdateTimer = DateTime.Now;

    public GUISkypeAcceptance()
    {
      //GetID = (int)GUIWindow.Window.WINDOW_DIALOG_YES_NO;
      GetID = (int)SkypeHelper.ACCEPTANCE_WINDOW_ID;
    }

    public override bool Init()
    {
      GUISkype.SkypeAccess.CallStatus += OnCallStatusChanged;

      return Load(GUIGraphicsContext.Skin + @"\mySkypeAcceptance.xml");
    }


    public override void OnAction(Action action)
    {
      //needRefresh = true;
      if (action.wID == Action.ActionType.ACTION_CLOSE_DIALOG || action.wID == Action.ActionType.ACTION_PREVIOUS_MENU)
      {
        m_DefaultYes = false;
        base.OnAction(action);
        return;
      }

      if (action.wID == Action.ActionType.ACTION_KEY_PRESSED)
      {
        if (action.m_key != null)
        {
          // Yes or No key
          if (action.m_key.KeyChar == iYesKey)
          {
            m_bConfirmed = true;
            PageDestroy();
            m_DefaultYes = false;
            return;
          }

          if (action.m_key.KeyChar == iNoKey)
          {
            m_bConfirmed = false;
            PageDestroy();
            m_DefaultYes = false;
            return;
          }
        }
      }
      base.OnAction(action);
    }

    private void OnCallStatusChanged(Call ChangedCall, TCallStatus CallProgress)
    {
      switch (CallProgress)
      {
        case TCallStatus.clsFinished:
        case TCallStatus.clsCancelled:
        case TCallStatus.clsRefused:
        case TCallStatus.clsFailed:
        case TCallStatus.clsMissed:
          Log.Debug("SPA->CallStatus handled externally");
          btnNo.Selected = true;
          break;

        case TCallStatus.clsRinging:
          Log.Debug("SPA->CallStatus ringing");
          break;

        case TCallStatus.clsOnHold:
        case TCallStatus.clsRemoteHold:
          Log.Debug("SPA->CallStatus OnHold");
          break;

        case TCallStatus.clsInProgress:
          Log.Debug("SPA->CallStatus InProgress");
          btnYes.Selected = true;

          break;

        default:
          Log.Debug("SPA->CallStatus default {0}", CallProgress);
          btnNo.Selected = true;
          break;
      }
    }

    public override bool OnMessage(GUIMessage message)
    {
      //needRefresh = true;
      switch (message.Message)
      {
        case GUIMessage.MessageType.GUI_MSG_WINDOW_DEINIT:
          {
            SetControlLabel(GetID, 1, string.Empty);
            base.OnMessage(message);
            return true;
          }

        case GUIMessage.MessageType.GUI_MSG_WINDOW_INIT:
          {
            m_bConfirmed = false;
            base.OnMessage(message);
            if (m_DefaultYes)
            {
              GUIControl.FocusControl(GetID, btnYes.GetID);
            }
            iYesKey = (int)btnYes.Label.ToLower()[0];
            iNoKey = (int)btnNo.Label.ToLower()[0];
          }
          return true;

        case GUIMessage.MessageType.GUI_MSG_CLICKED:
          {
            int iControl = message.SenderControlId;

            if (btnYes == null)
            {
              m_bConfirmed = true;
              PageDestroy();
              m_DefaultYes = false;
              return true;
            }
            if (iControl == btnNo.GetID)
            {
              m_bConfirmed = false;
              PageDestroy();
              m_DefaultYes = false;
              return true;
            }
            if (iControl == btnYes.GetID)
            {
              m_bConfirmed = true;
              PageDestroy();
              m_DefaultYes = false;
              return true;
            }
          }
          break;

        default:
          Log.Debug("SPA->Acceptance: Msg {0}", message.Message);
          break;
      }

      return base.OnMessage(message);
    }


    public bool IsConfirmed
    {
      get { return m_bConfirmed; }
    }

    public void SetHeading(string strLine)
    {
      LoadSkin();
      AllocResources();
      InitControls();

      SetControlLabel(GetID, 1, strLine);

      //GUIMessage msg = new GUIMessage(GUIMessage.MessageType.GUI_MSG_LABEL_SET, GetID, 0, 1, 0, 0, null);
      //msg.Label = strLine;
      //OnMessage(msg);
      SetLine(1, string.Empty);
      SetLine(2, string.Empty);
      SetLine(3, string.Empty);
    }

    public void SetHeading(int iString)
    {
      if (iString == 0) SetHeading(string.Empty);
      else SetHeading(GUILocalizeStrings.Get(iString));
    }

    public void SetLine(int iLine, string strLine)
    {
      if (iLine <= 0) return;
      GUIMessage msg = new GUIMessage(GUIMessage.MessageType.GUI_MSG_LABEL_SET, GetID, 0, 1 + iLine, 0, 0, null);
      msg.Label = strLine;
      OnMessage(msg);
    }

    public void SetLine(int iLine, int iString)
    {
      if (iLine <= 0) return;
      if (iString == 0) SetLine(iLine, string.Empty);
      else SetLine(iLine, GUILocalizeStrings.Get(iString));
    }

    public void SetDefaultToYes(bool bYesNo)
    {
      m_DefaultYes = bYesNo;
    }

  }
  }

