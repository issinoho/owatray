// ------------------------------------------------------------------
//  OWAtray
//  DrunkenBakery.OWAtray.Tests.Connections
//
//  <copyright file="ConnectionStateTests.cs" company="The Drunken Bakery">
//      Copyright (c) 2009-2026 The Drunken Bakery. All rights reserved.
//  </copyright>
// ------------------------------------------------------------------

namespace DrunkenBakery.OWAtray.Tests.Connections
{
    using DrunkenBakery.OWAtray.Connections.Abstract;

    using NUnit.Framework;

    [TestFixture]
    public class ConnectionStateTests
    {
        [TestCase(ConnectionState.Disconnected)]
        [TestCase(ConnectionState.Connecting)]
        [TestCase(ConnectionState.Connected)]
        [TestCase(ConnectionState.Disconnecting)]
        [TestCase(ConnectionState.Failed)]
        public void Description_EveryState_ReturnsNonEmptyText(ConnectionState state)
        {
            Assert.IsNotEmpty(state.Description());
        }
    }
}
