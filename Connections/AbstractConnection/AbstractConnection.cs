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
		private string _encryptedPassword;

		protected AbstractConnection()
		{
			Username = string.Empty;
			Password = string.Empty;
			EmailAddress = string.Empty;
			Type = EmailType.Exchange;
			ConnectedState = ConnectionState.Disconnected;
			Interval = 5;
		}

		#region IEmailInterface Members

		public int Interval { get; set; }

		public string Username { get; set; }

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
			get { return _encryptedPassword.Decrypt(); }
			set { _encryptedPassword = value.Encrypt(); }
		}

		public string EncryptedPassword
		{
			get { return _encryptedPassword; }
			set { _encryptedPassword = value; }
		}

		public virtual EmailType Type { get; set; }

		public virtual string Version { get; set; }

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
		public event Action<string, Severity> LogMessage;
		public event Action<DateTime, string, string> NewMail;

		public virtual event Action<IEmailInterface, ConnectionState> ConnectedStateChange
		{
			add { }
			remove { }
		}

		#endregion

		public virtual void RaiseLogMessage(string message)
		{
			RaiseLogMessage(message, Severity.Info);
		}

		public virtual void RaiseLogMessage(string message, Severity severity)
		{
			if (LogMessage != null) LogMessage(string.Format("[{0}] - {1}", EmailAddress, message), severity);
		}

		public virtual void RaiseNewMail(DateTime arrivalTime, string subject, string sender)
		{
			if (NewMail != null) NewMail(arrivalTime, subject, sender);
		}
	}
}