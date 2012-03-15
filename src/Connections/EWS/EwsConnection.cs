//------------------------------------------------------------------
// DrunkenBakery OWA Tray Monitor
// EwsConnection Class
//
// <copyright file="EwsConnection.cs" company="The Drunken Bakery">
//     Copyright (c) 2012 The Drunken Bakery. All rights reserved.
// </copyright>
//
// Concrete implementation of an Exchange (EWS) connection
//
//------------------------------------------------------------------

using System;
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

namespace DrunkenBakery.OWAtray.Connections.EWS
{
	public class EwsConnection : AbstractConnection
	{
		private readonly Timer backgroundPoll = new Timer();
		private readonly Timer appointmentPoll = new Timer();
		private readonly object locker = new object();
		private ExchangeService service;
		private DateTime timeLastChecked;
		private int mailCount = -1;

		public override string Version
		{
			get { return (!IsConnected ? ServerVersion == "Default" ? ExchangeVersion.Exchange2007_SP1.ToString() : ServerVersion : this.service.ServerInfo.VersionString); }
		}

		public override bool SupportsDirectMessageAccess
		{
			get { return (Version != ExchangeVersion.Exchange2007_SP1.ToString()); }
		}

		public override EmailType Type
		{
			get { return EmailType.Exchange; }
		}

		public override int UnreadCount
		{
			get
			{
				var count = 0;
				try
				{
					var myFolder = Folder.Bind(this.service, WellKnownFolderName.Inbox);
					count = myFolder.UnreadCount;
				}
				catch
				{
				}
				return count;
			}
		}

		public override event Action<IEmailInterface, ConnectionState> ConnectedStateChange;

		public override void Connect()
		{
			// Check input
			if (EmailAddress.Length == 0)
			{
				RaiseLogMessage("Please provide a valid Email Address", Severity.Fail);
				return;
			}
			if (!OnWindowsDomain && Password.Length == 0)
			{
				RaiseLogMessage("Please provide a Password", Severity.Fail);
				return;
			}

			if (!UseAutodiscovery)
			{
				if (DerivedServiceUrl.Length == 0)
				{
					RaiseLogMessage("Please provide a valid Server Address", Severity.Fail);
					return;
				}
			}

			try
			{
				// State
				ChangeState(ConnectionState.Connecting);

				// Validate the server certificate
				ServicePointManager.ServerCertificateValidationCallback = CertificateValidationCallBack;

				// Define service
				this.service = ServerVersion == "Default" ? new ExchangeService() : new ExchangeService((ExchangeVersion)Enum.Parse(typeof(ExchangeVersion), ServerVersion));

				// Enable Tracing (if required)
				if (Settings.Default.UseTracing)
				{
					this.service.TraceListener = new EwsTraceListener();
					this.service.TraceFlags = TraceFlags.EwsRequest | TraceFlags.EwsResponse;
					this.service.TraceEnabled = true;
				}

				// Are we on a Windows domain?
				this.service.UseDefaultCredentials = OnWindowsDomain;

				if (!OnWindowsDomain)
				{
					this.service.Credentials = AccountDomain.Length > 0 ? new WebCredentials((Username.Length == 0 ? EmailAddress : Username), Password, AccountDomain) : new WebCredentials((Username.Length == 0 ? EmailAddress : Username), Password);
				}

				// Connect using Autodiscover?
				if (UseAutodiscovery)
				{
					if (OverrideAutodiscoveryValidation)
					{
						this.service.AutodiscoverUrl(EmailAddress, delegate { return true; });
					}
					else
					{
						this.service.AutodiscoverUrl(EmailAddress);
					}

					// Probe for autodiscover information
                    //var autodiscoverService = new AutodiscoverService((ExchangeVersion)Enum.Parse(typeof(ExchangeVersion), Version));
                    var autodiscoverService = new AutodiscoverService(ExchangeVersion.Exchange2007_SP1);

					// Credentials
					if (OnWindowsDomain)
					{
						autodiscoverService.UseDefaultCredentials = true;
					}
					else
					{
						autodiscoverService.Credentials = AccountDomain.Length > 0 ? new WebCredentials((Username.Length == 0 ? EmailAddress : Username), Password, AccountDomain) : new WebCredentials((Username.Length == 0 ? EmailAddress : Username), Password);
					}

					// Redirection Callback
					if (OverrideAutodiscoveryValidation)
					{
						autodiscoverService.RedirectionUrlValidationCallback = delegate { return true; };
					}

					// Is this Internal or External ?
					if (autodiscoverService.IsExternal == false)
					{
						// Probe for values
						var userresponse = autodiscoverService.GetUserSettings(EmailAddress,
																				UserSettingName.InternalWebClientUrls,
																				UserSettingName.InternalEwsUrl,
																				UserSettingName.InternalMailboxServer,
																				UserSettingName.UserDisplayName);

						// OWA Url
						WebClientUrlCollection webCollection;
						if (userresponse.TryGetSettingValue(UserSettingName.InternalWebClientUrls, out webCollection))
						{
							foreach (WebClientUrl url in webCollection.Urls)
							{
								DiscoveredEmailUrl = url.Url;
							}
						}

						// EWS Url
						string internalUrl;
						if (userresponse.TryGetSettingValue(UserSettingName.InternalEwsUrl, out internalUrl))
						{
							DiscoveredServiceUrl = internalUrl;
						}

						// Server
						string internalServer;
						if (userresponse.TryGetSettingValue(UserSettingName.InternalMailboxServer, out internalServer))
						{
							DiscoveredEmailServer = internalServer;
						}

						// User Name
						string userName;
						if (userresponse.TryGetSettingValue(UserSettingName.UserDisplayName, out userName))
						{
							DiscoveredUsername = userName;
						}
					}
					else
					{
						// Probe for values
						var userresponse = autodiscoverService.GetUserSettings(EmailAddress,
																				UserSettingName.ExternalWebClientUrls,
																				UserSettingName.ExternalEwsUrl,
																				UserSettingName.ExternalMailboxServer,
																				UserSettingName.UserDisplayName);

						// OWA Url
						WebClientUrlCollection webCollection;
						if (userresponse.TryGetSettingValue(UserSettingName.ExternalWebClientUrls, out webCollection))
						{
							foreach (WebClientUrl url in webCollection.Urls)
							{
								DiscoveredEmailUrl = url.Url;
							}
						}

						// EWS Url
						string externalUrl;
						if (userresponse.TryGetSettingValue(UserSettingName.ExternalEwsUrl, out externalUrl))
						{
							DiscoveredServiceUrl = externalUrl;
						}

						// Server
						string externalServer;
						if (userresponse.TryGetSettingValue(UserSettingName.ExternalMailboxServer, out externalServer))
						{
							DiscoveredEmailServer = externalServer;
						}

						// User Name
						string userName;
						if (userresponse.TryGetSettingValue(UserSettingName.UserDisplayName, out userName))
						{
							DiscoveredUsername = userName;
						}
					}
				}
				else
				{
					if (DerivedServiceUrl.Length > 0)
					{
						var myUri = new Uri(DerivedServiceUrl);
						this.service.Url = myUri;

						// Update properties
						DiscoveredEmailServer = EmailServer;
						DiscoveredUsername = (OnWindowsDomain ? "" : (Username.Length == 0 ? EmailAddress : Username));
					}
				}

				// Get initial timestamp
				this.timeLastChecked = TimeOfNewestEmail().AddSeconds(1);

				// Initial Message
				var count = UnreadCount;
				this.RaiseMessageCount(count);

				// Timers
				this.backgroundPoll.Interval = (Interval * 1000);
				this.backgroundPoll.Elapsed += backgroundPoll_Elapsed;
				this.backgroundPoll.Start();
				this.appointmentPoll.Interval = (Settings.Default.ApptInterval * 1000);
				this.appointmentPoll.Elapsed += appointmentPoll_Elapsed;
				this.appointmentPoll.Start();

				// Set timeout
				this.service.Timeout = (Interval * 1000) - 500;

				// Initial check
				this.mailCount = -1;
				CheckForNewMailA();
				if (!DisableCalendar) CheckForNewAppointmentA();

				// State
				ChangeState(ConnectionState.Connected);
			}
			catch (Exception ex)
			{
                RaiseLogMessage(ex.Message, Severity.Fail);
                ChangeState(ConnectionState.Failed);
			}
		}

		private bool CertificateValidationCallBack(
			object sender,
			X509Certificate certificate,
			X509Chain chain,
			SslPolicyErrors sslPolicyErrors)
		{
			// If the override has been set then just return true
			if (OverrideCertificate) return true;

			// If the certificate is a valid, signed certificate, return true.
			if (sslPolicyErrors == SslPolicyErrors.None) return true;

			// If there are errors in the certificate chain, look at each error to determine the cause.
			if ((sslPolicyErrors & SslPolicyErrors.RemoteCertificateChainErrors) == 0)
			{
				// In all other cases, return false.
				return false;
			}
			else
			{
				if (chain != null)
				{
					foreach (var status in chain.ChainStatus)
					{
						if ((certificate.Subject == certificate.Issuer) &&
							(status.Status == X509ChainStatusFlags.UntrustedRoot))
						{
							// Self-signed certificates with an untrusted root are valid.
							continue;
						}
						else
						{
							if (status.Status != X509ChainStatusFlags.NoError)
							{
								// If there are any other errors in the certificate chain, the certificate is invalid
								return false;
							}
						}
					}
				}

				return true;
			}
		}

		private void ChangeState(ConnectionState state)
		{
			ConnectedState = state;
			if (ConnectedStateChange != null) ConnectedStateChange(this, state);
			RaiseLogMessage(string.Format("Changed state to {0}", state.ToString()));
		}

		public override void ConnectA()
		{
			lock (this.locker)
			{
				new Thread(Connect).Start();
			}
		}

		public override void Disconnect()
		{
			if (!IsConnected) return;

			try
			{
				ChangeState(ConnectionState.Disconnecting);
				this.backgroundPoll.Stop();
				this.backgroundPoll.Elapsed -= backgroundPoll_Elapsed;
				this.appointmentPoll.Stop();
				this.appointmentPoll.Elapsed -= appointmentPoll_Elapsed;
				this.service = null;
				ChangeState(ConnectionState.Disconnected);
			}
			catch (Exception ex)
			{
				if (ex.InnerException.Message != "The operation has timed out")
				{
					ChangeState(ConnectionState.Failed);
					RaiseLogMessage(ex.ToString(), Severity.Fail);
				}
			}
		}

		public override void DisconnectA()
		{
			lock (this.locker)
			{
				new Thread(Disconnect).Start();
			}
		}

		private void backgroundPoll_Elapsed(object sender, EventArgs e)
		{
			CheckForNewMailA();
		}

		private void appointmentPoll_Elapsed(object sender, EventArgs e)
		{
			if (!DisableCalendar) CheckForNewAppointmentA();
		}

		private void CheckForNewMailA()
		{
			lock (this.locker)
			{
				new Thread(CheckForNewMail).Start();
			}
		}

		private void CheckForNewAppointmentA()
		{
			lock (this.locker)
			{
				new Thread(CheckForNewAppointment).Start();
			}
		}

		private DateTime TimeOfNewestEmail()
		{
			var myTime = DateTime.Now;

			// Define filters collection
			var filters = new SearchFilter.SearchFilterCollection(LogicalOperator.And) { new SearchFilter.IsEqualTo(EmailMessageSchema.IsRead, false) };

			// Item view
			var view = new ItemView(10, 0, OffsetBasePoint.Beginning) { PropertySet = new PropertySet(BasePropertySet.IdOnly) { ItemSchema.DateTimeReceived } };
			view.OrderBy.Add(ItemSchema.DateTimeReceived, SortDirection.Descending);

			// Now search
			var findResults = this.service.FindItems(WellKnownFolderName.Inbox, filters, view);

			// Process each item.
			foreach (var myItem in findResults.Items)
			{
				var myEmail = myItem as EmailMessage;
				if (myEmail == null) continue;

				var ps = new PropertySet(BasePropertySet.FirstClassProperties);
				myEmail.Load(ps);
				myTime = myEmail.DateTimeReceived;
				break;
			}

			return myTime;
		}

		private void CheckForNewMail()
		{
			try
			{
				// Belt & braces
				if (!IsConnected) return;

				// Quick mail count check
				var count = UnreadCount;
				if (count != this.mailCount)
				{
					this.mailCount = count;
					RaiseMessageCount(count);
				}

                // Only process further if there is unread email
                if (count == 0) return;
         
				// Set the offset for the paged search.
				var offset = 0;

				// Set the page size.
				var pageSize = Settings.Default.BatchAmount;

				// Set the flag that indicates whether to continue iterating through additional pages.
				var moreItems = true;

				// Continue paging while there are more items to page.
				while (moreItems)
				{
					// Define filters collection
					var filters = new SearchFilter.SearchFilterCollection(LogicalOperator.And)
					              	{
					              		new SearchFilter.IsEqualTo(EmailMessageSchema.IsRead, false),
					              		new SearchFilter.IsGreaterThan(ItemSchema.DateTimeReceived, this.timeLastChecked)
					              	};

					// Item view
					var view = new ItemView(pageSize, offset, OffsetBasePoint.Beginning)
								{
									PropertySet =
										new PropertySet(BasePropertySet.IdOnly) { ItemSchema.Subject, ItemSchema.DateTimeReceived }
								};
					view.OrderBy.Add(ItemSchema.DateTimeReceived, SortDirection.Descending);

					// Now search
					var findResults = this.service.FindItems(WellKnownFolderName.Inbox, filters, view);

					// Only update timestamp once
					var isFlagged = false;

					// Process each item.
					foreach (EmailMessage myItem in findResults.Items)
					{
						// Get the email details
						var ps = new PropertySet(BasePropertySet.FirstClassProperties);

						myItem.Load(ps);
						var mySender = myItem.Sender.Name;
						var mySubject = (myItem.Subject ?? "No subject");
						var myAccessUrl = SupportsDirectMessageAccess ? myItem.WebClientReadFormQueryString : "";
						DateTime myTime = myItem.DateTimeReceived;

						// Pop message
						RaiseNewMail(myTime, mySubject, mySender, myAccessUrl);

						// Update flag
						if (isFlagged) continue;
						this.timeLastChecked = myTime.AddSeconds(1);
						isFlagged = true;
					}

					// Set the flag to discontinue paging.
					if (!findResults.MoreAvailable)
					{
						moreItems = false;
					}

					// Update the offset if there are more items to page.
					if (moreItems)
					{
						offset = offset + pageSize;
					}
				}
			}
			catch (Exception ex)
			{
				if (ex.InnerException.Message != "The operation has timed out")
				{
					RaiseLogMessage(ex.ToString(), Severity.Fail);
					ChangeState(ConnectionState.Failed);
				}
			}
		}

		private void CheckForNewAppointment()
		{
			try
			{
				// Interrogate default Calendar
				var cView = new CalendarView(DateTime.Now, DateTime.Now.AddMinutes(Convert.ToDouble(Settings.Default.ApptWindow))) { PropertySet = PropertySet.FirstClassProperties };
				var findResults = this.service.FindAppointments(WellKnownFolderName.Calendar, cView);

				// Process each item.
				foreach (Appointment myItem in findResults.Items)
				{
					if (myItem == null) continue;

					var duration = 0;
					var myAppt = myItem;
					var ps = new PropertySet(BasePropertySet.FirstClassProperties);
					myAppt.Load(ps);
					var myLocation = myAppt.Location;
					var mySubject = (myAppt.Subject ?? "No subject");
					var span = myAppt.Start.Subtract(DateTime.Now);
					duration = (int)Math.Floor(span.TotalMinutes);
					var myTime = myAppt.Start;
					var myAccessUrl = SupportsDirectMessageAccess ? myItem.WebClientReadFormQueryString : "";
					if (duration > 0) RaiseNewAppointment(duration, myTime, mySubject, myLocation, myAccessUrl);
				}
			}
			catch (Exception ex)
			{
				if (ex.InnerException.Message != "The operation has timed out")
				{
					RaiseLogMessage(ex.ToString(), Severity.Fail);
					ChangeState(ConnectionState.Failed);
				}
			}
		}

		public override void Send(string subject, string recipient)
		{
			if (!IsConnected)
			{
				RaiseLogMessage("Unable to send. Email provider is disconnected.", Severity.Fail);
				return;
			}

			if (recipient.Length == 0)
			{
				RaiseLogMessage("Unable to send. No recipient specified.", Severity.Fail);
				return;
			}

			try
			{
				var message = new EmailMessage(this.service) { Subject = subject, Body = "OWAtray Test Message" };
				message.ToRecipients.Add(recipient);
				message.Send();
			}
			catch (Exception ex)
			{
				RaiseLogMessage(ex.ToString(), Severity.Fail);
			}
		}

		private void Send(object payload)
		{
			var p = (EmailPayload)payload;
			Send(p.Subject, p.Recipient);
		}

		public override void SendA(string subject, string recipient)
		{
			lock (this.locker)
			{
				var payload = new EmailPayload
								{
									Subject = subject,
									Recipient = recipient
								};
				ThreadPool.QueueUserWorkItem(Send, payload);
			}
		}
	}
}