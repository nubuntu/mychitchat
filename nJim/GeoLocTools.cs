using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Net;
using System.Xml;
using System.Drawing;
using System.IO;
using System.Drawing.Imaging;
using System.IO.Ports;

namespace nJim
{

    /// <summary>
    /// Fournit un ensemble d'outils sur le thème de la géolocalisation
    /// </summary>
    public static class GeoLocTools
    {

        /// <summary>
        /// Structure d'une géolocalisation
        /// </summary>
        public struct GeoStructure
        {

            /// <summary>
            /// Numéro et nom de la rue
            /// </summary>
            public string street;

            /// <summary>
            /// Ville
            /// </summary>
            public string city;

            /// <summary>
            /// Pays
            /// </summary>
            public string country;

            /// <summary>
            /// Code postal
            /// </summary>
            public string zipcode;

            /// <summary>
            /// Adresse au format libre
            /// </summary>
            public string location;

            /// <summary>
            /// Lattitude
            /// </summary>
            public double latitude;

            /// <summary>
            /// Longitude
            /// </summary>
            public double longitude;

        }

		/// <summary>
		/// Système métrique utilisable par cet outil
		/// </summary>
		public enum GeoMetrics
		{

			/// <summary>
			/// Les kilomêtres
			/// </summary>
			kilometers,

			/// <summary>
			/// Les noeuds
			/// </summary>
			miles,

			/// <summary>
			/// Les noeuds marins
			/// </summary>
			nautical_miles

		}

		/// <summary>
		/// Taille en pixels de la carte
		/// </summary>
		public static Size mapSize = new Size(640, 480);

		/// <summary>
		/// Zoom satellite de la carte
		/// <remarks>
		///		minimum 1 (le plus prohe)
		///		maximum 12 (le plus éloigné)
		/// </remarks>
		/// </summary>
		public static int mapZoom = 6;

        /// <summary>
        /// Retrouve les coordonnées géographiques en fonction d'une adresse postale
        /// </summary>
        /// <param name="geo">Parammêtres de l'adresse postale</param>
        public static void physicalAddressToGeoCodes(ref GeoStructure geo)
        {
            if (Properties.Settings.Default.Properties["YAK"] != null && Properties.Settings.Default.Properties["YGCU"] != null)
            {
                string yak = Properties.Settings.Default.YAK.Trim();
                string ygcu = Properties.Settings.Default.YGCU.Trim();
                if (yak != string.Empty && ygcu != string.Empty)
                {
                    string url = ygcu + "?appid=" + yak + "&output=xml";
                    if (geo.location != null)
                    {
                        url += "&address=" + HttpUtility.UrlEncode(geo.location.Trim(), Encoding.UTF8);
                    }
                    else
                    {
                        if (geo.street != null) { url += "&street=" + HttpUtility.UrlEncode(geo.street.Trim(), Encoding.UTF8); }
                        if (geo.city != null) { url += "&city=" + HttpUtility.UrlEncode(geo.city.Trim(), Encoding.UTF8); }
                        if (geo.zipcode != null) { url += "&zip=" + HttpUtility.UrlEncode(geo.zipcode.Trim(), Encoding.UTF8); }
                    }
                    url += "&state=" + ((geo.country != null) ? HttpUtility.UrlEncode(geo.country.Trim(), Encoding.UTF8) : string.Empty);
                    try
                    {
                        WebClient wc = new WebClient();
                        string response = wc.DownloadString(url);
                        wc.Dispose();
                        XmlDocument xDoc = new XmlDocument();
                        xDoc.LoadXml(response);
                        if (xDoc.DocumentElement != null && xDoc.DocumentElement["Result"] != null)
                        {
                            XmlElement result = xDoc.DocumentElement["Result"];
                            if (result["Latitude"] != null) { geo.latitude = Convert.ToDouble(result["Latitude"].InnerText, System.Globalization.CultureInfo.InvariantCulture); }
							if (result["Longitude"] != null) { geo.longitude = Convert.ToDouble(result["Longitude"].InnerText, System.Globalization.CultureInfo.InvariantCulture); }
                        }
                    }
					catch (Exception ex) { Console.WriteLine(ex); }
                }
            }
        }

		/// <summary>
		/// Renvoi une image de la carte en fonction de la lattitude et de la longitude
		/// </summary>
		/// <param name="latitude">Latitude</param>
		/// <param name="longitude">Longitude</param>
		/// <returns>Image ou nul</returns>
		public static Image geoCodesToMapImage(double latitude, double longitude)
		{
			if (Properties.Settings.Default.Properties["YAK"] != null && Properties.Settings.Default.Properties["YMIU"] != null)
			{
				string yak = Properties.Settings.Default.YAK.Trim();
				string ymiu = Properties.Settings.Default.YMIU.Trim();
				if (yak != string.Empty && ymiu != string.Empty)
				{
					string url = ymiu + "?appid=" + yak + "&output=xml&image_type=png&image_width=" + mapSize.Width.ToString() + "&image_height=" + mapSize.Height.ToString() + "&zoom=" + mapZoom.ToString();
					url += "&latitude=" + Convert.ToString(latitude, System.Globalization.CultureInfo.InvariantCulture);
					url += "&longitude=" + Convert.ToString(longitude, System.Globalization.CultureInfo.InvariantCulture);
					try
					{
						WebClient wc = new WebClient();
						string response = wc.DownloadString(url);
						wc.Dispose();
						XmlDocument xDoc = new XmlDocument();
						xDoc.LoadXml(response);
						if (xDoc.DocumentElement != null)
						{
							if (xDoc.DocumentElement.InnerText != null)
							{
								wc = new WebClient();
								byte[] datas = wc.DownloadData(xDoc.DocumentElement.InnerText);
								wc.Dispose();
								MemoryStream ms = new MemoryStream(datas);
								Image i = Image.FromStream(ms);
								ms.Close();
								ms.Dispose();
								return i;
							}
						}
					}
					catch (Exception ex) { Console.WriteLine(ex); }
				}
			}
			return null;
		}

		/// <summary>
		/// Renvoi une image de la carte en fonction d'une adresse postale
		/// </summary>
		/// <param name="geo">Parammêtres de l'adresse postale</param>
		/// <returns>Image ou nul</returns>
		public static Image physicalAddressToMapImage(GeoStructure geo)
		{
			GeoStructure gs = geo;
			physicalAddressToGeoCodes(ref gs);
			return geoCodesToMapImage(gs.latitude, gs.longitude);
		}

		/// <summary>
		/// Calcule la distance entre 2 coordonées géographique
		/// </summary>
		/// <param name="latitude1">Latitude du point 1</param>
		/// <param name="longitude1">Longitude du point 1</param>
		/// <param name="latitude2">Latitude du point 2</param>
		/// <param name="longitude2">Longitude du point 2</param>
		/// <param name="units">Unité de conversion de retrour</param>
		/// <returns>Distance</returns>
		public static double distanceBetweenTwoGeoCodes(double latitude1, double longitude1, double latitude2, double longitude2, GeoMetrics units)
		{
			double distance = rad2deg(Math.Acos(Math.Sin(deg2rad(latitude1)) * Math.Sin(deg2rad(latitude2)) + Math.Cos(deg2rad(latitude1)) * Math.Cos(deg2rad(latitude2)) * Math.Cos(deg2rad(longitude1 - longitude2))));
			distance = distance * 60 * 1.1515;
			switch(units)
			{
				case GeoMetrics.kilometers: return (distance * 1.609344);
				case GeoMetrics.nautical_miles: return (distance * 0.8684);
				default: return distance;
			}
		}

		/// <summary>
		/// Convertit les degrés en radians
		/// </summary>
		/// <param name="deg">Degrés à convertir</param>
		/// <returns>Résultat en radian</returns>
		public static double deg2rad(double deg)
		{
			return (deg * Math.PI / 180.0);
		}

		/// <summary>
		/// Convertit les radians en degrés
		/// </summary>
		/// <param name="rad">Radians à convertir</param>
		/// <returns>Résultat en degrés</returns>
		public static double rad2deg(double rad)
		{
			return (rad / Math.PI * 180.0);
		}

    }

}
