// ------------------------------------------------------------------
//  DrunkenBakery OWA Tray Monitor
//  OWAtray.DrunkenBakery.OWAtray.Framework
// 
//  <copyright file="Scenario.cs" company="The Drunken Bakery”>
//      Copyright (c) 2009-2012 The Drunken Bakery. All rights reserved.
//  </copyright>
// 
//  Author: IRS
// ------------------------------------------------------------------

namespace DrunkenBakery.OWAtray.Framework
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Xml;

    using DrunkenBakery.OWAtray.Connections.Abstract;
    using DrunkenBakery.OWAtray.Connections.Proxy;

    /// <summary>
    /// The scenario.
    /// </summary>
    public class Scenario
    {
        #region Constants and Fields

        /// <summary>
        /// The element account domain.
        /// </summary>
        private const string ElementAccountDomain = "AccountDomain";

        /// <summary>
        /// The element always use internet explorer.
        /// </summary>
        private const string ElementAlwaysUseInternetExplorer = "AlwaysUseInternetExplorer";

        /// <summary>
        /// The element auto login.
        /// </summary>
        private const string ElementAutoLogin = "AutoLogin";

        /// <summary>
        /// The element disable calendar.
        /// </summary>
        private const string ElementDisableCalendar = "DisableCalendar";

        /// <summary>
        /// The element email address.
        /// </summary>
        private const string ElementEmailAddress = "EmailAddress";

        /// <summary>
        /// The element email server.
        /// </summary>
        private const string ElementEmailServer = "EmailServer";

        /// <summary>
        /// The element email url.
        /// </summary>
        private const string ElementEmailUrl = "EmailUrl";

        /// <summary>
        /// The element interval.
        /// </summary>
        private const string ElementInterval = "Interval";

        /// <summary>
        /// The element on windows domain.
        /// </summary>
        private const string ElementOnWindowsDomain = "OnWindowsDomain";

        /// <summary>
        /// The element override autodiscovery validation.
        /// </summary>
        private const string ElementOverrideAutodiscoveryValidation = "OverrideAutodiscoveryValidation";

        /// <summary>
        /// The element override certificate.
        /// </summary>
        private const string ElementOverrideCertificate = "OverrideCertificate";

        /// <summary>
        /// The element override email url.
        /// </summary>
        private const string ElementOverrideEmailUrl = "OverrideEmailUrl";

        /// <summary>
        /// The element override office 365 login.
        /// </summary>
        private const string ElementOverrideOffice365Login = "OverrideOffice365Login";

        /// <summary>
        /// The element override service url.
        /// </summary>
        private const string ElementOverrideServiceUrl = "OverrideServiceUrl";

        /// <summary>
        /// The element password.
        /// </summary>
        private const string ElementPassword = "EncryptedPassword";

        /// <summary>
        /// The element server version.
        /// </summary>
        private const string ElementServerVersion = "ServerVersion";

        /// <summary>
        /// The element service url.
        /// </summary>
        private const string ElementServiceUrl = "ServiceUrl";

        /// <summary>
        /// The element type.
        /// </summary>
        private const string ElementType = "Type";

        /// <summary>
        /// The element use autodiscovery.
        /// </summary>
        private const string ElementUseAutodiscovery = "UseAutodiscovery";

        /// <summary>
        /// The element username.
        /// </summary>
        private const string ElementUsername = "Username";

        /// <summary>
        /// The group connections.
        /// </summary>
        private const string GroupConnections = "Connections";

        /// <summary>
        /// The node connection.
        /// </summary>
        private const string NodeConnection = "Connection";

        /// <summary>
        /// The root element.
        /// </summary>
        private const string RootElement = "Scenario";

        /// <summary>
        /// The search connection.
        /// </summary>
        private const string SearchConnection = "//Connection";

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets Connections.
        /// </summary>
        public EmailConnections Connections { get; set; }

        /// <summary>
        /// Gets or sets ScenarioFile.
        /// </summary>
        public string ScenarioFile { private get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The load.
        /// </summary>
        public void Load()
        {
            this.Load(this.ScenarioFile);
        }

        /// <summary>
        /// The load.
        /// </summary>
        /// <param name="filename">
        /// The filename.
        /// </param>
        public void Load(string filename)
        {
            if (!File.Exists(filename))
            {
                return;
            }

            // Clear out old data
            this.Connections.Clear();

            var doc = new XmlDocument();
            doc.Load(filename);

            // Connections
            XmlNodeList connections = doc.SelectNodes(SearchConnection);
            if (connections != null)
            {
                foreach (XmlNode node in connections)
                {
                    IEmailInterface item =
                        ConnectionFactory.CreateConnection(
                            (EmailType)Enum.Parse(typeof(EmailType), node[ElementType].InnerText));
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
                    item.AlwaysUseInternetExplorer = node[ElementAlwaysUseInternetExplorer].InnerText == "0"
                                                         ? false
                                                         : true;
                    item.DisableCalendar = node[ElementDisableCalendar].InnerText == "0" ? false : true;
                    item.AutoLogin = node[ElementAutoLogin].InnerText == "0" ? false : true;
                    item.OverrideOffice365Login = node[ElementOverrideOffice365Login].InnerText == "0" ? false : true;
                    item.OverrideAutodiscoveryValidation = node[ElementOverrideAutodiscoveryValidation].InnerText == "0"
                                                               ? false
                                                               : true;
                    item.ServerVersion = node[ElementServerVersion].InnerText == "Autodetect"
                                             ? "Default"
                                             : node[ElementServerVersion].InnerText;
                    this.Connections.Add(item);
                }
            }
        }

        /// <summary>
        /// The save.
        /// </summary>
        public void Save()
        {
            this.Save(this.ScenarioFile);
        }

        #endregion

        #region Methods

        /// <summary>
        /// The save.
        /// </summary>
        /// <param name="filename">
        /// The filename.
        /// </param>
        private void Save(string filename)
        {
            using (XmlWriter writer = XmlWriter.Create(filename))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement(RootElement);

                // Connections
                writer.WriteStartElement(GroupConnections);
                foreach (IEmailInterface item in this.Connections)
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
                    writer.WriteElementString(ElementOverrideEmailUrl, item.OverrideEmailUrl ? "1" : "0");
                    writer.WriteElementString(ElementEmailUrl, item.EmailUrl);
                    writer.WriteElementString(ElementUseAutodiscovery, item.UseAutodiscovery ? "1" : "0");
                    writer.WriteElementString(ElementOnWindowsDomain, item.OnWindowsDomain ? "1" : "0");
                    writer.WriteElementString(ElementOverrideCertificate, item.OverrideCertificate ? "1" : "0");
                    writer.WriteElementString(
                        ElementAlwaysUseInternetExplorer, item.AlwaysUseInternetExplorer ? "1" : "0");
                    writer.WriteElementString(ElementDisableCalendar, item.DisableCalendar ? "1" : "0");
                    writer.WriteElementString(ElementAutoLogin, item.AutoLogin ? "1" : "0");
                    writer.WriteElementString(ElementOverrideOffice365Login, item.OverrideOffice365Login ? "1" : "0");
                    writer.WriteElementString(
                        ElementOverrideAutodiscoveryValidation, item.OverrideAutodiscoveryValidation ? "1" : "0");
                    writer.WriteElementString(ElementServerVersion, item.ServerVersion);
                    writer.WriteEndElement();
                }

                writer.WriteEndElement();

                writer.WriteEndElement();
                writer.WriteEndDocument();
            }
        }

        #endregion
    }
}