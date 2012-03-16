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

using DrunkenBakery.OWAtray.Connections.Abstract.Properties;

namespace DrunkenBakery.OWAtray.Connections.Abstract
{
	public enum ConnectionState
	{
		Disconnected,
		Connecting,
		Connected,
		Disconnecting,
		Failed
	}

	public static partial class EnumExtensions
	{
		public static string Description(this ConnectionState e)
		{
			var state = "";

			switch (e)
			{
				case ConnectionState.Disconnected:
					state = Resources.EnumExtensions_Description_Disconnected;
					break;
				case ConnectionState.Connecting:
					state = Resources.EnumExtensions_Description_Connecting;
					break;
				case ConnectionState.Connected:
					state = Resources.EnumExtensions_Description_Connected;
					break;
				case ConnectionState.Disconnecting:
					state = Resources.EnumExtensions_Description_Disconnecting;
					break;
				case ConnectionState.Failed:
					state = Resources.EnumExtensions_Description_Failed;
					break;
			}

			return state;
		}
	}
}