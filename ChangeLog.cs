//------------------------------------------------------------------
// DrunkenBakery OWA Tray Monitor
// ChangeLog Form
//
// <copyright file="ContactUs.cs" company="The Drunken Bakery">
//     Copyright (c) 2009, 2010 The Drunken Bakery. All rights reserved.
// </copyright>
//
// Form to provide an RSS feed of changes scraped from the website.
//
//------------------------------------------------------------------
namespace DrunkenBakery.OWAtray
{
    using System;
    using System.ServiceModel.Syndication;
    using System.Windows.Forms;
    using System.Xml;

    /// <summary>
    /// Displays a Change Log scraped from an RSS Feed
    /// </summary>
    public partial class ChangeLog : Form
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ChangeLog"/> class.
        /// </summary>
        public ChangeLog(string rssUrl)
        {
            InitializeComponent();

            try
            {
                listBox1.Items.Clear();
                XmlReader reader = XmlReader.Create(rssUrl);
                SyndicationFeed feed = SyndicationFeed.Load(reader);

                foreach (SyndicationItem item in feed.Items)
                {
                    listBox1.Items.Add(item.Title.Text);
                    listBox1.Items.Add(item.PublishDate.ToString("dd MMMM yyyy, hh:mm:ss") + " | " + item.Authors[0].Email + " (" + item.Authors[0].Name + ")");
                    listBox1.Items.Add(string.Empty);
                }

                reader.Close();
            }
            catch (Exception)
            {
            }
        }

        #endregion Constructors

        #region Methods

        private void cmdOK_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        #endregion Methods
    }
}