using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MediaPortal.GUI.Library;
using MyChitChat.Plugin;

namespace MyChitChat.Gui {
   public class Main : GUIWindow{

        public override bool Init() {
            return Load(Helper.SKINFILE_WINDOW_MAIN);
        }
              

        // With GetID it will be an window-plugin / otherwise a process-plugin
        // Enter the id number here again
        public override int GetID {
            get {
                return Helper.WINDOW_ID_MAIN;
            }
            set {
            }
        }

    }
}
