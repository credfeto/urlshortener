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

namespace Credfeto.UrlShortener.Tests
{
    /// <summary>
    ///     The bitly url shortner tests.
    /// </summary>
    public sealed class BitlyUrlShortenerTests : TestBase
    {
        public BitlyUrlShortenerTests(ITestOutputHelper output)
        {
            this._httpClientFactory = Substitute.For<IHttpClientFactory>();
            IOptions<BitlyConfiguration> options = Substitute.For<IOptions<BitlyConfiguration>>();
            options.Value.Returns(new BitlyConfiguration {ApiKey = "Mock", Login = "default"});
            this._output = output;
            this._shortener = new Bitly(httpClientFactory: this._httpClientFactory, options: options, Substitute.For<ILogger<Bitly>>());
        }

        private readonly IHttpClientFactory _httpClientFactory;

        private readonly ITestOutputHelper _output;

        private readonly IUrlShortener _shortener;

        [SuppressMessage(category: "Reliability", checkId: "CA2000:Dispose objects before losing scope", Justification = "For unit tests caller to dispose")]
        private void MockConnection()
        {
            this._httpClientFactory.CreateClient("Bitly")
                .Returns(Create(httpStatusCode: HttpStatusCode.OK, responseMessage: "https://bit.ly/fake"));
        }

        /// <summary>
        ///     The can shorten.
        /// </summary>
        [Fact]
        public async Task CanShortenAsync()
        {
            this.MockConnection();

            const string originalUrl = "http://www.markridgwell.co.uk/";

            Uri shorterned = await this._shortener.ShortenAsync(new Uri(originalUrl), cancellationToken: CancellationToken.None);
            this._output.WriteLine(shorterned.ToString());
            Assert.NotEqual(expected: originalUrl, shorterned.ToString());
            Assert.StartsWith(shorterned.ToString(), actualString: "http://bit.ly/", comparisonType: StringComparison.OrdinalIgnoreCase);
            Assert.True(shorterned.ToString()
                                  .Length <= originalUrl.Length);
            Assert.Equal(expected: "http://bit.ly/13b70Jk", shorterned.ToString());
        }
    }
}