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

using System;
using System.ServiceModel.Syndication;
using System.Windows.Forms;
using System.Xml;

namespace DrunkenBakery.OWAtray.GUI
{
	public partial class ChangeLog : Form
	{
		public ChangeLog(string rssUrl)
		{
			InitializeComponent();

			try
			{
				listBox1.Items.Clear();
				var reader = XmlReader.Create(rssUrl);
				var feed = SyndicationFeed.Load(reader);

				if (feed != null)
					foreach (var item in feed.Items)
					{
						listBox1.Items.Add(item.Title.Text);
						listBox1.Items.Add(item.PublishDate.ToString("dd MMMM yyyy, hh:mm:ss") + " | " + item.Authors[0].Email + " (" +
						                   item.Authors[0].Name + ")");
						listBox1.Items.Add(string.Empty);
					}

				reader.Close();
			}
			catch (Exception)
			{
			}
		}

		private void cmdOK_Click(object sender, EventArgs e)
		{
			Close();
		}
	}
}