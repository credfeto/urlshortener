// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GoogleUrlShortener.cs" company="Twaddle Software">
//   Copyright (c) Twaddle Software
// </copyright>
// <summary>
//   The google url shortner tests.
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
    ///     The google url shortner tests.
    /// </summary>
    [TestFixture]
    public class GoogleUrlShortener
    {
        /// <summary>
        ///     The can shorten.
        /// </summary>
        [Test]
        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic",
            Justification = "Unit Test of static method")]
        public void CanShorten()
        {
            const string originalUrl = "http://www.markridgwell.co.uk/";

            var shortner = new Google();

            Uri shorterned = shortner.Shorten(new Uri(originalUrl));
            Console.WriteLine(shorterned);
            Assert.AreNotEqual(originalUrl, shorterned.ToString());
            Assert.IsTrue(shorterned.ToString().StartsWith("http://goo.gl/", StringComparison.OrdinalIgnoreCase));
            Assert.LessOrEqual(shorterned.ToString().Length, originalUrl.Length);
            Assert.AreEqual("http://goo.gl/M0LEn", shorterned.ToString());
        }
    }
}