using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyChitChat.Jabber;
using agsXMPP;
using agsXMPP.Collections;
using nJim;


namespace MyChitChat.Plugin {

    public delegate void OnChatSessionUpdatedEventHandler();

    class Session {

        private Client _chatClient;
        private Identity _chatPartner;
        private List<Message> _listMessageHistory;
        private DateTime _dateTimeSessionStarted;

        public Session(Identity chatPartner, Client chatClient) {
            this._chatClient = chatClient;
            this._chatPartner = chatPartner;
            this._listMessageHistory = new List<Message>();
            this._dateTimeSessionStarted = DateTime.Now;
            this._chatClient.MessageGrabber.Add(new Jid(this._chatPartner.jabberID.full) , new BareJidComparer(), new MessageCB(CurrentChatListener), null);
        }
        
        ~Session() {
            this._chatClient.MessageGrabber.Remove(new Jid(this._chatPartner.jabberID.full));
			this._chatClient = null;		
        }

        public event OnChatSessionUpdatedEventHandler OnChatSessionUpdated;

        public JabberID ChatPartnerJID { get { return this._chatPartner.jabberID; } }
        public String ChatPartnerNickname { get { return this._chatPartner.nickname; } }
        public DateTime DateTimeLastActive {
            get {
                return (_listMessageHistory.Count > 0)
                    ?
                    _listMessageHistory[_listMessageHistory.Count - 1].DateTimeReceived
                    :
                    _dateTimeSessionStarted;
            }
            set {this.DateTimeLastActive = value;}
        }

        public List<Message> ListMessages {
            get { return this._listMessageHistory; }
        }

        public void Reply(string replyMessage) {
            Message sentMsg = this._chatClient.SendMessage(replyMessage, new Jid(this.ChatPartnerJID.full));
            AddMessageHistory(sentMsg);            
        }

        private void CurrentChatListener(object sender, agsXMPP.protocol.client.Message msg, object data) {
            if (msg.Body != null) {
                AddMessageHistory(new Message(msg, MessageTypes.Incoming, DateTime.Now));
            }
        }

        private void AddMessageHistory(Message msg) {
            _listMessageHistory.Add(msg);
            //OnChatSessionUpdated();
        }

       
        public void SortMessagesAsc() {
            this._listMessageHistory.Sort(new MessageComparerDateAsc());
        }

        public void SortMessagesDesc() {
            this._listMessageHistory.Sort(new MessageComparerDateDesc());
        }

        public void ClearHistory() {
            this._listMessageHistory.Clear();
            this.DateTimeLastActive = DateTime.Now;
        }
    }
}
