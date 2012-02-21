//------------------------------------------------------------------
// DrunkenBakery OWA Tray Monitor
// ConnectionFactory Class
//
// <copyright file="ConnectionFactory.cs" company="The Drunken Bakery">
//     Copyright (c) 2012 The Drunken Bakery. All rights reserved.
// </copyright>
//
// Factory that knows how to build concrete connection classes
//
//------------------------------------------------------------------

using DrunkenBakery.OWAtray.Connections.Abstract;
using DrunkenBakery.OWAtray.Connections.EWS;

namespace DrunkenBakery.OWAtray.Connections.Proxy
{
	public static class ConnectionFactory
	{
		public static IEmailInterface CreateConnection(EmailType provider)
		{
			IEmailInterface connection = null;

			switch (provider)
			{
				case EmailType.Exchange:
					connection = new EwsConnection();
					break;
			}

			return connection;
		}
	}
}