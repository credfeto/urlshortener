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
            _output = output;
            _shortener = new Google();
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


            var shorterned = _shortener.Shorten(new Uri(originalUrl));
            _output.WriteLine(shorterned.ToString());
            Assert.NotEqual(originalUrl, shorterned.ToString());
            Assert.True(shorterned.ToString().StartsWith("http://goo.gl/", StringComparison.OrdinalIgnoreCase));
            Assert.True(shorterned.ToString().Length <= originalUrl.Length);
            Assert.Equal("http://goo.gl/M0LEn", shorterned.ToString());
        }
    }
}