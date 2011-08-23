//------------------------------------------------------------------
// DrunkenBakery OWA Tray Monitor
// MDACversions Form
//
// <copyright file="MDACversions.cs" company="The Drunken Bakery">
//     Copyright (c) 2009, 2010 The Drunken Bakery. All rights reserved.
// </copyright>
//
// Form to display the currently installed version(s) of MDAC.
// Uses the registry to get this information.
//
//------------------------------------------------------------------
namespace DrunkenBakery.OWAtray
{
    using System;
    using System.Windows.Forms;

    /// <summary>
    /// Reports on installed MDAC versions
    /// </summary>
    public partial class MDACversions : Form
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MDACversions"/> class.
        /// </summary>
        public MDACversions()
        {
            InitializeComponent();

            // Clear list
            lvStatus.Columns.Add(OWAtray.Major_Version, (lvStatus.Width / 2), HorizontalAlignment.Left);
            lvStatus.Columns.Add(OWAtray.Revision, (lvStatus.Width / 2) - 3, HorizontalAlignment.Left);
            lvStatus.Items.Clear();

            // Now get the versions from the reg
            ScrapeRegistry();
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// Adds an entry to the list of versions.
        /// </summary>
        /// <param name="newEntry">The new entry.</param>
        /// <param name="subEntry">The sub entry.</param>
        private void AddEntry(string newEntry, string subEntry)
        {
            ListViewItem itmX = null;

            itmX = new ListViewItem(newEntry, 0);
            lvStatus.Items.Add(itmX);
            int i = (lvStatus.Items.Count - 1);
            lvStatus.Items[i].SubItems.Add(subEntry);
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
        /// Scrapes the registry for .NET keys and lists them
        /// </summary>
        private void ScrapeRegistry()
        {
            Microsoft.Win32.RegistryKey regKey;

            regKey = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\DataAccess\", false);
            string verVal = (string)regKey.GetValue("Version");
            string revVal = (string)regKey.GetValue("FullInstallVer");
            AddEntry(verVal, revVal);
        }

        #endregion Methods
    }
}