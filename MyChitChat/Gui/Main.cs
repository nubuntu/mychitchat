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


namespace MyChitChat.Gui {
    public class Main : GUIWindow {

        #region ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ Member Fields ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        private Chat guiWindowChat;
        private bool? statusFilter;
        #endregion

        #region ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ Skin Controls ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

        [SkinControlAttribute(100)]
        GUIListControl ctrlListControlContacts = null;

        [SkinControlAttribute(200)]
        protected GUITextScrollUpControl ctrlTextboxLogHistory = null;

        [SkinControlAttribute(300)]
        protected GUITextControl ctrlTextboxLastMessage = null;

        [SkinControlAttribute(400)]
        protected GUIButtonControl btnContactDetails = null;


        #endregion

        #region ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ Skin Properties ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

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
        private const string TAG_CONTACT_LAST_MESSAGE = "#MyChitChat.Contact.Last.Message";
        private const string TAG_CONTACT_LAST_ACTIVE = "#MyChitChat.Contact.Last.Active";

        private const string TAG_CONTACT_STATUS_TYPE = "#MyChitChat.Contact.Status.Type";
        private const string TAG_CONTACT_STATUS_IMAGE = "#MyChitChat.Contact.Status.Image";
        private const string TAG_CONTACT_STATUS_MESSAGE = "#MyChitChat.Contact.Status.Message";   

        #endregion

        #region ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ Constructor & Initialization ~~~~~~~~~~~~~~~~~~~~~~

        public Main() {
            AddRosterEventHandlers();
            base.Title = Helper.PLUGIN_NAME;
            
            guiWindowChat = new Chat();
            statusFilter = null;
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
            }           
            base.OnPageLoad();

        }


        protected override void OnShowContextMenu() {
            switch (Dialog.Instance.ShowContextMenu(new List<Dialog.ContextMenuButtons>())) {
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
                    this.statusFilter = true;
                    UpdateContactsFacade();
                    break;
                case Dialog.ContextMenuButtons.BtnFilterOffline:
                    this.statusFilter = false;
                    UpdateContactsFacade();
                    break;
                case Dialog.ContextMenuButtons.BtnFilterNone:
                    this.statusFilter = null;
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

        protected override void OnWindowLoaded() {
            base.OnWindowLoaded();
            GUIPropertyManager.SetProperty("#header.text", Helper.PLUGIN_NAME);
            GUIPropertyManager.SetProperty("#header.label", "Main Window");
            AddItemSelectionEventHandlers();
            UpdateContactsFacade();
        }

        protected override void OnPreviousWindow() {
            base.OnPreviousWindow();
        }

        public override void OnAction(MediaPortal.GUI.Library.Action action) {
            switch (action.wID) {
                case MediaPortal.GUI.Library.Action.ActionType.ACTION_KEY_PRESSED:
                    if (ctrlListControlContacts != null && ctrlListControlContacts.IsFocused) {
                        History.Instance.CurrentSession.Reply(Dialog.Instance.GetKeyBoardInput(((char)action.m_key.KeyChar).ToString(), Helper.CurrentKeyboardType));
                    }
                    break;

            }
            base.OnAction(action);
        }

        protected override void OnClicked(int controlId, GUIControl control, MediaPortal.GUI.Library.Action.ActionType actionType) {

            if (control == ctrlListControlContacts && actionType == MediaPortal.GUI.Library.Action.ActionType.ACTION_SELECT_ITEM) {
                try {
                    ShowChatWindow(History.Instance.GetSession(ctrlListControlContacts.SelectedListItem.Path));
                } catch (Exception e) {
                    Log.Error(e);
                }
            }
            base.OnClicked(controlId, control, actionType);
        }


        #endregion

        #region ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ Public Methods ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

        #endregion

        #region ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ Private GUI Methods ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

        private void ShowChatWindow(Session currentChatSession) {
            // do not show info if no contact selected
            if (currentChatSession != null) {
                if (guiWindowChat != null) {
                    GUIWindowManager.ActivateWindow(guiWindowChat.GetID);
                    guiWindowChat.CurrentChatSession = currentChatSession;
                }
            }
        }

        private void UpdateContactsFacade() {
            if (ctrlListControlContacts != null) {
                ctrlListControlContacts.Clear();

                foreach (SessionListItem currentItem in History.Instance.SessionListItems) {
                    try {
                        if (statusFilter.HasValue) {
                            if (statusFilter.Value == currentItem.IsActiveSession) {
                                ctrlListControlContacts.Add(currentItem);
                            }
                        } else {
                            ctrlListControlContacts.Add(currentItem);
                        }

                    } catch (Exception e) {
                        Log.Error(e);
                    }
                }

            }
        }
        private void UpdateGuiUserProperties() {
            Presence myPres = Helper.JABBER_CLIENT.Presence;

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
            GUIPropertyManager.SetProperty(TAG_CONTACT_AVATAR_IMAGE, Cache.GetAvatarImagePath(selectedSession.ContactDetails));
            GUIPropertyManager.SetProperty(TAG_CONTACT_NAME_NICK, selectedSession.ContactDetails.nickname);
            GUIPropertyManager.SetProperty(TAG_CONTACT_LAST_ACTIVE, selectedSession.DateTimeLastActive.ToShortTimeString());
            GUIPropertyManager.SetProperty(TAG_CONTACT_LAST_MESSAGE, selectedSession.Messages.Values.Last().ToString());
            GUIPropertyManager.SetProperty(TAG_CONTACT_STATUS_TYPE, Translations.GetByName(selectedSession.Contact.status.type.ToString()));
            GUIPropertyManager.SetProperty(TAG_CONTACT_STATUS_IMAGE, Helper.GetStatusIcon(selectedSession.Contact.status.type.ToString()));
            GUIPropertyManager.SetProperty(TAG_CONTACT_STATUS_MESSAGE, selectedSession.Contact.status.message);

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

        private void AddItemSelectionEventHandlers() {
            History.Instance.OnSessionItemSelected += new OnSessionItemSelectedEventHandler(History_OnSessionItemSelected);
        }

        private void RemoveEventHandlers() {
            Helper.JABBER_CLIENT.OnRosterStart -= new OnRosterStartEventHandler(JABBER_CLIENT_OnRosterStart);
            Helper.JABBER_CLIENT.OnRosterEnd -= new OnRosterEndEventHandler(JABBER_CLIENT_OnRosterEnd);
            History.Instance.OnUpdatedRoster -= new OnUpdatedRosterEventHandler(History_OnRosterUpdated);
            History.Instance.OnUpdatedPresence -= new OnUpdatedPresenceEventHandler(History_OnContactPresence);
            History.Instance.OnUpdatedSession -= new OnUpdatedSessionEventHandler(History_OnUpdatedSession);
            History.Instance.OnSessionItemSelected -= new OnSessionItemSelectedEventHandler(History_OnSessionItemSelected);
            History.Instance.OnUpdatedLog -= new OnUpdatedLogEventhandler(History_OnUpdatedLog);
        }


        #endregion

        #region ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ EventHandlers ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~


        void JABBER_CLIENT_OnRosterStart() {
            GUIWaitCursor.Show();
        }

        void JABBER_CLIENT_OnRosterEnd() {
            AddHistoryEventHandlers();
            GUIWaitCursor.Hide();
            Helper.SetDefaultPresence();
            if (Settings.selectStatusOnStartup) {
                Dialog.Instance.SelectAndSetStatus();
            }

        }

        void History_OnRosterUpdated(Contact changedContact) {
            UpdateContactsFacade();

        }


        void History_OnContactPresence(Contact updatedContact) {
            UpdateContactsFacade();
        }


        void History_OnUpdatedSession(Session updatedSession, Message msg) {            
            UpdateContactsFacade();
            if (ctrlListControlContacts != null && !ctrlListControlContacts.IsFocused) {
                ctrlListControlContacts.SelectedListItemIndex = History.Instance.GetSessionIndex(updatedSession);
            }
        }

        void History_OnSessionItemSelected(Session selectedSession, GUIControl parentControl) {
            if (parentControl == ctrlListControlContacts) {
                UpdateGuiContactProperties(selectedSession);
            }
        }

        void History_OnUpdatedLog(string logText) {
            if (ctrlTextboxLogHistory != null) {
                ctrlTextboxLogHistory.Label = logText;
            }
        }

        #endregion

        #region ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ Properties Gets/Sets ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

        // With GetID it will be an window-plugin / otherwise a process-plugin
        // Enter the id number here again
        public override int GetID {
            get { return (int)Helper.PLUGIN_WINDOW_IDS.WINDOW_ID_MAIN; }
        }
        #endregion

    }




}
