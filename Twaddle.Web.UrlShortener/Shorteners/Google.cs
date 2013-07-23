// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Google.cs" company="Twaddle Software">
//   Copyright (c) Twaddle Software
// </copyright>
// <summary>
//   Google's URL Shortener.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#region Using Directives

using System;
using System.Configuration;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using JetBrains.Annotations;

#endregion

namespace Twaddle.Web.UrlShortener.Shorteners
{
    /// <summary>
    ///     Google's URL Shortener.
    /// </summary>
    /// <remarks>
    ///     Get free key from http://code.google.com/apis/console/ for up to 1000000 shortenings per day.
    /// </remarks>
    public class Google : IUrlShortener
    {
        #region Constants and Fields

        /// <summary>
        ///     The API key.
        /// </summary>
        [NotNull] private readonly string _key;

        #endregion

        public Google()
        {
            _key = ConfigurationManager.AppSettings["GoogleApiKey"];
        }

        #region Public Methods

        /// <summary>
        ///     Shortens the given URL.
        /// </summary>
        /// <param name="url">
        ///     The URL to shorten.
        /// </param>
        /// <returns>
        ///     The shortened version of the URL.
        /// </returns>
        [NotNull]
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes",
            Justification = "Many possible exceptions")]
        public Uri Shorten([NotNull] Uri url)
        {
            Contract.Requires(url != null);
            Contract.Ensures(Contract.Result<Uri>() != null);

            string post = "{\"longUrl\": \"" + url + "\"}";
            var request =
                (HttpWebRequest)
                WebRequest.Create(new Uri("https://www.googleapis.com/urlshortener/v1/url?key=" + _key));
            try
            {
                request.ServicePoint.Expect100Continue = false;
                request.Method = "POST";
                request.ContentLength = post.Length;
                request.ContentType = "application/json";
                request.Headers.Add("Cache-Control", "no-cache");

                using (Stream requestStream = request.GetRequestStream())
                {
                    byte[] postBuffer = Encoding.ASCII.GetBytes(post);
                    requestStream.Write(postBuffer, 0, postBuffer.Length);
                }

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
                            string json = responseReader.ReadToEnd();
                            if (string.IsNullOrEmpty(json))
                            {
                                return url;
                            }

                            string shortened = Regex.Match(json, @"""id"": ?""(?<id>.+)""").Groups["id"].Value;
                            if (string.IsNullOrEmpty(shortened))
                            {
                                return url;
                            }

                            return new Uri(shortened);
                        }
                    }
                }
            }
            catch (Exception)
            {
                // if Google's URL Shortner is down...
                return url;
            }
        }

        #endregion
    }
}