// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ShortenerFactory.cs" company="Twaddle Software">
//   Copyright (c) Twaddle Software
// </copyright>
// <summary>
//   Factory for creating Url Shorteners.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#region Using Directives

using System;
using System.Diagnostics.Contracts;

using JetBrains.Annotations;
using Twaddle.Web.UrlShortener.Shorteners;

#endregion

namespace Twaddle.Web.UrlShortener
{
    /// <summary>
    /// Factory for creating Url Shorteners.
    /// </summary>
    public static class ShortenerFactory
    {
        #region Public Methods

        /// <summary>
        ///     Creates an instance of the specified publisher.
        /// </summary>
        /// <param name="type">
        ///     The type of the publisher to create.
        /// </param>
        /// <returns>
        ///     The requested publisher if a matching one can be found; otherwise, <see langword="null" />.
        /// </returns>
        [NotNull]
        public static IUrlShortener Create([NotNull] string type)
        {
            Contract.Requires(!string.IsNullOrEmpty(type));
            Contract.Ensures(Contract.Result<IUrlShortener>() != null);

            if (StringComparer.InvariantCultureIgnoreCase.Equals(type, "Bitly"))
            {
                return new Bitly();
            }

            if (StringComparer.InvariantCultureIgnoreCase.Equals(type, "Google"))
            {
                return new Google();
            }

            return new Generic(DoNotShorten);
        }

        #endregion

        #region Methods

        private static Uri DoNotShorten(Uri fullUrl)
        {
            Contract.Requires(fullUrl != null);
            Contract.Ensures(Contract.Result<Uri>() != null);
            Contract.Ensures(Contract.Result<System.Uri>() == fullUrl);

            return fullUrl;
        }

        #endregion
    }
}