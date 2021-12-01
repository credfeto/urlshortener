using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;

namespace Credfeto.UrlShortener;

/// <summary>
///     URL Shortener registration
/// </summary>
[ExcludeFromCodeCoverage]
[SuppressMessage(category: "ReSharper", checkId: "UnusedType.Global", Justification = "TODO: Ad Tests")]
public static class ShortenerSetup
{
    /// <summary>
    ///     Configures URL Sorteners
    /// </summary>
    /// <param name="services"></param>
    /// <exception cref="ArgumentNullException"></exception>
    [SuppressMessage(category: "ReSharper", checkId: "UnusedMember.Global", Justification = "TODO: Ad Tests")]
    public static void Configure(IServiceCollection services)
    {
        if (services == null)
        {
            throw new ArgumentNullException(nameof(services));
        }

        services.AddSingleton<IShortenerFactory, ShortenerFactory>();
    }
}