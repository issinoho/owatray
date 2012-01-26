//------------------------------------------------------------------
// DrunkenBakery OWA Tray Monitor
// Scenario Class
//
// <copyright file="Scenario.cs" company="The Drunken Bakery">
//     Copyright (c) 2012 The Drunken Bakery. All rights reserved.
// </copyright>
//
// Models a complete scenario
//
//------------------------------------------------------------------

using System;
using System.IO;
using System.Xml;
using DrunkenBakery.OWAtray.Connections.Abstract;
using DrunkenBakery.OWAtray.Connections.Proxy;

namespace DrunkenBakery.OWAtray.Framework
{
	public class Scenario
	{
		private const string ElementEmailAddress = "EmailAddress";
		private const string ElementPassword = "EncryptedPassword";
		private const string ElementType = "Type";
		private const string ElementUsername = "Username";
		private const string GroupConnections = "Connections";
		private const string NodeConnection = "Connection";
		private const string RootElement = "Scenario";
		private const string SearchConnection = "//Connection";

		public string ScenarioFile { get; set; }

		public EmailConnections Connections { get; set; }

		public void Save()
		{
			Save(ScenarioFile);
		}

		public void Save(string filename)
		{
			using (var writer = XmlWriter.Create(filename))
			{
				writer.WriteStartDocument();
				writer.WriteStartElement(RootElement);

				// Connections
				writer.WriteStartElement(GroupConnections);
				foreach (var item in Connections)
				{
					writer.WriteStartElement(NodeConnection);
					writer.WriteElementString(ElementType, item.Type.ToString());
					writer.WriteElementString(ElementEmailAddress, item.EmailAddress);
					writer.WriteElementString(ElementUsername, item.Username);
					writer.WriteElementString(ElementPassword, item.EncryptedPassword);
					writer.WriteEndElement();
				}
				writer.WriteEndElement();

				writer.WriteEndElement();
				writer.WriteEndDocument();
			}
		}

		public void Load()
		{
			Load(ScenarioFile);
		}

		public void Load(string filename)
		{
			if (!File.Exists(filename)) return;

			// Clear out old data
			Connections.Clear();

			var doc = new XmlDocument();
			doc.Load(filename);

			// Connections
			var connections = doc.SelectNodes(SearchConnection);
			if (connections != null)
				foreach (XmlNode node in connections)
				{
					var item = ConnectionFactory.CreateConnection((EmailType) Enum.Parse(typeof (EmailType), node[ElementType].InnerText));
					item.EmailAddress = node[ElementEmailAddress].InnerText;
					item.EncryptedPassword = node[ElementPassword].InnerText;
					item.Username = node[ElementUsername].InnerText;
					Connections.Add(item);
				}
		}
	}
}