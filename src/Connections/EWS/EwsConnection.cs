// ------------------------------------------------------------------
//  DrunkenBakery OWA Tray Monitor
//  OWAtray.DrunkenBakery.OWAtray.Connections.EWS
// 
//  <copyright file="EwsConnection.cs" company="The Drunken Bakery">
//      Copyright (c) 2009-2012 The Drunken Bakery. All rights reserved.
//  </copyright>
// 
//  Author: IRS
// ------------------------------------------------------------------

namespace DrunkenBakery.OWAtray.Connections.EWS
{
    using System;
    using System.Linq;
    using System.Net;
    using System.Net.Security;
    using System.Security.Cryptography.X509Certificates;
    using System.Threading;

    using DrunkenBakery.OWAtray.Connections.Abstract;
    using DrunkenBakery.OWAtray.Connections.EWS.Properties;
    using DrunkenBakery.OWAtray.Logging;

    using Microsoft.Exchange.WebServices.Autodiscover;
    using Microsoft.Exchange.WebServices.Data;

    using Timer = System.Timers.Timer;

    /// <summary>
    /// The ews connection.
    /// </summary>
    public class EwsConnection : AbstractConnection
    {
        #region Constants and Fields

        /// <summary>
        /// The _appointment poll.
        /// </summary>
        private readonly Timer appointmentPoll = new Timer();

        /// <summary>
        /// The _background poll.
        /// </summary>
        private readonly Timer backgroundPoll = new Timer();

        /// <summary>
        /// The _locker.
        /// </summary>
        private readonly object locker = new object();

        /// <summary>
        /// The _mail count.
        /// </summary>
        private int mailCount = -1;

        /// <summary>
        /// The _service.
        /// </summary>
        private ExchangeService service;

        /// <summary>
        /// The _time last checked.
        /// </summary>
        private DateTime timeLastChecked;

        #endregion

        #region Public Events

        /// <summary>
        /// The connected state change.
        /// </summary>
        public override event Action<IEmailInterface, ConnectionState> ConnectedStateChange;

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets a value indicating whether SupportsDirectMessageAccess.
        /// </summary>
        public override bool SupportsDirectMessageAccess
        {
            get
            {
                return this.Version != ExchangeVersion.Exchange2007_SP1.ToString();
            }
        }

        /// <summary>
        /// Gets Type.
        /// </summary>
        public override EmailType Type
        {
            get
            {
                return EmailType.Exchange;
            }
        }

        /// <summary>
        /// Gets UnreadCount.
        /// </summary>
        public override int UnreadCount
        {
            get
            {
                int count = 0;
                try
                {
                    Folder myFolder = Folder.Bind(this.service, WellKnownFolderName.Inbox);
                    count = myFolder.UnreadCount;
                }
                catch
                {
                }

                return count;
            }
        }

        /// <summary>
        /// Gets Version.
        /// </summary>
        public override string Version
        {
            get
            {
                return !this.IsConnected
                           ? this.ServerVersion == Resources.EwsConnection_Version_Default
                                 ? ExchangeVersion.Exchange2007_SP1.ToString()
                                 : this.ServerVersion
                           : this.service.ServerInfo.VersionString;
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The connect.
        /// </summary>
        public override void Connect()
        {
            lock (this.locker)
            {
                // Check input
                if (this.EmailAddress.Length == 0)
                {
                    this.RaiseLogMessage(
                        Resources.EwsConnection_Connect_Please_provide_a_valid_Email_Address, Severity.Fail);
                    return;
                }

                if (!this.OnWindowsDomain && this.Password.Length == 0)
                {
                    this.RaiseLogMessage(Resources.EwsConnection_Connect_Please_provide_a_Password, Severity.Fail);
                    return;
                }

                if (!this.UseAutodiscovery)
                {
                    if (this.DerivedServiceUrl.Length == 0)
                    {
                        this.RaiseLogMessage(
                            Resources.EwsConnection_Connect_Please_provide_a_valid_Server_Address, Severity.Fail);
                        return;
                    }
                }

                try
                {
                    // State
                    this.ChangeState(ConnectionState.Connecting);

                    // Validate the server certificate
                    ServicePointManager.ServerCertificateValidationCallback = this.CertificateValidationCallBack;

                    // Define service
                    this.service = this.ServerVersion == Resources.EwsConnection_Version_Default
                                       ? new ExchangeService()
                                       : new ExchangeService(
                                             (ExchangeVersion)Enum.Parse(typeof(ExchangeVersion), this.ServerVersion));

                    // Enable Tracing (if required)
                    if (Settings.Default.UseTracing)
                    {
                        this.service.TraceListener = new EwsTraceListener();
                        this.service.TraceFlags = TraceFlags.EwsRequest | TraceFlags.EwsResponse;
                        this.service.TraceEnabled = true;
                    }

                    // Are we on a Windows domain?
                    this.service.UseDefaultCredentials = this.OnWindowsDomain;

                    if (!this.OnWindowsDomain)
                    {
                        this.service.Credentials = this.AccountDomain.Length > 0
                                                       ? new WebCredentials(
                                                             this.Username.Length == 0
                                                                 ? this.EmailAddress
                                                                 : this.Username, 
                                                             this.Password, 
                                                             this.AccountDomain)
                                                       : new WebCredentials(
                                                             this.Username.Length == 0
                                                                 ? this.EmailAddress
                                                                 : this.Username, 
                                                             this.Password);
                    }

                    // Connect using Autodiscover?
                    if (this.UseAutodiscovery)
                    {
                        if (this.OverrideAutodiscoveryValidation)
                        {
                            this.service.AutodiscoverUrl(this.EmailAddress, delegate { return true; });
                        }
                        else
                        {
                            this.service.AutodiscoverUrl(this.EmailAddress);
                        }

                        // Probe for autodiscover information
                        // var autodiscoverService = new AutodiscoverService((ExchangeVersion)Enum.Parse(typeof(ExchangeVersion), Version));
                        var autodiscoverService = new AutodiscoverService(ExchangeVersion.Exchange2007_SP1);

                        // Credentials
                        if (this.OnWindowsDomain)
                        {
                            autodiscoverService.UseDefaultCredentials = true;
                        }
                        else
                        {
                            autodiscoverService.Credentials = this.AccountDomain.Length > 0
                                                                  ? new WebCredentials(
                                                                        this.Username.Length == 0
                                                                            ? this.EmailAddress
                                                                            : this.Username, 
                                                                        this.Password, 
                                                                        this.AccountDomain)
                                                                  : new WebCredentials(
                                                                        this.Username.Length == 0
                                                                            ? this.EmailAddress
                                                                            : this.Username, 
                                                                        this.Password);
                        }

                        // Redirection Callback
                        if (this.OverrideAutodiscoveryValidation)
                        {
                            autodiscoverService.RedirectionUrlValidationCallback = delegate { return true; };
                        }

                        // Is this Internal or External ?
                        if (autodiscoverService.IsExternal == false)
                        {
                            // Probe for values
                            GetUserSettingsResponse userresponse = autodiscoverService.GetUserSettings(
                                this.EmailAddress, 
                                UserSettingName.InternalWebClientUrls, 
                                UserSettingName.InternalEwsUrl, 
                                UserSettingName.InternalMailboxServer, 
                                UserSettingName.UserDisplayName);

                            // OWA Url
                            WebClientUrlCollection webCollection;
                            if (userresponse.TryGetSettingValue(
                                UserSettingName.InternalWebClientUrls, out webCollection))
                            {
                                foreach (WebClientUrl url in webCollection.Urls)
                                {
                                    this.DiscoveredEmailUrl = url.Url;
                                }
                            }

                            // EWS Url
                            string internalUrl;
                            if (userresponse.TryGetSettingValue(UserSettingName.InternalEwsUrl, out internalUrl))
                            {
                                this.DiscoveredServiceUrl = internalUrl;
                            }

                            // Server
                            string internalServer;
                            if (userresponse.TryGetSettingValue(
                                UserSettingName.InternalMailboxServer, out internalServer))
                            {
                                this.DiscoveredEmailServer = internalServer;
                            }

                            // User Name
                            string userName;
                            if (userresponse.TryGetSettingValue(UserSettingName.UserDisplayName, out userName))
                            {
                                this.DiscoveredUsername = userName;
                            }
                        }
                        else
                        {
                            // Probe for values
                            GetUserSettingsResponse userresponse = autodiscoverService.GetUserSettings(
                                this.EmailAddress, 
                                UserSettingName.ExternalWebClientUrls, 
                                UserSettingName.ExternalEwsUrl, 
                                UserSettingName.ExternalMailboxServer, 
                                UserSettingName.UserDisplayName);

                            // OWA Url
                            WebClientUrlCollection webCollection;
                            if (userresponse.TryGetSettingValue(
                                UserSettingName.ExternalWebClientUrls, out webCollection))
                            {
                                foreach (WebClientUrl url in webCollection.Urls)
                                {
                                    this.DiscoveredEmailUrl = url.Url;
                                }
                            }

                            // EWS Url
                            string externalUrl;
                            if (userresponse.TryGetSettingValue(UserSettingName.ExternalEwsUrl, out externalUrl))
                            {
                                this.DiscoveredServiceUrl = externalUrl;
                            }

                            // Server
                            string externalServer;
                            if (userresponse.TryGetSettingValue(
                                UserSettingName.ExternalMailboxServer, out externalServer))
                            {
                                this.DiscoveredEmailServer = externalServer;
                            }

                            // User Name
                            string userName;
                            if (userresponse.TryGetSettingValue(UserSettingName.UserDisplayName, out userName))
                            {
                                this.DiscoveredUsername = userName;
                            }
                        }
                    }
                    else
                    {
                        if (this.DerivedServiceUrl.Length > 0)
                        {
                            var myUri = new Uri(this.DerivedServiceUrl);
                            this.service.Url = myUri;

                            // Update properties
                            this.DiscoveredEmailServer = this.EmailServer;
                            this.DiscoveredUsername = this.OnWindowsDomain
                                                          ? string.Empty
                                                          : (this.Username.Length == 0
                                                                 ? this.EmailAddress
                                                                 : this.Username);
                            this.DiscoveredServiceUrl = this.DerivedServiceUrl;
                            this.DiscoveredEmailUrl = this.DerivedEmailUrl;
                        }
                    }

                    // Get initial timestamp
                    this.timeLastChecked = this.TimeOfNewestEmail().AddSeconds(1);

                    // Initial Message
                    int count = this.UnreadCount;
                    this.RaiseMessageCount(count);

                    // Timers
                    this.backgroundPoll.Interval = this.Interval * 1000;
                    this.backgroundPoll.Elapsed += this.BackgroundPollElapsed;
                    this.backgroundPoll.Start();
                    this.appointmentPoll.Interval = Settings.Default.ApptInterval * 1000;
                    this.appointmentPoll.Elapsed += this.AppointmentPollElapsed;
                    this.appointmentPoll.Start();

                    // Set timeout
                    this.service.Timeout = (this.Interval * 1000) - 500;

                    // Initial check
                    this.mailCount = -1;
                    this.CheckForNewMailA();
                    if (!this.DisableCalendar)
                    {
                        this.CheckForNewAppointmentA();
                    }

                    // State
                    this.ChangeState(ConnectionState.Connected);
                }
                catch (Exception ex)
                {
                    this.RaiseLogMessage(ex.Message, ex);
                    this.ChangeState(ConnectionState.Failed);
                }
            }
        }

        /// <summary>
        /// The connect a.
        /// </summary>
        public override void ConnectA()
        {
            new Thread(this.Connect).Start();
        }

        /// <summary>
        /// The disconnect.
        /// </summary>
        public override void Disconnect()
        {
            if (!this.IsConnected)
            {
                return;
            }

            try
            {
                this.ChangeState(ConnectionState.Disconnecting);
                this.backgroundPoll.Stop();
                this.backgroundPoll.Elapsed -= this.BackgroundPollElapsed;
                this.appointmentPoll.Stop();
                this.appointmentPoll.Elapsed -= this.AppointmentPollElapsed;
                this.service = null;
                this.ChangeState(ConnectionState.Disconnected);
            }
            catch
            {
            }
        }

        /// <summary>
        /// The disconnect a.
        /// </summary>
        public override void DisconnectA()
        {
            new Thread(this.Disconnect).Start();
        }

        /// <summary>
        /// The send.
        /// </summary>
        /// <param name="subject">
        /// The subject. 
        /// </param>
        /// <param name="recipient">
        /// The recipient. 
        /// </param>
        public override void Send(string subject, string recipient)
        {
            lock (this.locker)
            {
                if (!this.IsConnected)
                {
                    this.RaiseLogMessage(
                        Resources.EwsConnection_Send_Unable_to_send__Email_provider_is_disconnected_, Severity.Fail);
                    return;
                }

                if (recipient.Length == 0)
                {
                    this.RaiseLogMessage(
                        Resources.EwsConnection_Send_Unable_to_send__No_recipient_specified_, Severity.Fail);
                    return;
                }

                try
                {
                    var message = new EmailMessage(this.service)
                        {
                           Subject = subject, Body = Resources.EwsConnection_Send_OWAtray_Test_Message 
                        };
                    message.ToRecipients.Add(recipient);
                    message.Send();
                }
                catch (Exception ex)
                {
                    this.RaiseLogMessage(ex.Message, ex);
                }
            }
        }

        /// <summary>
        /// The send a.
        /// </summary>
        /// <param name="subject">
        /// The subject. 
        /// </param>
        /// <param name="recipient">
        /// The recipient. 
        /// </param>
        public override void SendA(string subject, string recipient)
        {
            var payload = new EmailPayload { Subject = subject, Recipient = recipient };
            ThreadPool.QueueUserWorkItem(this.Send, payload);
        }

        #endregion

        #region Methods

        /// <summary>
        /// The certificate validation call back.
        /// </summary>
        /// <param name="sender">
        /// The sender. 
        /// </param>
        /// <param name="certificate">
        /// The certificate. 
        /// </param>
        /// <param name="chain">
        /// The chain. 
        /// </param>
        /// <param name="sslPolicyErrors">
        /// The ssl policy errors. 
        /// </param>
        /// <returns>
        /// True if it worked. 
        /// </returns>
        private bool CertificateValidationCallBack(
            object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            // If the override has been set then just return true
            if (this.OverrideCertificate)
            {
                return true;
            }

            // If the certificate is a valid, signed certificate, return true.
            if (sslPolicyErrors == SslPolicyErrors.None)
            {
                return true;
            }

            // If there are errors in the certificate chain, look at each error to determine the cause.
            if ((sslPolicyErrors & SslPolicyErrors.RemoteCertificateChainErrors) == 0)
            {
                // In all other cases, return false.
                return false;
            }

            return chain == null
                   ||
                   chain.ChainStatus.Where(
                       status =>
                       (certificate.Subject != certificate.Issuer)
                       || (status.Status != X509ChainStatusFlags.UntrustedRoot)).All(
                           status => status.Status == X509ChainStatusFlags.NoError);
        }

        /// <summary>
        /// The change state.
        /// </summary>
        /// <param name="state">
        /// The state. 
        /// </param>
        private void ChangeState(ConnectionState state)
        {
            this.ConnectedState = state;
            if (this.ConnectedStateChange != null)
            {
                this.ConnectedStateChange(this, state);
            }

            this.RaiseLogMessage(
                string.Format("{0} {1}", Resources.EwsConnection_ChangeState_Changed_state_to, state.Description()));
        }

        /// <summary>
        /// The check for new appointment.
        /// </summary>
        private void CheckForNewAppointment()
        {
            lock (this.locker)
            {
                try
                {
                    // Interrogate default Calendar
                    var calendarView = new CalendarView(
                        DateTime.Now, DateTime.Now.AddMinutes(Convert.ToDouble(Settings.Default.ApptWindow)))
                        {
                           PropertySet = PropertySet.FirstClassProperties 
                        };
                    FindItemsResults<Appointment> findResults =
                        this.service.FindAppointments(WellKnownFolderName.Calendar, calendarView);

                    // Process each item.
                    foreach (Appointment myItem in findResults.Items)
                    {
                        if (myItem == null)
                        {
                            continue;
                        }

                        Appointment myAppt = myItem;
                        var ps = new PropertySet(BasePropertySet.FirstClassProperties);
                        myAppt.Load(ps);
                        string myLocation = myAppt.Location;
                        string mySubject = myAppt.Subject ?? Resources.EwsConnection_CheckForNewMail_No_subject;
                        TimeSpan span = myAppt.Start.Subtract(DateTime.Now);
                        var duration = (int)Math.Floor(span.TotalMinutes);
                        DateTime myTime = myAppt.Start;
                        string myAccessUrl = this.SupportsDirectMessageAccess
                                                 ? myItem.WebClientReadFormQueryString
                                                 : string.Empty;
                        if (duration > 0)
                        {
                            this.RaiseNewAppointment(duration, myTime, mySubject, myLocation, myAccessUrl);
                        }
                    }
                }
                catch
                {
                }
            }
        }

        /// <summary>
        /// The check for new appointment a.
        /// </summary>
        private void CheckForNewAppointmentA()
        {
            new Thread(this.CheckForNewAppointment).Start();
        }

        /// <summary>
        /// The check for new mail.
        /// </summary>
        private void CheckForNewMail()
        {
            lock (this.locker)
            {
                try
                {
                    // Belt & braces
                    if (!this.IsConnected)
                    {
                        return;
                    }

                    // Quick mail count check
                    int count = this.UnreadCount;
                    if (count != this.mailCount)
                    {
                        this.mailCount = count;
                        this.RaiseMessageCount(count);
                    }

                    // Only process further if there is unread email
                    if (count == 0)
                    {
                        return;
                    }

                    // Define filters collection
                    // RaiseLogMessage("Checking for mail after " + _timeLastChecked);
                    var filters = new SearchFilter.SearchFilterCollection(LogicalOperator.And)
                        {
                            new SearchFilter.IsEqualTo(EmailMessageSchema.IsRead, false), 
                            new SearchFilter.IsGreaterThan(ItemSchema.DateTimeReceived, this.timeLastChecked)
                        };

                    // Set initial flags
                    int offset = 0;
                    bool moreItems = true;

                    // Continue paging while there are more items to fetch
                    while (moreItems)
                    {
                        // RaiseLogMessage("Looking for items...");

                        // Item view
                        var view = new ItemView(Settings.Default.BatchAmount, offset, OffsetBasePoint.Beginning)
                            {
                                PropertySet =
                                    new PropertySet(BasePropertySet.IdOnly)
                                        {
                                           ItemSchema.Subject, ItemSchema.DateTimeReceived 
                                        }
                            };
                        view.OrderBy.Add(ItemSchema.DateTimeReceived, SortDirection.Ascending);

                        // Now search
                        FindItemsResults<Item> findResults = this.service.FindItems(
                            WellKnownFolderName.Inbox, filters, view);

                        // RaiseLogMessage("Found " + findResults.Items.Count + " messages");

                        // Process each item.
                        foreach (EmailMessage myItem in findResults.Items)
                        {
                            // RaiseLogMessage("Processing message");

                            // Get the email details
                            var ps = new PropertySet(BasePropertySet.FirstClassProperties);

                            myItem.Load(ps);
                            string mySender = myItem.Sender.Name;
                            string mySubject = myItem.Subject ?? Resources.EwsConnection_CheckForNewMail_No_subject;
                            string myAccessUrl = this.SupportsDirectMessageAccess
                                                     ? myItem.WebClientReadFormQueryString
                                                     : string.Empty;
                            DateTime myTime = myItem.DateTimeReceived;

                            // Update timestamp
                            this.timeLastChecked = myTime.AddSeconds(1);

                            // Pop message
                            this.RaiseNewMail(myTime, mySubject, mySender, myAccessUrl);
                        }

                        // Set the flag to discontinue paging.
                        if (!findResults.MoreAvailable)
                        {
                            moreItems = false;
                        }

                        // Update the offset if there are more items to page.
                        if (moreItems)
                        {
                            offset = offset + Settings.Default.BatchAmount;
                        }
                    }
                }
                catch
                {
                }
            }
        }

        /// <summary>
        /// The check for new mail a.
        /// </summary>
        private void CheckForNewMailA()
        {
            new Thread(this.CheckForNewMail).Start();
        }

        /// <summary>
        /// The send.
        /// </summary>
        /// <param name="payload">
        /// The payload. 
        /// </param>
        private void Send(object payload)
        {
            var p = (EmailPayload)payload;
            this.Send(p.Subject, p.Recipient);
        }

        /// <summary>
        /// Get the time of newest email.
        /// </summary>
        /// <returns>
        /// The time that the newest mail arived
        /// </returns>
        private DateTime TimeOfNewestEmail()
        {
            DateTime myTime = DateTime.Now;

            // Define filters collection
            var filters = new SearchFilter.SearchFilterCollection(LogicalOperator.And)
                {
                   new SearchFilter.IsEqualTo(EmailMessageSchema.IsRead, false) 
                };

            // Item view
            var view = new ItemView(10, 0, OffsetBasePoint.Beginning)
                {
                   PropertySet = new PropertySet(BasePropertySet.IdOnly) { ItemSchema.DateTimeReceived } 
                };
            view.OrderBy.Add(ItemSchema.DateTimeReceived, SortDirection.Descending);

            // Now search
            FindItemsResults<Item> findResults = this.service.FindItems(WellKnownFolderName.Inbox, filters, view);

            // Process each item.
            foreach (Item myItem in findResults.Items)
            {
                var myEmail = myItem as EmailMessage;
                if (myEmail == null)
                {
                    continue;
                }

                var ps = new PropertySet(BasePropertySet.FirstClassProperties);
                myEmail.Load(ps);
                myTime = myEmail.DateTimeReceived;
                break;
            }

            return myTime;
        }

        /// <summary>
        /// The appointment poll_ elapsed.
        /// </summary>
        /// <param name="sender">
        /// The sender. 
        /// </param>
        /// <param name="e">
        /// The e. 
        /// </param>
        private void AppointmentPollElapsed(object sender, EventArgs e)
        {
            if (!this.DisableCalendar)
            {
                this.CheckForNewAppointmentA();
            }
        }

        /// <summary>
        /// The background poll_ elapsed.
        /// </summary>
        /// <param name="sender">
        /// The sender. 
        /// </param>
        /// <param name="e">
        /// The e. 
        /// </param>
        private void BackgroundPollElapsed(object sender, EventArgs e)
        {
            this.CheckForNewMailA();
        }

        #endregion
    }
}