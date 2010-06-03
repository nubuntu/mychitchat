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
    /// Identifiant Jabber scindé en multiples parties
    /// </summary>
    public struct JabberID
    {

        /// <summary>
        /// Utilisateur
        /// </summary>
        public string user;

        /// <summary>
        /// Domaine
        /// </summary>
        public string domain;

        /// <summary>
        /// Ressource
        /// </summary>
        public string resource;

        /// <summary>
        /// Identifiant sans sa ressource
        /// </summary>
        public string bare;

        /// <summary>
        /// Identifiant au complet
        /// </summary>
        public string full;

    }

	/// <summary>
	/// Nomination scindée en multiples parties
	/// </summary>
	public struct Name
	{

		/// <summary>
		/// Prénom
		/// </summary>
		public string firstname;

		/// <summary>
		/// Second prénom ou 2ème partie d'un prénom composé
		/// </summary>
		public string middle;

		/// <summary>
		/// Nom de famille
		/// </summary>
		public string lastname;

	}

	/// <summary>
	/// Entreprise scindée en multiples parties
	/// </summary>
	public struct Organization
	{

		/// <summary>
		/// Nom de l'entreprise
		/// </summary>
		public string name;

		/// <summary>
		/// Secteur d'activité
		/// </summary>
		public string unit;

	}

	/// <summary>
	/// Structure d'un email
	/// </summary>
	public struct Email
	{

		/// <summary>
		/// Catégorie de l'adresse email
		/// </summary>
		public Enums.EmailType type;

		/// <summary>
		/// Adresse email
		/// </summary>
		public string address;

	}

	/// <summary>
	/// Structure d'un favoris
	/// </summary>
	public struct Favorite
	{

		/// <summary>
		/// Nommination
		/// </summary>
		public string name;

		/// <summary>
		/// Adresse
		/// </summary>
		public string address;

	}

	/// <summary>
	/// Structure d'un numéro de téléphone
	/// </summary>
	public struct Telehone
	{

		/// <summary>
		/// Emplacement
		/// </summary>
		public Enums.LocationType location;

		/// <summary>
		/// Catégorie
		/// </summary>
		public Enums.PhoneType type;

		/// <summary>
		/// Numéro associé
		/// </summary>
		public string number;

	}
	
	/// <summary>
	/// Structure d'une adresse
	/// </summary>
	public struct Address
	{

		/// <summary>
		/// Emplacement
		/// </summary>
		public Enums.LocationType location;

		/// <summary>
		/// Numéro et nom de la rue
		/// </summary>
		public string street;

		/// <summary>
		/// Complément d'adresse
		/// </summary>
		public string extra;

		/// <summary>
		/// Code postal
		/// </summary>
		public string zipcode;

		/// <summary>
		/// Ville
		/// </summary>
		public string city;

		/// <summary>
		/// Région / Département
		/// </summary>
		public string region;

		/// <summary>
		/// Pays
		/// </summary>
		public string country;

	}

	/// <summary>
	/// Status d'une présence
	/// </summary>
	public struct Status
	{

		/// <summary>
		/// Type du status
		/// </summary>
		public Enums.StatusType type;

		/// <summary>
		/// Message
		/// </summary>
		public string message;

	}

	/// <summary>
	/// Service
	/// </summary>
	public struct Service
	{

		/// <summary>
		/// Identifiant Jabber du server
		/// </summary>
		public JabberID jabberID;

		/// <summary>
		/// Nom public du service
		/// </summary>
		public string name;

		/// <summary>
		/// Catégorie du service
		/// </summary>
		public string category;

		/// <summary>
		/// Liste des fonctionnalités liées au service
		/// </summary>
		public List<string> features;

	}

	/// <summary>
	/// Version d'un client logiciel
	/// </summary>
	public struct ClientVersion
	{

		/// <summary>
		/// Nom du logiciel client
		/// </summary>
		public string name;

		/// <summary>
		/// Version du système d'exploitation
		/// </summary>
		public string os;

		/// <summary>
		/// Version du logiciel client
		/// </summary>
		public string version;

	}

	/// <summary>
	/// Structure utilisée pour le passage par référnce de données de blocage
	/// </summary>
	public struct PrivacyStructure
	{

		/// <summary>
		/// Identifiant de la requete
		/// </summary>
		public string id;

		/// <summary>
		/// Identifiant Jabber incriminé
		/// </summary>
		public agsXMPP.Jid jid;

	}

	/// <summary>
	/// Structure d'un salon
	/// </summary>
	public struct Room
	{

		/// <summary>
		/// Rejoindre automatiquement le salon
		/// </summary>
		public bool autoJoin;

		/// <summary>
		/// Adresse du salon
		/// </summary>
		public JabberID jabberID;

		/// <summary>
		/// Nom du salon
		/// </summary>
		public string name;

		/// <summary>
		/// Nom d'utilisateur de connexion
		/// </summary>
		public string nickname;

		/// <summary>
		/// Mot de passe de connexion
		/// </summary>
		public string password;

	}

	/// <summary>
	/// Structure d'une humeur
	/// </summary>
	public struct Mood
	{

		/// <summary>
		/// Type de l'hummeur
		/// </summary>
		public Enums.MoodType type;

		/// <summary>
		/// Description
		/// </summary>
		public string text;

	}

	/// <summary>
	/// Structure d'une activité
	/// </summary>
	public struct Activity
	{

		/// <summary>
		/// Type de l'activité
		/// </summary>
		public Enums.ActivityType type;

		/// <summary>
		/// Description
		/// </summary>
		public string text;

	}

    /// <summary>
    /// Structure d'un emplacement géographique
    /// </summary>
    public struct Location
    {

        /// <summary>
        /// Altitude
        /// </summary>
        public double altitude;

        /// <summary>
        /// Emplacement complémentaire
        /// </summary>
        public string area;

        /// <summary>
        /// Décalage directionnel satellite
        /// </summary>
        public double bearing;

        /// <summary>
        /// Nom de l'immeuble
        /// </summary>
        public string building;

        /// <summary>
        /// Nom du pays
        /// </summary>
        public string country;

        /// <summary>
        /// GPS Datum
        /// </summary>
        public string datum;

        /// <summary>
        /// Description de l'emplacement
        /// </summary>
        public string description;

        /// <summary>
        /// Erreur GPS horizontale
        /// </summary>
        public double error;

        /// <summary>
        /// Nom ou numéro du niveau d'un immeuble
        /// </summary>
        public string floor;

        /// <summary>
        /// Latitude
        /// </summary>
        public double latitude;

        /// <summary>
        /// Ville (nom complet sans abréviations)
        /// </summary>
        public string locality;

        /// <summary>
        /// Longitude
        /// </summary>
        public double longitude;

        /// <summary>
        /// Code postal
        /// </summary>
        public string postalcode;

        /// <summary>
        /// Région ou département
        /// </summary>
        public string region;

        /// <summary>
        /// Emplacement à l'interieur d'un immeuble
        /// </summary>
        public string room;

        /// <summary>
        /// Vitesse de déplacement
        /// </summary>
        public double speed;

        /// <summary>
        /// Numéro et nom de la rue
        /// </summary>
        public string street;

        /// <summary>
        /// Complément d'adresse
        /// </summary>
        public string text;

        /// <summary>
        /// Temps universel en provenance du GPS
        /// </summary>
        public DateTime timestamp;

        /// <summary>
        /// URL pointant vers d'autres informations
        /// </summary>
        public string uri;

    }

	/// <summary>
	/// Structure d'une lecture en cours
	/// </summary>
	public struct Tune
	{

		/// <summary>
		/// Nom de l'artiste
		/// </summary>
		public string artist;

		/// <summary>
		/// Durée de la lecture en cours en secondes
		/// </summary>
		public int length;

		/// <summary>
		/// De 1 à 10, avis sur la lecture
		/// </summary>
		public int rating;

		/// <summary>
		/// Nom de l'album, de la playslist
		/// </summary>
		public string source;

		/// <summary>
		/// Titre du morceau
		/// </summary>
		public string title;

		/// <summary>
		/// Numéro de la piste
		/// </summary>
		public int track;

		/// <summary>
		/// Lien direct vers des informations sur la lecture
		/// </summary>
		public string uri;

	}

    /// <summary>
    /// Structure regroupant les Users trucs
    /// </summary>
    public struct UserPEP
    {

        /// <summary>
        /// Humeur
        /// </summary>
        public Mood mood;

        /// <summary>
        /// Activité
        /// </summary>
        public Activity activity;

        /// <summary>
        /// Emplacement géographique
        /// </summary>
        public Location location;

		/// <summary>
		/// Lecture en cours
		/// </summary>
		public Tune tune;

    }











}
