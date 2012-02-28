//------------------------------------------------------------------
// DrunkenBakery OWA Tray Monitor
// GrowlHelper
//
// <copyright file="GrowlHelper.cs" company="The Drunken Bakery">
//     Copyright (c) 2009, 2010 The Drunken Bakery. All rights reserved.
// </copyright>
//
// Helper class for Growl
//
//------------------------------------------------------------------
using Growl.Connector;

namespace DrunkenBakery.OWAtray.Growl
{
	public static class GrowlHelper
	{
		private static string application;
		private static string notificationTitle;
		private static GrowlConnector simpleGrowl;

		public static void RegisterGrowl(string application, string iconPath, string notificationTitle, string notificationText)
		{
			GrowlHelper.application = application;
			GrowlHelper.notificationTitle = notificationTitle;
			simpleGrowl = new GrowlConnector();
			var thisApp = new Application(application) { Icon = iconPath };
			var simpleGrowlType = new NotificationType(notificationTitle, notificationText);
			simpleGrowl.Register(thisApp, new NotificationType[] { simpleGrowlType });
		}

		public static void PopGrowl(string title, string message = "")
		{
			var myGrowl = new Notification(application, notificationTitle, title, title, message);
			simpleGrowl.Notify(myGrowl);
		}
	}
}