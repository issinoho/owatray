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
                this.listBox1.Items.Clear();
                XmlReader reader = XmlReader.Create(rssUrl);
                SyndicationFeed feed = SyndicationFeed.Load(reader);

                if (feed != null)
                {
                    foreach (SyndicationItem item in feed.Items)
                    {
                        this.listBox1.Items.Add(item.Title.Text);
                        this.listBox1.Items.Add(
                            item.PublishDate.ToString("dd MMMM yyyy, hh:mm:ss") + " | " + item.Authors[0].Email + " ("
                            + item.Authors[0].Name + ")");
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
        private void cmdOK_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        #endregion
    }
}