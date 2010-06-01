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
        public static readonly string SKIN_FILE_CHAT = SKIN_PATH_XML + PLUGIN_NAME +"_Chat.xml";
        public static readonly string SKIN_FILE_CONTACT = SKIN_PATH_XML + PLUGIN_NAME + "_Contact.xml";

        public static readonly string MEDIA_ICON_DEFAULT = SKIN_PATH_MEDIA + "icon_default.png";
        public static readonly string MEDIA_ICON_ERROR = SKIN_PATH_MEDIA + "icon_error.png";        
        public static readonly string MEDIA_ICON_MESSAGE = SKIN_PATH_MEDIA + "icon_message.png";
        public static readonly string MEDIA_ICON_PRESENCE = SKIN_PATH_MEDIA + "icon_presence.png";
        
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
            [Description("Notify Window (lower right)")]
            WINDOW_DIALOG_NOTIFY = GUIWindow.Window.WINDOW_DIALOG_NOTIFY,
            [Description("Dialog Window (small) (centered)")]
            WINDOW_DIALOG_OK = GUIWindow.Window.WINDOW_DIALOG_OK,
            [Description("Dialog Window (large) (centered)")]
            WINDOW_DIALOG_Text = GUIWindow.Window.WINDOW_DIALOG_TEXT            
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
   
        public static String GetFriendlyPresenceState(ShowType showType){
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
               
        public static void ShowNotifyDialog(int timeOut, string header, string icon, string text, Helper.PLUGIN_NOTIFY_WINDOWS notifyType) {
            try {
                GUIWindow guiWindow = GUIWindowManager.GetWindow((int)notifyType);
                switch (notifyType) { 
                    default:
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
                        okDialog.SetLine(1, (text.Split(new char[]{'\n'}, StringSplitOptions.RemoveEmptyEntries))[0]);
                        okDialog.DoModal(GUIWindowManager.ActiveWindow);
                        break;
                    case PLUGIN_NOTIFY_WINDOWS.WINDOW_DIALOG_Text:
                        GUIDialogText textDialog = (GUIDialogText)guiWindow;
                        textDialog.Reset();
                        textDialog.SetImage(icon);
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


    }
}
