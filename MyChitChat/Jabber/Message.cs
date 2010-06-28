using System;
using agsXMPP;
using agsXMPP.protocol.client;
using agsXMPP.protocol.extensions.chatstates;
using MyChitChat.Plugin;
using System.Collections.Generic;
using MediaPortal.GUI.Library;

namespace MyChitChat.Jabber {

    public enum DirectionTypes {
        Incoming,
        Outgoing
    }

    public class Message : GUIListItem {

        #region ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ Constructor & Initialization ~~~~~~~~~~~~~~~~~~~~~~

        public Message(agsXMPP.protocol.client.Message msg, DirectionTypes directionType, DateTime receivedTime) {
            this.InternalMessage = msg;
            this.DateTimeReceived = receivedTime;
            this.DirectionType = directionType;
            this.Unread = (directionType == DirectionTypes.Incoming);
            this.MessageID = Guid.NewGuid();
            this.InternalMessage = msg;
            this.Replied = false;           
            base.Path = MessageID.ToString();
            UpdateItemInfo();
            base.OnItemSelected += new ItemSelectedHandler(Message_OnItemSelected);
        }

        #endregion

        #region ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ Properties Gets/Sets ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

        private agsXMPP.protocol.client.Message InternalMessage { get; set; }
        public Jid FromJID { get { return this.InternalMessage.From; } }
        public Jid ToJID { get { return this.InternalMessage.To; } }
        public String Subject { get { return (this.InternalMessage.Subject != null) ? this.InternalMessage.Subject : this.InternalMessage.Body; } }
        public String Body { get { return this.InternalMessage.Body; } }
        public String Error { get { return this.InternalMessage.Error.Message; } }
        public MessageType MessageType { get { return this.InternalMessage.Type; } }
        public DirectionTypes DirectionType { get; private set; }
        public string DirectionTypeSymbol {
            get { return (DirectionType == DirectionTypes.Incoming) ? "=>" : "<="; }
        }
        
        public Chatstate ChatState { get { return this.InternalMessage.Chatstate; } }
        public DateTime DateTimeReceived { get; private set; }
        public Guid MessageID { get; private set; }
        public bool Unread { get; set; }
        public bool Replied { get; set; }

        #endregion

        #region ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ Public Methods ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        #endregion

        #region ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ Private Methods ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

        private void UpdateItemInfo() {
            base.FileInfo = new MediaPortal.Util.FileInformation();
            base.FileInfo.CreationTime = DateTimeReceived;
            this.Label = this.Subject;
            this.Label2 = this.DateTimeReceived.ToShortTimeString();
            this.IsPlayed = !this.Unread;
            this.IsRemote = this.Unread;
            UpdateMessageIcon(); 
        }

        private void UpdateMessageIcon() {
            string iconPath = String.Empty;
            if (DirectionType == DirectionTypes.Outgoing) {
                iconPath = Helper.MEDIA_ICON_MESSAGE_OUT;
            } else if(Replied){
                iconPath = Helper.MEDIA_ICON_MESSAGE_IN_REPLIED;
            } else if (Unread) {
                iconPath = Helper.MEDIA_ICON_MESSAGE_IN_UNREAD;
            } else {
                iconPath = Helper.MEDIA_ICON_MESSAGE_IN_READ;
            }
            base.IconImage = base.IconImageBig = iconPath;
        }

        #endregion

        #region ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ Override Methods ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

        public override string ToString() {
            return String.Format("[{0}] {1}", this.DateTimeReceived.ToShortTimeString(), this.Subject);
        }

        #endregion



        #region IComparable Member
        public int CompareTo(object obj) {
            return DateTime.Compare(this.DateTimeReceived, ((Message)obj).DateTimeReceived);
        }
        #endregion

        #region ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ EventHandlers ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

        void Message_OnItemSelected(GUIListItem item, GUIControl parent) {
            if (item.Path == this.Path) {
                this.Unread = false;
            }
        }

        #endregion
    }


    #region ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ Helper Classes ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

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

    #endregion
}
