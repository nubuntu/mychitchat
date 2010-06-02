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

        public Roster() {
            this._internalRoster = new RosterControl();
            this._dicRosterContacts = new Dictionary<Jid, RosterContact>();           
        }

        Dictionary<Jid, RosterContact> RosterContacts {
            get { return this._dicRosterContacts; }
        }

        public int Count {
            get { return this._internalRoster.Roster.Count; }
        }

        #region IRosterControl Member

        public RosterNode AddRosterItem(RosterItem ritem) {
            RosterNode tmpNode = this._internalRoster.AddRosterItem(ritem);
            Jid tmpJid = tmpNode.RosterItem.Jid;
            if (this._internalRoster.Roster.ContainsKey(tmpJid.Bare)) {
                RosterContact tmpContact = new RosterContact(tmpJid, this._internalRoster.Roster[tmpJid.Bare]);
                this._dicRosterContacts.Add(tmpContact.JID, tmpContact);
            }
            return tmpNode;
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
            return (this._dicRosterContacts.ContainsKey(jid)) ? this._dicRosterContacts[jid] : null;
        }

        public List<RosterContact> GetRosterContacts(bool? onlineContactsOnly) {
            return this._dicRosterContacts.Values.Where(
                    contact => !onlineContactsOnly.HasValue || contact.Online == onlineContactsOnly.Value
                )
                .ToList<RosterContact>();
        }
    }

    public class RosterContact {

        private Jid _internalJid = null;
        private RosterData _internalRosterData = null;
        private Vcard _vcard = null;

        public RosterContact(Jid jid, RosterData rdata) {
            this._internalJid = jid;
            this._internalRosterData = rdata;
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
                    return Helper.GetFriendlyPresenceState((Helper.JABBER_PRESENCE_STATES)this._internalRosterData.Presences[this.JID.ToString()].Presence.Show);
                } else {
                    return String.Empty;
                }
            }
        }

        public String StatusMessage {
            get {
                if (this._internalRosterData.Presences.ContainsKey(this.JID.ToString())) {
                    return this._internalRosterData.Presences[this.JID.ToString()].Presence.Status;
                } else { return String.Empty; }
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
