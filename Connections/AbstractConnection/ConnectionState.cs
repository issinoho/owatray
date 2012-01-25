//------------------------------------------------------------------
// DrunkenBakery OWA Tray Monitor
// ConnectionState Class
//
// <copyright file="ConnectionState.cs" company="The Drunken Bakery">
//     Copyright (c) 2012 The Drunken Bakery. All rights reserved.
// </copyright>
//
// Defines the discrete states that a connection must be in
//
//------------------------------------------------------------------

namespace DrunkenBakery.OWAtray.Connections.Abstract
{
	public enum ConnectionState
	{
		Disconnected,
		Connecting,
		Connected,
		Disconnecting
	}
}