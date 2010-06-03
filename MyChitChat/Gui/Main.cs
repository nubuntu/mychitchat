using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using agsXMPP;
using MediaPortal.Dialogs;
using MediaPortal.GUI.Library;
using MyChitChat.Jabber;
using MyChitChat.Plugin;
using nJim;


namespace MyChitChat.Gui {
    public class Main : GUIWindow {

        #region ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ Member Fields ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        private Dictionary<Jid, Session> _dicChatSessions;
        #endregion

        #region ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ Skin Controls ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        [SkinControlAttribute(50)]
        protected GUIFacadeControl ctrlFacade = null;
        #endregion

        #region ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ Constructor & Initialization ~~~~~~~~~~~~~~~~~~~~~~

        public Main() {
            Helper.JABBER_CLIENT.OnLogin += new OnLoginEventHandler(JABBER_CLIENT_OnLogin);
            Helper.JABBER_CLIENT.OnError += new OnErrorEventHandler(JABBER_CLIENT_OnError);
            Helper.JABBER_CLIENT.OnRosterEnd += new OnRosterEndEventHandler(JABBER_CLIENT_OnRosterEnd);
            Helper.JABBER_CLIENT.OnRosterStart += new OnRosterStartEventHandler(JABBER_CLIENT_OnRosterStart);
            this._dicChatSessions = new Dictionary<Jid, Session>();
        }



        ~Main() {
            try {
                Helper.JABBER_CLIENT.Close();
                Helper.JABBER_CLIENT.Roster.PresenceUpdated += new ResourceHandler(Roster_PresenceUpdated);
                Helper.JABBER_CLIENT.Roster.MoodUpdated += new ResourceMoodHandler(Roster_MoodUpdated);
                Helper.JABBER_CLIENT.Roster.ActivityUpdated += new ResourceActivityHandler(Roster_ActivityUpdated);
                Helper.JABBER_CLIENT.Roster.TuneUpdated += new ResourceTuneHandler(Roster_TuneUpdated);
            } catch (Exception ex) {
                Log.Error(ex);
            }
        }

        #endregion

        #region ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ Override Methods ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

        public override bool Init() {
            if (Settings.autoConnectStartup) {
                Helper.JABBER_CLIENT.Login();
            }
            return Load(Helper.SKIN_FILE_MAIN);
        }

        protected override void OnPageLoad() {
            if (!Helper.JABBER_CLIENT.LoggedIn) {
                Helper.JABBER_CLIENT.Login();
            }
            this.CreateGuiElements();
            base.OnPageLoad();
        }

        protected override void OnShowContextMenu() {
            UpdateMyPresence("Test", true);
            //Helper.JABBER_CLIENT.Roster..Clear();
            base.OnShowContextMenu();
        }

        protected override void OnWindowLoaded() {
            //Helper.SetMyCurrentPresencePluginEnabled();
            base.OnWindowLoaded();
        }

        protected override void OnPreviousWindow() {
            //Helper.SetMyCurrentPresencePluginDisabled();
            base.OnPreviousWindow();
        }


        #endregion

        #region ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ Public Methods ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

        #endregion

        #region ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ Private GUI Methods ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~


        private void UpdateMyPresence(string headerText, bool showCustomStatus) {
            //GUIDialogSelect2 dlgSelectStatus = (GUIDialogSelect2)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_SELECT2);
            //dlgSelectStatus.Reset();
            //dlgSelectStatus.SetHeading(headerText);
            //List<GUIListItem> labelList = new List<GUIListItem>();

            //labelList.Add(Helper.CreateGuiListItem(Helper.JABBER_PRESENCE_STATES.ONLINE.ToString(),
            //                                            Helper.GetFriendlyPresenceState(Helper.JABBER_PRESENCE_STATES.ONLINE),
            //                                            Helper.MEDIA_STATUS_AVAILABLE,
            //                                            null)
            //                                            );
            //labelList.Add(Helper.CreateGuiListItem(Helper.JABBER_PRESENCE_STATES.DO_NO_DISTURB.ToString(),
            //                                            Helper.GetFriendlyPresenceState(Helper.JABBER_PRESENCE_STATES.DO_NO_DISTURB),
            //                                            Helper.MEDIA_STATUS_DND,
            //                                            null)
            //                                            );
            //labelList.Add(Helper.CreateGuiListItem(Helper.JABBER_PRESENCE_STATES.FREE_FOR_CHAT.ToString(),
            //                                            Helper.GetFriendlyPresenceState(Helper.JABBER_PRESENCE_STATES.FREE_FOR_CHAT),
            //                                            Helper.MEDIA_STATUS_CHAT,
            //                                            null)
            //                                            );
            //labelList.Add(Helper.CreateGuiListItem(Helper.JABBER_PRESENCE_STATES.AWAY.ToString(),
            //                                            Helper.GetFriendlyPresenceState(Helper.JABBER_PRESENCE_STATES.AWAY),
            //                                            Helper.MEDIA_STATUS_AWAY,
            //                                            null)
            //                                            );
            //labelList.Add(Helper.CreateGuiListItem(Helper.JABBER_PRESENCE_STATES.EXTENDED_AWAY.ToString(),
            //                                            Helper.GetFriendlyPresenceState(Helper.JABBER_PRESENCE_STATES.EXTENDED_AWAY),
            //                                            Helper.MEDIA_STATUS_XA,
            //                                            null)
            //                                            );
            //if (showCustomStatus) {
            //    labelList.Add(new GUIListItem("Custom Status..."));
            //}
            //foreach (GUIListItem currentItem in labelList) {
            //    dlgSelectStatus.Add(currentItem);
            //}
            //dlgSelectStatus.DoModal(GUIWindowManager.ActiveWindow);
            //if (dlgSelectStatus.SelectedLabelText == "Custom Status...") {
            //    UpdateMyPresence("Show Custom Status as...", false);
            //    return;
            //}

            //try {
            //    Helper.JABBER_PRESENCE_STATES selectedStatus = (Helper.JABBER_PRESENCE_STATES)Enum.Parse(typeof(Helper.JABBER_PRESENCE_STATES), labelList[dlgSelectStatus.SelectedLabel].Path);
            //    string tmpMessage = Helper.GetFriendlyPresenceState(selectedStatus);
            //    if (!showCustomStatus) {
            //        VirtualKeyboard keyboard = (VirtualKeyboard)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_VIRTUAL_KEYBOARD);
            //        if (null == keyboard)
            //            return;

            //        keyboard.Reset();
            //        keyboard.Text = Helper.GetFriendlyPresenceState(selectedStatus);
            //        keyboard.DoModal(GUIWindowManager.ActiveWindow);
            //        if (keyboard.IsConfirmed) {
            //            tmpMessage = keyboard.Text;
            //        }
            //    }
            //    Helper.SetMyCurrentPresence(selectedStatus, tmpMessage);
            //    Helper.JABBER_CLIENT.SendyMyPresence(Helper.JABBER_PRESENCE_CURRENT);
            //} catch (Exception ex) {
            //    Log.Error(ex);
            //}

        }

        private void CreateGuiElements() {
            if (ctrlFacade == null) {
                Log.Error(new NullReferenceException("No FacadeControl specified in: " + Helper.SKIN_FILE_MAIN));
                return;
            }

        }

        private void InitializeGuiElements() {

        }

        private void BuildRoster() {

        }

        //private void ShowContactWindow(RosterContact currentContact) {
        //    // do not show info if no contact selected
        //    if (currentContact != null) {
        //        Contact guiWindowContact = (Contact)GUIWindowManager.GetWindow((int)Helper.PLUGIN_WINDOW_IDS.WINDOW_ID_CONTACT);
        //        if (guiWindowContact != null) {
        //            GUIWindowManager.ActivateWindow(guiWindowContact.GetID);
        //        }
        //    }
        //}

        private void ShowChatWindow(Session currentChatSession) {
            // do not show info if no contact selected
            if (currentChatSession != null) {
                Chat guiWindowChat = (Chat)GUIWindowManager.GetWindow((int)Helper.PLUGIN_WINDOW_IDS.WINDOW_ID_CHAT);
                guiWindowChat.CurrentChatSession = currentChatSession;
                if (guiWindowChat != null && guiWindowChat.CurrentChatSession.Equals(currentChatSession)) {
                    GUIWindowManager.ActivateWindow(guiWindowChat.GetID);
                }
            }
        }

        private void NotifyError(nJim.Enums.ErrorType type, string message) {
            Helper.ShowNotifyDialog(3 * Settings.notifyTimeOut,
                Helper.PLUGIN_NAME + " Error!",
                Helper.MEDIA_ICON_ERROR,
                type.ToString() + "\n" + message,
                Helper.PLUGIN_NOTIFY_WINDOWS.WINDOW_DIALOG_NOTIFY
            );
        }

        private void NotifyPresMooActTun(nJim.Contact contact, Mood? mood, Activity? activity, Tune? tune) {
            string header = String.Empty;
            string message = string.Empty;

            Helper.PresMooActNotifyInfo notifyInfo = new Helper.PresMooActNotifyInfo();
            if (contact.identity.nickname != String.Empty) {
                notifyInfo.nickname = contact.identity.nickname;
            } else {
                notifyInfo.nickname = contact.identity.jabberID.user;
            }
            notifyInfo.resource = contact.identity.jabberID.resource;
            notifyInfo.stamp = contact.lastUpdated;
            notifyInfo.header = string.Format("[{1}] {0} ", notifyInfo.nickname, notifyInfo.stamp.ToShortTimeString());
            notifyInfo.status = Helper.GetFriendlyStatusType(contact.status.type);
            notifyInfo.message = contact.status.message;
            notifyInfo.icon = Helper.GetStatusIcon(contact.status);
            if (mood.HasValue) {
                notifyInfo.mood = mood.Value.type.ToString().ToUpper();
                notifyInfo.message = notifyInfo.mood + "\n" + mood.Value.text;
                notifyInfo.icon = Helper.GetMoodIcon(mood.Value);
            }
            if (activity.HasValue) {
                notifyInfo.activity = activity.Value.type.ToString().ToUpper();
                notifyInfo.message = notifyInfo.activity + "\n" + activity.Value.text;
                notifyInfo.icon = Helper.GetActivityIcon(activity.Value);
            }
            if (tune.HasValue) {
                notifyInfo.tune = string.Format("{0}\n{1}", tune.Value.artist, tune.Value.title);
                notifyInfo.message = notifyInfo.tune;
                notifyInfo.icon = Helper.GetTuneIcon(tune.Value);
            }
            header = notifyInfo.header;
            message += notifyInfo.status;
            message += "\n\"" + notifyInfo.message + "\"";
            message += "\n" + notifyInfo.resource;
            Helper.ShowNotifyDialog(header, notifyInfo.icon, message, Settings.notifyWindowTypePresence);
        }

        private void NotifyMessage(Message msg) {
            Helper.ShowNotifyDialog(Settings.notifyTimeOut, msg.FromJID.User, Helper.MEDIA_ICON_MESSAGE, msg.Body, Settings.notifyWindowTypeMessage);
        }

        #endregion

        #region ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ Private Business Logic Methods ~~~~~~~~~~~~~~~~~~~~


        private Session CheckCreateSession(Message msg) {
            //if (this._dicChatSessions.ContainsKey(msg.FromJID)) {
            //    return this._dicChatSessions[msg.FromJID];
            //} else {
            //    Session tmpSession = null;
            //    RosterContact tmpContact = Helper.JABBER_CLIENT.Roster.GetRosterContact(msg.FromJID);
            //    if (tmpContact != null) {
            //        tmpSession = new Session(tmpContact, Helper.JABBER_CLIENT);
            //        this._dicChatSessions.Add(msg.FromJID, tmpSession);
            //    }
            //    return tmpSession;
            //}
            return null;
        }


        #endregion

        #region ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ Events & Delegates ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        #endregion

        #region ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ EventHandlers ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~


        void JABBER_CLIENT_OnLogin(object sender) {
            // Once Connected to Jabber keep 'em Messages/Presences pumpin'!
            //ShowNotifyDialog("MyChitChat loaded...");   

            Helper.JABBER_CLIENT.OnMessage += new OnMessageEventHandler(JABBER_CLIENT_OnMessage);
            Helper.JABBER_CLIENT.Roster.PresenceUpdated += new ResourceHandler(Roster_PresenceUpdated);
            Helper.JABBER_CLIENT.Roster.MoodUpdated += new ResourceMoodHandler(Roster_MoodUpdated);
            Helper.JABBER_CLIENT.Roster.ActivityUpdated += new ResourceActivityHandler(Roster_ActivityUpdated);
            Helper.JABBER_CLIENT.Roster.TuneUpdated += new ResourceTuneHandler(Roster_TuneUpdated);

            Status s = new Status();
            s.type = Enums.StatusType.Normal;
            s.message = "dotNet Jabber Instant Messaging Library";
            Helper.JABBER_CLIENT.Presence.status = s;
            Helper.JABBER_CLIENT.Presence.autoIdleMinutes = 1;
            Mood m = new Mood();
            m.type = Enums.MoodType.neutral;
            m.text = "Hummeur normale";
            Helper.JABBER_CLIENT.Presence.mood = m;
            Activity a = new Activity();
            a.type = Enums.ActivityType.coding;
            a.text = "Dévelopement de nJim";
            Helper.JABBER_CLIENT.Presence.activity = a;

        }


        void JABBER_CLIENT_OnError(nJim.Enums.ErrorType type, string message) {
            NotifyError(type, message);
        }

        void JABBER_CLIENT_OnRosterStart() {
            GUIWaitCursor.Show();
        }

        void JABBER_CLIENT_OnRosterEnd() {
            GUIWaitCursor.Hide();
            if (Settings.setPresenceOnStartup) {
                this.UpdateMyPresence(Helper.PLUGIN_NAME + " - I'm currently...", true);
            } else {
                //Helper.JABBER_CLIENT.SendyMyPresence(Helper.JABBER_PRESENCE_DEFAULT);
            }
            InitializeGuiElements();
        }

        void JABBER_CLIENT_OnMessage(Message msg) {
            if (Settings.notifyOnMessage && Settings.notifyOutsidePlugin)
                NotifyMessage(msg);
            Session currentSession = CheckCreateSession(msg);
        }

        void Roster_PresenceUpdated(nJim.Contact contact) {
            if (Settings.notifyOnPresenceUpdate && Settings.notifyOutsidePlugin && contact.identity.jabberID.full != Helper.JABBER_CLIENT.MyJabberID.full )
                NotifyPresMooActTun(contact, null, null, null);
        }

        void Roster_MoodUpdated(nJim.Contact contact, Mood mood) {
            if (Settings.notifyOnMoodUpdate && Settings.notifyOutsidePlugin)
                NotifyPresMooActTun(contact, mood, null, null);
        }

        void Roster_ActivityUpdated(nJim.Contact contact, Activity activity) {
            if (Settings.notifyOnActivityUpdate && Settings.notifyOutsidePlugin)
                NotifyPresMooActTun(contact, null, activity, null);
        }

        void Roster_TuneUpdated(nJim.Contact contact, Tune tune) {
            if (Settings.notifyOnTuneUpdate && Settings.notifyOutsidePlugin)
                NotifyPresMooActTun(contact, null, null, tune);
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
