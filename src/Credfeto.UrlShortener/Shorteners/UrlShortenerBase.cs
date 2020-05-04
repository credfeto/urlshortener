using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Polly;

namespace Credfeto.UrlShortener.Shorteners
{
    public abstract class UrlShortenerBase
    {
        private readonly string _clientName;
        private readonly IHttpClientFactory _httpClientFactory;

        protected UrlShortenerBase(IHttpClientFactory httpClientFactory, string clientName, ILogger logging)
        {
            this._httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
            this._clientName = clientName ?? throw new ArgumentNullException(nameof(clientName));
            this.Logging = logging ?? throw new ArgumentNullException(nameof(logging));
        }

        protected ILogger Logging { get; }

        protected HttpClient CreateClient()
        {
            return this._httpClientFactory.CreateClient(this._clientName);
        }

        public static void RegisterHttpClientFactory(IServiceCollection serviceCollection, string userAgent, string clientName, Uri baseUri)
        {
            const int maxRetries = 3;

            void ConfigureClient(HttpClient httpClient)
            {
                httpClient.BaseAddress = baseUri;
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(mediaType: @"application/json"));
                httpClient.DefaultRequestHeaders.Add(name: "User-Agent", value: userAgent);
                httpClient.DefaultRequestHeaders.Add(name: "Cache-Control", value: "no-cache");
            }

            static TimeSpan Calculate(int attempts)
            {
                return attempts > 1 ? TimeSpan.FromSeconds(Math.Pow(x: 2.0, y: attempts)) : TimeSpan.Zero;
            }

            serviceCollection.AddHttpClient(clientName)
                             .ConfigureHttpClient(ConfigureClient)
                             .ConfigurePrimaryHttpMessageHandler(configureHandler: x => new HttpClientHandler {AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate})
                             .AddPolicyHandler(Policy.TimeoutAsync<HttpResponseMessage>(TimeSpan.FromSeconds(value: 30)))
                             .AddTransientHttpErrorPolicy(configurePolicy: p => p.WaitAndRetryAsync(retryCount: maxRetries, sleepDurationProvider: Calculate));
        }
    }
}