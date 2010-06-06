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
    /// Les différentes énumérations pour conversion
    /// </summary>
    public class Enums
	{

		#region SocketConnectionType

		/// <summary>
        /// Différents types de connexions possibles
        /// </summary>
        public enum SocketConnectionType
        {

            /// <summary>
            /// Standart
            /// </summary>
            Normal,

            /// <summary>
            /// Méthode HTTP
            /// </summary>
            Http,

            /// <summary>
            /// Méthode HTTP par stream synchronsé
            /// </summary>
            Bosh

        }

        /// <summary>
        /// Convertit le type de connexion utilisé par nJim en type utilisé par agsXMPP
        /// </summary>
        /// <param name="type">Type de connexion</param>
		/// <returns>Type de connexion</returns>
        public static agsXMPP.net.SocketConnectionType SocketConnectionTypeConverter(SocketConnectionType type)
        {
            switch (type)
            {
                case SocketConnectionType.Http: return agsXMPP.net.SocketConnectionType.HttpPolling;
                case SocketConnectionType.Bosh: return agsXMPP.net.SocketConnectionType.Bosh;
                default: return agsXMPP.net.SocketConnectionType.Direct;
            }
        }

        /// <summary>
        /// Convertit le type de connexion utilisé par agsXMPP en type utilisé par nJim
        /// </summary>
		/// <param name="type">Type de connexion</param>
		/// <returns>Type de connexion</returns>
        public static SocketConnectionType SocketConnectionTypeConverter(agsXMPP.net.SocketConnectionType type)
        {
            switch (type)
            {
                case agsXMPP.net.SocketConnectionType.HttpPolling: return SocketConnectionType.Http;
                case agsXMPP.net.SocketConnectionType.Bosh: return SocketConnectionType.Bosh;
                default: return SocketConnectionType.Normal;
            }
		}

		#endregion

		#region EmailType

		/// <summary>
		/// Différent type d'email
		/// </summary>
		public enum EmailType
		{

			/// <summary>
			/// Type nul
			/// </summary>
			None,

			/// <summary>
			/// Personnel
			/// </summary>
			Home,

			/// <summary>
			/// Travail
			/// </summary>
			Work,

			/// <summary>
			/// Publique
			/// </summary>
			Internet,

			/// <summary>
			/// X400
			/// </summary>
			X400

		}

		/// <summary>
		/// Convertit le type d'email utilisé par la librairie en type utilisé par agsXMPP
		/// </summary>
		/// <param name="type">Type d'émail</param>
		/// <returns>Type d'émail</returns>
		public static agsXMPP.protocol.iq.vcard.EmailType EmailTypeConverter(EmailType type)
		{
			switch (type)
			{
				case EmailType.Home: return agsXMPP.protocol.iq.vcard.EmailType.HOME;
				case EmailType.Internet: return agsXMPP.protocol.iq.vcard.EmailType.INTERNET;
				case EmailType.Work: return agsXMPP.protocol.iq.vcard.EmailType.WORK;
				case EmailType.X400: return agsXMPP.protocol.iq.vcard.EmailType.X400;
				default: return agsXMPP.protocol.iq.vcard.EmailType.NONE;
			}
		}

		/// <summary>
		/// Convertit le type d'email utilisé par agsXMPP en type utilisé par la librairie
		/// </summary>
		/// <param name="type">Type d'émail</param>
		/// <returns>Type d'émail</returns>
		public static EmailType EmailTypeConverter(agsXMPP.protocol.iq.vcard.EmailType type)
		{
			switch (type)
			{
				case agsXMPP.protocol.iq.vcard.EmailType.HOME: return EmailType.Home;
				case agsXMPP.protocol.iq.vcard.EmailType.INTERNET: return EmailType.Internet;
				case agsXMPP.protocol.iq.vcard.EmailType.WORK: return EmailType.Work;
				case agsXMPP.protocol.iq.vcard.EmailType.X400: return EmailType.X400;
				default: return EmailType.None;
			}
		}

		#endregion

		#region LocationType

		/// <summary>
		/// Différents emplacements
		/// </summary>
		public enum LocationType
		{

			/// <summary>
			/// N'importe où
			/// </summary>
			None,

			/// <summary>
			/// A la maison
			/// </summary>
			Home,

			/// <summary>
			/// Au travail
			/// </summary>
			Work

		}

		/// <summary>
		/// Convertit l'emplacement utilisé par la librairie en type utilisé par agsXMPP
		/// </summary>
		/// <param name="type">Emplacement</param>
		/// <returns>Emplacement</returns>
		public static agsXMPP.protocol.iq.vcard.TelephoneLocation LocationTypeConverter(LocationType type)
		{
			switch (type)
			{
				case LocationType.Home: return agsXMPP.protocol.iq.vcard.TelephoneLocation.HOME;
				case LocationType.Work: return agsXMPP.protocol.iq.vcard.TelephoneLocation.WORK;
				default: return agsXMPP.protocol.iq.vcard.TelephoneLocation.NONE;
			}
		}

		/// <summary>
		/// Convertit l'emplacement utilisé par agsXMPP en type utilisé par la librairie
		/// </summary>
		/// <param name="type">Emplacement</param>
		/// <returns>Emplacement</returns>
		public static LocationType LocationTypeConverter(agsXMPP.protocol.iq.vcard.TelephoneLocation type)
		{
			switch(type)
			{
				case agsXMPP.protocol.iq.vcard.TelephoneLocation.HOME: return LocationType.Home;
				case agsXMPP.protocol.iq.vcard.TelephoneLocation.WORK: return LocationType.Work;
				default: return LocationType.None;
			}
		}

		/// <summary>
		/// Convertit l'emplacement utilisé par la librairie en type utilisé par agsXMPP
		/// </summary>
		/// <param name="type">Emplacement</param>
		/// <returns>Emplacement</returns>
		public static agsXMPP.protocol.iq.vcard.AddressLocation AddressLocationTypeConverter(LocationType type)
		{
			switch (type)
			{
				case LocationType.Home: return agsXMPP.protocol.iq.vcard.AddressLocation.HOME;
				case LocationType.Work: return agsXMPP.protocol.iq.vcard.AddressLocation.WORK;
				default: return agsXMPP.protocol.iq.vcard.AddressLocation.NONE;
			}
		}

		/// <summary>
		/// Convertit l'emplacement utilisé par agsXMPP en type utilisé par la librairie
		/// </summary>
		/// <param name="type">Emplacement</param>
		/// <returns>Emplacement</returns>
		public static LocationType AddressLocationTypeConverter(agsXMPP.protocol.iq.vcard.AddressLocation type)
		{
			switch (type)
			{
				case agsXMPP.protocol.iq.vcard.AddressLocation.HOME: return LocationType.Home;
				case agsXMPP.protocol.iq.vcard.AddressLocation.WORK: return LocationType.Work;
				default: return LocationType.None;
			}
		}

		#endregion

		#region PhoneType

		/// <summary>
		/// Différent type de numéros de téléphone
		/// </summary>
		public enum PhoneType
		{

			/// <summary>
			/// BBS
			/// </summary>
			Bbs,

			/// <summary>
			/// Mobile
			/// </summary>
			Cell,

			/// <summary>
			/// Fax
			/// </summary>
			Fax,

			/// <summary>
			/// Ligne de données haute vitesse
			/// </summary>
			Isdn,

			/// <summary>
			/// Ligne de données basse vitesse
			/// </summary>
			Modem,

			/// <summary>
			/// Messagerie
			/// </summary>
			Msg,

			/// <summary>
			/// Type nul
			/// </summary>
			None,

			/// <summary>
			/// Numéro principal
			/// </summary>
			Number,

			/// <summary>
			/// Pager, Messager
			/// </summary>
			Pager,

			/// <summary>
			/// PCS
			/// </summary>
			Pcs,

			/// <summary>
			/// Numéro préferentiel
			/// </summary>
			Pref,

			/// <summary>
			/// Ligne numérique vidéo
			/// </summary>
			Video,

			/// <summary>
			/// Ligne numérique vocale
			/// </summary>
			Voice

		}

		/// <summary>
		/// Convertit le type utilisé par la librairie en type utilisé par agsXMPP
		/// </summary>
		/// <param name="type">Type de numéro</param>
		/// <returns>Type de numéro</returns>
		public static agsXMPP.protocol.iq.vcard.TelephoneType PhoneTypeConverter(PhoneType type)
		{
			switch (type)
			{
				case PhoneType.Bbs: return agsXMPP.protocol.iq.vcard.TelephoneType.BBS;
				case PhoneType.Cell: return agsXMPP.protocol.iq.vcard.TelephoneType.CELL;
				case PhoneType.Fax: return agsXMPP.protocol.iq.vcard.TelephoneType.FAX;
				case PhoneType.Isdn: return agsXMPP.protocol.iq.vcard.TelephoneType.ISDN;
				case PhoneType.Modem: return agsXMPP.protocol.iq.vcard.TelephoneType.MODEM;
				case PhoneType.Msg: return agsXMPP.protocol.iq.vcard.TelephoneType.MSG;
				case PhoneType.Number: return agsXMPP.protocol.iq.vcard.TelephoneType.NUMBER;
				case PhoneType.Pager: return agsXMPP.protocol.iq.vcard.TelephoneType.PAGER;
				case PhoneType.Pcs: return agsXMPP.protocol.iq.vcard.TelephoneType.PCS;
				case PhoneType.Pref: return agsXMPP.protocol.iq.vcard.TelephoneType.PREF;
				case PhoneType.Video: return agsXMPP.protocol.iq.vcard.TelephoneType.VIDEO;
				case PhoneType.Voice: return agsXMPP.protocol.iq.vcard.TelephoneType.VOICE;
				default: return agsXMPP.protocol.iq.vcard.TelephoneType.NONE;
			}
		}

		/// <summary>
		/// Convertit le type utilisé par agsXMPP en type utilisé par la librairie
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public static PhoneType PhoneTypeConverter(agsXMPP.protocol.iq.vcard.TelephoneType type)
		{
			switch (type)
			{
				case agsXMPP.protocol.iq.vcard.TelephoneType.BBS: return PhoneType.Bbs;
				case agsXMPP.protocol.iq.vcard.TelephoneType.CELL: return PhoneType.Cell;
				case agsXMPP.protocol.iq.vcard.TelephoneType.FAX: return PhoneType.Fax;
				case agsXMPP.protocol.iq.vcard.TelephoneType.ISDN: return PhoneType.Isdn;
				case agsXMPP.protocol.iq.vcard.TelephoneType.MODEM: return PhoneType.Modem;
				case agsXMPP.protocol.iq.vcard.TelephoneType.MSG: return PhoneType.Msg;
				case agsXMPP.protocol.iq.vcard.TelephoneType.NUMBER: return PhoneType.Number;
				case agsXMPP.protocol.iq.vcard.TelephoneType.PAGER: return PhoneType.Pager;
				case agsXMPP.protocol.iq.vcard.TelephoneType.PCS: return PhoneType.Pcs;
				case agsXMPP.protocol.iq.vcard.TelephoneType.PREF: return PhoneType.Pref;
				case agsXMPP.protocol.iq.vcard.TelephoneType.VIDEO: return PhoneType.Video;
				case agsXMPP.protocol.iq.vcard.TelephoneType.VOICE: return PhoneType.Voice;
				default: return PhoneType.None;
			}
		}

		#endregion

		#region ErrorType

		/// <summary>
		/// Différents types d'erreur
		/// </summary>
		public enum ErrorType
		{

			/// <summary>
			/// Le client provoque l'erreur
			/// </summary>
			Client,

			/// <summary>
			/// Le serveur provoque l'erreur
			/// </summary>
			Server,

			/// <summary>
			/// La requete est incorrecte
			/// </summary>
			Query,

			/// <summary>
			/// Erreur lors d'un transfert d'identité
			/// </summary>
			Authentification,

			/// <summary>
			/// Erreur importante
			/// </summary>
			Warning

		}

		#endregion

		#region StatusType

		/// <summary>
		/// LEs différents type de status
		/// </summary>
		public enum StatusType
		{

			/// <summary>
			/// Disponible
			/// </summary>
			Normal,

			/// <summary>
			/// Prêt à discuter
			/// </summary>
			ReadyToChat,

			/// <summary>
			/// Absent
			/// </summary>
			Away,

			/// <summary>
			/// Absent pour longtemps
			/// </summary>
			ExtendedAway,

			/// <summary>
			/// Ne pas déranger
			/// </summary>
			DoNotDisturb,

			/// <summary>
			/// Indisponible
			/// </summary>
			Unvailable,

			/// <summary>
			/// Invisible
			/// </summary>
			Invisible

		}

		/// <summary>
		/// Convertit le type de status utilisé par la librairie en type de status utilisé par la librairie agsXMPP
		/// </summary>
		/// <param name="type">Type de status</param>
		/// <returns></returns>
		public static agsXMPP.protocol.client.ShowType StatusTypeConverter(StatusType type)
		{
			switch (type)
			{
				case StatusType.Away: return agsXMPP.protocol.client.ShowType.away;
				case StatusType.DoNotDisturb: return agsXMPP.protocol.client.ShowType.dnd;
				case StatusType.ExtendedAway: return agsXMPP.protocol.client.ShowType.xa;
				case StatusType.ReadyToChat: return agsXMPP.protocol.client.ShowType.chat;
				default: return agsXMPP.protocol.client.ShowType.NONE;
			}
		}

		/// <summary>
		/// Convertit le type de status utilisé par la librairie agsXMPP en type de status utilisé par la librairie
		/// </summary>
		/// <param name="type">Type de status</param>
		/// <returns></returns>
		public static StatusType StatusTypeConverter(agsXMPP.protocol.client.ShowType type)
		{
			switch (type)
			{
				case agsXMPP.protocol.client.ShowType.away: return StatusType.Away;
				case agsXMPP.protocol.client.ShowType.dnd: return StatusType.DoNotDisturb;
				case agsXMPP.protocol.client.ShowType.xa: return StatusType.ExtendedAway;
				case agsXMPP.protocol.client.ShowType.chat: return StatusType.ReadyToChat;
				default: return StatusType.Normal;
			}
		}

		/// <summary>
		/// Convertit le type de presence utilisé par la librairie agsXMPP en type de presence utilisé par la librairie
		/// </summary>
		/// <param name="type">Type de presence</param>
		/// <returns></returns>
		public static agsXMPP.protocol.client.PresenceType PresenceTypeConverter(StatusType type)
		{
			switch (type)
			{
				case StatusType.Unvailable: return agsXMPP.protocol.client.PresenceType.unavailable;
				case StatusType.Invisible: return agsXMPP.protocol.client.PresenceType.invisible;
				default: return agsXMPP.protocol.client.PresenceType.available;
			}
		}

		/// <summary>
		/// Convertit le type de presence utilisé par la librairie agsXMPP en type de presence utilisé par la librairie
		/// </summary>
		/// <param name="type">Type de presence</param>
		/// <returns></returns>
		public static StatusType PresenceTypeConverter(agsXMPP.protocol.client.PresenceType type)
		{
			switch (type)
			{
				case agsXMPP.protocol.client.PresenceType.unavailable: return StatusType.Unvailable;
				case agsXMPP.protocol.client.PresenceType.invisible: return StatusType.Invisible;
				default: return StatusType.Normal;
			}
		}

		#endregion

		#region SubscribtionType

		/// <summary>
		/// Différents types d'abonnements
		/// </summary>
		public enum SubscribtionType
		{

			/// <summary>
			/// Aucun abonnement
			/// </summary>
			None,

			/// <summary>
			/// Il est abonné
			/// </summary>
			From,

			/// <summary>
			/// Vous êtes abonné
			/// </summary>
			To,

			/// <summary>
			/// Tout le monde est abonné
			/// </summary>
			Both

		}

		/// <summary>
		/// Convertit les types d'abonnements
		/// </summary>
		/// <param name="type">Type d'abonnement</param>
		/// <returns>Type d'abonnement</returns>
		public static agsXMPP.protocol.iq.roster.SubscriptionType SubscribtionTypeConverter(SubscribtionType type)
		{
			switch (type)
			{
				case SubscribtionType.Both: return agsXMPP.protocol.iq.roster.SubscriptionType.both;
				case SubscribtionType.From: return agsXMPP.protocol.iq.roster.SubscriptionType.from;
				case SubscribtionType.To: return agsXMPP.protocol.iq.roster.SubscriptionType.to;
				default: return agsXMPP.protocol.iq.roster.SubscriptionType.none;
			}
		}

		/// <summary>
		/// Convertit les types d'abonnements
		/// </summary>
		/// <param name="type">Type d'abonnement</param>
		/// <returns>Type d'abonnement</returns>
		public static SubscribtionType SubscribtionTypeConverter(agsXMPP.protocol.iq.roster.SubscriptionType type)
		{
			switch (type)
			{
				case agsXMPP.protocol.iq.roster.SubscriptionType.both: return SubscribtionType.Both;
				case agsXMPP.protocol.iq.roster.SubscriptionType.from: return SubscribtionType.From;
				case agsXMPP.protocol.iq.roster.SubscriptionType.to: return SubscribtionType.To;
				default: return SubscribtionType.None;
			}
		}

		#endregion

		#region MoodType

		/// <summary>
		/// Les différents types d'hummeur
		/// </summary>
		public enum MoodType
		{

			/// <summary>
			/// Effrayé
			/// </summary>
			afraid,

			/// <summary>
			/// Stupéfait
			/// </summary>
			amazed,

			/// <summary>
			/// En colère
			/// </summary>
			angry,

			/// <summary>
			/// Agacé
			/// </summary>
			annoyed,

			/// <summary>
			/// Anxieux
			/// </summary>
			anxious,

			/// <summary>
			/// Éveillé
			/// </summary>
			aroused,

			/// <summary>
			/// Honte
			/// </summary>
			ashamed,

			/// <summary>
			/// S'ennuyer
			/// </summary>
			bored,

			/// <summary>
			/// Courageux
			/// </summary>
			brave,

			/// <summary>
			/// Calme
			/// </summary>
			calm,

			/// <summary>
			/// Froid
			/// </summary>
			cold,

			/// <summary>
			/// Confondue
			/// </summary>
			confused,

			/// <summary>
			/// Contented
			/// </summary>
			contented,

			/// <summary>
			/// Grincheux
			/// </summary>
			cranky,

			/// <summary>
			/// Curieux
			/// </summary>
			curious,

			/// <summary>
			/// Déprimé
			/// </summary>
			depressed,

			/// <summary>
			/// Déçu
			/// </summary>
			disappointed,

			/// <summary>
			/// Dégoûté
			/// </summary>
			disgusted,
			
			/// <summary>
			/// Distrait
			/// </summary>
			distracted,

			/// <summary>
			/// Embarrassé
			/// </summary>
			embarrassed,

			/// <summary>
			/// Excités
			/// </summary>
			excited,

			/// <summary>
			/// Charmeur
			/// </summary>
			flirtatious,

			/// <summary>
			/// Frustré
			/// </summary>
			frustrated,

			/// <summary>
			/// Grumpy
			/// </summary>
			grumpy,

			/// <summary>
			/// Coupable
			/// </summary>
			guilty,

			/// <summary>
			/// Heureux
			/// </summary>
			happy,

			/// <summary>
			/// Chaud
			/// </summary>
			hot,

			/// <summary>
			/// Humilié
			/// </summary>
			humbled,

			/// <summary>
			/// Humiliée
			/// </summary>
			humiliated,

			/// <summary>
			/// Affamés
			/// </summary>
			hungry,

			/// <summary>
			/// Hurt
			/// </summary>
			hurt,

			/// <summary>
			/// Impressionné
			/// </summary>
			impressed,

			/// <summary>
			/// Dans la crainte
			/// </summary>
			in_awe,

			/// <summary>
			/// Dans l'amour
			/// </summary>
			in_love,

			/// <summary>
			/// Indignés
			/// </summary>
			indignant,

			/// <summary>
			/// Intéressés
			/// </summary>
			interested,

			/// <summary>
			/// Intoxiqués
			/// </summary>
			intoxicated,

			/// <summary>
			/// Invincible
			/// </summary>
			invincible,

			/// <summary>
			/// Jaloux
			/// </summary>
			jealous,

			/// <summary>
			/// Lonely
			/// </summary>
			lonely,

			/// <summary>
			/// Signifie
			/// </summary>
			mean,

			/// <summary>
			/// Moody
			/// </summary>
			moody,

			/// <summary>
			/// Nerveux
			/// </summary>
			nervous,

			/// <summary>
			/// Neutre
			/// </summary>
			neutral,

			/// <summary>
			/// Aucune humeur
			/// </summary>
			none,

			/// <summary>
			/// Offensée
			/// </summary>
			offended,

			/// <summary>
			/// Ludique
			/// </summary>
			playful,

			/// <summary>
			/// Fière
			/// </summary>
			proud,

			/// <summary>
			/// Soulagée
			/// </summary>
			relieved,

			/// <summary>
			/// Remords
			/// </summary>
			remorseful,

			/// <summary>
			/// Restless
			/// </summary>
			restless,

			/// <summary>
			/// Triste
			/// </summary>
			sad,

			/// <summary>
			/// Sarcastique
			/// </summary>
			sarcastic,

			/// <summary>
			/// Grave
			/// </summary>
			serious,

			/// <summary>
			/// Choqué
			/// </summary>
			shocked,

			/// <summary>
			/// Timide
			/// </summary>
			shy,

			/// <summary>
			/// Malade
			/// </summary>
			sick,

			/// <summary>
			/// Dormeur
			/// </summary>
			sleepy,

			/// <summary>
			/// Souligné
			/// </summary>
			stressed,

			/// <summary>
			/// Surpris
			/// </summary>
			surprised,

			/// <summary>
			/// Soif
			/// </summary>
			thirsty,

			/// <summary>
			/// Inquiet
			/// </summary>
			worried
		}

		#endregion

		#region ActivityType

		/// <summary>
		/// LEs différentes types d'activités
		/// </summary>
		public enum ActivityType
		{
           
			/// <summary>
			/// Aucun activité
			/// </summary>
			none,

			/// <summary>
			/// Faire des corvées
			/// </summary>
			doing_chores,

			/// <summary>
			/// L'achat d'épicerie
			/// </summary>
			buying_groceries,

			/// <summary>
			/// Nettoyage
			/// </summary>
			cleaning,

			/// <summary>
			/// Cuisine
			/// </summary>
			cooking,

			/// <summary>
			/// Faire l'entretien
			/// </summary>
			doing_maintenance,

			/// <summary>
			/// Faire la vaisselle
			/// </summary>
			doing_the_dishes,

			/// <summary>
			/// Fait la lessive
			/// </summary>
			doing_the_laundry,

			/// <summary>
			/// Jardinage
			/// </summary>
			gardening,

			/// <summary>
			/// Courir une course
			/// </summary>
			running_an_errand,

			/// <summary>
			/// Promener le chien
			/// </summary>
			walking_the_dog,

			/// <summary>
			/// Potable
			/// </summary>
			drinking,

			/// <summary>
			/// Avoir une bière
			/// </summary>
			having_a_beer,

			/// <summary>
			/// Ayant café
			/// </summary>
			having_coffee,

			/// <summary>
			/// Ayant thé
			/// </summary>
			having_tea,

			/// <summary>
			/// Manger
			/// </summary>
			eating,

			/// <summary>
			/// Avoir une collation
			/// </summary>
			having_a_snack,

			/// <summary>
			/// Petit déjeuner
			/// </summary>
			having_breakfast,

			/// <summary>
			/// Dînais
			/// </summary>
			having_dinner,

			/// <summary>
			/// Son déjeuner
			/// </summary>
			having_lunch,

			/// <summary>
			/// Exerçant
			/// </summary>
			exercising,

			/// <summary>
			/// Cyclisme
			/// </summary>
			cycling,

			/// <summary>
			/// Randonnée
			/// </summary>
			hiking,

			/// <summary>
			/// Jogging
			/// </summary>
			jogging,

			/// <summary>
			/// Sport
			/// </summary>
			playing_sports,

			/// <summary>
			/// Marche
			/// </summary>
			running,

			/// <summary>
			/// Ski
			/// </summary>
			skiing,

			/// <summary>
			/// Natation
			/// </summary>
			swimming,

			/// <summary>
			/// Élaboration
			/// </summary>
			working_out,

			/// <summary>
			/// Toilettage
			/// </summary>
			grooming,

			/// <summary>
			/// Au Spa
			/// </summary>
			at_the_spa,

			/// <summary>
			/// Se brosser les dents
			/// </summary>
			brushing_teeth,

			/// <summary>
			/// Obtenir une décote
			/// </summary>
			getting_a_haircut,

			/// <summary>
			/// Rasage
			/// </summary>
			shaving,

			/// <summary>
			/// Prendre un bain
			/// </summary>
			taking_a_bath,

			/// <summary>
			/// Prendre une douche
			/// </summary>
			taking_a_shower,

			/// <summary>
			/// Avoir rendez-vous
			/// </summary>
			having_appointment,

			/// <summary>
			/// Inactif
			/// </summary>
			inactive,

			/// <summary>
			/// Jour férié
			/// </summary>
			day_off,

			/// <summary>
			/// Hanging out
			/// </summary>
			hanging_out,

			/// <summary>
			/// En vacances
			/// </summary>
			on_vacation,

			/// <summary>
			/// Prévue vacances
			/// </summary>
			scheduled_holiday,

			/// <summary>
			/// Dormant
			/// </summary>
			sleeping,

			/// <summary>
			/// Relaxant
			/// </summary>
			relaxing,

			/// <summary>
			/// Jeu
			/// </summary>
			gaming,

			/// <summary>
			/// Sorties
			/// </summary>
			going_out,

			/// <summary>
			/// La fête
			/// </summary>
			partying,

			/// <summary>
			/// Lecture
			/// </summary>
			reading,

			/// <summary>
			/// Répétitions
			/// </summary>
			rehearsing,

			/// <summary>
			/// Shopping
			/// </summary>
			shopping,

			/// <summary>
			/// Socialisant
			/// </summary>
			socializing,

			/// <summary>
			/// Bronzer
			/// </summary>
			sunbathing,

			/// <summary>
			/// Regarder la télévision
			/// </summary>
			watching_tv,

			/// <summary>
			/// Regarder un film
			/// </summary>
			watching_a_movie,

			/// <summary>
			/// Parler
			/// </summary>
			talking,

			/// <summary>
			/// Dans la vraie vie
			/// </summary>
			in_real_life,

			/// <summary>
			/// Sur le téléphone
			/// </summary>
			on_the_phone,

			/// <summary>
			/// Vidéo sur téléphone
			/// </summary>
			on_video_phone,

			/// <summary>
			/// Voyage
			/// </summary>
			traveling,

			/// <summary>
			/// Commuer
			/// </summary>
			commuting,

			/// <summary>
			/// Conduite
			/// </summary>
			driving,

			/// <summary>
			/// Dans une voiture
			/// </summary>
			in_a_car,

			/// <summary>
			/// Dans un bus
			/// </summary>
			on_a_bus,

			/// <summary>
			/// Dans un avion
			/// </summary>
			on_a_plane,

			/// <summary>
			/// Dans le train
			/// </summary>
			on_a_train,

			/// <summary>
			/// Au cours d'un voyage
			/// </summary>
			on_a_trip,

			/// <summary>
			/// Marche
			/// </summary>
			walking,

			/// <summary>
			/// Travail
			/// </summary>
			working,

			/// <summary>
			/// Codage
			/// </summary>
			coding,

			/// <summary>
			/// À une réunion
			/// </summary>
			in_a_meeting,

			/// <summary>
			/// Étudiant
			/// </summary>
			studying,

			/// <summary>
			/// Écrit
			/// </summary>
			writing


		}

		#endregion







	}

}
