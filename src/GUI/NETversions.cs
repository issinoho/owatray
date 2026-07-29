// ------------------------------------------------------------------
//  DrunkenBakery OWA Tray Monitor
//  OWAtray.DrunkenBakery.OWAtray.GUI
//
//  <copyright file="NETversions.cs" company="The Drunken Bakery">
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
    /// The ne tversions.
    /// </summary>
    public partial class NeTversions : Form
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="NeTversions"/> class.
        /// </summary>
        public NeTversions()
        {
            this.InitializeComponent();

            // Clear list
            this.lvStatus.Columns.Add(
                Resources.NeTversions_NeTversions_Major_Version, this.lvStatus.Width / 2, HorizontalAlignment.Left);
            this.lvStatus.Columns.Add(
                Resources.NeTversions_NeTversions_Revision, (this.lvStatus.Width / 2) - 3, HorizontalAlignment.Left);
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
            RegistryKey regKey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\NET Framework Setup\NDP\", false);
            if (regKey == null)
            {
                return;
            }

            foreach (string keyname in regKey.GetSubKeyNames())
            {
                // .NET Framework 4.5 and every version after it are in-place updates of .NET 4, not
                // separate installs - they never get their own top-level key here the way v2.0.50727,
                // v3.0, and v3.5 do. The actually-installed 4.x version instead lives under v4\Full's
                // "Release" value, which this code never looked at, so on any machine with 4.5+
                // installed (i.e. any current Windows 10/11 machine) this row used to show up blank.
                if (string.Equals(keyname, "v4", StringComparison.OrdinalIgnoreCase))
                {
                    this.AddNetFourEntries(regKey);
                    continue;
                }

                RegistryKey revKey = regKey.OpenSubKey(keyname, false);
                if (revKey == null)
                {
                    continue;
                }

                var revVal = revKey.GetValue("Version") as string;
                if (!string.IsNullOrEmpty(revVal))
                {
                    this.AddEntry(keyname, revVal);
                }
            }
        }

        /// <summary>
        /// Adds a row for each installed .NET 4.x profile (normally just "Full" - "Client" was
        /// dropped after 4.0 and only shows up on a machine that still has the standalone 4.0 Client
        /// Profile). The friendly 4.5+ version is derived from the numeric "Release" value using
        /// Microsoft's documented release-key ranges, since the profile's own "Version" string isn't
        /// reliably updated for in-place 4.5+ updates.
        /// </summary>
        /// <param name="ndpKey">
        /// The opened NDP registry key.
        /// </param>
        private void AddNetFourEntries(RegistryKey ndpKey)
        {
            string[] profiles = { "Full", "Client" };
            foreach (string profile in profiles)
            {
                RegistryKey profileKey = ndpKey.OpenSubKey(@"v4\" + profile, false);
                if (profileKey == null)
                {
                    continue;
                }

                object releaseValue = profileKey.GetValue("Release");
                string displayName = releaseValue != null
                    ? "v4 " + profile + " (" + GetFriendly45PlusVersion((int)releaseValue) + ")"
                    : "v4 " + profile;

                var version = profileKey.GetValue("Version") as string ?? string.Empty;
                this.AddEntry(displayName, version);
            }
        }

        /// <summary>
        /// Maps a .NET 4.5+ "Release" registry value to the friendly version it corresponds to, per
        /// https://learn.microsoft.com/dotnet/framework/migration-guide/how-to-determine-which-versions-are-installed.
        /// </summary>
        /// <param name="releaseKey">
        /// The release key.
        /// </param>
        /// <returns>
        /// The friendly version string.
        /// </returns>
        private static string GetFriendly45PlusVersion(int releaseKey)
        {
            if (releaseKey >= 533320)
            {
                return "4.8.1";
            }

            if (releaseKey >= 528040)
            {
                return "4.8";
            }

            if (releaseKey >= 461808)
            {
                return "4.7.2";
            }

            if (releaseKey >= 461308)
            {
                return "4.7.1";
            }

            if (releaseKey >= 460798)
            {
                return "4.7";
            }

            if (releaseKey >= 394802)
            {
                return "4.6.2";
            }

            if (releaseKey >= 394254)
            {
                return "4.6.1";
            }

            if (releaseKey >= 393295)
            {
                return "4.6";
            }

            if (releaseKey >= 379893)
            {
                return "4.5.2";
            }

            if (releaseKey >= 378675)
            {
                return "4.5.1";
            }

            if (releaseKey >= 378389)
            {
                return "4.5";
            }

            return "4.0";
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