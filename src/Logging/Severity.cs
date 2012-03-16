//------------------------------------------------------------------
// DrunkenBakery OWA Tray Monitor
// Severity
//
// <copyright file="ContactUs.cs" company="The Drunken Bakery">
//     Copyright (c) 2009, 2010 The Drunken Bakery. All rights reserved.
// </copyright>
//
// Defines the logging levels supported by the application.
//
//------------------------------------------------------------------

using DrunkenBakery.OWAtray.Logging.Properties;

namespace DrunkenBakery.OWAtray.Logging
{
	public enum Severity
	{
		Success,
		Fail,
		Info
	}

	public static class EnumExtensions
	{
		public static string Description(this Severity e)
		{
			var state = "";

			switch (e)
			{
				case Severity.Success:
					state = Resources.EnumExtensions_Description_Success;
					break;
				case Severity.Fail:
					state = Resources.EnumExtensions_Description_Fail;
					break;
				case Severity.Info:
					state = Resources.EnumExtensions_Description_Info;
					break;
			}

			return state;
		}
	}
}