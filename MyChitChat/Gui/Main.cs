using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MediaPortal.GUI.Library;
using MyChitChat.Plugin;
using System.Windows.Forms;
using MyChitChat.Jabber;
using MediaPortal.Dialogs;

namespace MyChitChat.Gui {
    public class Main : GUIWindow {

        #region Private members

        #endregion

        public Main() {
            Helper.JabberClient.OnLogin += new OnLoginEventHandler(JabberClient_OnLogin);
            Helper.JabberClient.OnMessage += new OnMessageEventHandler(JabberClient_OnMessage);
        }

        void JabberClient_OnMessage(object sender, agsXMPP.protocol.client.Message msg) {
            
            ShowNotifyDialog(msg.From.User.Replace("%", "@"), GUIGraphicsContext.Skin + @"\Media\MyChitChat_incoming_message.png", msg.Body);

        }

        ~Main() {
            Helper.JabberClient.Disconnect();
        }

        public override bool Init() {
            if (Settings.StartWithMediaPortal) {
                Helper.JabberClient.Connect();
            }
            return Load(Helper.SKINFILE_WINDOW_MAIN);
        }

        void JabberClient_OnLogin(object sender, TestEventArgs e) {
            //ShowNotifyDialog("MyChitChat loaded...");
        }


        protected override void OnWindowLoaded() {
            if (!Settings.StartWithMediaPortal) {
                Helper.JabberClient.Connect();
            }
            base.OnWindowLoaded();
        }

        public static void ShowNotifyDialog(string notifyMessage) {
            ShowNotifyDialog(Helper.PLUGIN_NAME, GUIGraphicsContext.Skin + @"\Media\MyChitChat_read_message.png", notifyMessage);
        }

        public static void ShowNotifyDialog(string title, string icon, string message) {
            try {
                GUIDialogNotify dialogMailNotify = (GUIDialogNotify)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_NOTIFY);
                dialogMailNotify.TimeOut = Settings.NotifyTimeOut;
                dialogMailNotify.SetImage(icon);
                dialogMailNotify.SetHeading(title);
                dialogMailNotify.SetText(message);
                dialogMailNotify.DoModal(GUIWindowManager.ActiveWindow);
            } catch (Exception ex) {
                Log.Error(ex);
            }
        }

        // With GetID it will be an window-plugin / otherwise a process-plugin
        // Enter the id number here again
        public override int GetID {
            get {
                return Helper.WINDOW_ID_MAIN;
            }
            set {
            }
        }

    }
}
