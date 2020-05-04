using System;
using System.Collections.Generic;
using System.Linq;
using Credfeto.UrlShortener.Shorteners;

namespace Credfeto.UrlShortener
{
    /// <summary>
    ///     Factory for creating Url Shorteners.
    /// </summary>
    public sealed class ShortenerFactory : IShortenerFactory
    {
        private readonly IReadOnlyList<IUrlShortener> _shorteners;

        /// <summary>
        ///     Constructor.
        /// </summary>
        /// <param name="shorteners">Registered shorteners.</param>
        public ShortenerFactory(IEnumerable<IUrlShortener> shorteners)
        {
            this._shorteners = shorteners.ToArray();
        }

        /// <summary>
        ///     Creates an instance of the specified publisher.
        /// </summary>
        /// <param name="type">
        ///     The type of the publisher to create.
        /// </param>
        /// <returns>
        ///     The requested publisher if a matching one can be found; otherwise, <see langword="null" />.
        /// </returns>
        public IUrlShortener Create(string type)
        {
            return this._shorteners.FirstOrDefault(s => s.Name == type) ?? new Generic(DoNotShorten);
        }

        private static Uri DoNotShorten(Uri fullUrl)
        {
            return fullUrl;
        }
    }
}