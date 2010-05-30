using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MediaPortal.GUI.Library;
using MyChitChat.Jabber;

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
        
        public const int WINDOW_ID_MAIN = 300383;
        public const int WINDOW_ID_CHAT = WINDOW_ID_MAIN + 1001;

        public static readonly string SKINFILE_WINDOW_MAIN = PLUGIN_SKIN_PREFIX + "Main.xml";
        public static readonly string SKINFILE_WINDOW_CHAT = PLUGIN_SKIN_PREFIX + "Chat.xml";
                
        #endregion

        public static readonly Client JabberClient = Client.Instance;
    }
}
