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
using Microsoft.DirectX.DirectSound;

namespace maniac.MediaPortal.Skype
{
  /// <summary>
  /// Description résumée de GUISkype.
  /// </summary>
  public class GUIAudioDevices : GUIWindow
  {
    #region CONST

    [SkinControlAttribute(1001)]
    protected GUIToggleButtonControl btnAudioIn1 = null;
    [SkinControlAttribute(1002)]
    protected GUIToggleButtonControl btnAudioIn2 = null;
    [SkinControlAttribute(1003)]
    protected GUIToggleButtonControl btnAudioIn3 = null;
    [SkinControlAttribute(1004)]
    protected GUIToggleButtonControl btnAudioIn4 = null;
    [SkinControlAttribute(1005)]
    protected GUIToggleButtonControl btnAudioIn5 = null;
    [SkinControlAttribute(1006)]
    protected GUIToggleButtonControl btnAudioIn6 = null;
    [SkinControlAttribute(1007)]
    protected GUIToggleButtonControl btnAudioIn7 = null;
    [SkinControlAttribute(1008)]
    protected GUIToggleButtonControl btnAudioIn8 = null;
    List<GUIToggleButtonControl> audioInBtns;

    [SkinControlAttribute(2001)]
    protected GUIToggleButtonControl btnAudioOut1 = null;
    [SkinControlAttribute(2002)]
    protected GUIToggleButtonControl btnAudioOut2 = null;
    [SkinControlAttribute(2003)]
    protected GUIToggleButtonControl btnAudioOut3 = null;
    [SkinControlAttribute(2004)]
    protected GUIToggleButtonControl btnAudioOut4 = null;
    [SkinControlAttribute(2005)]
    protected GUIToggleButtonControl btnAudioOut5 = null;
    [SkinControlAttribute(2006)]
    protected GUIToggleButtonControl btnAudioOut6 = null;
    [SkinControlAttribute(2007)]
    protected GUIToggleButtonControl btnAudioOut7 = null;
    [SkinControlAttribute(2008)]
    protected GUIToggleButtonControl btnAudioOut8 = null;
    List<GUIToggleButtonControl> audioOutBtns;
    #endregion

          // Check sound devices
    DXSoundDeviceList dxlst = null;

    #region Constructors
    public GUIAudioDevices()
    {
      try
      {
        GetID = SkypeHelper.AUDIO_DEVICES_WINDOW_ID;
      }
      catch (Exception e)
      {
        Log.Warn("Unable to initialise GUIAudioDevices");
        Log.Warn(e.Message);
        Log.Warn(e.StackTrace);
      }

      // Initialise and log the two lists of devices
      dxlst = new DXSoundDeviceList();
      dxlst.DXAudioInList();
      dxlst.DXAudioOutList();

      return;
    }

    #endregion

    #region Methods
    public override bool Init()
    {
      return Load(GUIGraphicsContext.Skin + @"\mySkypeAudioDevices.xml");
    }

    /// <summary>
    /// Defines the link between a Settings file parameter and the button on the SetupForm.
    /// </summary>
    private void InitLists()
    {
      GUISkype.SkypeAccess.ResetCache();

      audioInBtns = new List<GUIToggleButtonControl>(new GUIToggleButtonControl[]
        {
          btnAudioIn1, btnAudioIn2, btnAudioIn3, btnAudioIn4,
          btnAudioIn5, btnAudioIn6, btnAudioIn7, btnAudioIn8
        }
      );

      foreach (GUIToggleButtonControl gtbc in audioInBtns)
      {
        gtbc.IsVisible = false;
      }

      int i;
      string fnt = audioInBtns[0].FontName;
      string ain = GUISkype.SkypeAccess.Settings.AudioIn;
      string aout = GUISkype.SkypeAccess.Settings.AudioOut;
      long clr = audioInBtns[0].TextColor;

      for (i=0; i < dxlst.myInDevices.Count; i++)
      {
        DeviceInformation d = dxlst.myInDevices[i];
        string s = GetDeviceDisplayName(d.Description);
        if (d.Description.Equals(ain))
        {
          audioInBtns[i].Selected = true;
        }
        else
        {
          audioInBtns[i].Selected = false;
        }
        audioInBtns[i].SetLabel(fnt, s, clr);
        audioInBtns[i].IsVisible = true;
      }

      audioOutBtns = new List<GUIToggleButtonControl>(new GUIToggleButtonControl[]
        {
          btnAudioOut1, btnAudioOut2, btnAudioOut3, btnAudioOut4,
          btnAudioOut5, btnAudioOut6, btnAudioOut7, btnAudioOut8
        }
      );

      foreach (GUIToggleButtonControl gtbc in audioOutBtns)
      {
        gtbc.IsVisible = false;
      }

      for (i = 0; i < dxlst.myOutDevices.Count; i++)
      {
        DeviceInformation d = dxlst.myOutDevices[i];
        string s = GetDeviceDisplayName(d.Description);
        if (d.Description.Equals(aout))
        {
          audioOutBtns[i].Selected = true;
        }
        else
        {
          audioOutBtns[i].Selected = false;
        }
        audioOutBtns[i].SetLabel(fnt, s, clr);
        audioOutBtns[i].IsVisible = true;
      }

    }

    /// <summary>
    /// Prettify the name supplied by Windows so we can fit it all in the 
    /// label provided in the GUI
    /// 
    /// Assumes that the following rules apply:
    /// 
    /// Primary Sound Capture Driver
    /// Microphone (Realtek High Definition Audio)
    /// Headset Microphone (2- Logitech USB Headset)
    /// Realtek Digital Input (Realtek High Definition Audio)
    /// Microphone (2- PHILIPS VOIP321)
    /// 
    /// "Primary Sound Capture Driver" and "Primary Sound Driver" => Windows Default Device
    /// If no "(" then use the whole string;
    /// Otherwise, if the contents of the () start with a numeric char, strip everything up
    /// to and including the first space and use the rest;
    /// Otherwise use everything prior to the "("
    /// 
    /// This worked on TesterBoy's Vista32 machine, so that's what we got...
    /// 
    /// </summary>
    /// <param name="s"></param>
    /// <returns></returns>
    private string GetDeviceDisplayName(string s)
    {
      if (s.IndexOf("(") <= 0)
      {
        if (s.Equals("Primary Sound Capture Driver") || s.Equals("Primary Sound Driver"))
        {
          return "Windows default device";
        }
        return s;
      }
      string t, c;
      t = s.Substring(s.IndexOf("(") + 1, s.IndexOf(")") - s.IndexOf("(") - 1);
      c = t.Substring(0, 1);
      if (t[0] >= '0' && t[0] <= '9')
      {
        return t.Substring(t.IndexOf(" ")+1);
      }

      return s.Substring(0, s.IndexOf("(")-1);
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
          // Load the button names, and hide the ones that we aren't using
          InitLists();
          return true;

        case GUIMessage.MessageType.GUI_MSG_WINDOW_DEINIT:
          break;

        case GUIMessage.MessageType.GUI_MSG_ITEM_SELECTED:
          break;

        case GUIMessage.MessageType.GUI_MSG_CLICKED:
          if ((message.SenderControlId >= 1001 && message.SenderControlId <= 1008) ||
              (message.SenderControlId >= 2001 && message.SenderControlId <= 2008))
          {
            SwitchToDevice(message.SenderControlId);
          }

          break;
      }
      return base.OnMessage(message);
    }
    #endregion

    public void SwitchToDevice(int ctrlId)
    {
      GUIToggleButtonControl ctrl = (GUIToggleButtonControl)this.GetControl(ctrlId);
      if (ctrlId >= 1001 && ctrlId <= 1008)
      {
        string desc = dxlst.myInDevices[ctrlId - 1001].Description;
        if (desc.Equals("Primary Sound Capture Driver"))
        {
          desc = "";
        }

        GUISkype.SkypeAccess.Settings.AudioIn = desc; //ctrl.Label;

        foreach (GUIToggleButtonControl b in audioInBtns)
        {
            b.Selected = (b.GetID == ctrl.GetID);
        }

      }
      else
      {
        string desc = dxlst.myOutDevices[ctrlId - 2001].Description;
        if (desc.Equals("Primary Sound Driver"))
        {
          desc = "";
        }

        GUISkype.SkypeAccess.Settings.AudioOut = desc; //ctrl.Label;
        foreach (GUIToggleButtonControl b in audioOutBtns)
        {
            b.Selected = (b.GetID == ctrl.GetID);
        }
      }
    }

  }
}
