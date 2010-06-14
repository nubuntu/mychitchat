using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyChitChat.Jabber;
using agsXMPP;
using agsXMPP.Collections;
using nJim;


namespace MyChitChat.Plugin {

    public delegate void OnChatSessionUpdatedEventHandler(Session session, Message msg);

    public class Session {

        private Jid _chatPartnerJID;
        private Client _chatClient;
        private Contact _chatPartner;
        private Dictionary<Guid, Message> _dicMessageHistory;
        private DateTime _dateTimeSessionStarted;

        public Session(Contact chatPartner, Client chatClient) {
            this._chatClient = chatClient;
            this._chatPartner = chatPartner;
            this._chatPartnerJID = new Jid(chatPartner.identity.jabberID.full) ;
            this._dicMessageHistory = new Dictionary<Guid, Message> ();
            this._dateTimeSessionStarted = DateTime.Now;
            this._chatClient.MessageGrabber.Add(this._chatPartnerJID, new BareJidComparer(), new MessageCB(CurrentChatListener), null);
        }
        
        ~Session() {
            this._chatClient.MessageGrabber.Remove(_chatPartnerJID);
			this._chatClient = null;		
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
            set {this.DateTimeLastActive = value;}
        }

        public Dictionary<Guid, Message> Messages {
            get { return this._dicMessageHistory; }
        }

        public void Reply(string replyMessage) {
            Message sentMsg = this._chatClient.SendMessage(replyMessage, this._chatPartnerJID);
            AddMessageHistory(sentMsg);
            OnChatSessionUpdated(this, sentMsg);
        }

        private void CurrentChatListener(object sender, agsXMPP.protocol.client.Message msg, object data) {
            if (msg.Body != null) {
                AddMessageHistory(new Message(msg, DirectionTypes.Incoming, DateTime.Now));
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
    }
}
