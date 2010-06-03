/* 
 * dotNet Jabber Instant Messaging Library - nJim
 * Copyright ©2008 Christophe LEMOINE
 * 
 * This file is part of dotNet Jabber Instant Messaging Library.
 * 
 * dotNet Jabber Instant Messaging Library is free software;
 * you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation; either version 2 of the License,
 * or any later version.
 * 
 * dotNet Jabber Instant Messaging Library is distributed in the hope
 * that it will be useful, but WITHOUT ANY WARRANTY; without even the implied
 * warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
 * See the GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with Foobar; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA  02110-1301  USA
 * 
 */
using System;
using System.Collections.Generic;
using System.Text;
using agsXMPP.protocol.iq.disco;
using agsXMPP;

namespace nJim
{
	
	/// <summary>
	/// Gestion d'un contact du Roster
	/// </summary>
	public class Contact : IDisposable
	{

		#region Events

		/// <summary>
		/// La version du logiciel client du contact est disponible
		/// </summary>
		public event ClientVersionHandler ClientVersionAvailable;
		private void OnClientVersionAvailable(ClientVersion version)
		{
			try
			{
				ClientVersionAvailable(version);
			}
			catch { }
		}

		/// <summary>
		/// Le temps du contact est disponible
		/// </summary>
		public event ClientTimeHandler ClientTimeAvailable;
		private void OnClientTimeAvailable(DateTime time)
		{
			try
			{
				ClientTimeAvailable(time);
			}
			catch { }
		}

		/// <summary>
		/// La durée d'inativité du contact est disponible
		/// </summary>
		public event ClientTimespanHandler ClientLastAvailable;
		private void OnClientLastAvailable(TimeSpan time)
		{
			try
			{
				ClientLastAvailable(time);
			}
			catch { }
		}

		#endregion

		#region Properties

		private Identity _identity = null;
		/// <summary>
		/// Fiche d'identité
		/// </summary>
		public Identity identity
		{
			get { return _identity; }
		}

		private List<string> _groups = new List<string>();
		/// <summary>
		/// Liste des groupes
		/// </summary>
		public List<string> groups
		{
			get { return _groups; }
		}

		private int _priority = 0;
		/// <summary>
		/// Priorité de la resource
		/// </summary>
		public int priority
		{
			get { return _priority; }
			set
			{
				if (value != _priority)
				{
					_priority = value;
				}
			}
		}

		private Status _status;
		/// <summary>
		/// Status de la présence
		/// </summary>
		public Status status
		{
			get { return _status; }
			set { _status = value; }
		}

		private DateTime _lastUpdated = DateTime.Now;
		/// <summary>
		/// Date du dernier changement de presence
		/// </summary>
		public DateTime lastUpdated
		{
			get { return _lastUpdated; }
			set { _lastUpdated = value; }
		}

		private TimeSpan _timeInterval = new TimeSpan(0);
		/// <summary>
		/// Différence entre la date locale et la date du client
		/// </summary>
		public TimeSpan timeInterval
		{
			get { return _timeInterval; }
			set
			{
				if (value != _timeInterval)
				{
					_timeInterval = value;
				}
			}
		}

		private List<Service> _services = new List<Service>();
		/// <summary>
		/// Contient les fonctionnalités disponibles sur le server
		/// </summary>
		public List<Service> services
		{
			get { return _services; }
		}

		private bool _blocked = false;
		/// <summary>
		/// Indique si le contact est bloqué
		/// </summary>
		public bool blocked
		{
			get { return _blocked; }
			set
			{
				if (value != _blocked)
				{
					_blocked = value;
				}
			}
		}

		private Enums.SubscribtionType _subscription = Enums.SubscribtionType.None;
		/// <summary>
		/// Type d'abonnement
		/// </summary>
		public Enums.SubscribtionType subscription
		{
			get { return _subscription; }
			set
			{
				if (value != _subscription)
				{
					_subscription = value;
				}
			}
		}

		private Mood _mood;
		/// <summary>
		/// Humeur du contact
		/// </summary>
		public Mood mood
		{
			get { return _mood; }
			set { _mood = value; }
		}

		private Activity _activity;
		/// <summary>
		/// Activité du contact
		/// </summary>
		public Activity activity
		{
			get { return _activity; }
			set { _activity = value; }
		}

        private Location _location;
        /// <summary>
        /// Emplacement géographique
        /// </summary>
        public Location location
        {
            get { return _location; }
            set { _location = value; }
        }

		private Tune _tune;
		/// <summary>
		/// Lecture en cours
		/// </summary>
		public Tune tune
		{
			get { return _tune; }
			set { _tune = value; }
		}

		#endregion

		#region Constructor

		/// <summary>
		/// Constructeur
		/// </summary>
		public Contact(agsXMPP.Jid jid, string nickname, agsXMPP.Xml.Dom.ElementList grps)
		{
			_identity = new Identity(jid);
			if (nickname != null && nickname.Trim() != string.Empty)
			{
				_identity.nickname = nickname;
			}
			if (grps != null)
			{
				foreach (agsXMPP.protocol.Base.Group g in grps)
				{
					if (!_groups.Contains(g.Name))
					{
						_groups.Add(g.Name);
					}
				}
			}
			_status = new Status();
			_status.type = Enums.StatusType.Unvailable;
			_status.message = string.Empty;
			_mood = new Mood();
			_mood.type = Enums.MoodType.none;
			_mood.text = string.Empty;
			_activity = new Activity();
			_activity.type = Enums.ActivityType.none;
			_activity.text = string.Empty;
            _location = new Location();
            _location.altitude = 0;
            _location.latitude = 0;
            _location.longitude = 0;
            _location.bearing = 0;
            _location.error = 0;
            _location.speed = 0;
            _location.area = string.Empty;
            _location.building = string.Empty;
            _location.country = string.Empty;
            _location.datum = string.Empty;
            _location.description = string.Empty;
            _location.floor = string.Empty;
            _location.locality = string.Empty;
            _location.postalcode = string.Empty;
            _location.region = string.Empty;
            _location.room = string.Empty;
            _location.street = string.Empty;
            _location.text = string.Empty;
            _location.timestamp = new DateTime();
            _location.uri = string.Empty;
			_tune = new Tune();
			_tune.artist = string.Empty;
			_tune.length = 0;
			_tune.rating = 1;
			_tune.source = string.Empty;
			_tune.title = string.Empty;
			_tune.track = 0;
			_tune.uri = string.Empty;
		}

		#endregion

		#region Destructor

		/// <summary>
		/// Destructeur
		/// </summary>
		public void Dispose()
		{
			if (_identity != null)
			{
				_identity.save();
				_identity.Dispose();
				_identity = null;
			}
		}

		#endregion

		#region Public methods

		/// <summary>
		/// Récupère la version logicielle du contact
		/// </summary>
		public void getVersion()
		{
			if (Jabber.xmpp.Authenticated && status.type != Enums.StatusType.Unvailable && status.type != Enums.StatusType.Invisible)
			{
				agsXMPP.protocol.iq.version.VersionIq viq = new agsXMPP.protocol.iq.version.VersionIq();
				viq.From = Jabber.xmpp.MyJID;
				viq.To = new Jid(identity.jabberID.full);
				viq.Type = agsXMPP.protocol.client.IqType.get;
				viqID = viq.Id;
				Jabber.xmpp.IqGrabber.SendIq(viq, new IqCB(getVersionResult), null);
			}
		}

		/// <summary>
		/// Récupère le temps du contact
		/// </summary>
		public void getTime()
		{
			if (Jabber.xmpp.Authenticated && status.type != Enums.StatusType.Unvailable && status.type != Enums.StatusType.Invisible)
			{
				agsXMPP.protocol.iq.time.TimeIq tiq = new agsXMPP.protocol.iq.time.TimeIq();
				tiq.From = Jabber.xmpp.MyJID;
				tiq.To = new Jid(identity.jabberID.full);
				tiq.Type = agsXMPP.protocol.client.IqType.get;
				tiqID = tiq.Id;
				Jabber.xmpp.IqGrabber.SendIq(tiq, new IqCB(getTimeResult), null);
			}
		}

		/// <summary>
		/// Récupère le temps d'inactivité du contact
		/// </summary>
		public void getLast()
		{
			if (Jabber.xmpp.Authenticated && status.type != Enums.StatusType.Unvailable && status.type != Enums.StatusType.Invisible)
			{
				agsXMPP.protocol.iq.last.LastIq liq = new agsXMPP.protocol.iq.last.LastIq();
				liq.From = Jabber.xmpp.MyJID;
				liq.To = new Jid(identity.jabberID.full);
				liq.Type = agsXMPP.protocol.client.IqType.get;
				liqID = liq.Id;
				Jabber.xmpp.IqGrabber.SendIq(liq, new IqCB(getLastResult), null);
			}
		}

		#endregion

		private string viqID = string.Empty;
		/// <summary>
		/// Resultat de la requète getVersion
		/// </summary>
		/// <param name="sender">Objet parent</param>
		/// <param name="iq">Résultat de la requète</param>
		/// <param name="data">Données supplémentaires</param>
		private void getVersionResult(object sender, agsXMPP.protocol.client.IQ iq, object data)
		{
			if (iq.Type == agsXMPP.protocol.client.IqType.result)
			{
				if (iq.Query != null && iq.Query is agsXMPP.protocol.iq.version.Version)
				{
					agsXMPP.protocol.iq.version.Version version = iq.Query as agsXMPP.protocol.iq.version.Version;
					ClientVersion cv = new ClientVersion();
					cv.name = (version.Name != null) ? version.Name.Trim() : string.Empty;
					cv.os = (version.Os != null) ? version.Os.Trim() : string.Empty;
					cv.version = (version.Ver != null) ? version.Ver.Trim() : string.Empty;
					OnClientVersionAvailable(cv);
					if (Jabber.xmpp.IqGrabber != null && viqID != string.Empty)
					{
						Jabber.xmpp.IqGrabber.Remove(viqID);
					}
				}
			}
		}

		private string tiqID = string.Empty;
		/// <summary>
		/// Resultat de la requète getTime
		/// </summary>
		/// <param name="sender">Objet parent</param>
		/// <param name="iq">Résultat de la requète</param>
		/// <param name="data">Données supplémentaires</param>
		private void getTimeResult(object sender, agsXMPP.protocol.client.IQ iq, object data)
		{
			if (iq.Type == agsXMPP.protocol.client.IqType.result)
			{
				if (iq.Query != null && iq.Query is agsXMPP.protocol.iq.time.Time)
				{
					agsXMPP.protocol.iq.time.Time time = iq.Query as agsXMPP.protocol.iq.time.Time;
					OnClientTimeAvailable(DateTime.Parse(time.Utc));
					if (Jabber.xmpp.IqGrabber != null && tiqID != string.Empty)
					{
						Jabber.xmpp.IqGrabber.Remove(tiqID);
					}
				}
			}
		}

		private string liqID = string.Empty;
		/// <summary>
		/// Resultat de la requète getLast
		/// </summary>
		/// <param name="sender">Objet parent</param>
		/// <param name="iq">Résultat de la requète</param>
		/// <param name="data">Données supplémentaires</param>
		private void getLastResult(object sender, agsXMPP.protocol.client.IQ iq, object data)
		{
			if (iq.Type == agsXMPP.protocol.client.IqType.result)
			{
				if (iq.Query != null && iq.Query is agsXMPP.protocol.iq.last.Last)
				{
					agsXMPP.protocol.iq.last.Last last = iq.Query as agsXMPP.protocol.iq.last.Last;
					OnClientLastAvailable(new TimeSpan(0, 0, 0, last.Seconds));
					if (Jabber.xmpp.IqGrabber != null && liqID != string.Empty)
					{
						Jabber.xmpp.IqGrabber.Remove(liqID);
					}
				}
			}
		}

	}

}
