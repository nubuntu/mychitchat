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
	/// Gestion des différentes erreurs
	/// </summary>
	public class ErrorManager : IDisposable
	{

		#region Events

		/// <summary>
		/// Se produit lorsqu'une erreur se produit
		/// </summary>
		public event ErrorHandler ErrorRaised;
		private void OnErrorRaised(Enums.ErrorType type, string message)
		{
			try
			{
				ErrorRaised(type, message);
			}
			catch { }
		}

		#endregion

		#region Constructor

		/// <summary>
		/// Constructeur
		/// </summary>
		public ErrorManager()
		{
			Jabber.xmpp.OnError += new agsXMPP.ErrorHandler(Error);
			Jabber.xmpp.OnAuthError += new agsXMPP.XmppElementHandler(ElementError);
			Jabber.xmpp.OnRegisterError += new agsXMPP.XmppElementHandler(ElementError);
			Jabber.xmpp.OnSocketError += new agsXMPP.ErrorHandler(Error);
			//Jabber.xmpp.OnXmppError += new agsXMPP.XmppElementHandler(ElementError);
		}

		#endregion

		#region Destructor

		/// <summary>
		/// Destructeur
		/// </summary>
		public void Dispose()
		{
			Jabber.xmpp.OnError -= new agsXMPP.ErrorHandler(Error);
			Jabber.xmpp.OnAuthError -= new agsXMPP.XmppElementHandler(ElementError);
			Jabber.xmpp.OnRegisterError -= new agsXMPP.XmppElementHandler(ElementError);
			Jabber.xmpp.OnSocketError -= new agsXMPP.ErrorHandler(Error);
			//Jabber.xmpp.OnXmppError -= new agsXMPP.XmppElementHandler(ElementError);
		}

		#endregion

		#region Public method

		/// <summary>
		/// Provoque un événement d'erreur personnalisé
		/// </summary>
		/// <param name="type">Type de l'erreur</param>
		/// <param name="message">Description de l'erreur</param>
		public void custom(Enums.ErrorType type, string message)
		{
			OnErrorRaised(type, message);
		}

		#endregion

		/// <summary>
		/// Exceptions
		/// </summary>
		/// <param name="sender">Objet parent</param>
		/// <param name="ex">Exception</param>
		private void Error(object sender, Exception ex)
		{
			OnErrorRaised(Enums.ErrorType.Client, ex.Message.Trim() + ex.StackTrace.Trim());
		}

		/// <summary>
		/// XMPP
		/// </summary>
		/// <param name="sender">Objet parent</param>
		/// <param name="el">Element</param>
		private void ElementError(object sender, agsXMPP.Xml.Dom.Element el)
		{
			if (el.HasTag("error", true))
			{
				agsXMPP.Xml.Dom.Element e = el.SelectSingleElement("error", true);
				int code = 0;
				if (e.HasAttribute("code")) { code = Convert.ToInt32(e.Attribute("code")); }
				switch (code)
				{
					case 302:
						OnErrorRaised(Enums.ErrorType.Query, "Demande redirigé.");
						break;
					case 400:
						OnErrorRaised(Enums.ErrorType.Query, "Demande incorrecte.");
						break;
					case 401:
						OnErrorRaised(Enums.ErrorType.Authentification, "Compte non autaurisé.");
						break;
					case 402:
						OnErrorRaised(Enums.ErrorType.Authentification, "Compte incorrect.");
						break;
					case 403:
						OnErrorRaised(Enums.ErrorType.Authentification, "Compte refusé.");
						break;
					case 404:
						OnErrorRaised(Enums.ErrorType.Warning, "Demande inconnue.");
						break;
					case 405:
						OnErrorRaised(Enums.ErrorType.Warning, "Demande interdite.");
						break;
					case 406:
						OnErrorRaised(Enums.ErrorType.Query, "Demande non autorisée.");
						break;
					case 407:
						OnErrorRaised(Enums.ErrorType.Authentification, "Compte non enregistré.");
						break;
					case 408:
						OnErrorRaised(Enums.ErrorType.Server, "Temps limite atteint.");
						break;
					case 409:
						OnErrorRaised(Enums.ErrorType.Warning, "Demande en conflit.");
						break;
					case 500:
						OnErrorRaised(Enums.ErrorType.Server, "Erreur interne.");
						break;
					case 501:
						OnErrorRaised(Enums.ErrorType.Warning, "Demande non implémentée.");
						break;
					case 502:
						OnErrorRaised(Enums.ErrorType.Server, "Erreur distante.");
						break;
					case 503:
						OnErrorRaised(Enums.ErrorType.Warning, "Service temporairement innaccessible.");
						break;
					case 504:
						OnErrorRaised(Enums.ErrorType.Server, "Temps limite distant atteint.");
						break;
					case 510:
						OnErrorRaised(Enums.ErrorType.Warning, "Déconnecté.");
						break;
					default:
						OnErrorRaised(Enums.ErrorType.Client, "Erreur générale.");
						break;
				}
			}
			else
			{
				if(el.HasTag("bad-request", true)) { OnErrorRaised(Enums.ErrorType.Query, "Demande incorrecte."); }
				else if(el.HasTag("conflict", true)) { OnErrorRaised(Enums.ErrorType.Warning, "Demande en conflit."); }
				else if(el.HasTag("feature-not-implemented", true)) { OnErrorRaised(Enums.ErrorType.Warning, "Demande non implémentée."); }
				else if(el.HasTag("forbidden", true)) { OnErrorRaised(Enums.ErrorType.Authentification, "Compte non autorisé."); }
				else if(el.HasTag("gone", true)) { OnErrorRaised(Enums.ErrorType.Query, "Demande redirigée"); }
				else if(el.HasTag("internal-server-error", true)) { OnErrorRaised(Enums.ErrorType.Server, "Erreur interne."); }
				else if(el.HasTag("item-not-found", true)) { OnErrorRaised(Enums.ErrorType.Warning, "Elément inconnu."); }
				else if(el.HasTag("jid-malformed", true)) { OnErrorRaised(Enums.ErrorType.Query, "Jabber ID incorrect."); }
				else if(el.HasTag("not-acceptable", true)) { OnErrorRaised(Enums.ErrorType.Query, "Demande non acceptée."); }
				else if(el.HasTag("not-allowed", true)) { OnErrorRaised(Enums.ErrorType.Server, "Opération non autorisée."); }
				else if(el.HasTag("not-authorized", true)) { OnErrorRaised(Enums.ErrorType.Query, "Demande non autorisée."); }
				else if(el.HasTag("payment-required", true)) { OnErrorRaised(Enums.ErrorType.Query, "Finalisation requise."); }
				else if(el.HasTag("recipient-unavailable", true)) { OnErrorRaised(Enums.ErrorType.Query, "Destinataire non disponible."); }
				else if(el.HasTag("redirect", true)) { OnErrorRaised(Enums.ErrorType.Query, "Demande redirigé."); }
				else if(el.HasTag("registration-required", true)) { OnErrorRaised(Enums.ErrorType.Authentification, "Compte non enregistré."); }
				else if(el.HasTag("remote-server-not-found", true)) { OnErrorRaised(Enums.ErrorType.Server, "Erreur distante."); }
				else if(el.HasTag("remote-server-timeout", true)) { OnErrorRaised(Enums.ErrorType.Server, "Temps limite distant atteint."); }
				else if(el.HasTag("resource-constraint", true)) { OnErrorRaised(Enums.ErrorType.Authentification, "Ressource valide requise."); }
				else if(el.HasTag("service-unavailable", true)) { OnErrorRaised(Enums.ErrorType.Warning, "Service temporairement innaccessible."); }
				else if(el.HasTag("subscription-required", true)) { OnErrorRaised(Enums.ErrorType.Client, "Abonnement requis."); }
				else if(el.HasTag("undefined-condition", true)) { OnErrorRaised(Enums.ErrorType.Client, "Condition inconnue."); }
				else if(el.HasTag("unexpected-request", true)) { OnErrorRaised(Enums.ErrorType.Query, "Demande incorrecte."); }
			}
		}

	}

}
