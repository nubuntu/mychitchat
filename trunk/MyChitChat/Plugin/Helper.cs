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
using System.Drawing;
using System.Diagnostics;
using System.Threading;

namespace MyChitChat.Plugin {
    public static class Helper {
        #region Constants

        static Helper() {
            JABBER_CLIENT.OnError += new OnErrorEventHandler(JABBER_CLIENT_OnError);
            MediaPortal.Player.g_Player.PlayBackStarted += new MediaPortal.Player.g_Player.StartedHandler(g_Player_PlayBackStarted);
            MediaPortal.Player.g_Player.PlayBackChanged += new MediaPortal.Player.g_Player.ChangedHandler(g_Player_PlayBackChanged);
            MediaPortal.Player.g_Player.PlayBackStopped += new MediaPortal.Player.g_Player.StoppedHandler(g_Player_PlayBackStopped);
            MediaPortal.Player.g_Player.PlayBackEnded += new MediaPortal.Player.g_Player.EndedHandler(g_Player_PlayBackEnded);
            GUIPropertyManager.OnPropertyChanged += new GUIPropertyManager.OnPropertyChangedHandler(GUIPropertyManager_OnPropertyChanged);
            CurrentKeyboardType = Settings.defaultKeyboardType;
        }

        /// <summary>
        /// Section in the MediaPortal config for this plugin
        /// </summary>
        public const string PLUGIN_NAME = "MyChitChat";
        public const string PLUGIN_AUTHOR = "Anthrax";
        public const string PLUGIN_VERSION = "0.9.0";
        public const string PLUGIN_DESCRIPTION = "TODO";

        public static readonly string PLUGIN_LINK_PROJECT_HOME = new UriBuilder("http://mychitchat.googlecode.com").ToString();
        public static readonly string PLUGIN_LINK_NEW_ISSUE = new UriBuilder("http://code.google.com/p/mychitchat/issues/entry").ToString();
        public static readonly string PLUGIN_LINK_FORUM = new UriBuilder("http://forum.team-mediaportal.com/mediaportal-plugins-47/upcoming-mychitchat-jabber-im-plugin-muc-irc-aim-gg-gtalk-icq-msn-qq-sms-email-83953/#post632708").ToString();

        public static readonly string SKIN_PATH_XML = GUIGraphicsContext.Skin + @"\";
        public static readonly string SKIN_PATH_MEDIA = GUIGraphicsContext.Skin + @"\Media\" + PLUGIN_NAME + @"\";
        public static readonly string SKIN_FILE_MAIN = SKIN_PATH_XML + PLUGIN_NAME + "_Main.xml";
        public static readonly string SKIN_FILE_CHAT = SKIN_PATH_XML + PLUGIN_NAME + "_Chat.xml";
        public static readonly string SKIN_FILE_CONTACT = SKIN_PATH_XML + PLUGIN_NAME + "_Contact.xml";


        public static readonly string MEDIA_HOVER_HOME = @"hover_MyChitChat.png";

        public static readonly string MEDIA_ICON_DEFAULT = SKIN_PATH_MEDIA + @"\plugin\icon_default.png";
        public static readonly string MEDIA_ICON_ERROR = SKIN_PATH_MEDIA + @"\plugin\icon_error.png";
        public static readonly string MEDIA_ICON_MESSAGE = SKIN_PATH_MEDIA + @"\plugin\icon_message.png";
        public static readonly string MEDIA_ICON_PRESENCE = SKIN_PATH_MEDIA + @"\plugin\icon_presence.png";
        public static readonly string MEDIA_ICON_MESSAGE_IN_READ = SKIN_PATH_MEDIA + @"\plugin\icon_message_in_read.png";
        public static readonly string MEDIA_ICON_MESSAGE_IN_UNREAD = SKIN_PATH_MEDIA + @"\plugin\icon_message_in_unread.png";
        public static readonly string MEDIA_ICON_MESSAGE_IN_REPLIED = SKIN_PATH_MEDIA + @"\plugin\icon_message_in_replied.png";
        public static readonly string MEDIA_ICON_MESSAGE_OUT = SKIN_PATH_MEDIA + @"\plugin\icon_message_out.png";

        public static string GetStatusIcon(string status) {
            string tmpPath = String.Format(@"{0}\status\{1}.png", SKIN_PATH_MEDIA, status);
            return File.Exists(tmpPath) ? tmpPath : String.Format(@"{0}status\default_status.png", SKIN_PATH_MEDIA);
        }

        public static string GetMoodIcon(string mood) {
            string tmpPath = String.Format(@"{0}\mood\{1}.png", SKIN_PATH_MEDIA, mood);
            return File.Exists(tmpPath) ? tmpPath : String.Format(@"{0}mood\default_mood.png", SKIN_PATH_MEDIA);
        }

        public static string GetActivityIcon(string activity) {
            string tmpPath = String.Format(@"{0}\activity\{1}.png", SKIN_PATH_MEDIA, activity);
            return File.Exists(tmpPath) ? tmpPath : String.Format(@"{0}activity\default_activity.png", SKIN_PATH_MEDIA);
        }

        public static string GetTuneIcon(Tune tune) {
            //TODO: FanArt yeah!!!
            //return String.Format(@"{0}\tune\{1}", SKIN_PATH_MEDIA, tune.artist);
            string tmpPath = String.Format(@"{0}tune\default.png", SKIN_PATH_MEDIA);
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

        public static Dialog.KeyBoardTypes CurrentKeyboardType { get; set; }

        #endregion

        static readonly Client _client = new Client();

        public static Client JABBER_CLIENT {
            get {
                return _client;
            }
        }

        public static void JABBER_CLIENT_OnError(nJim.Enums.ErrorType type, string message) {
            if ((Settings.notifyOnErrorPlugin && Helper.PLUGIN_WINDOW_ACTIVE) || Settings.notifyOnErrorGlobally) {
                Dialog.Instance.ShowErrorDialog(type, message);
            }
        }

        public static Roster JABBER_CONTACTS {
            get { return _client.Roster; }
        }

        private static Status JABBER_LAST_STATUS;
        public static Presence JABBER_CURRENT_PRESENCE {
            get {
                return (JABBER_CLIENT.Presence != null) ? JABBER_CLIENT.Presence : new Presence();
            }
        }

        public static void SetPluginEnterPresence() {
            if (JABBER_LAST_STATUS.message == null) {
                JABBER_LAST_STATUS = SetDefaultPresence().status;

            } else {
                SetStatus(JABBER_LAST_STATUS.type, JABBER_LAST_STATUS.message);
            }
        }

        public static void SetPluginLeavePresence() {
            try {
                Status idleStatus = new Status();
                idleStatus.type = JABBER_CLIENT.Presence.autoIdleStatus.type;
                idleStatus.message = JABBER_CLIENT.Presence.autoIdleStatus.message;
                Helper.JABBER_CLIENT.Presence.status = idleStatus;
                Helper.JABBER_CLIENT.Presence.applyStatus();
            } catch (Exception e) {
                Log.Error(e);
            }
        }




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

        public static Presence SetDefaultPresence() {
            SetStatus(Settings.defaultStatusType, Settings.defaultStatusMessage);
            SetActivity(Settings.defaultActivityType, Settings.defaultActivityMessage);
            SetMood(Settings.defaultMoodType, Settings.defaultMoodMessage);
            Log.Info("Default Presence info set.");
            return Helper.JABBER_CLIENT.Presence;
        }

        public static void SetStatus(Enums.StatusType type, string message) {
            try {
                Status tmpStatus = new Status();
                tmpStatus.type = type;
                tmpStatus.message = message;
                JABBER_LAST_STATUS = tmpStatus;
                Helper.JABBER_CLIENT.Presence.status = tmpStatus;
                Helper.JABBER_CLIENT.Presence.applyStatus();
            } catch (Exception e) {
                Log.Error(e);
            }
        }

        public static void SetActivity(Enums.ActivityType type, string message) {
            try {
                JABBER_CLIENT.Presence.setActivity(type, message);
            } catch (Exception e) {
                Log.Error(e);
            }
        }

        public static void SetMood(Enums.MoodType type, string message) {
            try {
                JABBER_CLIENT.Presence.setMood(type, message);
            } catch (Exception e) {
                Log.Error(e);
            }
        }

        public static Status GetStatusFromType(Enums.StatusType type, string message) {
            Status tmpStatus = new Status();
            tmpStatus.type = type;
            tmpStatus.message = message;
            return tmpStatus;
        }

        public static void SetTune(string title, string artist, int length) {
            try {
                JABBER_CLIENT.Presence.setTune(title, artist, length);
            } catch (Exception e) {
                Log.Error(e);
            }
        }

        public static void SetTune(Tune tune) {
            JABBER_CLIENT.Presence.setTune(tune);
        }


        public static Status GetStatusFromType(Enums.StatusType type) {
            Status tmpStatus = new Status();
            tmpStatus.type = type;
            tmpStatus.message = Translations.GetByName(type.ToString());
            return tmpStatus;
        }

        static void g_Player_PlayBackStopped(MediaPortal.Player.g_Player.MediaType type, int stoptime, string filename) {
            SetActivity(Settings.defaultActivityType, Settings.defaultActivityMessage);
        }


        static void g_Player_PlayBackEnded(MediaPortal.Player.g_Player.MediaType type, string filename) {
            SetActivity(Settings.defaultActivityType, Settings.defaultActivityMessage);
        }

        private static bool setTuneOnChange = false;

        static void g_Player_PlayBackStarted(MediaPortal.Player.g_Player.MediaType type, string filename) {
            switch (type) {
                case MediaPortal.Player.g_Player.MediaType.Music:
                    setTuneOnChange = true;
                    if (Settings.publishActivityMusic) {
                        SetActivity(Enums.ActivityType.partying, "see Tune");
                    }
                    break;
                case MediaPortal.Player.g_Player.MediaType.Radio:
                    setTuneOnChange = true;
                    if (Settings.publishActivityRadio) {
                        SetActivity(Enums.ActivityType.relaxing, "see Tune");
                    }
                    break;
                case MediaPortal.Player.g_Player.MediaType.TV:
                    if (Settings.publishActivityTV) {
                        SetActivity(Enums.ActivityType.watching_tv, String.Format("watching: {0} on {1}", GUIPropertyManager.GetProperty("#TV.View.title"), GUIPropertyManager.GetProperty("#TV.View.channel")));
                    }
                    break;
                case MediaPortal.Player.g_Player.MediaType.Video:
                    if (Settings.publishActivityMovie) {
                        SetActivity(Enums.ActivityType.watching_a_movie, String.Format("watching: '{0}' (Director: {1}, Cast: {2}",GUIPropertyManager.GetProperty("#Play.Current.Title"), GUIPropertyManager.GetProperty("#Play.Current.Director"),GUIPropertyManager.GetProperty("#Play.Current.Cast")));
                    }
                    break;
                case MediaPortal.Player.g_Player.MediaType.Recording:
                    if (Settings.publishActivityRecording) {
                        SetActivity(Enums.ActivityType.watching_tv, String.Format("recording: {0} on {1}" + GUIPropertyManager.GetProperty("#TV.View.title") + " on " + GUIPropertyManager.GetProperty("#TV.View.channel")));
                    }
                    break;
                case MediaPortal.Player.g_Player.MediaType.Unknown:
                default:
                    break;
            }
        }

        static void g_Player_PlayBackChanged(MediaPortal.Player.g_Player.MediaType type, int stoptime, string filename) {
            g_Player_PlayBackStarted(type, filename);
        }

        static void GUIPropertyManager_OnPropertyChanged(string tag, string tagValue) {
            if (setTuneOnChange && tag.Equals("#Play.Current.Duration")) {
                if (!String.IsNullOrEmpty(tagValue)) {
                    GetSetCurrentTune();
                }
                setTuneOnChange = false;
            }
        }


        private static void GetSetCurrentTune() {
            if (Settings.publishTuneInfo) {
                Tune currentTune = new Tune();
                currentTune.artist = GUIPropertyManager.GetProperty("#Play.Current.Artist");
                currentTune.title = GUIPropertyManager.GetProperty("#Play.Current.Title");
                currentTune.source = GUIPropertyManager.GetProperty("#Play.Current.Album");
                try {
                    currentTune.track = int.Parse(GUIPropertyManager.GetProperty("#Play.Current.Track"));
                    currentTune.length = (int)TimeSpan.Parse(GUIPropertyManager.GetProperty("#Play.Current.Duration")).TotalSeconds;
                    currentTune.rating = int.Parse(GUIPropertyManager.GetProperty("#Play.Current.Rating"));
                } catch (Exception e) {
                    Log.Error(e);
                }
                SetTune(currentTune);
            }
        }
    }
}
