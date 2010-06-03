using System;
using System.Collections.Generic;
using System.Text;
using agsXMPP.protocol.client;
using agsXMPP.protocol.iq.disco;
using System.Reflection;
using System.IO;
using System.Security.Cryptography;
using agsXMPP.protocol.extensions.caps;

namespace nJim
{

	/// <summary>
	/// Gestion de requètes entrantes
	/// </summary>
	public class Queries : IDisposable
	{

		#region Constructor

		/// <summary>
		/// Constructeur
		/// </summary>
		public Queries()
		{
			Jabber.xmpp.OnIq += new IqHandler(xmppOnIq);
		}

		#endregion

		#region Desctructor

		/// <summary>
		/// Destructeur
		/// </summary>
		public void Dispose()
		{
			Jabber.xmpp.OnIq -= new IqHandler(xmppOnIq);
		}

		#endregion

		/// <summary>
		/// Empaqquete un DIscoInfo
		/// </summary>
		/// <param name="info">DicoInfo</param>
		/// <returns>Ampaquetage</returns>
		public static string DiscoInfoToVersion(DiscoInfo info)
		{
			StringBuilder builder = new StringBuilder(256);
			DiscoIdentity[] identities = info.GetIdentities();
			string[] ids = new string[identities.Length];
			for (int i = 0; i < identities.Length; i++)
			{
				ids[i] = string.Format("{0}/{1}", identities[i].Category, identities[i].Type);
			}
			Array.Sort(ids);
			foreach (string id in ids)
			{
				builder.AppendFormat("{0}<", id);
			}
			DiscoFeature[] features = info.GetFeatures();
			string[] feas = new string[features.Length];
			for (int i = 0; i < features.Length; i++)
			{
				feas[i] = features[i].Var;
			}
			Array.Sort(feas);
			foreach (string fea in feas)
			{
				builder.AppendFormat("{0}<", fea);
			}
			SHA1Managed sha1 = new SHA1Managed();
			byte[] hash = sha1.ComputeHash(Encoding.Unicode.GetBytes(builder.ToString()));
			return Convert.ToBase64String(hash);
		}

		/// <summary>
		/// Retourne les fonctionnalité du client
		/// </summary>
		/// <returns>Fonctionnalités</returns>
		public static Capabilities getCapabilities()
		{
			agsXMPP.protocol.extensions.caps.Capabilities caps = new agsXMPP.protocol.extensions.caps.Capabilities(Queries.DiscoInfoToVersion(Queries.getDiscoInfo()), "http://microphage71.hostarea.org/caps");
			caps.AddExtension("mood");
			caps.AddExtension("tune");
			caps.AddExtension("geoloc");
			caps.AddExtension("activity");
			caps.AddExtension("cs");
			caps.AddExtension("ep-notify");
			return caps;
		}

		/// <summary>
		/// Retourne notre DiscoInfo
		/// </summary>
		/// <returns>DiscoInfo</returns>
		public static DiscoInfo getDiscoInfo()
		{
			DiscoInfo di = new DiscoInfo();
			di.AddIdentity(new DiscoIdentity("pc", "client", "nJim"));
			di.AddFeature(new DiscoFeature(agsXMPP.Uri.CLIENT));
			di.AddFeature(new DiscoFeature(agsXMPP.Uri.DISCO_INFO));
			di.AddFeature(new DiscoFeature(agsXMPP.Uri.IQ_AVATAR));
			di.AddFeature(new DiscoFeature(agsXMPP.Uri.VCARD));
			di.AddFeature(new DiscoFeature(agsXMPP.Uri.VCARD_UPDATE));
			di.AddFeature(new DiscoFeature(agsXMPP.Uri.X_AVATAR));
			di.AddFeature(new DiscoFeature(agsXMPP.Uri.X_DELAY));
			di.AddFeature(new DiscoFeature(agsXMPP.Uri.IQ_VERSION));
			di.AddFeature(new DiscoFeature(agsXMPP.Uri.IQ_TIME));
			di.AddFeature(new DiscoFeature(agsXMPP.Uri.IQ_LAST));
			di.AddFeature(new DiscoFeature(agsXMPP.Uri.PING));
			di.AddFeature(new DiscoFeature(agsXMPP.Uri.IQ_PRIVACY));
			di.AddFeature(new DiscoFeature(agsXMPP.Uri.STORAGE_BOOKMARKS));
			di.AddFeature(new DiscoFeature("http://jabber.org/protocol/mood"));
			di.AddFeature(new DiscoFeature("http://jabber.org/protocol/mood+notify"));
			di.AddFeature(new DiscoFeature("http://jabber.org/protocol/tune"));
			di.AddFeature(new DiscoFeature("http://jabber.org/protocol/tune+notify"));
			di.AddFeature(new DiscoFeature("http://jabber.org/protocol/geoloc"));
			di.AddFeature(new DiscoFeature("http://jabber.org/protocol/geoloc+notify"));
			di.AddFeature(new DiscoFeature("http://jabber.org/protocol/activity"));
			di.AddFeature(new DiscoFeature("http://jabber.org/protocol/activity+notify"));
			return di;
		}

		private void xmppOnIq(object sender, IQ iq)
		{
			if (iq.Type == IqType.get)
			{
				if (iq.Query != null)
				{
					if (iq.Query is DiscoInfo)
					{
						iq.SwitchDirection();
						iq.Type = IqType.result;
						DiscoInfo di = getDiscoInfo();
						iq.Query = di;
						Jabber.xmpp.Send(iq);
					}
					else if (iq.Query is DiscoItems)
					{
						iq.SwitchDirection();
						iq.Type = IqType.error;
						iq.Error = new Error(ErrorType.cancel, ErrorCondition.FeatureNotImplemented);
						Jabber.xmpp.Send(iq);
					}
					else if (iq.Query is agsXMPP.protocol.iq.version.Version)
					{
						iq.SwitchDirection();
						iq.Type = IqType.result;
						agsXMPP.protocol.iq.version.Version version = iq.Query as agsXMPP.protocol.iq.version.Version;
						version.Name = Assembly.GetExecutingAssembly().GetName().Name;
						version.Ver = Assembly.GetExecutingAssembly().GetName().Version.ToString();
						version.Os = Environment.OSVersion.ToString();
						Jabber.xmpp.Send(iq);
					}
					else if (iq.Query is agsXMPP.protocol.iq.last.Last)
					{
						iq.SwitchDirection();
						iq.Type = IqType.result;
						agsXMPP.protocol.iq.last.Last last = iq.Query as agsXMPP.protocol.iq.last.Last;
						last.Seconds = new TimeSpan(Jabber._presence.getLastActivityTicks()).Seconds;
						Jabber.xmpp.Send(iq);
					}
					else if (iq.Query is agsXMPP.protocol.iq.time.Time)
					{
						iq.SwitchDirection();
						iq.Type = IqType.result;
						agsXMPP.protocol.iq.time.Time time = iq.Query as agsXMPP.protocol.iq.time.Time;
						time.Display = DateTime.Now.ToLongDateString() + " " + DateTime.Now.ToLongTimeString();
						time.Tz = TimeZone.CurrentTimeZone.StandardName;
						time.Utc = agsXMPP.util.Time.ISO_8601Date(DateTime.Now);
						Jabber.xmpp.Send(iq);
					}
					else if (iq.Query is agsXMPP.protocol.extensions.ping.Ping)
					{
						iq.SwitchDirection();
						iq.Type = IqType.result;
						agsXMPP.protocol.extensions.ping.Ping ping = iq.Query as agsXMPP.protocol.extensions.ping.Ping;
						Jabber.xmpp.Send(iq);
					}
					else if (iq.Query is agsXMPP.protocol.iq.avatar.Avatar)
					{
						iq.SwitchDirection();
						iq.Type = IqType.result;
						agsXMPP.protocol.iq.avatar.Avatar avatar = iq.Query as agsXMPP.protocol.iq.avatar.Avatar;
						avatar.Data = null;
						if (Jabber._identity.photo != null && Jabber._identity.photoFormat != null)
						{
							MemoryStream ms = new MemoryStream();
							Jabber._identity.photo.Save(ms, Jabber._identity.photoFormat);
							avatar.Data = ms.ToArray();
							avatar.MimeType = "image/" + Jabber._identity.photoFormat.ToString();
							ms.Close();
							ms.Dispose();
						}
						Jabber.xmpp.Send(iq);
					}
					else if (iq.Query is agsXMPP.protocol.iq.vcard.Vcard)
					{
						iq.SwitchDirection();
						iq.Type = IqType.result;
						iq.Query = Jabber._identity.toVcard();
						Jabber.xmpp.Send(iq);
					}









				}
			}
		}

	}

}
