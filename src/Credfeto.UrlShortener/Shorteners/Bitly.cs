using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Credfeto.UrlShortener.Shorteners;

[SuppressMessage(category: "Microsoft.Naming", checkId: "CA1704:IdentifiersShouldBeSpelledCorrectly", Justification = "Bitly is name of site.")]
public sealed class Bitly : UrlShortenerBase, IUrlShortener
{
    private const string HTTP_CLIENT_NAME = nameof(Bitly);
    private readonly BitlyConfiguration _options;

    public Bitly(IHttpClientFactory httpClientFactory, IOptions<BitlyConfiguration> options, ILogger<Bitly> logger)
        : base(httpClientFactory: httpClientFactory, clientName: HTTP_CLIENT_NAME, logger: logger)
    {
        this._options = options.Value ?? throw new ArgumentNullException(nameof(options));
    }

    public string Name { get; } = nameof(Bitly);

    public async Task<Uri> ShortenAsync(Uri fullUrl, CancellationToken cancellationToken)
    {
        string encodedUrl = HttpUtility.UrlEncode(fullUrl.ToString());
        Uri shortUrl = new(string.Format(provider: CultureInfo.InvariantCulture,
                                         format: "/v3/shorten?apiKey={0}&login={1}&format=txt&longurl={2}",
                                         arg0: this._options.ApiKey,
                                         arg1: this._options.Login,
                                         arg2: encodedUrl),
                           uriKind: UriKind.Relative);

        try
        {
            HttpClient client = this.CreateClient();

            HttpResponseMessage response = await client.GetAsync(requestUri: shortUrl, cancellationToken: cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                return fullUrl;
            }

            string shortened = await response.Content.ReadAsStringAsync(cancellationToken);

            return string.IsNullOrEmpty(shortened)
                ? fullUrl
                : new(shortened);
        }
        catch (Exception exception)
        {
            this.Logging.LogError(new(exception.HResult), exception: exception, $"Error: Could not build Short Url: {exception.Message}");

            return fullUrl;
        }
    }

    public static void Register(IServiceCollection serviceCollection)
    {
        serviceCollection.AddSingleton<IUrlShortener, Bitly>();

        RegisterHttpClientFactory(serviceCollection: serviceCollection,
                                  userAgent: "Credfeto.UrlShortner.Bitly",
                                  clientName: HTTP_CLIENT_NAME,
                                  new(uriString: @"https://api-ssl.bit.ly/"));
    }
}