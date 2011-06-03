using System;
using System.Collections.Generic;
using System.Text;
using MediaPortal.GUI.Library;
using MediaPortal.Dialogs;
using MediaPortal.Configuration;
using MediaPortal.Profile;
using MyChitChat.Jabber;
using MyChitChat.Gui;

namespace MyChitChat.Plugin {
    [PluginIcons("MyChitChat.Resources.img.MyChitChat_icon_enabled.png", "MyChitChat.Resources.img.MyChitChat_icon_disabled")]
    public class Plugin : ISetupForm{
        #region Constants


        #endregion

        [STAThread]
        static void Main() {            
            new Plugin().ShowPlugin();           
        }

        public Plugin() {
            Log.Debug("MyChitChat started");            
        }

        #region ISetupForm Members

        // Returns the name of the plugin which is shown in the plugin menu
        public string PluginName() {
            return Helper.PLUGIN_NAME;
        }

        // Returns the description of the plugin is shown in the plugin menu
        public string Description() {
            return Helper.PLUGIN_DESCRIPTION;
        }

        // Returns the author of the plugin which is shown in the plugin menu
        public string Author() {
            return Helper.PLUGIN_AUTHOR;
        }

        // show the setup dialog
        public void ShowPlugin() {
            new Config().ShowDialog();
        }

        // Indicates whether plugin can be enabled/disabled
        public bool CanEnable() {
            return true;
        }

        // Get Windows-ID
        public int GetWindowId() {
            // WindowID of windowplugin belonging to this setup
            // enter your own unique code
            return GetWID;
        }

        // Indicates if plugin is enabled by default;
        public bool DefaultEnabled() {
            return true;
        }

        // indicates if a plugin has it's own setup screen
        public bool HasSetup() {
            return true;
        }

        /// <summary>
        /// If the plugin should have it's own button on the main menu of Mediaportal then it
        /// should return true to this method, otherwise if it should not be on home
        /// it should return false
        /// </summary>
        /// <param name="strButtonText">text the button should have</param>
        /// <param name="strButtonImage">image for the button, or empty for default</param>
        /// <param name="strButtonImageFocus">image for the button, or empty for default</param>
        /// <param name="strPictureImage">subpicture for the button or empty for none</param>
        /// <returns>true : plugin needs it's own button on home
        /// false : plugin does not need it's own button on home</returns>
        public bool GetHome(out string strButtonText, out string strButtonImage, out string strButtonImageFocus, out string strPictureImage) {
            strButtonText = Helper.PLUGIN_NAME;
            strButtonImage = String.Empty;
            strButtonImageFocus = String.Empty;
            strPictureImage = Helper.MEDIA_HOVER_HOME;
            return true;
        }
       

        public static int GetWID {
            get {
                return (int)Helper.PLUGIN_WINDOW_IDS.WINDOW_ID_MAIN;
            }
        }
        
        #endregion

      
    }
}
