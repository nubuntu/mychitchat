using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Xml;
using MediaPortal.GUI.Library;

namespace MyChitChat.Plugin {
    //Most of this code was taken from the wonderful MovingPictures Plugin
    public static class Translations {

        #region Private variables

        private static Dictionary<string, string> localizedStrings;
        private static Regex translateExpr = new Regex(@"\$\{([^\}]+)\}");
        private static string path = string.Empty;

        #endregion

        #region Constructor

        static Translations() {
            string lang;

            try {
                lang = GUILocalizeStrings.GetCultureName(GUILocalizeStrings.CurrentLanguage());
            } catch (Exception) {
                // when running MovingPicturesConfigTester outside of the MediaPortal directory this happens unfortunately
                // so we grab the active culture name from the system            
                lang = CultureInfo.CurrentUICulture.Name;
            }

            Log.Info("Using language " + lang);

            path = GetPluginLanguagesPath();
            if (!System.IO.Directory.Exists(path))
                System.IO.Directory.CreateDirectory(path);

            LoadTranslations(lang);
        }

        private static string GetPluginLanguagesPath() {
            return MediaPortal.Configuration.Config.GetSubFolder(MediaPortal.Configuration.Config.Dir.Language, Helper.PLUGIN_NAME);
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the translated strings collection in the active language
        /// </summary>
        public static Dictionary<string, string> LocalizedStrings {
            get {
                if (localizedStrings == null) {
                    localizedStrings = new Dictionary<string, string>();
                    Type transType = typeof(Translations);
                    FieldInfo[] fields = transType.GetFields(BindingFlags.Public | BindingFlags.Static);
                    foreach (FieldInfo field in fields) {
                        localizedStrings.Add(field.Name, !String.IsNullOrEmpty(field.GetValue(transType).ToString()) ? field.GetValue(transType).ToString() : field.Name);
                    }
                }
                return localizedStrings;
            }
        }

        #endregion

        #region Public Methods

        public static int LoadTranslations(string lang) {
            XmlDocument doc = new XmlDocument();
            Dictionary<string, string> TranslatedStrings = new Dictionary<string, string>();
            string langPath = "";
            try {
                langPath = Path.Combine(path, lang + ".xml");
                doc.Load(langPath);
            } catch (Exception e) {
                if (lang == "en")
                    return 0; // otherwise we are in an endless loop!

                if (e.GetType() == typeof(FileNotFoundException))
                    Log.Warn("Cannot find translation file {0}.  Failing back to English", langPath);
                else
                    Log.Error(new Exception(String.Format("Error in translation xml file: {0}. Failing back to English", lang), e));

                return LoadTranslations("en");
            }
            foreach (XmlNode stringEntry in doc.DocumentElement.ChildNodes) {
                if (stringEntry.NodeType == XmlNodeType.Element)
                    try {
                        TranslatedStrings.Add(stringEntry.Attributes.GetNamedItem("Field").Value, stringEntry.InnerText);
                    } catch (Exception ex) {
                        Log.Error(new Exception("Error in Translation Engine", ex));
                    }
            }

            Type TransType = typeof(Translations);
            FieldInfo[] fieldInfos = TransType.GetFields(BindingFlags.Public | BindingFlags.Static);
            foreach (FieldInfo fi in fieldInfos) {
                if (TranslatedStrings != null && TranslatedStrings.ContainsKey(fi.Name))
                    TransType.InvokeMember(fi.Name, BindingFlags.SetField, null, TransType, new object[] { TranslatedStrings[fi.Name] });
                else
                    Log.Info("Translation not found for field: {0}.  Using hard-coded English default.", fi.Name);
            }
            return TranslatedStrings.Count;
        }

        public static string GetByName(string name) {
            if (!LocalizedStrings.ContainsKey(name))
                return name;

            return LocalizedStrings[name];
        }

        public static string GetByName(string name, params object[] args) {
            return String.Format(GetByName(name), args);
        }

        public static string[] TranslateEnumToList<T>() {
            Type enumType = typeof(T);
            // Can't use type constraints on value types, so have to do check like this
            if (enumType.BaseType != typeof(Enum))
                throw new ArgumentException("T must be of type System.Enum");

            string[] enumValArray = Enum.GetNames(enumType);

            return enumValArray;
        }

        public static List<KeyValuePair<string, T>> EnumToList<T>() {
            Type enumType = typeof(T);

            // Can't use type constraints on value types, so have to do check like this
            if (enumType.BaseType != typeof(Enum))
                throw new ArgumentException("T must be of type System.Enum");

            Array enumValArray = Enum.GetValues(enumType);

            List<KeyValuePair<string, T>> enumValList = new List<KeyValuePair<string, T>>(enumValArray.Length);

            foreach (int val in enumValArray) {
                T tmpType = (T)Enum.Parse(enumType, val.ToString());
                enumValList.Add(new KeyValuePair<string, T>(GetByName(tmpType.ToString()), tmpType));
            }

            return enumValList;
        }

        public static string CreateTranslationTemplate(string language) {
            LoadTranslations(language);
            string languageFilePath = Path.Combine(GetPluginLanguagesPath(), language + ".xml");
            using (XmlTextWriter writer = new XmlTextWriter( languageFilePath, System.Text.Encoding.UTF8) ){
                writer.Formatting = Formatting.Indented;
                writer.WriteStartDocument();
                writer.WriteComment("MyChitChat translation file");
                writer.WriteComment("Language: " + language);
                writer.WriteComment("Note: English is the fallback for any strings not found in other languages");
                writer.WriteComment("Contributed by [Your Name]");
                writer.WriteComment("=========================================================================");
                writer.WriteStartElement("strings"); // <-- Important root element
                foreach (KeyValuePair<string, string> currentField in LocalizedStrings) {
                    writer.WriteStartElement("string");                    
                    writer.WriteAttributeString("Field", currentField.Key);
                    writer.WriteValue(currentField.Value);
                    writer.WriteEndElement();
                }
                writer.WriteEndElement();              // <-- Closes it
                writer.WriteEndDocument();
                writer.Flush();
                writer.Close();
            }
            return languageFilePath;
        }

        public static List<string> GetCultureLanguages() {
            CultureInfo[] cultureList = CultureInfo.GetCultures(CultureTypes.AllCultures);
            List<string> languages = new List<string>();

            foreach (CultureInfo current in cultureList) {
                languages.Add(current.Name);
            }
            return languages;
        }


        public static string GetCurrentCultureLanguage() {
            return GUILocalizeStrings.GetCultureName(GUILocalizeStrings.CurrentLanguage());
        }


        /// <summary>
        /// Takes an input string and replaces all ${named} variables with the proper translation if available
        /// </summary>
        /// <param name="input">a string containing ${named} variables that represent the translation keys</param>
        /// <returns>translated input string</returns>
        public static string ParseString(string input) {
            MatchCollection matches = translateExpr.Matches(input);
            foreach (Match match in matches) {
                input = input.Replace(match.Value, GetByName(match.Groups[1].Value));
            }
            return input;
        }

        #endregion

        #region Translations / Strings


        /* Plugin default strings - these are overridden by the user defined strings in the plugin config */

        public static string defaultStatusMessage = Normal + " using " + Helper.PLUGIN_NAME;
        public static string defaultMoodMessage = "I'm feeling very entertained right now thx to MediaPortal ('Ya know the kick-ass open source MediaCenter alternative)...";
        public static string defaultActivityMessage = "I'm using MediaPortal at the moment...";
        public static string defaultIdleMessage = Helper.PLUGIN_NAME + " idle at the moment...";

        public static string WINDOW_DIALOG_AUTO = "Automatically use a suitable dialog for Message length";
        public static string WINDOW_DIALOG_NOTIFY = "Notify Window (lower right)";
        public static string WINDOW_DIALOG_OK = "Dialog Window (small) (centered)";
        public static string WINDOW_DIALOG_TEXT = "Dialog Window (large) (centered)";

        public static string BtnSelectStatus = "Set My Status";
        public static string BtnSelectActivity = "Set My Activity";
        public static string BtnSelectMood = "Set My Mood";
        public static string BtnSendNewMessage = "IM selected Contact";
        public static string BtnFilterNone = "Show all Contacts";
        public static string BtnFilterOnline = "Show Online only";
        public static string BtnFilterOffline = "Show Offline only";
        public static string BtnJabberReconnect = "Jabber reconnect";
        public static string BtnJabberDisconnect = "Jabber disconnect";
        public static string BtnSelectKeyboard = "Set Keyboard type";
        public static string NothingSelected = "Cancel";


        public static string none = "";

        /* Jabber / XMPP Status Types */

        public static string Normal = "Available";
        public static string ReadyToChat = "Ready to chat";
        public static string Away = "Away";
        public static string ExtendedAway = "Extended away";
        public static string DoNotDisturb = "Do not disturb";
        public static string Unavailable = "Unvailable";
        public static string Invisible = "Invisible";

        /* Jabber / XMPP Mood Types */

        public static string afraid = "";
        public static string amazed = "";
        public static string angry = "";
        public static string annoyed = "";
        public static string anxious = "";
        public static string aroused = "";
        public static string ashamed = "";
        public static string bored = "";
        public static string brave = "";
        public static string calm = "";
        public static string cold = "";
        public static string confused = "";
        public static string contented = "";
        public static string cranky = "";
        public static string curious = "";
        public static string depressed = "";
        public static string disappointed = "";
        public static string disgusted = "";
        public static string distracted = "";
        public static string embarrassed = "";
        public static string excited = "";
        public static string flirtatious = "";
        public static string frustrated = "";
        public static string grumpy = "";
        public static string guilty = "";
        public static string happy = "";
        public static string hot = "";
        public static string humbled = "";
        public static string humiliated = "";
        public static string hungry = "";
        public static string hurt = "";
        public static string impressed = "";
        public static string in_awe = "in awe";
        public static string in_love = "in love";
        public static string indignant = "";
        public static string interested = "";
        public static string intoxicated = "";
        public static string invincible = "";
        public static string jealous = "";
        public static string lonely = "";
        public static string mean = "";
        public static string moody = "";
        public static string nervous = "";
        public static string neutral = "";
        public static string offended = "";
        public static string playful = "";
        public static string proud = "";
        public static string relieved = "";
        public static string remorseful = "";
        public static string restless = "";
        public static string sad = "";
        public static string sarcastic = "";
        public static string serious = "";
        public static string shocked = "";
        public static string shy = "";
        public static string sick = "";
        public static string sleepy = "";
        public static string stressed = "";
        public static string surprised = "";
        public static string thirsty = "";
        public static string worried = "";

        /* Jabber / XMPP Activity Types */

        public static string doing_chores = "doing chores";
        public static string buying_groceries = "buying groceries";
        public static string cleaning = "";
        public static string cooking = "";
        public static string doing_maintenance = "doing maintenance";
        public static string doing_the_dishes = "doing the dishes";
        public static string doing_the_laundry = "doing the laundry";
        public static string gardening = "";
        public static string running_an_errand = "running an errand";
        public static string walking_the_dog = "walking the dog";
        public static string drinking = "";
        public static string having_a_beer = "having a beer";
        public static string having_coffee = "having coffee";
        public static string having_tea = "having tea";
        public static string eating = "";
        public static string having_a_snack = "having a snack";
        public static string having_breakfast = "having breakfast";
        public static string having_dinner = "having dinner";
        public static string having_lunch = "having lunch";
        public static string exercising = "";
        public static string cycling = "";
        public static string hiking = "";
        public static string jogging = "";
        public static string playing_sports = "playing sports";
        public static string running = "";
        public static string skiing = "";
        public static string swimming = "";
        public static string working_out = "working out";
        public static string grooming = "";
        public static string at_the_spa = "at the spa";
        public static string brushing_teeth = "brushing teeth";
        public static string getting_a_haircut = "getting a haircut";
        public static string shaving = "";
        public static string taking_a_bath = "taking a bath";
        public static string taking_a_shower = "taking a shower";
        public static string having_appointment = "having appointment";
        public static string inactive = "";
        public static string day_off = "day off";
        public static string hanging_out = "hanging out";
        public static string on_vacation = "on vacation";
        public static string scheduled_holiday = "scheduled holiday";
        public static string sleeping = "";
        public static string relaxing = "";
        public static string gaming = "";
        public static string going_out = "going out";
        public static string partying = "";
        public static string reading = "";
        public static string rehearsing = "";
        public static string shopping = "";
        public static string socializing = "";
        public static string sunbathing = "";
        public static string watching_tv = "watching tv";
        public static string watching_a_movie = "watching a movie";
        public static string talking = "";
        public static string in_real_life = "in real life";
        public static string on_the_phone = "on the phone";
        public static string on_video_phone = "on video phone";
        public static string traveling = "";
        public static string commuting = "";
        public static string driving = "";
        public static string in_a_car = "in a car";
        public static string on_a_bus = "on a bus";
        public static string on_a_plane = "on a plane";
        public static string on_a_train = "on a train";
        public static string on_a_trip = "on a trip";
        public static string walking = "";
        public static string working = "";
        public static string coding = "";
        public static string in_a_meeting = "in a meeting";
        public static string studying = "";
        public static string writing = "";

        #endregion

    }

}

