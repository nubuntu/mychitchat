using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyChitChat.Jabber;
using agsXMPP;
using agsXMPP.Collections;
using nJim;
using MediaPortal.GUI.Library;
using MyChitChat.Plugin;
using MyChitChat.Gui;


namespace MyChitChat.Jabber {

    public delegate void OnChatSessionUpdatedEventHandler(Session session, Message msg);

    public class Session : GUIListItem, IEquatable<Session>{

        #region ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ Properties Gets/Sets ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

        public bool IsActiveSession { 
            get {
                return Contact.status.type != Enums.StatusType.Unvailable;
            }
        }
        public List<Message> Messages { get; private set; }
        public Contact Contact { get; private set; }
        public Jid ContactJID { get; private set; }
        public Identity ContactDetails { get { return Contact.identity; } }
        public String ContactNickname { get { return ContactDetails.nickname; } }

        public DateTime DateTimeLastActive {
            get {
                return (Messages.Count > 0)
                    ?
                    Messages.Last().DateTimeReceived
                    :
                    DateTimeSessionStarted;
            }
            set { DateTimeLastActive = value; }
        }
        private DateTime DateTimeSessionStarted { get; set; }        

        #endregion

        #region ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ Constructor & Initialization ~~~~~~~~~~~~~~~~~~~~~~

        public Session(Contact chatPartner) {
            Contact = chatPartner;
            ContactJID = new Jid(chatPartner.identity.jabberID.full);
            DateTimeSessionStarted = DateTime.Now;
            Messages = new List<Message>();
            this.Label = ToString();
            this.Path = ContactJID.ToString();
            this.IconImage = this.IconImageBig = Helper.GetStatusIcon(Contact.status.type.ToString());
        }

        #endregion

        #region ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ Public Methods ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

        public void UpdateItemImage() {
            this.IconImage = this.IconImageBig = Helper.GetStatusIcon(Contact.status.type.ToString());
        }

        public void Reply(string replyMessage) {
            if (String.IsNullOrEmpty(replyMessage)) {
                replyMessage = Dialog.Instance.GetKeyBoardInput();
            }            
            AddMessageHistory(Helper.JABBER_CLIENT.SendMessage(replyMessage, ContactJID));
        }

        public void Reply() {
            Reply(String.Empty);
        }

        public void AddMessage(Message msg) {
            if (msg.Body != null && String.Compare(ContactJID.Bare, msg.FromJID.Bare, true) == 0) {
                AddMessageHistory(msg);
            }
        }

        public void ClearHistory() {
            if (Messages != null) {
                Messages.Clear();
            }
            DateTimeLastActive = DateTime.Now;
        }

        #endregion

        #region ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ Private Methods ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

        private void AddMessageHistory(Message msg) {
            Messages.Add(msg);
            this.Label = ToString();
            OnChatSessionUpdated(this, msg);
        }

        #endregion

        #region ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ Events & Delegates ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

        public event OnChatSessionUpdatedEventHandler OnChatSessionUpdated;

        #endregion

        #region ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ EventHandlers ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

        #endregion

        #region ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ Override Methods ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

        public override string ToString() {
            return String.Format("{0} [{1}/{2}]", ContactNickname, Messages.Count(msg => msg.Unread).ToString(), Messages.Count);
        }

        #endregion


        #region IEquatable<Session> Member

        public bool Equals(Session other) {
            return String.Equals( this.ContactJID.ToString(), other.ContactJID.ToString(), StringComparison.CurrentCultureIgnoreCase);
        }

        #endregion
    }  
}
