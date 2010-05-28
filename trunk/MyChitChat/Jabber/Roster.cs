using System;
using System.Collections.Generic;
using System.Text;
using agsXMPP;
using agsXMPP.ui.roster;
using agsXMPP.protocol.iq.roster;
using agsXMPP.protocol.client;

namespace MyChitChat.Jabber {
    class Roster : IRosterControl{

        private RosterControl _internalRoster;

        public Roster() {
            this._internalRoster = new RosterControl();
        }

        #region IRosterControl Member

        public RosterNode AddRosterItem(RosterItem ritem) {
            throw new NotImplementedException();
        }

        public void Clear() {
            throw new NotImplementedException();
        }

        public bool RemoveRosterItem(RosterItem ritem) {
            throw new NotImplementedException();
        }

        public bool RemoveRosterItem(Jid jid) {
            throw new NotImplementedException();
        }

        public void SetPresence(Presence pres) {
            throw new NotImplementedException();
        }

        #endregion
    }
}
