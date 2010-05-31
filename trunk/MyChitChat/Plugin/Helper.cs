using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MediaPortal.GUI.Library;
using MyChitChat.Jabber;
using agsXMPP.protocol.client;
using System.Reflection;
using System.ComponentModel;

namespace MyChitChat.Plugin {
    static class Helper {
        #region Constants

        /// <summary>
        /// Section in the MediaPortal config for this plugin
        /// </summary>
        public const string PLUGIN_NAME = "MyChitChat";
        public const string PLUGIN_AUTHOR = "Anthrax";
        public const string PLUGIN_DESCRIPTION = "TODO";
        public static readonly string PLUGIN_SKIN_PREFIX = GUIGraphicsContext.Skin + @"\" + PLUGIN_NAME + "_";

        /// <summary>
        /// Version of the plugin
        /// </summary>
        public const string PLUGIN_VERSION = "0.1";

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
               
        public static bool SHOULD_NOTIFY_MESSAGE {
            get { return Settings.NotifyOnMessage || Enum.IsDefined(typeof(PLUGIN_WINDOW_IDS), GUIWindowManager.ActiveWindow); }
        }

        public static bool SHOULD_NOTIFY_PRESENCE {
            get { return Settings.NotifyOnPresence || Enum.IsDefined(typeof(PLUGIN_WINDOW_IDS), GUIWindowManager.ActiveWindow); }
        }
                
        public static readonly string SKINFILE_WINDOW_MAIN = PLUGIN_SKIN_PREFIX + "Main.xml";
        public static readonly string SKINFILE_WINDOW_CHAT = PLUGIN_SKIN_PREFIX + "Chat.xml";
        public static readonly string SKINFILE_WINDOW_CONTACT = PLUGIN_SKIN_PREFIX + "Contact.xml";
                
        #endregion

        public static Client JABBER_CLIENT { 
            get { return Client.Instance; } 
        }

        public static string GetFriendlyPresenceState<T>(this object enumerationValue) where T : struct {
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

    }
}
