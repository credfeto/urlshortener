using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace Credfeto.UrlShortener.Shorteners
{
    /// <summary>
    ///     Google's URL Shortener.
    /// </summary>
    /// <remarks>
    ///     Get free key from http://code.google.com/apis/console/ for up to 1000000 shortenings per day.
    /// </remarks>
    public class Google : IUrlShortener
    {
        /// <summary>
        ///     The API key.
        /// </summary>
        private readonly string _key;


        public Google()
        {
            _key = ConfigurationManager.AppSettings["GoogleApiKey"];
        }


        /// <summary>
        ///     Shortens the given URL.
        /// </summary>
        /// <param name="url">
        ///     The URL to shorten.
        /// </param>
        /// <returns>
        ///     The shortened version of the URL.
        /// </returns>
        public Uri Shorten([NotNull] Uri url)
        {
            var post = "{\"longUrl\": \"" + url + "\"}";
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

                using (var requestStream = request.GetRequestStream())
                {
                    var postBuffer = Encoding.ASCII.GetBytes(post);
                    requestStream.Write(postBuffer, 0, postBuffer.Length);
                }

                using (var response = (HttpWebResponse) request.GetResponse())
                {
                    using (var responseStream = response.GetResponseStream())
                    {
                        if (responseStream == null) return url;

                        using (var responseReader = new StreamReader(responseStream))
                        {
                            var json = responseReader.ReadToEnd();
                            if (string.IsNullOrEmpty(json)) return url;

                            var shortened = Regex.Match(json, @"""id"": ?""(?<id>.+)""").Groups["id"].Value;
                            if (string.IsNullOrEmpty(shortened)) return url;

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
    }
}