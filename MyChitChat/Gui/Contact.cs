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

//using SKYPEAPILib;
using MyChitChat.Plugin;
using agsXMPP;
using agsXMPP.protocol.iq.vcard;


namespace MyChitChat.Gui {
    /// <summary>
    /// Displays the user details for the selected contact
    /// </summary>
    public class Contact : GUIWindow, IRenderLayer {
        #region CONST
        public static int WINDOW_ID = (int)Helper.PLUGIN_WINDOW_IDS.WINDOW_ID_CONTACT;

        private const string TAG_CONTACT_NAME = "#Skype.ContactInfos.Name";
        private const string TAG_CONTACT_BIRTHDATE = "#Skype.ContactInfos.Birthdate";
        private const string TAG_CONTACT_COUNTRY = "#Skype.ContactInfos.Country";
        private const string TAG_CONTACT_PROVINCE = "#Skype.ContactInfos.Province";
        private const string TAG_CONTACT_CITY = "#Skype.ContactInfos.City";
        private const string TAG_CONTACT_STATUS = "#Skype.ContactInfos.Status";
        private const string TAG_CONTACT_SEX = "#Skype.ContactInfos.Sex";
        private const string TAG_CONTACT_HOMEPAGE = "#Skype.ContactInfos.HomePage";
        private const string TAG_CONTACT_IMG = "#Skype.Image.UserStatus";

        private enum Controls {
            CONTROL_LBLNAME = 2,
            CONTROL_LBLBIRTHDATE = 3,
            CONTROL_LBLCOUNTRY = 4,
            CONTROL_LBLCITY = 5,
            CONTROL_LBLPROVINCE = 6,
            CONTROL_LBLSEX = 7,
            CONTROL_LBLHOMEPAGE = 8,
            CONTROL_LBLSTATUS = 9,
            CONTROL_IMG = 10,
            CONTROL_BTNCLOSE = 20
        };
        #endregion

        private Vcard m_currentUser = null;

        public Vcard CurrentVcard {
            get { return this.m_currentUser; }
            set {
                this.m_currentUser = value;
                UpdateDisplay();
            }
        }

        bool m_bRunning = false;
        int m_dwParentWindowID = 0;
        GUIWindow m_pParentWindow = null;
        bool m_bPrevOverlay = false;

        [SkinControlAttribute(10)]
        protected GUIImage imgStatus = null;


        public Contact() {
            try {
                GetID = WINDOW_ID;
                LoadSettings();
            } catch (Exception e) {
                Log.Warn("Unable to construct GUISkypeContact");
                Log.Warn(e.Message);
                Log.Warn(e.StackTrace);
            }
            return;
        }


        #region Methods
        public void RenderDlg(float timePassed) {
            lock (this) {
                // render the parent window
                if (null != m_pParentWindow)
                    m_pParentWindow.Render(timePassed);

                GUIFontManager.Present();
                // render this dialog box
                base.Render(timePassed);
            }
        }
        public override bool Init() {
            return Load(GUIGraphicsContext.Skin + @"\mySkypeContact.xml");
        }
        public override void PreInit() {
            //AllocResources();
        }

        public override bool SupportsDelayedLoad {
            get { return true; }
        }

        private void DisableAllControls() {
            GUIControl.DisableControl(GetID, (int)Controls.CONTROL_BTNCLOSE);
            return;
        }

        private void UpdateDisplay() {

        //    if (m_currentUser != null) {
        //        try {
        //            string userSex = string.Empty;

        //            switch (m_currentUser.Sex) {
        //                case TUserSex.usexMale:
        //                    userSex = "Male";
        //                    break;
        //                case TUserSex.usexFemale:
        //                    userSex = "Female";
        //                    break;
        //                case TUserSex.usexUnknown:
        //                    userSex = "";
        //                    break;
        //            }

        //            if (userSex != "usexNotSpecified" && userSex != string.Empty && userSex != null) {
        //                GUIPropertyManager.SetProperty(TAG_CONTACT_SEX, userSex);
        //            } else {
        //                GUIPropertyManager.SetProperty(TAG_CONTACT_SEX, SkypeHelper.EMPTY_STRING);
        //            }
        //        } catch (Exception) {
        //            GUIPropertyManager.SetProperty(TAG_CONTACT_SEX, SkypeHelper.EMPTY_STRING);
        //        }

        //        try {
        //            string userName = m_currentUser.FullName;
        //            if (userName.Equals("")) {
        //                userName = m_currentUser.Handle;
        //            } else {
        //                userName += " (" + m_currentUser.Handle + ")";
        //            }

        //            GUIPropertyManager.SetProperty(TAG_CONTACT_NAME, userName);
        //        } catch (Exception) {
        //            GUIPropertyManager.SetProperty(TAG_CONTACT_NAME, SkypeHelper.EMPTY_STRING);
        //        }

        //        try {
        //            if (m_currentUser.Birthday.Year != 1900) {
        //                string userBirthdate = m_currentUser.Birthday.ToShortDateString();
        //                GUIPropertyManager.SetProperty(TAG_CONTACT_BIRTHDATE, userBirthdate);
        //            } else {
        //                GUIPropertyManager.SetProperty(TAG_CONTACT_BIRTHDATE, SkypeHelper.EMPTY_STRING);
        //            }
        //        } catch (Exception) {
        //            GUIPropertyManager.SetProperty(TAG_CONTACT_BIRTHDATE, SkypeHelper.EMPTY_STRING);
        //        }

        //        try {
        //            string userCountry = m_currentUser.Country;


        //            // Also calculate the time difference from here:
        //            int gmtOffsetContact = m_currentUser.Timezone;
        //            if (gmtOffsetContact != -24) {
        //                int gmtOffsetMe = GUISkype.SkypeAccess.CurrentUserProfile.Timezone;
        //                int diff = gmtOffsetContact - gmtOffsetMe;
        //                if (diff != 0) {
        //                    userCountry += " (" + diff.ToString("+#0;-#0;") + " hrs)";
        //                }
        //            }
        //            if (userCountry != string.Empty && userCountry != null) {
        //                GUIPropertyManager.SetProperty(TAG_CONTACT_COUNTRY, userCountry);
        //            } else {
        //                GUIPropertyManager.SetProperty(TAG_CONTACT_COUNTRY, SkypeHelper.EMPTY_STRING);
        //            }
        //        } catch (Exception) {
        //            GUIPropertyManager.SetProperty(TAG_CONTACT_COUNTRY, SkypeHelper.EMPTY_STRING);
        //        }

        //        try {
        //            string userCity = m_currentUser.City;

        //            if (userCity != string.Empty && userCity != null) {
        //                GUIPropertyManager.SetProperty(TAG_CONTACT_CITY, userCity);
        //            } else {
        //                GUIPropertyManager.SetProperty(TAG_CONTACT_CITY, SkypeHelper.EMPTY_STRING);
        //            }
        //        } catch (Exception) {
        //            GUIPropertyManager.SetProperty(TAG_CONTACT_CITY, SkypeHelper.EMPTY_STRING);
        //        }

        //        try {
        //            string userHomepage = m_currentUser.Homepage;

        //            if (userHomepage != string.Empty && userHomepage != null) {
        //                GUIPropertyManager.SetProperty(TAG_CONTACT_HOMEPAGE, userHomepage);
        //            } else {
        //                GUIPropertyManager.SetProperty(TAG_CONTACT_HOMEPAGE, SkypeHelper.EMPTY_STRING);
        //            }
        //        } catch (Exception) {
        //            GUIPropertyManager.SetProperty(TAG_CONTACT_HOMEPAGE, SkypeHelper.EMPTY_STRING);
        //        }

        //        try {
        //            string userProvince = m_currentUser.Province;

        //            if (userProvince != string.Empty && userProvince != null) {
        //                GUIPropertyManager.SetProperty(TAG_CONTACT_PROVINCE, userProvince);
        //            } else {
        //                GUIPropertyManager.SetProperty(TAG_CONTACT_PROVINCE, SkypeHelper.EMPTY_STRING);
        //            }
        //        } catch (Exception) {
        //            GUIPropertyManager.SetProperty(TAG_CONTACT_PROVINCE, SkypeHelper.EMPTY_STRING);
        //        }
        //        GUIPropertyManager.SetProperty(TAG_CONTACT_IMG, SkypeHelper.GetStatusImage(m_currentUser.OnlineStatus));

        //        GUIPropertyManager.SetProperty(TAG_CONTACT_STATUS, SkypeHelper.GetOnlineStatus(m_currentUser.OnlineStatus));
        //        GUILabelControl statusLabel = (GUILabelControl)GetControl((int)Controls.CONTROL_LBLSTATUS);
        //        statusLabel.TextColor = SkypeHelper.GetColorFromOnlineStatus(m_currentUser.OnlineStatus);
        //    } else {
        //        GUIPropertyManager.SetProperty(TAG_CONTACT_NAME, SkypeHelper.EMPTY_STRING);
        //        GUIPropertyManager.SetProperty(TAG_CONTACT_BIRTHDATE, SkypeHelper.EMPTY_STRING);
        //        GUIPropertyManager.SetProperty(TAG_CONTACT_COUNTRY, SkypeHelper.EMPTY_STRING);
        //        GUIPropertyManager.SetProperty(TAG_CONTACT_CITY, SkypeHelper.EMPTY_STRING);
        //        GUIPropertyManager.SetProperty(TAG_CONTACT_STATUS, SkypeHelper.EMPTY_STRING);
        //        GUIPropertyManager.SetProperty(TAG_CONTACT_SEX, SkypeHelper.EMPTY_STRING);
        //        GUIPropertyManager.SetProperty(TAG_CONTACT_HOMEPAGE, SkypeHelper.EMPTY_STRING);
        //        GUIPropertyManager.SetProperty(TAG_CONTACT_PROVINCE, SkypeHelper.EMPTY_STRING);
        //    }
        //    //			// Refresh
        //    //			GUIControl.RefreshControl(WINDOW_ID, (int)Controls.CONTROL_LBLNAME );
        //    //			GUIControl.RefreshControl(WINDOW_ID, (int)Controls.CONTROL_LBLBIRTHDATE );
        //    //			GUIControl.RefreshControl(WINDOW_ID, (int)Controls.CONTROL_LBLCOUNTRY );
        //    //			GUIControl.RefreshControl(WINDOW_ID, (int)Controls.CONTROL_LBLCITY );
        //    //			GUIControl.RefreshControl(WINDOW_ID, (int)Controls.CONTROL_LBLPROVINCE );
        //    //			GUIControl.RefreshControl(WINDOW_ID, (int)Controls.CONTROL_LBLSEX );
        //    //			GUIControl.RefreshControl(WINDOW_ID, (int)Controls.CONTROL_LBLHOMEPAGE );
        //    //			GUIControl.RefreshControl(WINDOW_ID, (int)Controls.CONTROL_LBLSTATUS );
        //}

        //public void DoModal(int dwParentId, Jid Friend) {
        //    m_dwParentWindowID = dwParentId;
        //    m_pParentWindow = GUIWindowManager.GetWindow(m_dwParentWindowID);
        //    if (null == m_pParentWindow) {
        //        m_dwParentWindowID = 0;
        //        return;
        //    }

        //    GUIWindowManager.IsSwitchingToNewWindow = true;
        //    GUIWindowManager.RouteToWindow(GetID);

        //    m_currentUser = Friend;

        //    // activate this window...
        //    //GUIMessage msg = new GUIMessage(GUIMessage.MessageType.GUI_MSG_WINDOW_INIT, GetID, 0, 0, 0, 0, null);
        //    GUIMessage msg = new GUIMessage(GUIMessage.MessageType.GUI_MSG_WINDOW_INIT, GetID, 0, 0, -1, 0, null);
        //    OnMessage(msg);

        //    GUIWindowManager.IsSwitchingToNewWindow = false;

        //    m_bRunning = true;
        //    while (m_bRunning && GUIGraphicsContext.CurrentState == GUIGraphicsContext.State.RUNNING) {
        //        GUIWindowManager.Process();
        //        //System.Threading.Thread.Sleep(100);
        //    }
        }
        void Close() {

            GUIWindowManager.IsSwitchingToNewWindow = true;
            lock (this) {
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
        public override void OnAction(MediaPortal.GUI.Library.Action action) {
            if (action.wID == MediaPortal.GUI.Library.Action.ActionType.ACTION_CLOSE_DIALOG || action.wID == MediaPortal.GUI.Library.Action.ActionType.ACTION_PREVIOUS_MENU) {
                Close();
                return;
            }
            base.OnAction(action);
        }

        public override bool OnMessage(GUIMessage message) {
            switch (message.Message) {
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

                    UpdateDisplay();
                    return true;

                case GUIMessage.MessageType.GUI_MSG_ITEM_SELECTED:
                    break;

                case GUIMessage.MessageType.GUI_MSG_CLICKED:
                    int iControl = message.SenderControlId;

                    if (iControl == (int)Controls.CONTROL_BTNCLOSE) {
                        Close();
                        return true;
                    }
                    break;
            }
            return base.OnMessage(message);
        }
        public override void Render(float timePassed) {
            RenderDlg(timePassed);
        }
        #endregion

        #region IRenderLayer
        public bool ShouldRenderLayer() {
            return true;
        }

        public void RenderLayer(float timePassed) {
            Render(timePassed);
        }
        #endregion

        #region Persistance
        private bool LoadSettings() {
            return true;
        }
        #endregion

    }
}
