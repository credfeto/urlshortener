namespace Credfeto.UrlShortener.Shorteners;

/// <summary>
///     Configuration for the Bitly url shortnener.
/// </summary>
public sealed class BitlyConfiguration
{
    /// <summary>
    ///     The Api key.
    /// </summary>
    public string ApiKey { get; set; } = default!;

    /// <summary>
    ///     The login.
    /// </summary>
    public string Login { get; set; } = default!;
}