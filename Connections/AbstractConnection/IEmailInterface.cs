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
		string Username { get; set; }

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