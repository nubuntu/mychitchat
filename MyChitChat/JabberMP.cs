using System;
using System.Collections.Generic;
using System.Text;
using MediaPortal.GUI.Library;
using MediaPortal.Configuration;
using MediaPortal.Profile;

namespace Jabber.MP
{
    [PluginIcons("Jabber.MP.jabber.png", "Jabber.MP.jabber_disabled.png")]
    public class JabberMP : ISetupForm, IPlugin
    {
        #region Constants

        /// <summary>
        /// Section in the MediaPortal config for this plugin
        /// </summary>
        public const string ConfigSection = "Jabber.MP";

        /// <summary>
        /// Version of the plugin
        /// </summary>
        public const string Version = "0.1";

        #endregion

        #region Private members

        /// <summary>
        /// The jabber wrapper class
        /// </summary>
        private Jabber _jabber = null;

        #endregion

        #region IPlugin Member

        /// <summary>
        /// The process plugin has been started
        /// </summary>
        public void Start()
        {
            Log.Debug("Jabber.MP started");
            Settings.Load();
            
            _jabber = new Jabber();
            _jabber.Connect();
        }

        /// <summary>
        /// The process plugin has been stopped
        /// </summary>
        public void Stop()
        {
            _jabber.Disconnect();
        }

        #endregion

        #region ISetupForm Member

        public string Author()
        {
            return "Shukuyen";
        }

        public bool CanEnable()
        {
            return true;
        }

        public bool DefaultEnabled()
        {
            return false;
        }

        public string Description()
        {
            return "A Jabber client displaying received messages inside MediaPortal.";
        }

        public bool GetHome(out string strButtonText, out string strButtonImage, out string strButtonImageFocus, out string strPictureImage)
        {
            strButtonText = null;
            strButtonImage = null;
            strButtonImageFocus = null;
            strPictureImage = null;
            return false;
        }

        public int GetWindowId()
        {
            return -1;
        }

        public bool HasSetup()
        {
            return true;
        }

        public string PluginName()
        {
            return "Jabber.MP";
        }

        public void ShowPlugin()
        {
            SetupForm setup = new SetupForm();
            setup.ShowDialog();
        }

        #endregion
    }
}
