using System;
using System.Collections.Generic;
using System.Text;
using MediaPortal;
using nJim;
using MyChitChat.Gui;

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
        public static bool notifyOnMessagePlugin = true;
        public static bool notifyOnMessageGlobally = false;
        public static bool notifyOnStatusPlugin = true;
        public static bool notifyOnStatusGlobally = false;
        public static bool notifyOnMoodPlugin = true;
        public static bool notifyOnMoodGlobally = false;
        public static bool notifyOnActivityPlugin = true;
        public static bool notifyOnActivityGlobally = false;
        public static bool notifyOnTunePlugin = true;
        public static bool notifyOnTuneGlobally = false;

        public static bool notifyOnErrorPlugin = true;
        public static bool notifyOnErrorGlobally = true;
        public static bool selectStatusOnStartup = false;

        public static bool publishTuneInfo = true;
        public static bool publishActivityMusic = true;
        public static bool publishActivityRadio = true;
        public static bool publishActivityTV = false;
        public static bool publishActivityRecording = false;
        public static bool publishActivityMovie = false;

        public static int autoIdleTimeOut = 5;
        public static Enums.StatusType autoIdleStatusType = Enums.StatusType.Away;
        public static string autoIdleStatusMessage = Translations.defaultIdleMessage;

        public static Enums.StatusType defaultStatusType = Enums.StatusType.Normal;
        public static string defaultStatusMessage = Translations.defaultStatusMessage;

        public static Enums.MoodType defaultMoodType = Enums.MoodType.contented;
        public static string defaultMoodMessage = Translations.defaultMoodMessage;

        public static Enums.ActivityType defaultActivityType = Enums.ActivityType.relaxing;
        public static string defaultActivityMessage = Translations.defaultActivityMessage;

        public static Helper.PLUGIN_NOTIFY_WINDOWS notifyWindowType = Helper.PLUGIN_NOTIFY_WINDOWS.WINDOW_DIALOG_AUTO;

        public static Dialog.KeyBoardTypes defaultKeyboardType = Dialog.KeyBoardTypes.Default;

        #endregion

        /// <summary>
        /// Load the settings from the mediaportal config
        /// </summary>
        public static void Load() {
            using (MediaPortal.Profile.Settings reader = new MediaPortal.Profile.Settings(MediaPortal.Configuration.Config.GetFile(MediaPortal.Configuration.Config.Dir.Config, "MediaPortal.xml"))) {
                username = reader.GetValue(Helper.PLUGIN_NAME, "username");
                server = reader.GetValue(Helper.PLUGIN_NAME, "server");
                string tmpSettingsString = reader.GetValue(Helper.PLUGIN_NAME, "resource");
                if (!String.IsNullOrEmpty(tmpSettingsString)) {
                    resource = tmpSettingsString;
                } 
                string encryptedPassword = reader.GetValue(Helper.PLUGIN_NAME, "password");
                password = decryptString(encryptedPassword);
                autoConnectStartup = reader.GetValueAsBool(Helper.PLUGIN_NAME, "autoConnectStartup", autoConnectStartup);
                notifyOnMessagePlugin = reader.GetValueAsBool(Helper.PLUGIN_NAME, "notifyOnMessagePlugin", notifyOnMessagePlugin);
                notifyOnMessageGlobally = reader.GetValueAsBool(Helper.PLUGIN_NAME, "notifyOnMessageGlobally", notifyOnMessageGlobally);
                notifyOnStatusPlugin = reader.GetValueAsBool(Helper.PLUGIN_NAME, "notifyOnStatusPlugin", notifyOnStatusPlugin);
                notifyOnStatusGlobally = reader.GetValueAsBool(Helper.PLUGIN_NAME, "notifyOnStatusGlobally", notifyOnStatusGlobally);
                notifyOnMoodPlugin = reader.GetValueAsBool(Helper.PLUGIN_NAME, "notifyOnMoodPlugin", notifyOnMoodPlugin);
                notifyOnMoodGlobally = reader.GetValueAsBool(Helper.PLUGIN_NAME, "notifyOnMoodGlobally", notifyOnMoodGlobally);
                notifyOnActivityPlugin = reader.GetValueAsBool(Helper.PLUGIN_NAME, "notifyOnActivityPlugin", notifyOnActivityPlugin);
                notifyOnActivityGlobally = reader.GetValueAsBool(Helper.PLUGIN_NAME, "notifyOnActivityGlobally", notifyOnActivityGlobally);
                notifyOnTunePlugin = reader.GetValueAsBool(Helper.PLUGIN_NAME, "notifyOnTunePlugin", notifyOnTunePlugin);
                notifyOnTuneGlobally = reader.GetValueAsBool(Helper.PLUGIN_NAME, "notifyOnTuneGlobally", notifyOnTuneGlobally);
                notifyOnErrorPlugin = reader.GetValueAsBool(Helper.PLUGIN_NAME, "notifyOnErrorPlugin", notifyOnErrorPlugin);
                notifyOnErrorGlobally = reader.GetValueAsBool(Helper.PLUGIN_NAME, "notifyOnErrorGlobally", notifyOnErrorGlobally);
                selectStatusOnStartup = reader.GetValueAsBool(Helper.PLUGIN_NAME, "selectStatusOnStartup", selectStatusOnStartup);
                notifyTimeOut = reader.GetValueAsInt(Helper.PLUGIN_NAME, "notifyTimeOut", notifyTimeOut);
                autoIdleTimeOut = reader.GetValueAsInt(Helper.PLUGIN_NAME, "autoIdleTimeOut", autoIdleTimeOut);
                autoIdleStatusType = (Enums.StatusType)reader.GetValueAsInt(Helper.PLUGIN_NAME, "autoIdleStatusType", (int)autoIdleStatusType);
                tmpSettingsString = reader.GetValue(Helper.PLUGIN_NAME, "autoIdleStatusMessage");
                if (!String.IsNullOrEmpty(tmpSettingsString)) {
                    autoIdleStatusMessage = tmpSettingsString;
                }              
                defaultStatusType = (Enums.StatusType)reader.GetValueAsInt(Helper.PLUGIN_NAME, "defaultStatusType", (int)defaultStatusType);
                tmpSettingsString = reader.GetValue(Helper.PLUGIN_NAME, "defaultStatusMessage");
                if (!String.IsNullOrEmpty(tmpSettingsString)) {
                    defaultStatusMessage = tmpSettingsString;
                } 
                defaultMoodType = (Enums.MoodType)reader.GetValueAsInt(Helper.PLUGIN_NAME, "defaultMoodType", (int)defaultMoodType);
                tmpSettingsString = reader.GetValue(Helper.PLUGIN_NAME, "defaultMoodMessage");
                if (!String.IsNullOrEmpty(tmpSettingsString)) {
                    defaultMoodMessage = tmpSettingsString;
                }  
                defaultActivityType = (Enums.ActivityType)reader.GetValueAsInt(Helper.PLUGIN_NAME, "defaultActivityType", (int)defaultActivityType);
                tmpSettingsString = reader.GetValue(Helper.PLUGIN_NAME, "defaultActivityMessage");
                if (!String.IsNullOrEmpty(tmpSettingsString)) {
                    defaultActivityMessage = tmpSettingsString;
                }  
                notifyWindowType = (Helper.PLUGIN_NOTIFY_WINDOWS)reader.GetValueAsInt(Helper.PLUGIN_NAME, "notifyWindowType", (int)notifyWindowType);
                defaultKeyboardType = (Dialog.KeyBoardTypes)reader.GetValueAsInt(Helper.PLUGIN_NAME, "defaultKeyboardType", (int)Dialog.KeyBoardTypes.Default);

                publishTuneInfo = reader.GetValueAsBool(Helper.PLUGIN_NAME, "publishTuneInfo", publishTuneInfo);
                publishActivityMusic = reader.GetValueAsBool(Helper.PLUGIN_NAME, "publishActivityMusic", publishActivityMusic);
                publishActivityRadio = reader.GetValueAsBool(Helper.PLUGIN_NAME, "publishActivityRadio", publishActivityRadio);
                publishActivityMovie = reader.GetValueAsBool(Helper.PLUGIN_NAME, "publishActivityMovie", publishActivityMovie);
                publishActivityTV = reader.GetValueAsBool(Helper.PLUGIN_NAME, "publishActivityTV", publishActivityTV);
                publishActivityRecording = reader.GetValueAsBool(Helper.PLUGIN_NAME, "publishActivityRecording", publishActivityRecording);       
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
                xmlwriter.SetValueAsBool(Helper.PLUGIN_NAME, "autoConnectStartup", autoConnectStartup);
                xmlwriter.SetValueAsBool(Helper.PLUGIN_NAME, "notifyOnMessagePlugin", notifyOnMessagePlugin);
                xmlwriter.SetValueAsBool(Helper.PLUGIN_NAME, "notifyOnMessageGlobally", notifyOnMessageGlobally);
                xmlwriter.SetValueAsBool(Helper.PLUGIN_NAME, "notifyOnStatusPlugin", notifyOnStatusPlugin);
                xmlwriter.SetValueAsBool(Helper.PLUGIN_NAME, "notifyOnStatusGlobally", notifyOnStatusGlobally);
                xmlwriter.SetValueAsBool(Helper.PLUGIN_NAME, "notifyOnMoodPlugin", notifyOnMoodPlugin);
                xmlwriter.SetValueAsBool(Helper.PLUGIN_NAME, "notifyOnMoodGlobally", notifyOnMoodGlobally);
                xmlwriter.SetValueAsBool(Helper.PLUGIN_NAME, "notifyOnActivityPlugin", notifyOnActivityPlugin);
                xmlwriter.SetValueAsBool(Helper.PLUGIN_NAME, "notifyOnActivityGlobally", notifyOnActivityGlobally);
                xmlwriter.SetValueAsBool(Helper.PLUGIN_NAME, "notifyOnTunePlugin", notifyOnTunePlugin);
                xmlwriter.SetValueAsBool(Helper.PLUGIN_NAME, "notifyOnTuneGlobally", notifyOnTuneGlobally);
                xmlwriter.SetValueAsBool(Helper.PLUGIN_NAME, "notifyOnErrorPlugin", notifyOnErrorPlugin);
                xmlwriter.SetValueAsBool(Helper.PLUGIN_NAME, "notifyOnErrorGlobally", notifyOnErrorGlobally);
                xmlwriter.SetValueAsBool(Helper.PLUGIN_NAME, "selectStatusOnStartup", selectStatusOnStartup);
                xmlwriter.SetValue(Helper.PLUGIN_NAME, "notifyTimeOut", notifyTimeOut);
                xmlwriter.SetValue(Helper.PLUGIN_NAME, "autoIdleTimeOut", autoIdleTimeOut);
                xmlwriter.SetValue(Helper.PLUGIN_NAME, "autoIdleStatusType", (int)autoIdleStatusType);
                xmlwriter.SetValue(Helper.PLUGIN_NAME, "autoIdleStatusMessage", autoIdleStatusMessage);
                xmlwriter.SetValue(Helper.PLUGIN_NAME, "defaultStatusType", (int)defaultStatusType);
                xmlwriter.SetValue(Helper.PLUGIN_NAME, "defaultStatusMessage", defaultStatusMessage);
                xmlwriter.SetValue(Helper.PLUGIN_NAME, "defaultMoodType", (int)defaultMoodType);
                xmlwriter.SetValue(Helper.PLUGIN_NAME, "defaultMoodMessage", defaultMoodMessage);
                xmlwriter.SetValue(Helper.PLUGIN_NAME, "defaultActivityType", (int)defaultActivityType);
                xmlwriter.SetValue(Helper.PLUGIN_NAME, "defaultActivityMessage", defaultActivityMessage);
                xmlwriter.SetValue(Helper.PLUGIN_NAME, "notifyWindowType", (int)notifyWindowType);
                xmlwriter.SetValue(Helper.PLUGIN_NAME, "defaultKeyboardType", (int)defaultKeyboardType);
                xmlwriter.SetValueAsBool(Helper.PLUGIN_NAME, "publishTuneInfo", publishTuneInfo);
                xmlwriter.SetValueAsBool(Helper.PLUGIN_NAME, "publishActivityMusic", publishActivityMusic);
                xmlwriter.SetValueAsBool(Helper.PLUGIN_NAME, "publishActivityRadio", publishActivityRadio);
                xmlwriter.SetValueAsBool(Helper.PLUGIN_NAME, "publishActivityMovie", publishActivityMovie);
                xmlwriter.SetValueAsBool(Helper.PLUGIN_NAME, "publishActivityTV", publishActivityTV);
                xmlwriter.SetValueAsBool(Helper.PLUGIN_NAME, "publishActivityRecording", publishActivityRecording);          
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
