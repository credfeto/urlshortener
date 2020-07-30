using Credfeto.UrlShortener.Shorteners;

namespace Credfeto.UrlShortener.Tests
{
    /// <summary>
    ///     The google url shortner tests.
    /// </summary>
    public class GoogleUrlShortener
    {
        private readonly ITestOutputHelper _output;

        private readonly IUrlShortener _shortener;

        public GoogleUrlShortener(ITestOutputHelper output)
        {
            IHttpClientFactory httpClientFactory = Substitute.For<IHttpClientFactory>();
            IOptions<GoogleConfiguration> options = Substitute.For<IOptions<GoogleConfiguration>>();
            options.Value.Returns(new GoogleConfiguration {ApiKey = "Mock"});
            this._output = output;
            this._shortener = new Google(httpClientFactory: httpClientFactory, options: options, Substitute.For<ILogger<Google>>());
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
            Assert.StartsWith(shorterned.ToString(), actualString: "http://goo.gl/", comparisonType: StringComparison.OrdinalIgnoreCase);
            Assert.True(shorterned.ToString()
                                  .Length <= originalUrl.Length);
            Assert.Equal(expected: "http://goo.gl/M0LEn", shorterned.ToString());
        }
    }
}