using System;
using Credfeto.UrlShortener.Shorteners;
using Xunit;
using Xunit.Abstractions;

namespace Credfeto.UrlShortener.Tests
{
    /// <summary>
    ///     The google url shortner tests.
    /// </summary>
    public class GoogleUrlShortener
    {
        public GoogleUrlShortener(ITestOutputHelper output)
        {
            this._output = output;
            this._shortener = new Google();
        }

        private readonly ITestOutputHelper _output;

        private readonly IUrlShortener _shortener;

        /// <summary>
        ///     The can shorten.
        /// </summary>
        [Fact]
        public void CanShorten()
        {
            const string originalUrl = "http://www.markridgwell.co.uk/";

            Uri shorterned = this._shortener.ShortenAsync(new Uri(originalUrl));
            this._output.WriteLine(shorterned.ToString());
            Assert.NotEqual(expected: originalUrl, shorterned.ToString());
            Assert.True(shorterned.ToString()
                                  .StartsWith(value: "http://goo.gl/", comparisonType: StringComparison.OrdinalIgnoreCase));
            Assert.True(shorterned.ToString()
                                  .Length <= originalUrl.Length);
            Assert.Equal(expected: "http://goo.gl/M0LEn", shorterned.ToString());
        }
    }
}