//------------------------------------------------------------------
// DrunkenBakery OWA Tray Monitor
// AboutBox Form
//
// <copyright file="AboutBox1.cs" company="The Drunken Bakery">
//     Copyright (c) 2009, 2010 The Drunken Bakery. All rights reserved.
// </copyright>
//
// Standard About box.
// Uses the Assembly details to populate the various fields.
// Also reports the name and version of all dependent assemblies.
//
//------------------------------------------------------------------

using DrunkenBakery.OWAtray.GUI.Properties;

namespace DrunkenBakery.OWAtray.GUI
{
	using System;
	using System.Windows.Forms;

	partial class AboutBox1 : Form
	{
		public AboutBox1()
		{
			this.InitializeComponent();

			Text = String.Format("{0} {1}", Resources.AboutBox1_AboutBox1_About, AssemblyHelpers.AssemblyTitle);
			labelProductName.Text = String.Format("{0} - {1}", AssemblyHelpers.AssemblyProduct, AssemblyHelpers.AssemblyTitle);
			labelVersion.Text = String.Format("{0} {1}", Resources.AboutBox1_AboutBox1_Version, AssemblyHelpers.AssemblyVersion);
			labelCopyright.Text = AssemblyHelpers.AssemblyCopyright;
			labelCompanyName.Text = AssemblyHelpers.AssemblyCompany;
			textBoxDescription.Text = AssemblyHelpers.AssemblyDescription +
									  Environment.NewLine +
									  Environment.NewLine +
									  Resources.AboutBox1_AboutBox1_Compiled_on__NET + AssemblyHelpers.DotNetRuntimeVersion +
									  Environment.NewLine +
									  Resources.AboutBox1_AboutBox1_Running_on__NET_v + Environment.Version +
									  Environment.NewLine;

			foreach (var myRef in AssemblyHelpers.DependentAssemblies())
			{
				textBoxDescription.AppendText(Environment.NewLine + myRef);
			}
		}

		public override sealed string Text
		{
			get { return base.Text; }
			set { base.Text = value; }
		}

		private void OkButton_Click(object sender, EventArgs e)
		{
			this.Close();
		}
	}
}