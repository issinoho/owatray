// ------------------------------------------------------------------
//  OWAtray
//  DrunkenBakery.OWAtray.Tests.Connections
//
//  <copyright file="TestEmailConnection.cs" company="The Drunken Bakery">
//      Copyright (c) 2009-2026 The Drunken Bakery. All rights reserved.
//  </copyright>
// ------------------------------------------------------------------

namespace DrunkenBakery.OWAtray.Tests.Connections
{
    using DrunkenBakery.OWAtray.Connections.Abstract;

    /// <summary>
    /// A minimal, no-op <see cref="AbstractConnection"/> implementation, used to exercise the base
    /// class's own logic (defaults, encrypted password storage, connection state) without depending on
    /// a real mail provider.
    /// </summary>
    public class TestEmailConnection : AbstractConnection
    {
        public override void Connect()
        {
        }

        public override void ConnectA()
        {
        }

        public override void Disconnect()
        {
        }

        public override void DisconnectA()
        {
        }

        public override void Send(string subject, string recipient)
        {
        }

        public override void SendA(string subject, string recipient)
        {
        }
    }
}
