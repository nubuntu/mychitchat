using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MediaPortal.GUI.Library;
using MyChitChat.Jabber;
using System.Reflection;
using System.ComponentModel;
using MediaPortal.Dialogs;
using System.Text.RegularExpressions;
using System.IO;
using nJim;
using MyChitChat.Gui;

namespace MyChitChat.Plugin {
    public static class Helper {
        #region Constants

        /// <summary>
        /// Section in the MediaPortal config for this plugin
        /// </summary>
        public const string PLUGIN_NAME = "MyChitChat";
        public const string PLUGIN_AUTHOR = "Anthrax";
        public const string PLUGIN_VERSION = "0.1.0";
        public const string PLUGIN_DESCRIPTION = "TODO";

        public static readonly string SKIN_PATH_XML = GUIGraphicsContext.Skin + @"\" + PLUGIN_NAME+ @"\";
        public static readonly string SKIN_PATH_MEDIA = GUIGraphicsContext.Skin + @"\Media\" + PLUGIN_NAME + @"\";
        public static readonly string SKIN_FILE_MAIN = SKIN_PATH_XML + PLUGIN_NAME + "_Main.xml";
        public static readonly string SKIN_FILE_CHAT = SKIN_PATH_XML + PLUGIN_NAME + "_Chat.xml";
        public static readonly string SKIN_FILE_CONTACT = SKIN_PATH_XML + PLUGIN_NAME + "_Contact.xml";

        public static readonly string MEDIA_ICON_DEFAULT = SKIN_PATH_MEDIA + @"\plugin\icon_default.png";
        public static readonly string MEDIA_ICON_ERROR = SKIN_PATH_MEDIA + @"\plugin\icon_error.png";
        public static readonly string MEDIA_ICON_MESSAGE = SKIN_PATH_MEDIA + @"\plugin\icon_message.png";
        public static readonly string MEDIA_ICON_PRESENCE = SKIN_PATH_MEDIA + @"\plugin\icon_presence.png";
               

        public static string GetStatusIcon(string status) {
            string tmpPath = String.Format(@"{0}\status\{1}.png", SKIN_PATH_MEDIA, status);
            return File.Exists(tmpPath) ? tmpPath : String.Format(@"{0}\status\default_status.png", SKIN_PATH_MEDIA);
        }

        public static string GetMoodIcon(string mood) {
            string tmpPath = String.Format(@"{0}\mood\{1}.png", SKIN_PATH_MEDIA, mood);
            return File.Exists(tmpPath) ? tmpPath : String.Format(@"{0}\mood\default_mood.png", SKIN_PATH_MEDIA);
        }

        public static string GetActivityIcon(string activity) {
            string tmpPath = String.Format(@"{0}\activity\{1}.png", SKIN_PATH_MEDIA, activity);
            return File.Exists(tmpPath) ? tmpPath : String.Format(@"{0}\activity\default_activity.png", SKIN_PATH_MEDIA);
        }

        public static string GetTuneIcon(Tune tune) {
            //TODO: FanArt yeah!!!
            //return String.Format(@"{0}\tune\{1}", SKIN_PATH_MEDIA, tune.artist);
            string tmpPath = String.Format(@"{0}\tune\default.png", SKIN_PATH_MEDIA);
            return File.Exists(tmpPath) ? tmpPath : MEDIA_ICON_DEFAULT;
        }
               
        public static bool PLUGIN_WINDOW_ACTIVE {
            get { return Enum.IsDefined(typeof(PLUGIN_WINDOW_IDS), GUIWindowManager.ActiveWindow); }
        }

        public enum PLUGIN_WINDOW_IDS : int {
            WINDOW_ID_MAIN = 90000,
            WINDOW_ID_CHAT = WINDOW_ID_MAIN + 1001,
            WINDOW_ID_DETAILS = WINDOW_ID_MAIN + 1002,
        }
                
        public enum PLUGIN_NOTIFY_WINDOWS {
            WINDOW_DIALOG_AUTO,
            WINDOW_DIALOG_NOTIFY = GUIWindow.Window.WINDOW_DIALOG_NOTIFY,
            WINDOW_DIALOG_OK = GUIWindow.Window.WINDOW_DIALOG_OK,
            WINDOW_DIALOG_TEXT = GUIWindow.Window.WINDOW_DIALOG_TEXT
        }

        public struct PresMooActNotifyInfo {
            public string header;
            public string nickname;
            public string resource;
            public string status;
            public string activity;
            public string tune;
            public string mood;
            public string message;
            public DateTime stamp;
            public string icon;
        }

    
        #endregion

        static readonly Client _client = new Client();

        // Explicit static constructor to tell C# compiler
        // not to mark type as beforefieldinit
        
       
        
        public static Client JABBER_CLIENT {
            get {
                return _client;
            }
        }

        public static Roster JABBER_CONTACTS {
            get { return _client.Roster; }
        }

        public static Presence JABBER_PRESENCE_DEFAULT {
            get {
                Presence currentPresence = new Presence();

                return currentPresence;
            }
        }

        public static Mood PRESENCE_CURRENT_MOOD {
            get { return myCurrentPresence.mood; }
        }

        //public static void SetMyCurrentPresence(Enums.StatusType showType, string statusMessage) {
        //    myCurrentPresence = new Presence((ShowType)showType, statusMessage);
        //    myCurrentPresence.Type = PresenceType.invisible;
        //}

        //public static void SetMyCurrentPresencePluginEnabled() {
        //    JABBER_CLIENT.SendyMyPresence(new Presence(myCurrentPresence.Show, myCurrentPresence.Status + String.Format(" [MediaPortal {0} enabled]", PLUGIN_NAME )));
        //}
        //public static void SetMyCurrentPresencePluginDisabled() {
        //    JABBER_CLIENT.SendyMyPresence(new Presence(myCurrentPresence.Show, myCurrentPresence.Status + String.Format(" [MediaPortal {0} disabled]", PLUGIN_NAME))); 
        //    JABBER_CLIENT.SendyMyPresence(myCurrentPresence);
        //}


        private static Presence myCurrentPresence = JABBER_PRESENCE_DEFAULT;


        #region ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ GUI Helper Methods ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

        public static GUIListItem CreateGuiListFolder(string itemID, string itemTitle, string itemIcon) {
            return CreateGuiListItem(itemID, itemTitle, string.Empty, string.Empty, itemIcon, false, null, true);
        }
        public static GUIListItem CreateGuiListItem(string itemID, string itemTitle, string itemIcon, GUIListItem.ItemSelectedHandler callBackItemSelected) {
            return CreateGuiListItem(itemID, itemTitle, string.Empty, string.Empty, itemIcon, false, callBackItemSelected, false);
        }

        public static GUIListItem CreateGuiListItem(string itemID, string itemTitle, string itemInfo1, string itemInfo2, string itemIcon, bool itemShaded, GUIListItem.ItemSelectedHandler callBackItemSelected, bool isFolder) {
            GUIListItem tmp = new GUIListItem(itemTitle);
            tmp.Label2 = itemInfo1;
            tmp.Label3 = itemInfo2;
            tmp.Path = itemID;
            tmp.Shaded = itemShaded;
            tmp.IsFolder = isFolder;
            if (File.Exists(itemIcon)) {
                tmp.IconImage = itemIcon;
                tmp.IconImageBig = itemIcon;
                tmp.ThumbnailImage = itemIcon;
            }
            if (!isFolder) {
                tmp.OnItemSelected += callBackItemSelected;
            }
            return tmp;
        }



        #endregion

        public static void SetDefaultPresence() {

            Status initialStatus = new Status();
            initialStatus.type = Settings.defaultStatusType;
            initialStatus.message = Settings.defaultStatusMessage;           
            Helper.JABBER_CLIENT.Presence.status = initialStatus;
            Helper.JABBER_CLIENT.Presence.setActivity(Settings.defaultActivityType, Settings.defaultActivityMessage);
            Helper.JABBER_CLIENT.Presence.setMood(Settings.defaultMoodType, Settings.defaultMoodMessage);
            Log.Info("Default Presence info set.");            
        }

        public static void SetStatus(Enums.StatusType type, string message){
            Status tmpStatus = new Status();
            tmpStatus.type = type;
            tmpStatus.message = message;
            Helper.JABBER_CLIENT.Presence.status = tmpStatus;
            Helper.JABBER_CLIENT.Presence.applyStatus();
        }

        public static void SetActivity(Enums.ActivityType type, string message) {
            JABBER_CLIENT.Presence.setActivity(type, message);
        }

        public static void SetMood(Enums.MoodType type, string message) {
            JABBER_CLIENT.Presence.setMood(type, message);
        }

        public static Status GetStatusFromType(Enums.StatusType type, string message) {
            Status tmpStatus = new Status();
            tmpStatus.type = type;
            tmpStatus.message = message;
            return tmpStatus;
        }

        public static void SetTune(string title, string artist, int length) {           
            JABBER_CLIENT.Presence.setTune(title, artist, length);
        }


        public static Status GetStatusFromType(Enums.StatusType type) {
            Status tmpStatus = new Status();
            tmpStatus.type = type;
            tmpStatus.message = Translations.GetByName(type.ToString());
            return tmpStatus;
        }
    }
}
