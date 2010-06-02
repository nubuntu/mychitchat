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
        private RosterContact _myContactInfo;
        #endregion

        #region ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ Skin Controls ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        [SkinControlAttribute(50)]
        protected GUIFacadeControl ctrlFacade = null;
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

        protected override void OnPageLoad() {
            if (!Helper.JABBER_CLIENT.Connected) {
                Helper.JABBER_CLIENT.Connect();
            }
            this.CreateGuiElements();
            base.OnPageLoad();
        }

        protected override void OnShowContextMenu() {
            UpdateMyPresence("Test", true);
            Helper.JABBER_CLIENT.Roster.Clear();
            base.OnShowContextMenu();
        }

        protected override void OnWindowLoaded() {
            Helper.SetMyCurrentPresencePluginEnabled();
            base.OnWindowLoaded();
        }

        protected override void OnPreviousWindow() {
            Helper.SetMyCurrentPresencePluginDisabled();
            base.OnPreviousWindow();
        }


        #endregion

        #region ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ Public Methods ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

        #endregion

        #region ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ Private GUI Methods ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~


        private void UpdateMyPresence(string headerText, bool showCustomStatus) {
            GUIDialogSelect2 dlgSelectStatus = (GUIDialogSelect2)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_SELECT2);
            dlgSelectStatus.Reset();
            dlgSelectStatus.SetHeading(headerText);
            List<GUIListItem> labelList = new List<GUIListItem>();

            labelList.Add(Helper.CreateGuiListItem(Helper.JABBER_PRESENCE_STATES.ONLINE.ToString(),
                                                        Helper.GetFriendlyPresenceState(Helper.JABBER_PRESENCE_STATES.ONLINE),
                                                        Helper.MEDIA_STATUS_AVAILABLE,
                                                        null)
                                                        );
            labelList.Add(Helper.CreateGuiListItem(Helper.JABBER_PRESENCE_STATES.DO_NO_DISTURB.ToString(),
                                                        Helper.GetFriendlyPresenceState(Helper.JABBER_PRESENCE_STATES.DO_NO_DISTURB),
                                                        Helper.MEDIA_STATUS_DND,
                                                        null)
                                                        );
            labelList.Add(Helper.CreateGuiListItem(Helper.JABBER_PRESENCE_STATES.FREE_FOR_CHAT.ToString(),
                                                        Helper.GetFriendlyPresenceState(Helper.JABBER_PRESENCE_STATES.FREE_FOR_CHAT),
                                                        Helper.MEDIA_STATUS_CHAT,
                                                        null)
                                                        );
            labelList.Add(Helper.CreateGuiListItem(Helper.JABBER_PRESENCE_STATES.AWAY.ToString(),
                                                        Helper.GetFriendlyPresenceState(Helper.JABBER_PRESENCE_STATES.AWAY),
                                                        Helper.MEDIA_STATUS_AWAY,
                                                        null)
                                                        );
            labelList.Add(Helper.CreateGuiListItem(Helper.JABBER_PRESENCE_STATES.EXTENDED_AWAY.ToString(),
                                                        Helper.GetFriendlyPresenceState(Helper.JABBER_PRESENCE_STATES.EXTENDED_AWAY),
                                                        Helper.MEDIA_STATUS_XA,
                                                        null)
                                                        );
            if (showCustomStatus) {
                labelList.Add(new GUIListItem("Custom Status..."));
            }
            foreach (GUIListItem currentItem in labelList) {
                dlgSelectStatus.Add(currentItem);
            }
            dlgSelectStatus.DoModal(GUIWindowManager.ActiveWindow);
            if (dlgSelectStatus.SelectedLabelText == "Custom Status...") {
                UpdateMyPresence("Show Custom Status as...", false);
                return;
            }

            try {
                Helper.JABBER_PRESENCE_STATES selectedStatus = (Helper.JABBER_PRESENCE_STATES)Enum.Parse(typeof(Helper.JABBER_PRESENCE_STATES), labelList[dlgSelectStatus.SelectedLabel].Path);
                string tmpMessage = Helper.GetFriendlyPresenceState(selectedStatus);
                if (!showCustomStatus) {
                    VirtualKeyboard keyboard = (VirtualKeyboard)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_VIRTUAL_KEYBOARD);
                    if (null == keyboard)
                        return;

                    keyboard.Reset();
                    keyboard.Text = Helper.GetFriendlyPresenceState(selectedStatus);
                    keyboard.DoModal(GUIWindowManager.ActiveWindow);
                    if (keyboard.IsConfirmed) {
                        tmpMessage = keyboard.Text;
                    }
                }
                Helper.SetMyCurrentPresence(selectedStatus, tmpMessage);
                Helper.JABBER_CLIENT.SendyMyPresence(Helper.JABBER_PRESENCE_CURRENT);
            } catch (Exception ex) {
                Log.Error(ex);
            }

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

        #endregion

        #region ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ Private Business Logic Methods ~~~~~~~~~~~~~~~~~~~~


        private Session CheckCreateSession(Message msg) {
            if (this._dicChatSessions.ContainsKey(msg.FromJID)) {
                return this._dicChatSessions[msg.FromJID];
            } else {
                Session tmpSession = null;
                RosterContact tmpContact = Helper.JABBER_CLIENT.Roster.GetRosterContact(msg.FromJID);
                if (tmpContact != null) {
                    tmpSession = new Session(tmpContact, Helper.JABBER_CLIENT);
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
            Settings.NotifyOnMessage = true;
            Settings.NotifyOnPresence = true;
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
            if (Settings.SetPresenceOnStartup) {
                this.UpdateMyPresence(Helper.PLUGIN_NAME + " - I'm currently...", true);
            } else {
                Helper.JABBER_CLIENT.SendyMyPresence(Helper.JABBER_PRESENCE_DEFAULT);
            }
            InitializeGuiElements();
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
