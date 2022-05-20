using System.Diagnostics.CodeAnalysis;

namespace Credfeto.UrlShortener.Shorteners.Serialisation;

[SuppressMessage(category: "Microsoft.Performance", checkId: "CA1812:AvoidUninstantiatedInternalClasses", Justification = "Used by serialization")]
internal sealed class GoogleResponse
{
    public string? Id { get; set; }
}