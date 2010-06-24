using System;
using System.IO;
using MediaPortal.GUI.Library;
using nJim;

namespace MyChitChat.Plugin {
    static class Cache {

        private static string thumbDir = MediaPortal.Configuration.Config.GetFolder(MediaPortal.Configuration.Config.Dir.Thumbs) + @"\" + Helper.PLUGIN_NAME + @"\";

        public static void SaveAvatarImage(Identity contactIdentity) {
            try {
                if (!Directory.Exists(thumbDir)) {
                    Directory.CreateDirectory(thumbDir);
                }
                if (contactIdentity.photo != null) {
                    contactIdentity.photo.Save(String.Format(thumbDir + "avatar_{0}.png", GetSafeFileName(contactIdentity.jabberID)));
                }
            } catch (Exception e) {
                Log.Error(e);
            }
        }

      
        public static string GetAvatarImagePath(Identity contactIdentity) {
            if (contactIdentity == null) {
                return string.Empty;
            }
            string tmpFileName = String.Format(thumbDir + "avatar_{0}.png", GetSafeFileName(contactIdentity.jabberID));
            if (File.Exists(tmpFileName)) {
                return tmpFileName;
            } else {
                return String.Empty;
            }
        }

        public static string GetSafeFileName(JabberID contact) {
            string safe = contact.full;
            foreach (char lDisallowed in System.IO.Path.GetInvalidFileNameChars()) {
                safe = safe.Replace(lDisallowed.ToString(), "");
            }
            foreach (char lDisallowed in System.IO.Path.GetInvalidPathChars()) {
                safe = safe.Replace(lDisallowed.ToString(), "");
            }
            return safe;
        }


    }

}
