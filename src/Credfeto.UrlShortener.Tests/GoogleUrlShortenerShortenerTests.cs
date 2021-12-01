using System;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Credfeto.UrlShortener.Shorteners;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;
using Xunit;
using Xunit.Abstractions;

namespace Credfeto.UrlShortener.Tests;

/// <summary>
///     The google url shortner tests.
/// </summary>
public sealed class GoogleUrlShortenerShortenerTests : ShortenerTestBase
{
    private readonly IHttpClientFactory _httpClientFactory;

    private readonly ITestOutputHelper _output;
    private readonly IUrlShortener _shortener;

    public GoogleUrlShortenerShortenerTests(ITestOutputHelper output)
    {
        this._httpClientFactory = Substitute.For<IHttpClientFactory>();
        IOptions<GoogleConfiguration> options = Substitute.For<IOptions<GoogleConfiguration>>();
        options.Value.Returns(new GoogleConfiguration { ApiKey = "Mock" });
        this._output = output;
        this._shortener = new Google(httpClientFactory: this._httpClientFactory, options: options, Substitute.For<ILogger<Google>>());
    }

    [SuppressMessage(category: "Reliability", checkId: "CA2000:Dispose objects before losing scope", Justification = "For unit tests caller to dispose")]
    private void MockConnection()
    {
        this._httpClientFactory.CreateClient("Google")
            .Returns(Create(httpStatusCode: HttpStatusCode.OK,
                            new { Id = "https://goo.gl/fake" },
                            new JsonSerializerOptions { DictionaryKeyPolicy = JsonNamingPolicy.CamelCase, PropertyNamingPolicy = JsonNamingPolicy.CamelCase }));
    }

    /// <summary>
    ///     The can shorten.
    /// </summary>
    [Fact]
    public async Task CanShortenAsync()
    {
        this.MockConnection();

        const string originalUrl = "https://www.markridgwell.co.uk/";

        Uri shorterned = await this._shortener.ShortenAsync(new Uri(originalUrl), cancellationToken: CancellationToken.None);
        this._output.WriteLine(shorterned.ToString());
        Assert.NotEqual(expected: originalUrl, shorterned.ToString());
        Assert.StartsWith(expectedStartString: "https://goo.gl/", shorterned.ToString(), comparisonType: StringComparison.OrdinalIgnoreCase);
        Assert.True(shorterned.ToString()
                              .Length <= originalUrl.Length,
                    userMessage: "Length should be less than the original");
        Assert.Equal(expected: "https://goo.gl/fake", shorterned.ToString());
    }
}