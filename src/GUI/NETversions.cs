//------------------------------------------------------------------
// DrunkenBakery OWA Tray Monitor
// NETversions Form
//
// <copyright file="NETversions.cs" company="The Drunken Bakery">
//     Copyright (c) 2009, 2010 The Drunken Bakery. All rights reserved.
// </copyright>
//
// Form to display the currently installed .NET versions
// Uses the registry to access this information.
//
//------------------------------------------------------------------

using Microsoft.Win32;

namespace DrunkenBakery.OWAtray.GUI
{
	using System;
	using System.Windows.Forms;

	public partial class NeTversions : Form
	{
		public NeTversions()
		{
			InitializeComponent();

			// Clear list
			lvStatus.Columns.Add("Major Version", (lvStatus.Width / 2), HorizontalAlignment.Left);
			lvStatus.Columns.Add("Revision", (lvStatus.Width / 2) - 3, HorizontalAlignment.Left);
			lvStatus.Items.Clear();

			// Now get the versions from the reg
			ScrapeRegistry();
		}

		private void AddEntry(string newEntry, string subEntry)
		{
			ListViewItem itmX = null;

			itmX = new ListViewItem(newEntry, 0);
			lvStatus.Items.Add(itmX);
			var i = (lvStatus.Items.Count - 1);
			lvStatus.Items[i].SubItems.Add(subEntry);
		}

		private void cmdOK_Click(object sender, EventArgs e)
		{
			this.Close();
		}

		private void ScrapeRegistry()
		{
			var regKey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\NET Framework Setup\NDP\", false);
			if (regKey != null)
				foreach (var keyname in regKey.GetSubKeyNames())
				{
					var revKey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\NET Framework Setup\NDP\" + keyname + @"\", false);
					if (revKey == null) continue;
					var revVal = (string)revKey.GetValue("Version");
					AddEntry(keyname, revVal);
				}
		}
	}
}