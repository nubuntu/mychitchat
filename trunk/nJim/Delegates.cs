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
	/// Délégué pour les événements liés à la classe Identity
	/// </summary>
	/// <param name="sender">Class Identity appelante</param>
	public delegate void IdentityHandler(Identity sender);

	/// <summary>
	/// Délégué pour les événements liés à la classe ErrorManager
	/// </summary>
	/// <param name="type">Type de l'erreur</param>
	/// <param name="message">Description de l'erreur</param>
	public delegate void ErrorHandler(Enums.ErrorType type, string message);

	/// <summary>
	/// Délégué neutre
	/// </summary>
	public delegate void NeutralHandler(object sender);

	/// <summary>
	/// Délégué pour les événements liés aux Contacts
	/// </summary>
	/// <param name="bare">Contact incriminé</param>
	public delegate void ContactHandler(string bare);

	/// <summary>
	/// Délégué pour les événements liés aux Resources
	/// </summary>
	/// <param name="contact">Ressource incriminée</param>
	public delegate void ResourceHandler(Contact contact);

	/// <summary>
	/// Délégué pour les événements liés aux Services
	/// </summary>
	/// <param name="service">Service</param>
	public delegate void ServiceHandler(Service service);

	/// <summary>
	/// Délégué pour les événements liés à la classe Contact
	/// </summary>
	/// <param name="version">Version du client logiciel</param>
	public delegate void ClientVersionHandler(ClientVersion version);

	/// <summary>
	/// Délégué pour les événements liés à la classe Contact
	/// </summary>
	/// <param name="time">Temps du contact</param>
	public delegate void ClientTimeHandler(DateTime time);

	/// <summary>
	/// Délégué pour les événements liés à la classe Contact
	/// </summary>
	/// <param name="time">Durée d'inactivité</param>
	public delegate void ClientTimespanHandler(TimeSpan time);

	/// <summary>
	/// Délégué lié au changement d'hummeur
	/// </summary>
	/// <param name="contact">LA resource incriminée</param>
	/// <param name="mood">L'hummeur annoncée</param>
	public delegate void ResourceMoodHandler(Contact contact, Mood mood);

	/// <summary>
	/// Délégué lié au changement d'activité
	/// </summary>
	/// <param name="contact">LA resource incriminée</param>
	/// <param name="activity">L'activité annoncée</param>
	public delegate void ResourceActivityHandler(Contact contact, Activity activity);

    /// <summary>
    /// Délégué lié au changement d'emplacement géographique
    /// </summary>
    /// <param name="contact">LA resource incriminée</param>
    /// <param name="location">L'emplacement géographique annoncé</param>
    public delegate void ResourceLocationHandler(Contact contact, Location location);

	/// <summary>
	/// Délégué lié au changement de lecture
	/// </summary>
	/// <param name="contact">LA resource incriminée</param>
	/// <param name="tune">Lecture en cours annoncée</param>
	public delegate void ResourceTuneHandler(Contact contact, Tune tune);





}
