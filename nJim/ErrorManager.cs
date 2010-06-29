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
using System.Diagnostics;

namespace nJim {

    /// <summary>
    /// Gestion des différentes erreurs
    /// </summary>
    public class ErrorManager : IDisposable {

        #region Events

        /// <summary>
        /// Se produit lorsqu'une erreur se produit
        /// </summary>
        public event ErrorHandler ErrorRaised;
        private void OnErrorRaised(Enums.ErrorType type, string message) {
            try {
                ErrorRaised(type, message);
            } catch (Exception e) { Debug.WriteLine(e.ToString()); }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Constructeur
        /// </summary>
        public ErrorManager() {
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
        public void Dispose() {
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
        public void custom(Enums.ErrorType type, string message) {
            OnErrorRaised(type, message);
        }

        #endregion

        /// <summary>
        /// Exceptions
        /// </summary>
        /// <param name="sender">Objet parent</param>
        /// <param name="ex">Exception</param>
        private void Error(object sender, Exception ex) {
            OnErrorRaised(Enums.ErrorType.Client, ex.Message.Trim());
        }

        /// <summary>
        /// XMPP
        /// </summary>
        /// <param name="sender">Objet parent</param>
        /// <param name="el">Element</param>
        private void ElementError(object sender, agsXMPP.Xml.Dom.Element el) {
            if (el.HasTag("error", true)) {
                agsXMPP.Xml.Dom.Element e = el.SelectSingleElement("error", true);
                int code = 0;
                if (e.HasAttribute("code")) { code = Convert.ToInt32(e.Attribute("code")); }
                switch (code) {
                    case 302:
                        OnErrorRaised(Enums.ErrorType.Query, "Request redirected.");
                        break;
                    case 400:
                        OnErrorRaised(Enums.ErrorType.Query, "Bad Request.");
                        break;
                    case 401:
                        OnErrorRaised(Enums.ErrorType.Authentification, "Account not authorized.");
                        break;
                    case 402:
                        OnErrorRaised(Enums.ErrorType.Authentification, "Account incorrect.");
                        break;
                    case 403:
                        OnErrorRaised(Enums.ErrorType.Authentification, "Account denied.");
                        break;
                    case 404:
                        OnErrorRaised(Enums.ErrorType.Warning, "Request discontinued.");
                        break;
                    case 405:
                        OnErrorRaised(Enums.ErrorType.Warning, "Request prohibited.");
                        break;
                    case 406:
                        OnErrorRaised(Enums.ErrorType.Query, "Request not authorized.");
                        break;
                    case 407:
                        OnErrorRaised(Enums.ErrorType.Authentification, "Account not registered.");
                        break;
                    case 408:
                        OnErrorRaised(Enums.ErrorType.Server, "Server Timeout.");
                        break;
                    case 409:
                        OnErrorRaised(Enums.ErrorType.Warning, "Conflicting request.");
                        break;
                    case 500:
                        OnErrorRaised(Enums.ErrorType.Server, "Internal error.");
                        break;
                    case 501:
                        OnErrorRaised(Enums.ErrorType.Warning, "Request not implemented.");
                        break;
                    case 502:
                        OnErrorRaised(Enums.ErrorType.Server, "Distant error.");
                        break;
                    case 503:
                        OnErrorRaised(Enums.ErrorType.Warning, "Service temporarily not available.");
                        break;
                    case 504:
                        OnErrorRaised(Enums.ErrorType.Server, "Distant time limit reached.");
                        break;
                    case 510:
                        OnErrorRaised(Enums.ErrorType.Warning, "Disconnected.");
                        break;
                    default:
                        OnErrorRaised(Enums.ErrorType.Client, "General error.");
                        break;
                }
            } else {
                if (el.HasTag("bad-request", true)) {
                    OnErrorRaised(Enums.ErrorType.Query, "Request incorrect.");
                } else if (el.HasTag("conflict", true)) {
                    OnErrorRaised(Enums.ErrorType.Warning, "Conflicting request.");
                } else if (el.HasTag("feature-not-implemented", true)) {
                    OnErrorRaised(Enums.ErrorType.Warning, "Request not implemented.");
                } else if (el.HasTag("forbidden", true)) {
                    OnErrorRaised(Enums.ErrorType.Authentification, "Account not authorized.");
                } else if (el.HasTag("gone", true)) {
                    OnErrorRaised(Enums.ErrorType.Query, "Request redirected.");
                } else if (el.HasTag("internal-server-error", true)) {
                    OnErrorRaised(Enums.ErrorType.Server, "Internal error.");
                } else if (el.HasTag("item-not-found", true)) {
                    OnErrorRaised(Enums.ErrorType.Warning, "Request discontinued.");
                } else if (el.HasTag("jid-malformed", true)) {
                    OnErrorRaised(Enums.ErrorType.Query, "Jabber ID incorrect.");
                } else if (el.HasTag("not-acceptable", true)) {
                    OnErrorRaised(Enums.ErrorType.Query, "Request not accepted.");
                } else if (el.HasTag("not-allowed", true)) {
                    OnErrorRaised(Enums.ErrorType.Server, "Operation not allowed");
                } else if (el.HasTag("not-authorized", true)) {
                    OnErrorRaised(Enums.ErrorType.Query, "Request not authorized.");
                } else if (el.HasTag("payment-required", true)) {
                    OnErrorRaised(Enums.ErrorType.Query, "Finalisation requise.");
                } else if (el.HasTag("recipient-unavailable", true)) {
                    OnErrorRaised(Enums.ErrorType.Query, "Recipient unavailable.");
                } else if (el.HasTag("redirect", true)) {
                    OnErrorRaised(Enums.ErrorType.Query, "Request redirected.");
                } else if (el.HasTag("registration-required", true)) {
                    OnErrorRaised(Enums.ErrorType.Authentification, "Account not registered.");
                } else if (el.HasTag("remote-server-not-found", true)) {
                    OnErrorRaised(Enums.ErrorType.Server, "Distant error.");
                } else if (el.HasTag("remote-server-timeout", true)) {
                    OnErrorRaised(Enums.ErrorType.Server, "Server Timeout.");
                } else if (el.HasTag("resource-constraint", true)) {
                    OnErrorRaised(Enums.ErrorType.Authentification, "Resource invalid.");
                } else if (el.HasTag("service-unavailable", true)) {
                    OnErrorRaised(Enums.ErrorType.Warning, "Service temporary not available");
                } else if (el.HasTag("subscription-required", true)) {
                    OnErrorRaised(Enums.ErrorType.Client, "Abonnement required.");
                } else if (el.HasTag("undefined-condition", true)) {
                    OnErrorRaised(Enums.ErrorType.Client, "Condition undefined.");
                } else if (el.HasTag("unexpected-request", true)) {
                    OnErrorRaised(Enums.ErrorType.Query, "Bad Request.");
                }
            }
        }

    }

}
