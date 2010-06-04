using System;
using System.Collections.Generic;
using System.Text;
using MediaPortal;
using nJim;

namespace MyChitChat.Plugin {
    class Settings {
        #region Properties

        /// <summary>
        /// The jabber account information
        /// </summary>       
        public static string username = String.Empty;       
        public static string server = String.Empty;
        public static string resource = "MediaPortal";
        public static string password = String.Empty;  
  

        public static int notifyTimeOut = 10;        
        public static bool autoConnectStartup = true;
        public static bool notifyOutsidePlugin = true;
        public static bool notifyOnMessage = true;
        public static bool notifyOnPresenceUpdate = true;
        public static bool notifyOnMoodUpdate = true;
        public static bool notifyOnActivityUpdate = true;
        public static bool notifyOnTuneUpdate = true;
        
        public static bool notifyOnError = true;     
        public static bool setPresenceOnStartup = true;

        public static Enums.StatusType defaultStatusType = Enums.StatusType.Normal;
        public static string defaultStatusMessage = "Online with " + Helper.PLUGIN_NAME;

        public static int autoIdleTimeOut = 5;
        public static Enums.StatusType autoIdleStatusType = Enums.StatusType.Away;
        public static string autoIdleStatusMessage = Helper.ToSentence(autoIdleStatusType.ToString());

        public static Enums.MoodType defaultMoodType = Enums.MoodType.neutral;
        public static string defaultMoodMessage = "I'm feeling very entertained right now thx to MediaPortal ('Ya know the kick-ass open source MediaCenter alternative)...";

        public static Enums.ActivityType defaultActivityType = Enums.ActivityType.relaxing;
        public static string defaultActivityMessage = "I'm using MediaPortal at the moment...";

        public static Helper.PLUGIN_NOTIFY_WINDOWS notifyWindowTypeMessage = Helper.PLUGIN_NOTIFY_WINDOWS.AUTO;
        public static Helper.PLUGIN_NOTIFY_WINDOWS notifyWindowTypePresence = Helper.PLUGIN_NOTIFY_WINDOWS.AUTO;


        #endregion

        /// <summary>
        /// Load the settings from the mediaportal config
        /// </summary>
        public static void Load() {
            using (MediaPortal.Profile.Settings reader = new MediaPortal.Profile.Settings(MediaPortal.Configuration.Config.GetFile(MediaPortal.Configuration.Config.Dir.Config, "MediaPortal.xml"))) {
                username = reader.GetValue(Helper.PLUGIN_NAME, "username");
                server = reader.GetValue(Helper.PLUGIN_NAME, "server");
                resource = reader.GetValue(Helper.PLUGIN_NAME, "resource");

                string encryptedPassword = reader.GetValue(Helper.PLUGIN_NAME, "password");
                password = decryptString(encryptedPassword);
            }
        }

        /// <summary>
        /// Save the settings to the MP config
        /// </summary>
        public static void Save() {
            using (MediaPortal.Profile.Settings xmlwriter = new MediaPortal.Profile.Settings(MediaPortal.Configuration.Config.GetFile(MediaPortal.Configuration.Config.Dir.Config, "MediaPortal.xml"))) {
                // Encrypt the password
                string encryptedPassword = encryptString(password);

                xmlwriter.SetValue(Helper.PLUGIN_NAME, "username", username);
                xmlwriter.SetValue(Helper.PLUGIN_NAME, "server", server);
                xmlwriter.SetValue(Helper.PLUGIN_NAME, "password", encryptedPassword);
                xmlwriter.SetValue(Helper.PLUGIN_NAME, "resource", resource);
            }
        }

        /// <summary>
        /// Decrypt an encrypted setting string
        /// </summary>
        /// <param name="encrypted">The string to decrypt</param>
        /// <returns>The decrypted string or an empty string if something went wrong</returns>
        private static string decryptString(string encrypted) {
            string decrypted = String.Empty;

            EncryptDecrypt Crypto = new EncryptDecrypt();
            try {
                decrypted = Crypto.Decrypt(encrypted);
            } catch (Exception) {
                MediaPortal.GUI.Library.Log.Error("Could not decrypt config string!");
            }

            return decrypted;
        }

        /// <summary>
        /// Encrypt a setting string
        /// </summary>
        /// <param name="decrypted">An unencrypted string</param>
        /// <returns>The string encrypted</returns>
        private static string encryptString(string decrypted) {
            EncryptDecrypt Crypto = new EncryptDecrypt();
            string encrypted = String.Empty;

            try {
                encrypted = Crypto.Encrypt(decrypted);
            } catch (Exception) {
                MediaPortal.GUI.Library.Log.Error("Could not encrypt setting string!");
                encrypted = String.Empty;
            }

            return encrypted;
        }
    }
}
