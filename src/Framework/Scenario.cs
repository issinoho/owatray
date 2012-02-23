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
using System.Globalization;
using System.IO;
using System.Xml;
using DrunkenBakery.OWAtray.Connections.Abstract;
using DrunkenBakery.OWAtray.Connections.Proxy;

namespace DrunkenBakery.OWAtray.Framework
{
	public class Scenario
	{
		private const string GroupConnections = "Connections";
		private const string NodeConnection = "Connection";
		private const string RootElement = "Scenario";
		private const string SearchConnection = "//Connection";
		private const string ElementEmailServer = "EmailServer";
		private const string ElementAccountDomain = "AccountDomain";
		private const string ElementOverrideServiceUrl = "OverrideServiceUrl";
		private const string ElementServiceUrl = "ServiceUrl";
		private const string ElementOverrideEmailUrl = "OverrideEmailUrl";
		private const string ElementEmailUrl = "EmailUrl";
		private const string ElementEmailAddress = "EmailAddress";
		private const string ElementInterval = "Interval";
		private const string ElementPassword = "EncryptedPassword";
		private const string ElementType = "Type";
		private const string ElementUsername = "Username";
		private const string ElementUseAutodiscovery = "UseAutodiscovery";
		private const string ElementOnWindowsDomain = "OnWindowsDomain";
		private const string ElementOverrideCertificate = "OverrideCertificate";
		private const string ElementAlwaysUseInternetExplorer = "AlwaysUseInternetExplorer";
		private const string ElementDisableCalendar = "DisableCalendar";
		private const string ElementAutoLogin = "AutoLogin";
		private const string ElementOverrideOffice365Login = "OverrideOffice365Login";
		private const string ElementOverrideAutodiscoveryValidation = "OverrideAutodiscoveryValidation";
		private const string ElementServerVersion = "ServerVersion";

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
					writer.WriteElementString(ElementInterval, item.Interval.ToString(CultureInfo.InvariantCulture));
					writer.WriteElementString(ElementEmailServer, item.EmailServer);
					writer.WriteElementString(ElementAccountDomain, item.AccountDomain);
					writer.WriteElementString(ElementOverrideServiceUrl, item.OverrideServiceUrl ? "1" : "0");
					writer.WriteElementString(ElementServiceUrl, item.ServiceUrl);
					writer.WriteElementString(ElementOverrideEmailUrl, item.OverrideEmailUrl ? "1": "0");
					writer.WriteElementString(ElementEmailUrl, item.EmailUrl);
					writer.WriteElementString(ElementUseAutodiscovery, item.UseAutodiscovery ? "1" : "0");
					writer.WriteElementString(ElementOnWindowsDomain, item.OnWindowsDomain ? "1" : "0");
					writer.WriteElementString(ElementOverrideCertificate, item.OverrideCertificate ? "1" : "0");
					writer.WriteElementString(ElementAlwaysUseInternetExplorer, item.AlwaysUseInternetExplorer ? "1" : "0");
					writer.WriteElementString(ElementDisableCalendar, item.DisableCalendar ? "1" : "0");
					writer.WriteElementString(ElementAutoLogin, item.AutoLogin ? "1" : "0");
					writer.WriteElementString(ElementOverrideOffice365Login, item.OverrideOffice365Login ? "1" : "0");
					writer.WriteElementString(ElementOverrideAutodiscoveryValidation, item.OverrideAutodiscoveryValidation ? "1" : "0");
					writer.WriteElementString(ElementServerVersion, item.ServerVersion);
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
					item.Interval = Convert.ToInt32(node[ElementInterval].InnerText);
					item.EmailServer = node[ElementEmailServer].InnerText;
					item.AccountDomain = node[ElementAccountDomain].InnerText;
					item.OverrideServiceUrl = node[ElementOverrideServiceUrl].InnerText == "0" ? false : true;
					item.ServiceUrl = node[ElementServiceUrl].InnerText;
					item.OverrideEmailUrl = node[ElementOverrideEmailUrl].InnerText == "0" ? false : true;
					item.EmailUrl = node[ElementEmailUrl].InnerText;
					item.UseAutodiscovery = node[ElementUseAutodiscovery].InnerText == "0" ? false : true;
					item.OnWindowsDomain = node[ElementOnWindowsDomain].InnerText == "0" ? false : true;
					item.OverrideCertificate = node[ElementOverrideCertificate].InnerText == "0" ? false : true;
					item.AlwaysUseInternetExplorer = node[ElementAlwaysUseInternetExplorer].InnerText == "0" ? false : true;
					item.DisableCalendar = node[ElementDisableCalendar].InnerText == "0" ? false : true;
					item.AutoLogin = node[ElementAutoLogin].InnerText == "0" ? false : true;
					item.OverrideOffice365Login = node[ElementOverrideOffice365Login].InnerText == "0" ? false : true;
					item.OverrideAutodiscoveryValidation = node[ElementOverrideAutodiscoveryValidation].InnerText == "0" ? false : true;
					item.ServerVersion = node[ElementServerVersion].InnerText == "Autodetect" ? "Default" : node[ElementServerVersion].InnerText;
					Connections.Add(item);
				}
		}
	}
}