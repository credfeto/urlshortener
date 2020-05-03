﻿using System;
using System.Diagnostics.Contracts;

namespace Credfeto.UrlShortener.Shorteners
{
    /// <summary>
    ///     Generic URL Shortener.
    /// </summary>
    internal sealed class Generic : IUrlShortener
    {
        private readonly Func<Uri, Uri> _shortener;


        public Generic(Func<Uri, Uri> shortener)
        {
            Contract.Requires(shortener != null);
            Contract.Ensures(shortener == _shortener);
            _shortener = shortener;
        }


        public Uri Shorten(Uri fullUrl)
        {
            return _shortener(fullUrl);
        }
    }
}