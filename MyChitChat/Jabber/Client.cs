using System;
using agsXMPP;
using agsXMPP.protocol.client;
using agsXMPP.protocol.iq.roster;
using agsXMPP.protocol.iq.vcard;
using MediaPortal.GUI.Library;
using MyChitChat.Plugin;
using System.Collections.Generic;

namespace MyChitChat.Jabber {
    /// <summary>
    /// Delegate for the TestCompleted event
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void TestCompletedEventHandler(object sender, TestEventArgs e);
    public delegate void OnLoginEventHandler(object sender);
    public delegate void OnMessageEventHandler(Message newMsg);
    public delegate void OnPresenceEventHandler(RosterContact contact);
    public delegate void OnErrorEventHandler(Exception e);    
    public delegate void OnRosterItemEventHandler(object sender, Jid jid);
    public delegate void OnRosterStartEventHandler();
    public delegate void OnRosterEndEventHandler();

    public sealed class Client {
        static readonly Client instance = new Client();
        // Explicit static constructor to tell C# compiler
        // not to mark type as beforefieldinit
        static Client() {
        }

        Client() {
            this._roster = new Roster();           
        }

        public static Client Instance {
            get { return instance; }
        }

        public Roster Roster {
            get { return this._roster; }
        }

        public MessageGrabber MessageGrabber {
            get { return this._connection.MessageGrabber; }
        }

        public bool Connected {
            get { return _connection.Authenticated; }
        }

        public Jid MyJID {
            get { return _connection.MyJID; }
        }

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
        private Roster _roster;
        
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
        public event OnLoginEventHandler OnLogin;
        public event OnMessageEventHandler OnMessage;
        public event OnErrorEventHandler OnError;        
        public event OnPresenceEventHandler OnPresence;
        public event OnRosterItemEventHandler OnRosterItem;
        public event OnRosterStartEventHandler OnRosterStart;
        public event OnRosterEndEventHandler OnRosterEnd;

        #endregion

        #region Methods


        /// <summary>
        /// Test the connection settings
        /// </summary>
        public void TestSettings() {
            _connection.Priority = -1;
            _isTest = true;
        }

        /// <summary>
        /// Connect to the jabber server,
        /// connection is done async.
        /// </summary>
        public void Connect() {
            _connection.Password = Settings.Password;
            _connection.Username = Settings.Username;
            _connection.Server = Settings.Server;
            _connection.Resource = Settings.Resource;

            _connection.EnableCapabilities = true;

            _connection.AutoAgents = false;
            _connection.AutoPresence = true;
            _connection.AutoRoster = true;
            _connection.AutoResolveConnectServer = true;

            try {
                _connection.OnLogin += new ObjectHandler(_connection_OnLogin);
                _connection.OnClose += new ObjectHandler(_connection_OnClose);
                _connection.OnMessage += new agsXMPP.protocol.client.MessageHandler(_connection_OnMessage);
                _connection.OnRosterItem += new XmppClientConnection.RosterHandler(_connection_OnRosterItem);
                _connection.OnRosterStart += new ObjectHandler(_connection_OnRosterStart);
                _connection.OnRosterEnd += new ObjectHandler(_connection_OnRosterEnd);
                _connection.OnPresence += new PresenceHandler(_connection_OnPresence);
                _connection.OnError += new ErrorHandler(_connection_OnError);
                _connection.OnAuthError += new XmppElementHandler(_connection_OnAuthError);
                _connection.OnSocketError += new ErrorHandler(_connection_OnSocketError);
                _connection.OnStreamError += new XmppElementHandler(_connection_OnStreamError);
                _connection.Open();
            } catch (Exception e) {
                Log.Error(e.Message);
            }
        }

        public void SendyMyPresence(Presence myPresence) {
            _connection.Show = myPresence.Show;
            _connection.Status = myPresence.Status;
            _connection.SendMyPresence();
        }

        void _connection_OnRosterStart(object sender) {
            OnRosterStart();
        }

        void _connection_OnRosterEnd(object sender) {
            OnRosterEnd();
        }

        void _connection_OnRosterItem(object sender, RosterItem item) {
            _roster.AddRosterItem(item);
            OnRosterItem(sender, item.Jid);
        }


        void _connection_OnPresence(object sender, Presence pres) {
            _roster.SetPresence(pres);
            RosterContact presentContact = _roster.GetRosterContact(pres.From);           
            if(presentContact != null && presentContact.Online && !presentContact.Group.Contains("Transport")){
                OnPresence(presentContact);
            }
        }

        /// <summary>
        /// Refresh/Request the Roster
        /// </summary>
        public void RefreshRoster() {
            this._connection.RequestRoster();
        }

        public void RequestVcard(Jid jid, IqCB VcardResult) {
            VcardIq viq = new VcardIq(IqType.get, new Jid( jid.Bare));
            _connection.IqGrabber.SendIq(viq, VcardResult, null);            
        }
       

        /// <summary>
        /// Disconnect from the server
        /// </summary>
        public void Disconnect() {
            _disconnect = true;
            _connection.Close();
        }

        /// <summary>
        /// Waits for some seconds then tries to reconnect to the jabber server
        /// </summary>
        public void Reconnect() {
            if (reconnectCounter < reconnectTries) {
                reconnectCounter++;
                Log.Info(String.Format("Jabber trying to reconnect (Try {0}) ...", reconnectCounter));
                System.Threading.Thread.Sleep(reconnectTimeout);
                Connect();
            } else {
                _disconnect = true;
            }
        }

        /// <summary>
        /// Send a jabber message
        /// </summary>
        /// <param name="Message">The message to send</param>
        /// <param name="To">Receiver of the message</param>
        public Message SendMessage(string Message, Jid To) {
           return SendMessage(Message, To, MessageType.chat);
        }

        public Message SendMessage(string Message, Jid To, MessageType Type) {
            agsXMPP.protocol.client.Message tmpMsg = new agsXMPP.protocol.client.Message(To, Type, Message);
            _connection.Send(tmpMsg);
            return new Message(tmpMsg, MessageTypes.Outgoing, DateTime.Now);
        }       

        #endregion

        #region Events

        /// <summary>
        /// A stream error occured.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void _connection_OnStreamError(object sender, agsXMPP.Xml.Dom.Element e) {
            Log.Error(String.Format("Jabber stream error: {0}", e.ToString()));
        }

        /// <summary>
        /// A socket error has occured
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="ex"></param>
        void _connection_OnSocketError(object sender, Exception ex) {
            if (_isTest) {
                TestEventArgs args = new TestEventArgs(false);
                args.Exception = ex;

                TestCompleted(this, args);
            } else {
                Reconnect();
            }

            Log.Error(String.Format("Jabber socket error: {0}", ex.Message));
        }

        /// <summary>
        /// Login completed
        /// </summary>
        /// <param name="sender"></param>
        void _connection_OnLogin(object sender) {
            reconnectCounter = 0;

            // This was a test for the login credentials
            if (_isTest) {
                TestCompleted(this, new TestEventArgs(true));
                return;
            }

            Log.Info("Login to jabber server completed.");            
            OnLogin(sender);
        }


        /// <summary>
        /// An error has occured.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="ex"></param>
        void _connection_OnError(object sender, Exception ex) {
            // This was a test for the login credentials
            if (_isTest) {
                TestCompleted(this, new TestEventArgs(false));
            }

            Log.Error(String.Format("Jabber error: {0}", ex.Message));
            OnError(ex);
        }

        /// <summary>
        /// A jabber message has been received
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="msg"></param>
        void _connection_OnMessage(object sender, agsXMPP.protocol.client.Message msg) {
            Log.Debug(String.Format("New jabber message from {0}: {1}", msg.From.ToString(), msg.Body));
            OnMessage(new Message(msg,MessageTypes.Incoming, DateTime.Now));            
        }

        /// <summary>
        /// Disconnected.
        /// </summary>
        /// <param name="sender"></param>
        void _connection_OnClose(object sender) {
            if (!_disconnect) {
                Reconnect();
            } else {
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
        void _connection_OnAuthError(object sender, agsXMPP.Xml.Dom.Element e) {
            // This was a test for the login credentials
            if (_isTest) {
                TestCompleted(this, new TestEventArgs(false));
            }

            Log.Error("Your jabber username or password is invalid");
        }

        #endregion
    }

    /// <summary>
    /// Event args for the test event
    /// </summary>
    public class TestEventArgs : EventArgs {
        /// <summary>
        /// Was the test successful?
        /// </summary>
        public bool Success {
            get { return success; }
            set { success = value; }
        }
        private bool success;

        public Exception Exception {
            get { return exception; }
            set { exception = value; }
        }
        private Exception exception = null;

        public TestEventArgs(bool WasSuccess) {
            success = WasSuccess;
        }
    }
}
