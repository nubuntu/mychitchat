using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using agsXMPP;
using MediaPortal.Dialogs;
using MediaPortal.GUI.Library;
using MyChitChat.Jabber;
using MyChitChat.Plugin;


namespace MyChitChat.Gui {
    public class Main : GUIWindow {

        #region ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ Member Fields ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        private Dictionary<Jid, Session> _dicChatSessions;
        #endregion

        #region ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ Constructor & Initialization ~~~~~~~~~~~~~~~~~~~~~~

        public Main() {
            Helper.JABBER_CLIENT.OnLogin += new OnLoginEventHandler(JabberClient_OnLogin);
            Helper.JABBER_CLIENT.OnError += new OnErrorEventHandler(JABBER_CLIENT_OnError);
            Helper.JABBER_CLIENT.OnMessage += new OnMessageEventHandler(JabberClient_OnMessage);
            Helper.JABBER_CLIENT.OnPresence += new OnPresenceEventHandler(JabberClient_OnPresence);
            Helper.JABBER_CLIENT.OnRosterItem += new OnRosterItemEventHandler(JabberClient_OnRosterItem);
            Helper.JABBER_CLIENT.OnRosterStart += new OnRosterStartEventHandler(JabberClient_OnRosterStart);
            Helper.JABBER_CLIENT.OnRosterEnd += new OnRosterEndEventHandler(JabberClient_OnRosterEnd);
            this._dicChatSessions = new Dictionary<Jid, Session>();
        }

        ~Main() {
            Helper.JABBER_CLIENT.Disconnect();
        }

        #endregion

        #region ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ Override Methods ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

        public override bool Init() {
            if (Settings.AutoConnectStartup) {
                Helper.JABBER_CLIENT.Connect();
            }
            return Load(Helper.SKIN_FILE_MAIN);
        }

        protected override void OnWindowLoaded() {
            if (!Helper.JABBER_CLIENT.Connected) {
                Helper.JABBER_CLIENT.Connect();
            }
            base.OnWindowLoaded();
        }

        #endregion

        #region ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ Public Methods ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

        #endregion

        #region ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ Private GUI Methods ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

        private void ShowContactWindow(RosterContact currentContact) {
            // do not show info if no contact selected
            if (currentContact != null) {
                Contact guiWindowContact = (Contact)GUIWindowManager.GetWindow((int)Helper.PLUGIN_WINDOW_IDS.WINDOW_ID_CONTACT);
                if (guiWindowContact != null) {
                    GUIWindowManager.ActivateWindow(guiWindowContact.GetID);
                }
            }
        }

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

        private void NotifyError(Exception exception) {
            if (Helper.SHOULD_NOTIFY_ERROR) {
                Helper.ShowNotifyDialog(3 * Settings.NotifyTimeOut,
                    Helper.PLUGIN_NAME + " Error!",
                    Helper.MEDIA_ICON_ERROR,
                    exception.ToString(),
                    Helper.PLUGIN_NOTIFY_WINDOWS.WINDOW_DIALOG_NOTIFY
                );
            }
        }

        private void NotifyPresence(RosterContact contact) {
            if (Helper.SHOULD_NOTIFY_PRESENCE) {
                string status = contact.Status;
                if (!String.IsNullOrEmpty(contact.StatusMessage)) {
                    status += "\n\"" + contact.StatusMessage + "\"";
                }
                if (!String.IsNullOrEmpty(contact.Resource)) {
                    status += "\n@" + contact.Resource;
                }
                Helper.ShowNotifyDialog(Settings.NotifyTimeOut, contact.Nickname, Helper.MEDIA_ICON_PRESENCE, status, Settings.NotifyWindowTypePresence);
            }
        }

        private void NotifyMessage(Message msg) {
            if (Helper.SHOULD_NOTIFY_MESSAGE) {
                Helper.ShowNotifyDialog(Settings.NotifyTimeOut, msg.FromNickname, Helper.MEDIA_ICON_MESSAGE, msg.Body, Settings.NotifyWindowTypeMessage);
            }
        }

        private void RefreshContactList() {
            Helper.JABBER_CLIENT.RefreshRoster();
        }

        private void UpdateGuiElements() {
            //throw new NotImplementedException();
        }

        #endregion

        #region ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ Private Business Logic Methods ~~~~~~~~~~~~~~~~~~~~

        private void BuildRoster() {
            List<RosterContact> test = Helper.JABBER_CLIENT.Roster.GetOnlineContacts();

        }

        private Session CheckCreateSession(Message msg) {
            if (this._dicChatSessions.ContainsKey(msg.FromJID)) {
                return this._dicChatSessions[msg.FromJID];
            } else {
                Session tmpSession = null;
                RosterContact tmpContact = Helper.JABBER_CLIENT.Roster.GetRosterContact(msg.FromJID);
                if (tmpContact != null) {
                    tmpSession = new Session(tmpContact,Helper.JABBER_CLIENT);
                    this._dicChatSessions.Add(msg.FromJID, tmpSession);
                }                
                return tmpSession;
            }
        }


        #endregion

        #region ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ Events & Delegates ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

        #endregion

        #region ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ EventHandlers ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

        void JabberClient_OnLogin(object sender) {
            // Once Connected to Jabber keep 'em Messages/Presences pumpin'!
            //Settings.NotifyOnMessage = true;
            //Settings.NotifyOnPresence = true;            
            //ShowNotifyDialog("MyChitChat loaded...");
        }

        void JABBER_CLIENT_OnError(Exception exception) {
            NotifyError(exception);
        }

        void JabberClient_OnPresence(RosterContact presContact) {
            NotifyPresence(presContact);
        }

        void JabberClient_OnMessage(MyChitChat.Jabber.Message msg) {
            NotifyMessage(msg);
            Session currentSession = CheckCreateSession(msg);
        }

        void JabberClient_OnRosterStart() {
            if (Helper.PLUGIN_WINDOW_ACTIVE) {
                GUIWaitCursor.Show();
            }
        }

        void JabberClient_OnRosterItem(object sender, Jid jid) {

        }

        void JabberClient_OnRosterEnd() {
            if (Helper.PLUGIN_WINDOW_ACTIVE) {
                GUIWaitCursor.Hide();
            }
            UpdateGuiElements();
        }
        
        protected override void OnShowContextMenu() {
            BuildRoster();
            base.OnShowContextMenu();
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
