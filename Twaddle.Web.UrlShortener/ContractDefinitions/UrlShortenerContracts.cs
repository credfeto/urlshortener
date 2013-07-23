// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UrlShortenerContracts.cs" company="Twaddle Software">
//   Copyright (c) Twaddle Software
// </copyright>
// <summary>
//   Contact for Url shortners.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#region Using Directives

using System;
using System.Diagnostics.Contracts;

using JetBrains.Annotations;

#endregion

namespace Twaddle.Web.UrlShortener.ContractDefinitions
{
    [ContractClassFor(typeof(IUrlShortener))]
    internal abstract class UrlShortenerContracts : IUrlShortener
    {
        #region Implemented Interfaces

        #region IUrlShortener

        [NotNull]
        public Uri Shorten([NotNull] Uri fullUrl)
        {
            Contract.Requires(fullUrl != null);
            Contract.Ensures(Contract.Result<Uri>() != null);

            throw new NotImplementedException();
        }

        #endregion

        #endregion
    }
}