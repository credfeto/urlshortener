namespace Credfeto.UrlShortener.Tests
{
    /// <summary>
    ///     The bitly url shortner tests.
    /// </summary>
    public sealed class BitlyUrlShortener
    {
        private readonly ITestOutputHelper _output;

        private readonly IUrlShortener _shortener;

        public BitlyUrlShortener(ITestOutputHelper output)
        {
            IHttpClientFactory httpClientFactory = Substitute.For<IHttpClientFactory>();
            IOptions<BitlyConfiguration> options = Substitute.For<IOptions<BitlyConfiguration>>();
            options.Value.Returns(new BitlyConfiguration {ApiKey = "Mock", Login = "default"});
            this._output = output;
            this._shortener = new Bitly(httpClientFactory: httpClientFactory, options: options, Substitute.For<ILogger<Bitly>>());
        }

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