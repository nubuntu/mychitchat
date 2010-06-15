using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using agsXMPP;
using MediaPortal.Dialogs;
using MyChitChat.Jabber;
using MyChitChat.Plugin;
using nJim;
using MediaPortal.GUI.Library;
using MediaPortal.TagReader;


namespace MyChitChat.Gui {
    public class Main : GUIWindow {

        #region ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ Member Fields ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~        
        private Chat guiWindowChat;
                
        #endregion

        #region ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ Skin Controls ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

        [SkinControlAttribute(50)]
        GUIFacadeControl ctrlFacadeContactList = null;

        [SkinControlAttribute(499)]
        protected GUIButtonControl btnSetStatus = null;

        [SkinControlAttribute(500)]
        protected GUIButtonControl btnSetMood = null;

        [SkinControlAttribute(501)]
        protected GUIButtonControl btnSetActivity = null;

        [SkinControlAttribute(502)]
        protected GUIButtonControl btnRoster = null;

        [SkinControlAttribute(700)]
        protected GUITextControl ctrlTextboxMessageHistory = null;

        #endregion

        #region ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ Skin Properties ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        
        private const string TAG_USER_STATUS_TYPE = "#MyChitChat.User.Status.Type";
        private const string TAG_USER_STATUS_MESSAGE = "#MyChitChat.User.Status.Message";
        private const string TAG_USER_MOOD_TYPE = "#MyChitChat.User.Mood.Type";
        private const string TAG_USER_MOOD_MESSAGE = "#MyChitChat.User.Mood.Message";
        private const string TAG_USER_ACTIVITY_TYPE = "#MyChitChat.User.Activity.Type";
        private const string TAG_USER_ACTIVITY_MESSAGE = "#MyChitChat.User.Activity.Message";
        private const string TAG_USER_TUNE_TITLE = "#MyChitChat.User.Tune.Title";
        private const string TAG_USER_TUNE_MESSAGE = "#MyChitChat.User.Tune.Artist";     
        private const string TAG_CALL_STATUS = "#MyChitChat.Call.Status";
        private const string TAG_CALL_DURATION = "#MyChitChat.Call.Duration";
        private const string TAG_CONTACT_NAME = "#MyChitChat.Contact.Name";
        private const string TAG_CONTACT_STATUS = "#MyChitChat.Contact.Status";
        private const string TAG_CONTACT_COUNT = "#MyChitChat.Contact.Count";
        private const string TAG_MISSED_CALLS = "#MyChitChat.Call.Missed";
        private const string TAG_MISSED_CALLS2 = "#MyChitChat.Call.2Missed";
        private const string TAG_MyChitChat_CREDIT = "#MyChitChat.Credit";
        private const string TAG_IMG_STATUS = "#MyChitChat.Image.MyStatus";
        private const string TAG_IMG_AVATAR = "#MyChitChat.Image.Avatar";
        private const string TAG_CONTACT_MOODTEXT = "#MyChitChat.Contact.MoodText";

        #endregion

        #region ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ Constructor & Initialization ~~~~~~~~~~~~~~~~~~~~~~

        public Main() {
            AddRosterEventHandlers();
            
        }
       
        ~Main() {
            try {
                RemoveEventHandlers();
            } catch (Exception ex) {
                Log.Error(ex);
            }
        }
      
        #endregion


        #region ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ Override Methods ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        //public override void OnAction(MediaPortal.GUI.Library.Action action) {
        //    switch ((MediaPortal.GUI.Library.Action.ActionType)action.wID) {
        //        case MediaPortal.GUI.Library.Action.ActionType.ACTION_MUSIC_PLAY:
        //            Helper.JABBER_CLIENT.Presence.setActivity(Enums.ActivityType.relaxing, "Music....");                    
        //            break;
        //        case MediaPortal.GUI.Library.Action.ActionType.ACTION_SHOW_FULLSCREEN:
        //            Helper.JABBER_CLIENT.Presence.status = Helper.GetStatusFromType(Enums.StatusType.DoNotDisturb, "I'm in Fullscreen mode so please knock it off...");
        //            Helper.JABBER_CLIENT.Presence.applyStatus();
        //            break;

        //    }
        //    base.OnAction(action);

        //}
        public override bool Init() {
            guiWindowChat = new Chat();
            GUIWindow window = (GUIWindow)guiWindowChat;
            GUIWindowManager.Add(ref window);
            guiWindowChat.Init();
            if (Settings.autoConnectStartup) {
                Helper.JABBER_CLIENT.Login();
            }
            return Load(Helper.SKIN_FILE_MAIN);
        }

        protected override void OnPageLoad() {
            if (!Helper.JABBER_CLIENT.LoggedIn) {
                Helper.JABBER_CLIENT.Login();
            }
            base.OnPageLoad();
            
        }


        protected override void OnShowContextMenu() {            
            base.OnShowContextMenu();
        }

        protected override void OnWindowLoaded() {            
            base.OnWindowLoaded();
            AddItemSelectionEventHandlers();
            UpdateContactsFacade();
        }

        protected override void OnPreviousWindow() {
            base.OnPreviousWindow();
        }

        protected override void OnClicked(int controlId, GUIControl control, MediaPortal.GUI.Library.Action.ActionType actionType) {
            if (control == btnSetStatus)
                Dialog.Instance.SelectAndSetStatus();
            if (control == btnSetMood)
                Dialog.Instance.SelectAndSetMood();
            if (control == btnSetActivity)
                Dialog.Instance.SelectAndSetActivity();
            if (control == ctrlFacadeContactList && actionType == MediaPortal.GUI.Library.Action.ActionType.ACTION_SELECT_ITEM) {
                try {
                    ShowChatWindow(History.Instance.GetSession(ctrlFacadeContactList.SelectedListItem.Path));
                } catch {
                }
            }
            base.OnClicked(controlId, control, actionType);
        }


        #endregion

        #region ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ Public Methods ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

        #endregion

        #region ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ Private GUI Methods ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

        private void ShowChatWindow(Session currentChatSession) {
            // do not show info if no contact selected
            if (currentChatSession != null) {                 
                if (guiWindowChat != null) {
                    GUIWindowManager.ActivateWindow(guiWindowChat.GetID);
                    guiWindowChat.CurrentChatSession = currentChatSession;
                }              
            }
        }        

        private void UpdateContactsFacade() {
            if (ctrlFacadeContactList != null) {
                ctrlFacadeContactList.Clear();
                try {
                    foreach (SessionListItem currentItem in History.Instance.SessionListItems ) {
                        ctrlFacadeContactList.Add(currentItem);
                    }
                } catch { }
            }
        }

        private void UpdateGuiProperties(Session selectedSession) {
            UIPropertyManager.SetProperty("#"+Helper.PLUGIN_NAME+".Session.");
        }



        #endregion

        #region ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ Private Business Logic Methods ~~~~~~~~~~~~~~~~~~~~




        #endregion

        #region ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ Events & Delegates ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

        private void AddRosterEventHandlers() {
            Helper.JABBER_CLIENT.OnRosterStart += new OnRosterStartEventHandler(JABBER_CLIENT_OnRosterStart);
            Helper.JABBER_CLIENT.OnRosterEnd += new OnRosterEndEventHandler(JABBER_CLIENT_OnRosterEnd);
        }
        
        private void AddHistoryEventHandlers() {
            History.Instance.OnUpdatedRoster += new OnUpdatedRosterEventHandler(History_OnRosterUpdated);
            History.Instance.OnUpdatedPresence += new OnUpdatedPresenceEventHandler(History_OnContactUpdated);
            History.Instance.OnUpdatedSession += new OnUpdatedSessionEventHandler(History_OnUpdatedSession);
        }

        private void AddItemSelectionEventHandlers() {
            History.Instance.OnSessionItemSelected += new OnSessionItemSelectedEventHandler(History_OnSessionItemSelected);
        }
              
        private void RemoveEventHandlers() {
            Helper.JABBER_CLIENT.OnRosterStart -= new OnRosterStartEventHandler(JABBER_CLIENT_OnRosterStart);
            Helper.JABBER_CLIENT.OnRosterEnd -= new OnRosterEndEventHandler(JABBER_CLIENT_OnRosterEnd);
            History.Instance.OnUpdatedRoster -= new OnUpdatedRosterEventHandler(History_OnRosterUpdated);
            History.Instance.OnUpdatedPresence -= new OnUpdatedPresenceEventHandler(History_OnContactUpdated);
            History.Instance.OnUpdatedSession -= new OnUpdatedSessionEventHandler(History_OnUpdatedSession);
            History.Instance.OnSessionItemSelected -= new OnSessionItemSelectedEventHandler(History_OnSessionItemSelected);
        }


        #endregion

        #region ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ EventHandlers ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

        
        void JABBER_CLIENT_OnRosterStart() {
            GUIWaitCursor.Show();
        }

        void JABBER_CLIENT_OnRosterEnd() {
            AddHistoryEventHandlers();
            GUIWaitCursor.Hide();
            Helper.SetDefaultPresence();
            if (Settings.selectStatusOnStartup) {
                Dialog.Instance.SelectAndSetStatus();
            }
            
        }      

        void History_OnRosterUpdated(Contact changedContact) {
            UpdateContactsFacade();
        }


        void History_OnContactUpdated(Contact updatedContact) {
            UpdateContactsFacade();
        }


        void History_OnUpdatedSession(Session updatedSession, Message msg) {
            if (this.ctrlTextboxMessageHistory != null) {
                ctrlTextboxMessageHistory.AddSubItem(msg.ToString());
            }
            UpdateContactsFacade();
        }

        void History_OnSessionItemSelected(Session selectedSession, GUIControl parentControl) {
            if (parentControl == ctrlFacadeContactList) {
                UpdateGuiProperties(selectedSession);
            }
        }
        
        #endregion

        #region ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ Properties Gets/Sets ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

        // With GetID it will be an window-plugin / otherwise a process-plugin
        // Enter the id number here again
        public override int GetID {
            get { return (int)Helper.PLUGIN_WINDOW_IDS.WINDOW_ID_MAIN; }
        }
        #endregion

    }

    


}
