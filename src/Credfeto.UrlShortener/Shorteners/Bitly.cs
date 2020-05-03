using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Web;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Credfeto.UrlShortener.Shorteners
{
    /// <summary>
    ///     Bit.ly's URL Shortener.
    /// </summary>
    /// <remarks>
    ///     Get free key from https://bitly.com/a/your_api_key for up to 1000000 shortenings per day.
    /// </remarks>
    [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly",
        Justification = "Bitly is name of site.")]
    public sealed class Bitly : IUrlShortener
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<Bitly> _logging;
        private readonly IOptions<BitlyConfiguration> _options;

        public Bitly(IHttpClientFactory httpClientFactory, IOptions<BitlyConfiguration> options, ILogger<Bitly> logging)
        {
            _httpClientFactory = httpClientFactory;
            _logging = logging;
            _options = options;
        }


        /// <summary>
        ///     Shortens the given URL.
        /// </summary>
        /// <param name="url">The URL to shorten.</param>
        /// <returns>
        ///     The shortened version of the URL.
        /// </returns>
        public Uri Shorten([NotNull] Uri url)
        {
            var encodedUrl = HttpUtility.UrlEncode(url.ToString());
            var urlRequest =
                string.Format(
                    CultureInfo.InvariantCulture,
                    "https://api-ssl.bit.ly/v3/shorten?apiKey={0}&login={1}&format=txt&longurl={2}",
                    _options.Value.ApiKey,
                    _options.Value.Login,
                    encodedUrl);

            var request = (HttpWebRequest) WebRequest.Create(new Uri(urlRequest));
            try
            {
                request.ContentType = "application/json";
                request.Headers.Add("Cache-Control", "no-cache");
                using (var response = (HttpWebResponse) request.GetResponse())
                {
                    using (var responseStream = response.GetResponseStream())
                    {
                        if (responseStream == null) return url;

                        using (var responseReader = new StreamReader(responseStream))
                        {
                            var shortened = responseReader.ReadToEnd();

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
    }

    public sealed class BitlyConfiguration
    {
        public string ApiKey { get; set; }
        public string Login { get; set; }
    }
}