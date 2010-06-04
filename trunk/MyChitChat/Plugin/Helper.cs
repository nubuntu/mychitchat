﻿using System;
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
    static class Helper {
        #region Constants

        /// <summary>
        /// Section in the MediaPortal config for this plugin
        /// </summary>
        public const string PLUGIN_NAME = "MyChitChat";
        public const string PLUGIN_AUTHOR = "Anthrax";
        public const string PLUGIN_DESCRIPTION = "TODO";

        public static readonly string SKIN_PATH_XML = GUIGraphicsContext.Skin + @"\";
        public static readonly string SKIN_PATH_MEDIA = GUIGraphicsContext.Skin + @"\Media\" + PLUGIN_NAME + @"\";
        public static readonly string SKIN_FILE_MAIN = SKIN_PATH_XML + PLUGIN_NAME + "_Main.xml";
        public static readonly string SKIN_FILE_CHAT = SKIN_PATH_XML + PLUGIN_NAME + "_Chat.xml";
        public static readonly string SKIN_FILE_CONTACT = SKIN_PATH_XML + PLUGIN_NAME + "_Contact.xml";

        public static readonly string MEDIA_ICON_DEFAULT = SKIN_PATH_MEDIA + "icon_default.png";
        public static readonly string MEDIA_ICON_ERROR = SKIN_PATH_MEDIA + "icon_error.png";
        public static readonly string MEDIA_ICON_MESSAGE = SKIN_PATH_MEDIA + "icon_message.png";
        public static readonly string MEDIA_ICON_PRESENCE = SKIN_PATH_MEDIA + "icon_presence.png";

        public static readonly string MEDIA_STATUS_AVAILABLE = SKIN_PATH_MEDIA + "status_available.png";
        public static readonly string MEDIA_STATUS_AWAY = SKIN_PATH_MEDIA + "status_away.png";
        public static readonly string MEDIA_STATUS_XA = SKIN_PATH_MEDIA + "status_xa.png";
        public static readonly string MEDIA_STATUS_CHAT = SKIN_PATH_MEDIA + "status_chat.png";
        public static readonly string MEDIA_STATUS_DND = SKIN_PATH_MEDIA + "status_dnd.png";

        public static string GetStatusIcon(string status) {
            string tmpPath = String.Format(@"{0}\status\{1}.png", SKIN_PATH_MEDIA, status);
            return File.Exists(tmpPath) ? tmpPath : String.Format(@"{0}\status\default.png", SKIN_PATH_MEDIA);
        }

        public static string GetMoodIcon(string mood) {
            string tmpPath = String.Format(@"{0}\mood\{1}.png", SKIN_PATH_MEDIA, mood);
            return File.Exists(tmpPath) ? tmpPath : String.Format(@"{0}\mood\default.png", SKIN_PATH_MEDIA);
        }

        public static string GetActivityIcon(string activity) {
            string tmpPath = String.Format(@"{0}\activity\{1}.png", SKIN_PATH_MEDIA, activity);
            return File.Exists(tmpPath) ? tmpPath : String.Format(@"{0}\activity\default.png", SKIN_PATH_MEDIA);
        }

        public static string GetTuneIcon(Tune tune) {
            //TODO: FanArt yeah!!!
            //return String.Format(@"{0}\tune\{1}", SKIN_PATH_MEDIA, tune.artist);
            string tmpPath = String.Format(@"{0}\tune\default.png", SKIN_PATH_MEDIA);
            return File.Exists(tmpPath) ? tmpPath : MEDIA_ICON_DEFAULT;
        }

        /// <summary>
        /// Version of the plugin
        /// </summary>
        public const string PLUGIN_VERSION = "0.1";

        public static bool PLUGIN_WINDOW_ACTIVE {
            get { return Enum.IsDefined(typeof(PLUGIN_WINDOW_IDS), GUIWindowManager.ActiveWindow); }
        }

        public enum PLUGIN_WINDOW_IDS : int {
            WINDOW_ID_MAIN = 90000,
            WINDOW_ID_CHAT = WINDOW_ID_MAIN + 1001,
            WINDOW_ID_CONTACT = WINDOW_ID_MAIN + 1002,
        }

        public static string ToSentence(string input) {
            return Regex.Replace(input, ".[A-Z]", m => m.ToString()[0] + " " + char.ToLower(m.ToString()[1]));
        }

        public enum PLUGIN_NOTIFY_WINDOWS {
            [Description("Use Windowsize suitable for Message length")]
            AUTO,
            [Description("Notify Window (lower right)")]
            WINDOW_DIALOG_NOTIFY = GUIWindow.Window.WINDOW_DIALOG_NOTIFY,
            [Description("Dialog Window (small) (centered)")]
            WINDOW_DIALOG_OK = GUIWindow.Window.WINDOW_DIALOG_OK,
            [Description("Dialog Window (large) (centered)")]
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

        public static Client JABBER_CLIENT {
            get { return Client.Instance; }
        }

        public static Roster JABBER_CONTACTS {
            get { return Client.Instance.Roster; }
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

        private void SetDefaultPresence() {
            Helper.JABBER_CLIENT.Presence.status = Helper.GetStatusFromType(Settings.defaultStatusType, Settings.defaultStatusMessage);
            Helper.JABBER_CLIENT.Presence.applyStatus();
            Helper.JABBER_CLIENT.Presence.setActivity(Settings.defaultActivityTypem, Settings.defaultActivityMessage);
            Helper.JABBER_CLIENT.Presence.setMood(Settings.defaultMoodType, Settings.defaultMoodMessage);
        }


        public static Status GetStatusFromType(Enums.StatusType type, string message) {
            Status tmpStatus = new Status();
            tmpStatus.type = type;
            tmpStatus.message = message;
            return tmpStatus;
        }
        public static Status GetStatusFromType(Enums.StatusType type) {
            Status tmpStatus = new Status();
            tmpStatus.type = type;
            tmpStatus.message = Helper.ToSentence(type.ToString());
            return tmpStatus;
        }
    }
}
