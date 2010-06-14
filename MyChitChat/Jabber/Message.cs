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
        private DirectionTypes _directionType;
        private bool _unread;
        private Guid _messagID;

        public Message(agsXMPP.protocol.client.Message msg, DirectionTypes directionType, DateTime receivedTime) {
            this._internalMessage = msg;
            this._dateTimeReceived = receivedTime;
            this._directionType = directionType;
            this._unread = (directionType == DirectionTypes.Incoming);
            this._messagID = new Guid();
        }
        public Jid FromJID { get { return this._internalMessage.From; } }
        public Jid ToJID { get { return this._internalMessage.To; } }
        public String Subject { get { return (!String.IsNullOrEmpty( this._internalMessage.Subject)) ?  this._internalMessage.Subject : this._internalMessage.Body; } }
        public String Body { get { return this._internalMessage.Body; } }
        public String Error { get { return this._internalMessage.Error.Message; } }       
        public MessageType MessageType { get { return this._internalMessage.Type; } }
        public DirectionTypes DirectionType { get { return this._directionType; } }
        public Chatstate ChatState { get { return this._internalMessage.Chatstate; } }
        public DateTime DateTimeReceived { get { return this._dateTimeReceived; } }
        public Guid MessageID { get { return this._messagID; } }
        public bool Unread {
            get { return this._unread; }
            set { this._unread = value; }
        }

        #region IComparable Member
        public int CompareTo(object obj) {
            return DateTime.Compare(this.DateTimeReceived, ((Message)obj).DateTimeReceived);
        }
        #endregion

        public override string ToString() {
            return String.Format("[{0}] {1}: \"{2}\"",this.DateTimeReceived.ToShortTimeString(), this.FromJID.Bare, this.Subject);
        }

    }

    public enum DirectionTypes {
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
