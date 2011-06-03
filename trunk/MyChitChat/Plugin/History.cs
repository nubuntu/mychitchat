using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MediaPortal.GUI.Library;
using MyChitChat.Jabber;
using nJim;
using MyChitChat.Gui;
using agsXMPP;
using agsXMPP.Collections;
using System.Diagnostics;

namespace MyChitChat.Plugin {

    public delegate void OnUpdatedRosterEventHandler(Contact changedContact);
    public delegate void OnUpdatedPresenceEventHandler(Contact updatedContact);
    public delegate void OnUpdatedSessionEventHandler(Session updatedSession, Message updatedMessage);
    public delegate void OnUpdatedLogEventhandler(string logText);

    public sealed class History {

        #region ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ Constructor & Singleton Stuff ~~~~~~~~~~~~~~~~~~~~~

        static readonly History instance = new History();

        // Explicit static constructor to tell C# compiler
        // not to mark type as beforefieldinit
        static History() {
        }

        History() {
            Helper.JABBER_CLIENT.OnLogin += new OnLoginEventHandler(JABBER_CLIENT_OnLogin);
            ChatSessions = new List<Session>();
            LogHistory = new StringBuilder();
        }

        ~History() {
            try {
                Helper.JABBER_CLIENT.Roster.PresenceUpdated -= new ResourceHandler(Roster_PresenceUpdated);
                Helper.JABBER_CLIENT.Roster.MoodUpdated -= new ResourceMoodHandler(Roster_MoodUpdated);
                Helper.JABBER_CLIENT.Roster.ActivityUpdated -= new ResourceActivityHandler(Roster_ActivityUpdated);
                Helper.JABBER_CLIENT.Roster.TuneUpdated -= new ResourceTuneHandler(Roster_TuneUpdated);
                Helper.JABBER_CLIENT.Close();
            } catch (Exception ex) {
                Log.Error(ex);
            }
        }

        public static History Instance {
            get {
                return instance;
            }
        }

        #endregion

        #region ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ Properties Gets/Sets ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        public List<Session> ChatSessions { get; private set; }

        //public IEnumerable<SessionListItem> SessionListItems {
        //    get {
        //        foreach (Session currentSession in Sessions) {
        //            yield return new SessionListItem(currentSession, new GUIListItem.ItemSelectedHandler(OnSessionListItemSelected));
        //        }
        //    }
        //}

        //private IEnumerable<Session> Sessions {
        //    get {
        //        foreach (KeyValuePair<Jid, Session> currentSession in ChatSessions) {
        //            yield return currentSession.Value;
        //        }
        //    }
        //}

        public StringBuilder LogHistory { get; private set; }

        #endregion

        #region ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ Member Fields ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

        #endregion

        #region ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ Public Methods ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

        public Session GetSession(Jid chatPartner) {
            return ChatSessions.Find(session => session.ContactJID.ToString().ToLower() == chatPartner.ToString().ToLower());
        }

        public Session GetSession(string fullJid) {
            return GetSession(new Jid(fullJid));
        }

        public int GetSessionIndex(Session session) {
            if (Main.StatusFilter.HasValue) {
                if (Main.StatusFilter.Value) {
                    return ChatSessions.FindAll(x => (x.IsActiveSession)).IndexOf(session);
                } else {
                    return ChatSessions.FindAll(x => (!x.IsActiveSession)).IndexOf(session);
                }
            } else {
                return ChatSessions.IndexOf(session);
            }
        }

        public void ResetHistory() {
            ChatSessions.Clear();
        }

        #endregion

        #region ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ Private Methods ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

        private void NotifyPresMooActTun(nJim.Contact contact, Mood? mood, Activity? activity, Tune? tune) {
            string message = String.Empty;
            Helper.PresMooActNotifyInfo notifyInfo = new Helper.PresMooActNotifyInfo();
            if (contact.identity.nickname != String.Empty) {
                notifyInfo.nickname = contact.identity.nickname;
            } else {
                notifyInfo.nickname = contact.identity.jabberID.user;
            }
            message = notifyInfo.nickname;

            notifyInfo.resource = contact.identity.jabberID.resource;
            notifyInfo.stamp = contact.lastUpdated;
            notifyInfo.status = Translations.GetByName(contact.status.type.ToString());
            notifyInfo.message = contact.status.message;
            notifyInfo.icon = Helper.GetStatusIcon(contact.status.type.ToString());

            if (mood.HasValue) {
                notifyInfo.mood = mood.Value.type.ToString().ToUpperInvariant();
                notifyInfo.message = notifyInfo.mood;
                if (!String.IsNullOrEmpty(mood.Value.text)) {
                    notifyInfo.message += "\n'" + mood.Value.text + "'";
                }
                notifyInfo.icon = Helper.GetMoodIcon(mood.Value.type.ToString());
            }
            if (activity.HasValue) {
                notifyInfo.activity = activity.Value.type.ToString().ToUpperInvariant();
                notifyInfo.message = notifyInfo.activity;
                if (!String.IsNullOrEmpty(activity.Value.text)) {
                    notifyInfo.message += "\n'" + activity.Value.text + "'";
                }
                notifyInfo.icon = Helper.GetActivityIcon(activity.Value.type.ToString());
            }
            if (tune.HasValue) {
                notifyInfo.tune = string.Format("{0}\n{1}\n{2}", tune.Value.artist, tune.Value.title, tune.Value.length);
                notifyInfo.message = notifyInfo.tune;
                notifyInfo.icon = Helper.GetTuneIcon(tune.Value);
            }
            if (!String.IsNullOrEmpty(notifyInfo.message)) {
                message += "\n" + notifyInfo.message;
            }
            notifyInfo.header = string.Format("[{0}] {1} @ {2} ", notifyInfo.stamp.ToShortTimeString(), notifyInfo.status, notifyInfo.resource);

            Dialog.Instance.ShowNotifyDialog(notifyInfo.header, notifyInfo.icon, message, Settings.notifyWindowType);
        }

        private void NotifyMessage(Message msg) {
            Dialog.Instance.ShowNotifyDialog(Settings.notifyTimeOut, msg.FromJID.User, Helper.MEDIA_ICON_MESSAGE, msg.Body, Settings.notifyWindowType);
        }

        private void AppendLogEvent(DateTime when, string why, string who, string what) {
            string tmp = String.Format("[{0}] {1} {2}: \n      '{3}'", new string[] { when.ToShortTimeString(), why, who, what });
            LogHistory.Insert(0,tmp + Environment.NewLine);
            Log.Info(tmp);
            Debug.WriteLine(tmp);
            OnUpdatedLog(LogHistory.ToString());
        }


        #endregion

        #region ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ Events & Delegates ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        public event OnUpdatedRosterEventHandler OnUpdatedRoster;
        public event OnUpdatedPresenceEventHandler OnUpdatedPresence;
        public event OnUpdatedSessionEventHandler OnUpdatedSession;
        public event OnUpdatedLogEventhandler OnUpdatedLog;
        #endregion

        #region ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ EventHandlers ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

        void JABBER_CLIENT_OnLogin(object sender) {
            // Once Connected to Jabber keep 'em Messages/Presences pumpin'!
            Helper.JABBER_CLIENT.OnMessage += new OnMessageEventHandler(JABBER_CLIENT_OnMessage);
            Helper.JABBER_CLIENT.Roster.ResourceAdded += new ResourceHandler(Roster_ResourceAdded);
            Helper.JABBER_CLIENT.Roster.ResourceRemoved += new ResourceHandler(Roster_ResourceRemoved);
            Helper.JABBER_CLIENT.Roster.PresenceUpdated += new ResourceHandler(Roster_PresenceUpdated);
            Helper.JABBER_CLIENT.Roster.MoodUpdated += new ResourceMoodHandler(Roster_MoodUpdated);
            Helper.JABBER_CLIENT.Roster.ActivityUpdated += new ResourceActivityHandler(Roster_ActivityUpdated);
            Helper.JABBER_CLIENT.Roster.TuneUpdated += new ResourceTuneHandler(Roster_TuneUpdated);
            AppendLogEvent(DateTime.Now, "MyChitChat started!",  Settings.username , "Jabber connected...");
        }

       void Roster_ResourceAdded(nJim.Contact contact) {
            Session newSession = new Session(contact);
            newSession.OnChatSessionUpdated += new OnChatSessionUpdatedEventHandler(newSession_OnChatSessionUpdated);
           if (!ChatSessions.Contains(newSession)) {
                ChatSessions.Add(newSession);
                OnUpdatedRoster(contact);
                AppendLogEvent(contact.lastUpdated, "Added", contact.identity.nickname, Translations.GetByName(contact.status.type.ToString()));
           }
            
        }



        void Roster_ResourceRemoved(nJim.Contact contact) {
            ChatSessions.Remove(new Session(contact));
            OnUpdatedRoster(contact);
            AppendLogEvent(contact.lastUpdated, "Removed", contact.identity.nickname, Translations.GetByName(contact.status.type.ToString()));
        }

        void JABBER_CLIENT_OnMessage(Message msg) {
            if ((Settings.notifyOnMessagePlugin && Helper.PLUGIN_WINDOW_ACTIVE) || Settings.notifyOnMessageGlobally && !Helper.PLUGIN_WINDOW_ACTIVE) {
                NotifyMessage(msg);               
            }
            AppendLogEvent(msg.DateTimeReceived, "Msg", msg.FromJID.User, msg.Subject);
            try {
                if (msg.FromJID.User != null) {
                    GetSession(msg.FromJID).AddMessage(msg);
                }
            } catch (Exception e) {
                Log.Error(e);
            }

        }

        void Roster_PresenceUpdated(nJim.Contact contact) {
            if (((Settings.notifyOnStatusPlugin && Helper.PLUGIN_WINDOW_ACTIVE) || Settings.notifyOnStatusGlobally && !Helper.PLUGIN_WINDOW_ACTIVE) && contact.identity.jabberID.full != Helper.JABBER_CLIENT.MyJabberID.full && contact.status.type != Enums.StatusType.Unavailable) {
                NotifyPresMooActTun(contact, null, null, null);
            }
            AppendLogEvent(contact.lastUpdated, "Status", contact.identity.nickname, Translations.GetByName(contact.status.type.ToString()));            
            OnUpdatedPresence(contact);      
        }

        void Roster_MoodUpdated(nJim.Contact contact, Mood mood) {
            if ((Settings.notifyOnMoodPlugin && Helper.PLUGIN_WINDOW_ACTIVE) || Settings.notifyOnMoodGlobally && !Helper.PLUGIN_WINDOW_ACTIVE) {
                NotifyPresMooActTun(contact, mood, null, null);
            }
            AppendLogEvent(contact.lastUpdated, "Mood", contact.identity.nickname, Translations.GetByName(mood.type.ToString()));
            OnUpdatedPresence(contact);           
        }

        void Roster_ActivityUpdated(nJim.Contact contact, Activity activity) {
            if ((Settings.notifyOnActivityPlugin && Helper.PLUGIN_WINDOW_ACTIVE) || Settings.notifyOnActivityGlobally && !Helper.PLUGIN_WINDOW_ACTIVE) {
                NotifyPresMooActTun(contact, null, activity, null);

            }
            OnUpdatedPresence(contact);
            AppendLogEvent(contact.lastUpdated, "Activity", contact.identity.nickname, Translations.GetByName(activity.type.ToString()));
        }

        void Roster_TuneUpdated(nJim.Contact contact, Tune tune) {
            if ((Settings.notifyOnTunePlugin && Helper.PLUGIN_WINDOW_ACTIVE) || Settings.notifyOnTuneGlobally && !Helper.PLUGIN_WINDOW_ACTIVE) {
                NotifyPresMooActTun(contact, null, null, tune);

            }
            OnUpdatedPresence(contact);
            AppendLogEvent(contact.lastUpdated, "Tune", contact.identity.nickname, tune.artist + " - " + tune.title);
        }

       
        void newSession_OnChatSessionUpdated(Session session, Message msg) {
            //if (((Settings.notifyOnMessagePlugin && Helper.PLUGIN_WINDOW_ACTIVE) || Settings.notifyOnMessageGlobally && !Helper.PLUGIN_WINDOW_ACTIVE )&& msg.DirectionType != DirectionTypes.Outgoing) {
            //    NotifyMessage(msg);
            //}
            OnUpdatedSession(session, msg);
            
        }

        #endregion

        #region ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ Override Methods ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

        #endregion
    }
}
