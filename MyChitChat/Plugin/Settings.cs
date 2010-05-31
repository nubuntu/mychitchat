using System;
using System.Collections.Generic;
using System.Text;
using MediaPortal;

namespace MyChitChat.Plugin {
    class Settings {
        #region Properties

        /// <summary>
        /// The jabber username
        /// </summary>
        public static string Username {
            get { return username; }
            set { username = value; }
        }
        private static string username = String.Empty;

        /// <summary>
        /// The jabber server
        /// </summary>
        public static string Server {
            get { return server; }
            set { server = value; }
        }
        private static string server = String.Empty;

        /// <summary>
        /// The jabber resource
        /// </summary>
        public static string Resource {
            get { return resource; }
            set { resource = value; }
        }
        private static string resource = "MediaPortal";

        /// <summary>
        /// The jabber password
        /// </summary>
        public static string Password {
            get { return password; }
            set { password = value; }
        }
        private static string password = String.Empty;

        /// <summary>
        /// The jabber password
        /// </summary>
        public static int NotifyTimeOut {
            get { return notifyTimeOut; }
            set { notifyTimeOut = value; }
        }
        private static int notifyTimeOut = 10;

        /// <summary>
        /// Automatically connect to Jabber on MediaPortal startup / on Plugin started
        /// </summary>
        public static bool AutoConnectStartup {
            get { return autoConnectStartup; }
            set { autoConnectStartup = value; }
        }
        private static bool autoConnectStartup = true;

        /// <summary>
        /// The jabber password
        /// </summary>
        public static bool NotifyOnMessage {
            get { return notifyOnMessage; }
            set { notifyOnMessage = value; }
        }
        private static bool notifyOnMessage = true;

        /// <summary>
        /// The jabber password
        /// </summary>
        public static bool NotifyOnPresence {
            get { return notifyOnPresence; }
            set { notifyOnPresence = value; }
        }
        private static bool notifyOnPresence = true;


        #endregion

        /// <summary>
        /// Load the settings from the mediaportal config
        /// </summary>
        public static void Load() {
            using (MediaPortal.Profile.Settings reader = new MediaPortal.Profile.Settings(MediaPortal.Configuration.Config.GetFile(MediaPortal.Configuration.Config.Dir.Config, "MediaPortal.xml"))) {
                Username = reader.GetValue(Helper.PLUGIN_NAME, "username");
                Server = reader.GetValue(Helper.PLUGIN_NAME, "server");
                Resource = reader.GetValue(Helper.PLUGIN_NAME, "resource");

                string encryptedPassword = reader.GetValue(Helper.PLUGIN_NAME, "password");
                Password = decryptString(encryptedPassword);
            }
        }

        /// <summary>
        /// Save the settings to the MP config
        /// </summary>
        public static void Save() {
            using (MediaPortal.Profile.Settings xmlwriter = new MediaPortal.Profile.Settings(MediaPortal.Configuration.Config.GetFile(MediaPortal.Configuration.Config.Dir.Config, "MediaPortal.xml"))) {
                // Encrypt the password
                string encryptedPassword = encryptString(Password);

                xmlwriter.SetValue(Helper.PLUGIN_NAME, "username", Username);
                xmlwriter.SetValue(Helper.PLUGIN_NAME, "server", Server);
                xmlwriter.SetValue(Helper.PLUGIN_NAME, "password", encryptedPassword);
                xmlwriter.SetValue(Helper.PLUGIN_NAME, "resource", Resource);
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
