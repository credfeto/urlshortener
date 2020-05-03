// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BitlyUrlShortener.cs" company="Twaddle Software">
//   Copyright (c) Twaddle Software
// </copyright>
// <summary>
//   The bitly url shortner tests.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#region Using Directives

using System;
using System.Diagnostics.CodeAnalysis;

using NUnit.Framework;
using Twaddle.Web.UrlShortener.Shorteners;

#endregion

namespace Twaddle.Web.UrlShortener.Tests
{
    /// <summary>
    /// The bitly url shortner tests.
    /// </summary>
    [TestFixture]
    [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Justification = "Bitly is name of site.")]
    public class BitlyUrlShortener
    {
        #region Public Methods

        /// <summary>
        /// The can shorten.
        /// </summary>
        [Test]
        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Unit Test of static method")]
        public void CanShorten()
        {
            const string originalUrl = "http://www.markridgwell.co.uk/";

            var shortner = new Bitly();

            var shorterned = shortner.Shorten(new Uri(originalUrl));
            Console.WriteLine(shorterned);
            Assert.AreNotEqual(originalUrl, shorterned.ToString());
            Assert.IsTrue(shorterned.ToString().StartsWith("http://bit.ly/", StringComparison.OrdinalIgnoreCase));
            Assert.LessOrEqual(shorterned.ToString().Length, originalUrl.Length);
            Assert.AreEqual("http://bit.ly/13b70Jk", shorterned.ToString());
        }

        #endregion
    }
}