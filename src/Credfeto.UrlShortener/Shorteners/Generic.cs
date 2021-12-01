using System;
using System.Threading;
using System.Threading.Tasks;

namespace Credfeto.UrlShortener.Shorteners;

/// <summary>
///     Generic URL Shortener.
/// </summary>
internal sealed class Generic : IUrlShortener
{
    private readonly Func<Uri, Uri> _shortener;

    public Generic(Func<Uri, Uri> shortener)
    {
        this._shortener = shortener;
    }

    /// <inheritdoc />
    public string Name { get; } = nameof(Generic);

    public Task<Uri> ShortenAsync(Uri fullUrl, CancellationToken cancellationToken)
    {
        return Task.FromResult(this._shortener(fullUrl));
    }
}