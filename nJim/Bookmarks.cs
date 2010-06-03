using System;
using System.Collections.Generic;
using System.Text;

namespace nJim
{

	/// <summary>
	/// Gestion des marques ta page!
	/// </summary>
	public class Bookmarks : IDisposable
	{
		
		#region events

		/// <summary>
		/// Une nouvelle liste de favoris est disponible
		/// </summary>
		public event NeutralHandler FavoritesAvailables;
		private void OnFavoritesAvailables()
		{
			try
			{
				FavoritesAvailables(this);
			}
			catch { }
		}

		/// <summary>
		/// Une nouvelle liste de salons est disponible
		/// </summary>
		public event NeutralHandler RoomsAvailables;
		private void OnRoomsAvailables()
		{
			try
			{
				RoomsAvailables(this);
			}
			catch { }
		}

		#endregion

		#region Properties

		private List<Favorite> _favorites = new List<Favorite>();
		/// <summary>
		/// Liste des favoris
		/// </summary>
		public List<Favorite> favorites
		{
			get { return _favorites; }
		}

		private List<Room> _rooms = new List<Room>();
		/// <summary>
		/// Liste des salons de discussions
		/// </summary>
		public List<Room> rooms
		{
			get { return _rooms; }
		}

		#endregion

		#region Constructor

		/// <summary>
		/// Constructeur
		/// </summary>
		public Bookmarks()
		{
			retrieve();
		}

		#endregion

		#region Destructor

		/// <summary>
		/// Destructeur
		/// </summary>
		public void Dispose()
		{
			publishFavorites();
			publishRooms();
		}

		#endregion

		#region Public methods

		/// <summary>
		/// Charge les bookamrks enregistrés sur le serveur
		/// </summary>
		public void retrieve()
		{
			if (Jabber.xmpp.Authenticated)
			{
				agsXMPP.protocol.extensions.bookmarks.BookmarkManager bm = new agsXMPP.protocol.extensions.bookmarks.BookmarkManager(Jabber.xmpp);
				bm.RequestBookmarks(new agsXMPP.IqCB(retrieveResult));
			}
		}

		/// <summary>
		/// Sauvegarde les favoris
		/// </summary>
		public void publishFavorites()
		{
			if (Jabber.xmpp.Authenticated && _favorites.Count > 0)
			{
				agsXMPP.protocol.extensions.bookmarks.BookmarkManager bm = new agsXMPP.protocol.extensions.bookmarks.BookmarkManager(Jabber.xmpp);
				agsXMPP.protocol.extensions.bookmarks.Url[] urls = new agsXMPP.protocol.extensions.bookmarks.Url[_favorites.Count];
				int counter = 0;
				foreach (Favorite f in _favorites)
				{
					urls[counter] = new agsXMPP.protocol.extensions.bookmarks.Url(f.address, f.name);
					counter++;
				}
				bm.StoreBookmarks(urls);
			}
		}

		/// <summary>
		/// Sauvegarde les salons
		/// </summary>
		public void publishRooms()
		{
			if (Jabber.xmpp.Authenticated && _rooms.Count > 0)
			{
				agsXMPP.protocol.extensions.bookmarks.BookmarkManager bm = new agsXMPP.protocol.extensions.bookmarks.BookmarkManager(Jabber.xmpp);
				agsXMPP.protocol.extensions.bookmarks.Conference[] conferences = new agsXMPP.protocol.extensions.bookmarks.Conference[_rooms.Count];
				int counter = 0;
				foreach (Room r in _rooms)
				{
					conferences[counter] = new agsXMPP.protocol.extensions.bookmarks.Conference(new agsXMPP.Jid(r.jabberID.full), r.name, r.nickname, r.password, r.autoJoin);
					counter++;
				}
				bm.StoreBookmarks(conferences);
			}
		}

		#endregion

		/// <summary>
		/// Resultat de la requette RequestBookmarks
		/// </summary>
		/// <param name="sender">Objet parent</param>
		/// <param name="iq">Requette</param>
		/// <param name="data">Données supplémentaires</param>
		private void retrieveResult(object sender, agsXMPP.protocol.client.IQ iq, object data)
		{
			if (iq.Type == agsXMPP.protocol.client.IqType.result)
			{
				if (iq.Query != null && iq.Query.HasTag(typeof(agsXMPP.protocol.extensions.bookmarks.Storage)))
				{
					if (iq.Query.SelectSingleElement(typeof(agsXMPP.protocol.extensions.bookmarks.Storage)) != null)
					{
						agsXMPP.protocol.extensions.bookmarks.Storage st = iq.Query.SelectSingleElement(typeof(agsXMPP.protocol.extensions.bookmarks.Storage)) as agsXMPP.protocol.extensions.bookmarks.Storage;
						agsXMPP.protocol.extensions.bookmarks.Url[] urls = st.GetUrls();
						if (urls != null)
						{
							_favorites.Clear();
							foreach (agsXMPP.protocol.extensions.bookmarks.Url url in urls)
							{
								if (url.Address != null && url.Name != null)
								{
									Favorite f = new Favorite();
									f.address = url.Address;
									f.name = url.Name;
									if (!_favorites.Contains(f))
									{
										_favorites.Add(f);
									}
								}
							}
							OnFavoritesAvailables();
						}
						agsXMPP.protocol.extensions.bookmarks.Conference[] conferences = st.GetConferences();
						if (conferences != null)
						{
							_rooms.Clear();
							foreach (agsXMPP.protocol.extensions.bookmarks.Conference conference in conferences)
							{
								if (conference.Name != null && conference.Jid != null && conference.Nickname != null)
								{
									Room r = new Room();
									r.autoJoin = conference.AutoJoin;
									r.jabberID = new JabberID();
									r.jabberID.user = conference.Jid.User;
									r.jabberID.resource = conference.Jid.Resource;
									r.jabberID.domain = conference.Jid.Server;
									r.jabberID.bare = conference.Jid.Bare;
									r.jabberID.full = conference.Jid.ToString();
									r.name = conference.Name;
									r.nickname = conference.Nickname;
									r.password = (conference.Password != null) ? conference.Password : string.Empty;
									if (!_rooms.Contains(r))
									{
										_rooms.Add(r);
									}

								}
							}
							OnRoomsAvailables();
						}
					}
				}
			}
		}

	}

}
