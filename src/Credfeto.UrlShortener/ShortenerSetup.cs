using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;

namespace Credfeto.UrlShortener;

[ExcludeFromCodeCoverage]
[SuppressMessage(category: "ReSharper", checkId: "UnusedType.Global", Justification = "TODO: Ad Tests")]
public static class ShortenerSetup
{
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