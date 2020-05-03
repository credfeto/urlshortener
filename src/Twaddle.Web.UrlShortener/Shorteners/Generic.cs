// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Generic.cs" company="Twaddle Software">
//   Copyright (c) Twaddle Software
// </copyright>
// <summary>
//   Generic URL Shortener.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#region Using Directives

using System;
using System.Diagnostics.Contracts;

#endregion

namespace Twaddle.Web.UrlShortener.Shorteners
{
    /// <summary>
    /// Generic URL Shortener.
    /// </summary>
    internal sealed class Generic : IUrlShortener
    {
        #region Constants and Fields

        private readonly Func<Uri, Uri> _shortener;

        #endregion

        #region Constructors and Destructors

        public Generic(Func<Uri, Uri> shortener)
        {
            Contract.Requires(shortener != null);
            Contract.Ensures(shortener == this._shortener);
            _shortener = shortener;
        }

        #endregion

        #region Implemented Interfaces

        #region IUrlShortener

        public Uri Shorten(Uri fullUrl)
        {
            return _shortener(fullUrl);
        }

        #endregion

        #endregion

        #region Methods

        [ContractInvariantMethod]
        private void ObjectInvariant()
        {
            Contract.Invariant(_shortener != null);
        }

        #endregion
    }
}