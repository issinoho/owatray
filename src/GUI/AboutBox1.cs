// ------------------------------------------------------------------
//  DrunkenBakery OWA Tray Monitor
//  OWAtray.DrunkenBakery.OWAtray.GUI
//
//  <copyright file="AboutBox1.cs" company="The Drunken Bakery">
//      Copyright (c) 2009-2012 The Drunken Bakery. All rights reserved.
//  </copyright>
//
//  Author: IRS
// ------------------------------------------------------------------

namespace DrunkenBakery.OWAtray.GUI
{
    using System;
    using System.Windows.Forms;

    using DrunkenBakery.OWAtray.GUI.Properties;

    /// <summary>
    /// The about box 1.
    /// </summary>
    internal partial class AboutBox1 : Form
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="AboutBox1"/> class.
        /// </summary>
        public AboutBox1()
        {
            this.InitializeComponent();

            this.Text = string.Format("{0} {1}", Resources.AboutBox1_AboutBox1_About, AssemblyHelpers.AssemblyTitle);
            this.labelProductName.Text = string.Format(
                "{0} - {1}", AssemblyHelpers.AssemblyProduct, AssemblyHelpers.AssemblyTitle);
            this.labelVersion.Text = string.Format(
                "{0} {1}", Resources.AboutBox1_AboutBox1_Version, AssemblyHelpers.AssemblyVersion);
            this.labelCopyright.Text = AssemblyHelpers.AssemblyCopyright;
            this.labelCompanyName.Text = AssemblyHelpers.AssemblyCompany;
            this.textBoxDescription.Text = AssemblyHelpers.AssemblyDescription + Environment.NewLine
                                           + Environment.NewLine + Resources.AboutBox1_AboutBox1_Compiled_on__NET
                                           + AssemblyHelpers.DotNetRuntimeVersion + Environment.NewLine
                                           + Resources.AboutBox1_AboutBox1_Running_on__NET_v + Environment.Version
                                           + Environment.NewLine;

            foreach (string myRef in AssemblyHelpers.DependentAssemblies())
            {
                this.textBoxDescription.AppendText(Environment.NewLine + myRef);
            }
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets Text.
        /// </summary>
        public override sealed string Text
        {
            get
            {
                return base.Text;
            }

            set
            {
                base.Text = value;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// The ok button_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void OkButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        #endregion
    }
}