using System;
using System.Threading;
using System.Threading.Tasks;

namespace Credfeto.UrlShortener;

/// <summary>
///     URL Shortener interface.
/// </summary>
public interface IUrlShortener
{
    /// <summary>
    ///     Name of the URL Shortener.
    /// </summary>
    string Name { get; }

    /// <summary>
    ///     Shorten the url.
    /// </summary>
    /// <param name="fullUrl">The url to shorten.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The shortened url.</returns>
    Task<Uri> ShortenAsync(Uri fullUrl, CancellationToken cancellationToken);
}