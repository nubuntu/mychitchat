using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MediaPortal.Dialogs;
using MediaPortal.GUI.Library;
using MyChitChat.Plugin;

namespace MyChitChat.Gui {
    public class Contact : GUIListItem {

        private nJim.Contact _jabberContact = null;

        public Contact( nJim.Contact jabberContact) {
            this._jabberContact = jabberContact;
            this.Path = jabberContact.identity.jabberID.full;
            this.Label = jabberContact.identity.nickname;
            this.Label2 = Translations.GetByName(jabberContact.status.type.ToString());
            this.Label3 = Translations.GetByName(jabberContact.activity.type.ToString());
            this.IconImage = this.IconImageBig = Helper.GetStatusIcon(jabberContact.status.type.ToString());
            

        }

        
        public new nJim.JabberID ItemId;

        public nJim.JabberID JID {
            get { return this.ItemId; }
        }

    }
}
