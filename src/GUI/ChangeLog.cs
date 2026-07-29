// ------------------------------------------------------------------
//  DrunkenBakery OWA Tray Monitor
//  OWAtray.DrunkenBakery.OWAtray.GUI
//
//  <copyright file="ChangeLog.cs" company="The Drunken Bakery">
//      Copyright (c) 2009-2012 The Drunken Bakery. All rights reserved.
//  </copyright>
//
//  Author: IRS
// ------------------------------------------------------------------

namespace DrunkenBakery.OWAtray.GUI
{
    using System;
    using System.Net;
    using System.ServiceModel.Syndication;
    using System.Windows.Forms;
    using System.Xml;

    /// <summary>
    /// The change log.
    /// </summary>
    public partial class ChangeLog : Form
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ChangeLog"/> class.
        /// </summary>
        /// <param name="rssUrl">
        /// The rss url.
        /// </param>
        public ChangeLog(string rssUrl)
        {
            this.InitializeComponent();

            try
            {
                // GitHub (the feed's host as of this writing) only accepts TLS 1.2+. This project
                // targets .NET Framework 4.0, whose SecurityProtocolType enum predates Tls12 (added in
                // 4.5), so the symbolic name isn't available at compile time - the raw value (0x0C00)
                // still works against whatever .NET Framework is actually installed at runtime, which
                // on any current Windows 10/11 machine supports it regardless of this app's target.
                ServicePointManager.SecurityProtocol |= (SecurityProtocolType)0x0C00;

                this.listBox1.Items.Clear();
                XmlReader reader = XmlReader.Create(rssUrl);
                SyndicationFeed feed = SyndicationFeed.Load(reader);

                if (feed != null)
                {
                    foreach (SyndicationItem item in feed.Items)
                    {
                        this.listBox1.Items.Add(item.Title.Text);
                        this.listBox1.Items.Add(item.LastUpdatedTime.ToString("dd MMMM yyyy, hh:mm:ss"));
                        this.listBox1.Items.Add(string.Empty);
                    }
                }

                reader.Close();
            }
            catch (Exception)
            {
            }
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

        #endregion
    }
}