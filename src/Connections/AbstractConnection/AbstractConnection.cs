// ------------------------------------------------------------------
//  DrunkenBakery OWA Tray Monitor
//  OWAtray.DrunkenBakery.OWAtray.Connections.Abstract
// 
//  <copyright file="AbstractConnection.cs" company="The Drunken Bakery”>
//      Copyright (c) 2009-2012 The Drunken Bakery. All rights reserved.
//  </copyright>
// 
//  Author: IRS
// ------------------------------------------------------------------

namespace DrunkenBakery.OWAtray.Connections.Abstract
{
    using System;

    using DrunkenBakery.OWAtray.Connections.Abstract.Properties;
    using DrunkenBakery.OWAtray.Logging;

    /// <summary>
    /// The abstract connection.
    /// </summary>
    public abstract class AbstractConnection : IEmailInterface
    {
        #region Constants and Fields

        /// <summary>
        /// The _encrypted password.
        /// </summary>
        private string encryptedPassword;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="AbstractConnection"/> class.
        /// </summary>
        protected AbstractConnection()
        {
            this.Username = string.Empty;
            this.Password = string.Empty;
            this.EmailAddress = string.Empty;
            this.Type = EmailType.Exchange;
            this.ConnectedState = ConnectionState.Disconnected;
            this.Interval = 5;
            this.Description = string.Empty;
            this.EmailServer = string.Empty;
            this.AccountDomain = string.Empty;
            this.OverrideServiceUrl = false;
            this.ServiceUrl = string.Empty;
            this.OverrideEmailUrl = false;
            this.EmailUrl = string.Empty;
            this.UseAutodiscovery = true;
            this.OnWindowsDomain = false;
            this.OverrideCertificate = false;
            this.AlwaysUseInternetExplorer = true;
            this.DisableCalendar = false;
            this.AutoLogin = false;
            this.OverrideOffice365Login = false;
            this.OverrideAutodiscoveryValidation = true;
            this.ServerVersion = Resources.AbstractConnection_AbstractConnection_Default;
            this.DiscoveredEmailServer = string.Empty;
            this.DiscoveredEmailUrl = string.Empty;
            this.DiscoveredServiceUrl = string.Empty;
            this.DiscoveredUsername = string.Empty;
            this.DerivedServiceUrl = string.Empty;
            this.DerivedEmailUrl = string.Empty;
        }

        #endregion

        #region Public Events

        /// <summary>
        /// The connected state change.
        /// </summary>
        public virtual event Action<IEmailInterface, ConnectionState> ConnectedStateChange
        {
            add
            {
            }

            remove
            {
            }
        }

        /// <summary>
        /// The log exception.
        /// </summary>
        public event Action<string, Exception> LogException;

        /// <summary>
        /// The log message.
        /// </summary>
        public event Action<string, Severity> LogMessage;

        /// <summary>
        /// The message count.
        /// </summary>
        public event Action<int> MessageCount;

        /// <summary>
        /// The new appointment.
        /// </summary>
        public event Action<int, DateTime, string, string, string> NewAppointment;

        /// <summary>
        /// The new mail.
        /// </summary>
        public event Action<string, string, string> NewMail;

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets AccountDomain.
        /// </summary>
        public string AccountDomain { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether AlwaysUseInternetExplorer.
        /// </summary>
        public bool AlwaysUseInternetExplorer { get; set; }

        /// <summary>
        /// Gets a value indicating whether AreEventsDefined.
        /// </summary>
        public bool AreEventsDefined
        {
            get
            {
                return this.LogMessage != null;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether AutoLogin.
        /// </summary>
        public bool AutoLogin { get; set; }

        /// <summary>
        /// Gets or sets ConnectedState.
        /// </summary>
        public virtual ConnectionState ConnectedState { get; set; }

        /// <summary>
        /// Gets or sets DerivedEmailUrl.
        /// </summary>
        public string DerivedEmailUrl { get; set; }

        /// <summary>
        /// Gets or sets DerivedServiceUrl.
        /// </summary>
        public string DerivedServiceUrl { get; set; }

        /// <summary>
        /// Gets or sets Description.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether DisableCalendar.
        /// </summary>
        public bool DisableCalendar { get; set; }

        /// <summary>
        /// Gets or sets DiscoveredEmailServer.
        /// </summary>
        public string DiscoveredEmailServer { get; set; }

        /// <summary>
        /// Gets or sets DiscoveredEmailUrl.
        /// </summary>
        public string DiscoveredEmailUrl { get; set; }

        /// <summary>
        /// Gets or sets DiscoveredServiceUrl.
        /// </summary>
        public string DiscoveredServiceUrl { get; set; }

        /// <summary>
        /// Gets or sets DiscoveredUsername.
        /// </summary>
        public string DiscoveredUsername { get; set; }

        /// <summary>
        /// Gets or sets EmailAddress.
        /// </summary>
        public string EmailAddress { get; set; }

        /// <summary>
        /// Gets or sets EmailServer.
        /// </summary>
        public string EmailServer { get; set; }

        /// <summary>
        /// Gets or sets EmailUrl.
        /// </summary>
        public string EmailUrl { get; set; }

        /// <summary>
        /// Gets or sets EncryptedPassword.
        /// </summary>
        public string EncryptedPassword
        {
            get
            {
                return this.encryptedPassword;
            }

            set
            {
                this.encryptedPassword = value;
            }
        }

        /// <summary>
        /// Gets or sets Interval.
        /// </summary>
        public int Interval { get; set; }

        /// <summary>
        /// Gets a value indicating whether IsConnected.
        /// </summary>
        public bool IsConnected
        {
            get
            {
                return this.ConnectedState == ConnectionState.Connected;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether OnWindowsDomain.
        /// </summary>
        public bool OnWindowsDomain { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether OverrideAutodiscoveryValidation.
        /// </summary>
        public bool OverrideAutodiscoveryValidation { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether OverrideCertificate.
        /// </summary>
        public bool OverrideCertificate { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether OverrideEmailUrl.
        /// </summary>
        public bool OverrideEmailUrl { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether OverrideOffice365Login.
        /// </summary>
        public bool OverrideOffice365Login { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether OverrideServiceUrl.
        /// </summary>
        public bool OverrideServiceUrl { get; set; }

        /// <summary>
        /// Gets or sets Password.
        /// </summary>
        public string Password
        {
            get
            {
                return this.encryptedPassword.Decrypt();
            }

            set
            {
                this.encryptedPassword = value.Encrypt();
            }
        }

        /// <summary>
        /// Gets or sets ServerVersion.
        /// </summary>
        public string ServerVersion { get; set; }

        /// <summary>
        /// Gets or sets ServiceUrl.
        /// </summary>
        public string ServiceUrl { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether SupportsDirectMessageAccess.
        /// </summary>
        public virtual bool SupportsDirectMessageAccess { get; set; }

        /// <summary>
        /// Gets or sets Type.
        /// </summary>
        public virtual EmailType Type { get; set; }

        /// <summary>
        /// Gets or sets UnreadCount.
        /// </summary>
        public virtual int UnreadCount { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether UseAutodiscovery.
        /// </summary>
        public bool UseAutodiscovery { get; set; }

        /// <summary>
        /// Gets or sets Username.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// Gets or sets Version.
        /// </summary>
        public virtual string Version { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The connect.
        /// </summary>
        public abstract void Connect();

        /// <summary>
        /// The connect.
        /// </summary>
        /// <param name="email">
        /// The email.
        /// </param>
        /// <param name="password">
        /// The password.
        /// </param>
        public virtual void Connect(string email, string password)
        {
            this.EmailAddress = email;
            this.Password = password;
            this.Connect();
        }

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
        public virtual void Connect(string username, string email, string password)
        {
            this.Username = username;
            this.EmailAddress = email;
            this.Password = password;
            this.Connect();
        }

        /// <summary>
        /// The connect a.
        /// </summary>
        public abstract void ConnectA();

        /// <summary>
        /// The disconnect.
        /// </summary>
        public abstract void Disconnect();

        /// <summary>
        /// The disconnect a.
        /// </summary>
        public abstract void DisconnectA();

        /// <summary>
        /// The send.
        /// </summary>
        /// <param name="subject">
        /// The subject.
        /// </param>
        /// <param name="recipient">
        /// The recipient.
        /// </param>
        public abstract void Send(string subject, string recipient);

        /// <summary>
        /// The send a.
        /// </summary>
        /// <param name="subject">
        /// The subject.
        /// </param>
        /// <param name="recipient">
        /// The recipient.
        /// </param>
        public abstract void SendA(string subject, string recipient);

        #endregion

        #region Methods

        /// <summary>
        /// The raise exception.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="ex">
        /// The ex.
        /// </param>
        protected virtual void RaiseException(string message, Exception ex)
        {
            if (this.LogException != null)
            {
                this.LogException(string.Format("[{0}] - {1}", this.EmailAddress, message), ex);
            }
        }

        // Events

        /// <summary>
        /// The raise log message.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        protected virtual void RaiseLogMessage(string message)
        {
            this.RaiseLogMessage(message, Severity.Info);
        }

        /// <summary>
        /// The raise log message.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="ex">
        /// The ex.
        /// </param>
        protected virtual void RaiseLogMessage(string message, Exception ex)
        {
            this.RaiseException(message, ex);
        }

        /// <summary>
        /// The raise log message.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="severity">
        /// The severity.
        /// </param>
        protected virtual void RaiseLogMessage(string message, Severity severity)
        {
            if (this.LogMessage != null)
            {
                this.LogMessage(string.Format("[{0}] - {1}", this.EmailAddress, message), severity);
            }
        }

        /// <summary>
        /// The raise message count.
        /// </summary>
        /// <param name="count">
        /// The count.
        /// </param>
        protected virtual void RaiseMessageCount(int count)
        {
            if (this.MessageCount != null)
            {
                this.MessageCount(count);
            }
        }

        /// <summary>
        /// The raise new appointment.
        /// </summary>
        /// <param name="minsToGo">
        /// The mins to go.
        /// </param>
        /// <param name="startTime">
        /// The start time.
        /// </param>
        /// <param name="subject">
        /// The subject.
        /// </param>
        /// <param name="location">
        /// The location.
        /// </param>
        /// <param name="accessUrl">
        /// The access url.
        /// </param>
        protected virtual void RaiseNewAppointment(
            int minsToGo, DateTime startTime, string subject, string location, string accessUrl)
        {
            if (this.NewAppointment != null)
            {
                this.NewAppointment(minsToGo, startTime, subject, location, accessUrl);
            }
        }

        /// <summary>
        /// The raise new mail.
        /// </summary>
        /// <param name="arrivalTime">
        /// The arrival time.
        /// </param>
        /// <param name="subject">
        /// The subject.
        /// </param>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="accessUrl">
        /// The access url.
        /// </param>
        protected virtual void RaiseNewMail(DateTime arrivalTime, string subject, string sender, string accessUrl)
        {
            if (this.NewMail != null)
            {
                this.NewMail(subject, sender, accessUrl);
            }
        }

        #endregion
    }
}