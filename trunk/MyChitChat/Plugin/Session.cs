using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyChitChat.Jabber;
using agsXMPP;
using agsXMPP.Collections;
using nJim;
using MediaPortal.GUI.Library;


namespace MyChitChat.Plugin {

    public delegate void OnChatSessionUpdatedEventHandler(Session session, Message msg);

    public class Session {

        private Jid _chatPartnerJID;
        private Contact _chatPartner;
        private Dictionary<Guid, Message> _dicMessageHistory;
        private DateTime _dateTimeSessionStarted;

        public Session(Contact chatPartner, Client chatClient) {
            this._chatPartner = chatPartner;
            this._chatPartnerJID = new Jid(chatPartner.identity.jabberID.full);
            this._dicMessageHistory = new Dictionary<Guid, Message>();
            this._dateTimeSessionStarted = DateTime.Now;
            Helper.JABBER_CLIENT.OnMessage += new OnMessageEventHandler(JABBER_CLIENT_OnMessage);
        }

        ~Session() {
        }

        public event OnChatSessionUpdatedEventHandler OnChatSessionUpdated;

        public Jid PartnerJID { get { return this._chatPartnerJID; } }
        public String PartnerNickname { get { return this._chatPartner.identity.nickname; } }
        public Contact Contact { get { return this._chatPartner; } }
        public Identity Identity { get { return this._chatPartner.identity; } }
        public DateTime DateTimeLastActive {
            get {
                return (_dicMessageHistory.Count > 0)
                    ?
                    _dicMessageHistory.Last().Value.DateTimeReceived
                    :
                    _dateTimeSessionStarted;
            }
            set { this.DateTimeLastActive = value; }
        }

        public Dictionary<Guid, Message> Messages {
            get { return this._dicMessageHistory; }
        }

        public void Reply(string replyMessage) {
            Message sentMsg = Helper.JABBER_CLIENT.SendMessage(replyMessage, this._chatPartnerJID);
            AddMessageHistory(sentMsg);
            OnChatSessionUpdated(this, sentMsg);
        }

        void JABBER_CLIENT_OnMessage(Message msg) {
            if (msg.Body != null && String.Compare(this._chatPartnerJID.Bare, msg.FromJID.Bare, true) == 0) {
                AddMessageHistory(msg);
            }
        }

        private void AddMessageHistory(Message msg) {
            _dicMessageHistory.Add(msg.MessageID, msg);
            OnChatSessionUpdated(this, msg);
        }


        //public void SortMessagesAsc() {
        //    this._dicMessageHistory.Sort(new MessageComparerDateAsc());
        //}

        //public void SortMessagesDesc() {
        //    this._dicMessageHistory.Sort(new MessageComparerDateDesc());
        //}

        public void ClearHistory() {
            this._dicMessageHistory.Clear();
            this.DateTimeLastActive = DateTime.Now;
        }

        public override string ToString() {
            return String.Format("[{0}] {1}: \"{2}\" {3}", new string[] { this.DateTimeLastActive.ToShortTimeString(), this.PartnerNickname,Translations.GetByName(this.Contact.status.type.ToString()), String.Format("[{0} unread]", this.Messages.Count(x => x.Value.Unread).ToString())});
        }
    }

    public class SessionListItem : GUIListItem {

        private Session _jabberSession = null;

        public SessionListItem(Session session) {
            this._jabberSession = session;
            this.Path = session.PartnerJID.ToString();
            this.Label = session.ToString() ;
            //this.Label2 = Translations.GetByName(session.Contact.status.type.ToString());
            ////this.Label3 = Translations.GetByName(session.Contact.activity.type.ToString());
            //this.Label3 = String.Format("[{0} unread]", session.Messages.Count(x => x.Value.Unread));
            this.IconImage = this.IconImageBig = Helper.GetStatusIcon(session.Contact.status.type.ToString());
        }

        public Jid JID {
            get { return this._jabberSession.PartnerJID; }
        }

    }
}
