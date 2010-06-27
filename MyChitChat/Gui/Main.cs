using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using agsXMPP;
using MediaPortal.Dialogs;
using MyChitChat.Jabber;
using MyChitChat.Plugin;
using nJim;
using MediaPortal.GUI.Library;
using MediaPortal.TagReader;
using System.Text;
using System.Windows.Forms;


namespace MyChitChat.Gui {
    public class Main : GUIWindow {

        #region ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ Member Fields ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        private Chat guiWindowChat;
        
        #endregion

        #region ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ Skin Controls ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

        [SkinControlAttribute(100)]
        GUIListControl ctrlListControlContacts = null;

        [SkinControlAttribute(200)]
        protected GUITextControl ctrlTextboxEventLog = null;

        [SkinControlAttribute(300)]
        protected GUITextControl ctrlTextboxLastMessage = null;

        [SkinControlAttribute(401)]
        protected GUIButtonControl btnNewMessage = null;
        
        #endregion

        #region ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ Skin Properties ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

        private const string TAG_USER_NAME_NICK = "#MyChitChat.User.Name.Nick";              
        private const string TAG_USER_AVATAR_IMAGE = "#MyChitChat.User.Avatar.Image";
        private const string TAG_USER_STATUS_TYPE = "#MyChitChat.User.Status.Type";
        private const string TAG_USER_STATUS_IMAGE = "#MyChitChat.User.Status.Image";
        private const string TAG_USER_STATUS_MESSAGE = "#MyChitChat.User.Status.Message";
        private const string TAG_USER_MOOD_TYPE = "#MyChitChat.User.Mood.Type";
        private const string TAG_USER_MOOD_IMAGE = "#MyChitChat.User.Mood.Image";
        private const string TAG_USER_MOOD_MESSAGE = "#MyChitChat.User.Mood.Message";
        private const string TAG_USER_ACTIVITY_TYPE = "#MyChitChat.User.Activity.Type";
        private const string TAG_USER_ACTIVITY_IMAGE = "#MyChitChat.User.Activity.Image";
        private const string TAG_USER_ACTIVITY_MESSAGE = "#MyChitChat.User.Activity.Message";
        private const string TAG_USER_TUNE_TITLE = "#MyChitChat.User.Tune.Title";
        private const string TAG_USER_TUNE_MESSAGE = "#MyChitChat.User.Tune.Artist";

        private const string TAG_CONTACT_AVATAR_IMAGE = "#MyChitChat.Contact.Avatar.Image";
        private const string TAG_CONTACT_NAME_NICK = "#MyChitChat.Contact.Name.Nick";
        private const string TAG_CONTACT_LAST_ACTIVE = "#MyChitChat.Contact.Last.Active";

        private const string TAG_CONTACT_STATUS_TYPE = "#MyChitChat.Contact.Status.Type";
        private const string TAG_CONTACT_STATUS_IMAGE = "#MyChitChat.Contact.Status.Image";
        private const string TAG_CONTACT_STATUS_MESSAGE = "#MyChitChat.Contact.Status.Message";

        #endregion

        #region ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ Constructor & Initialization ~~~~~~~~~~~~~~~~~~~~~~

        public Main() {
            AddRosterEventHandlers();
            guiWindowChat = new Chat();
            StatusFilter = true;
        }

        ~Main() {
            try {
                RemoveEventHandlers();
            } catch (Exception ex) {
                Log.Error(ex);
            }
        }

        #endregion

        #region ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ Override Methods ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        //public override void OnAction(MediaPortal.GUI.Library.Action action) {
        //    switch ((MediaPortal.GUI.Library.Action.ActionType)action.wID) {
        //        case MediaPortal.GUI.Library.Action.ActionType.ACTION_MUSIC_PLAY:
        //            Helper.JABBER_CLIENT.Presence.setActivity(Enums.ActivityType.relaxing, "Music....");                    
        //            break;
        //        case MediaPortal.GUI.Library.Action.ActionType.ACTION_SHOW_FULLSCREEN:
        //            Helper.JABBER_CLIENT.Presence.status = Helper.GetStatusFromType(Enums.StatusType.DoNotDisturb, "I'm in Fullscreen mode so please knock it off...");
        //            Helper.JABBER_CLIENT.Presence.applyStatus();
        //            break;

        //    }
        //    base.OnAction(action);

        //}
        public override bool Init() {
            History.Instance.ToString();
            GUIWindow window = (GUIWindow)guiWindowChat;
            GUIWindowManager.Add(ref window);
            guiWindowChat.Init();
            if (Settings.autoConnectStartup) {
                Helper.JABBER_CLIENT.Login();
            }
            return Load(Helper.SKIN_FILE_MAIN);
        }

        protected override void OnPageLoad() {
            if (!Helper.JABBER_CLIENT.LoggedIn) {
                Helper.JABBER_CLIENT.Login();
            } else {
                Helper.SetPluginEnterPresence();
            }
            SetupGuiControls();
            UpdateContactsFacade();
            if (ctrlTextboxEventLog != null) {
                ctrlTextboxEventLog.Label = History.Instance.LogHistory.ToString();
            }
            base.OnPageLoad();
        }

        protected override void OnPreviousWindow() {
            if (Helper.JABBER_CLIENT.LoggedIn) {            
                Helper.SetPluginLeavePresence();
            }
            base.OnPreviousWindow();
        }

        public override void OnAction(MediaPortal.GUI.Library.Action action) {
            switch (action.wID) {
                case MediaPortal.GUI.Library.Action.ActionType.ACTION_KEY_PRESSED:
                    if (ctrlListControlContacts.IsFocused && ctrlListControlContacts.SelectedListItem != null) {
                        if (action.m_key.KeyChar != 13 && action.m_key.KeyChar != 27 && ctrlListControlContacts != null) {
                            History.Instance.GetSession(ctrlListControlContacts.SelectedListItem.Path).Reply(Dialog.Instance.GetKeyBoardInput(((char)action.m_key.KeyChar).ToString(), Helper.CurrentKeyboardType));
                            return;
                        }
                    }
                    break;
            }
            base.OnAction(action);
        }

        protected override void OnClicked(int controlId, GUIControl control, MediaPortal.GUI.Library.Action.ActionType actionType) {

            if (control == ctrlListControlContacts && actionType == MediaPortal.GUI.Library.Action.ActionType.ACTION_SELECT_ITEM) {
                try {
                    CurrentSession = History.Instance.GetSession(ctrlListControlContacts.SelectedListItem.Path);
                    ShowChatWindow(CurrentSession);
                } catch (Exception e) {
                    Log.Error(e);
                }
            }
            if (control == btnNewMessage && CurrentSession != null) {
                CurrentSession.Reply();
            
            }
            base.OnClicked(controlId, control, actionType);           
        }
       
        protected override void OnShowContextMenu() {
            List<Dialog.ContextMenuButtons> tmpList = new List<Dialog.ContextMenuButtons>();
            foreach (Dialog.ContextMenuButtons tmp in Enum.GetValues(typeof(Dialog.ContextMenuButtons))) {
                tmpList.Add(tmp);
            }
            switch (Dialog.Instance.ShowContextMenu(tmpList)) {
                case Dialog.ContextMenuButtons.BtnSelectStatus:
                    Dialog.Instance.SelectAndSetStatus();
                    UpdateGuiUserProperties();
                    break;
                case Dialog.ContextMenuButtons.BtnSelectMood:
                    Dialog.Instance.SelectAndSetMood();
                    UpdateGuiUserProperties();
                    break;

                case Dialog.ContextMenuButtons.BtnSelectActivity:
                    Dialog.Instance.SelectAndSetActivity();
                    UpdateGuiUserProperties();
                    break;
                case Dialog.ContextMenuButtons.BtnFilterOnline:
                    StatusFilter = true;
                    UpdateContactsFacade();
                    break;
                case Dialog.ContextMenuButtons.BtnFilterOffline:
                    StatusFilter = false;
                    UpdateContactsFacade();
                    break;
                case Dialog.ContextMenuButtons.BtnFilterNone:
                    StatusFilter = null;
                    UpdateContactsFacade();
                    break;
                case Dialog.ContextMenuButtons.BtnJabberDisconnect:
                    Helper.JABBER_CLIENT.Close();
                    History.Instance.ResetHistory();
                    break;
                case Dialog.ContextMenuButtons.BtnJabberReconnect:
                    Helper.JABBER_CLIENT.Reconnect();
                    break;
                case Dialog.ContextMenuButtons.BtnSelectKeyboard:
                    Dialog.Instance.SelectAndSetKeyboardType();
                    break;
                case Dialog.ContextMenuButtons.NothingSelected:
                default:
                    //throw new ArgumentOutOfRangeException();
                    return;
            }
        }

        #endregion

        #region ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ Public Methods ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

        #endregion

        #region ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ Private GUI Methods ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~


        /// <summary>
        /// Setups the GUI controls.
        /// </summary>
        private void SetupGuiControls() {
            GUIPropertyManager.SetProperty("#title", Helper.PLUGIN_NAME);
            GUIPropertyManager.SetProperty("#header.value", Helper.PLUGIN_NAME);
            GUIPropertyManager.SetProperty("#header.text", Helper.PLUGIN_NAME);
            GUIPropertyManager.SetProperty("#header.label", "Main Window");
            GUIPropertyManager.SetProperty("#header.image", Helper.MEDIA_HOVER_HOME);
            GUIPropertyManager.SetProperty("#currentmodule", Helper.PLUGIN_NAME);
            
            this.ctrlListControlContacts.RemoteColor = 0xFFFF6347;
            this.ctrlListControlContacts.PlayedColor = 0x2090EE90;
            this.ctrlListControlContacts.DownloadColor = 0xFF90EE90;
            this.ctrlListControlContacts.ShadedColor = 0xffff00000;
        }

        /// <summary>
        /// Shows the chat window.
        /// </summary>
        /// <param name="currentChatSession">The current chat session.</param>
        private void ShowChatWindow(Session currentChatSession) {
            // do not show info if no contact selected
            if (currentChatSession != null) {
                if (guiWindowChat != null) {
                    GUIWindowManager.ActivateWindow(guiWindowChat.GetID);
                    guiWindowChat.CurrentChatSession = currentChatSession;
                }
            }
        }

        /// <summary>
        /// Updates the contacts facade.
        /// </summary>
        private void UpdateContactsFacade() {
            if (ctrlListControlContacts != null) {
                ctrlListControlContacts.Clear();
                foreach (Session currentSession in History.Instance.ChatSessions) {
                    currentSession.UpdateItemInfo();
                    currentSession.OnItemSelected -= new GUIListItem.ItemSelectedHandler(OnSessionItemSelected);
                    currentSession.OnItemSelected += new GUIListItem.ItemSelectedHandler(OnSessionItemSelected);
                    try {
                        if (StatusFilter.HasValue) {
                            if (StatusFilter.Value == currentSession.IsActiveSession) {
                                ctrlListControlContacts.Add(currentSession);
                            }
                        } else {
                            ctrlListControlContacts.Add(currentSession);
                        }

                    } catch (Exception e) {
                        Log.Error(e);
                    }
                }
                ctrlListControlContacts.DoUpdate();
            }
        }
        private void UpdateGuiUserProperties() {
            Presence myPres = Helper.JABBER_CLIENT.Presence;
            GUIPropertyManager.SetProperty(TAG_USER_NAME_NICK, Helper.JABBER_CLIENT.Identity.nickname);
            
            GUIPropertyManager.SetProperty(TAG_USER_STATUS_TYPE, Translations.GetByName(myPres.status.type.ToString()));
            GUIPropertyManager.SetProperty(TAG_USER_STATUS_IMAGE, Helper.GetStatusIcon(myPres.status.type.ToString()));
            GUIPropertyManager.SetProperty(TAG_USER_STATUS_MESSAGE, myPres.status.message);
            GUIPropertyManager.SetProperty(TAG_USER_MOOD_TYPE, Translations.GetByName(myPres.mood.type.ToString()));
            GUIPropertyManager.SetProperty(TAG_USER_MOOD_IMAGE, Helper.GetMoodIcon(myPres.mood.type.ToString()));
            GUIPropertyManager.SetProperty(TAG_USER_MOOD_MESSAGE, myPres.mood.text);
            GUIPropertyManager.SetProperty(TAG_USER_ACTIVITY_TYPE, Translations.GetByName(myPres.activity.type.ToString()));
            GUIPropertyManager.SetProperty(TAG_USER_ACTIVITY_IMAGE, Helper.GetActivityIcon(myPres.activity.type.ToString()));
            GUIPropertyManager.SetProperty(TAG_USER_ACTIVITY_MESSAGE, myPres.activity.text);
            GUIPropertyManager.SetProperty(TAG_USER_TUNE_TITLE, myPres.tune.title);
            GUIPropertyManager.SetProperty(TAG_USER_TUNE_MESSAGE, myPres.tune.artist);
        }

        private void UpdateGuiContactProperties(Session selectedSession) {
            if (selectedSession == null) {
                return;
            }
            GUIPropertyManager.SetProperty(TAG_CONTACT_AVATAR_IMAGE, Cache.GetAvatarImagePath(selectedSession.ContactDetails));
            GUIPropertyManager.SetProperty(TAG_CONTACT_NAME_NICK, selectedSession.ContactNickname);
            GUIPropertyManager.SetProperty(TAG_CONTACT_LAST_ACTIVE, selectedSession.DateTimeLastActive.ToShortTimeString());
            GUIPropertyManager.SetProperty(TAG_CONTACT_STATUS_TYPE, Translations.GetByName(selectedSession.Contact.status.type.ToString()));
            GUIPropertyManager.SetProperty(TAG_CONTACT_STATUS_IMAGE, Helper.GetStatusIcon(selectedSession.Contact.status.type.ToString()));
            GUIPropertyManager.SetProperty(TAG_CONTACT_STATUS_MESSAGE, selectedSession.Contact.status.message);
            if (ctrlTextboxLastMessage != null) {
                ctrlTextboxLastMessage.Clear();
                if (selectedSession.Messages.Count > 0) {
                    ctrlTextboxLastMessage.Label = selectedSession.Messages.Last().ToString();
                }
            }
        }



        #endregion

        #region ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ Private Business Logic Methods ~~~~~~~~~~~~~~~~~~~~




        #endregion

        #region ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ Events & Delegates ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

        private void AddRosterEventHandlers() {
            Helper.JABBER_CLIENT.OnRosterStart += new OnRosterStartEventHandler(JABBER_CLIENT_OnRosterStart);
            Helper.JABBER_CLIENT.OnRosterEnd += new OnRosterEndEventHandler(JABBER_CLIENT_OnRosterEnd);
        }

        private void AddHistoryEventHandlers() {
            History.Instance.OnUpdatedRoster += new OnUpdatedRosterEventHandler(History_OnRosterUpdated);
            History.Instance.OnUpdatedPresence += new OnUpdatedPresenceEventHandler(History_OnContactPresence);
            History.Instance.OnUpdatedSession += new OnUpdatedSessionEventHandler(History_OnUpdatedSession);
            History.Instance.OnUpdatedLog += new OnUpdatedLogEventhandler(History_OnUpdatedLog);
        }

        private void RemoveEventHandlers() {
            Helper.JABBER_CLIENT.OnRosterStart -= new OnRosterStartEventHandler(JABBER_CLIENT_OnRosterStart);
            Helper.JABBER_CLIENT.OnRosterEnd -= new OnRosterEndEventHandler(JABBER_CLIENT_OnRosterEnd);
            History.Instance.OnUpdatedRoster -= new OnUpdatedRosterEventHandler(History_OnRosterUpdated);
            History.Instance.OnUpdatedPresence -= new OnUpdatedPresenceEventHandler(History_OnContactPresence);
            History.Instance.OnUpdatedSession -= new OnUpdatedSessionEventHandler(History_OnUpdatedSession);
            History.Instance.OnUpdatedLog -= new OnUpdatedLogEventhandler(History_OnUpdatedLog);
        }


        #endregion

        #region ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ EventHandlers ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~


        void JABBER_CLIENT_OnRosterStart() {
            AddHistoryEventHandlers();
            GUIWaitCursor.Init();
            GUIWaitCursor.Show();
        }

        void JABBER_CLIENT_OnRosterEnd() {
            GUIWaitCursor.Hide();
            History_OnUpdatedLog(History.Instance.LogHistory.ToString());            
            Helper.SetPluginEnterPresence();
            History_OnUpdatedLog(History.Instance.LogHistory.ToString());
            if (Settings.selectStatusOnStartup) {
                Dialog.Instance.SelectAndSetStatus();
            }
            UpdateGuiUserProperties();
        }

        void History_OnRosterUpdated(Contact changedContact) {
            UpdateContactsFacade();
            
        }


        void History_OnContactPresence(Contact updatedContact) {
            UpdateContactsFacade();
            UpdateGuiContactProperties(History.Instance.GetSession(updatedContact.identity.jabberID.full));
            if (guiWindowChat.CurrentChatSession != null && updatedContact.identity.jabberID.bare.ToLower() == guiWindowChat.CurrentChatSession.ContactJID.Bare.ToLower()) {
                guiWindowChat.UpdateGuiContactProperties();
            }
        }


        void History_OnUpdatedSession(Session updatedSession, MyChitChat.Jabber.Message msg) {
            UpdateContactsFacade();
            if (ctrlListControlContacts != null){// && !ctrlListControlContacts.IsFocused) {
                ctrlListControlContacts.SelectedListItemIndex = History.Instance.GetSessionIndex(updatedSession);
            }
            UpdateGuiContactProperties(updatedSession);
        }

        void OnSessionItemSelected(GUIListItem sessionItem, GUIControl parentControl) {
            if (sessionItem != null && parentControl == ctrlListControlContacts) {
                CurrentSession = History.Instance.GetSession(sessionItem.Path);
                UpdateGuiContactProperties(CurrentSession);                
               
            }
        }

        void History_OnUpdatedLog(string logText) {
            try {
                if (ctrlTextboxEventLog != null) {
                    ctrlTextboxEventLog.Label = logText;
                }
            } catch { }
        }

        #endregion

        #region ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ Properties Gets/Sets ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

        public static bool? StatusFilter { get; private set; }

        private static Session CurrentSession { get; set; }

        // With GetID it will be an window-plugin / otherwise a process-plugin
        // Enter the id number here again
        public override int GetID {
            get { return (int)Helper.PLUGIN_WINDOW_IDS.WINDOW_ID_MAIN; }
        }
        #endregion

    }




}
