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
    [SuppressMessage(category: "Microsoft.Naming", checkId: "CA1704:IdentifiersShouldBeSpelledCorrectly", Justification = "Bitly is name of site.")]
    public sealed class Bitly : IUrlShortener
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<Bitly> _logging;
        private readonly IOptions<BitlyConfiguration> _options;

        public Bitly(IHttpClientFactory httpClientFactory, IOptions<BitlyConfiguration> options, ILogger<Bitly> logging)
        {
            this._httpClientFactory = httpClientFactory;
            this._logging = logging;
            this._options = options;
        }

        /// <summary>
        ///     Shortens the given URL.
        /// </summary>
        /// <param name="fullUrl">The URL to shorten.</param>
        /// <returns>
        ///     The shortened version of the URL.
        /// </returns>
        public Uri Shorten([NotNull] Uri fullUrl)
        {
            string encodedUrl = HttpUtility.UrlEncode(fullUrl.ToString());
            string urlRequest = string.Format(provider: CultureInfo.InvariantCulture,
                                              format: "https://api-ssl.bit.ly/v3/shorten?apiKey={0}&login={1}&format=txt&longurl={2}",
                                              arg0: this._options.Value.ApiKey,
                                              arg1: this._options.Value.Login,
                                              arg2: encodedUrl);

            HttpWebRequest request = (HttpWebRequest) WebRequest.Create(new Uri(urlRequest));

            try
            {
                request.ContentType = "application/json";
                request.Headers.Add(name: "Cache-Control", value: "no-cache");

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
                            string shortened = responseReader.ReadToEnd();

                            return string.IsNullOrEmpty(shortened) ? fullUrl : new Uri(shortened);
                        }
                    }
                }
            }
            catch (Exception)
            {
                // if Bitly's URL Shortner is down...
                return fullUrl;
            }
        }
    }

    public sealed class BitlyConfiguration
    {
        public string ApiKey { get; set; }

        public string Login { get; set; }
    }
}