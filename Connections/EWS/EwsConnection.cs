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
using System.Linq;
using System.Threading;
using DrunkenBakery.OWAtray.Connections.Abstract;
using DrunkenBakery.OWAtray.Connections.EWS.Properties;
using DrunkenBakery.OWAtray.Logging;
using Microsoft.Exchange.WebServices.Data;
using Timer = System.Timers.Timer;

namespace DrunkenBakery.OWAtray.Connections.EWS
{
	public class EwsConnection : AbstractConnection
	{
		private readonly Timer _backgroundPoll = new Timer();
		private readonly object _locker = new object();
		private ExchangeService _service;
		private DateTime _timeLastChecked;

		public override string Version
		{
			get { return (!IsConnected ? "Disconnected" : _service.RequestedServerVersion.ToString()); }
		}

		public override EmailType Type
		{
			get { return EmailType.Exchange; }
		}

		private int UnreadCount
		{
			get
			{
				var myFolder = Folder.Bind(_service, WellKnownFolderName.Inbox);
				return myFolder.UnreadCount;
			}
		}

		// Events
		public override event Action<IEmailInterface, ConnectionState> ConnectedStateChange;

		public override void Connect()
		{
			if (IsConnected)
			{
				RaiseLogMessage("Already connected", Severity.Fail);
				return;
			}
			else
			{
				RaiseLogMessage("Connecting, please wait");
			}

			try
			{
				// State
				ChangeState(ConnectionState.Connecting);

				// Define service
				_service = new ExchangeService();

				// Enable Tracing (if required)
				if (Settings.Default.UseTracing)
				{
					_service.TraceListener = new EwsTraceListener();
					_service.TraceFlags = TraceFlags.EwsRequest | TraceFlags.EwsResponse;
					_service.TraceEnabled = true;
				}

				// Connect - this is a synchronous operation
				_service.Credentials = new WebCredentials((Username.Length == 0 ? EmailAddress : Username), Password);
				_service.AutodiscoverUrl(EmailAddress, delegate { return true; });

				// State
				ChangeState(ConnectionState.Connected);

				// Get initial timestamp
				_timeLastChecked = TimeOfNewestEmail().AddSeconds(1);

				// Initial Message
				var count = UnreadCount;
				if (count > 0)
				{
					RaiseLogMessage(string.Format("You have {0} unread messages in your Inbox", count));
				}

				// Timer
				_backgroundPoll.Interval = (Interval * 1000);
				_backgroundPoll.Elapsed += backgroundPoll_Elapsed;
				_backgroundPoll.Start();
			}
			catch (Exception ex)
			{
				ChangeState(ConnectionState.Disconnected);
				RaiseLogMessage(ex.Message, Severity.Fail);
			}
		}

		private void ChangeState(ConnectionState state)
		{
			ConnectedState = state;
			if (ConnectedStateChange != null) ConnectedStateChange(this, state);
		}

		public override void ConnectA()
		{
			lock (_locker)
			{
				new Thread(Connect).Start();
			}
		}

		public override void Disconnect()
		{
			if (!IsConnected)
			{
				return;
			}
			else
			{
				RaiseLogMessage("Disconnecting, please wait");
			}

			try
			{
				ChangeState(ConnectionState.Disconnecting);
				_backgroundPoll.Stop();
				_backgroundPoll.Elapsed -= backgroundPoll_Elapsed;
				_service = null;
			}
			catch (Exception ex)
			{
				RaiseLogMessage(ex.Message, Severity.Fail);
			}
			finally
			{
				ChangeState(ConnectionState.Disconnected);
			}
		}

		public override void DisconnectA()
		{
			lock (_locker)
			{
				new Thread(Disconnect).Start();
			}
		}

		private void backgroundPoll_Elapsed(object sender, EventArgs e)
		{
			CheckForNewMailA();
		}

		private void CheckForNewMailA()
		{
			lock (_locker)
			{
				new Thread(CheckForNewMail).Start();
			}
		}

		private DateTime TimeOfNewestEmail()
		{
			var myTime = DateTime.Now;

			// Define filters collection
			var filters = new SearchFilter.SearchFilterCollection(LogicalOperator.And)
			              	{new SearchFilter.IsEqualTo(EmailMessageSchema.IsRead, false)};

			// Item view
			var view = new ItemView(10, 0, OffsetBasePoint.Beginning)
			           	{PropertySet = new PropertySet(BasePropertySet.IdOnly) {ItemSchema.DateTimeReceived}};
			view.OrderBy.Add(ItemSchema.DateTimeReceived, SortDirection.Descending);

			try
			{
				// Now search
				var findResults = _service.FindItems(WellKnownFolderName.Inbox, filters, view);

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
			}
			catch (Exception ex)
			{
				RaiseLogMessage(ex.Message, Severity.Fail);
			}

			return myTime;
		}

		private void CheckForNewMail()
		{
			try
			{
				// Belt & braces
				if (!IsConnected) return;

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
					              		new SearchFilter.IsGreaterThan(ItemSchema.DateTimeReceived, _timeLastChecked)
					              	};

					// Item view
					var view = new ItemView(pageSize, offset, OffsetBasePoint.Beginning)
					           	{
					           		PropertySet =
					           			new PropertySet(BasePropertySet.IdOnly) {ItemSchema.Subject, ItemSchema.DateTimeReceived}
					           	};
					view.OrderBy.Add(ItemSchema.DateTimeReceived, SortDirection.Descending);

					// Now search
					var findResults = _service.FindItems(WellKnownFolderName.Inbox, filters, view);

					// Only update timestamp once
					var isFlagged = false;

					// Process each item.
					foreach (var myItem in findResults.Items.OfType<EmailMessage>())
					{
						var myTime = DateTime.Now;

						try
						{
							// Get the email details
							var ps = new PropertySet(BasePropertySet.FirstClassProperties);
							myItem.Load(ps);
							var mySender = myItem.Sender.Name;
							var mySubject = (myItem.Subject ?? "No subject");
							myTime = myItem.DateTimeReceived;

							// Pop message
							RaiseNewMail(myTime, mySubject, mySender);
						}
						catch (Exception ex)
						{
							RaiseLogMessage(ex.Message, Severity.Fail);
						}

						// Update flag
						if (isFlagged) continue;
						_timeLastChecked = myTime.AddSeconds(1);
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
				RaiseLogMessage(ex.Message, Severity.Fail);
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
				var message = new EmailMessage(_service) {Subject = subject, Body = "OWAtray Test Message"};
				message.ToRecipients.Add(recipient);
				message.Send();
			}
			catch (Exception ex)
			{
				RaiseLogMessage(ex.Message, Severity.Fail);
			}
		}

		private void Send(object payload)
		{
			var p = (EmailPayload) payload;
			Send(p.Subject, p.Recipient);
		}

		public override void SendA(string subject, string recipient)
		{
			lock (_locker)
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