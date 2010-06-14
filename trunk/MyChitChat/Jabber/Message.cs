using System;
using agsXMPP;
using agsXMPP.protocol.client;
using agsXMPP.protocol.extensions.chatstates;
using MyChitChat.Plugin;
using System.Collections.Generic;
using MediaPortal.GUI.Library;

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
            this._messagID = Guid.NewGuid();
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
            return String.Format("[{0}] {1}",this.DateTimeReceived.ToShortTimeString(), this.Subject);
        }

    }

    public class MessageListItem : GUIListItem {

        private Message _internalMessage = null;

        public MessageListItem(Message msg) {
            this._internalMessage = msg;
            this.Path = msg.MessageID.ToString();
            this.Label = msg.ToString();
            this.FileInfo = new MediaPortal.Util.FileInformation();
            this.FileInfo.CreationTime = msg.DateTimeReceived;
            
            //this.Label2 = msg.ChatState.ToString();
            //this.Label3 = msg.DateTimeReceived.ToShortTimeString();
            this.Shaded = !msg.Unread;
            this.DimColor = 4;
            this.IconImage = this.IconImageBig = (msg.DirectionType == DirectionTypes.Incoming) ? Helper.MEDIA_ICON_INCOMING_MESSAGE : Helper.MEDIA_ICON_OUTGOING_MESSAGE;
        }

        public Jid JID {
            get { return this._internalMessage.FromJID; }
        }

    }

    public enum DirectionTypes {
        Incoming,
        Outgoing
    }
   
    

    /// <summary>Implements ascending sort algorithm</summary>
    class MessageComparerDateAsc : IComparer<GUIListItem> {
        #region IComparer<GUIListItem> Member

        public int Compare(GUIListItem x, GUIListItem y) {
            return DateTime.Compare(x.FileInfo.CreationTime, y.FileInfo.CreationTime);
        }

        #endregion
    }

    class MessageComparerDateDesc : IComparer<GUIListItem> {
        #region IComparer<GUIListItem> Member

        public int Compare(GUIListItem x, GUIListItem y) {
            return DateTime.Compare(y.FileInfo.CreationTime, x.FileInfo.CreationTime);
        }

        #endregion
    }
}
