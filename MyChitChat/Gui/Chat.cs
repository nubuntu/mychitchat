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
        }
        #endregion

        #region ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ Skin Controls ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        [SkinControlAttribute(50)]
        GUIFacadeControl ctrlFacadeMessageList = null;

        [SkinControlAttribute(700)]
        protected GUITextControl ctrlTextboxCurrentMessage = null;

        #endregion

        #region ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ Skin Properties ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

        private const string TAG_CONTACT_STATUS_TYPE = "#MyChitChat.Contact.Status.Type";
        private const string TAG_CONTACT_STATUS_IMAGE = "#MyChitChat.Contact.Status.Image";
        private const string TAG_CONTACT_STATUS_MESSAGE = "#MyChitChat.Contact.Status.Message";
        private const string TAG_CONTACT_MOOD_TYPE = "#MyChitChat.Contact.Mood.Type";
        private const string TAG_CONTACT_MOOD_IMAGE = "#MyChitChat.Contact.Mood.Image";
        private const string TAG_CONTACT_MOOD_MESSAGE = "#MyChitChat.Contact.Mood.Message";
        private const string TAG_CONTACT_ACTIVITY_TYPE = "#MyChitChat.Contact.Activity.Type";
        private const string TAG_CONTACT_ACTIVITY_IMAGE = "#MyChitChat.Contact.Activity.Image";
        private const string TAG_CONTACT_ACTIVITY_MESSAGE = "#MyChitChat.Contact.Activity.Message";
        private const string TAG_CONTACT_TUNE_TITLE = "#MyChitChat.Contact.Tune.Title";
        private const string TAG_CONTACT_TUNE_MESSAGE = "#MyChitChat.Contact.Tune.Artist";

        #endregion

        #region ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ Override Methods ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        
        // With GetID it will be an window-plugin / otherwise a process-plugin
        // Enter the id number here again
        public override int GetID {
            get { return (int)Helper.PLUGIN_WINDOW_IDS.WINDOW_ID_CHAT; }
        }
        
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
            GUIPropertyManager.SetProperty("#header.text", Helper.PLUGIN_NAME);
            GUIPropertyManager.SetProperty("#header.label", "Chat Window");
        }

        protected override void OnShowContextMenu() {
            base.OnShowContextMenu();
        }

        protected override void OnClicked(int controlId, GUIControl control, MediaPortal.GUI.Library.Action.ActionType actionType) {
            if (control == ctrlFacadeMessageList && actionType == MediaPortal.GUI.Library.Action.ActionType.ACTION_SELECT_ITEM) {
               //Dialog.Instance.ShowNotifyDialog(_currentChatSession.Messages[new Guid(ctrlFacadeMessageList.SelectedListItem.Path)].ToString());
                _currentChatSession.Reply("Copy That: \n" + _currentChatSession.Messages[new Guid(ctrlFacadeMessageList.SelectedListItem.Path)].ToString());
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
                foreach (MessageListItem currentMessageItem in _currentChatSession.MessageListItems) {
                    this.ctrlFacadeMessageList.Add(currentMessageItem);
                }
                this.ctrlFacadeMessageList.Sort(new MessageComparerDateDesc());
            }
        }

        private void UpdateGuiContactProperties(Session selectedSession) {
            GUIPropertyManager.SetProperty(TAG_CONTACT_STATUS_TYPE, Translations.GetByName(selectedSession.Contact.status.type.ToString()));
            GUIPropertyManager.SetProperty(TAG_CONTACT_STATUS_IMAGE, Helper.GetStatusIcon(selectedSession.Contact.status.type.ToString()));
            GUIPropertyManager.SetProperty(TAG_CONTACT_STATUS_MESSAGE, selectedSession.Contact.status.message);
            GUIPropertyManager.SetProperty(TAG_CONTACT_MOOD_TYPE, Translations.GetByName(selectedSession.Contact.mood.type.ToString()));
            GUIPropertyManager.SetProperty(TAG_CONTACT_MOOD_IMAGE, Helper.GetMoodIcon(selectedSession.Contact.mood.type.ToString()));
            GUIPropertyManager.SetProperty(TAG_CONTACT_MOOD_MESSAGE, selectedSession.Contact.mood.text);
            GUIPropertyManager.SetProperty(TAG_CONTACT_ACTIVITY_TYPE, Translations.GetByName(selectedSession.Contact.activity.type.ToString()));
            GUIPropertyManager.SetProperty(TAG_CONTACT_ACTIVITY_IMAGE, Helper.GetActivityIcon(selectedSession.Contact.activity.type.ToString()));
            GUIPropertyManager.SetProperty(TAG_CONTACT_ACTIVITY_MESSAGE, selectedSession.Contact.activity.text);
            GUIPropertyManager.SetProperty(TAG_CONTACT_TUNE_TITLE, selectedSession.Contact.tune.title);
            GUIPropertyManager.SetProperty(TAG_CONTACT_TUNE_MESSAGE, selectedSession.Contact.tune.artist);
        }

        #endregion

        #region ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ Events & Delegates ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

        #endregion

        #region ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ EventHandlers ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

        void _currentChatSession_OnChatSessionUpdated(Session session, Message msg) {
            if (this._currentChatSession.Equals(session)) {
                UpdateChatHistory();               
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
