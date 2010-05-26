using System;
using System.Collections.Generic;
using System.Text;
using MediaPortal.Configuration;

namespace Jabber.MP
{
    class Settings
    {
        #region Properties

        /// <summary>
        /// The jabber username
        /// </summary>
        public static string Username
        {
            get { return username; }
            set { username = value; }
        }
        private static string username = String.Empty;

        /// <summary>
        /// The jabber server
        /// </summary>
        public static string Server
        {
            get { return server; }
            set { server = value; }
        }
        private static string server = String.Empty;

        /// <summary>
        /// The jabber resource
        /// </summary>
        public static string Resource
        {
            get { return resource; }
            set { resource = value; }
        }
        private static string resource = "MediaPortal";

        /// <summary>
        /// The jabber password
        /// </summary>
        public static string Password
        {
            get { return password; }
            set { password = value; }
        }
        private static string password = String.Empty;

        #endregion

        /// <summary>
        /// Load the settings from the mediaportal config
        /// </summary>
        public static void Load()
        {
            using (MediaPortal.Profile.Settings reader = new MediaPortal.Profile.Settings(Config.GetFile(Config.Dir.Config, "MediaPortal.xml")))
            {
                Username = reader.GetValue(JabberMP.ConfigSection, "username");
                Server = reader.GetValue(JabberMP.ConfigSection, "server");
                Resource = reader.GetValue(JabberMP.ConfigSection, "resource");

                string encryptedPassword = reader.GetValue(JabberMP.ConfigSection, "password");
                Password = decryptString(encryptedPassword);
            }
        }

        /// <summary>
        /// Save the settings to the MP config
        /// </summary>
        public static void Save()
        {
            using (MediaPortal.Profile.Settings xmlwriter = new MediaPortal.Profile.Settings(Config.GetFile(Config.Dir.Config, "MediaPortal.xml")))
            {
                // Encrypt the password
                string encryptedPassword = encryptString(Password);

                xmlwriter.SetValue(JabberMP.ConfigSection, "username", Username);
                xmlwriter.SetValue(JabberMP.ConfigSection, "server", Server);
                xmlwriter.SetValue(JabberMP.ConfigSection, "password", encryptedPassword);
                xmlwriter.SetValue(JabberMP.ConfigSection, "resource", Resource);
            }
        }

        /// <summary>
        /// Decrypt an encrypted setting string
        /// </summary>
        /// <param name="encrypted">The string to decrypt</param>
        /// <returns>The decrypted string or an empty string if something went wrong</returns>
        private static string decryptString(string encrypted)
        {
            string decrypted = String.Empty;

            EncryptDecrypt Crypto = new EncryptDecrypt();
            try
            {
                decrypted = Crypto.Decrypt(encrypted);
            }
            catch (Exception)
            {
                MediaPortal.GUI.Library.Log.Error("Could not decrypt config string!");
            }

            return decrypted;
        }

        /// <summary>
        /// Encrypt a setting string
        /// </summary>
        /// <param name="decrypted">An unencrypted string</param>
        /// <returns>The string encrypted</returns>
        private static string encryptString(string decrypted)
        {
            EncryptDecrypt Crypto = new EncryptDecrypt();
            string encrypted = String.Empty;

            try
            {
                encrypted = Crypto.Encrypt(decrypted);
            }
            catch (Exception)
            {
                MediaPortal.GUI.Library.Log.Error("Could not encrypt setting string!");
                encrypted = String.Empty;
            }

            return encrypted;
        }
    }
}
