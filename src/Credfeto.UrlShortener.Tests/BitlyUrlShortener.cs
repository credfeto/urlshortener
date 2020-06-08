using System;
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
    public sealed class BitlyUrlShortener
    {
        public BitlyUrlShortener(ITestOutputHelper output)
        {
            IHttpClientFactory httpClientFactory = Substitute.For<IHttpClientFactory>();
            IOptions<BitlyConfiguration> options = Substitute.For<IOptions<BitlyConfiguration>>();
            this._output = output;
            this._shortener = new Bitly(httpClientFactory: httpClientFactory, options: options, Substitute.For<ILogger<Bitly>>());
        }

        private readonly ITestOutputHelper _output;

        private readonly IUrlShortener _shortener;

        /// <summary>
        ///     The can shorten.
        /// </summary>
        [Fact]
        public async Task CanShortenAsync()
        {
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