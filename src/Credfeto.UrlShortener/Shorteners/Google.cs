using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

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
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<Google> _logging;
        private readonly IOptions<GoogleConfiguration> _options;

        public Google(IHttpClientFactory httpClientFactory, IOptions<GoogleConfiguration> options, ILogger<Google> logging)
        {
            this._httpClientFactory = httpClientFactory;
            this._options = options;
            this._logging = logging;
        }

        /// <summary>
        ///     Shortens the given URL.
        /// </summary>
        /// <param name="fullUrl">
        ///     The URL to shorten.
        /// </param>
        /// <returns>
        ///     The shortened version of the URL.
        /// </returns>
        public Uri Shorten([NotNull] Uri fullUrl)
        {
            string post = "{\"longUrl\": \"" + fullUrl + "\"}";
            HttpWebRequest request = (HttpWebRequest) WebRequest.Create(new Uri("https://www.googleapis.com/urlshortener/v1/url?key=" + this._options.Value.ApiKey));

            try
            {
                request.ServicePoint.Expect100Continue = false;
                request.Method = "POST";
                request.ContentLength = post.Length;
                request.ContentType = "application/json";
                request.Headers.Add(name: "Cache-Control", value: "no-cache");

                using (Stream requestStream = request.GetRequestStream())
                {
                    byte[] postBuffer = Encoding.ASCII.GetBytes(post);
                    requestStream.Write(buffer: postBuffer, offset: 0, count: postBuffer.Length);
                }

                using (HttpWebResponse response = (HttpWebResponse) request.GetResponse())
                {
                    using (Stream responseStream = response.GetResponseStream())
                    {
                        if (responseStream == null)
                        {
                            return fullUrl;
                        }

                        using (StreamReader responseReader = new StreamReader(responseStream))
                        {
                            string json = responseReader.ReadToEnd();

                            if (string.IsNullOrEmpty(json))
                            {
                                return fullUrl;
                            }

                            string shortened = Regex.Match(input: json, pattern: @"""id"": ?""(?<id>.+)""")
                                                    .Groups["id"]
                                                    .Value;

                            if (string.IsNullOrEmpty(shortened))
                            {
                                return fullUrl;
                            }

                            return new Uri(shortened);
                        }
                    }
                }
            }
            catch (Exception)
            {
                // if Google's URL Shortner is down...
                return fullUrl;
            }
        }
    }

    public sealed class GoogleConfiguration
    {
        public string ApiKey { get; set; }

        public string Login { get; set; }
    }
}