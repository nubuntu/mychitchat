using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace nJim
{

	/// <summary>
	/// Gestion de sa Presence et de ses Status
	/// </summary>
	public class Presence : IDisposable
	{

		private System.Timers.Timer autoIdleTimer = new System.Timers.Timer(1000);

		#region Win32

		private struct LASTINPUTINFO
		{
			public uint cbSize;
			public uint dwTime;
		}

		[DllImport("User32.dll")]
		private static extern bool GetLastInputInfo(ref LASTINPUTINFO plii);

		#endregion

		#region Properties

		private Status _status;
		/// <summary>
		/// Obtient ou modifie son propre status
		/// </summary>
		public Status status
		{
			get { return _status; }
			set { _status = value; }
		}

		private Status _autoIdleLastStatus;
		private bool _autoIdleState = false;
		private Status _autoIdleStatus;
		/// <summary>
		/// Obtient ou modifie le status utilisé en cas d'absence automatique
		/// </summary>
		public Status autoIdleStatus
		{
			get { return _autoIdleStatus; }
			set { _autoIdleStatus = value; }
		}

		private int _autoIdleMinutes = 5;
		/// <summary>
		/// Obtien ou modifie le temps après lequel le status changera en celui d'abscence automatique
		/// </summary>
		public int autoIdleMinutes
		{
			get { return _autoIdleMinutes; }
			set
			{
				if (value != _autoIdleMinutes && value > 0 && value <= 3600)
				{
					_autoIdleMinutes = value;
				}
			}
		}

		private bool _autoIdle = true;
		/// <summary>
		/// Active ou désactive l'abscence automatique
		/// </summary>
		public bool autoIdle
		{
			get { return _autoIdle; }
			set
			{
				if (value != _autoIdle)
				{
					_autoIdle = value;
				}
			}
		}

		private Mood _mood;
		/// <summary>
		/// Son hummeur
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
		public Presence()
		{
			_status = new Status();
			_status.type = Enums.StatusType.Normal;
			_status.message = string.Empty;
			_mood = new Mood();
			_mood.type = Enums.MoodType.none;
			_mood.text = string.Empty;
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
			_autoIdleStatus = new Status();
			_autoIdleStatus.type = Enums.StatusType.Away;
			_autoIdleStatus.message = "(Auto Idle)";
			autoIdleTimer.Elapsed += new System.Timers.ElapsedEventHandler(autoIdleTimerElapsed);
			autoIdleTimer.Enabled = true;
		}

		#endregion

		#region Destructor

		/// <summary>
		/// Destructeur
		/// </summary>
		public void Dispose()
		{
			autoIdleTimer.Enabled = false;
			autoIdleTimer.Elapsed -= new System.Timers.ElapsedEventHandler(autoIdleTimerElapsed);
		}

		#endregion

		#region Public methods

		/// <summary>
		/// Change l'état de son status
		/// </summary>
		public void applyStatus()
		{
			if (Jabber.xmpp.Authenticated)
			{
				agsXMPP.protocol.client.Presence p = new agsXMPP.protocol.client.Presence();
				p.From = Jabber.xmpp.MyJID;
				p.Priority = Jabber.xmpp.Priority;
				p.Type = Enums.PresenceTypeConverter(_status.type);
				if (p.Type == agsXMPP.protocol.client.PresenceType.available)
				{
					p.Show = Enums.StatusTypeConverter(_status.type);
					p.Status = _status.message;
					p.XDelay = new agsXMPP.protocol.x.Delay();
					p.XDelay.Stamp = DateTime.Now;
					p.AddChild(Queries.getCapabilities());
					Jabber.xmpp.Show = p.Show;
					Jabber.xmpp.Status = p.Status;
					p.Nickname = new agsXMPP.protocol.extensions.nickname.Nickname(Jabber._identity.nickname);
				}
				Jabber.xmpp.Send(p);
			}
		}

		/// <summary>
		/// Retourne en Ticks le temps de la dernière activité
		/// </summary>
		/// <returns>Nombre de ticks depuis la dernière activité</returns>
		public int getLastActivityTicks()
		{
			LASTINPUTINFO LastInputInfo = new LASTINPUTINFO();
			LastInputInfo.cbSize = (uint)Marshal.SizeOf(LastInputInfo);
			LastInputInfo.dwTime = 0;
			if (GetLastInputInfo(ref LastInputInfo))
			{
				return ((int)LastInputInfo.dwTime);
			}
			return 0;
		}

		/// <summary>
		/// Retourne la durée d'inactivité
		/// </summary>
		/// <returns>Durée d'inactivité</returns>
		public TimeSpan getIdle()
		{
			return new TimeSpan(Environment.TickCount - getLastActivityTicks());
		}

		/// <summary>
		/// Retourne au format Date/Heure le temps de la dernière activité
		/// </summary>
		/// <returns>Format Date/Heure du temps de la dernière activité</returns>
		public DateTime getLAstActivity()
		{
			return new DateTime(getLastActivityTicks());
		}

		/// <summary>
		/// Change son hummeur
		/// </summary>
		/// <param name="type">Type de l'hummeur</param>
		/// <param name="text">Description</param>
		public void setMood(Enums.MoodType type, string text)
		{
			if (Jabber.xmpp.Authenticated && Jabber._pepCapable)
			{
				agsXMPP.protocol.client.IQ iq = new agsXMPP.protocol.client.IQ();
				iq.Type = agsXMPP.protocol.client.IqType.set;
				iq.GenerateId();
				agsXMPP.Xml.Dom.Element pb = new agsXMPP.Xml.Dom.Element("pubsub");
				pb.Namespace = "http://jabber.org/protocol/pubsub";
				agsXMPP.Xml.Dom.Element ps = new agsXMPP.Xml.Dom.Element("publish");
				ps.Attributes.Add("node", "http://jabber.org/protocol/mood");
				agsXMPP.Xml.Dom.Element item = new agsXMPP.Xml.Dom.Element("item");
				item.Attributes.Add("id", "current");
				agsXMPP.Xml.Dom.Element mood = new agsXMPP.Xml.Dom.Element("mood");
				mood.Namespace = "http://jabber.org/protocol/mood";
				agsXMPP.Xml.Dom.Element moodType = new agsXMPP.Xml.Dom.Element(Enum.GetName(typeof(Enums.MoodType), type));
				mood.AddChild(moodType);
				agsXMPP.Xml.Dom.Element moodText = new agsXMPP.Xml.Dom.Element("text");
				moodText.Value = text;
				mood.AddChild(moodText);
				item.AddChild(mood);
				ps.AddChild(item);
				pb.AddChild(ps);
				agsXMPP.Xml.Dom.Element conf = new agsXMPP.Xml.Dom.Element("configure");
				agsXMPP.Xml.Dom.Element x = new agsXMPP.Xml.Dom.Element("x");
				agsXMPP.Xml.Dom.Element field1 = new agsXMPP.Xml.Dom.Element("field");
				field1.Attributes.Add("type", "hidden");
				field1.Attributes.Add("var", "FORM_TYPE");
				agsXMPP.Xml.Dom.Element value1 = new agsXMPP.Xml.Dom.Element("value");
				value1.Value = "http://jabber.org/protocol/pubsub#node_config";
				field1.AddChild(value1);
				x.AddChild(field1);
				agsXMPP.Xml.Dom.Element field2 = new agsXMPP.Xml.Dom.Element("field");
				field2.Attributes.Add("var", "pubsub#access_model");
				agsXMPP.Xml.Dom.Element value2 = new agsXMPP.Xml.Dom.Element("value");
				value2.Value = "presence";
				field2.AddChild(value2);
				x.AddChild(field2);
				conf.AddChild(x);
				pb.AddChild(conf);
				iq.AddChild(pb);
				Jabber.xmpp.Send(iq);
				_mood = new Mood();
				_mood.type = type;
				_mood.text = text;
			}
		}

		/// <summary>
		/// Change son activité
		/// </summary>
		/// <param name="type">Type de l'activité</param>
		/// <param name="text">Description</param>
		public void setActivity(Enums.ActivityType type, string text)
		{
			if (Jabber.xmpp.Authenticated && Jabber._pepCapable)
			{
				agsXMPP.protocol.client.IQ iq = new agsXMPP.protocol.client.IQ();
				iq.Type = agsXMPP.protocol.client.IqType.set;
				iq.GenerateId();
				agsXMPP.Xml.Dom.Element pb = new agsXMPP.Xml.Dom.Element("pubsub");
				pb.Namespace = "http://jabber.org/protocol/pubsub";
				agsXMPP.Xml.Dom.Element ps = new agsXMPP.Xml.Dom.Element("publish");
				ps.Attributes.Add("node", "http://jabber.org/protocol/activity");
				agsXMPP.Xml.Dom.Element item = new agsXMPP.Xml.Dom.Element("item");
				item.Attributes.Add("id", "current");
				agsXMPP.Xml.Dom.Element activity = new agsXMPP.Xml.Dom.Element("activity");
				activity.Namespace = "http://jabber.org/protocol/activity";
				agsXMPP.Xml.Dom.Element activityType = new agsXMPP.Xml.Dom.Element(Enum.GetName(typeof(Enums.ActivityType), type));
				activity.AddChild(activityType);
				agsXMPP.Xml.Dom.Element activityText = new agsXMPP.Xml.Dom.Element("text");
				activityText.Value = text;
				activity.AddChild(activityText);
				item.AddChild(activity);
				ps.AddChild(item);
				pb.AddChild(ps);
				agsXMPP.Xml.Dom.Element conf = new agsXMPP.Xml.Dom.Element("configure");
				agsXMPP.Xml.Dom.Element x = new agsXMPP.Xml.Dom.Element("x");
				agsXMPP.Xml.Dom.Element field1 = new agsXMPP.Xml.Dom.Element("field");
				field1.Attributes.Add("type", "hidden");
				field1.Attributes.Add("var", "FORM_TYPE");
				agsXMPP.Xml.Dom.Element value1 = new agsXMPP.Xml.Dom.Element("value");
				value1.Value = "http://jabber.org/protocol/pubsub#node_config";
				field1.AddChild(value1);
				x.AddChild(field1);
				agsXMPP.Xml.Dom.Element field2 = new agsXMPP.Xml.Dom.Element("field");
				field2.Attributes.Add("var", "pubsub#access_model");
				agsXMPP.Xml.Dom.Element value2 = new agsXMPP.Xml.Dom.Element("value");
				value2.Value = "presence";
				field2.AddChild(value2);
				x.AddChild(field2);
				conf.AddChild(x);
				pb.AddChild(conf);
				iq.AddChild(pb);
				Jabber.xmpp.Send(iq);
				_activity = new Activity();
				_activity.type = type;
				_activity.text = text;
			}
		}

		/// <summary>
		/// Définit un nouvel emplacement géographique
		/// </summary>
		/// <param name="latitude">Latitude</param>
		/// <param name="longitude">Longitude</param>
		/// <param name="country">Pays</param>
		public void setLocation(double latitude, double longitude, string country)
		{
			Location location = new Location();
			location.latitude = latitude;
			location.longitude = longitude;
			location.country = country;
			setLocation(location);
		}

		/// <summary>
		/// Définit un nouvek emplacement géographique
		/// </summary>
		/// <param name="geoStructure">Strucure d'informations provenant directement d'un outils de géolocalisation incorporé à la librairie</param>
		public void setLocation(GeoLocTools.GeoStructure geoStructure)
		{
			Location location = new Location();
			location.locality = geoStructure.city;
			location.country = geoStructure.country;
			location.description = geoStructure.location;
			location.latitude = geoStructure.latitude;
			location.longitude = geoStructure.longitude;
			location.postalcode = geoStructure.zipcode;
			location.street = geoStructure.street;
			setLocation(location);
		}

		/// <summary>
		/// Définit un nouvel emplacement géographique
		/// </summary>
		/// <param name="location">Informations complète sur l'emplacement</param>
		public void setLocation(Location location)
		{
			if (Jabber.xmpp.Authenticated && Jabber._pepCapable)
			{
				agsXMPP.protocol.client.IQ iq = new agsXMPP.protocol.client.IQ();
				iq.Type = agsXMPP.protocol.client.IqType.set;
				iq.GenerateId();
				agsXMPP.Xml.Dom.Element pb = new agsXMPP.Xml.Dom.Element("pubsub");
				pb.Namespace = "http://jabber.org/protocol/pubsub";
				agsXMPP.Xml.Dom.Element ps = new agsXMPP.Xml.Dom.Element("publish");
				ps.Attributes.Add("node", "http://jabber.org/protocol/geoloc");
				agsXMPP.Xml.Dom.Element item = new agsXMPP.Xml.Dom.Element("item");
				item.Attributes.Add("id", "current");
				agsXMPP.Xml.Dom.Element geoloc = new agsXMPP.Xml.Dom.Element("geoloc");
				geoloc.Namespace = "http://jabber.org/protocol/geoloc";
				geoloc.AddChild(new agsXMPP.Xml.Dom.Element("alt", Convert.ToString(location.altitude, System.Globalization.CultureInfo.InvariantCulture)));
				geoloc.AddChild(new agsXMPP.Xml.Dom.Element("lat", Convert.ToString(location.latitude, System.Globalization.CultureInfo.InvariantCulture)));
				geoloc.AddChild(new agsXMPP.Xml.Dom.Element("lon", Convert.ToString(location.longitude, System.Globalization.CultureInfo.InvariantCulture)));
				geoloc.AddChild(new agsXMPP.Xml.Dom.Element("bearing", Convert.ToString(location.bearing, System.Globalization.CultureInfo.InvariantCulture)));
				geoloc.AddChild(new agsXMPP.Xml.Dom.Element("error", Convert.ToString(location.error, System.Globalization.CultureInfo.InvariantCulture)));
				geoloc.AddChild(new agsXMPP.Xml.Dom.Element("speed", Convert.ToString(location.speed, System.Globalization.CultureInfo.InvariantCulture)));
				geoloc.AddChild(new agsXMPP.Xml.Dom.Element("area", (location.area != null) ? location.area.Trim() : string.Empty));
				geoloc.AddChild(new agsXMPP.Xml.Dom.Element("building", (location.building != null) ? location.building.Trim() : string.Empty));
				geoloc.AddChild(new agsXMPP.Xml.Dom.Element("country", (location.country != null) ? location.country.Trim() : string.Empty));
				geoloc.AddChild(new agsXMPP.Xml.Dom.Element("datum", (location.datum != null) ? location.datum.Trim() : string.Empty));
				geoloc.AddChild(new agsXMPP.Xml.Dom.Element("description", (location.description != null) ? location.description.Trim() : string.Empty));
				geoloc.AddChild(new agsXMPP.Xml.Dom.Element("floor", (location.floor != null) ? location.floor.Trim() : string.Empty));
				geoloc.AddChild(new agsXMPP.Xml.Dom.Element("locality", (location.locality != null) ? location.locality.Trim() : string.Empty));
				geoloc.AddChild(new agsXMPP.Xml.Dom.Element("postalcode", (location.postalcode != null) ? location.postalcode.Trim() : string.Empty));
				geoloc.AddChild(new agsXMPP.Xml.Dom.Element("region", (location.region != null) ? location.region.Trim() : string.Empty));
				geoloc.AddChild(new agsXMPP.Xml.Dom.Element("room", (location.room != null) ? location.room.Trim() : string.Empty));
				geoloc.AddChild(new agsXMPP.Xml.Dom.Element("street", (location.street != null) ? location.street.Trim() : string.Empty));
				geoloc.AddChild(new agsXMPP.Xml.Dom.Element("text", (location.text != null) ? location.text.Trim() : string.Empty));
				geoloc.AddChild(new agsXMPP.Xml.Dom.Element("timestamp", agsXMPP.util.Time.ISO_8601Date(location.timestamp)));
				geoloc.AddChild(new agsXMPP.Xml.Dom.Element("uri", (location.uri != null) ? location.uri.Trim() : string.Empty));
				item.AddChild(geoloc);
				ps.AddChild(item);
				pb.AddChild(ps);
				agsXMPP.Xml.Dom.Element conf = new agsXMPP.Xml.Dom.Element("configure");
				agsXMPP.Xml.Dom.Element x = new agsXMPP.Xml.Dom.Element("x");
				agsXMPP.Xml.Dom.Element field1 = new agsXMPP.Xml.Dom.Element("field");
				field1.Attributes.Add("type", "hidden");
				field1.Attributes.Add("var", "FORM_TYPE");
				agsXMPP.Xml.Dom.Element value1 = new agsXMPP.Xml.Dom.Element("value");
				value1.Value = "http://jabber.org/protocol/pubsub#node_config";
				field1.AddChild(value1);
				x.AddChild(field1);
				agsXMPP.Xml.Dom.Element field2 = new agsXMPP.Xml.Dom.Element("field");
				field2.Attributes.Add("var", "pubsub#access_model");
				agsXMPP.Xml.Dom.Element value2 = new agsXMPP.Xml.Dom.Element("value");
				value2.Value = "presence";
				field2.AddChild(value2);
				x.AddChild(field2);
				conf.AddChild(x);
				pb.AddChild(conf);
				iq.AddChild(pb);
				Jabber.xmpp.Send(iq);
				_location = new Location();
				_location.altitude = location.altitude;
				_location.area = (location.area != null) ? location.area.Trim() : string.Empty;
				_location.bearing = location.bearing;
				_location.building = (location.building != null) ? location.building.Trim() : string.Empty;
				_location.country = (location.country != null) ? location.country.Trim() : string.Empty;
				_location.datum = (location.datum != null) ? location.datum.Trim() : string.Empty;
				_location.description = (location.description != null) ? location.description.Trim() : string.Empty;
				_location.error = location.error;
				_location.floor = (location.floor != null) ? location.floor.Trim() : string.Empty;
				_location.latitude = location.latitude;
				_location.locality = (location.locality != null) ? location.locality.Trim() : string.Empty;
				_location.longitude = location.longitude;
				_location.postalcode = (location.postalcode != null) ? location.postalcode.Trim() : string.Empty;
				_location.region = (location.region != null) ? location.region.Trim() : string.Empty;
				_location.room = (location.room != null) ? location.room.Trim() : string.Empty;
				_location.speed = location.speed;
				_location.street = (location.street != null) ? location.street.Trim() : string.Empty;
				_location.text = (location.text != null) ? location.text.Trim() : string.Empty;
				_location.timestamp = location.timestamp;
				_location.uri = (location.uri != null) ? location.uri.Trim() : string.Empty;
			}
		}

		/// <summary>
		/// Change la lecture en cours
		/// </summary>
		/// <param name="title">Titre du morceau</param>
		/// <param name="artist">Nom de l'artiste</param>
		/// <param name="length">Durée du morceau</param>
		public void setTune(string title, string artist, int length)
		{
			Tune t = new Tune();
			t.title = title;
			t.artist = artist;
			t.length = length;
			setTune(t);
		}

		/// <summary>
		/// Change la lecture en cours
		/// </summary>
		/// <param name="utune">Informations sur la lecture en cours</param>
		public void setTune(Tune utune)
		{
			if (Jabber.xmpp.Authenticated && Jabber._pepCapable)
			{
				agsXMPP.protocol.client.IQ iq = new agsXMPP.protocol.client.IQ();
				iq.Type = agsXMPP.protocol.client.IqType.set;
				iq.GenerateId();
				agsXMPP.Xml.Dom.Element pb = new agsXMPP.Xml.Dom.Element("pubsub");
				pb.Namespace = "http://jabber.org/protocol/pubsub";
				agsXMPP.Xml.Dom.Element ps = new agsXMPP.Xml.Dom.Element("publish");
				ps.Attributes.Add("node", "http://jabber.org/protocol/tune");
				agsXMPP.Xml.Dom.Element item = new agsXMPP.Xml.Dom.Element("item");
				item.Attributes.Add("id", "current");
				agsXMPP.Xml.Dom.Element tune = new agsXMPP.Xml.Dom.Element("tune");
				tune.Namespace = "http://jabber.org/protocol/tune";
				tune.AddChild(new agsXMPP.Xml.Dom.Element("length", Convert.ToString(utune.length, System.Globalization.CultureInfo.InvariantCulture)));
				tune.AddChild(new agsXMPP.Xml.Dom.Element("rating", Convert.ToString(utune.rating, System.Globalization.CultureInfo.InvariantCulture)));
				tune.AddChild(new agsXMPP.Xml.Dom.Element("track", Convert.ToString(utune.track, System.Globalization.CultureInfo.InvariantCulture)));
				tune.AddChild(new agsXMPP.Xml.Dom.Element("artist", (utune.artist != null) ? utune.artist.Trim() : string.Empty));
				tune.AddChild(new agsXMPP.Xml.Dom.Element("source", (utune.source != null) ? utune.source.Trim() : string.Empty));
				tune.AddChild(new agsXMPP.Xml.Dom.Element("title", (utune.title != null) ? utune.title.Trim() : string.Empty));
				tune.AddChild(new agsXMPP.Xml.Dom.Element("uri", (utune.uri != null) ? utune.uri.Trim() : string.Empty));
				item.AddChild(tune);
				ps.AddChild(item);
				pb.AddChild(ps);
				agsXMPP.Xml.Dom.Element conf = new agsXMPP.Xml.Dom.Element("configure");
				agsXMPP.Xml.Dom.Element x = new agsXMPP.Xml.Dom.Element("x");
				agsXMPP.Xml.Dom.Element field1 = new agsXMPP.Xml.Dom.Element("field");
				field1.Attributes.Add("type", "hidden");
				field1.Attributes.Add("var", "FORM_TYPE");
				agsXMPP.Xml.Dom.Element value1 = new agsXMPP.Xml.Dom.Element("value");
				value1.Value = "http://jabber.org/protocol/pubsub#node_config";
				field1.AddChild(value1);
				x.AddChild(field1);
				agsXMPP.Xml.Dom.Element field2 = new agsXMPP.Xml.Dom.Element("field");
				field2.Attributes.Add("var", "pubsub#access_model");
				agsXMPP.Xml.Dom.Element value2 = new agsXMPP.Xml.Dom.Element("value");
				value2.Value = "presence";
				field2.AddChild(value2);
				x.AddChild(field2);
				conf.AddChild(x);
				pb.AddChild(conf);
				iq.AddChild(pb);
				Jabber.xmpp.Send(iq);
				_tune = new Tune();
				_tune.artist = (utune.artist != null) ? utune.artist.Trim() : string.Empty;
				_tune.length = utune.length;
				_tune.rating = utune.rating;
				_tune.source = (utune.source != null) ? utune.source.Trim() : string.Empty;
				_tune.title = (utune.title != null) ? utune.title.Trim() : string.Empty;
				_tune.track = utune.track;
				_tune.uri = (utune.uri != null) ? utune.uri.Trim() : string.Empty;
			}
		}

		/// <summary>
		/// Suprimme la notification de ce que j'écoute
		/// </summary>
		public void clearTune()
		{
			if (Jabber.xmpp.Authenticated && Jabber._pepCapable)
			{
				agsXMPP.protocol.client.IQ iq = new agsXMPP.protocol.client.IQ();
				iq.Type = agsXMPP.protocol.client.IqType.set;
				iq.GenerateId();
				agsXMPP.Xml.Dom.Element pb = new agsXMPP.Xml.Dom.Element("pubsub");
				pb.Namespace = "http://jabber.org/protocol/pubsub";
				agsXMPP.Xml.Dom.Element ps = new agsXMPP.Xml.Dom.Element("publish");
				ps.Attributes.Add("node", "http://jabber.org/protocol/tune");
				agsXMPP.Xml.Dom.Element item = new agsXMPP.Xml.Dom.Element("item");
				item.Attributes.Add("id", "current");
				agsXMPP.Xml.Dom.Element tune = new agsXMPP.Xml.Dom.Element("tune");
				tune.Namespace = "http://jabber.org/protocol/tune";
				item.AddChild(tune);
				ps.AddChild(item);
				pb.AddChild(ps);
				iq.AddChild(pb);
				Jabber.xmpp.Send(iq);
				_tune = new Tune();
				_tune.artist = string.Empty;
				_tune.length = 0;
				_tune.rating = 1;
				_tune.source = string.Empty;
				_tune.title = string.Empty;
				_tune.track = 0;
				_tune.uri = string.Empty;
			}
		}

		#endregion

		/// <summary>
		/// Se produit à chaque fois que l'inactivité est trop longue
		/// </summary>
		/// <param name="sender">Objet parent</param>
		/// <param name="e">Arguments du timer</param>
		private void autoIdleTimerElapsed(object sender, System.Timers.ElapsedEventArgs e)
		{
			if (autoIdle)
			{
				int idleMinutes = ((Environment.TickCount - getLastActivityTicks()) / 1000) / 60;
				if (idleMinutes >= autoIdleMinutes && !_autoIdleState)
				{
					_autoIdleState = true;
					_autoIdleLastStatus = status;
					status = autoIdleStatus;
					applyStatus();
				}
				else if (idleMinutes < autoIdleMinutes && _autoIdleState)
				{
					_autoIdleState = false;
					status = _autoIdleLastStatus;
					applyStatus();
				}
			}
		}

	}

}
