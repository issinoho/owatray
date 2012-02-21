//------------------------------------------------------------------
// DrunkenBakery OWA Tray Monitor
// EmailConnections Class
//
// <copyright file="EmailConnections.cs" company="The Drunken Bakery">
//     Copyright (c) 2012 The Drunken Bakery. All rights reserved.
// </copyright>
//
// Generic list of all email connections
//
//------------------------------------------------------------------

using System.Collections.Generic;

namespace DrunkenBakery.OWAtray.Connections.Abstract
{
	public class EmailConnections : List<IEmailInterface>
	{
	}
}