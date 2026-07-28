// ------------------------------------------------------------------
//  DrunkenBakery OWA Tray Monitor
//  OWAtray.DrunkenBakery.OWAtray.GUI
//
//  <copyright file="MDACversions.cs" company="The Drunken Bakery">
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

    using Microsoft.Win32;

    /// <summary>
    /// The mda cversions.
    /// </summary>
    public partial class MdaCversions : Form
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MdaCversions"/> class.
        /// </summary>
        public MdaCversions()
        {
            this.InitializeComponent();

            // Clear list
            this.lvStatus.Columns.Add(
                Resources.MdaCversions_MdaCversions_Major_Version, this.lvStatus.Width / 2, HorizontalAlignment.Left);
            this.lvStatus.Columns.Add(
                Resources.MdaCversions_MdaCversions_Revision, (this.lvStatus.Width / 2) - 3, HorizontalAlignment.Left);
            this.lvStatus.Items.Clear();

            // Now get the versions from the reg
            this.ScrapeRegistry();
        }

        #endregion

        #region Methods

        /// <summary>
        /// The add entry.
        /// </summary>
        /// <param name="newEntry">
        /// The new entry.
        /// </param>
        /// <param name="subEntry">
        /// The sub entry.
        /// </param>
        private void AddEntry(string newEntry, string subEntry)
        {
            ListViewItem itmX = null;

            itmX = new ListViewItem(newEntry, 0);
            this.lvStatus.Items.Add(itmX);
            int i = this.lvStatus.Items.Count - 1;
            this.lvStatus.Items[i].SubItems.Add(subEntry);
        }

        /// <summary>
        /// The scrape registry.
        /// </summary>
        private void ScrapeRegistry()
        {
            RegistryKey regKey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\DataAccess\", false);
            if (regKey == null)
            {
                return;
            }

            var verVal = (string)regKey.GetValue("Version");
            var revVal = (string)regKey.GetValue("FullInstallVer");
            this.AddEntry(verVal, revVal);
        }

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

        #endregion
    }
}