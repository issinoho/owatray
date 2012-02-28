using System;
using Snarl;

namespace DrunkenBakery.OWAtray.Snarl
{
	public static class SnarlHelper
	{
		private const Int32 ReplyMsg = 0x400 + 100;

		private static string application;

		public static void RegisterSnarl(string thisApplication, string iconPath, IntPtr handle)
		{
			SnarlHelper.application = thisApplication;
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