//------------------------------------------------------------------
// DrunkenBakery OWA Tray Monitor
// EmailPayload Class
//
// <copyright file="EmailPayload.cs" company="The Drunken Bakery">
//     Copyright (c) 2012 The Drunken Bakery. All rights reserved.
// </copyright>
//
// Composite structure defining the properties of an email we are interested in
//
//------------------------------------------------------------------

namespace DrunkenBakery.OWAtray.Connections.Abstract
{
	public class EmailPayload
	{
		public string Subject { get; set; }

		public string Body { get; set; }

		public string Recipient { get; set; }
	}
}