using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MediaPortal.GUI.Library;
using MyChitChat.Plugin;
using MyChitChat.Jabber;
using agsXMPP;


namespace MyChitChat.Gui {
    class Chat : GUIWindow {

        #region ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ Member Fields ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        private Session _currentChatSession;

        #endregion

        #region ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ Constructor & Initialization ~~~~~~~~~~~~~~~~~~~~~~
        public Chat() {
            GetID = (int)Helper.PLUGIN_WINDOW_IDS.WINDOW_ID_CHAT;
        }
        #endregion

        #region ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ Skin Controls ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        [SkinControlAttribute(50)]
        GUIFacadeControl ctrlFacadeMessageList = null;

        [SkinControlAttribute(600)]
        protected GUITextControl ctrlTextboxCurrentMessage = null;

        #endregion

        #region ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ Override Methods ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        /// <summary>
        /// Loads the XML for the window
        /// </summary>
        public override bool Init() {
            return Load(Helper.SKIN_FILE_CHAT);
        }

        protected override void OnWindowLoaded() {
            if (this._currentChatSession == null) {
                Log.Error(new Exception(Helper.PLUGIN_NAME + " - Error: Chat.OnWindowLoaded() - CurrentChatSeesion empty!"));
            }
            foreach (KeyValuePair<Guid, Message> currentMessage in this._currentChatSession.Messages) {
                ctrlFacadeMessageList.Add(new MessageListItem(currentMessage.Value));
            }
            base.OnWindowLoaded();
        }

        protected override void OnClicked(int controlId, GUIControl control, MediaPortal.GUI.Library.Action.ActionType actionType) {
            if (control == ctrlFacadeMessageList && actionType == MediaPortal.GUI.Library.Action.ActionType.ACTION_SELECT_ITEM) {
                ctrlTextboxCurrentMessage.Label = _currentChatSession.Messages[new Guid(ctrlFacadeMessageList.SelectedListItem.Path)].ToString();
            }
        }

        #endregion

        #region ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ Public Methods ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

        #endregion

        #region ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ Private Methods ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

        #endregion

        #region ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ Events & Delegates ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

        #endregion

        #region ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ EventHandlers ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

        void _currentChatSession_OnChatSessionUpdated(Session session, Message msg) {
            if (this._currentChatSession.Equals(session)) {
                if (Settings.notifyOnMessagePlugin) {
                    Dialog.Instance.ShowNotifyDialog(this._currentChatSession.Messages.Count.ToString());
                }
                this.ctrlFacadeMessageList.Add(new MessageListItem(msg));
                this.ctrlTextboxCurrentMessage.Label = msg.ToString();
            } else {
                Log.Error("Grabbed message from different Chat Session!");
            }
        }

        #endregion

        #region ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ Properties Gets/Sets ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

        public Session CurrentChatSession {
            get { return this._currentChatSession; }
            set {
                this._currentChatSession.OnChatSessionUpdated -= new OnChatSessionUpdatedEventHandler(_currentChatSession_OnChatSessionUpdated);
                this._currentChatSession = value;
                this._currentChatSession.OnChatSessionUpdated += new OnChatSessionUpdatedEventHandler(_currentChatSession_OnChatSessionUpdated);
            }
        }
        #endregion
    }

    public class MessageListItem : GUIListItem {

        private Message _internalMessage = null;

        public MessageListItem(Message msg) {
            this._internalMessage = msg;
            this.Path = msg.MessageID.ToString();
            this.Label = (!String.IsNullOrEmpty(msg.Subject)) ? msg.Subject : msg.Body;
            this.Label2 = msg.ChatState.ToString();
            this.Label3 = msg.DateTimeReceived.ToShortTimeString();
            this.Shaded = !msg.Unread;
            this.IconImage = this.IconImageBig = (msg.DirectionType == DirectionTypes.Incoming) ? Helper.MEDIA_ICON_INCOMING_MESSAGE : Helper.MEDIA_ICON_OUTGOING_MESSAGE;
        }

        public Jid JID {
            get { return this._internalMessage.FromJID; }
        }

    }
}
