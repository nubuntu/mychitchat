using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MediaPortal.GUI.Library;
using MyChitChat.Plugin;
using MyChitChat.Jabber;
using MediaPortal.Dialogs;
using agsXMPP;
using agsXMPP.protocol.client;
using System.Text.RegularExpressions;
using agsXMPP.protocol.iq.vcard;
using agsXMPP.ui.roster;


namespace MyChitChat.Gui {
    public class Main : GUIWindow {

        #region ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ Member Fields ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        
        #endregion

        #region ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ Constructor & Initialization ~~~~~~~~~~~~~~~~~~~~~~

        public Main() {
            Helper.JABBER_CLIENT.OnLogin += new OnLoginEventHandler(JabberClient_OnLogin);
            Helper.JABBER_CLIENT.OnMessage += new OnMessageEventHandler(JabberClient_OnMessage);
            Helper.JABBER_CLIENT.OnPresence += new OnPresenceEventHandler(JabberClient_OnPresence);
            Helper.JABBER_CLIENT.OnRosterItem += new OnRosterItemEventHandler(JabberClient_OnRosterItem);
            Helper.JABBER_CLIENT.OnRosterStart += new OnRosterStartEventHandler(JabberClient_OnRosterStart);
            Helper.JABBER_CLIENT.OnRosterEnd += new OnRosterEndEventHandler(JabberClient_OnRosterEnd);
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
            return Load(Helper.SKINFILE_WINDOW_MAIN);
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

        #region ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ Private Methods ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

        private static void ShowNotifyDialog(string notifyMessage) {
            ShowNotifyDialog(Helper.PLUGIN_NAME, GUIGraphicsContext.Skin + @"\Media\MyChitChat_read_message.png", notifyMessage);
        }

        private static void ShowNotifyDialog(string header, string icon, string text) {
            try {
                GUIDialogNotify dialogMailNotify = (GUIDialogNotify)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_NOTIFY);
                dialogMailNotify.TimeOut = Settings.NotifyTimeOut;
                dialogMailNotify.SetImage(icon);
                dialogMailNotify.SetHeading(header);
                dialogMailNotify.SetText(text);
                dialogMailNotify.DoModal(GUIWindowManager.ActiveWindow);
            } catch (Exception ex) {
                Log.Error(ex);
            }
        }

        private void ShowContactWindow(RosterContact currentContact) {
            // do not show info if no contact selected
            if (currentContact != null) {
                Contact guiWindowContact = (Contact)GUIWindowManager.GetWindow((int)Helper.PLUGIN_WINDOW_IDS.WINDOW_ID_CONTACT);
                if (guiWindowContact != null) {
                    guiWindowContact.CurrentVcard = currentContact.GetContactVcard(Helper.JABBER_CLIENT);
                    GUIWindowManager.ActivateWindow(guiWindowContact.GetID);
                }
            }
        }

        /// <summary>
        /// Show message containing a question (mark).
        /// The user can answer with yes or no.
        /// </summary>
        /// <param name="msg"></param>
        private void showQuestion(Message msg) {
            GUIDialogYesNo dialog = (GUIDialogYesNo)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_YES_NO);
            dialog.Reset();
            dialog.SetHeading(String.Format("Message from {0}@{1}:", msg.From.User, msg.From.Server));
            dialog.SetLine(1, msg.Body);
            dialog.SetDefaultToYes(true);
            dialog.DoModal(GUIWindowManager.ActiveWindow);

            Helper.JABBER_CLIENT.SendMessage((dialog.IsConfirmed) ? "Yes" : "No", msg.From);
        }

        /// <summary>
        /// Show a message
        /// </summary>
        /// <param name="msg"></param>
        private void showMessage(Message msg) {
            Regex lineCounter = new Regex("\n", RegexOptions.Multiline);
            MatchCollection lines = lineCounter.Matches(msg.Body);

            // Use a DialogText if there are more than 4 lines of text
            if (lines.Count < 4) {
                GUIDialogOK dialog = (GUIDialogOK)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_OK);
                dialog.Reset();
                dialog.SetHeading(String.Format("Message from {0}@{1}:", msg.From.User, msg.From.Server));
                dialog.SetLine(1, msg.Body);
                dialog.DoModal(GUIWindowManager.ActiveWindow);
            } else {
                GUIDialogText dialog = (GUIDialogText)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_TEXT);
                dialog.Reset();
                dialog.SetHeading(String.Format("Message from {0}@{1}:", msg.From.User, msg.From.Server));
                dialog.SetText(msg.Body);
                dialog.DoModal(GUIWindowManager.ActiveWindow);
            }
        }

        private void BuildRoster() {
            List<RosterContact> test = Helper.JABBER_CLIENT.Roster.GetOnlineContacts();
            if (test.Count > 0) {
                Vcard vcard = test[1].GetContactVcard(Helper.JABBER_CLIENT);
            }
        }

        #endregion

        #region ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ Events & Delegates ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

        #endregion

        #region ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ EventHandlers ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

        void JabberClient_OnLogin(object sender) {
            // Once Connected to Jabber keep 'em Messages/Presences pumpin'!
            //Settings.NotifyOnMessage = true;
            Settings.NotifyOnPresence = true;            
            //ShowNotifyDialog("MyChitChat loaded...");
        }

        void JabberClient_OnRosterStart() {
            if (Helper.PLUGIN_WINDOW_ACTIVE) {
                GUIWaitCursor.Show();
            }
        }

        void JabberClient_OnRosterEnd() {
            if (Helper.PLUGIN_WINDOW_ACTIVE) {
                GUIWaitCursor.Hide();
            }        
        }

        protected override void OnShowContextMenu() {
            BuildRoster();
            base.OnShowContextMenu();
        }

        void JabberClient_OnRosterItem(object sender, Jid jid) {

        }

        void JabberClient_OnPresence(object sender, Presence pres) {
            if (Helper.SHOULD_NOTIFY_PRESENCE && Helper.JABBER_CLIENT.Roster.Count > 0 && pres.From.User != null && (pres.Type == PresenceType.available || pres.Type == PresenceType.invisible)) {
                RosterContact temp = Helper.JABBER_CLIENT.Roster.GetRosterContact(pres.From);
                Vcard t = temp.GetContactVcard(Helper.JABBER_CLIENT);
                //temp.Presence = pres;
                string user = temp.Nickname;
                string status = Helper.GetFriendlyPresenceState<Helper.JABBER_PRESENCE_STATES>((Helper.JABBER_PRESENCE_STATES)temp.Presence.Show);
                if (!String.IsNullOrEmpty(pres.Status)) {
                    status += "\n\"" + pres.Status + "\"";
                }
                if (!String.IsNullOrEmpty(pres.From.Resource)) {
                    status += "\n@" + pres.From.Resource;
                }
                
                ShowNotifyDialog(user, GUIGraphicsContext.Skin + @"\Media\MyChitChat_contacts.png", status);
            }
        }

        void JabberClient_OnMessage(object sender, agsXMPP.protocol.client.Message msg) {
            if (Helper.SHOULD_NOTIFY_MESSAGE) {
                //ShowNotifyDialog(msg.From.User.Replace("%", "@"), GUIGraphicsContext.Skin + @"\Media\MyChitChat_incoming_message.png", msg.Body);
                showMessage(msg);
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
