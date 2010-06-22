using System;
using MediaPortal.GUI.Library;
using MyChitChat.Plugin;
using nJim;
using agsXMPP.protocol.client;
using agsXMPP;

namespace MyChitChat.Jabber {
    /// <summary>
    /// Delegate for the TestCompleted event
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void OnLoginEventHandler(object sender);
    public delegate void OnMessageEventHandler(Message msg);
    public delegate void OnErrorEventHandler(Enums.ErrorType type, string message);
    public delegate void OnRosterStartEventHandler();
    public delegate void OnRosterEndEventHandler();

    public class Client {

        #region ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ Singleton Stuff ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
       
        public Client() {
            this._jabberConnection = new nJim.Jabber();            
        }

        #endregion

        #region ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ Properties Gets/Sets ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        
        public Roster Roster {
            get { return this._jabberConnection.roster; }
        }

        public Identity Identity {
            get { return this._jabberConnection.identity; }
        }

        public nJim.Presence Presence {
            get { return this._jabberConnection.presence; }
        }

        public MessageGrabber MessageGrabber {
            get { return this._jabberConnection.XMPPConnection.MessageGrabber; }
        }

        public Jid MyJID {
            get { return new Jid(this._jabberConnection.identity.jabberID.full); }
        }
        public JabberID MyJabberID {
            get { return this._jabberConnection.identity.jabberID; }
        }

        public bool LoggedIn {
            get { return this._jabberConnection.XMPPConnection.Authenticated; }
        }

        #endregion

        #region ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ Member Fields ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        /// <summary>
        /// The connection to the jabber server
        /// </summary>
        private nJim.Jabber _jabberConnection;
        /// <summary>
        /// Was a disconnect requested?
        /// </summary>
        private bool _disconnect = false;
        /// <summary>
        /// Milliseconds to wait before trying to reconnect
        /// </summary>
        private const int _reconnectTimeout = 10000;
        /// <summary>
        /// Number of reconnects before giving up
        /// </summary>
        private const uint _reconnectAttempts = 5;
        /// <summary>
        /// Number of tries without successful connect
        /// </summary>
        private uint _reconnectCounter = 0;

        #endregion

       
        #region ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ Override Methods ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

        #endregion

        #region ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ Public Methods ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        /// <summary>
        /// Connect to the server
        /// </summary>
        public void Login() {
            _jabberConnection.username = Settings.username;
            _jabberConnection.password = Settings.password;
            _jabberConnection.server = Settings.server;
            _jabberConnection.resource = Settings.resource;
            
            _jabberConnection.debug = true;
            _jabberConnection.compress = true;
            _jabberConnection.keepAlive = true;
            _jabberConnection.keepAliveInterval = 55;
            _jabberConnection.priority = 5;
            _jabberConnection.socketConnectionType = Enums.SocketConnectionType.Normal;

            _jabberConnection.ssl = false;
            _jabberConnection.tls = true;

            _jabberConnection.autoResolveConnectServer = true;
            _jabberConnection.port = 5222;
            _jabberConnection.ignoreSslWarnings = false;

            _jabberConnection.errors.ErrorRaised += new nJim.ErrorHandler(_jabberConnection_ErrorRaised);
            _jabberConnection.XMPPConnection.OnMessage += new agsXMPP.protocol.client.MessageHandler(JabberClient_OnMessage);
           
            _jabberConnection.Connected += new NeutralHandler(_jabberConnection_Connected);
            _jabberConnection.Disconnected += new NeutralHandler(_jabberConnection_Disconnected);
            _jabberConnection.RosterStartUpdate += new NeutralHandler(_jabberConnection_RosterStartUpdate);
            _jabberConnection.RosterEndUpdate += new NeutralHandler(_jabberConnection_RosterEndUpdate);
            _jabberConnection.connect();
           
        }      

        /// <summary>
        /// Disconnect from the server
        /// </summary>
        public void Close() {
            _disconnect = true;
            _jabberConnection.disconnect();
        }

        /// <summary>
        /// Waits for some seconds then tries to reconnect to the jabber server
        /// </summary>
        public void Reconnect() {
            if (_reconnectCounter < _reconnectAttempts) {
                _reconnectCounter++;
                Log.Info(String.Format("Jabber trying to reconnect (Try {0}) ...", _reconnectCounter));
                System.Threading.Thread.Sleep(_reconnectTimeout);
                Login();
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
            _jabberConnection.XMPPConnection.Send(tmpMsg);
            return new Message(tmpMsg, DirectionTypes.Outgoing, DateTime.Now);
        }

        #endregion

        #region ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ Private Methods ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

        #endregion

        #region ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ Events & Delegates ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        public event OnMessageEventHandler OnMessage;
        public event OnLoginEventHandler OnLogin;
        public event OnErrorEventHandler OnError;
        public event OnRosterStartEventHandler OnRosterStart;
        public event OnRosterEndEventHandler OnRosterEnd;
        #endregion

        #region ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ EventHandlers ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
              
        void _jabberConnection_Connected(object sender) {
            _reconnectCounter = 0;
            _jabberConnection.roster.autoAcceptSubscribtion = false;
            _jabberConnection.presence.autoIdleMinutes = Settings.autoIdleTimeOut;
            _jabberConnection.presence.autoIdleStatus = Helper.GetStatusFromType(Settings.autoIdleStatusType);
            Log.Info("Login to jabber server completed.");

            OnLogin(sender);
        }

        void _jabberConnection_Disconnected(object sender) {
            if (!_disconnect) {
                Reconnect();
            } else {
                Log.Info("Jabber disconnected");
                _disconnect = false;
                _reconnectCounter = 0;
            }
        }

        void _jabberConnection_RosterStartUpdate(object sender) {
            OnRosterStart();
        }

        void _jabberConnection_RosterEndUpdate(object sender) {
            OnRosterEnd();
        }

        void JabberClient_OnMessage(object sender, agsXMPP.protocol.client.Message msg) {
            Log.Debug(String.Format("New jabber message from {0}: {1}", msg.From.ToString(), msg.Body));
            if (msg.Type == MessageType.chat && msg.From != null && msg.From != new Jid(_jabberConnection.identity.jabberID.full)) {
                OnMessage(new Message(msg, DirectionTypes.Incoming, DateTime.Now));
            }
        }

        void _jabberConnection_ErrorRaised(Enums.ErrorType type, string message) {
            Log.Error(String.Format("Jabber error: {0} {1}", type.ToString(), message));
            switch (type) {
                case Enums.ErrorType.Authentification:
                    Log.Error("Your jabber username or password is invalid");
                    break;
                case Enums.ErrorType.Server:
                    Reconnect();
                    break;
            }
            OnError(type, message);
        }

        #endregion


    }
}