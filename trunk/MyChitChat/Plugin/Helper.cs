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

        public static string GetStatusIcon(Status status) {
            string tmpPath = String.Format(@"{0}\status\{1}.png", SKIN_PATH_MEDIA, status.type.ToString());
            return File.Exists(tmpPath) ? tmpPath : String.Format(@"{0}\status\default.png", SKIN_PATH_MEDIA);
        }

        public static string GetMoodIcon(Mood mood) {
            string tmpPath = String.Format(@"{0}\mood\{1}.png", SKIN_PATH_MEDIA, mood.type.ToString());
            return File.Exists(tmpPath) ? tmpPath : String.Format(@"{0}\mood\default.png", SKIN_PATH_MEDIA);
        }

        public static string GetActivityIcon(Activity activity) {
            string tmpPath = String.Format(@"{0}\activity\{1}.png", SKIN_PATH_MEDIA, activity.type.ToString());
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
        
        /// <summary>
        /// Convertit le type de status utilisé par la librairie en type de status utilisé par la librairie agsXMPP
        /// </summary>
        /// <param name="type">Type de status</param>
        /// <returns></returns>
        public static string GetFriendlyStatusType(Enums.StatusType type) {
            switch (type) {
                case Enums.StatusType.Normal: return "Available";
                case Enums.StatusType.Unvailable: return "Unavailable";
                case Enums.StatusType.Away: return "Away";
                case Enums.StatusType.ExtendedAway: return "Extended Away";
                case Enums.StatusType.DoNotDisturb: return "Do not Disturb";
                case Enums.StatusType.ReadyToChat: return "Free for Chat";
                case Enums.StatusType.Invisible: return "Invisible";
                default: return "Unknown";           
            }
        }

        public static Status GetStatusFromType(Enums.StatusType type) {
            Status tmpStatus = new Status();
            tmpStatus.type = type;
            tmpStatus.message = GetFriendlyStatusType(type);
            return tmpStatus;
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
            get { return myCurrentPresence.mood ; }
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

        /// <summary>
        /// Displays a yes/no dialog with custom labels for the buttons
        /// This method may become obsolete in the future if media portal adds more dialogs
        /// </summary>
        /// <returns>True if yes was clicked, False if no was clicked</returns>
        /// This has been taken (stolen really) from the wonderful MovingPictures Plugin -Anthrax.
        public static bool ShowCustomYesNo(int parentWindowID, string heading, string lines, string yesLabel, string noLabel, bool defaultYes) {
            GUIDialogYesNo dialog = (GUIDialogYesNo)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_YES_NO);
            try {
                dialog.Reset();
                dialog.SetHeading(heading);
                string[] linesArray = lines.Split(new string[] { "\\n" }, StringSplitOptions.None);
                if (linesArray.Length > 0)
                    dialog.SetLine(1, linesArray[0]);
                if (linesArray.Length > 1)
                    dialog.SetLine(2, linesArray[1]);
                if (linesArray.Length > 2)
                    dialog.SetLine(3, linesArray[2]);
                if (linesArray.Length > 3)
                    dialog.SetLine(4, linesArray[3]);
                dialog.SetDefaultToYes(defaultYes);

                foreach (System.Windows.UIElement item in dialog.Children) {
                    if (item is GUIButtonControl) {
                        GUIButtonControl btn = (GUIButtonControl)item;
                        if (btn.GetID == 11 && !String.IsNullOrEmpty(yesLabel)) // Yes button
                            btn.Label = yesLabel;
                        else if (btn.GetID == 10 && !String.IsNullOrEmpty(noLabel)) // No button
                            btn.Label = noLabel;
                    }
                }
                dialog.DoModal(parentWindowID);
                return dialog.IsConfirmed;
            } finally {
                // set the standard yes/no dialog back to it's original state (yes/no buttons)
                if (dialog != null) {
                    dialog.ClearAll();
                }
            }
        }
        public static void ShowNotifyDialog(string header, string icon, string text, Helper.PLUGIN_NOTIFY_WINDOWS notifyType) {
            ShowNotifyDialog(header, icon, text, notifyType);
        }
        public static void ShowNotifyDialog(int timeOut, string header, string icon, string text, Helper.PLUGIN_NOTIFY_WINDOWS notifyType) {
            try {
                GUIWindow guiWindow = GUIWindowManager.GetWindow((int)notifyType);
                switch (notifyType) {
                    default:
                    case PLUGIN_NOTIFY_WINDOWS.AUTO:
                        if (text.Length <= 60) {
                            ShowNotifyDialog(timeOut, header, icon, text, Helper.PLUGIN_NOTIFY_WINDOWS.WINDOW_DIALOG_NOTIFY);
                        } else {
                            ShowNotifyDialog(timeOut, header, icon, text, Helper.PLUGIN_NOTIFY_WINDOWS.WINDOW_DIALOG_TEXT);
                        }
                        break;
                    case PLUGIN_NOTIFY_WINDOWS.WINDOW_DIALOG_NOTIFY:
                        GUIDialogNotify notifyDialog = (GUIDialogNotify)guiWindow;
                        notifyDialog.Reset();
                        notifyDialog.TimeOut = timeOut;
                        notifyDialog.SetImage(icon);
                        notifyDialog.SetHeading(header);
                        notifyDialog.SetText(text);
                        notifyDialog.DoModal(GUIWindowManager.ActiveWindow);
                        break;
                    case PLUGIN_NOTIFY_WINDOWS.WINDOW_DIALOG_OK:
                        GUIDialogOK okDialog = (GUIDialogOK)guiWindow;
                        okDialog.Reset();
                        okDialog.SetHeading(header);
                        okDialog.SetLine(1, (text.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries))[0]);
                        okDialog.DoModal(GUIWindowManager.ActiveWindow);
                        break;
                    case PLUGIN_NOTIFY_WINDOWS.WINDOW_DIALOG_TEXT:
                        GUIDialogText textDialog = (GUIDialogText)guiWindow;
                        textDialog.Reset();
                        try {
                            textDialog.SetImage(icon);
                        } catch { }
                        textDialog.SetHeading(header);
                        textDialog.SetText(text);
                        textDialog.DoModal(GUIWindowManager.ActiveWindow);
                        break;
                }
            } catch (Exception ex) {
                Log.Error(ex);
            }
        }


        public static void ShowNotifyDialog(string text) {
            ShowNotifyDialog(Settings.notifyTimeOut, PLUGIN_NAME, MEDIA_ICON_DEFAULT, text, PLUGIN_NOTIFY_WINDOWS.WINDOW_DIALOG_NOTIFY);
        }

        #endregion
    }
}
