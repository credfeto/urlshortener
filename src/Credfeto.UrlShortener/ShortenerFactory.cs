using System;
using System.Collections.Generic;
using System.Linq;
using Credfeto.UrlShortener.Shorteners;

namespace Credfeto.UrlShortener;

public sealed class ShortenerFactory : IShortenerFactory
{
    private readonly IReadOnlyList<IUrlShortener> _shorteners;

    public ShortenerFactory(IEnumerable<IUrlShortener> shorteners)
    {
        this._shorteners = shorteners.ToArray();
    }

    public IUrlShortener Create(string type)
    {
        return this._shorteners.FirstOrDefault(s => StringComparer.OrdinalIgnoreCase.Equals(x: s.Name, y: type)) ?? new Generic(DoNotShorten);
    }

    private static Uri DoNotShorten(Uri fullUrl)
    {
        return fullUrl;
    }
}