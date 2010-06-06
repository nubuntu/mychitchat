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
using System.Text.RegularExpressions;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Security.Cryptography;
using System.Xml;
using System.Reflection;
using System.Drawing.Drawing2D;

namespace nJim {

    /// <summary>
    /// Informations d'identité
    /// </summary>
    public class Identity : IDisposable {

        #region Events

        /// <summary>
        /// Se produit lorsque la fiche d'identité a été modifiée
        /// </summary>
        public event IdentityHandler identityRetrieved;
        private void onIdentityRetrieved() {
            try {
                identityRetrieved(this);
            } catch { }
        }

        /// <summary>
        /// Se prodit lorsque la fiche d'identité a été publiée
        /// </summary>
        public event IdentityHandler identityPublished;
        private void onIdentityPublished() {
            try {
                identityPublished(this);
            } catch { }
        }

        /// <summary>
        /// Se produit lorsque le pseudonyme est modifié
        /// </summary>
        public event IdentityHandler NicknameUpdated;
        private void OnNicknameUpdated() {
            try {
                NicknameUpdated(this);
            } catch { }
        }

        #endregion

        #region Properties

        private JabberID _jabberID;
        /// <summary>
        /// Identifiant Jabber accessible en lecture seule
        /// </summary>
        public JabberID jabberID {
            get { return _jabberID; }
        }

        private DateTime _birthday = DateTime.Now;
        /// <summary>
        /// Date de naissance
        /// </summary>
        public DateTime birthday {
            get { return _birthday; }
            set {
                // si c'est la fiche d'identité d'un contact, alors on ne peut pas la modifier
                if (Jabber.xmpp.MyJID.ToString() != jabberID.full) {
                    return;
                }
                _birthday = value;
            }
        }

        private string _description = string.Empty;
        /// <summary>
        /// Description
        /// </summary>
        public string description {
            get { return _description; }
            set {
                // si c'est la fiche d'identité d'un contact, alors on ne peut pas la modifier
                if (Jabber.xmpp.MyJID.ToString() != jabberID.full) {
                    return;
                }
                _description = value;
            }
        }

        private string _fullname = string.Empty;
        /// <summary>
        /// Nom complet pour l'affichage
        /// </summary>
        public string fullname {
            get { return _fullname; }
            set {
                // si c'est la fiche d'identité d'un contact, alors on ne peut pas la modifier
                if (Jabber.xmpp.MyJID.ToString() != jabberID.full) {
                    return;
                }
                _fullname = value;
            }
        }

        private Name _name;
        /// <summary>
        /// Nom composé
        /// </summary>
        public Name name {
            get { return _name; }
            set {
                // si c'est la fiche d'identité d'un contact, alors on ne peut pas la modifier
                if (Jabber.xmpp.MyJID.ToString() != jabberID.full) {
                    return;
                }
                _name = value;
            }
        }

        private string _nickname = string.Empty;
        /// <summary>
        /// Nom d'utilisateur
        /// </summary>
        public string nickname {
            get {
                if (_nickname.Trim() != string.Empty) { return _nickname; }
                if (_fullname.Trim() != string.Empty) { return _fullname; }
                return jabberID.user;
            }
            set {
                if (value != _nickname) {
                    _nickname = value;
                    if (Jabber.xmpp.Authenticated) {
                        if (Jabber.xmpp.MyJID.ToString() != jabberID.full) {
                            // si c'est la fiche d'un contact on change le compte du contact
                            Jabber.xmpp.RosterManager.UpdateRosterItem(new agsXMPP.Jid(jabberID.full), _nickname);
                            OnNicknameUpdated();
                        } else {
                            // si c'est notre pseudo alors on publie une notification
                            if (Jabber.xmpp.Authenticated && Jabber._pepCapable) {
                                agsXMPP.protocol.client.IQ iq = new agsXMPP.protocol.client.IQ();
                                iq.Type = agsXMPP.protocol.client.IqType.set;
                                iq.GenerateId();
                                agsXMPP.Xml.Dom.Element pb = new agsXMPP.Xml.Dom.Element("pubsub");
                                pb.Namespace = "http://jabber.org/protocol/pubsub";
                                agsXMPP.Xml.Dom.Element ps = new agsXMPP.Xml.Dom.Element("publish");
                                ps.Attributes.Add("node", "http://jabber.org/protocol/nick");
                                agsXMPP.Xml.Dom.Element item = new agsXMPP.Xml.Dom.Element("item");
                                item.Attributes.Add("id", "current");
                                agsXMPP.Xml.Dom.Element nick = new agsXMPP.Xml.Dom.Element("nick");
                                nick.Namespace = "http://jabber.org/protocol/nick";
                                nick.Value = nickname;
                                item.AddChild(nick);
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
                            }
                        }
                    }
                }
            }
        }

        private Organization _organization;
        /// <summary>
        /// Entreprise
        /// </summary>
        public Organization organization {
            get { return _organization; }
            set {
                // si c'est la fiche d'identité d'un contact, alors on ne peut pas la modifier
                if (Jabber.xmpp.MyJID.ToString() != jabberID.full) {
                    return;
                }
                _organization = value;
            }
        }

        private string _role = string.Empty;
        /// <summary>
        /// Rôle
        /// </summary>
        public string role {
            get { return _role; }
            set {
                // si c'est la fiche d'identité d'un contact, alors on ne peut pas la modifier
                if (Jabber.xmpp.MyJID.ToString() != jabberID.full) {
                    return;
                }
                _role = value;
            }
        }

        private string _title = string.Empty;
        /// <summary>
        /// Titre
        /// </summary>
        public string title {
            get { return _title; }
            set {
                // si c'est la fiche d'identité d'un contact, alors on ne peut pas la modifier
                if (Jabber.xmpp.MyJID.ToString() != jabberID.full) {
                    return;
                }
                _title = value;
            }
        }

        private string _url = string.Empty;
        /// <summary>
        /// Site Internet
        /// </summary>
        public string url {
            get { return _url; }
            set {
                // si c'est la fiche d'identité d'un contact, alors on ne peut pas la modifier
                if (Jabber.xmpp.MyJID.ToString() != jabberID.full) {
                    return;
                }
                _url = value;
            }
        }

        private List<Email> _email = new List<Email>();
        /// <summary>
        /// Liste des adresses emails
        /// </summary>
        public List<Email> email {
            get { return _email; }
            set {
                // si c'est la fiche d'identité d'un contact, alors on ne peut pas la modifier
                if (Jabber.xmpp.MyJID.ToString() != jabberID.full) {
                    return;
                }
                _email = value;
            }
        }

        private List<Telehone> _telephone = new List<Telehone>();
        /// <summary>
        /// Liste des numéros de téléphone
        /// </summary>
        public List<Telehone> telephone {
            get { return _telephone; }
            set {
                // si c'est la fiche d'identité d'un contact, alors on ne peut pas la modifier
                if (Jabber.xmpp.MyJID.ToString() != jabberID.full) {
                    return;
                }
                _telephone = value;
            }
        }

        private List<Address> _address = new List<Address>();
        /// <summary>
        /// Liste des adresses postales
        /// </summary>
        public List<Address> address {
            get { return _address; }
            set {
                // si c'est la fiche d'identité d'un contact, alors on ne peut pas la modifier
                if (Jabber.xmpp.MyJID.ToString() != jabberID.full) {
                    return;
                }
                _address = value;
            }
        }

        private Image _photo;
        /// <summary>
        /// Avatar
        /// </summary>
        public Image photo {
            get { return _photo; }
            set {
                // si c'est la fiche d'identité d'un contact, alors on ne peut pas la modifier
                if (Jabber.xmpp.MyJID.ToString() != jabberID.full) {
                    return;
                }
                // Formats acceptés: BMP, PNG, GIG, JPG, et ICON
                if (value == null) {
                    _photo = null;
                }else if (value.RawFormat.ToString().Contains(ImageFormat.Bmp.Guid.ToString()) ||
                    value.RawFormat.ToString().Contains(ImageFormat.Gif.Guid.ToString()) ||
                    value.RawFormat.ToString().Contains(ImageFormat.Icon.Guid.ToString()) ||
                    value.RawFormat.ToString().Contains(ImageFormat.Jpeg.Guid.ToString()) ||
                    value.RawFormat.ToString().Contains(ImageFormat.Png.Guid.ToString())) {
                    _photo = value;
                }
                _photoHash = string.Empty;
                if (_photo != null && photoFormat != null) {
                    if (_photo.Width > 96 || _photo.Height > 96) {
                        int sourceWidth = _photo.Width;
                        int sourceHeight = _photo.Height;
                        float nPercent = 0;
                        float nPercentW = 0;
                        float nPercentH = 0;
                        nPercentW = (96.0f / (float)sourceWidth);
                        nPercentH = (96.0f / (float)sourceHeight);
                        if (nPercentH < nPercentW) {
                            nPercent = nPercentH;
                        } else {
                            nPercent = nPercentW;
                        }
                        int destWidth = (int)(sourceWidth * nPercent);
                        int destHeight = (int)(sourceHeight * nPercent);
                        Bitmap b = new Bitmap(destWidth, destHeight);
                        Graphics g = Graphics.FromImage((Image)b);
                        g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                        g.DrawImage(_photo, 0, 0, destWidth, destHeight);
                        g.Dispose();
                        _photo = (Image)b;
                    }
                    MemoryStream ms = new MemoryStream();
                    _photo.Save(ms, photoFormat);
                    byte[] hash = SHA1.Create().ComputeHash(ms.ToArray());
                    ms.Close();
                    ms.Dispose();
                    string hashString = string.Empty;
                    foreach (byte b in hash) {
                        string hex = b.ToString("X").ToLower();
                        hex = ((hex.Length == 1) ? "0" : string.Empty) + hex;
                        hashString += hex;
                    }
                    hash = null;
                    _photoHash = hashString;
                    hashString = string.Empty;
                }
                if (Jabber.xmpp.Authenticated) {
                    agsXMPP.protocol.client.Presence presence = new agsXMPP.protocol.client.Presence();
                    presence.From = new agsXMPP.Jid(jabberID.full);
                    presence.Priority = Jabber.xmpp.Priority;
                    presence.To = new agsXMPP.Jid(Jabber.xmpp.Server);
                    presence.XDelay = new agsXMPP.protocol.x.Delay();
                    presence.XDelay.Stamp = DateTime.Now;
                    agsXMPP.protocol.x.Avatar avatar = new agsXMPP.protocol.x.Avatar();
                    avatar.Hash = photoHash;
                    presence.AddChild(avatar);
                    Jabber.xmpp.Send(presence);
                }
            }
        }

        /// <summary>
        /// Format de l'image Avatar
        /// </summary>
        public ImageFormat photoFormat {
            get {
                if (_photo == null) { return null; }
                if (_photo.RawFormat.ToString().Contains(ImageFormat.Bmp.Guid.ToString())) { return ImageFormat.Bmp; } else if (_photo.RawFormat.ToString().Contains(ImageFormat.Gif.Guid.ToString())) { return ImageFormat.Gif; } else if (_photo.RawFormat.ToString().Contains(ImageFormat.Icon.Guid.ToString())) { return ImageFormat.Icon; } else if (_photo.RawFormat.ToString().Contains(ImageFormat.Jpeg.Guid.ToString())) { return ImageFormat.Jpeg; } else if (_photo.RawFormat.ToString().Contains(ImageFormat.Png.Guid.ToString())) { return ImageFormat.Png; } else { return null; }
            }
        }

        private string _photoHash = string.Empty;
        /// <summary>
        /// Hashage SHA1 de l'avatar
        /// </summary>
        public string photoHash {
            get { return _photoHash; }
            set {
                // si le hash est renseigné directement, on le compare à l'ancien, s'ils sont différents alors
                // l'avatar a du être modifié! on recupère donc la nouvelle fiche d'identité.
                if (value != _photoHash) {
                    retrieve();
                }
            }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="jid">Identifiant Jabber</param>
        public Identity(agsXMPP.Jid jid) {
            if (jid == null) { return; }
            _jabberID = new JabberID();
            _jabberID.user = (jid.User != null) ? jid.User : string.Empty;
            _jabberID.domain = (jid.Server != null) ? jid.Server : string.Empty;
            _jabberID.resource = (jid.Resource != null) ? jid.Resource : string.Empty;
            _jabberID.bare = (jid.Bare != null) ? jid.Bare : string.Empty;
            _jabberID.full = jid.ToString();
            _name = new Name();
            _name.firstname = string.Empty;
            _name.lastname = string.Empty;
            _name.middle = string.Empty;
            _organization = new Organization();
            _organization.name = string.Empty;
            _organization.unit = string.Empty;
            load();
            Jabber.xmpp.OnIq += new agsXMPP.protocol.client.IqHandler(iqManager);
        }

        #endregion

        #region Destructor

        /// <summary>
        /// Destructeur
        /// </summary>
        public void Dispose() {
            Jabber.xmpp.OnIq -= new agsXMPP.protocol.client.IqHandler(iqManager);
            //save();
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Demande d'identité
        /// </summary>
        /// <returns>vrai si la demande est effectuée, sinon faux</returns>
        public bool retrieve() {
            // si il n'y a pas de stream XMPP en cours, on s'arrete là.
            if (!Jabber.xmpp.Authenticated) {
                return false;
            }
            agsXMPP.protocol.iq.vcard.VcardIq viq = new agsXMPP.protocol.iq.vcard.VcardIq();
            viq.Type = agsXMPP.protocol.client.IqType.get;
            viq.From = Jabber.xmpp.MyJID;
            if (Jabber.xmpp.MyJID.ToString() != jabberID.full) {
                viq.To = new agsXMPP.Jid(jabberID.full);
            }
            Jabber.xmpp.IqGrabber.SendIq(viq, new agsXMPP.IqCB(retrieveIqResult), viq.Id);
            return true;
        }

        /// <summary>
        /// Charge la fiche d'identité locale
        /// </summary>
        public void load() {
            string filename = Regex.Replace(jabberID.full, @"[^\w\.@-]", "_") + ".xml";
            string path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetName().CodeBase.Replace("file:///", ""));
            if (!path.EndsWith(Path.DirectorySeparatorChar.ToString())) { path += Path.DirectorySeparatorChar.ToString(); }
            path += filename;
            if (File.Exists(path)) {
                try {
                    XmlDocument xDoc = new XmlDocument();
                    xDoc.Load(path);
                    XmlElement vcard = xDoc["vcard"];
                    XmlElement xAddresses = vcard["addresses"];
                    if (xAddresses.HasChildNodes) {
                        _address.Clear();
                        foreach (XmlNode xAddress in xAddresses.ChildNodes) {
                            Address a = new Address();
                            if (xAddress.Attributes["location"].Value == Enums.LocationType.Home.ToString()) {
                                a.location = Enums.LocationType.Home;
                            } else if (xAddress.Attributes["location"].Value == Enums.LocationType.Work.ToString()) {
                                a.location = Enums.LocationType.Work;
                            } else {
                                a.location = Enums.LocationType.None;
                            }
                            a.city = (xAddress["city"].InnerText != null) ? xAddress["city"].InnerText : string.Empty;
                            a.country = (xAddress["country"].InnerText != null) ? xAddress["country"].InnerText : string.Empty;
                            a.extra = (xAddress["extra"].InnerText != null) ? xAddress["extra"].InnerText : string.Empty;
                            a.region = (xAddress["region"].InnerText != null) ? xAddress["region"].InnerText : string.Empty;
                            a.street = (xAddress["street"].InnerText != null) ? xAddress["street"].InnerText : string.Empty;
                            a.zipcode = (xAddress["zipcode"].InnerText != null) ? xAddress["zipcode"].InnerText : string.Empty;
                            _address.Add(a);
                        }
                    }
                    _birthday = (vcard["birthday"].InnerText != null) ? DateTime.Parse(vcard["birthday"].InnerText) : DateTime.Now;
                    _description = (vcard["description"].InnerText != null) ? vcard["description"].InnerText : string.Empty;
                    XmlElement xEmails = vcard["emails"];
                    if (xEmails.HasChildNodes) {
                        _email.Clear();
                        foreach (XmlNode xEmail in xEmails.ChildNodes) {
                            Email m = new Email();
                            if (xEmail.Attributes["type"].Value == Enums.EmailType.Home.ToString()) {
                                m.type = Enums.EmailType.Home;
                            } else if (xEmail.Attributes["type"].Value == Enums.EmailType.Internet.ToString()) {
                                m.type = Enums.EmailType.Internet;
                            } else if (xEmail.Attributes["type"].Value == Enums.EmailType.Work.ToString()) {
                                m.type = Enums.EmailType.Work;
                            } else if (xEmail.Attributes["type"].Value == Enums.EmailType.X400.ToString()) {
                                m.type = Enums.EmailType.X400;
                            } else {
                                m.type = Enums.EmailType.None;
                            }
                            m.address = (xEmail.InnerText != null) ? xEmail.InnerText : string.Empty;
                            _email.Add(m);
                        }
                    }
                    _fullname = (vcard["fullname"].InnerText != null) ? vcard["fullname"].InnerText : string.Empty;
                    XmlElement xName = vcard["name"];
                    Name n = new Name();
                    n.firstname = (xName["firstname"].InnerText != null) ? xName["firstname"].InnerText : string.Empty;
                    n.lastname = (xName["lastname"].InnerText != null) ? xName["lastname"].InnerText : string.Empty;
                    n.middle = (xName["middle"].InnerText != null) ? xName["middle"].InnerText : string.Empty;
                    _name = n;
                    nickname = (vcard["nickname"].InnerText != null) ? vcard["nickname"].InnerText : string.Empty;
                    XmlElement xOrganization = vcard["organization"];
                    Organization o = new Organization();
                    o.name = (xOrganization["name"].InnerText != null) ? xOrganization["name"].InnerText : string.Empty;
                    o.unit = (xOrganization["unit"].InnerText != null) ? xOrganization["unit"].InnerText : string.Empty;
                    _organization = o;
                    if (vcard["photo"].HasChildNodes && vcard["photo"].HasAttribute("format")) {
                        string format = vcard["photo"].Attributes["format"].Value;
                        ImageFormat ift = null;
                        if (format == ImageFormat.Bmp.ToString()) { ift = ImageFormat.Bmp; } else if (format == ImageFormat.Gif.ToString()) { ift = ImageFormat.Gif; } else if (format == ImageFormat.Icon.ToString()) { ift = ImageFormat.Icon; } else if (format == ImageFormat.Jpeg.ToString()) { ift = ImageFormat.Jpeg; } else if (format == ImageFormat.Png.ToString()) { ift = ImageFormat.Png; } else { ift = null; }
                        if (ift != null) {
                            byte[] bts = Convert.FromBase64String(vcard["photo"].ChildNodes[0].Value);
                            MemoryStream ms = new MemoryStream(bts);
                            photo = Image.FromStream(ms);
                            byte[] hash = SHA1.Create().ComputeHash(ms.ToArray());
                            ms.Close();
                            ms.Dispose();
                            string hashString = string.Empty;
                            foreach (byte b in hash) {
                                string hex = b.ToString("X").ToLower();
                                hex = ((hex.Length == 1) ? "0" : string.Empty) + hex;
                                hashString += hex;
                            }
                            hash = null;
                            _photoHash = hashString;
                            hashString = string.Empty;
                        }
                    }
                    _role = (vcard["role"].InnerText != null) ? vcard["role"].InnerText : string.Empty;
                    XmlElement xTel = vcard["telephones"];
                    if (xTel.HasChildNodes) {
                        _telephone.Clear();
                        foreach (XmlNode xT in xTel.ChildNodes) {
                            Telehone t = new Telehone();
                            if (xT.Attributes["location"].Value == Enums.LocationType.Home.ToString()) {
                                t.location = Enums.LocationType.Home;
                            } else if (xT.Attributes["location"].Value == Enums.LocationType.Work.ToString()) {
                                t.location = Enums.LocationType.Work;
                            } else {
                                t.location = Enums.LocationType.None;
                            }
                            if (xT.Attributes["type"].Value == Enums.PhoneType.Bbs.ToString()) {
                                t.type = Enums.PhoneType.Bbs;
                            } else if (xT.Attributes["type"].Value == Enums.PhoneType.Cell.ToString()) {
                                t.type = Enums.PhoneType.Cell;
                            } else if (xT.Attributes["type"].Value == Enums.PhoneType.Fax.ToString()) {
                                t.type = Enums.PhoneType.Fax;
                            } else if (xT.Attributes["type"].Value == Enums.PhoneType.Isdn.ToString()) {
                                t.type = Enums.PhoneType.Isdn;
                            } else if (xT.Attributes["type"].Value == Enums.PhoneType.Modem.ToString()) {
                                t.type = Enums.PhoneType.Modem;
                            } else if (xT.Attributes["type"].Value == Enums.PhoneType.Msg.ToString()) {
                                t.type = Enums.PhoneType.Msg;
                            } else if (xT.Attributes["type"].Value == Enums.PhoneType.Number.ToString()) {
                                t.type = Enums.PhoneType.Number;
                            } else if (xT.Attributes["type"].Value == Enums.PhoneType.Pager.ToString()) {
                                t.type = Enums.PhoneType.Pager;
                            } else if (xT.Attributes["type"].Value == Enums.PhoneType.Pcs.ToString()) {
                                t.type = Enums.PhoneType.Pcs;
                            } else if (xT.Attributes["type"].Value == Enums.PhoneType.Pref.ToString()) {
                                t.type = Enums.PhoneType.Pref;
                            } else if (xT.Attributes["type"].Value == Enums.PhoneType.Video.ToString()) {
                                t.type = Enums.PhoneType.Video;
                            } else if (xT.Attributes["type"].Value == Enums.PhoneType.Voice.ToString()) {
                                t.type = Enums.PhoneType.Voice;
                            } else {
                                t.type = Enums.PhoneType.None;
                            }
                            t.number = (xT.InnerText != null) ? xT.InnerText : string.Empty;
                            _telephone.Add(t);
                        }
                    }
                    _title = (vcard["title"].InnerText != null) ? vcard["title"].InnerText : string.Empty;
                    _url = (vcard["url"].InnerText != null) ? vcard["url"].InnerText : string.Empty;
                    onIdentityRetrieved();
                } catch { }
            }
        }

        /// <summary>
        /// Publication de la fiche d'identité
        /// </summary>
        /// <returns>vrai si la publication a bien été effectuée, sinon faux</returns>
        public bool publish() {
            // si il n'y a pas de stream XMPP en cours ou si ce n'est pas notre fiche d'identité, on s'arrete là.
            if (!Jabber.xmpp.Authenticated || Jabber.xmpp.MyJID.ToString() != jabberID.full) {
                return false;
            }
            agsXMPP.protocol.iq.vcard.VcardIq viq = new agsXMPP.protocol.iq.vcard.VcardIq();
            viq.Type = agsXMPP.protocol.client.IqType.set;
            viq.To = Jabber.xmpp.MyJID;
            viq.Vcard = toVcard();
            Jabber.xmpp.Send(viq);
            onIdentityPublished();
            return true;
        }

        /// <summary>
        /// Sauvegarde locale de la fiche d'identité
        /// </summary>
        public void save() {
            string filename = Regex.Replace(jabberID.full, @"[^\w\.@-]", "_") + ".xml";
            string path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetName().CodeBase.Replace("file:///", ""));
            if (!path.EndsWith(Path.DirectorySeparatorChar.ToString())) { path += Path.DirectorySeparatorChar.ToString(); }
            path += filename;
            XmlDocument xDoc = new XmlDocument();
            xDoc.AppendChild(xDoc.CreateXmlDeclaration("1.0", null, null));
            XmlElement vcard = xDoc.CreateElement("vcard");
            XmlElement xAddresses = xDoc.CreateElement("addresses");
            foreach (Address a in address) {
                XmlElement xAddress = xDoc.CreateElement("address");
                XmlAttribute xAddressLocation = xDoc.CreateAttribute("location");
                xAddressLocation.Value = a.location.ToString();
                xAddress.Attributes.Append(xAddressLocation);
                XmlElement xAddressCity = xDoc.CreateElement("city");
                xAddressCity.AppendChild(xDoc.CreateTextNode(a.city));
                xAddress.AppendChild(xAddressCity);
                XmlElement xAddressCountry = xDoc.CreateElement("country");
                xAddressCountry.AppendChild(xDoc.CreateTextNode(a.country));
                xAddress.AppendChild(xAddressCountry);
                XmlElement xAddressExtra = xDoc.CreateElement("extra");
                xAddressExtra.AppendChild(xDoc.CreateTextNode(a.extra));
                xAddress.AppendChild(xAddressExtra);
                XmlElement xAddressRegion = xDoc.CreateElement("region");
                xAddressRegion.AppendChild(xDoc.CreateTextNode(a.region));
                xAddress.AppendChild(xAddressRegion);
                XmlElement xAddressStreet = xDoc.CreateElement("street");
                xAddressStreet.AppendChild(xDoc.CreateTextNode(a.street));
                xAddress.AppendChild(xAddressStreet);
                XmlElement xAddressZipcode = xDoc.CreateElement("zipcode");
                xAddressZipcode.AppendChild(xDoc.CreateTextNode(a.zipcode));
                xAddress.AppendChild(xAddressZipcode);
                xAddresses.AppendChild(xAddress);
            }
            vcard.AppendChild(xAddresses);
            XmlElement xBirthday = xDoc.CreateElement("birthday");
            xBirthday.AppendChild(xDoc.CreateTextNode(birthday.ToString()));
            vcard.AppendChild(xBirthday);
            XmlElement xDescription = xDoc.CreateElement("description");
            xDescription.AppendChild(xDoc.CreateTextNode(description));
            vcard.AppendChild(xDescription);
            XmlElement xEmails = xDoc.CreateElement("emails");
            foreach (Email m in email) {
                XmlElement xEmail = xDoc.CreateElement("email");
                XmlAttribute xEmailType = xDoc.CreateAttribute("type");
                xEmailType.Value = m.type.ToString();
                xEmail.Attributes.Append(xEmailType);
                xEmail.AppendChild(xDoc.CreateTextNode(m.address));
                xEmails.AppendChild(xEmail);
            }
            vcard.AppendChild(xEmails);
            XmlElement xFullname = xDoc.CreateElement("fullname");
            xFullname.AppendChild(xDoc.CreateTextNode(fullname));
            vcard.AppendChild(xFullname);
            XmlElement xName = xDoc.CreateElement("name");
            XmlElement xNameFirstname = xDoc.CreateElement("firstname");
            xNameFirstname.AppendChild(xDoc.CreateTextNode(name.firstname));
            xName.AppendChild(xNameFirstname);
            XmlElement xNameMiddle = xDoc.CreateElement("middle");
            xNameMiddle.AppendChild(xDoc.CreateTextNode(name.middle));
            xName.AppendChild(xNameMiddle);
            XmlElement xNameLastname = xDoc.CreateElement("lastname");
            xNameLastname.AppendChild(xDoc.CreateTextNode(name.lastname));
            xName.AppendChild(xNameLastname);
            vcard.AppendChild(xName);
            XmlElement xNickname = xDoc.CreateElement("nickname");
            xNickname.AppendChild(xDoc.CreateTextNode(nickname));
            vcard.AppendChild(xNickname);
            XmlElement xOrganization = xDoc.CreateElement("organization");
            XmlElement xOrganizationName = xDoc.CreateElement("name");
            xOrganizationName.AppendChild(xDoc.CreateTextNode(organization.name));
            xOrganization.AppendChild(xOrganizationName);
            XmlElement xOrganizationUnit = xDoc.CreateElement("unit");
            xOrganizationUnit.AppendChild(xDoc.CreateTextNode(organization.unit));
            xOrganization.AppendChild(xOrganizationUnit);
            vcard.AppendChild(xOrganization);
            try {
                XmlElement xPhoto = xDoc.CreateElement("photo");
                if (photo != null && photoFormat != null) {
                    XmlAttribute xPhotoFormat = xDoc.CreateAttribute("format");
                    xPhotoFormat.Value = photoFormat.ToString();
                    xPhoto.Attributes.Append(xPhotoFormat);
                    MemoryStream ms = new MemoryStream();
                    photo.Save(ms, photoFormat);
                    xPhoto.AppendChild(xDoc.CreateCDataSection(Convert.ToBase64String(ms.ToArray())));
                    ms.Close();
                    ms.Dispose();
                }
                vcard.AppendChild(xPhoto);
            } catch { }
            XmlElement xRole = xDoc.CreateElement("role");
            xRole.AppendChild(xDoc.CreateTextNode(role));
            vcard.AppendChild(xRole);
            XmlElement xTel = xDoc.CreateElement("telephones");
            foreach (Telehone t in telephone) {
                XmlElement xT = xDoc.CreateElement("telephone");
                XmlAttribute xTLocation = xDoc.CreateAttribute("location");
                xTLocation.Value = t.location.ToString();
                xT.Attributes.Append(xTLocation);
                XmlAttribute xTType = xDoc.CreateAttribute("type");
                xTType.Value = t.type.ToString();
                xT.Attributes.Append(xTType);
                xT.AppendChild(xDoc.CreateTextNode(t.number));
                xTel.AppendChild(xT);
            }
            vcard.AppendChild(xTel);
            XmlElement xTitle = xDoc.CreateElement("title");
            xTitle.AppendChild(xDoc.CreateTextNode(title));
            vcard.AppendChild(xTitle);
            XmlElement xUrl = xDoc.CreateElement("url");
            xUrl.AppendChild(xDoc.CreateTextNode(url));
            vcard.AppendChild(xUrl);
            xDoc.AppendChild(vcard);
            xDoc.Save(path);
        }

        /// <summary>
        /// Convertit la fiche d'identité au format utilisé par la librairie agsXMPP
        /// </summary>
        /// <returns>Carte de visite</returns>
        public agsXMPP.protocol.iq.vcard.Vcard toVcard() {
            agsXMPP.protocol.iq.vcard.Vcard vcard = new agsXMPP.protocol.iq.vcard.Vcard();
            vcard.Birthday = birthday;
            vcard.Description = description;
            vcard.Fullname = fullname;
            vcard.JabberId = new agsXMPP.Jid(jabberID.full);
            vcard.Name = new agsXMPP.protocol.iq.vcard.Name(name.lastname, name.firstname, name.middle);
            vcard.Nickname = nickname;
            vcard.Organization = new agsXMPP.protocol.iq.vcard.Organization(organization.name, organization.unit);
            if (photoFormat != null) {
                vcard.Photo = new agsXMPP.protocol.iq.vcard.Photo(photo, photoFormat);
            } else {
                vcard.Photo = null;
            }
            vcard.Role = role;
            vcard.Title = title;
            vcard.Url = url;
            foreach (Email m in email) {
                vcard.AddEmailAddress(new agsXMPP.protocol.iq.vcard.Email(Enums.EmailTypeConverter(m.type), m.address, false));
            }
            foreach (Telehone t in telephone) {
                vcard.AddTelephoneNumber(new agsXMPP.protocol.iq.vcard.Telephone(Enums.LocationTypeConverter(t.location), Enums.PhoneTypeConverter(t.type), t.number));
            }
            foreach (Address a in address) {
                agsXMPP.protocol.iq.vcard.Address ad = new agsXMPP.protocol.iq.vcard.Address();
                ad.Country = a.country;
                ad.ExtraAddress = a.extra;
                ad.Locality = a.city;
                ad.Location = Enums.AddressLocationTypeConverter(a.location);
                ad.PostalCode = a.zipcode;
                ad.Region = a.region;
                ad.Street = a.street;
                vcard.AddAddress(ad);
            }
            return vcard;
        }

        #endregion

        /// <summary>
        /// Traitement du résultat de la demande de récupération de la fiche d'identité
        /// </summary>
        /// <param name="sender">Objet représentant la routine ayant fait appel à cette méthode</param>
        /// <param name="iq">Requète représentant le résultat de la demande</param>
        /// <param name="data">Identifiant de la requète</param>
        private void retrieveIqResult(object sender, agsXMPP.protocol.client.IQ iq, object data) {
            string iqID = data as string;
            // si on a une erreur alors on en informe la librairie
            if (iq.Type == agsXMPP.protocol.client.IqType.error) {
                throw new Exception(iq.Error.Code.ToString() + " - " + iq.Error.Message);
            }
                // sinon et seulement si c'est le résultat, on lance le traitement
            else if (iq.Type == agsXMPP.protocol.client.IqType.result) {
                // si la requette nous transmet la carte de visite
                if (iq.Vcard != null) {
                    _birthday = iq.Vcard.Birthday;
                    _description = (iq.Vcard.Description != null) ? iq.Vcard.Description.Trim() : string.Empty;
                    _fullname = (iq.Vcard.Fullname != null) ? iq.Vcard.Fullname.Trim() : string.Empty;
                    _name = new Name();
                    _name.firstname = (iq.Vcard.Name != null && iq.Vcard.Name.Given != null) ? iq.Vcard.Name.Given.Trim() : string.Empty;
                    _name.middle = (iq.Vcard.Name != null && iq.Vcard.Name.Middle != null) ? iq.Vcard.Name.Middle.Trim() : string.Empty;
                    _name.lastname = (iq.Vcard.Name != null && iq.Vcard.Name.Family != null) ? iq.Vcard.Name.Family.Trim() : string.Empty;
                    nickname = (iq.Vcard.Nickname != null) ? iq.Vcard.Nickname.Trim() : string.Empty;
                    _organization = new Organization();
                    _organization.name = (iq.Vcard.Organization != null && iq.Vcard.Organization.Name != null) ? iq.Vcard.Organization.Name.Trim() : string.Empty;
                    _organization.unit = (iq.Vcard.Organization != null && iq.Vcard.Organization.Unit != null) ? iq.Vcard.Organization.Unit.Trim() : string.Empty;
                    _role = (iq.Vcard.Role != null) ? iq.Vcard.Role.Trim() : string.Empty;
                    _title = (iq.Vcard.Title != null) ? iq.Vcard.Title.Trim() : string.Empty;
                    _url = (iq.Vcard.Url != null) ? iq.Vcard.Url.Trim() : string.Empty;
                    if (iq.Vcard.GetEmailAddresses() != null) {
                        _email.Clear();
                        foreach (agsXMPP.protocol.iq.vcard.Email em in iq.Vcard.GetEmailAddresses()) {
                            if (em != null && em.UserId != null) {
                                Email m = new Email();
                                m.type = Enums.EmailTypeConverter(em.Type);
                                m.address = em.UserId;
                                _email.Add(m);
                            }
                        }
                    }
                    if (iq.Vcard.GetTelephoneNumbers() != null) {
                        _telephone.Clear();
                        foreach (agsXMPP.protocol.iq.vcard.Telephone phone in iq.Vcard.GetTelephoneNumbers()) {
                            if (phone != null && phone.Number != null) {
                                Telehone t = new Telehone();
                                t.location = Enums.LocationTypeConverter(phone.Location);
                                t.type = Enums.PhoneTypeConverter(phone.Type);
                                t.number = phone.Number;
                                _telephone.Add(t);
                            }
                        }
                    }
                    if (iq.Vcard.GetAddresses() != null) {
                        _address.Clear();
                        foreach (agsXMPP.protocol.iq.vcard.Address ad in iq.Vcard.GetAddresses()) {
                            if (ad != null) {
                                Address a = new Address();
                                a.location = Enums.AddressLocationTypeConverter(ad.Location);
                                a.city = (ad.Locality != null) ? ad.Locality.Trim() : string.Empty;
                                a.country = (ad.Country != null) ? ad.Country.Trim() : string.Empty;
                                a.extra = (ad.ExtraAddress != null) ? ad.ExtraAddress.Trim() : string.Empty;
                                a.region = (ad.Region != null) ? ad.Region.Trim() : string.Empty;
                                a.street = (ad.Street != null) ? ad.Street.Trim() : string.Empty;
                                a.zipcode = (ad.PostalCode != null) ? ad.PostalCode.Trim() : string.Empty;
                                _address.Add(a);
                            }
                        }
                    }
                    photo = (iq.Vcard.Photo != null) ? iq.Vcard.Photo.Image : null;
                    onIdentityRetrieved();
                }
            }
            if (Jabber.xmpp.IqGrabber != null) { Jabber.xmpp.IqGrabber.Remove(iqID); }
        }

        /// <summary>
        /// Réceptionne l'intégralité des requètes du stream en cours pour traiter seulement celles qui nous interresse
        /// </summary>
        /// <param name="sender">Objet parent</param>
        /// <param name="iq">Requete reçue</param>
        private void iqManager(object sender, agsXMPP.protocol.client.IQ iq) { }

    }

}
