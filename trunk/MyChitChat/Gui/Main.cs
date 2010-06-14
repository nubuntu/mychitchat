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


namespace MyChitChat.Gui {
    public class Main : GUIWindow {

        #region ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ Member Fields ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        private Dictionary<Jid, Session> _dicChatSessions;
        #endregion

        #region ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ Skin Controls ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        [SkinControlAttribute(50)]
        GUIFacadeControl ctrlFacadeContactList = null;

        [SkinControlAttribute(499)]
        protected GUIButtonControl btnSetStatus = null;

        [SkinControlAttribute(500)]
        protected GUIButtonControl btnSetMood = null;

        [SkinControlAttribute(501)]
        protected GUIButtonControl btnSetActivity = null;

        [SkinControlAttribute(502)]
        protected GUIButtonControl btnRoster = null;

        [SkinControlAttribute(600)]
        protected GUITextControl ctrlTextboxMessageHistory = null;

        #endregion

        #region ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ Constructor & Initialization ~~~~~~~~~~~~~~~~~~~~~~

        public Main() {
            Helper.JABBER_CLIENT.OnLogin += new OnLoginEventHandler(JABBER_CLIENT_OnLogin);
            Helper.JABBER_CLIENT.OnError += new OnErrorEventHandler(JABBER_CLIENT_OnError);
            Helper.JABBER_CLIENT.OnRosterEnd += new OnRosterEndEventHandler(JABBER_CLIENT_OnRosterEnd);
            Helper.JABBER_CLIENT.OnRosterStart += new OnRosterStartEventHandler(JABBER_CLIENT_OnRosterStart);

            GUIPropertyManager.OnPropertyChanged += new GUIPropertyManager.OnPropertyChangedHandler(GUIPropertyManager_OnPropertyChanged);
            this._dicChatSessions = new Dictionary<Jid, Session>();
        }

        void GUIPropertyManager_OnPropertyChanged(string tag, string tagValue) {
            if (tag == "#Play.Current.Title") {
                //Helper.SetTune(GUIPropertyManager.GetProperty("#Play.Current.File"), GUIPropertyManager.GetProperty("#Play.Current.Artist"), 1);
                Helper.SetTune("Hey Joe", "Jimi Hendrix", 1);
            }

        }


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
        ~Main() {
            try {
                Helper.JABBER_CLIENT.Close();
                Helper.JABBER_CLIENT.Roster.PresenceUpdated -= new ResourceHandler(Roster_PresenceUpdated);
                Helper.JABBER_CLIENT.Roster.MoodUpdated -= new ResourceMoodHandler(Roster_MoodUpdated);
                Helper.JABBER_CLIENT.Roster.ActivityUpdated -= new ResourceActivityHandler(Roster_ActivityUpdated);
                Helper.JABBER_CLIENT.Roster.TuneUpdated -= new ResourceTuneHandler(Roster_TuneUpdated);
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
            base.OnPageLoad();
        }


        protected override void OnShowContextMenu() {
            Helper.JABBER_CLIENT.Presence.setMood(Enums.MoodType.cold, "very");
            Helper.JABBER_CLIENT.Presence.setActivity(Enums.ActivityType.cooking, "plenty");
            Dialog.Instance.SelectAndSetStatus();
            Dialog.Instance.SelectAndSetActivity();
            Dialog.Instance.SelectAndSetMood();
            Helper.JABBER_CLIENT.Presence.setTune("Test song", "Anthrax", 200);
            base.OnShowContextMenu();
        }

        protected override void OnWindowLoaded() {
            base.OnWindowLoaded();
        }

        protected override void OnPreviousWindow() {
            base.OnPreviousWindow();
        }

        protected override void OnClicked(int controlId, GUIControl control, MediaPortal.GUI.Library.Action.ActionType actionType) {
            if (control == btnSetStatus)
                Dialog.Instance.SelectAndSetStatus();
            if (control == btnSetMood)
                Dialog.Instance.SelectAndSetMood();
            if (control == btnSetActivity)
                Dialog.Instance.SelectAndSetActivity();
            if (control == btnRoster)
                UpdateContactsFacade();
            if (control == ctrlFacadeContactList && actionType == MediaPortal.GUI.Library.Action.ActionType.ACTION_SELECT_ITEM) {
                ShowChatWindow(this._dicChatSessions[new Jid(ctrlFacadeContactList.SelectedListItem.Path)]);
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
                Chat guiWindowChat = (Chat)GUIWindowManager.GetWindow((int)Helper.PLUGIN_WINDOW_IDS.WINDOW_ID_CHAT);
                guiWindowChat.CurrentChatSession = currentChatSession;
                if (guiWindowChat != null && guiWindowChat.CurrentChatSession.Equals(currentChatSession)) {
                    GUIWindowManager.ActivateWindow(guiWindowChat.GetID);
                }
            }
        }

        private void NotifyError(nJim.Enums.ErrorType type, string message) {
            Dialog.Instance.ShowNotifyDialog(3 * Settings.notifyTimeOut,
                Helper.PLUGIN_NAME + " Error!",
                Helper.MEDIA_ICON_ERROR,
                type.ToString() + "\n" + message,
                Helper.PLUGIN_NOTIFY_WINDOWS.WINDOW_DIALOG_NOTIFY
            );
        }

        private void NotifyPresMooActTun(nJim.Contact contact, Mood? mood, Activity? activity, Tune? tune) {
            string message = String.Empty;
            Helper.PresMooActNotifyInfo notifyInfo = new Helper.PresMooActNotifyInfo();
            if (contact.identity.nickname != String.Empty) {
                notifyInfo.nickname = contact.identity.nickname;
            } else {
                notifyInfo.nickname = contact.identity.jabberID.user;
            }
            message = notifyInfo.nickname;

            notifyInfo.resource = contact.identity.jabberID.resource;
            notifyInfo.stamp = contact.lastUpdated;
            notifyInfo.status = Translations.GetByName(contact.status.type.ToString());
            notifyInfo.message = contact.status.message;
            notifyInfo.icon = Helper.GetStatusIcon(contact.status.type.ToString());

            if (mood.HasValue) {
                notifyInfo.mood = mood.Value.type.ToString().ToUpperInvariant();
                notifyInfo.message = notifyInfo.mood;
                if (!String.IsNullOrEmpty(mood.Value.text)) {
                    notifyInfo.message += "\n'" + mood.Value.text + "'";
                }
                notifyInfo.icon = Helper.GetMoodIcon(mood.Value.type.ToString());
            }
            if (activity.HasValue) {
                notifyInfo.activity = activity.Value.type.ToString().ToUpperInvariant();
                notifyInfo.message = notifyInfo.activity;
                if (!String.IsNullOrEmpty(activity.Value.text)) {
                    notifyInfo.message += "\n'" + activity.Value.text + "'";
                }
                notifyInfo.icon = Helper.GetActivityIcon(activity.Value.type.ToString());
            }
            if (tune.HasValue) {
                notifyInfo.tune = string.Format("{0}\n{1}\n{2}", tune.Value.artist, tune.Value.title, tune.Value.length);
                notifyInfo.message = notifyInfo.tune;
                notifyInfo.icon = Helper.GetTuneIcon(tune.Value);
            }
            if (!String.IsNullOrEmpty(notifyInfo.message)) {
                message += "\n" + notifyInfo.message;
            }
            notifyInfo.header = string.Format("[{0}] {1} @ {2} ", notifyInfo.stamp.ToShortTimeString(), notifyInfo.status, notifyInfo.resource);

            Dialog.Instance.ShowNotifyDialog(notifyInfo.header, notifyInfo.icon, message, Settings.notifyWindowType);
        }

        private void NotifyMessage(Message msg) {
            Dialog.Instance.ShowNotifyDialog(Settings.notifyTimeOut, msg.FromJID.User, Helper.MEDIA_ICON_MESSAGE, msg.Body, Settings.notifyWindowType);
        }

        private void UpdateContactsFacade() {
            foreach (KeyValuePair<Jid, Session> currentSession in this._dicChatSessions) {
                ctrlFacadeContactList.Add(new ContactListItem(currentSession.Value));
            }

        }


        #endregion

        #region ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ Private Business Logic Methods ~~~~~~~~~~~~~~~~~~~~




        #endregion

        #region ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ Events & Delegates ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        #endregion

        #region ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ EventHandlers ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~


        void JABBER_CLIENT_OnLogin(object sender) {
            // Once Connected to Jabber keep 'em Messages/Presences pumpin'!
            //ShowNotifyDialog("MyChitChat loaded...");   
            Helper.JABBER_CLIENT.Roster.ResourceAdded += new ResourceHandler(Roster_ResourceAdded);
            Helper.JABBER_CLIENT.Roster.ResourceRemoved += new ResourceHandler(Roster_ResourceRemoved);
            //Helper.JABBER_CLIENT.OnMessage += new OnMessageEventHandler(JABBER_CLIENT_OnMessage);
            Helper.JABBER_CLIENT.Roster.PresenceUpdated += new ResourceHandler(Roster_PresenceUpdated);
            Helper.JABBER_CLIENT.Roster.MoodUpdated += new ResourceMoodHandler(Roster_MoodUpdated);
            Helper.JABBER_CLIENT.Roster.ActivityUpdated += new ResourceActivityHandler(Roster_ActivityUpdated);
            Helper.JABBER_CLIENT.Roster.TuneUpdated += new ResourceTuneHandler(Roster_TuneUpdated);

        }

        void Roster_ResourceAdded(nJim.Contact contact) {
            Session newSession = new Session(contact, Helper.JABBER_CLIENT);
            newSession.OnChatSessionUpdated += new OnChatSessionUpdatedEventHandler(newSession_OnChatSessionUpdated);
            this._dicChatSessions.Add(newSession.PartnerJID, newSession);
        }

        void newSession_OnChatSessionUpdated(Session session, Message msg) {
            if ((Settings.notifyOnMessagePlugin && Helper.PLUGIN_WINDOW_ACTIVE) || Settings.notifyOnMessageGlobally) {
                NotifyMessage(msg);
            }
            ctrlTextboxMessageHistory.AddSubItem(msg.ToString());
        }

        void Roster_ResourceRemoved(nJim.Contact contact) {
            this._dicChatSessions.Remove(new Jid(contact.identity.jabberID.full));
        }

        void JABBER_CLIENT_OnError(nJim.Enums.ErrorType type, string message) {
            if ((Settings.notifyOnErrorPlugin && Helper.PLUGIN_WINDOW_ACTIVE) || Settings.notifyOnErrorGlobally) {
                NotifyError(type, message);
            }
        }

        void JABBER_CLIENT_OnRosterStart() {
            GUIWaitCursor.Show();
        }

        void JABBER_CLIENT_OnRosterEnd() {
            GUIWaitCursor.Hide();
            Helper.SetDefaultPresence();
            if (Settings.selectStatusOnStartup) {
                Dialog.Instance.SelectAndSetStatus();
            }
            UpdateContactsFacade();
        }


        //void JABBER_CLIENT_OnMessage(Message msg) {
        //    if ((Settings.notifyOnMessage && Helper.PLUGIN_WINDOW_ACTIVE) || Settings.notifyOnMessageGlobally) {
        //        NotifyMessage(msg);
        //    }

        //}

        void Roster_PresenceUpdated(nJim.Contact contact) {
            if (((Settings.notifyOnStatusPlugin && Helper.PLUGIN_WINDOW_ACTIVE) || Settings.notifyOnStatusGlobally) && contact.identity.jabberID.full != Helper.JABBER_CLIENT.MyJabberID.full) {
                NotifyPresMooActTun(contact, null, null, null);
                UpdateContactsFacade();
            }
        }

        void Roster_MoodUpdated(nJim.Contact contact, Mood mood) {
            if ((Settings.notifyOnMoodPlugin && Helper.PLUGIN_WINDOW_ACTIVE) || Settings.notifyOnMoodGlobally) {
                NotifyPresMooActTun(contact, mood, null, null);
                UpdateContactsFacade();
            }
        }

        void Roster_ActivityUpdated(nJim.Contact contact, Activity activity) {
            if ((Settings.notifyOnActivityPlugin && Helper.PLUGIN_WINDOW_ACTIVE) || Settings.notifyOnActivityGlobally) {
                NotifyPresMooActTun(contact, null, activity, null);
                UpdateContactsFacade();
            }
        }

        void Roster_TuneUpdated(nJim.Contact contact, Tune tune) {
            if ((Settings.notifyOnTunePlugin && Helper.PLUGIN_WINDOW_ACTIVE) || Settings.notifyOnTuneGlobally) {
                NotifyPresMooActTun(contact, null, null, tune);
                UpdateContactsFacade();
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

    public class ContactListItem : GUIListItem {

        private Session _jabberSession = null;

        public ContactListItem(Session session) {
            this._jabberSession = session;
            this.Path = session.PartnerJID.ToString();
            this.Label = session.PartnerNickname;
            this.Label2 = Translations.GetByName(session.Contact.status.type.ToString());
            //this.Label3 = Translations.GetByName(session.Contact.activity.type.ToString());
            this.Label3 = String.Format("[{0} unread]", session.Messages.Count(x => x.Value.Unread));
            this.IconImage = this.IconImageBig = Helper.GetStatusIcon(session.Contact.status.type.ToString());
        }

        public Jid JID {
            get { return this._jabberSession.PartnerJID; }
        }

    }


}
