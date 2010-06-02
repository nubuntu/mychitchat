using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MediaPortal.GUI.Library;
using MyChitChat.Jabber;
using agsXMPP.protocol.client;
using System.Reflection;
using System.ComponentModel;
using agsXMPP;
using MediaPortal.Dialogs;
using System.Text.RegularExpressions;
using System.IO;

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

        public static readonly string MEDIA_STATUS_ONLINE = SKIN_PATH_MEDIA + "status_online.png";
        public static readonly string MEDIA_STATUS_AWAY = SKIN_PATH_MEDIA + "status_away.png";
        public static readonly string MEDIA_STATUS_XA = SKIN_PATH_MEDIA + "status_xa.png";
        public static readonly string MEDIA_STATUS_CHAT = SKIN_PATH_MEDIA + "status_chat.png";
        public static readonly string MEDIA_STATUS_DND = SKIN_PATH_MEDIA + "status_dnd.png";


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

        public enum JABBER_PRESENCE_STATES : int {
            [Description("Online")]
            ONLINE = ShowType.NONE,
            [Description("Away")]
            AWAY = ShowType.away,
            [Description("Extended away")]
            EXTENDED_AWAY = ShowType.xa,
            [Description("Free for chat")]
            FREE_FOR_CHAT = ShowType.chat,
            [Description("Do not Disturb")]
            DO_NO_DISTURB = ShowType.dnd
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

        public static bool SHOULD_NOTIFY_MESSAGE {
            get { return Settings.NotifyOnMessage || PLUGIN_WINDOW_ACTIVE; }
        }

        public static bool SHOULD_NOTIFY_PRESENCE {
            get { return Settings.NotifyOnPresence || PLUGIN_WINDOW_ACTIVE; }
        }

        public static bool SHOULD_NOTIFY_ERROR {
            get { return Settings.NotifyOnError || PLUGIN_WINDOW_ACTIVE; }
        }


        #endregion

        public static Client JABBER_CLIENT {
            get { return Client.Instance; }
        }

        public static Roster JABBER_CONTACTS {
            get { return Client.Instance.Roster; }
        }

        public static Presence JABBER_PRESENCE_DEFAULT {
            get { return new Presence(Settings.DefaultShowType,Settings.DefaultStatusMessage); }
        }

        public static Presence JABBER_PRESENCE_CURRENT {
            get { return myCurrentPresence; }
            set { myCurrentPresence = value; }
        }
        private static Presence myCurrentPresence = JABBER_PRESENCE_DEFAULT;

        public static String GetFriendlyPresenceState(JABBER_PRESENCE_STATES showType) {
            return GetEnumDescription<Helper.JABBER_PRESENCE_STATES>((Helper.JABBER_PRESENCE_STATES)showType);
        }

        private static string GetEnumDescription<T>(this object enumerationValue) where T : struct {
            Type type = enumerationValue.GetType();
            if (!type.IsEnum) {
                throw new ArgumentException("EnumerationValue must be of Enum type", "enumerationValue");
            }

            //Tries to find a DescriptionAttribute for a potential friendly name
            //for the enum
            MemberInfo[] memberInfo = type.GetMember(enumerationValue.ToString());
            if (memberInfo != null && memberInfo.Length > 0) {
                object[] attrs = memberInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);

                if (attrs != null && attrs.Length > 0) {
                    //Pull out the description value
                    return ((DescriptionAttribute)attrs[0]).Description;
                }
            }
            //If we have no description attribute, just return the ToString of the enum
            return enumerationValue.ToString();

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
               
        public static void ShowDialogSelectStatus(string headerText, GUIListItem.ItemSelectedHandler callBack, bool custom) {
            GUIDialogSelect2 dlgSelectStatus = (GUIDialogSelect2)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_SELECT2);
            dlgSelectStatus.Reset();
            dlgSelectStatus.SetHeading(headerText);

            dlgSelectStatus.Add(Helper.CreateGuiListItem(Helper.JABBER_PRESENCE_STATES.ONLINE.ToString(),
                                                        Helper.GetFriendlyPresenceState(Helper.JABBER_PRESENCE_STATES.ONLINE),
                                                        Helper.MEDIA_STATUS_ONLINE,
                                                        callBack)
                                                        );
            dlgSelectStatus.Add(Helper.CreateGuiListItem(Helper.JABBER_PRESENCE_STATES.DO_NO_DISTURB.ToString(),
                                                        Helper.GetFriendlyPresenceState(Helper.JABBER_PRESENCE_STATES.DO_NO_DISTURB),
                                                        Helper.MEDIA_STATUS_DND,
                                                        callBack)
                                                        );
            dlgSelectStatus.Add(Helper.CreateGuiListItem(Helper.JABBER_PRESENCE_STATES.FREE_FOR_CHAT.ToString(),
                                                        Helper.GetFriendlyPresenceState(Helper.JABBER_PRESENCE_STATES.FREE_FOR_CHAT),
                                                        Helper.MEDIA_STATUS_CHAT,
                                                        callBack)
                                                        );
            dlgSelectStatus.Add(Helper.CreateGuiListItem(Helper.JABBER_PRESENCE_STATES.AWAY.ToString(),
                                                        Helper.GetFriendlyPresenceState(Helper.JABBER_PRESENCE_STATES.AWAY),
                                                        Helper.MEDIA_STATUS_AWAY,
                                                        callBack)
                                                        );
            dlgSelectStatus.Add(Helper.CreateGuiListItem(Helper.JABBER_PRESENCE_STATES.EXTENDED_AWAY.ToString(),
                                                        Helper.GetFriendlyPresenceState(Helper.JABBER_PRESENCE_STATES.EXTENDED_AWAY),
                                                        Helper.MEDIA_STATUS_XA,
                                                        callBack)
                                                        );
            if (!custom) {
                dlgSelectStatus.Add(Helper.CreateGuiListItem("custom","Custom Status...", String.Empty, String.Empty, callBack);                                                       
            }
            dlgSelectStatus.DoModal(GUIWindowManager.ActiveWindow);
            if(dlgSelectStatus.SelectedLabel
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
            ShowNotifyDialog(Settings.NotifyTimeOut, PLUGIN_NAME, MEDIA_ICON_DEFAULT, text, PLUGIN_NOTIFY_WINDOWS.WINDOW_DIALOG_NOTIFY);
        }

        #endregion
    }
}
