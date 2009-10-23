//------------------------------------------------------------------
// Cygnet OWA Tray Monitor
// ContactUs Form
//
// <copyright file="ContactUs.cs" company="Cygnet Solutions Ltd">
//     Copyright (c) 2009 Cygnet Solutions Ltd. All rights reserved.
// </copyright>
//
// Form to provide contact information for Cygnet to the user.
//
// Author: IRS
// $Revision: 1.2 $
//------------------------------------------------------------------

namespace Cygnet.OWAtray
{
    using System;
    using System.Windows.Forms;

    /// <summary>
    /// Provides contact information for Cygnet to the user.
    /// </summary>
    public partial class ContactUs : Form
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ContactUs"/> class.
        /// </summary>
        public ContactUs()
        {
            InitializeComponent();
        }

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
        /// Handles the LinkClicked event of the linkCygnet control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.LinkLabelLinkClickedEventArgs"/> instance containing the event data.</param>
        private void linkCygnet_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(linkCygnet.Text);
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
    }
}
