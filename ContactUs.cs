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
namespace DrunkenBakery.OWAtray
{
    using System;
    using System.Windows.Forms;

    /// <summary>
    /// Provides contact information to the user.
    /// </summary>
    public partial class ContactUs : Form
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ContactUs"/> class.
        /// </summary>
        public ContactUs()
        {
            InitializeComponent();

			// Flags
			lvX.Items.Add(new ListViewItem(OWAtray.Catalan, 0));
			lvX.Items[lvX.Items.Count - 1].SubItems.Add("Daniel Sabater");
			lvX.Items.Add(new ListViewItem(OWAtray.German, 1));
			lvX.Items[lvX.Items.Count - 1].SubItems.Add("Christian Treudler");
			lvX.Items.Add(new ListViewItem(OWAtray.Spanish, 2));
			lvX.Items[lvX.Items.Count - 1].SubItems.Add("Daniel Sabater");
		}

        #endregion Constructors

        #region Methods

        /// <summary>
        /// Handles the Click event of the cmdOK control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void cmdOK_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Handles the LinkClicked event of the linkBakery control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.LinkLabelLinkClickedEventArgs"/> instance containing the event data.</param>
        private void linkBakery_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(linkBakery.Text);
        }

        /// <summary>
        /// Handles the LinkClicked event of the linkEmail control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.LinkLabelLinkClickedEventArgs"/> instance containing the event data.</param>
        private void linkEmail_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(@"mailto:" + linkEmail.Text);
        }

        #endregion Methods
    }
}