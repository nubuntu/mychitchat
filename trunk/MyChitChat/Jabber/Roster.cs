using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using agsXMPP;
using agsXMPP.ui.roster;
using agsXMPP.protocol.iq.roster;
using agsXMPP.protocol.client;
using agsXMPP.protocol.iq.vcard;
using System.Threading;
using MyChitChat.Plugin;


namespace MyChitChat.Jabber {
    public class Roster : IRosterControl {

        private RosterControl _internalRoster;
        private List<RosterContact> _onlineContacts;
        private List<RosterContact> _offlineContacts;

        public Roster() {
            this._internalRoster = new RosterControl();
            //this._onlineContacts = new List<RosterContact>();
            //this._offlineContacts = new List<RosterContact>();
        }

        RosterControl InternalRoster {
            get { return this._internalRoster; }
        }

        public int Count {
            get { return this._internalRoster.Roster.Count; }
        }

        #region IRosterControl Member

        public RosterNode AddRosterItem(RosterItem ritem) {
            return this._internalRoster.AddRosterItem(ritem);
        }

        public void Clear() {
            this._internalRoster.Clear();
        }

        public bool RemoveRosterItem(RosterItem ritem) {
            return this._internalRoster.RemoveRosterItem(ritem);
        }

        public bool RemoveRosterItem(Jid jid) {
            return this._internalRoster.RemoveRosterItem(jid);
        }

        public void SetPresence(Presence pres) {
            this._internalRoster.SetPresence(pres);
        }

        #endregion

        public RosterContact GetRosterContact(Jid jid) {
            return (new RosterContact(this._internalRoster.Roster[jid.Bare]));
        }

        public List<RosterContact> GetOnlineContacts() {
            this._onlineContacts = new List<RosterContact>();

            foreach (KeyValuePair<string, RosterData> currentRosterInfo in this._internalRoster.Roster) {
                if (currentRosterInfo.Value.Online && currentRosterInfo.Value.RosterNode.RosterItem.Jid.User != null) {
                    this._onlineContacts.Add(new RosterContact(currentRosterInfo.Value));
                }
            }

            return this._onlineContacts;
        }

        public List<RosterContact> GetOfflineContacts() {
            this._offlineContacts = new List<RosterContact>();

            foreach (KeyValuePair<string, RosterData> currentRosterInfo in this._internalRoster.Roster) {
                if (!currentRosterInfo.Value.Online) {
                    this._offlineContacts.Add(new RosterContact(currentRosterInfo.Value));
                }
            }

            return this._offlineContacts;
        }
    }

    public class RosterContact {

        private RosterData _internalRosterData = null;
        private Vcard _vcard = null;

        public RosterContact(RosterData rdata) {
            this._internalRosterData = rdata;
        }

        public Jid JID {
            get { return _internalRosterData.RosterNode.RosterItem.Jid; }
        }

        public String Nickname {
            get { return _internalRosterData.RosterNode.Text; }
        }

        public String Group {
            get { return _internalRosterData.Group; }
        }

        public bool Online {
            get { return _internalRosterData.Online; }
        }

        public String Resource {
            get { return JID.Resource; }
        }

        public String Status {
            get {
                return Helper.GetFriendlyPresenceState(_internalRosterData.Presences[this.JID.Bare].Presence.Show);
            }
        }

        public String StatusMessage {
            get { return _internalRosterData.Presences[this.JID.Bare].Presence.Status; }
        }

        public ShowType StatusType {
            get {
                return _internalRosterData.Presences[this.JID.Bare].Presence.Show;
            }
        }

        public Vcard Vcard {
            get { return this._vcard; }
            set { this._vcard = value; }
        }
    }
}
