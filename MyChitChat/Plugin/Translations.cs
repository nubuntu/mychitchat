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

            path = MediaPortal.Configuration.Config.GetSubFolder(MediaPortal.Configuration.Config.Dir.Language, Helper.PLUGIN_NAME);
            if (!System.IO.Directory.Exists(path))
                System.IO.Directory.CreateDirectory(path);

            LoadTranslations(lang);
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
                        localizedStrings.Add(field.Name, field.GetValue(transType).ToString());
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

        public static List<KeyValuePair<string,T>> EnumToList<T>() {
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

        public static string none = "";

        /* Jabber / XMPP Status Types */

        public static string Normal = "Online";
        public static string ReadyToChat = "Ready to chat";
        public static string Away = "Away";
        public static string ExtendedAway = "Extended away";
        public static string DoNotDisturb = "Do not disturb";
        public static string Unvailable = "Offline";
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
        public static string in_awe = "";
        public static string in_love = "";
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

        public static string doing_chores = "Test";
        public static string buying_groceries = "";
        public static string cleaning = "";
        public static string cooking = "";
        public static string doing_maintenance = "";
        public static string doing_the_dishes = "";
        public static string doing_the_laundry = "";
        public static string gardening = "";
        public static string running_an_errand = "";
        public static string walking_the_dog = "";
        public static string drinking = "";
        public static string having_a_beer = "";
        public static string having_coffee = "";
        public static string having_tea = "";
        public static string eating = "";
        public static string having_a_snack = "";
        public static string having_breakfast = "";
        public static string having_dinner = "";
        public static string having_lunch = "";
        public static string exercising = "";
        public static string cycling = "";
        public static string hiking = "";
        public static string jogging = "";
        public static string playing_sports = "";
        public static string running = "";
        public static string skiing = "";
        public static string swimming = "";
        public static string working_out = "";
        public static string grooming = "";
        public static string at_the_spa = "";
        public static string brushing_teeth = "";
        public static string getting_a_haircut = "";
        public static string shaving = "";
        public static string taking_a_bath = "";
        public static string taking_a_shower = "";
        public static string having_appointment = "";
        public static string inactive = "";
        public static string day_off = "";
        public static string hanging_out = "";
        public static string on_vacation = "";
        public static string scheduled_holiday = "";
        public static string sleeping = "";
        public static string relaxing = "";
        public static string gaming = "";
        public static string going_out = "";
        public static string partying = "";
        public static string reading = "";
        public static string rehearsing = "";
        public static string shopping = "";
        public static string socializing = "";
        public static string sunbathing = "";
        public static string watching_tv = "";
        public static string watching_a_movie = "";
        public static string talking = "";
        public static string in_real_life = "";
        public static string on_the_phone = "";
        public static string on_video_phone = "";
        public static string traveling = "";
        public static string commuting = "";
        public static string driving = "";
        public static string in_a_car = "";
        public static string on_a_bus = "";
        public static string on_a_plane = "";
        public static string on_a_train = "";
        public static string on_a_trip = "";
        public static string walking = "";
        public static string working = "";
        public static string coding = "";
        public static string in_a_meeting = "";
        public static string studying = "";
        public static string writing = "";

        #endregion

    }

}

