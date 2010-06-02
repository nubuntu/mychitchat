using System;
using agsXMPP;
using agsXMPP.protocol.client;
using agsXMPP.protocol.extensions.chatstates;
using MyChitChat.Plugin;
using System.Collections.Generic;

namespace MyChitChat.Jabber {


    public class Message {
        private agsXMPP.protocol.client.Message _internalMessage;
        private DateTime _dateTimeReceived;
        private MessageTypes _messageType;

        public Message(agsXMPP.protocol.client.Message msg, MessageTypes messageType, DateTime receivedTime) {
            this._internalMessage = msg;
            this._dateTimeReceived = receivedTime;
            this._messageType = messageType;
        }
        public Jid FromJID { get { return this._internalMessage.From; } }
        public Jid ToJID { get { return this._internalMessage.To; } }
        public String Subject { get { return this._internalMessage.Subject; } }
        public String Body { get { return this._internalMessage.Body; } }
        public String Error { get { return this._internalMessage.Error.Message; } }
        public String FromNickname {
            get {
                RosterContact tmpContact = Helper.JABBER_CLIENT.Roster.GetRosterContact(this.FromJID);
                return (tmpContact != null) ? tmpContact.Nickname : this.FromJID.Bare;
            }
        }
        public MessageType MessageType { get { return this._internalMessage.Type; } }
        public Chatstate ChatState { get { return this._internalMessage.Chatstate; } }
        public DateTime DateTimeReceived { get { return this._dateTimeReceived; } }


        #region IComparable Member
        public int CompareTo(object obj) {
            return DateTime.Compare(this.DateTimeReceived, ((Message)obj).DateTimeReceived);
        }
        #endregion

    }

    public enum MessageTypes {
        Incoming,
        Outgoing
    }


    /// <summary>Implements ascending sort algorithm</summary>
    class MessageComparerDateAsc : IComparer<Message> {
        #region IComparer<Message> Member

        public int Compare(Message x, Message y) {
            return DateTime.Compare(x.DateTimeReceived, y.DateTimeReceived);
        }

        #endregion
    }

    class MessageComparerDateDesc : IComparer<Message> {
        #region IComparer<Message> Member

        public int Compare(Message x, Message y) {
            return DateTime.Compare(y.DateTimeReceived, x.DateTimeReceived);
        }

        #endregion
    }
}
