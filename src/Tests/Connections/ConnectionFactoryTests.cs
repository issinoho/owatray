// ------------------------------------------------------------------
//  OWAtray
//  DrunkenBakery.OWAtray.Tests.Connections
//
//  <copyright file="ConnectionFactoryTests.cs" company="The Drunken Bakery">
//      Copyright (c) 2009-2026 The Drunken Bakery. All rights reserved.
//  </copyright>
// ------------------------------------------------------------------

namespace DrunkenBakery.OWAtray.Tests.Connections
{
    using DrunkenBakery.OWAtray.Connections.Abstract;
    using DrunkenBakery.OWAtray.Connections.EWS;
    using DrunkenBakery.OWAtray.Connections.Proxy;

    using NUnit.Framework;

    [TestFixture]
    public class ConnectionFactoryTests
    {
        [Test]
        public void CreateConnection_Exchange_ReturnsEwsConnection()
        {
            var connection = ConnectionFactory.CreateConnection(EmailType.Exchange);

            Assert.IsNotNull(connection);
            Assert.IsInstanceOf<EwsConnection>(connection);
            Assert.AreEqual(EmailType.Exchange, connection.Type);
        }
    }
}
