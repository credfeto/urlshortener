// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IUrlShortener.cs" company="Twaddle Software">
//   Copyright (c) Twaddle Software
// </copyright>
// <summary>
//   URL Shortner interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#region Using Directives

using System;
using System.Diagnostics.Contracts;
using JetBrains.Annotations;
using Twaddle.Web.UrlShortener.ContractDefinitions;

#endregion

namespace Twaddle.Web.UrlShortener
{
    /// <summary>
    ///     URL Shortner interface.
    /// </summary>
    [ContractClass(typeof (UrlShortenerContracts))]
    public interface IUrlShortener
    {
        #region Public Methods

        [NotNull]
        Uri Shorten([NotNull] Uri fullUrl);

        #endregion
    }
}