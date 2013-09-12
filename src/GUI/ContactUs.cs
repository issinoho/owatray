// ------------------------------------------------------------------
//  DrunkenBakery OWA Tray Monitor
//  OWAtray.DrunkenBakery.OWAtray.GUI
// 
//  <copyright file="ContactUs.cs" company="The Drunken Bakery">
//      Copyright (c) 2009-2012 The Drunken Bakery. All rights reserved.
//  </copyright>
// 
//  Author: IRS
// ------------------------------------------------------------------

namespace DrunkenBakery.OWAtray.GUI
{
    using System;
    using System.Diagnostics;
    using System.Windows.Forms;

    using DrunkenBakery.OWAtray.GUI.Properties;

    /// <summary>
    /// Provides contact information to the user.
    /// </summary>
    public partial class ContactUs : Form
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ContactUs"/> class.
        /// </summary>
        public ContactUs()
        {
            this.InitializeComponent();

            // Flags
            this.lvX.Items.Add(new ListViewItem(Resources.ContactUs_ContactUs_Catalan, 0));
            this.lvX.Items[this.lvX.Items.Count - 1].SubItems.Add("Daniel Sabater");
            this.lvX.Items.Add(new ListViewItem(Resources.ContactUs_ContactUs_German, 1));
            this.lvX.Items[this.lvX.Items.Count - 1].SubItems.Add("Christian Treudler");
            this.lvX.Items.Add(new ListViewItem(Resources.ContactUs_ContactUs_Spanish, 2));
            this.lvX.Items[this.lvX.Items.Count - 1].SubItems.Add("Daniel Sabater");
            this.lvX.Items.Add(new ListViewItem("Turkish", 3));
            this.lvX.Items[this.lvX.Items.Count - 1].SubItems.Add("pi511");
            this.lvX.Items.Add(new ListViewItem("French", 4));
            this.lvX.Items[this.lvX.Items.Count - 1].SubItems.Add("Marc Lairet");
            this.lvX.Items.Add(new ListViewItem("Italian", 5));
            this.lvX.Items[this.lvX.Items.Count - 1].SubItems.Add("Marco Procida");
            this.lvX.Items.Add(new ListViewItem("Russian", 6));
            this.lvX.Items[this.lvX.Items.Count - 1].SubItems.Add("Aleksandr Bembel");
            this.lvX.Items.Add(new ListViewItem("Polish", 7));
            this.lvX.Items[this.lvX.Items.Count - 1].SubItems.Add("Ryszard Ostrowski");
            this.lvX.Items.Add(new ListViewItem("Macedonian", 8));
            this.lvX.Items[this.lvX.Items.Count - 1].SubItems.Add("Igor Vojnoski");
        }

        #endregion

        #region Methods

        /// <summary>
        /// The cmd o k_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender. 
        /// </param>
        /// <param name="e">
        /// The e. 
        /// </param>
        private void CmdOkClick(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// The link bakery_ link clicked.
        /// </summary>
        /// <param name="sender">
        /// The sender. 
        /// </param>
        /// <param name="e">
        /// The e. 
        /// </param>
        private void LinkBakeryLinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start(this.linkBakery.Text);
        }

        /// <summary>
        /// The link email_ link clicked.
        /// </summary>
        /// <param name="sender">
        /// The sender. 
        /// </param>
        /// <param name="e">
        /// The e. 
        /// </param>
        private void LinkEmailLinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start(@"mailto:" + this.linkEmail.Text);
        }

        #endregion
    }
}