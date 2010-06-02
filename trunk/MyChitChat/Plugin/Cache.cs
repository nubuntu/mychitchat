//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using agsXMPP.protocol.iq.vcard;
//using System.IO;
//using System.Text.RegularExpressions;
//using agsXMPP;

//namespace MyChitChat.Plugin {
//    static class Cache {

//        public static void SaveVCardPhoto(Vcard vCard) {            
//            return vCard.Photo.Image.Save( String.Format("vCard_{0}.png", vCard.JabberId.GetHashCode()), System.Drawing.Imaging.ImageFormat.Png);
//        }

//        public static string GetVCardPhoto(Jid jid) { 
//            string tmpFileName = String.Format("vCard_{0}.png", jid.GetHashCode());
//            if (File.Exists(tmpFileName)) {
//                return tmpFileName;
//            } else {
//                return String.Empty;
//            }
//        }
        
//        public static void CacheVCard(Vcard vcard, string jid) {
//            try {
//                DirectoryInfo directoryInfo = GetCacheFolder();

//                using (
//                        FileStream fileStream = new FileStream(string.Format("{0}\\{1:d}", directoryInfo.FullName, jid.GetHashCode()),
//                                                                FileMode.Create, FileAccess.Write, FileShare.None)) {
//                    using (StreamWriter streamWriter = new StreamWriter(fileStream)) {
//                        string vcardXml = string.Format("<vCard>{0}</vCard>", vcard.InnerXml);
//                        streamWriter.Write(vcardXml);
//                    }
//                }
//            } catch (Exception e) {
//                Client.Instance.Log(e.Message);
//            }
//        }

//        public static Vcard GetVcard(string jid) {
//            Vcard vcard = null;

//            try {
//                DirectoryInfo directoryInfo = GetCacheFolder();

//                using (
//                        FileStream fileStream = new FileStream(string.Format("{0}\\{1:d}", directoryInfo.FullName, jid.GetHashCode()),
//                                                                FileMode.Open, FileAccess.Read, FileShare.Read)) {
//                    using (StreamReader streamReader = new StreamReader(fileStream)) {
//                        Document doc = new Document();
//                        doc.LoadXml(streamReader.ReadToEnd());

//                        if (doc.RootElement != null) {
//                            vcard = new Vcard();

//                            foreach (Node node in doc.RootElement.ChildNodes) {
//                                vcard.AddChild(node);
//                            }
//                        }
//                    }
//                }
//            } catch (Exception e) {
//                Client.Instance.Log(e.Message);
//            }

//            return vcard;
//        }


//    }
//}
