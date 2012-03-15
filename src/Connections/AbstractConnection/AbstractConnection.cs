//------------------------------------------------------------------
// DrunkenBakery OWA Tray Monitor
// AbstractConnection Class
//
// <copyright file="AbstractConnection.cs" company="The Drunken Bakery">
//     Copyright (c) 2012 The Drunken Bakery. All rights reserved.
// </copyright>
//
// Abstract class for all generic email connection types
//
//------------------------------------------------------------------

using System;
using DrunkenBakery.OWAtray.Logging;

namespace DrunkenBakery.OWAtray.Connections.Abstract
{
	public abstract class AbstractConnection : IEmailInterface
	{
		private string encryptedPassword;

		protected AbstractConnection()
		{
			Username = string.Empty;
			Password = string.Empty;
			EmailAddress = string.Empty;
			Type = EmailType.Exchange;
			ConnectedState = ConnectionState.Disconnected;
			Interval = 5;
			EmailServer = "";
			AccountDomain = "";
			OverrideServiceUrl = false;
			ServiceUrl = "";
			OverrideEmailUrl = false;
			EmailUrl = "";
			UseAutodiscovery = true;
			OnWindowsDomain = false;
			OverrideCertificate = false;
			AlwaysUseInternetExplorer = true;
			DisableCalendar = false;
			AutoLogin = false;
			OverrideOffice365Login = false;
			OverrideAutodiscoveryValidation = true;
			ServerVersion = "Default";
			DiscoveredEmailServer = "";
			DiscoveredEmailUrl = "";
			DiscoveredServiceUrl = "";
			DiscoveredUsername = "";
			DerivedServiceUrl = "";
			DerivedEmailUrl = "";
		}

		#region IEmailInterface Members

		public string ServerVersion { get; set; }

		public bool OverrideCertificate { get; set; }

		public bool AlwaysUseInternetExplorer { get; set; }

		public bool DisableCalendar { get; set; }

		public bool AutoLogin { get; set; }

		public bool OverrideOffice365Login { get; set; }

		public bool OverrideAutodiscoveryValidation { get; set; }

		public bool UseAutodiscovery { get; set; }

		public bool OnWindowsDomain { get; set; }

		public string EmailServer { get; set; }

		public string DiscoveredEmailServer { get; set; }

		public string AccountDomain { get; set; }

		public bool OverrideServiceUrl { get; set; }

		public string ServiceUrl { get; set; }

		public string DiscoveredServiceUrl { get; set; }

		public string DerivedServiceUrl { get; set; }

		public bool OverrideEmailUrl { get; set; }

		public string EmailUrl { get; set; }

		public string DiscoveredEmailUrl { get; set; }

		public string DerivedEmailUrl { get; set; }

		public int Interval { get; set; }

		public string Username { get; set; }

		public string DiscoveredUsername { get; set; }

		public string EmailAddress { get; set; }

		public bool IsConnected
		{
			get { return ConnectedState == ConnectionState.Connected; }
		}

		public bool IsLogEventDefined
		{
			get { return LogMessage != null; }
		}

		public string Password
		{
			get { return this.encryptedPassword.Decrypt(); }
			set { this.encryptedPassword = value.Encrypt(); }
		}

		public string EncryptedPassword
		{
			get { return this.encryptedPassword; }
			set { this.encryptedPassword = value; }
		}

		public virtual EmailType Type { get; set; }

		public virtual int UnreadCount { get; set; }

		public virtual string Version { get; set; }

		public virtual bool SupportsDirectMessageAccess { get; set; }

		public virtual ConnectionState ConnectedState { get; set; }

		public abstract void Connect();

		public abstract void ConnectA();

		public virtual void Connect(string email, string password)
		{
			EmailAddress = email;
			Password = password;
			Connect();
		}

		public virtual void Connect(string username, string email, string password)
		{
			Username = username;
			EmailAddress = email;
			Password = password;
			Connect();
		}

		public abstract void Disconnect();

		public abstract void DisconnectA();

		public abstract void Send(string subject, string recipient);

		public abstract void SendA(string subject, string recipient);

		// Events
		public event Action<int> MessageCount;
		public event Action<string, Severity> LogMessage;
		public event Action<string, DateTime, string, string, string> NewMail;
		public event Action<int, DateTime, string, string, string> NewAppointment;

		public virtual event Action<IEmailInterface, ConnectionState> ConnectedStateChange
		{
			add { }
			remove { }
		}

		#endregion IEmailInterface Members

		protected virtual void RaiseMessageCount(int count)
		{
			if (MessageCount != null) MessageCount(count);
		}

		protected virtual void RaiseLogMessage(string message)
		{
			RaiseLogMessage(message, Severity.Info);
		}

		protected virtual void RaiseLogMessage(string message, Severity severity)
		{
			if (LogMessage != null) LogMessage(string.Format("[{0}] - {1}", EmailAddress, message), severity);
		}

		protected virtual void RaiseNewMail(DateTime arrivalTime, string subject, string sender, string accessUrl)
		{
			if (NewMail != null) NewMail(EmailAddress, arrivalTime, subject, sender, accessUrl);
		}

		protected virtual void RaiseNewAppointment(int minsToGo, DateTime startTime, string subject, string location, string accessUrl)
		{
			if (NewAppointment != null) NewAppointment(minsToGo, startTime, subject, location, accessUrl);
		}
	}
}