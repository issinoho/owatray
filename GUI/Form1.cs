//------------------------------------------------------------------
// DrunkenBakery OWA Tray Monitor
// Main Form
//
// <copyright file="Form1.cs" company="The Drunken Bakery">
//     Copyright (c) 2009-2011 The Drunken Bakery. All rights reserved.
// </copyright>
//
// Monitors Exchange email for OWA users
// Main application form which drives all functionality.
//
//------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;
using System.Threading;
using System.Windows.Forms;
using DrunkenBakery.OWAtray.Audio;
using DrunkenBakery.OWAtray.Connections.Abstract;
using DrunkenBakery.OWAtray.Connections.Proxy;
using DrunkenBakery.OWAtray.GUI.Properties;
using DrunkenBakery.OWAtray.Growl;
using DrunkenBakery.OWAtray.Logging;
using DrunkenBakery.OWAtray.Snarl;

namespace DrunkenBakery.OWAtray.GUI
{
	public partial class Form1 : Form
	{
		private const int MaxInterval = 3600;

		private static bool _overRideClose;
		private readonly string _alertIcon;
		private readonly string _emailIcon;
		private readonly string _graphicPath;
		private readonly List<ListViewItem> _lvBuffer = new List<ListViewItem>();
		private readonly string _shellPath;
		private readonly bool _startingUp;
		private bool _firstRun = true;
		private Form _frmAbout;
		private Form _frmChangeLog;
		private Form _frmContact;
		private Form _frmInfo;
		private Form _frmMdac;
		private Form _frmNet;
		private int _inboxCount;
		private string _lastPopMessage;
		private string _lastPopTitle;
		private string _lastPopUrl;
		private string _popUrl;
		private string _reportedEwsUrl = "";
		private string _reportedMailboxServer = "";
		private string _reportedOwaUrl = "";
		private string _reportedUserName = "";
		private bool _resetFlag;
		private DateTime _timeLastChecked = DateTime.Now;

		// New variables
		private EmailConnections Connections { get; set; }
		private IEmailInterface _connection;

		public Form1()
		{
			InitializeComponent();

			// Interlock for starting up
			_startingUp = true;

			// Set up look & feel
			WindowDressing();

			// Start Logging
			Logger.Execute();

			// Welcome message
			AddLogEntry(string.Format("{0} {1} v{2}", OWAtray.Welcome_to_the, AssemblyHelpers.AssemblyTitle,
			                          AssemblyHelpers.UpgradeSettings()));

			// Options
			exchange2007ToolStripMenuItem.SelectedIndex = 0;
			switch (Settings.Default.ExchangeVersion)
			{
				case "Autodetect":
					exchange2007ToolStripMenuItem.SelectedIndex = 0;
					break;

				case "Exchange2007_SP1":
					exchange2007ToolStripMenuItem.SelectedIndex = 1;
					break;

				case "Exchange2010":
					exchange2007ToolStripMenuItem.SelectedIndex = 2;
					break;

				case "Exchange2010_SP1":
					exchange2007ToolStripMenuItem.SelectedIndex = 3;
					break;
			}

			txtEmail.Text = Settings.Default.EMail;
			txtServer.Text = Settings.Default.Server;
			txtUser.Text = Settings.Default.Username;
			txtPwd.Text = Settings.Default.Password.Decrypt();
			txtDomain.Text = Settings.Default.Domain;
			txtInterval.Text = Settings.Default.UpdateInterval.ToString(CultureInfo.InvariantCulture);
			txtURLEdit.Text = Settings.Default.ManualURL;
			txtOWAEdit.Text = Settings.Default.ManualOWAUrl;

			// Startup Flag
			chkRunOnStartup.Checked = WindowsShortcut.Exists(Environment.SpecialFolder.Startup,
			                                                 AssemblyHelpers.AssemblyTitle);

			// Notifications
			balloonToolStripMenuItem.Checked = Settings.Default.Balloon;
			growlToolStripMenuItem.Checked = Settings.Default.Growl;
			snarlToolStripMenuItem.Checked = Settings.Default.Snarl;
			playSoundToolStripMenuItem.Checked = Settings.Default.Bell;

			// Checkboxes
			cbOverrideEWS.Checked = Settings.Default.OverrideURL;
			txtURLEdit.Enabled = cbOverrideEWS.Checked;
			cbOverrideOWA.Checked = Settings.Default.OverrideOWAUrl;
			txtOWAEdit.Enabled = cbOverrideOWA.Checked;

			// Menu Items
			overrideToolStripMenuItem.Checked = Settings.Default.OverrideCert;
			alwaysOpenOWAInIEToolStripMenuItem.Checked = Settings.Default.AlwaysIE;
			disableCalendarToolStripMenuItem.Checked = Settings.Default.DisableCalendar;
			loginAutomaticallyToolStripMenuItem.Checked = Settings.Default.AutoLogin;
			office365LoginOverrideToolStripMenuItem.Checked = Settings.Default.UseOffice365;
			overrideAutodiscoveryValidationToolStripMenuItem.Checked = Settings.Default.OverrideValidation;
			useDefaultWebProxyToolStripMenuItem.Checked = Settings.Default.UseWebProxy;

			// Autodiscover?
			chkAutodiscovery.Checked = Settings.Default.Autodiscovery;
			SelectAutodiscoveryOptions();

			// URLs
			UpdateUrl();
			UpdateOwaUrl();
			UpdateEmail();

			// Domain
			chkOnDomain.Checked = Settings.Default.NetworkCredentials;
			SelectDomainOptions();

			// Special lockdown option
			restoreToolStripMenuItem.Enabled = (!Settings.Default.LockDown);

			// Icon
			_graphicPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
			                            Settings.Default.EmailGraphic);
			_emailIcon = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), Settings.Default.EmailIcon);
			_alertIcon = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), Settings.Default.AlertIcon);

			// Tray icon (default)
			notifyIcon1.Icon = new Icon(_emailIcon);

			// Path to shell integration module
			_shellPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
			                          Settings.Default.ShellIntegration);

			// A few flags
			_lastPopTitle = "";
			_lastPopMessage = "";
			_popUrl = "";
			_lastPopUrl = "";
			_resetFlag = false;
			_inboxCount = 0;

			// Growl
			GrowlHelper.RegisterGrowl(AssemblyHelpers.AssemblyTitle, _graphicPath, "NEWMAIL", "New Mail");

			// Snarl
			SnarlHelper.RegisterSnarl(AssemblyHelpers.AssemblyTitle, _graphicPath, Handle);

			// Start Timers
			//timerAppt.Interval = Settings.Default.ApptInterval*1000;
			//timerUpdate.Interval = Settings.Default.UpdateInterval*1000;
			timerLogging.Enabled = true;

			// Now decide what to do based on whether this is the first run or not
			//if (!Settings.Default.FirstTime)
			//{
			//    // Set up Exchange
			//    if (ConfigureExchange())
			//    {
			//        // Start main timer
			//        timer1.Enabled = true;
			//    }
			//}

			// Release interlock
			_startingUp = false;
			AddLogEntry(OWAtray.Ready);
		}

		private string EmailAddress
		{
			get
			{
				var userAccount = (txtEmail.Text.Length > 0) ? txtEmail.Text : txtUser.Text;
				if (userAccount.Length > 0 && !userAccount.Contains("@"))
				{
					userAccount = userAccount + "@" + GetSubDomain(txtServer.Text);
				}

				return userAccount;
			}
		}

		private void WindowDressing()
		{
			Text = AssemblyHelpers.AssemblyTitle + OWAtray.Form1_WindowDressing__freshly_baked_at_ +
			       AssemblyHelpers.AssemblyCompany;
			notifyIcon1.Text = AssemblyHelpers.AssemblyTitle + Environment.NewLine + OWAtray.Not_Connected_to_Exchange;
			foreach (TabPage tab in tabMain.TabPages) tab.BackColor = SystemColors.Control;
			InitEventView(lvStatus);
		}

		private void SelectDomainOptions()
		{
			txtDomain.Enabled = !chkOnDomain.Checked;
			txtPwd.Enabled = !chkOnDomain.Checked;
			txtUser.Enabled = !chkOnDomain.Checked;
		}

		private void SelectAutodiscoveryOptions()
		{
			txtServer.Enabled = !Settings.Default.Autodiscovery;
			cbOverrideEWS.Enabled = !Settings.Default.Autodiscovery;
			cbOverrideOWA.Enabled = !Settings.Default.Autodiscovery;
			txtDomain.Enabled = !Settings.Default.Autodiscovery;
			overrideAutodiscoveryValidationToolStripMenuItem.Enabled = Settings.Default.Autodiscovery;
		}

		private void WireUpConnectionEvents()
		{
			foreach (var item in Connections.Where(item => !item.IsLogEventDefined))
			{
				// Logging event
				item.LogMessage += AddLogEntry;

				// State change event
				var itemCopy = item;
				item.ConnectedStateChange += (connection, state) =>
				{
					switch (state)
					{
						case ConnectionState.Connected:
							AddLogEntry(
								string.Format("[{0}] - Connected to {1}", connection.EmailAddress,
												connection.Version), Severity.Success);
							break;

						case ConnectionState.Disconnected:
							AddLogEntry(string.Format("[{0}] - Disconnected", connection.EmailAddress),
										Severity.Success);
							break;
					}
				};

				// New mail event
				item.NewMail += (arrivalTime, subject, sender) => AddLogEntry(string.Format("New mail from {0} - {1}", sender, subject));
			}
		}

		private void UpdateOwaUrl()
		{
			if (Settings.Default.UseOffice365)
			{
				lblOWAUrl.Text = Settings.Default.Office365OwaUrl + StripEmailDomain(lblEmail.Text);
			}
			else if (Settings.Default.Autodiscovery && _reportedOwaUrl.Length > 0)
			{
				lblOWAUrl.Text = _reportedOwaUrl;
			}
			else if (Settings.Default.OverrideOWAUrl && txtOWAEdit.Text.Length > 0)
			{
				lblOWAUrl.Text = txtOWAEdit.Text;
			}
			else if (txtServer.Text.Length > 0)
			{
				lblOWAUrl.Text = string.Format("{0}{1}{2}", "https://", txtServer.Text, "/owa/");
			}
			else
			{
				lblOWAUrl.Text = OWAtray.unknown;
			}

			if (!_startingUp)
			{
				// Update shell parameters
				ConfigureShell();
			}
		}

		private static string StripEmailDomain(string email)
		{
			string sub = "";
			int start = email.IndexOf("@", StringComparison.Ordinal);
			if (start > 0) sub = email.Substring(start + 1);
			return sub;
		}

		public bool IsUserAdministrator()
		{
			//bool value to hold our return value
			bool isAdmin = false;
			try
			{
				//get the currently logged in user
				WindowsIdentity user = WindowsIdentity.GetCurrent();
				if (user != null)
				{
					var principal = new WindowsPrincipal(user);
					isAdmin = principal.IsInRole(WindowsBuiltInRole.Administrator);
				}
			}
			catch (UnauthorizedAccessException ex)
			{
				isAdmin = false;
				MessageBox.Show(ex.Message);
			}
			catch (Exception ex)
			{
				isAdmin = false;
				MessageBox.Show(ex.Message);
			}
			return isAdmin;
		}

		private static string GetSubDomain(string domain)
		{
			string result = "";

			string[] parts = domain.Split('.');

			if (parts.Length > 1)
			{
				for (int f = 1; f < parts.Length; ++f)
				{
					result = result + parts[f];
					if (f != (parts.Length - 1)) result = result + ".";
				}
			}

			return result;
		}

		private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (_frmAbout == null) _frmAbout = new AboutBox1();
			_frmAbout.ShowDialog();
		}

		private void ActivateOwa()
		{
			var runSvc = new ProcessStartInfo(_shellPath) {WindowStyle = ProcessWindowStyle.Hidden};

			if (Settings.Default.AlwaysIE)
			{
				runSvc.Arguments = "owa" + ((_popUrl.Length > 0) ? " " + _popUrl : "");
			}
			else
			{
				runSvc.Arguments = "shell" + ((_popUrl.Length > 0) ? " " + _popUrl : "");
			}

			Process serviceProcess = Process.Start(runSvc);

			if (office365LoginOverrideToolStripMenuItem.CheckState == CheckState.Checked)
				office365LoginOverrideToolStripMenuItem.CheckState = CheckState.Unchecked;
		}

		private void AddLogEntry(string newEntry, Severity severity = Severity.Info)
		{
			try
			{
				_lvBuffer.Add(new ListViewItem(DateTime.Now.ToString(CultureInfo.InvariantCulture), Convert.ToInt32(severity)));
				_lvBuffer[_lvBuffer.Count - 1].SubItems.Add(newEntry);
				if (severity == Severity.Fail)
					Logger.Error(GetType(), newEntry);
				else
					Logger.Info(GetType(), newEntry);
			}
			catch (Exception)
			{
			}
		}

		private void alwaysOpenOWAInIEToolStripMenuItem_CheckStateChanged(object sender, EventArgs e)
		{
			if (_startingUp) return;

			Settings.Default.AlwaysIE = alwaysOpenOWAInIEToolStripMenuItem.Checked;
			Settings.Default.Save();
			AddLogEntry(OWAtray.Always_use_IE_switched + " " + (Settings.Default.AlwaysIE ? OWAtray.ON : OWAtray.OFF));

			ConfigureShell();
		}

		private void balloonToolStripMenuItem_CheckStateChanged(object sender, EventArgs e)
		{
			if (_startingUp) return;

			Settings.Default.Balloon = balloonToolStripMenuItem.Checked;
			Settings.Default.Save();
			AddLogEntry(OWAtray.Balloon_notifications_switched + " " + (Settings.Default.Balloon ? OWAtray.ON : OWAtray.OFF));
		}

		private static bool CertificateValidationCallBack(
			object sender,
			X509Certificate certificate,
			X509Chain chain,
			SslPolicyErrors sslPolicyErrors)
		{
			// If the override has been set then just return true
			if (Settings.Default.OverrideCert)
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
			else
			{
				if (chain != null)
				{
					foreach (X509ChainStatus status in chain.ChainStatus)
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
								// If there are any other errors in the certificate chain, the certificate is invalid,
								// so the method returns false.
								return false;
							}
						}
					}
				}

				// When processing reaches this line, the only errors in the certificate chain are
				// untrusted root errors for self-signed certifcates. These certificates are valid
				// for default Exchange server installations, so return true.
				return true;
			}
		}

		private void changeLogToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (_frmChangeLog == null) _frmChangeLog = new ChangeLog(Settings.Default.RSSFeed);
			_frmChangeLog.ShowDialog();
		}

		//private void CheckForAppointments()
		//{
		//    try
		//    {
		//        // Interrogate default Calendar
		//        var cView = new CalendarView(DateTime.Now, DateTime.Now.AddMinutes(Convert.ToDouble(Settings.Default.ApptWindow)))
		//                        {PropertySet = PropertySet.FirstClassProperties};
		//        FindItemsResults<Appointment> findResults = _myService.FindAppointments(WellKnownFolderName.Calendar, cView);

		//        // Process each item.
		//        int count = 0;
		//        bool allDone = false;
		//        foreach (Appointment myItem in findResults.Items)
		//        {
		//            if (++count > Settings.Default.MaxNotify)
		//            {
		//                if (!allDone)
		//                {
		//                    PopToast(OWAtray.Too_many_appointments,
		//                             OWAtray.There_are + " " + (findResults.Items.Count - Settings.Default.MaxNotify) + " " +
		//                             OWAtray.others);
		//                    allDone = true;
		//                }
		//            }
		//            else
		//            {
		//                if (myItem != null)
		//                {
		//                    int duration = 0;
		//                    Appointment myAppt = myItem;
		//                    var ps = new PropertySet(BasePropertySet.FirstClassProperties);
		//                    myAppt.Load(ps);
		//                    string myLocation = myAppt.Location;
		//                    string mySubject = (myAppt.Subject ?? OWAtray.No_Subject);
		//                    TimeSpan span = myAppt.Start.Subtract(DateTime.Now);
		//                    duration = (int) Math.Floor(span.TotalMinutes);
		//                    string myStart = duration.ToString(CultureInfo.InvariantCulture);
		//                    string myTime = myAppt.Start.ToString("HH:mm");

		//                    if (duration > 0)
		//                    {
		//                        _popUrl = (_reportedVersion == ExchangeVersion.Exchange2007_SP1 ? "" : myAppt.WebClientReadFormQueryString);
		//                        PopToast(
		//                            OWAtray.You_have_an_appointment_in + " " + myStart + " " + (duration != 1 ? OWAtray.mins : OWAtray.min),
		//                            myTime + " - " + mySubject + " (" + myLocation + ")");
		//                    }
		//                }
		//            }
		//        }
		//    }
		//    catch (Exception ex)
		//    {
		//        AddLogEntry(ex.ToString(), Severity.Fail);
		//        stopMonitoring();
		//        StartRetryTimer();
		//    }
		//}

		private void chkOnDomain_CheckedChanged(object sender, EventArgs e)
		{
			if (_startingUp) return;

			Settings.Default.NetworkCredentials = chkOnDomain.Checked;
			Settings.Default.Save();

			// Switch off some options when domain authentication selected
			SelectDomainOptions();
		}

		private void chkRunOnStartup_CheckedChanged(object sender, EventArgs e)
		{
			RunAtStartup(chkRunOnStartup.Checked);
		}

		private void RunAtStartup(bool switchOn)
		{
			try
			{
				WindowsShortcut.Update(Environment.SpecialFolder.Startup, Application.ExecutablePath, AssemblyHelpers.AssemblyTitle,
				                       switchOn);
				AddLogEntry(
					OWAtray.OWAtray_will + (switchOn ? " " : " " + OWAtray.not + " ") + OWAtray.autostart_with_Windows);
			}
			catch (Exception ex)
			{
				AddLogEntry(ex.Message, Severity.Fail);
			}
		}

		private void cmdStart_Click(object sender, EventArgs e)
		{
			//if (ConfigureExchange())
			//{
			//    // Start
			//    StartMonitoring();
			//}

			ConnectToExchange();
		}

		private void ConnectToExchange()
		{
			// Build connection collection
			if (Connections == null) Connections = new EmailConnections();

			// Remove any existing entry
			if (_connection != null)
			{
				_connection.Disconnect();
				Connections.Remove(_connection);
			}

			// Create the new entry
			_connection = ConnectionFactory.CreateConnection(EmailType.Exchange);
			_connection.EmailAddress = txtEmail.Text;
			_connection.Password = txtPwd.Text;
			_connection.Username = txtUser.Text;
			Connections.Add(_connection);
			WireUpConnectionEvents();				

			// Go!
			_connection.ConnectA();
		}

		private void DisconnectFromExchange()
		{
			if (_connection == null) return;

			_connection.Disconnect();
			Connections.Remove(_connection);
		}

		private void cmdStop_Click(object sender, EventArgs e)
		{
			//stopMonitoring();

			DisconnectFromExchange();
		}

		private void stopMonitoring()
		{
			//timerUpdate.Stop();
			//timerAppt.Stop();
			AddLogEntry(OWAtray.Timer_stopped);
			notifyIcon1.Text = AssemblyHelpers.AssemblyTitle + Environment.NewLine + OWAtray.Not_Connected_to_Exchange;
		}

		//private bool ConfigureExchange()
		//{
		//    try
		//    {
		//        // Cursor
		//        Cursor = Cursors.WaitCursor;

		//        // Validate the server certificate
		//        ServicePointManager.ServerCertificateValidationCallback = CertificateValidationCallBack;

		//        // Set up proxy if needed
		//        if (Settings.Default.UseWebProxy)
		//        {
		//            WebRequest.DefaultWebProxy.Credentials = CredentialCache.DefaultCredentials;
		//        }

		//        AddLogEntry(OWAtray.Binding_to_Exchange);
		//        switch (Settings.Default.ExchangeVersion)
		//        {
		//            case "Autodetect":
		//                _myService = new ExchangeService();
		//                break;

		//            case "Exchange2007_SP1":
		//                _myService = new ExchangeService(ExchangeVersion.Exchange2007_SP1);
		//                break;

		//            case "Exchange2010":
		//                _myService = new ExchangeService(ExchangeVersion.Exchange2010);
		//                break;

		//            case "Exchange2010_SP1":
		//                _myService = new ExchangeService(ExchangeVersion.Exchange2010_SP1);
		//                break;
		//        }

		//        // Credentials
		//        if (chkOnDomain.Checked)
		//        {
		//            _myService.UseDefaultCredentials = true;
		//        }
		//        else
		//        {
		//            if (txtUser.Text.Length == 0 && txtEmail.Text.Length == 0)
		//            {
		//                AddLogEntry(OWAtray.Please_supply_valid_email, Severity.Fail);
		//                return false;
		//            }

		//            if (txtPwd.Text.Length == 0)
		//            {
		//                AddLogEntry(OWAtray.Please_supply_valid_password, Severity.Fail);
		//                return false;
		//            }

		//            if (txtDomain.Text.Length > 0)
		//            {
		//                _myService.Credentials = new WebCredentials((txtUser.Text.Length == 0 ? txtEmail.Text : txtUser.Text), txtPwd.Text,
		//                                                            txtDomain.Text);
		//            }
		//            else
		//            {
		//                _myService.Credentials = new WebCredentials((txtUser.Text.Length == 0 ? txtEmail.Text : txtUser.Text), txtPwd.Text);
		//            }
		//        }

		//        // If autodiscover is on then that overrides the URI
		//        if (Settings.Default.Autodiscovery)
		//        {
		//            if (lblEmail.Text.Length == 0)
		//            {
		//                AddLogEntry(OWAtray.Autodiscovery_requires_an_Email, Severity.Fail);
		//                return false;
		//            }
		//            else
		//            {
		//                AddLogEntry(OWAtray.Starting_Autodiscovery);
		//                if (Settings.Default.OverrideValidation)
		//                {
		//                    _myService.AutodiscoverUrl(lblEmail.Text, delegate { return true; });
		//                }
		//                else
		//                {
		//                    _myService.AutodiscoverUrl(lblEmail.Text);
		//                }

		//                // Update server settings
		//                _reportedVersion = _myService.RequestedServerVersion;
		//                AddLogEntry(OWAtray.Connected_to + " " + _reportedVersion, Severity.Success);

		//                // Probe for autodiscover information
		//                var autodiscoverService = new AutodiscoverService(_myService.RequestedServerVersion);

		//                // Credentials
		//                if (chkOnDomain.Checked)
		//                {
		//                    autodiscoverService.UseDefaultCredentials = true;
		//                }
		//                else
		//                {
		//                    if (txtDomain.Text.Length > 0)
		//                    {
		//                        autodiscoverService.Credentials = new WebCredentials((txtUser.Text.Length == 0 ? txtEmail.Text : txtUser.Text),
		//                                                                             txtPwd.Text, txtDomain.Text);
		//                    }
		//                    else
		//                    {
		//                        autodiscoverService.Credentials = new WebCredentials((txtUser.Text.Length == 0 ? txtEmail.Text : txtUser.Text),
		//                                                                             txtPwd.Text);
		//                    }
		//                }

		//                // Redirection Callback
		//                if (Settings.Default.OverrideValidation)
		//                {
		//                    autodiscoverService.RedirectionUrlValidationCallback = delegate { return true; };
		//                }

		//                // Is this Internal or External ?
		//                if (autodiscoverService.IsExternal == false)
		//                {
		//                    // Internal
		//                    AddLogEntry(OWAtray.Endpoint_is_INSIDE_corporate);

		//                    // Probe for values
		//                    GetUserSettingsResponse userresponse = autodiscoverService.GetUserSettings(lblEmail.Text,
		//                                                                                               UserSettingName.InternalWebClientUrls,
		//                                                                                               UserSettingName.InternalEwsUrl,
		//                                                                                               UserSettingName.InternalMailboxServer,
		//                                                                                               UserSettingName.UserDisplayName);

		//                    // OWA Url
		//                    var col = (WebClientUrlCollection) userresponse.Settings[UserSettingName.InternalWebClientUrls];
		//                    WebClientUrl owaUrl = col.Urls[0];
		//                    _reportedOwaUrl = owaUrl.Url;
		//                    UpdateOwaUrl();
		//                    AddLogEntry(OWAtray.Autodiscovered_OWA_Url + " " + _reportedOwaUrl, Severity.Success);

		//                    // EWS Url
		//                    _reportedEwsUrl = (string) userresponse.Settings[UserSettingName.InternalEwsUrl];
		//                    UpdateUrl();
		//                    AddLogEntry(OWAtray.Autodiscovered_EWS_Url + " " + _reportedEwsUrl, Severity.Success);

		//                    // Mailbox
		//                    _reportedMailboxServer = (string) userresponse.Settings[UserSettingName.InternalMailboxServer];
		//                    AddLogEntry(OWAtray.Autodiscovered_Mailbox_Server + " " + _reportedMailboxServer, Severity.Success);

		//                    // User Name
		//                    _reportedUserName = (string) userresponse.Settings[UserSettingName.UserDisplayName];
		//                    AddLogEntry(OWAtray.Autodiscovered_User_Name + " " + _reportedUserName, Severity.Success);
		//                }
		//                else
		//                {
		//                    // External (default)
		//                    AddLogEntry(OWAtray.Endpoint_is_OUTSIDE_corporate);

		//                    // Probe for values
		//                    GetUserSettingsResponse userresponse = autodiscoverService.GetUserSettings(lblEmail.Text,
		//                                                                                               UserSettingName.ExternalWebClientUrls,
		//                                                                                               UserSettingName.ExternalEwsUrl,
		//                                                                                               UserSettingName.ExternalMailboxServer,
		//                                                                                               UserSettingName.UserDisplayName);

		//                    // OWA Url
		//                    var owaCollection = (WebClientUrlCollection) userresponse.Settings[UserSettingName.ExternalWebClientUrls];
		//                    WebClientUrl owaUrl = owaCollection.Urls[0];
		//                    _reportedOwaUrl = owaUrl.Url;
		//                    UpdateOwaUrl();
		//                    AddLogEntry(OWAtray.Autodiscovered_OWA_Url + " " + _reportedOwaUrl, Severity.Success);

		//                    // EWS Url
		//                    _reportedEwsUrl = (string) userresponse.Settings[UserSettingName.ExternalEwsUrl];
		//                    UpdateUrl();
		//                    AddLogEntry(OWAtray.Autodiscovered_EWS_Url + " " + _reportedEwsUrl, Severity.Success);

		//                    // Mailbox
		//                    _reportedMailboxServer = (string) userresponse.Settings[UserSettingName.ExternalMailboxServer];
		//                    AddLogEntry(OWAtray.Autodiscovered_Mailbox_Server + " " + _reportedMailboxServer, Severity.Success);

		//                    // User Name
		//                    _reportedUserName = (string) userresponse.Settings[UserSettingName.UserDisplayName];
		//                    AddLogEntry(OWAtray.Autodiscovered_User_Name + " " + _reportedUserName, Severity.Success);
		//                }
		//            }
		//        }
		//        else
		//        {
		//            if (lblUrl.Text.Length == 0)
		//            {
		//                AddLogEntry(OWAtray.Can_establish_valid_URL_for, Severity.Fail);
		//            }
		//            else
		//            {
		//                var myUri = new Uri(lblUrl.Text);
		//                _myService.Url = myUri;

		//                // Update server settings
		//                _reportedVersion = _myService.RequestedServerVersion;
		//                AddLogEntry(OWAtray.Connected_to + _reportedVersion, Severity.Success);

		//                // Update properties
		//                _reportedMailboxServer = txtServer.Text;
		//                _reportedUserName = (chkOnDomain.Checked ? "" : (txtUser.Text.Length == 0 ? txtEmail.Text : txtUser.Text));
		//            }
		//        }

		//        // Set a flag to indicate that subsequent runs can autostart
		//        Settings.Default.FirstTime = false;
		//        Settings.Default.Save();

		//        // All clear
		//        return true;
		//    }
		//    catch (Exception ex)
		//    {
		//        AddLogEntry(OWAtray.Error + ex.Message, Severity.Fail);
		//        return false;
		//    }
		//    finally
		//    {
		//        // Cursor
		//        Cursor = Cursors.Default;
		//    }
		//}

		private void ConfigureShell()
		{
			// Set OWA Url
			string owaUrl = lblOWAUrl.Text;

			try
			{
				var runSvc = new ProcessStartInfo(_shellPath) {Arguments = "url " + owaUrl, WindowStyle = ProcessWindowStyle.Hidden};
				Process serviceProcess = Process.Start(runSvc);

				while (!serviceProcess.HasExited)
				{
					Thread.Sleep(100);
					Application.DoEvents();
				}
			}
			catch (Exception ex)
			{
				AddLogEntry(OWAtray.Error + ex.Message, Severity.Fail);
				return;
			}

			try
			{
				var runSvc = new ProcessStartInfo(_shellPath)
				             	{Arguments = "account ", WindowStyle = ProcessWindowStyle.Hidden};
				Process serviceProcess = Process.Start(runSvc);

				while (!serviceProcess.HasExited)
				{
					Thread.Sleep(100);
					Application.DoEvents();
				}
			}
			catch (Exception ex)
			{
				AddLogEntry(OWAtray.Error + ex.Message, Severity.Fail);
				return;
			}

			try
			{
				var runSvc = new ProcessStartInfo(_shellPath)
				             	{Arguments = "exchange " + _connection.Version, WindowStyle = ProcessWindowStyle.Hidden};
				Process serviceProcess = Process.Start(runSvc);

				while (!serviceProcess.HasExited)
				{
					Thread.Sleep(100);
					Application.DoEvents();
				}
			}
			catch (Exception ex)
			{
				AddLogEntry(OWAtray.Error + ex.Message, Severity.Fail);
				return;
			}

			// Set Password
			try
			{
				var runSvc = new ProcessStartInfo(_shellPath)
				             	{Arguments = "password " + txtPwd.Text, WindowStyle = ProcessWindowStyle.Hidden};
				Process serviceProcess = Process.Start(runSvc);

				while (!serviceProcess.HasExited)
				{
					Thread.Sleep(100);
					Application.DoEvents();
				}
			}
			catch (Exception ex)
			{
				AddLogEntry(OWAtray.Error + ex.Message, Severity.Fail);
				return;
			}

			// Set Autologin
			try
			{
				var runSvc = new ProcessStartInfo(_shellPath)
				             	{
				             		Arguments = "autologin " + (Settings.Default.AutoLogin ? "Yes" : "No"),
				             		WindowStyle = ProcessWindowStyle.Hidden
				             	};
				Process serviceProcess = Process.Start(runSvc);

				while (!serviceProcess.HasExited)
				{
					Thread.Sleep(100);
					Application.DoEvents();
				}
			}
			catch (Exception ex)
			{
				AddLogEntry(OWAtray.Error + ex.Message, Severity.Fail);
				return;
			}

			// Set Browser
			try
			{
				var runSvc = new ProcessStartInfo(_shellPath)
				             	{
				             		Arguments = "browser " + (Settings.Default.AlwaysIE ? "Yes" : "No"),
				             		WindowStyle = ProcessWindowStyle.Hidden
				             	};
				Process serviceProcess = Process.Start(runSvc);

				while (!serviceProcess.HasExited)
				{
					Thread.Sleep(100);
					Application.DoEvents();
				}
			}
			catch (Exception ex)
			{
				AddLogEntry(OWAtray.Error + ex.Message, Severity.Fail);
				return;
			}
		}

		private void disableCalendarToolStripMenuItem_CheckStateChanged(object sender, EventArgs e)
		{
			if (_startingUp) return;

			Settings.Default.DisableCalendar = disableCalendarToolStripMenuItem.Checked;
			Settings.Default.Save();
			AddLogEntry(
				OWAtray.Calendar_notifications_switched + " " + (Settings.Default.DisableCalendar ? OWAtray.OFF : OWAtray.ON));
		}

		private void exchange2007ToolStripMenuItem_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (_startingUp) return;

			switch (exchange2007ToolStripMenuItem.SelectedIndex)
			{
				case 0:
					Settings.Default.ExchangeVersion = "Autodetect";
					break;

				case 1:
					Settings.Default.ExchangeVersion = "Exchange2007_SP1";
					break;

				case 2:
					Settings.Default.ExchangeVersion = "Exchange2010";
					break;

				case 3:
					Settings.Default.ExchangeVersion = "Exchange2010_SP1";
					break;
			}

			Settings.Default.Save();
		}

		private void exitToolStripMenuItem1_Click(object sender, EventArgs e)
		{
			_overRideClose = true;
			Close();
		}

		private void exitToolStripMenuItem_Click(object sender, EventArgs e)
		{
			_overRideClose = true;
			Close();
		}

		private void FlushOutput()
		{
			// Avoid Illegal Cross Thread Calls
			Invoke(new Action(() =>
			                  	{
			                  		if (_lvBuffer.Count <= 0) return;

			                  		// Avoid buffer overflows by trimming log after n entries
			                  		if (lvStatus.Items.Count >= Settings.Default.ScreenLines) lvStatus.Items.Clear();

			                  		try
			                  		{
			                  			// Copy from buffer to screen control
			                  			lvStatus.BeginUpdate();
			                  			// Note that .AddRange has a bug so avoid
			                  			foreach (ListViewItem lv in _lvBuffer.Where(lv => lv != null))
			                  				lvStatus.Items.Add(lv);
			                  		}
			                  		catch (Exception)
			                  		{
			                  		}
			                  		finally
			                  		{
			                  			// Make newest item visible
			                  			// We don't care about any spurious errors raised here
			                  			if (lvStatus.Items.Count > 0)
			                  			{
			                  				try
			                  				{
			                  					lvStatus.EnsureVisible(lvStatus.Items.Count - 1);
			                  					ListViewItem lv = lvStatus.Items[lvStatus.Items.Count - 1];
			                  					slStatus.Text = lv.SubItems[1].Text;
			                  				}
			                  				catch (Exception)
			                  				{
			                  				}
			                  			}

			                  			// Tidy up
			                  			_lvBuffer.Clear();
			                  			lvStatus.EndUpdate();
			                  			lvStatus.Refresh();
			                  			Refresh();
			                  		}
			                  	}));
		}

		private void Form1_FormClosed(object sender, FormClosedEventArgs e)
		{
			FormClosed -= Form1_FormClosed;
			AddLogEntry(OWAtray.Terminating);
		}

		private void Form1_FormClosing(object sender, FormClosingEventArgs e)
		{
			if (WindowState != FormWindowState.Minimized && _overRideClose == false)
			{
				e.Cancel = true;
				WindowState = FormWindowState.Minimized;
			}
			else
			{
				FormClosing -= Form1_FormClosing;
				SnarlHelper.Revoke(Handle);
				Application.Exit(e);
			}
		}

		private void Form1_Move(object sender, EventArgs e)
		{
			if (WindowState == FormWindowState.Minimized)
			{
				Hide();
			}
			else
			{
				Show();
			}
		}

		//private int CheckForNewMail()
		//{
		//    int myCount;

		//    if (_myService == null)
		//    {
		//        AddLogEntry(OWAtray.Not_Connected_to_Exchange, Severity.Fail);
		//        notifyIcon1.Text = AssemblyHelpers.AssemblyTitle + Environment.NewLine + OWAtray.Not_Connected_to_Exchange;
		//        return 0;
		//    }

		//    try
		//    {
		//        // Set time for initial run only
		//        if (_firstRun) _timeLastChecked = TimeOfNewestEmail().AddSeconds(1);

		//        // Is there new mail?
		//        var myFolder = Folder.Bind(_myService, WellKnownFolderName.Inbox);
		//        myCount = myFolder.UnreadCount;
		//        if (myCount > _inboxCount)
		//        {
		//            if (_firstRun)
		//            {
		//                PopToast(OWAtray.New_Mail,
		//                         OWAtray.You_have + " " + myCount + " " + OWAtray.unread_email + (myCount != 1 ? "s " : " ") +
		//                         OWAtray.in_your_inbox);
		//            }
		//            else
		//            {
		//                PopUnreadEmail(myCount);
		//            }

		//            _resetFlag = false;
		//        }

		//        if (!_resetFlag)
		//        {
		//            notifyIcon1.Icon = new Icon((myCount > 0 ? _alertIcon : _emailIcon));
		//        }

		//        var text1 = AssemblyHelpers.AssemblyTitle + Environment.NewLine + Environment.NewLine + myCount + " " +
		//                       OWAtray.unread_email + (myCount != 1 ? "s " : " ");
		//        const int maxTipLength = 63;
		//        var charsLeft = maxTipLength - text1.Length;
		//        var domainText = _reportedMailboxServer + @"\" + _reportedUserName;
		//        if (domainText.Length > charsLeft) domainText = domainText.Substring(0, charsLeft);
		//        var finalText = AssemblyHelpers.AssemblyTitle + Environment.NewLine + domainText + Environment.NewLine + myCount +
		//                           " " + OWAtray.unread_email + (myCount != 1 ? "s " : " ");
		//        notifyIcon1.Text = finalText;
		//        _inboxCount = myCount;
		//    }
		//    catch (Exception ex)
		//    {
		//        AddLogEntry(OWAtray.Error + ex.Message, Severity.Fail);
		//        myCount = 0;
		//        stopMonitoring();
		//        StartRetryTimer();
		//    }
		//    finally
		//    {
		//        _firstRun = false;
		//    }

		//    return myCount;
		//}

		private void growlToolStripMenuItem_CheckStateChanged(object sender, EventArgs e)
		{
			if (_startingUp) return;

			Settings.Default.Growl = growlToolStripMenuItem.Checked;
			Settings.Default.Save();
			AddLogEntry(OWAtray.Growl_notifications_switched + " " + (Settings.Default.Growl ? OWAtray.ON : OWAtray.OFF));
		}

		private static void InitEventView(ListView lvX)
		{
			lvX.Columns.Add(OWAtray.Time, 140, HorizontalAlignment.Left);
			lvX.Columns.Add(OWAtray.Event_Details, 1000, HorizontalAlignment.Left);
			lvX.Items.Clear();
		}

		private void makeOWADefaultToolStripMenuItem_Click(object sender, EventArgs e)
		{
			// Update shell parameters
			ConfigureShell();

			if (!IsUserAdministrator())
			{
				AddLogEntry(OWAtray.You_are_not_an_Admin_user, Severity.Fail);
			}

			// Configure registry
			AddLogEntry(OWAtray.Setting_up_Mail_handlers);

			try
			{
				var runSvc = new ProcessStartInfo(_shellPath) {Arguments = "registry", WindowStyle = ProcessWindowStyle.Hidden};
				if (Environment.OSVersion.Version.Major >= 6)
					runSvc.Verb = "runas";
				Process serviceProcess = Process.Start(runSvc);

				while (!serviceProcess.HasExited)
				{
					Thread.Sleep(100);
					Application.DoEvents();
				}
			}
			catch (Exception ex)
			{
				AddLogEntry(OWAtray.Error + ex.Message, Severity.Fail);
				return;
			}

			AddLogEntry(OWAtray.Mail_functions_will_now_be, Severity.Success);
		}

		private void mDACVersionsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (_frmMdac == null) _frmMdac = new MdaCversions();
			_frmMdac.ShowDialog();
		}

		private void nETVersionsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (_frmNet == null) _frmNet = new NeTversions();
			_frmNet.ShowDialog();
		}

		private void notifyIcon1_BalloonTipClicked(object sender, EventArgs e)
		{
			if (MouseButtons != MouseButtons.Left) return;

			ActivateOwa();
			_popUrl = "";
		}

		private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
		{
			_popUrl = "";
			ActivateOwa();
		}

		private void openOutlookToolStripMenuItem_Click(object sender, EventArgs e)
		{
			try
			{
				Process.Start(Settings.Default.OutlookPath);
			}
			catch (Exception ex)
			{
				AddLogEntry(ex.Message, Severity.Fail);
			}
		}

		private void openOWAToolStripMenuItem_Click(object sender, EventArgs e)
		{
			_popUrl = "";
			ActivateOwa();
		}

		private void overrideToolStripMenuItem_CheckStateChanged(object sender, EventArgs e)
		{
			if (_startingUp) return;

			Settings.Default.OverrideCert = overrideToolStripMenuItem.Checked;
			Settings.Default.Save();
			AddLogEntry(OWAtray.SSL_Certificate_override + " " + (Settings.Default.OverrideCert ? OWAtray.ON : OWAtray.OFF));
		}

		private void playSoundToolStripMenuItem_CheckStateChanged(object sender, EventArgs e)
		{
			if (_startingUp) return;

			Settings.Default.Bell = playSoundToolStripMenuItem.Checked;
			Settings.Default.Save();
			AddLogEntry(OWAtray.Audible_notifications_switched + " " + (Settings.Default.Bell ? OWAtray.ON : OWAtray.OFF));
		}

		private void PopToast(string myTitle, string myMessage)
		{
			// Belt & Braces
			if (myTitle.Length == 0) myTitle = OWAtray.No_Title;
			if (myMessage.Length == 0) myMessage = OWAtray.No_Subject;

			AddLogEntry(myTitle);

			// Store for recall
			_lastPopTitle = myTitle;
			_lastPopMessage = myMessage;
			_lastPopUrl = _popUrl;

			//Balloon
			if (Settings.Default.Balloon)
			{
				notifyIcon1.Tag = _popUrl;
				notifyIcon1.ShowBalloonTip(5000, myTitle, myMessage, ToolTipIcon.Info);
			}

			// Growl
			if (Settings.Default.Growl)
			{
				GrowlHelper.PopGrowl(myTitle, myMessage);
			}

			// Snarl
			if (Settings.Default.Snarl)
			{
				SnarlHelper.PopSnarl(myTitle, myMessage, _graphicPath, Handle);
			}

			// Audible
			if (Settings.Default.Bell)
			{
				AudioHelper.Play(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
				                              Settings.Default.SoundFile));
			}
		}

		//private int PopUnreadEmail(int unreadCount)
		//{
		//    // Set the offset for the paged search.
		//    int offset = 0;
		//    int count = 0;

		//    // Set the page size.
		//    int pageSize = Settings.Default.PageSize;

		//    // Set the flag that indicates whether to continue iterating through additional pages.
		//    bool moreItems = true;

		//    // Continue paging while there are more items to page.
		//    while (moreItems)
		//    {
		//        // Define filters collection
		//        var filters = new SearchFilter.SearchFilterCollection(LogicalOperator.And)
		//                        {
		//                            new SearchFilter.IsEqualTo(EmailMessageSchema.IsRead, false),
		//                            new SearchFilter.IsGreaterThan(ItemSchema.DateTimeReceived, _timeLastChecked)
		//                        };

		//        // Item view
		//        var view = new ItemView(pageSize, offset, OffsetBasePoint.Beginning)
		//                    {
		//                        PropertySet = new PropertySet(BasePropertySet.IdOnly) {ItemSchema.Subject, ItemSchema.DateTimeReceived}
		//                    };
		//        view.OrderBy.Add(ItemSchema.DateTimeReceived, SortDirection.Descending);

		//        // Now search
		//        FindItemsResults<Item> findResults = _myService.FindItems(WellKnownFolderName.Inbox, filters, view);

		//        // Process each item.
		//        bool allDone = false;
		//        bool isFlagged = false;
		//        foreach (Item myItem in findResults.Items)
		//        {
		//            if (++count > Settings.Default.MaxNotify)
		//            {
		//                if (!allDone)
		//                {
		//                    PopToast(OWAtray.Too_much_mail,
		//                             OWAtray.There_are + " " + (unreadCount - Settings.Default.MaxNotify) + " " +
		//                             OWAtray.other_new_emails);
		//                    allDone = true;
		//                }
		//            }
		//            else
		//            {
		//                var myEmail = myItem as EmailMessage;
		//                if (myEmail != null)
		//                {
		//                    DateTime myTime = DateTime.Now;

		//                    try
		//                    {
		//                        var ps = new PropertySet(BasePropertySet.FirstClassProperties);
		//                        myEmail.Load(ps);
		//                        string mySender = myEmail.Sender.Name;
		//                        string mySubject = (myEmail.Subject ?? OWAtray.No_Subject);
		//                        myTime = myEmail.DateTimeReceived;
		//                        _popUrl = (_reportedVersion == ExchangeVersion.Exchange2007_SP1 ? "" : myEmail.WebClientReadFormQueryString);
		//                        PopToast(OWAtray.New_Mail_from + " " + mySender, mySubject);
		//                    }
		//                    catch (Exception ex)
		//                    {
		//                        AddLogEntry(OWAtray.Error_when_getting_email + ex.Message, Severity.Fail);
		//                    }

		//                    // Update flag
		//                    if (!isFlagged)
		//                    {
		//                        _timeLastChecked = myTime.AddSeconds(1);
		//                        isFlagged = true;
		//                    }
		//                }
		//            }
		//        }

		//        // Set the flag to discontinue paging.
		//        if (!findResults.MoreAvailable)
		//            moreItems = false;

		//        // Update the offset if there are more items to page.
		//        if (moreItems)
		//            offset = offset + pageSize;
		//    }

		//    return count;
		//}

		private void recallLastPopupToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (_lastPopMessage.Length <= 0 || _lastPopTitle.Length <= 0) return;

			_popUrl = _lastPopUrl;
			PopToast(_lastPopTitle, _lastPopMessage);
		}

		private void resetTrayIconToolStripMenuItem_Click(object sender, EventArgs e)
		{
			notifyIcon1.Icon = new Icon(_emailIcon);
			_resetFlag = true;
		}

		private void restoreToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (WindowState == FormWindowState.Minimized)
			{
				Show();
				WindowState = FormWindowState.Normal;
			}

			// Activate the form.
			Activate();
			Focus();
		}

		private void snarlToolStripMenuItem_CheckStateChanged(object sender, EventArgs e)
		{
			if (_startingUp) return;

			Settings.Default.Snarl = snarlToolStripMenuItem.Checked;
			Settings.Default.Save();
			AddLogEntry(OWAtray.Snarl_notifications_switched + " " + (Settings.Default.Snarl ? OWAtray.ON : OWAtray.OFF));
		}

		private void StartMonitoring()
		{
			// Start Timer
			//timerAppt.Start();
			//timerUpdate.Interval = Settings.Default.UpdateInterval*1000;
			//timerUpdate.Start();
			AddLogEntry(txtInterval.Text + " " + OWAtray.second_timer_started);

			// Minimise to tray
			WindowState = FormWindowState.Minimized;

			// Configure Shell
			ConfigureShell();

			// Initial Check
			//CheckForNewMail();
			if (!Settings.Default.DisableCalendar)
			{
				//CheckForAppointments();
			}
		}

		private void supportToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (_frmContact == null) _frmContact = new ContactUs();
			_frmContact.ShowDialog();
		}

		private void switchOffToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (!IsUserAdministrator())
			{
				AddLogEntry(OWAtray.You_are_not_an_Admin_user, Severity.Fail);
			}

			// Configure registry
			AddLogEntry(OWAtray.Restoring_Mail_handlers);

			try
			{
				var runSvc = new ProcessStartInfo(_shellPath) {Arguments = "restore", WindowStyle = ProcessWindowStyle.Hidden};
				if (Environment.OSVersion.Version.Major >= 6)
					runSvc.Verb = "runas";
				Process serviceProcess = Process.Start(runSvc);

				while (!serviceProcess.HasExited)
				{
					Thread.Sleep(100);
					Application.DoEvents();
				}
			}
			catch (Exception ex)
			{
				AddLogEntry(OWAtray.Error + ex.Message, Severity.Fail);
				return;
			}

			AddLogEntry(OWAtray.Mail_handler_restored_to_system, Severity.Success);
		}

		private void systemInformationToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (_frmInfo == null) _frmInfo = new SysInfo();
			_frmInfo.ShowDialog();
		}

		private void timer1_Tick(object sender, EventArgs e)
		{
			timer1.Enabled = false;
			StartMonitoring();
		}

		private void timerLogging_Tick(object sender, EventArgs e)
		{
			FlushOutput();
		}

		private void txtInterval_Validated(object sender, EventArgs e)
		{
			Settings.Default.UpdateInterval = Convert.ToInt32(txtInterval.Text);
			Settings.Default.Save();
		}

		private void txtInterval_Validating(object sender, CancelEventArgs e)
		{
			int result;

			if (int.TryParse(txtInterval.Text, out result))
			{
				if (result >= 1 && result <= MaxInterval)
				{
					errorProvider1.SetError(txtInterval, "");
					e.Cancel = false;
				}
				else
				{
					errorProvider1.SetError(txtInterval,
					                        OWAtray.Must_be_numeric_value_between + " " +
					                        MaxInterval.ToString(CultureInfo.InvariantCulture));
					e.Cancel = true;
				}
			}
			else
			{
				errorProvider1.SetError(txtInterval,
				                        OWAtray.Must_be_numeric_value_between + " " +
				                        MaxInterval.ToString(CultureInfo.InvariantCulture));
				e.Cancel = true;
			}
		}

		private void UpdateUrl()
		{
			if (Settings.Default.Autodiscovery && _reportedEwsUrl.Length > 0)
			{
				lblUrl.Text = _reportedEwsUrl;
			}
			else if (Settings.Default.OverrideURL && txtURLEdit.Text.Length > 0)
			{
				lblUrl.Text = txtURLEdit.Text;
			}
			else if (txtServer.Text.Length > 0)
			{
				lblUrl.Text = string.Format("{0}{1}{2}", "https://", txtServer.Text, "/ews/exchange.asmx");
			}
			else
			{
				lblUrl.Text = OWAtray.unknown;
			}
		}

		private void UpdateEmail()
		{
			lblEmail.Text = EmailAddress;

			if (!_startingUp)
			{
				ConfigureShell();
			}
		}

		private void loginAutomaticallyToolStripMenuItem_CheckStateChanged(object sender, EventArgs e)
		{
			if (_startingUp) return;

			Settings.Default.AutoLogin = loginAutomaticallyToolStripMenuItem.Checked;
			Settings.Default.Save();
			AddLogEntry(OWAtray.Automatic_Login_is_switched + " " + (Settings.Default.AutoLogin ? OWAtray.ON : OWAtray.OFF));

			ConfigureShell();
		}

		private void txtDomain_Validated(object sender, EventArgs e)
		{
			Settings.Default.Domain = txtDomain.Text;
			Settings.Default.Save();
		}

		private void txtPwd_Validated(object sender, EventArgs e)
		{
			Settings.Default.Password = (txtPwd.Text.Length > 0 ? txtPwd.Text.Encrypt() : "");
			Settings.Default.Save();
		}

		private void txtServer_Validated(object sender, EventArgs e)
		{
			Settings.Default.Server = txtServer.Text;
			Settings.Default.Save();
			UpdateUrl();
			UpdateOwaUrl();
		}

		private void txtUser_Validated(object sender, EventArgs e)
		{
			Settings.Default.Username = txtUser.Text;
			Settings.Default.Save();
			UpdateEmail();
		}

		private void txtEmail_Validated(object sender, EventArgs e)
		{
			Settings.Default.EMail = txtEmail.Text;
			Settings.Default.Save();
			UpdateEmail();
		}

		private void overrideAutodiscoveryValidationToolStripMenuItem_CheckStateChanged(object sender, EventArgs e)
		{
			if (_startingUp) return;

			Settings.Default.OverrideValidation = overrideAutodiscoveryValidationToolStripMenuItem.Checked;
			Settings.Default.Save();
			AddLogEntry(
				OWAtray.Autodiscovery_Validation + " " + (Settings.Default.OverrideValidation ? OWAtray.ON : OWAtray.OFF));
		}

		private void office365LoginOverrideToolStripMenuItem_CheckStateChanged(object sender, EventArgs e)
		{
			if (_startingUp) return;

			Settings.Default.UseOffice365 = office365LoginOverrideToolStripMenuItem.Checked;
			Settings.Default.Save();
			UpdateOwaUrl();
			AddLogEntry(OWAtray.Office_login_override + " " + (Settings.Default.UseOffice365 ? OWAtray.ON : OWAtray.OFF));
		}

		private void txtURLEdit_Validated(object sender, EventArgs e)
		{
			Settings.Default.ManualURL = txtURLEdit.Text;
			Settings.Default.Save();
			UpdateUrl();
		}

		private void cbOverrideEWS_CheckedChanged(object sender, EventArgs e)
		{
			if (_startingUp) return;

			txtURLEdit.Enabled = cbOverrideEWS.Checked;

			Settings.Default.OverrideURL = cbOverrideEWS.Checked;
			Settings.Default.Save();
			UpdateUrl();
			AddLogEntry(OWAtray.EWS_URL_override_switched + " " + (Settings.Default.OverrideURL ? OWAtray.ON : OWAtray.OFF));
		}

		private void chkAutodiscovery_CheckedChanged(object sender, EventArgs e)
		{
			if (_startingUp) return;

			Settings.Default.Autodiscovery = chkAutodiscovery.Checked;
			Settings.Default.Save();
			AddLogEntry(OWAtray.Autodiscovery_is_switched + " " + (chkAutodiscovery.Checked ? OWAtray.ON : OWAtray.OFF));

			// Switch off some options when Autodiscovery is checked
			SelectAutodiscoveryOptions();

			// Re-evaluate settings
			UpdateUrl();
			UpdateOwaUrl();
			UpdateEmail();
		}

		private void cbOverrideOWA_CheckedChanged(object sender, EventArgs e)
		{
			if (_startingUp) return;

			txtOWAEdit.Enabled = cbOverrideOWA.Checked;

			Settings.Default.OverrideOWAUrl = cbOverrideOWA.Checked;
			Settings.Default.Save();
			UpdateOwaUrl();
			AddLogEntry(OWAtray.OWA_URL_override_switched + " " + (Settings.Default.OverrideOWAUrl ? OWAtray.ON : OWAtray.OFF));
		}

		private void txtOWAEdit_Validated(object sender, EventArgs e)
		{
			Settings.Default.ManualOWAUrl = txtOWAEdit.Text;
			Settings.Default.Save();
			UpdateOwaUrl();
		}

		private void cbOverrideEWS_EnabledChanged(object sender, EventArgs e)
		{
			if (!cbOverrideEWS.Enabled)
				txtURLEdit.Enabled = false;
			else
			{
				if (cbOverrideEWS.Checked)
					txtURLEdit.Enabled = true;
			}
		}

		private void cbOverrideOWA_EnabledChanged(object sender, EventArgs e)
		{
			if (!cbOverrideOWA.Enabled)
				txtOWAEdit.Enabled = false;
			else
			{
				if (cbOverrideOWA.Checked)
					txtOWAEdit.Enabled = true;
			}
		}

		private void showLogFileToolStripMenuItem_Click(object sender, EventArgs e)
		{
			try
			{
				Process.Start(Logger.Filename);
			}
			catch (Exception ex)
			{
				AddLogEntry(ex.Message, Severity.Fail);
			}
		}

		private void useDefaultWebProxyToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (_startingUp) return;

			Settings.Default.UseWebProxy = useDefaultWebProxyToolStripMenuItem.Checked;
			Settings.Default.Save();
		}
	}
}