﻿using System;
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
        private StringBuilder LogMessages { get; set; }

        #endregion

        #region ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ Constructor & Initialization ~~~~~~~~~~~~~~~~~~~~~~
        public Chat(string skinFile)
            : base(skinFile) {
            LogMessages = new StringBuilder();
        }
        #endregion

        #region ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ Skin Controls ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

        [SkinControlAttribute(100)]
        GUIListControl ctrlListMessages = null;

        [SkinControlAttribute(200)]
        protected GUITextControl ctrlTextboxMessageHistory = null;

        [SkinControlAttribute(300)]
        protected GUITextControl ctrlTextboxSelectedMessage = null;

        [SkinControlAttribute(401)]
        protected GUIButtonControl btnNewMessage = null;

        #endregion

        #region ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ Skin Properties ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

        private const string TAG_CONTACT_NAME_NICK = "#MyChitChat.Contact.Name.Nick";
        private const string TAG_CONTACT_AVATAR_IMAGE = "#MyChitChat.Contact.Avatar.Image";
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

        protected override void OnPageLoad() {
            SetupGuiControls();
            if (this._currentChatSession == null) {
                Log.Error(new Exception(Helper.PLUGIN_NAME + " - Error: Chat.OnWindowLoaded() - CurrentChatSeesion empty!"));
            } else {
                UpdateGuiContactProperties();
            }
        }

        protected override void OnShowContextMenu() {
            base.OnShowContextMenu();
        }

        protected override void OnClicked(int controlId, GUIControl control, MediaPortal.GUI.Library.Action.ActionType actionType) {
            if (control == ctrlListMessages && actionType == MediaPortal.GUI.Library.Action.ActionType.ACTION_SELECT_ITEM) {
                HandleChatMessage(_currentChatSession.Messages[ctrlListMessages.SelectedListItemIndex]);
            }
            if (control == btnNewMessage && _currentChatSession != null) {
                _currentChatSession.Reply();
            }
        }

        private void HandleChatMessage(Message selectedMessage) {
            if (ctrlTextboxSelectedMessage != null) {
                ctrlTextboxSelectedMessage.Clear();
                ctrlTextboxSelectedMessage.VerifyAccess();
                if (ctrlTextboxSelectedMessage.CheckAccess()) {
                    ctrlTextboxSelectedMessage.Label = selectedMessage.Body;
                }
            }
            selectedMessage.Unread = false;
            if (_currentChatSession.Reply()) {
                selectedMessage.Replied = true;
            }
            selectedMessage.UpdateItemInfo();
        }

        #endregion

        #region ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ Public Methods ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

        #endregion

        #region ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ Private Methods ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

        private void SetupGuiControls() {
            GUIPropertyManager.SetProperty("#header.label", "Chat Window");
            GUIPropertyManager.SetProperty("#currentmodule", Helper.PLUGIN_NAME);

            GUIPropertyManager.SetProperty(TAG_CONTACT_NAME_NICK, "Contact's Nickname");
            GUIPropertyManager.SetProperty(TAG_CONTACT_STATUS_TYPE, "Contact's Status");
            GUIPropertyManager.SetProperty(TAG_CONTACT_STATUS_MESSAGE, "Contact's Status Message");
            GUIPropertyManager.SetProperty(TAG_CONTACT_MOOD_TYPE, "Contact's Mood");
            GUIPropertyManager.SetProperty(TAG_CONTACT_MOOD_MESSAGE, "Contact's Mood Message");
            GUIPropertyManager.SetProperty(TAG_CONTACT_ACTIVITY_TYPE, "Contact's Activity");
            GUIPropertyManager.SetProperty(TAG_CONTACT_ACTIVITY_MESSAGE, "Contact's Activity Message");

            GUIPropertyManager.SetProperty(TAG_CONTACT_STATUS_IMAGE, Helper.GetStatusIcon(""));
            GUIPropertyManager.SetProperty(TAG_CONTACT_MOOD_IMAGE, Helper.GetMoodIcon(""));
            GUIPropertyManager.SetProperty(TAG_CONTACT_ACTIVITY_IMAGE, Helper.GetActivityIcon(""));

            if (this.ctrlListMessages != null) {
                this.ctrlListMessages.RemoteColor = 0xFFFF6347;
                this.ctrlListMessages.PlayedColor = 0x2090EE90;
                this.ctrlListMessages.DownloadColor = 0xFF90EE90;
                this.ctrlListMessages.ShadedColor = 0xffff00000;
            }
        }

        private void AppendLogEvent(DateTime when, string why, string who, string what) {
            string tmp = String.Format("[{0}] {1}: '{3}'", new string[] { when.ToShortTimeString(), why, who, what });
            LogMessages.AppendLine(tmp);
            Log.Info(tmp);
            try {
                if (Helper.PLUGIN_WINDOW_ACTIVE) {
                    if (this.ctrlListMessages != null) {
                        ctrlTextboxMessageHistory.VerifyAccess();
                        if (ctrlTextboxMessageHistory.CheckAccess()) {
                            this.ctrlTextboxMessageHistory.Clear();
                            this.ctrlTextboxMessageHistory.Label = LogMessages.ToString();
                        }
                    }
                }
            } catch (Exception e) {
                Log.Error(e);
            }
        }

        private void UpdateChatHistory() {
            if (this.ctrlListMessages != null || this._currentChatSession != null) {
                this.ctrlListMessages.Clear();
                this.LogMessages = new StringBuilder();
                foreach (Message currentMessageItem in _currentChatSession.Messages) {
                    currentMessageItem.UpdateItemInfo();
                    AppendLogEvent(currentMessageItem.DateTimeReceived, currentMessageItem.DirectionTypeSymbol, (currentMessageItem.DirectionType == DirectionTypes.Incoming) ? _currentChatSession.ContactNickname : Helper.JABBER_CLIENT.Identity.nickname, currentMessageItem.Body);
                    this.ctrlListMessages.Add(currentMessageItem);
                }
                //this.ctrlListMessages.Sort(new MessageComparerDateDesc());
            }

        }
       
        private void UpdateGuiContactProperties() {
            if (_currentChatSession != null) {
                GUIPropertyManager.SetProperty(TAG_CONTACT_NAME_NICK, _currentChatSession.ContactDetails.nickname);
                GUIPropertyManager.SetProperty(TAG_CONTACT_STATUS_TYPE, Translations.GetByName(_currentChatSession.Contact.status.type.ToString()));
                GUIPropertyManager.SetProperty(TAG_CONTACT_STATUS_IMAGE, Helper.GetStatusIcon(_currentChatSession.Contact.status.type.ToString()));
                GUIPropertyManager.SetProperty(TAG_CONTACT_STATUS_MESSAGE, _currentChatSession.Contact.status.message);

                GUIPropertyManager.SetProperty(TAG_CONTACT_MOOD_TYPE, Translations.GetByName(_currentChatSession.Contact.mood.type.ToString()));
                GUIPropertyManager.SetProperty(TAG_CONTACT_MOOD_IMAGE, Helper.GetMoodIcon(_currentChatSession.Contact.mood.type.ToString()));
                GUIPropertyManager.SetProperty(TAG_CONTACT_MOOD_MESSAGE, _currentChatSession.Contact.mood.text);
                GUIPropertyManager.SetProperty(TAG_CONTACT_ACTIVITY_TYPE, Translations.GetByName(_currentChatSession.Contact.activity.type.ToString()));
                GUIPropertyManager.SetProperty(TAG_CONTACT_ACTIVITY_IMAGE, Helper.GetActivityIcon(_currentChatSession.Contact.activity.type.ToString()));
                GUIPropertyManager.SetProperty(TAG_CONTACT_ACTIVITY_MESSAGE, _currentChatSession.Contact.activity.text);
                GUIPropertyManager.SetProperty(TAG_CONTACT_TUNE_TITLE, _currentChatSession.Contact.tune.title);
                GUIPropertyManager.SetProperty(TAG_CONTACT_TUNE_MESSAGE, _currentChatSession.Contact.tune.artist);
            }
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
