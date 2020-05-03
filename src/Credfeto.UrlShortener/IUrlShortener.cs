using System;

namespace Credfeto.UrlShortener
{
    /// <summary>
    ///     URL Shortner interface.
    /// </summary>
    public interface IUrlShortener
    {
        /// <summary>
        ///     Shorten the url
        /// </summary>
        /// <param name="fullUrl">The url to shorten.</param>
        /// <returns>The shortened url.</returns>
        Uri Shorten(Uri fullUrl);
    }
}