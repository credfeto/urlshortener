using System;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Credfeto.UrlShortener.Shorteners;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;
using Xunit;
using Xunit.Abstractions;

namespace Credfeto.UrlShortener.Tests;

public sealed class BitlyUrlShortenerShortenerTests : ShortenerTestBase
{
    private readonly IHttpClientFactory _httpClientFactory;

    private readonly ITestOutputHelper _output;

    private readonly IUrlShortener _shortener;

    public BitlyUrlShortenerShortenerTests(ITestOutputHelper output)
    {
        this._httpClientFactory = Substitute.For<IHttpClientFactory>();
        IOptions<BitlyConfiguration> options = Substitute.For<IOptions<BitlyConfiguration>>();
        options.Value.Returns(new BitlyConfiguration { ApiKey = "Mock", Login = "default" });
        this._output = output;
        this._shortener = new Bitly(httpClientFactory: this._httpClientFactory, options: options, Substitute.For<ILogger<Bitly>>());
    }

    [SuppressMessage(category: "Reliability", checkId: "CA2000:Dispose objects before losing scope", Justification = "For unit tests caller to dispose")]
    private void MockConnection()
    {
        this._httpClientFactory.CreateClient(nameof(Bitly))
            .Returns(Create(httpStatusCode: HttpStatusCode.OK, responseMessage: "https://bit.ly/fake"));
    }

    [Fact]
    public async Task CanShortenAsync()
    {
        this.MockConnection();

        const string originalUrl = "https://www.markridgwell.co.uk/";

        Uri shorterned = await this._shortener.ShortenAsync(new(originalUrl), cancellationToken: CancellationToken.None);
        this._output.WriteLine(shorterned.ToString());
        Assert.NotEqual(expected: originalUrl, shorterned.ToString());
        Assert.StartsWith(expectedStartString: "https://bit.ly/", shorterned.ToString(), comparisonType: StringComparison.OrdinalIgnoreCase);
        Assert.True(shorterned.ToString()
                              .Length <= originalUrl.Length,
                    userMessage: "Length should be less than original");
        Assert.Equal(expected: "https://bit.ly/fake", shorterned.ToString());
    }
}