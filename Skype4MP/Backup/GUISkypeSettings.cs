using System;
using System.Collections;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using Microsoft.Win32;

using MediaPortal.Dialogs;
using MediaPortal.GUI.Library;
using MediaPortal.Util;
using System.Collections.Generic;

namespace maniac.MediaPortal.Skype
{
  /// <summary>
  /// Description résumée de GUISkype.
  /// </summary>
  public class GUISkypeSettings : GUIWindow
  {
    #region CONST

    [SkinControlAttribute(1001)]
    protected GUIToggleButtonControl btnGeneral = null;
    [SkinControlAttribute(1002)]
    protected GUIToggleButtonControl btnVoiceVideo = null;
    [SkinControlAttribute(1003)]
    protected GUIToggleButtonControl btnChat = null;
    [SkinControlAttribute(1004)]
    protected GUIToggleButtonControl btnSkypeCredit = null;

    [SkinControlAttribute(11)]
    protected GUIToggleButtonControl cBStartWithMP = null;
    [SkinControlAttribute(12)]
    protected GUIToggleButtonControl cBStopWithMP = null;
    [SkinControlAttribute(13)]
    protected GUIToggleButtonControl cbUseSilentMode = null;
    [SkinControlAttribute(21)]
    protected GUIToggleButtonControl cBHandleVoiceCalls = null;
    [SkinControlAttribute(22)]
    protected GUIToggleButtonControl cBPauseForCalls = null;
    [SkinControlAttribute(23)]
    protected GUIToggleButtonControl cBHangupIfDND = null;
    [SkinControlAttribute(24)]
    protected GUIToggleButtonControl cBIgnoreIncomingCalls = null;
    [SkinControlAttribute(31)]
    protected GUIToggleButtonControl cBHandleChats = null;
    [SkinControlAttribute(32)]
    protected GUIToggleButtonControl cBPauseForChats = null;
    [SkinControlAttribute(41)]
    protected GUIToggleButtonControl cBAllowSkypeOut = null;
    [SkinControlAttribute(42)]
    protected GUIToggleButtonControl cBAllowSMS = null;

    [SkinControlAttribute(101)]
    protected GUIButtonControl btnSave = null;

    /// <summary>
    /// options is the list of options and their current values
    /// </summary>
    private OptionTypeList options = new OptionTypeList();

#endregion

    Dictionary<string, GUIToggleButtonControl> opts = new Dictionary<string, GUIToggleButtonControl>();

    #region Constructors
    public GUISkypeSettings()
    {
      try
      {
        GetID = SkypeHelper.SETTINGS_WINDOW_ID;
      }
      catch (Exception e)
      {
        Log.Warn("Unable to initialise GUISkypeSettings");
        Log.Warn(e.Message);
        Log.Warn(e.StackTrace);
      }
      return;
    }

    #endregion

    #region Methods
    public override bool Init()
    {
      return Load(GUIGraphicsContext.Skin + @"\mySkypeSettings.xml");
    }

    private void DisableAllControls()
    {
      foreach (GUIToggleButtonControl gtbc in opts.Values)
      {
        gtbc.Disabled = true;
      }
    }

    /// <summary>
    /// Defines the link between a Settings file parameter and the button on the SetupForm.
    /// </summary>
    private void InitOptionList()
    {
      opts.Add("StartWithMP", cBStartWithMP);
      opts.Add("StopWithMP", cBStopWithMP);
      opts.Add("HangupIfDND", cBHangupIfDND);
      opts.Add("HandleChats", cBHandleChats);
      opts.Add("HandleVoiceCalls", cBHandleVoiceCalls);
      opts.Add("AllowSkypeOutCalls", cBAllowSkypeOut);
      opts.Add("AllowSMS", cBAllowSMS);
      opts.Add("PauseForIncomingCalls", cBPauseForCalls);
      opts.Add("PauseForIncomingChats", cBPauseForChats);
      opts.Add("UseSilentMode", cbUseSilentMode);
      opts.Add("IgnoreIncomingCalls", cBIgnoreIncomingCalls);
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

          DisableAllControls();

          // Get values from settings file
          if (options.Count == 0)
          {
            options.LoadList();
          }

          // Link buttons to settings
          if (opts.Count == 0)
          {
            InitOptionList();
          }

          // Set button values appropriately and hide the buttons
          // (no group is selected on entry)
          foreach (string ot in options.Keys)
          {
            opts[ot].Selected = options[ot];
            opts[ot].Disabled = false;
          }

          ToggleButtonGroup(null);
          HideAllExcept(new GUIToggleButtonControl[] { });

          // Enabled/disable the sub-options
          cBHandleVoiceCalls_CheckedChanged();
          cBHandleChats_CheckedChanged();

          return true;

        case GUIMessage.MessageType.GUI_MSG_WINDOW_DEINIT:
          break;

        case GUIMessage.MessageType.GUI_MSG_ITEM_SELECTED:
          break;

        case GUIMessage.MessageType.GUI_MSG_CLICKED:
          int iControl = message.SenderControlId;
          GUIToggleButtonControl[] tba = null;
          GUIToggleButtonControl b = null;

          if (iControl == btnSave.GetID)
          {
            Save();
          }
          else if (iControl == cBHandleChats.GetID)
          {
            cBHandleChats_CheckedChanged();
          }
          else if (iControl == cBHandleVoiceCalls.GetID)
          {
            cBHandleVoiceCalls_CheckedChanged();
          }
          else if (iControl == btnGeneral.GetID)
          {
            b = btnGeneral;
            tba = new GUIToggleButtonControl[] { cBStartWithMP, cBStopWithMP, cbUseSilentMode };
          }
          else if (iControl == btnVoiceVideo.GetID)
          {
            b=btnVoiceVideo;
            tba = new GUIToggleButtonControl[] { cBHandleVoiceCalls, cBPauseForCalls, cBHangupIfDND, cBIgnoreIncomingCalls };
          }
          else if (iControl == btnChat.GetID)
          {
            b = btnChat;
            tba = new GUIToggleButtonControl[] { cBHandleChats, cBPauseForChats };
          }
          else if (iControl == btnSkypeCredit.GetID)
          {
            b = btnSkypeCredit;
            tba = new GUIToggleButtonControl[] { cBAllowSkypeOut, cBAllowSMS };
          }

          if (b != null)
          {
            ToggleButtonGroup(b);
            HideAllExcept(tba);
          }
          break;
      }
      return base.OnMessage(message);
    }
    #endregion

    /// <summary>
    /// Set all the buttons except the one passed in to false
    /// </summary>
    /// <param name="gb"></param>
    public void ToggleButtonGroup(GUIToggleButtonControl gb)
    {
      btnGeneral.Selected = false;
      btnVoiceVideo.Selected = false;
      btnSkypeCredit.Selected = false;
      btnChat.Selected = false;

      if (gb != null)
      {
        gb.Selected = true;
      }
    }

    public void HideAllExcept(GUIToggleButtonControl[] tba)
    {
      foreach (GUIToggleButtonControl gb in opts.Values)
      {
        gb.Visible = false;
      }

      foreach (GUIToggleButtonControl gb in tba)
      {
        gb.Visible = true;
      }
    }

    /// <summary>
    /// Save the current vaklues of the settings to the xml file
    /// </summary>
    public void Save()
    {
      // Copy the current button values to the settings
      foreach (string ot in opts.Keys)
      {
        options[ot] = opts[ot].Selected;
      }
      options.SaveList();
      GUISkype sw = (GUISkype)GUIWindowManager.GetWindow(SkypeHelper.MAIN_WINDOW_ID);
      sw.ReloadSettings();
    }



    private void cBHandleVoiceCalls_CheckedChanged()
    {
      cBHangupIfDND.Disabled = !cBHandleVoiceCalls.Selected;
      cBPauseForCalls.Disabled = !cBHandleVoiceCalls.Selected;
      cBIgnoreIncomingCalls.Disabled = !cBHandleVoiceCalls.Selected;
    }

    private void cBHandleChats_CheckedChanged()
    {
      cBPauseForChats.Disabled = !cBHandleChats.Selected;
    }

  }
}
