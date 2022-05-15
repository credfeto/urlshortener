using System;
using System.Threading;
using System.Threading.Tasks;

namespace Credfeto.UrlShortener;

public interface IUrlShortener
{
    string Name { get; }

    Task<Uri> ShortenAsync(Uri fullUrl, CancellationToken cancellationToken);
}