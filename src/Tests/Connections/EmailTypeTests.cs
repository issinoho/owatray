// ------------------------------------------------------------------
//  OWAtray
//  DrunkenBakery.OWAtray.Tests.Connections
//
//  <copyright file="EmailTypeTests.cs" company="The Drunken Bakery">
//      Copyright (c) 2009-2026 The Drunken Bakery. All rights reserved.
//  </copyright>
// ------------------------------------------------------------------

namespace DrunkenBakery.OWAtray.Tests.Connections
{
    using DrunkenBakery.OWAtray.Connections.Abstract;

    using NUnit.Framework;

    [TestFixture]
    public class EmailTypeTests
    {
        [Test]
        public void Description_Exchange_ReturnsNonEmptyText()
        {
            Assert.AreEqual("Exchange", EmailType.Exchange.Description());
        }
    }
}
