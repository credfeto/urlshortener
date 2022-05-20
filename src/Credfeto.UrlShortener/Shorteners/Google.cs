using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Credfeto.UrlShortener.Shorteners.Serialisation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Credfeto.UrlShortener.Shorteners;

public sealed class Google : UrlShortenerBase, IUrlShortener
{
    private const string HTTP_CLIENT_NAME = nameof(Google);
    private readonly GoogleConfiguration _options;

    public Google(IHttpClientFactory httpClientFactory, IOptions<GoogleConfiguration> options, ILogger<Google> logger)
        : base(httpClientFactory: httpClientFactory, clientName: HTTP_CLIENT_NAME, logger: logger)

    {
        this._options = options.Value ?? throw new ArgumentNullException(nameof(options));
    }

    public string Name { get; } = HTTP_CLIENT_NAME;

    public async Task<Uri> ShortenAsync([NotNull] Uri fullUrl, CancellationToken cancellationToken)
    {
        try
        {
            HttpClient client = this.CreateClient();

            Uri uri = new("https://www.googleapis.com/urlshortener/v1/url?key=" + this._options.ApiKey);

            string requestJson = JsonSerializer.Serialize(new() { LongUrl = fullUrl.ToString() }, jsonTypeInfo: GoogleSerializationContext.Default.GoogleRequest);

            using (StringContent requestContent = new(content: requestJson, encoding: Encoding.UTF8, mediaType: "application/json"))
            {
                HttpResponseMessage response = await client.PutAsync(requestUri: uri, content: requestContent, cancellationToken: cancellationToken);

                if (!response.IsSuccessStatusCode)
                {
                    return fullUrl;
                }

                await using (Stream text = await response.Content.ReadAsStreamAsync(cancellationToken))
                {
                    GoogleResponse? responseModel =
                        await JsonSerializer.DeserializeAsync(utf8Json: text,
                                                              jsonTypeInfo: GoogleSerializationContext.Default.GoogleResponse,
                                                              cancellationToken: cancellationToken);

                    if (responseModel?.Id != null)
                    {
                        return new(responseModel.Id);
                    }
                }
            }
        }
        catch (Exception exception)
        {
            this.Logging.LogError(new(exception.HResult), exception: exception, $"Error: Could not build Short Url: {exception.Message}");
        }

        return fullUrl;
    }

    public static void Register(IServiceCollection serviceCollection)
    {
        serviceCollection.AddSingleton<IUrlShortener, Google>();

        RegisterHttpClientFactory(serviceCollection: serviceCollection,
                                  userAgent: "Credfeto.UrlShortner.Google",
                                  clientName: HTTP_CLIENT_NAME,
                                  new(uriString: @"https://www.googleapis.com"));
    }
}