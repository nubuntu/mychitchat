using System;
using System.Collections.Generic;
using System.Text;
using agsXMPP;
using MediaPortal.GUI.Library;
using agsXMPP.protocol.client;
using MediaPortal.Dialogs;
using System.Text.RegularExpressions;

namespace Jabber.MP
{
    /// <summary>
    /// Delegate for the TestCompleted event
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void TestCompletedEventHandler(object sender, TestEventArgs e);

    class Jabber
    {
        #region Private members

        /// <summary>
        /// If a message contains more lines than this a 
        /// dialogText control will be used to display the
        /// message.
        /// </summary>
        private const int maximumLinesForOkDialog = 4;

        /// <summary>
        /// Milliseconds to wait before trying to reconnect
        /// </summary>
        private const int reconnectTimeout = 10000;

        /// <summary>
        /// Number of reconnects before giving up
        /// </summary>
        private const uint reconnectTries = 5;

        /// <summary>
        /// Number of tries without successful connect
        /// </summary>
        private uint reconnectCounter = 0;

        /// <summary>
        /// The connection to the jabber server
        /// </summary>
        private XmppClientConnection _connection = new XmppClientConnection();

        /// <summary>
        /// Was a disconnect requested?
        /// </summary>
        private bool _disconnect = false;

        /// <summary>
        /// Is this a test for the settings?
        /// </summary>
        private bool _isTest = false;
        
        #endregion

        #region Public members

        /// <summary>
        /// Event fired when the test is completed
        /// </summary>
        public event TestCompletedEventHandler TestCompleted;

        #endregion

        #region Methods

        /// <summary>
        /// Constructor.
        /// </summary>
        public Jabber()
        {

        }

        /// <summary>
        /// Test the connection settings
        /// </summary>
        public void TestSettings()
        {
            _connection.Priority = -1;
            _isTest = true;
        }

        /// <summary>
        /// Connect to the jabber server,
        /// connection is done async.
        /// </summary>
        public void Connect()
        {
            _connection.Password = Settings.Password;
            _connection.Username = Settings.Username;
            _connection.Server = Settings.Server;
            _connection.Resource = Settings.Resource;

            _connection.AutoAgents = false;
            _connection.AutoPresence = true;
            _connection.AutoRoster = true;
            _connection.AutoResolveConnectServer = true;

            try
            {
                _connection.OnLogin += new ObjectHandler(_connection_OnLogin);
                _connection.OnClose += new ObjectHandler(_connection_OnClose);
                _connection.OnMessage += new agsXMPP.protocol.client.MessageHandler(_connection_OnMessage);
                _connection.OnError += new ErrorHandler(_connection_OnError);
                _connection.OnAuthError += new XmppElementHandler(_connection_OnAuthError);
                _connection.OnSocketError += new ErrorHandler(_connection_OnSocketError);
                _connection.OnStreamError += new XmppElementHandler(_connection_OnStreamError);

                _connection.Open();
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
            }
        }

        /// <summary>
        /// Disconnect from the server
        /// </summary>
        public void Disconnect()
        {
            _disconnect = true;
            _connection.Close();
        }

        /// <summary>
        /// Waits for some seconds then tries to reconnect to the jabber server
        /// </summary>
        public void Reconnect()
        {
            if (reconnectCounter < reconnectTries)
            {
                reconnectCounter++;
                Log.Info(String.Format("Jabber trying to reconnect (Try {0}) ...", reconnectCounter));
                System.Threading.Thread.Sleep(reconnectTimeout);
                Connect();
            }
            else
            {
                _disconnect = true;
            }
        }

        /// <summary>
        /// Send a jabber message
        /// </summary>
        /// <param name="Message">The message to send</param>
        /// <param name="To">Receiver of the message</param>
        public void SendMessage(string Message, Jid To)
        {
            _connection.Send(new Message(To, MessageType.chat, Message));
        }


        /// <summary>
        /// Show message containing a question (mark).
        /// The user can answer with yes or no.
        /// </summary>
        /// <param name="msg"></param>
        private void showQuestion(Message msg)
        {
            GUIDialogYesNo dialog = (GUIDialogYesNo)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_YES_NO);
            dialog.Reset();
            dialog.SetHeading(String.Format("Message from {0}@{1}:", msg.From.User, msg.From.Server));
            dialog.SetLine(1, msg.Body);
            dialog.SetDefaultToYes(true);
            dialog.DoModal(GUIWindowManager.ActiveWindow);

            SendMessage((dialog.IsConfirmed) ? "Yes" : "No", msg.From);
        }

        /// <summary>
        /// Show a message
        /// </summary>
        /// <param name="msg"></param>
        private void showMessage(Message msg)
        {
            Regex lineCounter = new Regex("\n", RegexOptions.Multiline);
            MatchCollection lines = lineCounter.Matches(msg.Body);

            // Use a DialogText if there are more than 4 lines of text
            if (lines.Count < maximumLinesForOkDialog)
            {
                GUIDialogOK dialog = (GUIDialogOK)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_OK);
                dialog.Reset();
                dialog.SetHeading(String.Format("Message from {0}@{1}:", msg.From.User, msg.From.Server));
                dialog.SetLine(1, msg.Body);
                dialog.DoModal(GUIWindowManager.ActiveWindow);
            }
            else
            {
                GUIDialogText dialog = (GUIDialogText)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_TEXT);
                dialog.Reset();
                dialog.SetHeading(String.Format("Message from {0}@{1}:", msg.From.User, msg.From.Server));
                dialog.SetText(msg.Body);
                dialog.DoModal(GUIWindowManager.ActiveWindow);
            }
        }

        #endregion

        #region Events

        /// <summary>
        /// A stream error occured.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void _connection_OnStreamError(object sender, agsXMPP.Xml.Dom.Element e)
        {
            Log.Error(String.Format("Jabber stream error: {0}", e.ToString()));
        }

        /// <summary>
        /// A socket error has occured
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="ex"></param>
        void _connection_OnSocketError(object sender, Exception ex)
        {
            if (_isTest)
            {
                TestEventArgs args = new TestEventArgs(false);
                args.Exception = ex;

                TestCompleted(this, args);
            }
            else
            {
                Reconnect();
            }

            Log.Error(String.Format("Jabber socket error: {0}", ex.Message));
        }
        
        /// <summary>
        /// Login completed
        /// </summary>
        /// <param name="sender"></param>
        void _connection_OnLogin(object sender)
        {
            reconnectCounter = 0;

            // This was a test for the login credentials
            if (_isTest)
            {
                TestCompleted(this, new TestEventArgs(true));
                return;
            }

            Log.Info("Login to jabber server completed.");
            _connection.Show = ShowType.NONE;
            _connection.Status = "Idle";
            _connection.SendMyPresence();
        }


        /// <summary>
        /// An error has occured.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="ex"></param>
        void _connection_OnError(object sender, Exception ex)
        {
            // This was a test for the login credentials
            if (_isTest)
            {
                TestCompleted(this, new TestEventArgs(false));
            }

            Log.Error(String.Format("Jabber error: {0}", ex.Message));
        }

        /// <summary>
        /// A jabber message has been received
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="msg"></param>
        void _connection_OnMessage(object sender, agsXMPP.protocol.client.Message msg)
        {
            Log.Debug(String.Format("New jabber message from {0}: {1}", msg.From.ToString(), msg.Body));

            if (msg.Body != null && !_isTest)
            {
                // Show YES/NO dialog for messages where a question mark is contained
                // The answer (yes or no) will be submitted.
                if (msg.Body.Contains("?"))
                {
                    showQuestion(msg);
                }
                else
                {
                    showMessage(msg);
                }
            }
        }

        /// <summary>
        /// Disconnected.
        /// </summary>
        /// <param name="sender"></param>
        void _connection_OnClose(object sender)
        {
            if (!_disconnect)
            {
                Reconnect();
            }
            else
            {
                Log.Info("Jabber disconnected");
                _disconnect = false;
                reconnectCounter = 0;
            }
        }

        /// <summary>
        /// User data supplied is invalid (username, password ...)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void _connection_OnAuthError(object sender, agsXMPP.Xml.Dom.Element e)
        {
            // This was a test for the login credentials
            if (_isTest)
            {
                TestCompleted(this, new TestEventArgs(false));
            }

            Log.Error("Your jabber username or password is invalid");
        }

        #endregion
    }

    /// <summary>
    /// Event args for the test event
    /// </summary>
    public class TestEventArgs : EventArgs
    {
        /// <summary>
        /// Was the test successful?
        /// </summary>
        public bool Success
        {
            get { return success; }
            set { success = value; }
        }
        private bool success;

        public Exception Exception
        {
            get { return exception; }
            set { exception = value; }
        }
        private Exception exception = null;

        public TestEventArgs(bool WasSuccess)
        {
            success = WasSuccess;
        }
    }
}
