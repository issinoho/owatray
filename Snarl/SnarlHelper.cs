// -----------------------------------------------------------------------
// <copyright file="SnarlHelper.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

using Snarl;

namespace DrunkenBakery.OWAtray.Snarl
{
	using System;

	/// <summary>
	/// TODO: Update summary.
	/// </summary>
	public class SnarlHelper
	{
		private const Int32 ReplyMsg = 0x400 + 100;

		private static string _application;

		public static void RegisterSnarl(string application, string iconPath, IntPtr handle)
		{
			_application = application;
			SnarlConnector.RegisterConfig(handle, application, WindowsMessage.WM_MDIMAXIMIZE, iconPath);
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
