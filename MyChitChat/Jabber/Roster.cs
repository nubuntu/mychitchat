using System;
using System.Collections.Generic;
using System.Text;
using agsXMPP;
using agsXMPP.ui.roster;
using agsXMPP.protocol.iq.roster;
using agsXMPP.protocol.client;
using agsXMPP.protocol.iq.vcard;
using System.Threading;


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

        public List<RosterContact> GetOnlineContacts() {
            this._onlineContacts = new List<RosterContact>();

            foreach (KeyValuePair<string, RosterData> currentRosterInfo in this._internalRoster.Roster) {
                if (currentRosterInfo.Value.Online) {
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
        private AutoResetEvent _vCardReceivedEvent = new AutoResetEvent(false);
        private object locker = new object(); 

        public RosterContact(RosterData rdata) {
            this._internalRosterData = rdata;
        }

        public Jid JID {
            get { return _internalRosterData.RosterNode.RosterItem.Jid; }
        }

        public String Nickname {
            get { return _internalRosterData.RosterNode.RosterItem.Name; }
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

        public ShowType Presence {
            get { return _internalRosterData.RosterNode.Presence.Show; }
        }

        public String Status {
            get { return _internalRosterData.RosterNode.Presence.Status; }
        }

        public Vcard GetContactVcard(Client jabberClient) {
            return (_vcard != null) ? _vcard : this.GetContactVcardIq(jabberClient);

        }

        private Vcard GetContactVcardIq(Client jabberClient) {
            jabberClient.RequestVcard(this.JID, new IqCB(VcardResult));
            _vCardReceivedEvent.WaitOne(30000, false);
            lock (locker) {
                return _vcard;
            }
        }

        private void VcardResult(object sender, IQ iq, object data) {            
            if (iq.Type == IqType.result) {
                lock (locker) {
                    this._vcard = iq.Vcard;
                }
                _vCardReceivedEvent.Set();
            }
        }
    }
}
