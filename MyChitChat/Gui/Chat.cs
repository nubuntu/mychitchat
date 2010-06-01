using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MediaPortal.GUI.Library;
using MyChitChat.Plugin;


namespace MyChitChat.Gui {
    class Chat : GUIWindow {

        public Chat() {
            GetID = (int)Helper.PLUGIN_WINDOW_IDS.WINDOW_ID_CHAT;
        }

        private Session _currentChatSession;

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

        public Session CurrentChatSession {
            get { return this._currentChatSession; }
            set {
                this._currentChatSession.OnChatSessionUpdated -= new OnChatSessionUpdatedEventHandler(_currentChatSession_OnChatSessionUpdated);
                this._currentChatSession = value;
                this._currentChatSession.OnChatSessionUpdated += new OnChatSessionUpdatedEventHandler(_currentChatSession_OnChatSessionUpdated);
            }
        }

        void _currentChatSession_OnChatSessionUpdated() {
            Helper.ShowNotifyDialog(this._currentChatSession.ListMessages.Count.ToString());
        }
    }
}
