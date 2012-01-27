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
using System.Reflection;
using System.Security.Principal;
using System.Threading;
using System.Windows.Forms;
using DrunkenBakery.OWAtray.Audio;
using DrunkenBakery.OWAtray.Connections.Abstract;
using DrunkenBakery.OWAtray.Connections.Proxy;
using DrunkenBakery.OWAtray.Framework;
using DrunkenBakery.OWAtray.GUI.Properties;
using DrunkenBakery.OWAtray.Growl;
using DrunkenBakery.OWAtray.Logging;
using DrunkenBakery.OWAtray.Snarl;

namespace DrunkenBakery.OWAtray.GUI
{
	public partial class Form1 : Form
	{
		private const int MaxInterval = 3600;

		private readonly List<ListViewItem> _lvBuffer = new List<ListViewItem>();
		private static bool _overRideClose;
		private string _alertIcon;
		private string _emailIcon;
		private string _graphicPath;
		private string _shellPath;
		private bool _booting;
		private Form _frmAbout;
		private Form _frmChangeLog;
		private Form _frmContact;
		private Form _frmInfo;
		private Form _frmMdac;
		private Form _frmNet;
		private string _lastPopMessage = "";
		private string _lastPopTitle = "";
		private string _lastPopUrl = "";
		private string _popUrl = "";
		private bool _resetFlag = false;

		// New variables
		private Scenario _scenario;
		private IEmailInterface _connection;

		public Form1()
		{
			InitializeComponent();

			// Set up look & feel
			WindowDressing();

			// Start Logging
			Logger.Execute();

			// Welcome message
			AddLogEntry(string.Format("{0} {1} v{2}", OWAtray.Welcome_to_the, AssemblyHelpers.AssemblyTitle,
			                          AssemblyHelpers.UpgradeSettings()));

			// The rest gets kicked off an a timer
			AddLogEntry("Ready.");
			timer1.Start();
		}

		private void timer1_Tick(object sender, EventArgs e)
		{
			timer1.Enabled = false;
			timerLogging.Enabled = true;

			// Interlock for booting up
			_booting = true;

			// Boot the various subsystems
			BootEnvironment();
			BootShell();
			BootScenario();
			BootIcons();
			BootHelpers();

			// Connect if autostart is good to go
			if (Settings.Default.Autostart)
			{
				//WindowState = FormWindowState.Minimized;
				ConnectToExchange();
			}

			// Release boot interlock
			_booting = false;
		}

		private void BootHelpers()
		{
			GrowlHelper.RegisterGrowl(AssemblyHelpers.AssemblyTitle, _graphicPath, "NEWMAIL", "New Mail");
			SnarlHelper.RegisterSnarl(AssemblyHelpers.AssemblyTitle, _graphicPath, Handle);			
		}

		private void BootShell()
		{
			_shellPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
									  Settings.Default.ShellIntegration);			
		}

		private void BootIcons()
		{
			_graphicPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
										Settings.Default.EmailGraphic);
			_emailIcon = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), Settings.Default.EmailIcon);
			_alertIcon = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), Settings.Default.AlertIcon);

			// Tray icon
			notifyIcon1.Icon = new Icon(_emailIcon);			
		}

		private void BootEnvironment()
		{
			// Startup Flag
			chkRunOnStartup.Checked = WindowsShortcut.Exists(Environment.SpecialFolder.Startup,
															 AssemblyHelpers.AssemblyTitle);

			// Notifications
			balloonToolStripMenuItem.Checked = Settings.Default.Balloon;
			growlToolStripMenuItem.Checked = Settings.Default.Growl;
			snarlToolStripMenuItem.Checked = Settings.Default.Snarl;
			playSoundToolStripMenuItem.Checked = Settings.Default.Bell;		
	
			// Web Proxy
			useDefaultWebProxyToolStripMenuItem.Checked = Settings.Default.UseWebProxy;
			UpdateWebProxySettings();

			// Lockdown mode
			restoreToolStripMenuItem.Enabled = (!Settings.Default.LockDown);
		}

		private void BootScenario()
		{
			// Create our whole universe
			_scenario = ScenarioFactory.CreateScenario(Settings.Default.ScenarioFile);

			// TODO: Special case for single connection use only
			if (_scenario.Connections.Count == 0)
			{
				// Create the new entry
				_connection = ConnectionFactory.CreateConnection(EmailType.Exchange);
				_scenario.Connections.Add(_connection);
				_scenario.Save();
			}
			else
			{
				// Retrieve all the settings
				_connection = _scenario.Connections[0];
			}

			// Update any UI
			txtEmail.Text = _connection.EmailAddress;
			txtUser.Text = _connection.Username;
			txtPwd.Text = _connection.Password;
			txtServer.Text = _connection.EmailServer;
			txtDomain.Text = _connection.AccountDomain;
			txtURLEdit.Text = _connection.ServiceUrl;
			txtOWAEdit.Text = _connection.EmailUrl;
			txtInterval.Text = _connection.Interval.ToString(CultureInfo.InvariantCulture);
			cbOverrideEWS.Checked = _connection.OverrideServiceUrl;
			txtURLEdit.Enabled = cbOverrideEWS.Checked;
			cbOverrideOWA.Checked = _connection.OverrideEmailUrl;
			txtOWAEdit.Enabled = cbOverrideOWA.Checked;
			chkAutodiscovery.Checked = _connection.UseAutodiscovery;
			SelectAutodiscoveryOptions();
			chkOnDomain.Checked = _connection.OnWindowsDomain;
			SelectDomainOptions();
			overrideCertificateToolStripMenuItem.Checked = _connection.OverrideCertificate;
			alwaysOpenOWAInIEToolStripMenuItem.Checked = _connection.AlwaysUseInternetExplorer;
			disableCalendarToolStripMenuItem.Checked = _connection.DisableCalendar;
			loginAutomaticallyToolStripMenuItem.Checked = _connection.AutoLogin;
			office365LoginOverrideToolStripMenuItem.Checked = _connection.OverrideOffice365Login;
			overrideAutodiscoveryValidationToolStripMenuItem.Checked = _connection.OverrideAutodiscoveryValidation;
			exchangeToolStripMenuItem.SelectedIndex = exchangeToolStripMenuItem.FindStringExact(_connection.ServerVersion);
			UpdateServiceUrl();
			UpdateOwaUrl();
			UpdateEmail();

			// Update shell handler
			ConfigureShell();

			// Set up event handling
			WireUpConnectionEvents();
		}

		private string EmailAddress
		{
			get
			{
				var email = (txtEmail.Text.Length > 0) ? txtEmail.Text : txtUser.Text;
				if (email.Length > 0 && !email.Contains("@"))
				{
					email = email + "@" + GetSubDomain(txtServer.Text);
				}

				return email;
			}
		}

		private void WindowDressing()
		{
			Text = string.Format("{0}{1}{2}", AssemblyHelpers.AssemblyTitle, OWAtray.Form1_WindowDressing__freshly_baked_at_, AssemblyHelpers.AssemblyCompany);
			notifyIcon1.Text = AssemblyHelpers.AssemblyTitle + Environment.NewLine + OWAtray.Not_Connected_to_Exchange;
			foreach (TabPage tab in tabMain.TabPages) tab.BackColor = SystemColors.Control;
			InitEventView(lvStatus);
		}

		private void SelectDomainOptions()
		{
			txtDomain.Enabled = !_connection.OnWindowsDomain;
			txtPwd.Enabled = !_connection.OnWindowsDomain;
			txtUser.Enabled = !_connection.OnWindowsDomain;
		}

		private void SelectAutodiscoveryOptions()
		{
			txtServer.Enabled = !_connection.UseAutodiscovery;
			cbOverrideEWS.Enabled = !_connection.UseAutodiscovery;
			cbOverrideOWA.Enabled = !_connection.UseAutodiscovery;
			txtDomain.Enabled = !_connection.UseAutodiscovery;
			overrideAutodiscoveryValidationToolStripMenuItem.Enabled = _connection.UseAutodiscovery;
		}

		private void WireUpConnectionEvents()
		{
			foreach (var item in _scenario.Connections.Where(item => !item.IsLogEventDefined))
			{
				// Logging event
				item.LogMessage += AddLogEntry;

				// State change event
				var itemCopy = item;
				item.ConnectedStateChange += (connection, state) =>
				{
					switch (state)
					{
						case ConnectionState.Connecting:
							AddLogEntry(string.Format("[{0}] - Connecting, please wait...", _connection.EmailAddress));
							break;
						
						case ConnectionState.Disconnecting:
							AddLogEntry(string.Format("[{0}] - Disconnecting...", _connection.EmailAddress));
							break;

						case ConnectionState.Failed:
							// Switch off autostart if we can't connect
							Settings.Default.Autostart = false;
							Settings.Default.Save();
							break;

						case ConnectionState.Connected:
							// After a successful connection then next time we can autostart
							Settings.Default.Autostart = true;
							Settings.Default.Save();

							// Update discovered properties
							if (_connection.DiscoveredUsername.Length > 0)
							{
								AddLogEntry(OWAtray.Autodiscovered_User_Name + " " + _connection.DiscoveredUsername, Severity.Success);
							}
							if (_connection.DiscoveredEmailServer.Length > 0)
							{
								AddLogEntry(OWAtray.Autodiscovered_Mailbox_Server + " " + _connection.DiscoveredEmailServer, Severity.Success);
								UpdateOwaUrl();
							}
							if (_connection.DiscoveredEmailUrl.Length > 0)
							{
								AddLogEntry(OWAtray.Autodiscovered_OWA_Url + " " + _connection.DiscoveredEmailUrl, Severity.Success);
								UpdateServiceUrl();
							}
							if (_connection.DiscoveredServiceUrl.Length > 0)
							{
								AddLogEntry(OWAtray.Autodiscovered_EWS_Url + " " + _connection.DiscoveredServiceUrl, Severity.Success);
							}

							// Configure Shell
							ShellExchangeVersion();

							AddLogEntry(
								string.Format("[{0}] - Connected to {1}", connection.EmailAddress,
												connection.Version), Severity.Success);
							break;

						case ConnectionState.Disconnected:
							AddLogEntry(string.Format("[{0}] - Disconnected", connection.EmailAddress),
										Severity.Success);
							notifyIcon1.Text = AssemblyHelpers.AssemblyTitle + Environment.NewLine + OWAtray.Not_Connected_to_Exchange;
							break;
					}
				};

				// New mail event
				item.NewMail += (arrivalTime, subject, sender) => AddLogEntry(string.Format("New mail from {0} - {1}", sender, subject));
			}
		}

		private void UpdateOwaUrl()
		{
			if (_connection.OverrideOffice365Login)
			{
				lblOWAUrl.Text = Settings.Default.Office365OwaUrl + StripEmailDomain(lblEmail.Text);
			}
			else if (_connection.UseAutodiscovery && _connection.DiscoveredEmailUrl.Length > 0)
			{
				lblOWAUrl.Text = _connection.DiscoveredEmailUrl;
			}
			else if (_connection.OverrideEmailUrl && txtOWAEdit.Text.Length > 0)
			{
				lblOWAUrl.Text = txtOWAEdit.Text;
			}
			else if (txtServer.Text.Length > 0)
			{
				lblOWAUrl.Text = string.Format("{0}{1}{2}", "https://", txtServer.Text, "/owa/");
			}
			else
			{
				lblOWAUrl.Text = "";
			}

			_connection.DerivedEmailUrl = lblOWAUrl.Text;
			ShellOwaUrl();
		}

		private static string StripEmailDomain(string email)
		{
			var sub = "";
			var start = email.IndexOf("@", StringComparison.Ordinal);
			if (start > 0) sub = email.Substring(start + 1);
			return sub;
		}

		public bool IsUserAdministrator()
		{
			var isAdmin = false;
			try
			{
				//get the currently logged in user
				var user = WindowsIdentity.GetCurrent();
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
			var result = "";

			var parts = domain.Split('.');

			if (parts.Length > 1)
			{
				for (var f = 1; f < parts.Length; ++f)
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

			if (_connection.AlwaysUseInternetExplorer)
			{
				runSvc.Arguments = "owa" + ((_popUrl.Length > 0) ? " " + _popUrl : "");
			}
			else
			{
				runSvc.Arguments = "shell" + ((_popUrl.Length > 0) ? " " + _popUrl : "");
			}

			var serviceProcess = Process.Start(runSvc);

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
			if (_booting) return;

			_connection.AlwaysUseInternetExplorer = alwaysOpenOWAInIEToolStripMenuItem.Checked;
			_scenario.Save();
			ShellBrowserVersion();

			AddLogEntry(OWAtray.Always_use_IE_switched + " " + (_connection.AlwaysUseInternetExplorer ? OWAtray.ON : OWAtray.OFF));
		}

		private void balloonToolStripMenuItem_CheckStateChanged(object sender, EventArgs e)
		{
			if (_booting) return;

			Settings.Default.Balloon = balloonToolStripMenuItem.Checked;
			Settings.Default.Save();
			AddLogEntry(OWAtray.Balloon_notifications_switched + " " + (Settings.Default.Balloon ? OWAtray.ON : OWAtray.OFF));
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
			if (_booting) return;

			_connection.OnWindowsDomain = chkOnDomain.Checked;
			_scenario.Save();

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
			ConnectToExchange();
		}

		private void ConnectToExchange()
		{
			if (_connection.ConnectedState != ConnectionState.Disconnected)
			{
				AddLogEntry("Already connected", Severity.Fail);
				return;
			}

			_connection.ConnectA();
		}

		private void DisconnectFromExchange()
		{
			if (_connection.ConnectedState != ConnectionState.Connected)
			{
				AddLogEntry("Not connected");
				return;
			}

			_connection.Disconnect();
		}

		private void cmdStop_Click(object sender, EventArgs e)
		{
			DisconnectFromExchange();
		}

		private static void UpdateWebProxySettings()
		{
			WebRequest.DefaultWebProxy.Credentials = Properties.Settings.Default.UseWebProxy ? CredentialCache.DefaultCredentials : null;
		}

		private void ShellOwaUrl()
		{
			try
			{
				var runSvc = new ProcessStartInfo(_shellPath) { Arguments = "url " + _connection.DerivedEmailUrl, WindowStyle = ProcessWindowStyle.Hidden };
				var serviceProcess = Process.Start(runSvc);

				while (!serviceProcess.HasExited)
				{
					Thread.Sleep(100);
					Application.DoEvents();
				}
			}
			catch (Exception ex)
			{
				AddLogEntry(OWAtray.Error + ex.Message, Severity.Fail);
			}			
		}

		private void ShellExchangeVersion()
		{
			try
			{
				var runSvc = new ProcessStartInfo(_shellPath) { Arguments = "exchange " + _connection.Version, WindowStyle = ProcessWindowStyle.Hidden };
				var serviceProcess = Process.Start(runSvc);

				while (!serviceProcess.HasExited)
				{
					Thread.Sleep(100);
					Application.DoEvents();
				}
			}
			catch (Exception ex)
			{
				AddLogEntry(OWAtray.Error + ex.Message, Severity.Fail);
			}			
		}

		private void ShellPassword()
		{
			try
			{
				var runSvc = new ProcessStartInfo(_shellPath) { Arguments = "password " + _connection.Password, WindowStyle = ProcessWindowStyle.Hidden };
				var serviceProcess = Process.Start(runSvc);

				while (!serviceProcess.HasExited)
				{
					Thread.Sleep(100);
					Application.DoEvents();
				}
			}
			catch (Exception ex)
			{
				AddLogEntry(OWAtray.Error + ex.Message, Severity.Fail);
			}			
		}

		private void ShellAutologin()
		{
			try
			{
				var runSvc = new ProcessStartInfo(_shellPath)
				{
					Arguments = "autologin " + (_connection.AutoLogin ? "Yes" : "No"),
					WindowStyle = ProcessWindowStyle.Hidden
				};
				var serviceProcess = Process.Start(runSvc);

				while (!serviceProcess.HasExited)
				{
					Thread.Sleep(100);
					Application.DoEvents();
				}
			}
			catch (Exception ex)
			{
				AddLogEntry(OWAtray.Error + ex.Message, Severity.Fail);
			}			
		}

		private void ShellBrowserVersion()
		{
			try
			{
				var runSvc = new ProcessStartInfo(_shellPath)
				{
					Arguments = "browser " + (_connection.AlwaysUseInternetExplorer ? "Yes" : "No"),
					WindowStyle = ProcessWindowStyle.Hidden
				};
				var serviceProcess = Process.Start(runSvc);

				while (!serviceProcess.HasExited)
				{
					Thread.Sleep(100);
					Application.DoEvents();
				}
			}
			catch (Exception ex)
			{
				AddLogEntry(OWAtray.Error + ex.Message, Severity.Fail);
			}			
		}

		private void ConfigureShell()
		{
			ShellAutologin();
			ShellBrowserVersion();
			ShellExchangeVersion();
			ShellOwaUrl();
			ShellPassword();
		}

		private void disableCalendarToolStripMenuItem_CheckStateChanged(object sender, EventArgs e)
		{
			if (_booting) return;

			_connection.DisableCalendar = disableCalendarToolStripMenuItem.Checked;
			_scenario.Save();

			AddLogEntry(
				OWAtray.Calendar_notifications_switched + " " + (_connection.DisableCalendar ? OWAtray.OFF : OWAtray.ON));
		}

		private void exchangeToolStripMenuItem_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (_booting) return;

			_connection.ServerVersion = exchangeToolStripMenuItem.Text;
			_scenario.Save();
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
			if (_booting) return;

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
				var serviceProcess = Process.Start(runSvc);

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

		private void overrideCertificateToolStripMenuItem_CheckStateChanged(object sender, EventArgs e)
		{
			if (_booting) return;

			_connection.OverrideCertificate = overrideCertificateToolStripMenuItem.Checked;
			_scenario.Save();

			AddLogEntry(OWAtray.SSL_Certificate_override + " " + (_connection.OverrideCertificate ? OWAtray.ON : OWAtray.OFF));
		}

		private void playSoundToolStripMenuItem_CheckStateChanged(object sender, EventArgs e)
		{
			if (_booting) return;

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
			if (_booting) return;

			Settings.Default.Snarl = snarlToolStripMenuItem.Checked;
			Settings.Default.Save();
			AddLogEntry(OWAtray.Snarl_notifications_switched + " " + (Settings.Default.Snarl ? OWAtray.ON : OWAtray.OFF));
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

		private void timerLogging_Tick(object sender, EventArgs e)
		{
			FlushOutput();
		}

		private void txtInterval_Validated(object sender, EventArgs e)
		{
			_connection.Interval = Convert.ToInt32(txtInterval.Text);
			_scenario.Save();
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

		private void UpdateServiceUrl()
		{
			if (_connection.UseAutodiscovery && _connection.DiscoveredServiceUrl.Length > 0)
			{
				lblServiceUrl.Text = _connection.DiscoveredServiceUrl;
			}
			else if (_connection.OverrideServiceUrl && txtURLEdit.Text.Length > 0)
			{
				lblServiceUrl.Text = txtURLEdit.Text;
			}
			else if (txtServer.Text.Length > 0)
			{
				lblServiceUrl.Text = string.Format("{0}{1}{2}", "https://", txtServer.Text, "/ews/exchange.asmx");
			}
			else
			{
				lblServiceUrl.Text = "";
			}

			_connection.DerivedServiceUrl = lblServiceUrl.Text;
		}

		private void UpdateEmail()
		{
			lblEmail.Text = EmailAddress;
		}

		private void loginAutomaticallyToolStripMenuItem_CheckStateChanged(object sender, EventArgs e)
		{
			if (_booting) return;

			_connection.AutoLogin = loginAutomaticallyToolStripMenuItem.Checked;
			_scenario.Save();
			ShellAutologin();

			AddLogEntry(OWAtray.Automatic_Login_is_switched + " " + (_connection.AutoLogin ? OWAtray.ON : OWAtray.OFF));
		}

		private void txtDomain_Validated(object sender, EventArgs e)
		{
			_connection.AccountDomain = txtDomain.Text;
			_scenario.Save();
		}

		private void txtPwd_Validated(object sender, EventArgs e)
		{
			_connection.Password = txtPwd.Text;
			_scenario.Save();
			ShellPassword();
		}

		private void txtServer_Validated(object sender, EventArgs e)
		{
			_connection.EmailServer = txtServer.Text;
			_scenario.Save();
			UpdateServiceUrl();
			UpdateOwaUrl();
		}

		private void txtUser_Validated(object sender, EventArgs e)
		{
			_connection.Username = txtUser.Text;
			_scenario.Save();
			UpdateEmail();
		}

		private void txtEmail_Validated(object sender, EventArgs e)
		{
			_connection.EmailAddress = txtEmail.Text;
			_scenario.Save();
			UpdateEmail();
		}

		private void overrideAutodiscoveryValidationToolStripMenuItem_CheckStateChanged(object sender, EventArgs e)
		{
			if (_booting) return;

			_connection.OverrideAutodiscoveryValidation = overrideAutodiscoveryValidationToolStripMenuItem.Checked;
			_scenario.Save();

			AddLogEntry(
				OWAtray.Autodiscovery_Validation + " " + (_connection.OverrideAutodiscoveryValidation ? OWAtray.ON : OWAtray.OFF));
		}

		private void office365LoginOverrideToolStripMenuItem_CheckStateChanged(object sender, EventArgs e)
		{
			if (_booting) return;

			_connection.OverrideOffice365Login = office365LoginOverrideToolStripMenuItem.Checked;
			_scenario.Save();

			AddLogEntry(OWAtray.Office_login_override + " " + (_connection.OverrideOffice365Login ? OWAtray.ON : OWAtray.OFF));

			UpdateOwaUrl();
		}

		private void txtURLEdit_Validated(object sender, EventArgs e)
		{
			_connection.ServiceUrl = txtURLEdit.Text;
			_scenario.Save();
			UpdateServiceUrl();
		}

		private void cbOverrideEWS_CheckedChanged(object sender, EventArgs e)
		{
			if (_booting) return;

			txtURLEdit.Enabled = cbOverrideEWS.Checked;

			_connection.OverrideServiceUrl = cbOverrideEWS.Checked;
			_scenario.Save();

			UpdateServiceUrl();
			AddLogEntry(OWAtray.EWS_URL_override_switched + " " + (_connection.OverrideServiceUrl ? OWAtray.ON : OWAtray.OFF));
		}

		private void chkAutodiscovery_CheckedChanged(object sender, EventArgs e)
		{
			if (_booting) return;

			_connection.UseAutodiscovery = chkAutodiscovery.Checked;
			_scenario.Save();
			AddLogEntry(OWAtray.Autodiscovery_is_switched + " " + (chkAutodiscovery.Checked ? OWAtray.ON : OWAtray.OFF));

			// Switch off some options when Autodiscovery is checked
			SelectAutodiscoveryOptions();

			// Re-evaluate settings
			UpdateServiceUrl();
			UpdateOwaUrl();
			UpdateEmail();
		}

		private void cbOverrideOWA_CheckedChanged(object sender, EventArgs e)
		{
			if (_booting) return;

			txtOWAEdit.Enabled = cbOverrideOWA.Checked;

			_connection.OverrideEmailUrl = cbOverrideOWA.Checked;
			_scenario.Save();

			UpdateOwaUrl();
			AddLogEntry(OWAtray.OWA_URL_override_switched + " " + (_connection.OverrideEmailUrl ? OWAtray.ON : OWAtray.OFF));
		}

		private void txtOWAEdit_Validated(object sender, EventArgs e)
		{
			_connection.EmailUrl = txtOWAEdit.Text;
			_scenario.Save();
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
			if (_booting) return;

			Settings.Default.UseWebProxy = useDefaultWebProxyToolStripMenuItem.Checked;
			Settings.Default.Save();

			UpdateWebProxySettings();
		}
	}
}