using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace Credfeto.UrlShortener.Shorteners.Serialisation;

[SuppressMessage(category: "ReSharper", checkId: "PartialTypeWithSinglePart", Justification = "Required for JsonSerializerContext")]
[JsonSourceGenerationOptions(GenerationMode = JsonSourceGenerationMode.Serialization | JsonSourceGenerationMode.Metadata)]
[JsonSerializable(typeof(GoogleResponse))]
[JsonSerializable(typeof(GoogleRequest))]
internal sealed partial class GoogleSerializationContext : JsonSerializerContext
{
}