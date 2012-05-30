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
using DrunkenBakery.OWAtray.Growl;
using DrunkenBakery.OWAtray.GUI.Properties;
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
        private string _audioPath;
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
        private Scenario _scenario;
        private IEmailInterface _connection;
        private bool _firstRun = true;

        private bool _bootOk;

        public Form1()
        {
            InitializeComponent();

            // Set up look & feel
            WindowDressing();

            // Welcome message
            AddLogEntry(string.Format("{0} {1} v{2}", Resources.Form1_Form1_Welcome_to_the, AssemblyHelpers.AssemblyTitle,
                                      AssemblyHelpers.UpgradeSettings()));

            // The rest gets kicked off an a timer
            AddLogEntry(String.Format("{0}.", Resources.Form1_Form1_Ready));
            timer1.Start();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                timer1.Enabled = false;
                timerLogging.Enabled = true;

                // Interlock for booting up
                _booting = true;

                // Boot the various subsystems
                BootEnvironment();
                BootShell();
                BootAudio();
                BootScenario();
                BootIcons();
                BootHelpers();

                // Connect if autostart is good to go
                if (Settings.Default.Autostart)
                {
                    ConnectToExchange();
                }

                // Only getting here means we've booted up ok
                _bootOk = true;
            }
            catch (Exception ex)
            {
                AddLogEntry(string.Format("{0}", ex.Message), Severity.Fail);
            }
            finally
            {
                // Release boot interlock
                _booting = false;                
            }
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

        private void BootAudio()
        {
            _audioPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
                                              Settings.Default.SoundFile);
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
            // Has a file been passed in?
            string filePath = Settings.Default.ScenarioFile;
            string[] args = Environment.GetCommandLineArgs();
            if (args.Count() > 1)
            {
                filePath = args[1];
            }

            if (!File.Exists(filePath))
            {
                throw new Exception("Scenario file not found!");
            }

            ////AddLogEntry(filePath);

            // Create our whole universe
            _scenario = ScenarioFactory.CreateScenario(filePath);

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

            // Set up event handling
            WireUpConnectionEvents();

            // Update any UI
            txtEmail.Text = _connection.EmailAddress;
            txtUser.Text = _connection.Username;
            txtPwd.Text = _connection.Password;
            txtServer.Text = _connection.EmailServer;
            txtDomain.Text = _connection.AccountDomain;
            txtURLEdit.Text = _connection.ServiceUrl;
            txtOWAEdit.Text = _connection.EmailUrl;
            txtInterval.Text = _connection.Interval.ToString();
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
            cmbExchangeVersion.SelectedIndex = cmbExchangeVersion.FindStringExact(_connection.ServerVersion);
            UpdateServiceUrl();
            UpdateOwaUrl();
            UpdateEmail();

            // Update shell handler
            ConfigureShell();
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
            Text = string.Format("{0} {1} {2}", AssemblyHelpers.AssemblyTitle, Resources.Form1_WindowDressing_freshly_baked_at, AssemblyHelpers.AssemblyCompany);
            notifyIcon1.Text = AssemblyHelpers.AssemblyTitle + Environment.NewLine + Resources.Form1_WindowDressing_Not_Connected_to_Exchange;
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

        private void ConnectedStateHandler(IEmailInterface connection, ConnectionState state)
        {
            if (IsHandleCreated)
            {
                Invoke(new Action(() =>
                {
                    switch (state)
                    {
                        case ConnectionState.Connecting:
                            break;

                        case ConnectionState.Disconnecting:
                            break;

                        case ConnectionState.Failed:
                            // Switch off autostart if there has been an issue
                            Settings.Default.Autostart = false;
                            Settings.Default.Save();

                            // Show failure message in tray & pop balloon
                            notifyIcon1.Text = AssemblyHelpers.AssemblyTitle + Environment.NewLine + String.Format("{0}!", Resources.Form1_WireUpConnectionEvents_Connection_Failure);
                            PopToast(string.Format("[{0}] - {1}!", connection.EmailAddress, Resources.Form1_WireUpConnectionEvents_Connection_Failure), Resources.Form1_WireUpConnectionEvents_Check_log_file_for_details);
                            break;

                        case ConnectionState.Connected:
                            // After a successful connection then next time we can autostart
                            Settings.Default.Autostart = true;
                            Settings.Default.Save();

                            // Update discovered properties
                            AddLogEntry(string.Format("{0} {1}", Resources.Form1_WireUpConnectionEvents_Connected_to, connection.Version));
                            if (connection.DiscoveredUsername.Length > 0)
                            {
                                AddLogEntry(String.Format("{0}: {1}", Resources.Form1_WireUpConnectionEvents_Discovered_User_Name, connection.DiscoveredUsername), Severity.Success);
                            }
                            if (connection.DiscoveredEmailServer.Length > 0)
                            {
                                AddLogEntry(String.Format("{0}: {1}", Resources.Form1_WireUpConnectionEvents_Discovered_Mailbox_Server, connection.DiscoveredEmailServer), Severity.Success);
                            }
                            if (connection.DiscoveredEmailUrl.Length > 0)
                            {
                                AddLogEntry(String.Format("{0}: {1}", Resources.Form1_WireUpConnectionEvents_Discovered_OWA_Url, connection.DiscoveredEmailUrl), Severity.Success);
                            }
                            if (connection.DiscoveredServiceUrl.Length > 0)
                            {
                                AddLogEntry(String.Format("{0}: {1}", Resources.Form1_WireUpConnectionEvents_Discovered_EWS_Url, connection.DiscoveredServiceUrl), Severity.Success);
                            }

                            // Configure Shell
                            UpdateOwaUrl();
                            UpdateServiceUrl();
                            ShellExchangeVersion();

                            // Minimize
                            WindowState = FormWindowState.Minimized;
                            notifyIcon1.Text = AssemblyHelpers.AssemblyTitle + Environment.NewLine + Resources.Form1_WireUpConnectionEvents_Connected_to_Exchange;
                            break;

                        case ConnectionState.Disconnected:
                            notifyIcon1.Text = AssemblyHelpers.AssemblyTitle + Environment.NewLine + Resources.Form1_WindowDressing_Not_Connected_to_Exchange;
                            break;
                    }
                }));
            }
        }

        private void NewMailHandler(string subject, string sender, string accessUrl)
        {
            if (IsHandleCreated)
            {
                Invoke(new Action(() =>
                {
                    _popUrl = accessUrl;
                    PopToast(string.Format("{0} {1}", Resources.Form1_WireUpConnectionEvents_New_Mail_from, sender), subject);
                }));
            }
        }

        private void NewAppointmentHandler(int minsToGo, DateTime startTime, string subject, string location, string accessUrl)
        {
            if (IsHandleCreated)
            {
                Invoke(new Action(() =>
                {
                    _popUrl = accessUrl;
                    PopToast(
                        string.Format("{0} {1} {2}", Resources.Form1_WireUpConnectionEvents_You_have_an_appointment_in, minsToGo,
                                      (minsToGo != 1
                                           ? Resources.Form1_WireUpConnectionEvents_minutes
                                           : Resources.Form1_WireUpConnectionEvents_minute)),
                        string.Format("{0} - {1} ({2})", startTime.ToShortTimeString(), subject, location));
                }));
            }
        }

        private void MailCountHandler(int count)
        {
            if (IsHandleCreated)
            {
                Invoke(new Action(() =>
                {
                    notifyIcon1.Text = NotificationText(count);
                    notifyIcon1.Icon = new Icon((count > 0 ? _alertIcon : _emailIcon));

                    if (!_firstRun) return;
                    if (count <= 0) return;
                    _firstRun = false;

                    // Special case - pop message at the start if there is any unread email
                    PopToast("New Mail",
                             string.Format("{0} {1} {2}{3}{4}", Resources.Form1_WireUpConnectionEvents_You_have, count,
                                           Resources.Form1_WireUpConnectionEvents_unread_email,
                                           (count != 1 ? "s " : " "), Resources.Form1_WireUpConnectionEvents_in_your_inbox));
                }));
            }
        }

        private void UnwireConnectionEvents()
        {
            foreach (var item in _scenario.Connections.Where(item => item.AreEventsDefined))
            {
                item.LogMessage -= AddLogEntry;
                item.LogException -= AddLogEntry;
                item.ConnectedStateChange -= ConnectedStateHandler;
                item.NewMail -= NewMailHandler;
                item.NewAppointment -= NewAppointmentHandler;
                item.MessageCount -= MailCountHandler;
            }
        }

        private void WireUpConnectionEvents()
        {
            foreach (var item in _scenario.Connections.Where(item => !item.AreEventsDefined))
            {
                item.LogMessage += AddLogEntry;
                item.LogException += AddLogEntry;
                item.ConnectedStateChange += ConnectedStateHandler;
                item.NewMail += NewMailHandler;
                item.NewAppointment += NewAppointmentHandler;
                item.MessageCount += MailCountHandler;
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
            var runSvc = new ProcessStartInfo(_shellPath) { WindowStyle = ProcessWindowStyle.Hidden };

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
                _lvBuffer.Add(new ListViewItem(DateTime.Now.ToString(), Convert.ToInt32(severity)));
                _lvBuffer[_lvBuffer.Count - 1].SubItems.Add(newEntry);
                LoggerProxy.Log(newEntry, severity != Severity.Fail);
            }
            catch (Exception)
            {
            }
        }

        private void AddLogEntry(string newEntry, Exception ex)
        {
            try
            {
                _lvBuffer.Add(new ListViewItem(DateTime.Now.ToString(), Convert.ToInt32(Severity.Fail)));
                _lvBuffer[_lvBuffer.Count - 1].SubItems.Add(newEntry);
                LoggerProxy.Log(newEntry, ex);
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

            AddLogEntry(String.Format("{0} {1}",
                                      Resources.
                                          Form1_alwaysOpenOWAInIEToolStripMenuItem_CheckStateChanged_Always_use_IE_switched_,
                                      (_connection.AlwaysUseInternetExplorer
                                           ? Resources.Form1_alwaysOpenOWAInIEToolStripMenuItem_CheckStateChanged_ON
                                           : Resources.Form1_alwaysOpenOWAInIEToolStripMenuItem_CheckStateChanged_OFF)));
        }

        private void balloonToolStripMenuItem_CheckStateChanged(object sender, EventArgs e)
        {
            if (_booting) return;

            Settings.Default.Balloon = balloonToolStripMenuItem.Checked;
            Settings.Default.Save();
            AddLogEntry(String.Format("{0} {1}",
                                      Resources.
                                          Form1_balloonToolStripMenuItem_CheckStateChanged_Balloon_notifications_switched,
                                      (Settings.Default.Balloon
                                           ? Resources.Form1_alwaysOpenOWAInIEToolStripMenuItem_CheckStateChanged_ON
                                           : Resources.Form1_alwaysOpenOWAInIEToolStripMenuItem_CheckStateChanged_OFF)));
        }

        private void changeLogToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_frmChangeLog == null) _frmChangeLog = new ChangeLog(Settings.Default.RSSFeed);
            _frmChangeLog.ShowDialog();
        }

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
            if (_booting) return;

            RunAtStartup(chkRunOnStartup.Checked);
        }

        private void RunAtStartup(bool switchOn)
        {
            try
            {
                WindowsShortcut.Update(Environment.SpecialFolder.Startup, Application.ExecutablePath,
                                       AssemblyHelpers.AssemblyTitle,
                                       switchOn);
                AddLogEntry(
                    String.Format("{0} {1} {2}", Resources.Form1_RunAtStartup_OWAtray_will,
                                  (switchOn ? string.Empty : Resources.Form1_RunAtStartup__not),
                                  Resources.Form1_RunAtStartup_autostart_with_Windows));
            }
            catch (Exception ex)
            {
                AddLogEntry(ex.Message, ex);
            }
        }

        private void cmdStart_Click(object sender, EventArgs e)
        {
            ConnectToExchange();
        }

        private void ConnectToExchange()
        {
            switch (_connection.ConnectedState)
            {
                case ConnectionState.Connected:
                    AddLogEntry(String.Format("{0}", Resources.Form1_ConnectToExchange_Already_connected), Severity.Fail);
                    break;
                case ConnectionState.Disconnecting:
                case ConnectionState.Connecting:
                    AddLogEntry(
                        String.Format("{0}, {1}...", Resources.Form1_ConnectToExchange_Transitioning_state,
                                      Resources.Form1_ConnectToExchange_please_wait), Severity.Fail);
                    break;
                default:
                    _connection.ConnectA();
                    break;
            }
        }

        private void DisconnectFromExchange()
        {
            switch (_connection.ConnectedState)
            {
                case ConnectionState.Disconnected:
                    AddLogEntry(string.Format("{0}", Resources.Form1_DisconnectFromExchange_Already_disconnected));
                    break;
                case ConnectionState.Disconnecting:
                case ConnectionState.Connecting:
                    AddLogEntry(
                        string.Format("{0}, {1}...", Resources.Form1_DisconnectFromExchange_Transitioning_state,
                                      Resources.Form1_DisconnectFromExchange_please_wait), Severity.Fail);
                    break;
                default:
                    _connection.Disconnect();
                    break;
            }
        }

        private void cmdStop_Click(object sender, EventArgs e)
        {
            DisconnectFromExchange();
        }

        private static void UpdateWebProxySettings()
        {
            WebRequest.DefaultWebProxy.Credentials = Properties.Settings.Default.UseWebProxy
                                                         ? CredentialCache.DefaultCredentials
                                                         : null;
        }

        private void ShellOwaUrl()
        {
            try
            {
                var runSvc = new ProcessStartInfo(_shellPath)
                                 {Arguments = "url " + _connection.DerivedEmailUrl, WindowStyle = ProcessWindowStyle.Hidden};
                var serviceProcess = Process.Start(runSvc);

                while (!serviceProcess.HasExited)
                {
                    Thread.Sleep(100);
                    Application.DoEvents();
                }
            }
            catch (Exception ex)
            {
                AddLogEntry(String.Format("{0}: {1}", Resources.Form1_ShellOwaUrl_Error, ex.Message), ex);
            }
        }

        private void ShellExchangeVersion()
        {
            try
            {
                var runSvc = new ProcessStartInfo(_shellPath)
                                 {Arguments = "exchange " + _connection.Version, WindowStyle = ProcessWindowStyle.Hidden};
                var serviceProcess = Process.Start(runSvc);

                while (!serviceProcess.HasExited)
                {
                    Thread.Sleep(100);
                    Application.DoEvents();
                }
            }
            catch (Exception ex)
            {
                AddLogEntry(String.Format("{0}: {1}", Resources.Form1_ShellOwaUrl_Error, ex.Message), ex);
            }
        }

        private void ShellPassword()
        {
            try
            {
                var runSvc = new ProcessStartInfo(_shellPath)
                                 {Arguments = "password " + _connection.Password, WindowStyle = ProcessWindowStyle.Hidden};
                var serviceProcess = Process.Start(runSvc);

                while (!serviceProcess.HasExited)
                {
                    Thread.Sleep(100);
                    Application.DoEvents();
                }
            }
            catch (Exception ex)
            {
                AddLogEntry(String.Format("{0}: {1}", Resources.Form1_ShellOwaUrl_Error, ex.Message), ex);
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
                AddLogEntry(String.Format("{0}: {1}", Resources.Form1_ShellOwaUrl_Error, ex.Message), ex);
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
                AddLogEntry(String.Format("{0}: {1}", Resources.Form1_ShellOwaUrl_Error, ex.Message), ex);
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

            AddLogEntry(String.Format("{0} {1}",
                                      Resources.Form1_disableCalendarToolStripMenuItem_CheckStateChanged_Calendar_notifications_switched,
                                      (_connection.DisableCalendar
                                           ? Resources.Form1_alwaysOpenOWAInIEToolStripMenuItem_CheckStateChanged_OFF
                                           : Resources.Form1_alwaysOpenOWAInIEToolStripMenuItem_CheckStateChanged_ON)));
        }

        private void exitToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            this.Shutdown();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Shutdown();
        }

        private void Shutdown()
        {
            _overRideClose = true;
            AddLogEntry(Resources.Form1_Form1_FormClosed_Terminating);

            if (_bootOk)
            {
                SnarlHelper.Revoke(Handle);
                UnwireConnectionEvents();
                DisconnectFromExchange();
            }

            this.Close();			
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
                                                var lv = lvStatus.Items[lvStatus.Items.Count - 1];
                                                var txt = lv.SubItems[1].Text;
                                                slStatus.Text = txt.Substring(0, txt.Length < 100 ? txt.Length : 100);
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

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (WindowState != FormWindowState.Minimized && _overRideClose == false)
            {
                e.Cancel = true;
                WindowState = FormWindowState.Minimized;
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

        private string NotificationText(int myCount)
        {
            const int maxTipLength = 63;
            var text1 = string.Format("{0}{1}{1}{2} {3}{4}", AssemblyHelpers.AssemblyTitle, Environment.NewLine, myCount, Resources.Form1_WireUpConnectionEvents_unread_email, (myCount != 1 ? "s " : " "));
            var charsLeft = maxTipLength - text1.Length;
            var domainText = string.Format("{0}\\{1}", _connection.DiscoveredEmailServer, _connection.DiscoveredUsername);
            if (domainText.Length > charsLeft) domainText = domainText.Substring(0, charsLeft);
            var finalText = string.Format("{0}{1}{2}{1}{3} {4}{5}", AssemblyHelpers.AssemblyTitle, Environment.NewLine, domainText, myCount, Resources.Form1_WireUpConnectionEvents_unread_email, (myCount != 1 ? "s " : " "));
            return finalText;
        }

        private void growlToolStripMenuItem_CheckStateChanged(object sender, EventArgs e)
        {
            if (_booting) return;

            Settings.Default.Growl = growlToolStripMenuItem.Checked;
            Settings.Default.Save();
            AddLogEntry(String.Format("{0} {1}", Resources.Form1_growlToolStripMenuItem_CheckStateChanged_Growl_notifications_switched,
                                      (Settings.Default.Growl
                                           ? Resources.Form1_alwaysOpenOWAInIEToolStripMenuItem_CheckStateChanged_ON
                                           : Resources.Form1_alwaysOpenOWAInIEToolStripMenuItem_CheckStateChanged_OFF)));
        }

        private static void InitEventView(ListView lvX)
        {
            lvX.Columns.Add(Resources.Form1_InitEventView_Time, 140, HorizontalAlignment.Left);
            lvX.Columns.Add(Resources.Form1_InitEventView_Event_Details, 1000, HorizontalAlignment.Left);
            lvX.Items.Clear();
        }

        private void makeOWADefaultToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!IsUserAdministrator())
            {
                AddLogEntry(
                    String.Format("{0}. {1}.", Resources.Form1_makeOWADefaultToolStripMenuItem_Click_You_are_not_an_Admin_user,
                                  Resources.Form1_makeOWADefaultToolStripMenuItem_Click_Operation_may_fail), Severity.Fail);
            }

            // Configure registry
            AddLogEntry(Resources.Form1_makeOWADefaultToolStripMenuItem_Click_Setting_up_Mail_handlers);

            try
            {
                var runSvc = new ProcessStartInfo(_shellPath) { Arguments = "registry", WindowStyle = ProcessWindowStyle.Hidden };
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
                AddLogEntry(String.Format("{0}: {1}", Resources.Form1_ShellOwaUrl_Error, ex.Message), ex);
                return;
            }

            AddLogEntry(Resources.Form1_makeOWADefaultToolStripMenuItem_Click_Mail_functions_will_now_be_handled_by_OWA, Severity.Success);
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
                AddLogEntry(ex.Message, ex);
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

            AddLogEntry(String.Format("{0} {1}",
                                      Resources.
                                          Form1_overrideCertificateToolStripMenuItem_CheckStateChanged_SSL_Certificate_override_switched,
                                      (_connection.OverrideCertificate
                                           ? Resources.Form1_alwaysOpenOWAInIEToolStripMenuItem_CheckStateChanged_ON
                                           : Resources.Form1_alwaysOpenOWAInIEToolStripMenuItem_CheckStateChanged_OFF)));
        }

        private void playSoundToolStripMenuItem_CheckStateChanged(object sender, EventArgs e)
        {
            if (_booting) return;

            Settings.Default.Bell = playSoundToolStripMenuItem.Checked;
            Settings.Default.Save();
            AddLogEntry(String.Format("{0} {1}",
                                      Resources.
                                          Form1_playSoundToolStripMenuItem_CheckStateChanged_Audible_notifications_switched,
                                      (Settings.Default.Bell
                                           ? Resources.Form1_alwaysOpenOWAInIEToolStripMenuItem_CheckStateChanged_ON
                                           : Resources.Form1_alwaysOpenOWAInIEToolStripMenuItem_CheckStateChanged_OFF)));
        }

        private void PopToast(string myTitle, string myMessage)
        {
            // Belt & Braces
            if (myTitle.Length == 0) myTitle = String.Format("<{0}>", Resources.Form1_PopToast_No_Title);
            if (myMessage.Length == 0) myMessage = String.Format("<{0}>", Resources.Form1_PopToast_No_Subject);

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
                AudioHelper.Play(_audioPath);
            }
        }

        private void recallLastPopupToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_lastPopMessage.Length <= 0 || _lastPopTitle.Length <= 0) return;

            _popUrl = _lastPopUrl;
            PopToast(_lastPopTitle, _lastPopMessage);
        }

        private void resetTrayIconToolStripMenuItem_Click(object sender, EventArgs e)
        {
            notifyIcon1.Icon = new Icon(_emailIcon);
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
            AddLogEntry(String.Format("{0} {1}",
                                      Resources.Form1_snarlToolStripMenuItem_CheckStateChanged_Snarl_notifications_switched,
                                      (Settings.Default.Snarl
                                           ? Resources.Form1_alwaysOpenOWAInIEToolStripMenuItem_CheckStateChanged_ON
                                           : Resources.Form1_alwaysOpenOWAInIEToolStripMenuItem_CheckStateChanged_OFF)));
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
                AddLogEntry(
                    String.Format("{0}. {1}.", Resources.Form1_switchOffToolStripMenuItem_Click_You_are_not_an_Admin_user,
                                  Resources.Form1_switchOffToolStripMenuItem_Click_Operation_may_fail), Severity.Fail);
            }

            // Configure registry
            AddLogEntry(Resources.Form1_switchOffToolStripMenuItem_Click_Restoring_Mail_handlers);

            try
            {
                var runSvc = new ProcessStartInfo(_shellPath) { Arguments = "restore", WindowStyle = ProcessWindowStyle.Hidden };
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
                AddLogEntry(String.Format("{0}: {1}", Resources.Form1_ShellOwaUrl_Error, ex.Message), ex);
                return;
            }

            AddLogEntry("Mail handler restored to system default", Severity.Success);
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
                                            Resources.Form1_txtInterval_Validating_Must_be_a_numeric_value_between_1_and_ +
                                            MaxInterval.ToString());
                    e.Cancel = true;
                }
            }
            else
            {
                errorProvider1.SetError(txtInterval,
                                        Resources.Form1_txtInterval_Validating_Must_be_a_numeric_value_between_1_and_ +
                                        MaxInterval.ToString());
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

            AddLogEntry(String.Format("{0} {1}",
                                      Resources.
                                          Form1_loginAutomaticallyToolStripMenuItem_CheckStateChanged_Automatic_Login_is_switched,
                                      (_connection.AutoLogin
                                           ? Resources.Form1_alwaysOpenOWAInIEToolStripMenuItem_CheckStateChanged_ON
                                           : Resources.Form1_alwaysOpenOWAInIEToolStripMenuItem_CheckStateChanged_OFF)));
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

            AddLogEntry(String.Format("{0} {1}",
                                      Resources.
                                          Form1_overrideAutodiscoveryValidationToolStripMenuItem_CheckStateChanged_Autodiscovery_Validation_override_switched,
                                      (_connection.OverrideAutodiscoveryValidation
                                           ? Resources.Form1_alwaysOpenOWAInIEToolStripMenuItem_CheckStateChanged_ON
                                           : Resources.Form1_alwaysOpenOWAInIEToolStripMenuItem_CheckStateChanged_OFF)));
        }

        private void office365LoginOverrideToolStripMenuItem_CheckStateChanged(object sender, EventArgs e)
        {
            if (_booting) return;

            _connection.OverrideOffice365Login = office365LoginOverrideToolStripMenuItem.Checked;
            _scenario.Save();

            AddLogEntry(String.Format("{0} {1}",
                                      Resources.
                                          Form1_office365LoginOverrideToolStripMenuItem_CheckStateChanged_Office365_login_override,
                                      (_connection.OverrideOffice365Login
                                           ? Resources.Form1_alwaysOpenOWAInIEToolStripMenuItem_CheckStateChanged_ON
                                           : Resources.Form1_alwaysOpenOWAInIEToolStripMenuItem_CheckStateChanged_OFF)));

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
            AddLogEntry(String.Format("{0} {1}", Resources.Form1_cbOverrideEWS_CheckedChanged_EWS_URL_override_switched,
                                      (_connection.OverrideServiceUrl
                                           ? Resources.Form1_alwaysOpenOWAInIEToolStripMenuItem_CheckStateChanged_ON
                                           : Resources.Form1_alwaysOpenOWAInIEToolStripMenuItem_CheckStateChanged_OFF)));
        }

        private void chkAutodiscovery_CheckedChanged(object sender, EventArgs e)
        {
            if (_booting) return;

            _connection.UseAutodiscovery = chkAutodiscovery.Checked;
            _scenario.Save();
            AddLogEntry(String.Format("{0} {1}", Resources.Form1_chkAutodiscovery_CheckedChanged_Autodiscovery_is_switched,
                                      (chkAutodiscovery.Checked
                                           ? Resources.Form1_alwaysOpenOWAInIEToolStripMenuItem_CheckStateChanged_ON
                                           : Resources.Form1_alwaysOpenOWAInIEToolStripMenuItem_CheckStateChanged_OFF)));

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
            AddLogEntry(String.Format("{0} {1}", Resources.Form1_cbOverrideOWA_CheckedChanged_OWA_URL_override_switched,
                                      (_connection.OverrideEmailUrl
                                           ? Resources.Form1_alwaysOpenOWAInIEToolStripMenuItem_CheckStateChanged_ON
                                           : Resources.Form1_alwaysOpenOWAInIEToolStripMenuItem_CheckStateChanged_OFF)));
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
                Process.Start(LoggerProxy.Filename);
            }
            catch (Exception ex)
            {
                AddLogEntry(ex.Message, ex);
            }
        }

        private void useDefaultWebProxyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_booting) return;

            Settings.Default.UseWebProxy = useDefaultWebProxyToolStripMenuItem.Checked;
            Settings.Default.Save();

            UpdateWebProxySettings();
        }

        private void cmbExchangeVersion_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_booting) return;

            _connection.ServerVersion = cmbExchangeVersion.Text;
            _scenario.Save();
        }
    }
}