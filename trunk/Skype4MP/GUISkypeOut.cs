using System;
using System.Collections;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
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
  public class GUISkypeOut : GUIWindow, IRenderLayer
  {
    #region CONST
    public static int WINDOW_ID = 1913;

    private const string TAG_SKYPEOUT_NUMBER = "#Skype.SkypeOut.Number";
    private const string INITIAL_STRING = "<Enter number>";

    private enum Controls
    {
      CONTROL_LBLNUMBER = 2,
      CONTROL_BTNPLUS = 3,
      CONTROL_BTNDEL = 4,
      CONTROL_BTNCALL = 21,
      CONTROL_BTNCLOSE = 22,
      CONTROL_BTN0 = 10,
      CONTROL_BTN1 = 11,
      CONTROL_BTN2 = 12,
      CONTROL_BTN3 = 13,
      CONTROL_BTN4 = 14,
      CONTROL_BTN5 = 15,
      CONTROL_BTN6 = 16,
      CONTROL_BTN7 = 17,
      CONTROL_BTN8 = 18,
      CONTROL_BTN9 = 19
    };
    #endregion

    [SkinControlAttribute(3)]
    protected GUIButtonControl btnPlus = null;
    [SkinControlAttribute(4)]
    protected GUIButtonControl btnDel = null;

    [SkinControlAttribute(10)]
    protected GUIButtonControl btn0 = null;
    [SkinControlAttribute(11)]
    protected GUIButtonControl btn1 = null;
    [SkinControlAttribute(12)]
    protected GUIButtonControl btn2 = null;
    [SkinControlAttribute(13)]
    protected GUIButtonControl btn3 = null;
    [SkinControlAttribute(14)]
    protected GUIButtonControl btn4 = null;
    [SkinControlAttribute(15)]
    protected GUIButtonControl btn5 = null;
    [SkinControlAttribute(16)]
    protected GUIButtonControl btn6 = null;
    [SkinControlAttribute(17)]
    protected GUIButtonControl btn7 = null;
    [SkinControlAttribute(18)]
    protected GUIButtonControl btn8 = null;
    [SkinControlAttribute(19)]
    protected GUIButtonControl btn9 = null;

    [SkinControlAttribute(21)]
    protected GUIButtonControl btnCall = null;
    [SkinControlAttribute(22)]
    protected GUIButtonControl btnClose = null;



    bool m_bRunning = false;
    int m_dwParentWindowID = 0;
    GUIWindow m_pParentWindow = null;
    bool m_bPrevOverlay = true;

    private string number = INITIAL_STRING;
    private bool cancelled = false;

    public GUISkypeOut()
    {
      try
      {
        GetID = WINDOW_ID;

        LoadSettings();
      }
      catch (Exception e)
      {
        Log.Warn("Unable to construct GUISkypeOut");
        Log.Warn(e.Message);
        Log.Warn(e.StackTrace);
      }
      return;
    }

    #region Properties
    public string Number
    {
      get
      {
        return number;
      }
    }

    public bool Cancelled
    {
      get
      {
        return cancelled;
      }
    }
    #endregion

    #region Methods
    public override bool Init()
    {
      return Load(GUIGraphicsContext.Skin + @"\mySkypeOut.xml");
    }
    public override void PreInit()
    {
      //AllocResources();
    }

    private void DisableAllControls()
    {
      GUIControl.DisableControl(GetID, (int)Controls.CONTROL_BTNCLOSE);
      return;
    }

    public void DoModal(int dwParentId)
    {
      m_dwParentWindowID = dwParentId;
      m_pParentWindow = GUIWindowManager.GetWindow(m_dwParentWindowID);
      if (null == m_pParentWindow)
      {
        m_dwParentWindowID = 0;
        return;
      }

      cancelled = true;       // Default to not calling

      GUIWindowManager.RouteToWindow(GetID);

      // activate this window...
      GUIMessage msg = new GUIMessage(GUIMessage.MessageType.GUI_MSG_WINDOW_INIT, GetID, 0, 0, 0, 0, null);
      OnMessage(msg);

      m_bRunning = true;
      while (m_bRunning && GUIGraphicsContext.CurrentState == GUIGraphicsContext.State.RUNNING)
      {
        GUIWindowManager.Process();
        //System.Threading.Thread.Sleep(100);
      }
    }
    void Close()
    {

      GUIWindowManager.IsSwitchingToNewWindow = true;
      lock (this)
      {
        GUIMessage msg = new GUIMessage(GUIMessage.MessageType.GUI_MSG_WINDOW_DEINIT, GetID, 0, 0, 0, 0, null);
        OnMessage(msg);

        GUIWindowManager.UnRoute();
        m_pParentWindow = null;
        m_bRunning = false;
      }
      GUIWindowManager.IsSwitchingToNewWindow = false;
    }
    #endregion

    #region BaseWindow Members
    public override void OnAction(Action action)
    {
      if (action.wID == Action.ActionType.ACTION_CLOSE_DIALOG || action.wID == Action.ActionType.ACTION_PREVIOUS_MENU)
      {
        Close();
        return;
      }
      base.OnAction(action);
    }

    public override bool OnMessage(GUIMessage message)
    {
      switch (message.Message)
      {
        case GUIMessage.MessageType.GUI_MSG_WINDOW_DEINIT:
          base.OnMessage(message);
          GUIGraphicsContext.Overlay = m_bPrevOverlay;
          FreeResources();
          DeInitControls();
          GUILayerManager.UnRegisterLayer(this);
          return true;

        case GUIMessage.MessageType.GUI_MSG_WINDOW_INIT:
          m_bPrevOverlay = GUIGraphicsContext.Overlay;
          base.OnMessage(message);
          GUIGraphicsContext.Overlay = base.IsOverlayAllowed;
          GUILayerManager.RegisterLayer(this, GUILayerManager.LayerType.Dialog);

          number = INITIAL_STRING;
          btnCall.Disabled = true;
          //btnPlus.Disabled = !btnCall.Disabled;
          UpdateDisplay();
          return true;

        case GUIMessage.MessageType.GUI_MSG_CLICKED:
          int iControl = message.SenderControlId;

          // Close and Cancel are the same, except Close doesn't call the number...
          if (iControl == (int)Controls.CONTROL_BTNCLOSE || iControl == (int)Controls.CONTROL_BTNCALL)
          {
            cancelled = (iControl == (int)Controls.CONTROL_BTNCLOSE);
            if (cancelled || number != INITIAL_STRING)
            {
              Close();
            }
            return true;
          }
          else if (iControl >= (int)Controls.CONTROL_BTN0 && iControl <= (int)Controls.CONTROL_BTN9)
          {
            if (number == INITIAL_STRING)
            {
              number = "";
              btnCall.Disabled = false;
              //btnPlus.Disabled = !btnCall.Disabled;
            }
            GUIButtonControl gc = (GUIButtonControl)GetControl(iControl);
            number += gc.Label.Trim();

//            number += (char)(iControl - 10 + 48);
            UpdateDisplay();
            return true;
          }
          else if (iControl == (int)Controls.CONTROL_BTNPLUS)
          {
            if (number == INITIAL_STRING)
            {
              number = "+";
              btnCall.Disabled = false;
              //btnPlus.Disabled = !btnCall.Disabled;
            }
            else if (number.StartsWith("+"))
            {
              if (number != "+")
              {
                number = number.Substring(1);
              }
              else
              {
                number = INITIAL_STRING;
              }
            }
            else if (!number.StartsWith("+"))
            {
              number = "+" + number;
            }
            UpdateDisplay();
            return true;
          }
          else if (iControl == (int)Controls.CONTROL_BTNDEL)
          {
            if (number == INITIAL_STRING)
            {
              number = "";
              btnCall.Disabled = false;
              //btnPlus.Disabled = !btnCall.Disabled;
            }
            if (number.Length > 1)
            {
              number = number.Substring(0, number.Length - 1);
            }
            else
            {
              number = INITIAL_STRING;
              btnCall.Disabled = true;
              //btnPlus.Disabled = !btnCall.Disabled;
            }
            UpdateDisplay();
            return true;
          }
          break;
      }
      return base.OnMessage(message);
    }

    private void UpdateDisplay()
    {
      GUIPropertyManager.SetProperty(TAG_SKYPEOUT_NUMBER, number);
    }
    #endregion

    #region IRenderLayer
    public bool ShouldRenderLayer()
    {
      return true;
    }

    public void RenderLayer(float timePassed)
    {
      Render(timePassed);
    }
    #endregion

    #region Persistance
    private bool LoadSettings()
    {
      return true;
    }
    #endregion
  }
}
