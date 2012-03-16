//------------------------------------------------------------------
// DrunkenBakery OWA Tray Monitor
// SnarlHelper
//
// <copyright file="SnarlHelper.cs" company="The Drunken Bakery">
//     Copyright (c) 2009, 2010 The Drunken Bakery. All rights reserved.
// </copyright>
//
// Helper class for Snarl
//
//------------------------------------------------------------------
using System;
using Snarl;

namespace DrunkenBakery.OWAtray.Snarl
{
	public static class SnarlHelper
	{
		private const Int32 ReplyMsg = 0x400 + 100;

		private static string _application;

		public static void RegisterSnarl(string thisApplication, string iconPath, IntPtr handle)
		{
			_application = thisApplication;
			SnarlConnector.RegisterConfig(handle, thisApplication, WindowsMessage.WM_MDIMAXIMIZE, iconPath);
		}

		public static void Revoke(IntPtr handle)
		{
			SnarlConnector.RevokeConfig(handle);
		}

		public static void PopSnarl(string myTitle, string myMessage, string iconPath, IntPtr handle)
		{
			SnarlConnector.ShowMessage(myTitle, myMessage, 10, iconPath, handle, (WindowsMessage)ReplyMsg);
		}
	}
}