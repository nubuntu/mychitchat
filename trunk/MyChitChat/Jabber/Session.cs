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


namespace MyChitChat.Jabber {

    public delegate void OnChatSessionUpdatedEventHandler(Session session, Message msg);

    public class Session {


        #region ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ Skin Controls ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

        #endregion

        #region ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ Properties Gets/Sets ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

        public Dictionary<Guid, Message> Messages { get; private set; }
        public Contact Contact { get; private set; }
        public Jid ContactJID { get; private set; }
        public Identity ContactDetails { get { return Contact.identity; } }
        public String ContactNickname { get { return ContactDetails.nickname; } }
        
        public DateTime DateTimeLastActive {
            get {
                return (Messages.Count > 0)
                    ?
                    Messages.Last().Value.DateTimeReceived
                    :
                    DateTimeSessionStarted;
            }
            set { DateTimeLastActive = value; }
        }
        private DateTime DateTimeSessionStarted { get; set; }

        public IEnumerable<MessageListItem> MessageListItems {
            get {
                foreach (KeyValuePair<Guid, Message> currentMessage in Messages) {
                    yield return new MessageListItem(currentMessage.Value);
                }
            }
        }

        #endregion     

        #region ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ Constructor & Initialization ~~~~~~~~~~~~~~~~~~~~~~

        public Session(Contact chatPartner) {
            Contact = chatPartner;
            ContactJID = new Jid(chatPartner.identity.jabberID.full);
            DateTimeSessionStarted = DateTime.Now;
            Messages = new Dictionary<Guid, Message>();
        }

        #endregion
        
        #region ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ Public Methods ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

        public void Reply(string replyMessage) {
            Message sentMsg = Helper.JABBER_CLIENT.SendMessage(replyMessage, ContactJID);
            AddMessageHistory(sentMsg);            
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
            Messages.Add(msg.MessageID, msg);
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
            return String.Format("[{0}] {1}: \"{2}\" {3}", new string[] { DateTimeLastActive.ToShortTimeString(), ContactNickname, Translations.GetByName(Contact.status.type.ToString()), String.Format("[{0} unread]", Messages.Count(x => x.Value.Unread).ToString()) });
        }

        #endregion

    }

    public class SessionListItem : GUIListItem {

        public bool IsActiveSession { get; set; }

        public SessionListItem(Session session, GUIListItem.ItemSelectedHandler callBackItemSelected) {
            this.Path = session.ContactJID.ToString();
            this.Label = session.ToString();
            this.IsActiveSession = session.Contact.status.type != Enums.StatusType.Unvailable;
            this.IconImage = this.IconImageBig = Helper.GetStatusIcon(session.Contact.status.type.ToString());            
            this.OnItemSelected += callBackItemSelected;
        }

        
    }
}
