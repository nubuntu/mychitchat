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
        private Dictionary<Jid, RosterContact> _dicRosterContacts;
        private List<RosterContact> _onlineContacts;
        private List<RosterContact> _offlineContacts;

        public Roster() {
            this._internalRoster = new RosterControl();
            this._dicRosterContacts = new Dictionary<Jid, RosterContact>();
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
            this._dicRosterContacts.Clear();
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
            if (this._dicRosterContacts.ContainsKey(jid)) {
                return this._dicRosterContacts[jid];            
            }else if (this._internalRoster.Roster.ContainsKey(jid.Bare)) {
                RosterContact tmpContact = new RosterContact(this._internalRoster.Roster[jid.Bare], jid);
                this._dicRosterContacts.Add(jid, tmpContact);
                return tmpContact;
            } else { return null; }
        }

        //public List<RosterContact> GetOnlineContacts() {
        //    this._onlineContacts = new List<RosterContact>();

        //    foreach (KeyValuePair<string, RosterData> currentRosterInfo in this._internalRoster.Roster) {
        //        if (currentRosterInfo.Value.Online && currentRosterInfo.Value.RosterNode.RosterItem.Jid.User != null) {
        //            this._onlineContacts.Add(new RosterContact(currentRosterInfo.Value));
        //        }
        //    }

        //    return this._onlineContacts;
        //}

        //public List<RosterContact> GetOfflineContacts() {
        //    this._offlineContacts = new List<RosterContact>();

        //    foreach (KeyValuePair<string, RosterData> currentRosterInfo in this._internalRoster.Roster) {
        //        if (!currentRosterInfo.Value.Online) {
        //            this._offlineContacts.Add(new RosterContact(currentRosterInfo.Value));
        //        }
        //    }

        //    return this._offlineContacts;
        //}
    }

    public class RosterContact {

        private Jid _internalJid = null;
        private RosterData _internalRosterData = null;
        private Vcard _vcard = null;

        public RosterContact(RosterData rdata, Jid jid) {
            this._internalRosterData = rdata;
            this._internalJid = jid;
            Helper.JABBER_CLIENT.RequestVcard(jid, new IqCB(VcardResult));
        
        }

        private void VcardResult(object sender, IQ iq, object data) {
            if (iq != null && iq.Type == IqType.result && iq.Vcard != null) {
                this.Vcard = iq.Vcard;
            }
        }

        public Jid JID {
            get { return this._internalJid; }
        }

        public String Nickname {
            get { return this._internalRosterData.RosterNode.Text; }
        }

        public String Group {
            get { return this._internalRosterData.Group; }
        }

        public bool Online {
            get { return this._internalRosterData.Online; }
        }

        public String Resource {
            get { return this.JID.Resource; }
        }

        public String Status {
            get {
                if (this._internalRosterData.Presences.ContainsKey(this.JID.ToString())) {
                    return Helper.GetFriendlyPresenceState(this._internalRosterData.Presences[this.JID.ToString()].Presence.Show);
                } else {
                    return "";
                }
            }
        }

        public String StatusMessage {
            get {
                if (this._internalRosterData.Presences.ContainsKey(this.JID.ToString())) {
                    return this._internalRosterData.Presences[this.JID.ToString()].Presence.Status;
                } else { return ""; }
            }
        }

        public ShowType StatusType {
            get {
                if (this._internalRosterData.Presences.ContainsKey(this.JID.ToString())) {
                    return this._internalRosterData.Presences[this.JID.ToString()].Presence.Show;
                } else { return ShowType.xa; }
            }
        }

        public Vcard Vcard {
            get { return this._vcard; }
            set { this._vcard = value; }
        }
    }
}
