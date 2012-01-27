//------------------------------------------------------------------
// DrunkenBakery OWA Tray Monitor
// IEmailInterface Class
//
// <copyright file="IEmailInterface.cs" company="The Drunken Bakery">
//     Copyright (c) 2012 The Drunken Bakery. All rights reserved.
// </copyright>
//
// The contract against which all email connection types must adhere
//
//------------------------------------------------------------------

using System;
using DrunkenBakery.OWAtray.Logging;

namespace DrunkenBakery.OWAtray.Connections.Abstract
{
	public interface IEmailInterface
	{
		string ServerVersion { get; set; }

		bool OverrideCertificate { get; set; }

		bool AlwaysUseInternetExplorer { get; set; }

		bool DisableCalendar { get; set; }

		bool AutoLogin { get; set; }

		bool OverrideOffice365Login { get; set; }

		bool OverrideAutodiscoveryValidation { get; set; }

		bool UseAutodiscovery { get; set; }

		bool OnWindowsDomain { get; set; }

		string EmailServer { get; set; }

		string DiscoveredEmailServer { get; set; }

		string AccountDomain { get; set; }

		bool OverrideServiceUrl { get; set; }

		string ServiceUrl { get; set; }

		string DiscoveredServiceUrl { get; set; }

		string DerivedServiceUrl { get; set; }

		bool OverrideEmailUrl { get; set; }

		string EmailUrl { get; set; }

		string DiscoveredEmailUrl { get; set; }

		string DerivedEmailUrl { get; set; }

		string Username { get; set; }

		string DiscoveredUsername { get; set; }

		string Password { get; set; }

		string EncryptedPassword { get; set; }

		string EmailAddress { get; set; }

		string Version { get; }

		int Interval { get; set; }

		bool IsConnected { get; }

		bool IsLogEventDefined { get; }

		EmailType Type { get; }

		ConnectionState ConnectedState { get; }

		void Connect();

		void Connect(string email, string password);

		void Connect(string username, string email, string password);

		void ConnectA();

		void Disconnect();

		void DisconnectA();

		void Send(string subject, string recipient);

		void SendA(string subject, string recipient);

		// Events
		event Action<string, Severity> LogMessage;
		event Action<DateTime, string, string> NewMail;
		event Action<IEmailInterface, ConnectionState> ConnectedStateChange;
	}
}