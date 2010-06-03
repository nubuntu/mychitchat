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

namespace nJim
{

	/// <summary>
	/// Gestion des listes privées
	/// </summary>
	public class Privacy : IDisposable
	{

		#region Events

		/// <summary>
		/// Se produit lorsque la liste d'identifiants Jabber a été mise à jour
		/// </summary>
		public event NeutralHandler PrivacyListUpdated;
		private void OnPrivacyListUpdated()
		{
			try
			{
				PrivacyListUpdated(this);
			}
			catch { }
		}

		#endregion

		#region Properties

		private List<string> _blockedJid = new List<string>();
		/// <summary>
		/// Liste des identifiants Jabber bloqués
		/// </summary>
		public List<string> blockedJid
		{
			get { return _blockedJid; }
		}

		#endregion

		#region Constructor

		/// <summary>
		/// Constructeur
		/// </summary>
		public Privacy()
		{
			getList();
		}

		#endregion

		#region Destructor

		/// <summary>
		/// Destructeur
		/// </summary>
		public void Dispose()
		{
			Jabber.xmpp.OnIq -= new agsXMPP.protocol.client.IqHandler(xmppOnIq);
		}

		#endregion

		#region Public methods

		/// <summary>
		/// Bloque un identifiant Jabber
		/// </summary>
		/// <param name="jbid">Identifiant Jabber</param>
		public void block(string jbid)
		{
			if (Jabber.xmpp.Authenticated)
			{
				agsXMPP.Jid jid = new agsXMPP.Jid(jbid);
				if (jid != null && jid.ToString() != Jabber.xmpp.MyJID.ToString())
				{
					agsXMPP.protocol.iq.privacy.PrivacyIq piq = new agsXMPP.protocol.iq.privacy.PrivacyIq();
					piq.From = Jabber.xmpp.MyJID;
					piq.Type = agsXMPP.protocol.client.IqType.set;
					agsXMPP.protocol.iq.privacy.List list = new agsXMPP.protocol.iq.privacy.List(Jabber.xmpp.MyJID.Bare.Replace("@", "_").Replace("/", "_").Replace(".", "_"));
					agsXMPP.protocol.iq.privacy.Item item = new agsXMPP.protocol.iq.privacy.Item();
					item.Action = agsXMPP.protocol.iq.privacy.Action.deny;
					item.BlockIncomingPresence = false;
					item.BlockIq = true;
					item.BlockMessage = true;
					item.BlockOutgoingPresence = true;
					item.Order = 1;
					item.Stanza = agsXMPP.protocol.iq.privacy.Stanza.Iq | agsXMPP.protocol.iq.privacy.Stanza.Message | agsXMPP.protocol.iq.privacy.Stanza.OutgoingPresence;
					item.Type = agsXMPP.protocol.iq.privacy.Type.jid;
					item.Val = jid.ToString();
					list.AddItem(item);
					piq.Query.AddList(list);
					PrivacyStructure ps = new PrivacyStructure();
					ps.id = piq.Id;
					ps.jid = jid;
					Jabber.xmpp.IqGrabber.SendIq(piq, new agsXMPP.IqCB(blockResult), ps);
				}
			}
		}

		/// <summary>
		/// Débloque un contact
		/// </summary>
		/// <param name="jbid">Identifiant Jabber</param>
		public void unblock(string jbid)
		{
			if (Jabber.xmpp.Authenticated)
			{
				agsXMPP.Jid jid = new agsXMPP.Jid(jbid);
				if (jid != null && jid.ToString() != Jabber.xmpp.MyJID.ToString())
				{
					agsXMPP.protocol.iq.privacy.PrivacyIq piq = new agsXMPP.protocol.iq.privacy.PrivacyIq();
					piq.From = Jabber.xmpp.MyJID;
					piq.Type = agsXMPP.protocol.client.IqType.set;
					agsXMPP.protocol.iq.privacy.List list = new agsXMPP.protocol.iq.privacy.List(Jabber.xmpp.MyJID.Bare.Replace("@", "_").Replace("/", "_").Replace(".", "_"));
					agsXMPP.protocol.iq.privacy.Item item = new agsXMPP.protocol.iq.privacy.Item();
					item.Action = agsXMPP.protocol.iq.privacy.Action.allow;
					item.BlockIncomingPresence = false;
					item.BlockIq = true;
					item.BlockMessage = true;
					item.BlockOutgoingPresence = true;
					item.Order = 1;
					item.Stanza = agsXMPP.protocol.iq.privacy.Stanza.Iq | agsXMPP.protocol.iq.privacy.Stanza.Message | agsXMPP.protocol.iq.privacy.Stanza.OutgoingPresence;
					item.Type = agsXMPP.protocol.iq.privacy.Type.jid;
					item.Val = jid.ToString();
					list.AddItem(item);
					piq.Query.AddList(list);
					PrivacyStructure ps = new PrivacyStructure();
					ps.id = piq.Id;
					ps.jid = jid;
					Jabber.xmpp.IqGrabber.SendIq(piq, new agsXMPP.IqCB(unblockResult), ps);
				}
			}
		}

		#endregion

		private void xmppOnIq(object sender, agsXMPP.protocol.client.IQ iq)
		{
			if (iq.Type == agsXMPP.protocol.client.IqType.set && iq.Id.ToLower().Contains("push"))
			{
				if (iq.Query != null && iq.Query is agsXMPP.protocol.iq.privacy.Privacy)
				{
					agsXMPP.protocol.client.IQ pushIqResult = new agsXMPP.protocol.client.IQ();
					pushIqResult.From = Jabber.xmpp.MyJID;
					pushIqResult.To = iq.From;
					pushIqResult.Type = agsXMPP.protocol.client.IqType.result;
					pushIqResult.Id = iq.Id;
					Jabber.xmpp.Send(pushIqResult);
					getList();
				}
			}
		}

		private void getList()
		{
			Jabber.xmpp.OnIq += new agsXMPP.protocol.client.IqHandler(xmppOnIq);
			agsXMPP.protocol.iq.privacy.PrivacyIq piq = new agsXMPP.protocol.iq.privacy.PrivacyIq();
			piq.From = Jabber.xmpp.MyJID;
			piq.Type = agsXMPP.protocol.client.IqType.get;
			piq.Query.AddList(new agsXMPP.protocol.iq.privacy.List(Jabber.xmpp.MyJID.Bare.Replace("@", "_").Replace("/", "_").Replace(".", "_")));
			Jabber.xmpp.IqGrabber.SendIq(piq, new agsXMPP.IqCB(getListResult), piq.Id);
		}

		private void getListResult(object sender, agsXMPP.protocol.client.IQ iq, object data)
		{
			if (iq.Type == agsXMPP.protocol.client.IqType.result && iq.Error == null)
			{
				if (iq.Query != null && iq.Query is agsXMPP.protocol.iq.privacy.Privacy)
				{
					agsXMPP.protocol.iq.privacy.Privacy privacy = iq.Query as agsXMPP.protocol.iq.privacy.Privacy;
					if (Jabber.xmpp.IqGrabber != null)
					{
						Jabber.xmpp.IqGrabber.Remove(data as string);
					}
					agsXMPP.protocol.iq.privacy.List customList = null;
					agsXMPP.protocol.iq.privacy.List[] lists = privacy.GetList();
					if (lists != null)
					{
						foreach (agsXMPP.protocol.iq.privacy.List list in lists)
						{
							if (list.Name == Jabber.xmpp.MyJID.Bare.Replace("@", "_").Replace("/", "_").Replace(".", "_"))
							{
								customList = list;
								break;
							}
						}
					}
					if (customList != null)
					{
						agsXMPP.protocol.iq.privacy.Item[] items = customList.GetItems();
						if (items != null)
						{
							bool updated = false;
							foreach (agsXMPP.protocol.iq.privacy.Item item in items)
							{
								if (item.Type == agsXMPP.protocol.iq.privacy.Type.jid)
								{
									agsXMPP.Jid jid = new agsXMPP.Jid(item.Val);
									if (jid != null)
									{
										if (item.Action == agsXMPP.protocol.iq.privacy.Action.deny && !_blockedJid.Contains(jid.ToString()))
										{
											_blockedJid.Add(jid.ToString());
											updated = true;
										}
										else if (item.Action == agsXMPP.protocol.iq.privacy.Action.allow && _blockedJid.Contains(jid.ToString()))
										{
											_blockedJid.Remove(jid.ToString());
											updated = true;
										}
									}
								}
							}
							if (updated)
							{
								OnPrivacyListUpdated();
							}
						}
					}
				}
			}
		}

		private void blockResult(object sender, agsXMPP.protocol.client.IQ iq, object data)
		{
			PrivacyStructure ps = (PrivacyStructure)data;
			if (iq.Type == agsXMPP.protocol.client.IqType.result && iq.Id == ps.id)
			{
				if (!_blockedJid.Contains(ps.jid.ToString()))
				{
					_blockedJid.Add(ps.jid.ToString());
				}
			}
		}

		private void unblockResult(object sender, agsXMPP.protocol.client.IQ iq, object data)
		{
			PrivacyStructure ps = (PrivacyStructure)data;
			if (iq.Type == agsXMPP.protocol.client.IqType.result && iq.Id == ps.id)
			{
				if (_blockedJid.Contains(ps.jid.ToString()))
				{
					_blockedJid.Remove(ps.jid.ToString());
				}
			}
		}



	}

}
