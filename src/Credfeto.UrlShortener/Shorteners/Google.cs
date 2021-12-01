using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Credfeto.UrlShortener.Shorteners;

/// <summary>
///     Google's URL Shortener.
/// </summary>
/// <remarks>
///     Get free key from http://code.google.com/apis/console/ for up to 1000000 shortenings per day.
/// </remarks>
public sealed class Google : UrlShortenerBase, IUrlShortener
{
    private const string HTTP_CLIENT_NAME = nameof(Google);
    private readonly JsonSerializerOptions _jsonSerializerOptions;
    private readonly GoogleConfiguration _options;

    public Google(IHttpClientFactory httpClientFactory, IOptions<GoogleConfiguration> options, ILogger<Google> logger)
        : base(httpClientFactory: httpClientFactory, clientName: HTTP_CLIENT_NAME, logger: logger)

    {
        this._options = options.Value ?? throw new ArgumentNullException(nameof(options));

        this._jsonSerializerOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = false, PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
    }

    /// <inheritdoc />
    public string Name { get; } = HTTP_CLIENT_NAME;

    /// <inheritdoc />
    public async Task<Uri> ShortenAsync([NotNull] Uri fullUrl, CancellationToken cancellationToken)
    {
        try
        {
            HttpClient client = this.CreateClient();

            Uri uri = new("https://www.googleapis.com/urlshortener/v1/url?key=" + this._options.ApiKey);

            string requestJson = JsonSerializer.Serialize(new Request { LongUrl = fullUrl.ToString() }, options: this._jsonSerializerOptions);

            using (StringContent requestContent = new(content: requestJson, encoding: Encoding.UTF8, mediaType: "application/json"))
            {
                HttpResponseMessage response = await client.PutAsync(requestUri: uri, content: requestContent, cancellationToken: cancellationToken);

                if (!response.IsSuccessStatusCode)
                {
                    return fullUrl;
                }

                await using (Stream text = await response.Content.ReadAsStreamAsync(cancellationToken))
                {
                    Response? responseModel = await JsonSerializer.DeserializeAsync<Response>(utf8Json: text, options: this._jsonSerializerOptions, cancellationToken: cancellationToken);

                    if (responseModel?.Id != null)
                    {
                        return new Uri(responseModel.Id);
                    }
                }
            }
        }
        catch (Exception exception)
        {
            this.Logging.LogError(new EventId(exception.HResult), exception: exception, $"Error: Could not build Short Url: {exception.Message}");
        }

        return fullUrl;
    }

    public static void Register(IServiceCollection serviceCollection)
    {
        serviceCollection.AddSingleton<IUrlShortener, Google>();

        RegisterHttpClientFactory(serviceCollection: serviceCollection, userAgent: "Credfeto.UrlShortner.Google", clientName: HTTP_CLIENT_NAME, new Uri(uriString: @"https://www.googleapis.com"));
    }

    private sealed class Request
    {
        public string? LongUrl { get; set; }
    }

    [SuppressMessage(category: "Microsoft.Performance", checkId: "CA1812:AvoidUninstantiatedInternalClasses", Justification = "Used by serialization")]
    private sealed class Response
    {
        public string? Id { get; set; }
    }
}