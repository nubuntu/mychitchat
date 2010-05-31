using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MediaPortal.GUI.Library;
using MyChitChat.Plugin;


namespace MyChitChat.Gui {
    class Chat : GUIWindow{

        public Chat() {
            GetID = (int)Helper.PLUGIN_WINDOW_IDS.WINDOW_ID_CHAT;
        }

        /// <summary>
        /// Loads the XML for the window
        /// </summary>
        public override bool Init() {
            return Load(GUIGraphicsContext.Skin + @"\MyChitChat_ChatWindow.xml");
        }
    }
}
