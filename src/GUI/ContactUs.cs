//------------------------------------------------------------------
// DrunkenBakery OWA Tray Monitor
// ContactUs Form
//
// <copyright file="ContactUs.cs" company="The Drunken Bakery">
//     Copyright (c) 2009, 2010 The Drunken Bakery. All rights reserved.
// </copyright>
//
// Form to provide contact information for Cygnet to the user.
//
//------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Windows.Forms;
using DrunkenBakery.OWAtray.GUI.Properties;

namespace DrunkenBakery.OWAtray.GUI
{
	/// <summary>
	/// Provides contact information to the user.
	/// </summary>
	public partial class ContactUs : Form
	{
		public ContactUs()
		{
			InitializeComponent();

			// Flags
			lvX.Items.Add(new ListViewItem(Resources.ContactUs_ContactUs_Catalan, 0));
			lvX.Items[lvX.Items.Count - 1].SubItems.Add("Daniel Sabater");
			lvX.Items.Add(new ListViewItem(Resources.ContactUs_ContactUs_German, 1));
			lvX.Items[lvX.Items.Count - 1].SubItems.Add("Christian Treudler");
			lvX.Items.Add(new ListViewItem(Resources.ContactUs_ContactUs_Spanish, 2));
			lvX.Items[lvX.Items.Count - 1].SubItems.Add("Daniel Sabater");
			lvX.Items.Add(new ListViewItem("Turkish", 3));
			lvX.Items[lvX.Items.Count - 1].SubItems.Add("pi511");
			lvX.Items.Add(new ListViewItem("French", 4));
			lvX.Items[lvX.Items.Count - 1].SubItems.Add("Marc Lairet");
			lvX.Items.Add(new ListViewItem("Italian", 5));
			lvX.Items[lvX.Items.Count - 1].SubItems.Add("Marco Procida");
			lvX.Items.Add(new ListViewItem("Russian", 6));
			lvX.Items[lvX.Items.Count - 1].SubItems.Add("Aleksandr Bembel");
		}

		private void cmdOK_Click(object sender, EventArgs e)
		{
			this.Close();
		}

		private void linkBakery_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			Process.Start(linkBakery.Text);
		}

		private void linkEmail_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			Process.Start(@"mailto:" + linkEmail.Text);
		}
	}
}