namespace Credfeto.UrlShortener
{
    /// <summary>
    ///     Factory for creating Url Shorteners.
    /// </summary>
    public interface IShortenerFactory
    {
        /// <summary>
        ///     Creates an instance of the specified publisher.
        /// </summary>
        /// <param name="type">
        ///     The type of the publisher to create.
        /// </param>
        /// <returns>
        ///     The requested publisher if a matching one can be found; otherwise, <see langword="null" />.
        /// </returns>
        IUrlShortener Create(string type);
    }
}