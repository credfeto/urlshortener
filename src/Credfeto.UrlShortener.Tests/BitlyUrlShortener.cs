using System;
using Credfeto.UrlShortener.Shorteners;
using Xunit;
using Xunit.Abstractions;

namespace Credfeto.UrlShortener.Tests
{
    /// <summary>
    ///     The bitly url shortner tests.
    /// </summary>
    public class BitlyUrlShortener
    {
        public BitlyUrlShortener(ITestOutputHelper output)
        {
            this._output = output;
            this._shortener = new Bitly();
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

            Uri shorterned = this._shortener.Shorten(new Uri(originalUrl));
            this._output.WriteLine(shorterned.ToString());
            Assert.NotEqual(expected: originalUrl, shorterned.ToString());
            Assert.True(shorterned.ToString()
                                  .StartsWith(value: "http://bit.ly/", comparisonType: StringComparison.OrdinalIgnoreCase));
            Assert.True(shorterned.ToString()
                                  .Length <= originalUrl.Length);
            Assert.Equal(expected: "http://bit.ly/13b70Jk", shorterned.ToString());
        }
    }
}