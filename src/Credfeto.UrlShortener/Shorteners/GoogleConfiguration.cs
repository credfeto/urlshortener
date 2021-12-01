namespace Credfeto.UrlShortener.Shorteners;

/// <summary>
///     The Google url shortener configuration
/// </summary>
public sealed class GoogleConfiguration
{
    /// <summary>
    ///     The Api key.
    /// </summary>
    public string ApiKey { get; set; } = default!;
}