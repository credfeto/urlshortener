// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BitLY.cs" company="Twaddle Software">
//   Copyright (c) Twaddle Software
// </copyright>
// <summary>
//   Bit.ly's URL Shortener.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#region Using Directives

using System;
using System.Configuration;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.IO;
using System.Net;
using System.Web;
using JetBrains.Annotations;

#endregion

namespace Twaddle.Web.UrlShortener.Shorteners
{
    /// <summary>
    ///     Bit.ly's URL Shortener.
    /// </summary>
    /// <remarks>
    ///     Get free key from https://bitly.com/a/your_api_key for up to 1000000 shortenings per day.
    /// </remarks>
    [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly",
        Justification = "Bitly is name of site.")]
    public class Bitly : IUrlShortener
    {
        #region Constants and Fields

        /// <summary>
        ///     The API key.
        /// </summary>
        [NotNull] private readonly string _key;

        /// <summary>
        ///     The bitly username.
        /// </summary>
        [NotNull] private readonly string _login;

        #endregion

        public Bitly()
        {
            _key = ConfigurationManager.AppSettings["BitLyApiKey"];
            _login = ConfigurationManager.AppSettings["BitLyLogin"];
        }

        #region Public Methods

        /// <summary>
        ///     Shortens the given URL.
        /// </summary>
        /// <param name="url">The URL to shorten.</param>
        /// <returns>
        ///     The shortened version of the URL.
        /// </returns>
        [NotNull]
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes",
            Justification = "Many possible exceptions")]
        public Uri Shorten([NotNull] Uri url)
        {
            string encodedUrl = HttpUtility.UrlEncode(url.ToString());
            string urlRequest =
                string.Format(
                    CultureInfo.InvariantCulture,
                    "https://api-ssl.bit.ly/v3/shorten?apiKey={0}&login={1}&format=txt&longurl={2}",
                    _key,
                    _login,
                    encodedUrl);

            var request = (HttpWebRequest) WebRequest.Create(new Uri(urlRequest));
            try
            {
                request.ContentType = "application/json";
                request.Headers.Add("Cache-Control", "no-cache");
                using (var response = (HttpWebResponse) request.GetResponse())
                {
                    using (Stream responseStream = response.GetResponseStream())
                    {
                        if (responseStream == null)
                        {
                            return url;
                        }

                        using (var responseReader = new StreamReader(responseStream))
                        {
                            string shortened = responseReader.ReadToEnd();

                            return string.IsNullOrEmpty(shortened) ? url : new Uri(shortened);
                        }
                    }
                }
            }
            catch (Exception)
            {
                // if Bitly's URL Shortner is down...
                return url;
            }
        }

        #endregion
    }
}