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

        [SkinControlAttribute(700)]
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
            base.OnWindowLoaded();           
        }

        protected override void OnShowContextMenu() {
            base.OnShowContextMenu();
        }

        protected override void OnClicked(int controlId, GUIControl control, MediaPortal.GUI.Library.Action.ActionType actionType) {
            if (control == ctrlFacadeMessageList && actionType == MediaPortal.GUI.Library.Action.ActionType.ACTION_SELECT_ITEM) {
               Dialog.Instance.ShowNotifyDialog(_currentChatSession.Messages[new Guid(ctrlFacadeMessageList.SelectedListItem.Path)].ToString());
               _currentChatSession.Messages[new Guid(ctrlFacadeMessageList.SelectedListItem.Path)].Unread = false;
            }
        }

        #endregion

        #region ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ Public Methods ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

        #endregion

        #region ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ Private Methods ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        private void UpdateChatHistory() {
            if (this.ctrlFacadeMessageList != null || this._currentChatSession != null) {
                this.ctrlFacadeMessageList.Clear();
                foreach (KeyValuePair<Guid, Message> currentMessage in this._currentChatSession.Messages) {
                    this.ctrlFacadeMessageList.Add(new MessageListItem(currentMessage.Value));
                }
                this.ctrlFacadeMessageList.Sort(new MessageComparerDateDesc());
            }
        }
        #endregion

        #region ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ Events & Delegates ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

        #endregion

        #region ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ EventHandlers ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

        void _currentChatSession_OnChatSessionUpdated(Session session, Message msg) {
            if (this._currentChatSession.Equals(session)) {
                UpdateChatHistory();
                //if (this.ctrlFacadeMessageList != null) {
                //    this.ctrlFacadeMessageList.Add(new MessageListItem(msg));
                //}
                //if (this.ctrlTextboxCurrentMessage != null) {
                //    this.ctrlTextboxCurrentMessage.Visible = true;
                //    this.ctrlTextboxCurrentMessage.Label = msg.ToString();
                //}
            } else {
                Log.Error("Grabbed message from different Chat Session!");
            }
        }

        #endregion

        #region ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ Properties Gets/Sets ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

        public Session CurrentChatSession {
            get { return this._currentChatSession; }
            set {
                if (this._currentChatSession != null) {
                    this._currentChatSession.OnChatSessionUpdated -= new OnChatSessionUpdatedEventHandler(_currentChatSession_OnChatSessionUpdated);
                }
                this._currentChatSession = value;
                this._currentChatSession.OnChatSessionUpdated += new OnChatSessionUpdatedEventHandler(_currentChatSession_OnChatSessionUpdated);
                UpdateChatHistory();
            }
        }

        
        #endregion
    }

   
}
