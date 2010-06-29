﻿using System;
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

    public class Session : GUIListItem, IEquatable<Session> {

        private static string vCardDir = MediaPortal.Configuration.Config.Dir.Database + @"\" + Helper.PLUGIN_NAME + @"\";

        #region ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ Properties Gets/Sets ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

        public bool IsActiveSession {
            get {
                return Contact.status.type != Enums.StatusType.Unavailable;
            }
        }
        public List<Message> Messages { get; private set; }
        public Contact Contact { get; private set; }
        public Jid ContactJID { get; private set; }
        public Identity ContactDetails {
            get {
                return Contact.identity;
            }
            private set {
                Contact.identity = value;
            }
        }
        public String ContactNickname {
            get {
                return (ContactDetails != null || String.IsNullOrEmpty(ContactDetails.nickname)) ? ContactDetails.nickname : ContactJID.User;
            }            
        }

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
            if (chatPartner.identity != null && chatPartner.identity.photo != null) {
                Cache.SaveAvatarImage(chatPartner.identity);
            }
            chatPartner.identity.NicknameUpdated += new IdentityHandler(identity_NicknameUpdated);
            chatPartner.identity.identityRetrieved += new IdentityHandler(identity_identityRetrieved);
            chatPartner.identity.retrieve();
           
            DateTimeSessionStarted = DateTime.Now;
            Messages = new List<Message>();
            this.Path = ContactJID.ToString();
            UpdateItemInfo();
        }

      
        #endregion

        #region ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ Public Methods ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

        public bool Reply(string replyMessage) {
            if (String.IsNullOrEmpty(replyMessage)) {
                replyMessage = Dialog.Instance.GetKeyBoardInput();
            }
            if (!String.IsNullOrEmpty(replyMessage)) {
                AddMessageHistory(Helper.JABBER_CLIENT.SendMessage(replyMessage, ContactJID));
                return true;
            }
            return false;
        }

        public bool Reply() {
            return Reply(String.Empty);
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

        public Message GetMessage(string messageGuid) {
            return Messages.Find(message => message.MessageID.ToString().Equals(messageGuid));
        }

        #endregion

        #region ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ Private Methods ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

        public void UpdateItemInfo() {
            string tmpAvatarImage = Cache.GetAvatarImagePath(ContactDetails);
            if (!String.IsNullOrEmpty(tmpAvatarImage)) {
                this.IconImage = this.IconImageBig = tmpAvatarImage;
            } else {
                this.IconImage = this.IconImageBig = Helper.GetStatusIcon(Contact.status.type.ToString());
            }
            
            this.Label = ContactNickname;
            this.Label2 = String.Format("[{0}/{1}]", Messages.Count(msg => msg.Unread).ToString(), Messages.Count);
            this.IsPlayed = this.IsActiveSession;
            this.IsRemote = !this.IsActiveSession;
            this.IsDownloading = Messages.Count > 0;
        }

        private void AddMessageHistory(Message msg) {
            Messages.Insert(0,msg);
            UpdateItemInfo();
            OnChatSessionUpdated(this, msg);
        }

        private string GetVCardFilePath() {
            return String.Format(@"{0}vCard_{1}.xml", vCardDir, this.ContactJID.GetHashCode());
        }

        #endregion

        #region ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ Events & Delegates ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

        public event OnChatSessionUpdatedEventHandler OnChatSessionUpdated;

        #endregion

        #region ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ EventHandlers ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

        void identity_identityRetrieved(Identity sender) {
            //this.ContactDetails = sender;
            //ContactDetails.Save(GetVCardFilePath());
            Cache.SaveAvatarImage(ContactDetails);
        }

        void identity_NicknameUpdated(Identity sender) {
           
        }


        #endregion

        #region ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ Override Methods ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

        public override string ToString() {
            return String.Format("{0} [{1}/{2}]", ContactNickname, Messages.Count(msg => msg.Unread).ToString(), Messages.Count);
        }

        #endregion


        #region IEquatable<Session> Member

        public bool Equals(Session other) {
            return String.Equals(this.ContactJID.ToString(), other.ContactJID.ToString(), StringComparison.CurrentCultureIgnoreCase);
        }

        #endregion
    }
}
