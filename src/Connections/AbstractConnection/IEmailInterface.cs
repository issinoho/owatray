// ------------------------------------------------------------------
//  DrunkenBakery OWA Tray Monitor
//  OWAtray.DrunkenBakery.OWAtray.Connections.Abstract
// 
//  <copyright file="IEmailInterface.cs" company="The Drunken Bakery”>
//      Copyright (c) 2009-2012 The Drunken Bakery. All rights reserved.
//  </copyright>
// 
//  Author: IRS
// ------------------------------------------------------------------

namespace DrunkenBakery.OWAtray.Connections.Abstract
{
    using System;

    using DrunkenBakery.OWAtray.Logging;

    /// <summary>
    /// The i email interface.
    /// </summary>
    public interface IEmailInterface
    {
        #region Public Events

        /// <summary>
        /// The connected state change.
        /// </summary>
        event Action<IEmailInterface, ConnectionState> ConnectedStateChange;

        /// <summary>
        /// The log exception.
        /// </summary>
        event Action<string, Exception> LogException;

        /// <summary>
        /// The log message.
        /// </summary>
        event Action<string, Severity> LogMessage;

        /// <summary>
        /// The message count.
        /// </summary>
        event Action<int> MessageCount;

        /// <summary>
        /// The new appointment.
        /// </summary>
        event Action<int, DateTime, string, string, string> NewAppointment;

        /// <summary>
        /// The new mail.
        /// </summary>
        event Action<string, string, string> NewMail;

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets AccountDomain.
        /// </summary>
        string AccountDomain { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether AlwaysUseInternetExplorer.
        /// </summary>
        bool AlwaysUseInternetExplorer { get; set; }

        /// <summary>
        /// Gets a value indicating whether AreEventsDefined.
        /// </summary>
        bool AreEventsDefined { get; }

        /// <summary>
        /// Gets or sets a value indicating whether AutoLogin.
        /// </summary>
        bool AutoLogin { get; set; }

        /// <summary>
        /// Gets ConnectedState.
        /// </summary>
        ConnectionState ConnectedState { get; }

        /// <summary>
        /// Gets or sets DerivedEmailUrl.
        /// </summary>
        string DerivedEmailUrl { get; set; }

        /// <summary>
        /// Gets or sets DerivedServiceUrl.
        /// </summary>
        string DerivedServiceUrl { get; set; }

        /// <summary>
        /// Gets or sets Description.
        /// </summary>
        string Description { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether DisableCalendar.
        /// </summary>
        bool DisableCalendar { get; set; }

        /// <summary>
        /// Gets or sets DiscoveredEmailServer.
        /// </summary>
        string DiscoveredEmailServer { get; set; }

        /// <summary>
        /// Gets or sets DiscoveredEmailUrl.
        /// </summary>
        string DiscoveredEmailUrl { get; set; }

        /// <summary>
        /// Gets or sets DiscoveredServiceUrl.
        /// </summary>
        string DiscoveredServiceUrl { get; set; }

        /// <summary>
        /// Gets or sets DiscoveredUsername.
        /// </summary>
        string DiscoveredUsername { get; set; }

        /// <summary>
        /// Gets or sets EmailAddress.
        /// </summary>
        string EmailAddress { get; set; }

        /// <summary>
        /// Gets or sets EmailServer.
        /// </summary>
        string EmailServer { get; set; }

        /// <summary>
        /// Gets or sets EmailUrl.
        /// </summary>
        string EmailUrl { get; set; }

        /// <summary>
        /// Gets or sets EncryptedPassword.
        /// </summary>
        string EncryptedPassword { get; set; }

        /// <summary>
        /// Gets or sets Interval.
        /// </summary>
        int Interval { get; set; }

        /// <summary>
        /// Gets a value indicating whether IsConnected.
        /// </summary>
        bool IsConnected { get; }

        /// <summary>
        /// Gets or sets a value indicating whether OnWindowsDomain.
        /// </summary>
        bool OnWindowsDomain { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether OverrideAutodiscoveryValidation.
        /// </summary>
        bool OverrideAutodiscoveryValidation { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether OverrideCertificate.
        /// </summary>
        bool OverrideCertificate { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether OverrideEmailUrl.
        /// </summary>
        bool OverrideEmailUrl { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether OverrideOffice365Login.
        /// </summary>
        bool OverrideOffice365Login { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether OverrideServiceUrl.
        /// </summary>
        bool OverrideServiceUrl { get; set; }

        /// <summary>
        /// Gets or sets Password.
        /// </summary>
        string Password { get; set; }

        /// <summary>
        /// Gets or sets ServerVersion.
        /// </summary>
        string ServerVersion { get; set; }

        /// <summary>
        /// Gets or sets ServiceUrl.
        /// </summary>
        string ServiceUrl { get; set; }

        /// <summary>
        /// Gets a value indicating whether SupportsDirectMessageAccess.
        /// </summary>
        bool SupportsDirectMessageAccess { get; }

        /// <summary>
        /// Gets Type.
        /// </summary>
        EmailType Type { get; }

        /// <summary>
        /// Gets UnreadCount.
        /// </summary>
        int UnreadCount { get; }

        /// <summary>
        /// Gets or sets a value indicating whether UseAutodiscovery.
        /// </summary>
        bool UseAutodiscovery { get; set; }

        /// <summary>
        /// Gets or sets Username.
        /// </summary>
        string Username { get; set; }

        /// <summary>
        /// Gets Version.
        /// </summary>
        string Version { get; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The connect.
        /// </summary>
        void Connect();

        /// <summary>
        /// The connect.
        /// </summary>
        /// <param name="email">
        /// The email.
        /// </param>
        /// <param name="password">
        /// The password.
        /// </param>
        void Connect(string email, string password);

        /// <summary>
        /// The connect.
        /// </summary>
        /// <param name="username">
        /// The username.
        /// </param>
        /// <param name="email">
        /// The email.
        /// </param>
        /// <param name="password">
        /// The password.
        /// </param>
        void Connect(string username, string email, string password);

        /// <summary>
        /// The connect a.
        /// </summary>
        void ConnectA();

        /// <summary>
        /// The disconnect.
        /// </summary>
        void Disconnect();

        /// <summary>
        /// The disconnect a.
        /// </summary>
        void DisconnectA();

        /// <summary>
        /// The send.
        /// </summary>
        /// <param name="subject">
        /// The subject.
        /// </param>
        /// <param name="recipient">
        /// The recipient.
        /// </param>
        void Send(string subject, string recipient);

        /// <summary>
        /// The send a.
        /// </summary>
        /// <param name="subject">
        /// The subject.
        /// </param>
        /// <param name="recipient">
        /// The recipient.
        /// </param>
        void SendA(string subject, string recipient);

        #endregion

        // Events
    }
}