using System;
using System.IO;
using MediaPortal.GUI.Library;
using nJim;

namespace MyChitChat.Plugin {
    static class Cache {

        private static string thumbDir = MediaPortal.Configuration.Config.Dir.Thumbs + @"\" + Helper.PLUGIN_NAME + @"\";

        private static void SaveAvatarImage(Identity contactIdentity, string filename) {
            try {
                if (!Directory.Exists(thumbDir)) {
                    Directory.CreateDirectory(thumbDir);
                }
                contactIdentity.photo.Save(filename, contactIdentity.photoFormat);
            } catch (Exception e) {
                Log.Error(e);
            }
        }

        public static string GetAvatarImagePath(Identity contactIdentity) {
            if (contactIdentity == null || contactIdentity.photo == null) {
                return string.Empty;
            }
            string tmpFileName = String.Format(thumbDir + "avatar_{0}.png", contactIdentity.jabberID.GetHashCode());
            SaveAvatarImage(contactIdentity, tmpFileName);
            if (File.Exists(tmpFileName)) {
                return tmpFileName;
            } else {
                return String.Empty;
            }
        }

    }
  
}
