// ------------------------------------------------------------------
//  Freshly Baked at the Drunken Bakery
//  OWAtray::DrunkenBakery.OWAtray.GUI
//
//  <copyright file="Form1.cs" company="The Drunken Bakery">
//      Copyright (c) 2009-2014 The Drunken Bakery. All rights reserved.
//  </copyright>
//
//  Author: IRS
// ------------------------------------------------------------------

namespace DrunkenBakery.OWAtray.GUI
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Drawing;
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
    using DrunkenBakery.OWAtray.Logging;

    /// <summary>
    ///     The main form.
    /// </summary>
    public partial class Form1 : Form
    {
        #region Constants and Fields

        /// <summary>
        ///     The max interval.
        /// </summary>
        private const int MaxInterval = 3600;

        /// <summary>
        ///     The _lv buffer.
        /// </summary>
        private readonly List<ListViewItem> logBuffer = new List<ListViewItem>();

        /// <summary>
        ///     The _alert icon.
        /// </summary>
        private string alertIcon;

        /// <summary>
        ///     ContextMenu's Exit command used.
        /// </summary>
        private bool allowClose;

        /// <summary>
        ///     ContextMenu's Show command used.
        /// </summary>
        private bool allowVisible;

        /// <summary>
        ///     The _audio path.
        /// </summary>
        private string audioPath;

        /// <summary>
        ///     The _boot ok.
        /// </summary>
        private bool bootOk;

        /// <summary>
        ///     The _booting.
        /// </summary>
        private bool booting;

        /// <summary>
        ///     The _connection.
        /// </summary>
        private IEmailInterface connection;

        /// <summary>
        ///     The _email icon.
        /// </summary>
        private string emailIcon;

        /// <summary>
        ///     The _first run.
        /// </summary>
        private bool firstRun = true;

        /// <summary>
        ///     The _frm about.
        /// </summary>
        private Form frmAbout;

        /// <summary>
        ///     The _frm change log.
        /// </summary>
        private Form frmChangeLog;

        /// <summary>
        ///     The _frm contact.
        /// </summary>
        private Form frmContact;

        /// <summary>
        ///     The _frm info.
        /// </summary>
        private Form frmInfo;

        /// <summary>
        ///     The _frm net.
        /// </summary>
        private Form frmNet;

        /// <summary>
        ///     The _graphic path.
        /// </summary>
        private string graphicPath;

        /// <summary>
        ///     The _last pop message.
        /// </summary>
        private string lastPopMessage = string.Empty;

        /// <summary>
        ///     The _last pop title.
        /// </summary>
        private string lastPopTitle = string.Empty;

        /// <summary>
        ///     The _last pop url.
        /// </summary>
        private string lastPopUrl = string.Empty;

        /// <summary>
        ///     The last read mail count.
        /// </summary>
        private int mailCount;

        /// <summary>
        ///     The _pop url.
        /// </summary>
        private string popUrl = string.Empty;

        /// <summary>
        ///     The _scenario.
        /// </summary>
        private Scenario scenario;

        /// <summary>
        ///     The _shell path.
        /// </summary>
        private string shellPath;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="Form1" /> class.
        /// </summary>
        public Form1()
        {
            this.InitializeComponent();

            // Set up look & feel
            this.WindowDressing();

            // Welcome message
            this.AddLogEntry(
                string.Format(
                    "{0} {1} v{2}",
                    Resources.Form1_Form1_Welcome_to_the,
                    AssemblyHelpers.AssemblyTitle,
                    AssemblyHelpers.UpgradeSettings()));

            // The rest gets kicked off an a timer
            this.AddLogEntry(string.Format("{0}.", Resources.Form1_Form1_Ready));
            this.timer1.Start();
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Gets EmailAddress.
        /// </summary>
        private string EmailAddress
        {
            get
            {
                string email = (this.txtEmail.Text.Length > 0) ? this.txtEmail.Text : this.txtUser.Text;
                if (email.Length > 0 && !email.Contains("@"))
                {
                    email = email + "@" + GetSubDomain(this.txtServer.Text);
                }

                return email;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Raises the <see cref="E:System.Windows.Forms.Form.FormClosing" /> event.
        /// </summary>
        /// <param name="e">
        ///     A <see cref="T:System.Windows.Forms.FormClosingEventArgs" /> that contains the event data.
        /// </param>
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (!this.allowClose)
            {
                // Special case if Windows is closing
                if (e.CloseReason != CloseReason.WindowsShutDown && e.CloseReason != CloseReason.TaskManagerClosing)
                {
                    this.Hide();
                    e.Cancel = true;
                }
            }

            base.OnFormClosing(e);
        }

        /// <summary>
        ///     Overrides the windows form logic.
        /// </summary>
        /// <param name="value">
        ///     true to make the control visible; otherwise, false.
        /// </param>
        protected override void SetVisibleCore(bool value)
        {
            if (!this.allowVisible)
            {
                value = false;
            }

            base.SetVisibleCore(value);
        }

        /// <summary>
        ///     The get sub domain.
        /// </summary>
        /// <param name="domain">
        ///     The domain.
        /// </param>
        /// <returns>
        ///     The sub domain.
        /// </returns>
        private static string GetSubDomain(string domain)
        {
            string result = string.Empty;

            string[] parts = domain.Split('.');

            if (parts.Length > 1)
            {
                for (int f = 1; f < parts.Length; ++f)
                {
                    result = result + parts[f];
                    if (f != (parts.Length - 1))
                    {
                        result = result + ".";
                    }
                }
            }

            return result;
        }

        /// <summary>
        ///     The init event view.
        /// </summary>
        /// <param name="lvX">
        ///     The lv x.
        /// </param>
        private static void InitEventView(ListView lvX)
        {
            lvX.Columns.Add(Resources.Form1_InitEventView_Time, 140, HorizontalAlignment.Left);
            lvX.Columns.Add(Resources.Form1_InitEventView_Event_Details, 1000, HorizontalAlignment.Left);
            lvX.Items.Clear();
        }

        /// <summary>
        ///     The is user administrator.
        /// </summary>
        /// <returns>
        ///     True if user is administrator.
        /// </returns>
        private static bool IsUserAdministrator()
        {
            bool isAdmin = false;
            try
            {
                // get the currently logged in user
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

        /// <summary>
        ///     The strip email domain.
        /// </summary>
        /// <param name="email">
        ///     The email.
        /// </param>
        /// <returns>
        ///     The email domain.
        /// </returns>
        private static string StripEmailDomain(string email)
        {
            string sub = string.Empty;
            int start = email.IndexOf("@", StringComparison.Ordinal);
            if (start > 0)
            {
                sub = email.Substring(start + 1);
            }

            return sub;
        }

        /// <summary>
        ///     The update web proxy settings.
        /// </summary>
        private static void UpdateWebProxySettings()
        {
            WebRequest.DefaultWebProxy.Credentials = Settings.Default.UseWebProxy
                ? CredentialCache.DefaultCredentials
                : null;
        }

        /// <summary>
        ///     The about tool strip menu item_ click.
        /// </summary>
        /// <param name="sender">
        ///     The sender.
        /// </param>
        /// <param name="e">
        ///     The e.
        /// </param>
        private void AboutToolStripMenuItemClick(object sender, EventArgs e)
        {
            if (this.frmAbout == null)
            {
                this.frmAbout = new AboutBox1();
            }

            this.frmAbout.ShowDialog();
        }

        /// <summary>
        ///     The activate owa.
        /// </summary>
        /// <param name="url">
        ///     The url.
        /// </param>
        private void ActivateOwa(string url = "")
        {
            string targetUrl = string.Empty;

            // If this is Exchange2013 then we need to process the URL
            if (this.connection.Version == "Exchange2013")
            {
                if (url.Length > 0)
                {
                    targetUrl =
                        "#viewmodel=_y.$TX&ItemID=AAMkADM2ZmI4ODcwLWY4YWEtNGQ2YS1hYjMyLTE0M2ZkNDM0MmQ2OABGAAAAAABE";
                    string temp1 = url.Replace("RgAAAABE", string.Empty);
                    string temp2 = temp1.Replace("J&exvsurl=1", string.Empty);
                    string temp3 = temp2.Replace("P&exvsurl=1", string.Empty);
                    string temp4 = temp3.Replace("?ae=Item&a=Open&t=IPM.Note&id=", string.Empty);
                    string temp5 = temp4.Replace("?ae=Item&t=IPM.Appointment&id=", string.Empty);
                    targetUrl = targetUrl + temp5 + "%3D";
                }
            }
            else
            {
                targetUrl = url;
            }

            var runSvc = new ProcessStartInfo(this.shellPath) { WindowStyle = ProcessWindowStyle.Hidden };

            // Choose browser
            if (this.connection.AlwaysUseInternetExplorer)
            {
                runSvc.Arguments = "owa" + ((targetUrl.Length > 0) ? " " + targetUrl : string.Empty);
            }
            else
            {
                runSvc.Arguments = "shell" + ((targetUrl.Length > 0) ? " " + targetUrl : string.Empty);
            }

            // Open window
            Process.Start(runSvc);

            // Switch off this override
            if (this.office365LoginOverrideToolStripMenuItem.CheckState == CheckState.Checked)
            {
                this.office365LoginOverrideToolStripMenuItem.CheckState = CheckState.Unchecked;
            }
        }

        /// <summary>
        ///     The add log entry.
        /// </summary>
        /// <param name="newEntry">
        ///     The new entry.
        /// </param>
        /// <param name="severity">
        ///     The severity.
        /// </param>
        private void AddLogEntry(string newEntry, Severity severity = Severity.Info)
        {
            try
            {
                this.logBuffer.Add(new ListViewItem(DateTime.Now.ToString(), Convert.ToInt32(severity)));
                this.logBuffer[this.logBuffer.Count - 1].SubItems.Add(newEntry);
                LoggerProxy.Log(newEntry, severity != Severity.Fail);
            }
            catch (Exception)
            {
            }
        }

        /// <summary>
        ///     The add log entry.
        /// </summary>
        /// <param name="newEntry">
        ///     The new entry.
        /// </param>
        /// <param name="ex">
        ///     The ex.
        /// </param>
        private void AddLogEntry(string newEntry, Exception ex)
        {
            try
            {
                this.logBuffer.Add(new ListViewItem(DateTime.Now.ToString(), Convert.ToInt32(Severity.Fail)));
                this.logBuffer[this.logBuffer.Count - 1].SubItems.Add(newEntry);
                LoggerProxy.Log(newEntry, ex);
            }
            catch (Exception)
            {
            }
        }

        /// <summary>
        ///     The always open owa in ie tool strip menu item_ check state changed.
        /// </summary>
        /// <param name="sender">
        ///     The sender.
        /// </param>
        /// <param name="e">
        ///     The e.
        /// </param>
        private void AlwaysOpenOwainIeToolStripMenuItemCheckStateChanged(object sender, EventArgs e)
        {
            if (this.booting)
            {
                return;
            }

            this.connection.AlwaysUseInternetExplorer = this.alwaysOpenOWAInIEToolStripMenuItem.Checked;
            this.scenario.Save();
            this.ShellBrowserVersion();

            var state = this.connection.AlwaysUseInternetExplorer
                ? Resources.Form1_alwaysOpenOWAInIEToolStripMenuItem_CheckStateChanged_ON
                : Resources.Form1_alwaysOpenOWAInIEToolStripMenuItem_CheckStateChanged_OFF;
            this.AddLogEntry(string.Format("{0} {1}", Resources.Form1_alwaysOpenOWAInIEToolStripMenuItem_CheckStateChanged_Always_use_IE_switched_, state));
        }

        /// <summary>
        ///     The balloon tool strip menu item_ check state changed.
        /// </summary>
        /// <param name="sender">
        ///     The sender.
        /// </param>
        /// <param name="e">
        ///     The e.
        /// </param>
        private void BalloonToolStripMenuItemCheckStateChanged(object sender, EventArgs e)
        {
            if (this.booting)
            {
                return;
            }

            Settings.Default.Balloon = this.balloonToolStripMenuItem.Checked;
            Settings.Default.Save();
            var state = Settings.Default.Balloon
                ? Resources.Form1_alwaysOpenOWAInIEToolStripMenuItem_CheckStateChanged_ON
                : Resources.Form1_alwaysOpenOWAInIEToolStripMenuItem_CheckStateChanged_OFF;
            this.AddLogEntry(string.Format("{0} {1}", Resources.Form1_balloonToolStripMenuItem_CheckStateChanged_Balloon_notifications_switched, state));
        }

        /// <summary>
        ///     The boot audio.
        /// </summary>
        private void BootAudio()
        {
            this.audioPath = Path.Combine(
                Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
                Settings.Default.SoundFile);
        }

        /// <summary>
        ///     The boot environment.
        /// </summary>
        private void BootEnvironment()
        {
            // Startup Flag
            this.chkRunOnStartup.Checked = WindowsShortcut.Exists(
                Environment.SpecialFolder.Startup,
                AssemblyHelpers.AssemblyTitle);

            // Notifications
            this.balloonToolStripMenuItem.Checked = Settings.Default.Balloon;
            this.playSoundToolStripMenuItem.Checked = Settings.Default.Bell;

            // Web Proxy
            this.useDefaultWebProxyToolStripMenuItem.Checked = Settings.Default.UseWebProxy;
            UpdateWebProxySettings();

            // Lockdown mode
            this.restoreToolStripMenuItem.Enabled = !Settings.Default.LockDown;
        }

        /// <summary>
        ///     The boot icons.
        /// </summary>
        private void BootIcons()
        {
            this.graphicPath = Path.Combine(
                Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
                Settings.Default.EmailGraphic);
            this.emailIcon = Path.Combine(
                Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
                Settings.Default.EmailIcon);
            this.alertIcon = Path.Combine(
                Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
                Settings.Default.AlertIcon);

            // Tray icon
            this.notifyIcon1.Icon = new Icon(this.emailIcon);
        }

        /// <summary>
        ///     The boot scenario.
        /// </summary>
        private void BootScenario()
        {
            // Has a file been passed in?
            string filePath = Settings.Default.ScenarioFile;
            string[] args = Environment.GetCommandLineArgs();
            if (args.Count() > 1)
            {
                filePath = args[1];
            }

            // Create our whole universe
            this.scenario = ScenarioFactory.CreateScenario(filePath);
            this.scenario.ScenarioEvent += this.ScenarioScenarioEvent;
            this.scenario.DebugMessage += this.DebugLogHandler;

            // Set up event handling
            this.WireUpConnectionEvents();

            // TODO: Special case for single theConnection use only
            if (this.scenario.Connections.Count == 0)
            {
                // Create the new entry
                this.connection = ConnectionFactory.CreateConnection(EmailType.Exchange);
                LoggerProxy.Debug("Created a new " + this.connection.Type + " connection (no existing scenario)");
                this.scenario.Connections.Add(this.connection);
                this.scenario.Save();
            }
            else
            {
                // Retrieve all the settings
                this.connection = this.scenario.Connections[0];
            }

            // Update any UI
            this.txtEmail.Text = this.connection.EmailAddress;
            this.txtUser.Text = this.connection.Username;
            this.txtPwd.Text = this.connection.Password;
            this.txtServer.Text = this.connection.EmailServer;
            this.txtDomain.Text = this.connection.AccountDomain;
            this.txtDescription.Text = this.connection.Description;
            this.txtURLEdit.Text = this.connection.ServiceUrl;
            this.txtOWAEdit.Text = this.connection.EmailUrl;
            this.txtInterval.Text = this.connection.Interval.ToString();
            this.cbOverrideEWS.Checked = this.connection.OverrideServiceUrl;
            this.txtURLEdit.Enabled = this.cbOverrideEWS.Checked;
            this.cbOverrideOWA.Checked = this.connection.OverrideEmailUrl;
            this.txtOWAEdit.Enabled = this.cbOverrideOWA.Checked;
            this.chkAutodiscovery.Checked = this.connection.UseAutodiscovery;
            this.chkOffice365.Checked = this.connection.Office365;
            this.SelectAutodiscoveryOptions();
            this.chkOnDomain.Checked = this.connection.OnWindowsDomain;
            this.SelectDomainOptions();
            this.overrideCertificateToolStripMenuItem.Checked = this.connection.OverrideCertificate;
            this.alwaysOpenOWAInIEToolStripMenuItem.Checked = this.connection.AlwaysUseInternetExplorer;
            this.disableCalendarToolStripMenuItem.Checked = this.connection.DisableCalendar;
            this.loginAutomaticallyToolStripMenuItem.Checked = this.connection.AutoLogin;
            this.office365LoginOverrideToolStripMenuItem.Checked = this.connection.OverrideOffice365Login;
            this.overrideAutodiscoveryValidationToolStripMenuItem.Checked =
                this.connection.OverrideAutodiscoveryValidation;
            this.cmbExchangeVersion.SelectedIndex =
                this.cmbExchangeVersion.FindStringExact(this.connection.ServerVersion);
            this.UpdateServiceUrl();
            this.UpdateOwaUrl();
            this.UpdateEmail();

            // Update shell handler
            this.ConfigureShell();
        }

        /// <summary>
        ///     The boot shell.
        /// </summary>
        private void BootShell()
        {
            this.shellPath = Path.Combine(
                Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
                Settings.Default.ShellIntegration);
        }

        /// <summary>
        ///     The cb override ew s_ checked changed.
        /// </summary>
        /// <param name="sender">
        ///     The sender.
        /// </param>
        /// <param name="e">
        ///     The e.
        /// </param>
        private void CbOverrideEwsCheckedChanged(object sender, EventArgs e)
        {
            if (this.booting)
            {
                return;
            }

            this.txtURLEdit.Enabled = this.cbOverrideEWS.Checked;

            this.connection.OverrideServiceUrl = this.cbOverrideEWS.Checked;
            this.scenario.Save();

            this.UpdateServiceUrl();
            var state = this.connection.OverrideServiceUrl
                ? Resources.Form1_alwaysOpenOWAInIEToolStripMenuItem_CheckStateChanged_ON
                : Resources.Form1_alwaysOpenOWAInIEToolStripMenuItem_CheckStateChanged_OFF;
            this.AddLogEntry(string.Format("{0} {1}", Resources.Form1_cbOverrideEWS_CheckedChanged_EWS_URL_override_switched, state));
        }

        /// <summary>
        ///     The cb override ew s_ enabled changed.
        /// </summary>
        /// <param name="sender">
        ///     The sender.
        /// </param>
        /// <param name="e">
        ///     The e.
        /// </param>
        private void CbOverrideEwsEnabledChanged(object sender, EventArgs e)
        {
            if (!this.cbOverrideEWS.Enabled)
            {
                this.txtURLEdit.Enabled = false;
            }
            else
            {
                if (this.cbOverrideEWS.Checked)
                {
                    this.txtURLEdit.Enabled = true;
                }
            }
        }

        /// <summary>
        ///     The cb override ow a_ checked changed.
        /// </summary>
        /// <param name="sender">
        ///     The sender.
        /// </param>
        /// <param name="e">
        ///     The e.
        /// </param>
        private void CbOverrideOwaCheckedChanged(object sender, EventArgs e)
        {
            if (this.booting)
            {
                return;
            }

            this.txtOWAEdit.Enabled = this.cbOverrideOWA.Checked;

            this.connection.OverrideEmailUrl = this.cbOverrideOWA.Checked;
            this.scenario.Save();

            this.UpdateOwaUrl();
            var state = this.connection.OverrideEmailUrl
                ? Resources.Form1_alwaysOpenOWAInIEToolStripMenuItem_CheckStateChanged_ON
                : Resources.Form1_alwaysOpenOWAInIEToolStripMenuItem_CheckStateChanged_OFF;
            this.AddLogEntry(string.Format("{0} {1}", Resources.Form1_cbOverrideOWA_CheckedChanged_OWA_URL_override_switched, state));
        }

        /// <summary>
        ///     The cb override ow a_ enabled changed.
        /// </summary>
        /// <param name="sender">
        ///     The sender.
        /// </param>
        /// <param name="e">
        ///     The e.
        /// </param>
        private void CbOverrideOwaEnabledChanged(object sender, EventArgs e)
        {
            if (!this.cbOverrideOWA.Enabled)
            {
                this.txtOWAEdit.Enabled = false;
            }
            else
            {
                if (this.cbOverrideOWA.Checked)
                {
                    this.txtOWAEdit.Enabled = true;
                }
            }
        }

        /// <summary>
        ///     The change log tool strip menu item_ click.
        /// </summary>
        /// <param name="sender">
        ///     The sender.
        /// </param>
        /// <param name="e">
        ///     The e.
        /// </param>
        private void ChangeLogToolStripMenuItemClick(object sender, EventArgs e)
        {
            if (this.frmChangeLog == null)
            {
                this.frmChangeLog = new ChangeLog(Settings.Default.RSSFeed);
            }

            this.frmChangeLog.ShowDialog();
        }

        /// <summary>
        ///     The chk autodiscovery_ checked changed.
        /// </summary>
        /// <param name="sender">
        ///     The sender.
        /// </param>
        /// <param name="e">
        ///     The e.
        /// </param>
        private void ChkAutodiscoveryCheckedChanged(object sender, EventArgs e)
        {
            if (this.booting)
            {
                return;
            }

            this.connection.UseAutodiscovery = this.chkAutodiscovery.Checked;
            this.scenario.Save();
            var state = this.chkAutodiscovery.Checked
                ? Resources.Form1_alwaysOpenOWAInIEToolStripMenuItem_CheckStateChanged_ON
                : Resources.Form1_alwaysOpenOWAInIEToolStripMenuItem_CheckStateChanged_OFF;
            this.AddLogEntry(string.Format("{0} {1}", Resources.Form1_chkAutodiscovery_CheckedChanged_Autodiscovery_is_switched, state));

            // Switch off some options when Autodiscovery is checked
            this.SelectAutodiscoveryOptions();

            // Re-evaluate settings
            this.UpdateServiceUrl();
            this.UpdateOwaUrl();
            this.UpdateEmail();
        }

        /// <summary>
        ///     The chk office 365 checked changed.
        /// </summary>
        /// <param name="sender">
        ///     The sender.
        /// </param>
        /// <param name="e">
        ///     The e.
        /// </param>
        private void ChkOffice365CheckedChanged(object sender, EventArgs e)
        {
            if (this.booting)
            {
                return;
            }

            this.connection.Office365 = this.chkOffice365.Checked;
            this.scenario.Save();
            this.ShellAutologin();
        }

        /// <summary>
        ///     The chk on domain_ checked changed.
        /// </summary>
        /// <param name="sender">
        ///     The sender.
        /// </param>
        /// <param name="e">
        ///     The e.
        /// </param>
        private void ChkOnDomainCheckedChanged(object sender, EventArgs e)
        {
            if (this.booting)
            {
                return;
            }

            this.connection.OnWindowsDomain = this.chkOnDomain.Checked;
            this.scenario.Save();

            // Switch off some options when domain authentication selected
            this.SelectDomainOptions();
        }

        /// <summary>
        ///     The chk run on startup_ checked changed.
        /// </summary>
        /// <param name="sender">
        ///     The sender.
        /// </param>
        /// <param name="e">
        ///     The e.
        /// </param>
        private void ChkRunOnStartupCheckedChanged(object sender, EventArgs e)
        {
            if (this.booting)
            {
                return;
            }

            this.RunAtStartup(this.chkRunOnStartup.Checked);
        }

        /// <summary>
        ///     The cmb exchange version_ selected index changed.
        /// </summary>
        /// <param name="sender">
        ///     The sender.
        /// </param>
        /// <param name="e">
        ///     The e.
        /// </param>
        private void CmbExchangeVersionSelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.booting)
            {
                return;
            }

            this.connection.ServerVersion = this.cmbExchangeVersion.Text;
            this.scenario.Save();
        }

        /// <summary>
        ///     The cmd start_ click.
        /// </summary>
        /// <param name="sender">
        ///     The sender.
        /// </param>
        /// <param name="e">
        ///     The e.
        /// </param>
        private void CmdStartClick(object sender, EventArgs e)
        {
            this.ConnectToExchange();
        }

        /// <summary>
        ///     The cmd stop_ click.
        /// </summary>
        /// <param name="sender">
        ///     The sender.
        /// </param>
        /// <param name="e">
        ///     The e.
        /// </param>
        private void CmdStopClick(object sender, EventArgs e)
        {
            this.DisconnectFromExchange();
        }

        /// <summary>
        ///     The configure shell.
        /// </summary>
        private void ConfigureShell()
        {
            this.ShellAutologin();
            this.ShellBrowserVersion();
            this.ShellExchangeVersion();
            this.ShellOwaUrl();
            this.ShellPassword();
        }

        /// <summary>
        ///     The connect to exchange.
        /// </summary>
        private void ConnectToExchange()
        {
            switch (this.connection.ConnectedState)
            {
                case ConnectionState.Connected:
                    this.AddLogEntry(
                        string.Format("{0}", Resources.Form1_ConnectToExchange_Already_connected),
                        Severity.Fail);
                    break;
                case ConnectionState.Disconnecting:
                case ConnectionState.Connecting:
                    this.AddLogEntry(
                        string.Format(
                            "{0}, {1}...",
                            Resources.Form1_ConnectToExchange_Transitioning_state,
                            Resources.Form1_ConnectToExchange_please_wait),
                        Severity.Fail);
                    break;
                default:
                    this.connection.ConnectA();
                    break;
            }
        }

        /// <summary>
        ///     The connected state handler.
        /// </summary>
        /// <param name="theConnection">
        ///     The theConnection.
        /// </param>
        /// <param name="state">
        ///     The state.
        /// </param>
        private void ConnectedStateHandler(IEmailInterface theConnection, ConnectionState state)
        {
            if (this.IsHandleCreated)
            {
                this.Invoke(
                    new Action(
                        () =>
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
                                    this.notifyIcon1.Text = AssemblyHelpers.AssemblyTitle + Environment.NewLine
                                                            + string.Format(
                                                                "{0}!",
                                                                Resources.Form1_WireUpConnectionEvents_Connection_Failure);
                                    this.PopToast(
                                        string.Format(
                                            "[{0}] - {1}!",
                                            theConnection.EmailAddress,
                                            Resources.Form1_WireUpConnectionEvents_Connection_Failure),
                                        Resources.Form1_WireUpConnectionEvents_Check_log_file_for_details);
                                    break;

                                case ConnectionState.Connected:

                                    // After a successful theConnection then next time we can autostart
                                    Settings.Default.Autostart = true;
                                    Settings.Default.Save();

                                    // Update discovered properties
                                    this.AddLogEntry(
                                        string.Format(
                                            "{0} {1}",
                                            Resources.Form1_WireUpConnectionEvents_Connected_to,
                                            theConnection.Version));
                                    if (theConnection.DiscoveredUsername.Length > 0)
                                    {
                                        this.AddLogEntry(
                                            string.Format(
                                                "{0}: {1}",
                                                Resources.Form1_WireUpConnectionEvents_Discovered_User_Name,
                                                theConnection.DiscoveredUsername),
                                            Severity.Success);
                                    }

                                    if (theConnection.DiscoveredEmailServer.Length > 0)
                                    {
                                        this.AddLogEntry(
                                            string.Format(
                                                "{0}: {1}",
                                                Resources.Form1_WireUpConnectionEvents_Discovered_Mailbox_Server,
                                                theConnection.DiscoveredEmailServer),
                                            Severity.Success);
                                    }

                                    if (theConnection.DiscoveredEmailUrl.Length > 0)
                                    {
                                        this.AddLogEntry(
                                            string.Format(
                                                "{0}: {1}",
                                                Resources.Form1_WireUpConnectionEvents_Discovered_OWA_Url,
                                                theConnection.DiscoveredEmailUrl),
                                            Severity.Success);
                                    }

                                    if (theConnection.DiscoveredServiceUrl.Length > 0)
                                    {
                                        this.AddLogEntry(
                                            string.Format(
                                                "{0}: {1}",
                                                Resources.Form1_WireUpConnectionEvents_Discovered_EWS_Url,
                                                theConnection.DiscoveredServiceUrl),
                                            Severity.Success);
                                    }

                                    // Configure Shell
                                    this.UpdateOwaUrl();
                                    this.UpdateServiceUrl();
                                    this.ShellExchangeVersion();

                                    // Minimize
                                    this.WindowState = FormWindowState.Minimized;
                                    this.notifyIcon1.Text = AssemblyHelpers.AssemblyTitle + Environment.NewLine
                                                            + Resources
                                                                .Form1_WireUpConnectionEvents_Connected_to_Exchange;
                                    break;

                                case ConnectionState.Disconnected:
                                    this.notifyIcon1.Text = AssemblyHelpers.AssemblyTitle + Environment.NewLine
                                                            + Resources.Form1_WindowDressing_Not_Connected_to_Exchange;
                                    break;
                            }
                        }));
            }
        }

        /// <summary>
        ///     The disable calendar tool strip menu item_ check state changed.
        /// </summary>
        /// <param name="sender">
        ///     The sender.
        /// </param>
        /// <param name="e">
        ///     The e.
        /// </param>
        private void DisableCalendarToolStripMenuItemCheckStateChanged(object sender, EventArgs e)
        {
            if (this.booting)
            {
                return;
            }

            this.connection.DisableCalendar = this.disableCalendarToolStripMenuItem.Checked;
            this.scenario.Save();

            var state = this.connection.DisableCalendar
                ? Resources.Form1_alwaysOpenOWAInIEToolStripMenuItem_CheckStateChanged_OFF
                : Resources.Form1_alwaysOpenOWAInIEToolStripMenuItem_CheckStateChanged_ON;
            this.AddLogEntry(string.Format("{0} {1}", Resources.Form1_disableCalendarToolStripMenuItem_CheckStateChanged_Calendar_notifications_switched, state));
        }

        /// <summary>
        ///     The disconnect from exchange.
        /// </summary>
        private void DisconnectFromExchange()
        {
            switch (this.connection.ConnectedState)
            {
                case ConnectionState.Disconnected:
                    this.AddLogEntry(string.Format("{0}", Resources.Form1_DisconnectFromExchange_Already_disconnected));
                    break;
                case ConnectionState.Disconnecting:
                case ConnectionState.Connecting:
                    this.AddLogEntry(
                        string.Format(
                            "{0}, {1}...",
                            Resources.Form1_DisconnectFromExchange_Transitioning_state,
                            Resources.Form1_DisconnectFromExchange_please_wait),
                        Severity.Fail);
                    break;
                default:
                    this.connection.Disconnect();
                    break;
            }
        }

        /// <summary>
        ///     The exit tool strip menu item 1_ click.
        /// </summary>
        /// <param name="sender">
        ///     The sender.
        /// </param>
        /// <param name="e">
        ///     The e.
        /// </param>
        private void ExitToolStripMenuItem1Click(object sender, EventArgs e)
        {
            this.Shutdown();
        }

        /// <summary>
        ///     The exit tool strip menu item_ click.
        /// </summary>
        /// <param name="sender">
        ///     The sender.
        /// </param>
        /// <param name="e">
        ///     The e.
        /// </param>
        private void ExitToolStripMenuItemClick(object sender, EventArgs e)
        {
            this.Shutdown();
        }

        /// <summary>
        ///     The flush output.
        /// </summary>
        private void FlushOutput()
        {
            // Avoid Illegal Cross Thread Calls
            if (this.IsHandleCreated)
            {
                this.Invoke(
                    new Action(
                        () =>
                        {
                            if (this.logBuffer.Count <= 0)
                            {
                                return;
                            }

                            // Avoid buffer overflows by trimming log after n entries
                            if (this.lvStatus.Items.Count >= Settings.Default.ScreenLines)
                            {
                                this.lvStatus.Items.Clear();
                            }

                            try
                            {
                                // Copy from buffer to screen control
                                this.lvStatus.BeginUpdate();

                                // Note that .AddRange has a bug so avoid
                                foreach (ListViewItem lv in this.logBuffer.Where(lv => lv != null))
                                {
                                    this.lvStatus.Items.Add(lv);
                                }
                            }
                            catch (Exception)
                            {
                            }
                            finally
                            {
                                // Make newest item visible
                                // We don't care about any spurious errors raised here
                                if (this.lvStatus.Items.Count > 0)
                                {
                                    try
                                    {
                                        this.lvStatus.EnsureVisible(this.lvStatus.Items.Count - 1);
                                        ListViewItem lv = this.lvStatus.Items[this.lvStatus.Items.Count - 1];
                                        string txt = lv.SubItems[1].Text;
                                        this.slStatus.Text = txt.Substring(0, txt.Length < 100 ? txt.Length : 100);
                                    }
                                    catch (Exception)
                                    {
                                    }
                                }

                                // Tidy up
                                this.logBuffer.Clear();
                                this.lvStatus.EndUpdate();
                                this.lvStatus.Refresh();
                                this.Refresh();
                            }
                        }));
            }
        }

        /// <summary>
        ///     Handles the Move event of the Form1 control.
        /// </summary>
        /// <param name="sender">
        ///     The source of the event.
        /// </param>
        /// <param name="e">
        ///     The <see cref="System.EventArgs" /> instance containing the event data.
        /// </param>
        private void Form1Move(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                this.Hide();
            }
            else
            {
                this.Show();
            }
        }

        /// <summary>
        ///     The lbl email_ link clicked.
        /// </summary>
        /// <param name="sender">
        ///     The sender.
        /// </param>
        /// <param name="e">
        ///     The e.
        /// </param>
        private void LblEmailLinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start(this.lblEmail.Text);
        }

        /// <summary>
        ///     The lbl owa url_ link clicked.
        /// </summary>
        /// <param name="sender">
        ///     The sender.
        /// </param>
        /// <param name="e">
        ///     The e.
        /// </param>
        private void LblOwaUrlLinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start(this.lblOWAUrl.Text);
        }

        /// <summary>
        ///     The lbl service url_ link clicked.
        /// </summary>
        /// <param name="sender">
        ///     The sender.
        /// </param>
        /// <param name="e">
        ///     The e.
        /// </param>
        private void LblServiceUrlLinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start(this.lblServiceUrl.Text);
        }

        /// <summary>
        ///     The login automatically tool strip menu item_ check state changed.
        /// </summary>
        /// <param name="sender">
        ///     The sender.
        /// </param>
        /// <param name="e">
        ///     The e.
        /// </param>
        private void LoginAutomaticallyToolStripMenuItemCheckStateChanged(object sender, EventArgs e)
        {
            if (this.booting)
            {
                return;
            }

            this.connection.AutoLogin = this.loginAutomaticallyToolStripMenuItem.Checked;
            this.scenario.Save();
            this.ShellAutologin();

            var state = this.connection.AutoLogin
                ? Resources.Form1_alwaysOpenOWAInIEToolStripMenuItem_CheckStateChanged_ON
                : Resources.Form1_alwaysOpenOWAInIEToolStripMenuItem_CheckStateChanged_OFF;
            this.AddLogEntry(string.Format("{0} {1}", Resources.Form1_loginAutomaticallyToolStripMenuItem_CheckStateChanged_Automatic_Login_is_switched, state));
        }

        /// <summary>
        ///     The mail count handler.
        /// </summary>
        /// <param name="count">
        ///     The count.
        /// </param>
        private void MailCountHandler(int count)
        {
            if (this.IsHandleCreated)
            {
                this.Invoke(
                    new Action(
                        () =>
                        {
                            this.mailCount = count;

                            this.notifyIcon1.Text = this.NotificationText(count);
                            this.notifyIcon1.Icon = new Icon(count > 0 ? this.alertIcon : this.emailIcon);

                            if (!this.firstRun)
                            {
                                return;
                            }

                            if (count <= 0)
                            {
                                return;
                            }

                            this.firstRun = false;

                            // Special case - pop message at the start if there is any unread email
                            this.PopToast(
                                "Unread Mail",
                                string.Format(
                                    "{0} {1} {2}{3}{4}",
                                    Resources.Form1_WireUpConnectionEvents_You_have,
                                    count,
                                    Resources.Form1_WireUpConnectionEvents_unread_email,
                                    count != 1 ? "s " : " ",
                                    Resources.Form1_WireUpConnectionEvents_in_your_inbox));
                        }));
            }
        }

        /// <summary>
        ///     The make owa default tool strip menu item_ click.
        /// </summary>
        /// <param name="sender">
        ///     The sender.
        /// </param>
        /// <param name="e">
        ///     The e.
        /// </param>
        private void MakeOwaDefaultToolStripMenuItemClick(object sender, EventArgs e)
        {
            if (!IsUserAdministrator())
            {
                this.AddLogEntry(
                    string.Format(
                        "{0}. {1}.",
                        Resources.Form1_makeOWADefaultToolStripMenuItem_Click_You_are_not_an_Admin_user,
                        Resources.Form1_makeOWADefaultToolStripMenuItem_Click_Operation_may_fail),
                    Severity.Fail);
            }

            // Configure registry
            this.AddLogEntry(Resources.Form1_makeOWADefaultToolStripMenuItem_Click_Setting_up_Mail_handlers);

            try
            {
                var runSvc = new ProcessStartInfo(this.shellPath)
                {
                    Arguments = "registry",
                    WindowStyle = ProcessWindowStyle.Hidden,
                };

                if (Environment.OSVersion.Version.Major >= 6)
                {
                    runSvc.Verb = "runas";
                }

                Process serviceProcess = Process.Start(runSvc);

                while (!serviceProcess.HasExited)
                {
                    Thread.Sleep(100);
                    Application.DoEvents();
                }
            }
            catch (Exception ex)
            {
                this.AddLogEntry(string.Format("{0}: {1}", Resources.Form1_ShellOwaUrl_Error, ex.Message), ex);
                return;
            }

            this.AddLogEntry(
                Resources.Form1_makeOWADefaultToolStripMenuItem_Click_Mail_functions_will_now_be_handled_by_OWA,
                Severity.Success);
        }

        /// <summary>
        ///     The net versions tool strip menu item_ click.
        /// </summary>
        /// <param name="sender">
        ///     The sender.
        /// </param>
        /// <param name="e">
        ///     The e.
        /// </param>
        private void NetVersionsToolStripMenuItemClick(object sender, EventArgs e)
        {
            if (this.frmNet == null)
            {
                this.frmNet = new NeTversions();
            }

            this.frmNet.ShowDialog();
        }

        /// <summary>
        ///     The new appointment handler.
        /// </summary>
        /// <param name="minsToGo">
        ///     The mins to go.
        /// </param>
        /// <param name="startTime">
        ///     The start time.
        /// </param>
        /// <param name="subject">
        ///     The subject.
        /// </param>
        /// <param name="location">
        ///     The location.
        /// </param>
        /// <param name="accessUrl">
        ///     The access url.
        /// </param>
        private void NewAppointmentHandler(
            int minsToGo,
            DateTime startTime,
            string subject,
            string location,
            string accessUrl)
        {
            if (this.IsHandleCreated)
            {
                this.Invoke(
                    new Action(
                        () =>
                        {
                            this.popUrl = accessUrl;
                            string minutesUnit = minsToGo != 1
                                ? Resources.Form1_WireUpConnectionEvents_minutes
                                : Resources.Form1_WireUpConnectionEvents_minute;
                            this.PopToast(
                                string.Format(
                                    "{0} {1} {2}",
                                    Resources.Form1_WireUpConnectionEvents_You_have_an_appointment_in,
                                    minsToGo,
                                    minutesUnit),
                                string.Format("{0} - {1} ({2})", startTime.ToShortTimeString(), subject, location));
                        }));
            }
        }

        /// <summary>
        ///     The new mail handler.
        /// </summary>
        /// <param name="subject">
        ///     The subject.
        /// </param>
        /// <param name="sender">
        ///     The sender.
        /// </param>
        /// <param name="accessUrl">
        ///     The access url.
        /// </param>
        private void NewMailHandler(string subject, string sender, string accessUrl)
        {
            if (this.IsHandleCreated)
            {
                this.Invoke(
                    new Action(
                        () =>
                        {
                            this.popUrl = accessUrl;
                            this.PopToast(
                                string.Format("{0} {1}", Resources.Form1_WireUpConnectionEvents_New_Mail_from, sender),
                                subject);
                        }));
            }
        }

        /// <summary>
        ///     Notifications the text.
        /// </summary>
        /// <returns>
        ///     The text.
        /// </returns>
        private string NotificationText()
        {
            return this.NotificationText(this.mailCount);
        }

        /// <summary>
        ///     The notification text.
        /// </summary>
        /// <param name="myCount">
        ///     The my count.
        /// </param>
        /// <returns>
        ///     The text.
        /// </returns>
        private string NotificationText(int myCount)
        {
            const int MaxTipLength = 63;
            string text1 = string.Format(
                "{0}{1}{1}{2} {3}{4}",
                AssemblyHelpers.AssemblyTitle,
                Environment.NewLine,
                myCount,
                Resources.Form1_WireUpConnectionEvents_unread_email,
                myCount != 1 ? "s " : " ");

            int charsLeft = MaxTipLength - text1.Length;
            string domainText = string.Format(
                "{0}\\{1}",
                this.connection.DiscoveredEmailServer,
                this.connection.DiscoveredUsername);

            if (this.connection.Description.Length > 0)
            {
                domainText = this.connection.Description;
            }

            if (domainText.Length > charsLeft)
            {
                domainText = domainText.Substring(0, charsLeft);
            }

            string finalText = string.Format(
                "{0}{1}{2}{1}{3} {4}{5}",
                AssemblyHelpers.AssemblyTitle,
                Environment.NewLine,
                domainText,
                myCount,
                Resources.Form1_WireUpConnectionEvents_unread_email,
                myCount != 1 ? "s " : " ");

            return finalText;
        }

        /// <summary>
        ///     The notify icon 1_ balloon tip clicked.
        /// </summary>
        /// <param name="sender">
        ///     The sender.
        /// </param>
        /// <param name="e">
        ///     The e.
        /// </param>
        [SuppressMessage(
            "StyleCop.CSharp.ReadabilityRules",
            "SA1126:PrefixCallsCorrectly",
            Justification = "Reviewed. Suppression is OK here.")]
        private void NotifyIcon1BalloonTipClicked(object sender, EventArgs e)
        {
            this.ActivateOwa(this.popUrl);
        }

        /// <summary>
        ///     The notify icon 1_ mouse double click.
        /// </summary>
        /// <param name="sender">
        ///     The sender.
        /// </param>
        /// <param name="e">
        ///     The e.
        /// </param>
        private void NotifyIcon1MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.ActivateOwa();
        }

        /// <summary>
        ///     The office 365 login override tool strip menu item_ check state changed.
        /// </summary>
        /// <param name="sender">
        ///     The sender.
        /// </param>
        /// <param name="e">
        ///     The e.
        /// </param>
        private void Office365LoginOverrideToolStripMenuItemCheckStateChanged(object sender, EventArgs e)
        {
            if (this.booting)
            {
                return;
            }

            this.connection.OverrideOffice365Login = this.office365LoginOverrideToolStripMenuItem.Checked;
            this.scenario.Save();

            var state = this.connection.OverrideOffice365Login
                ? Resources.Form1_alwaysOpenOWAInIEToolStripMenuItem_CheckStateChanged_ON
                : Resources.Form1_alwaysOpenOWAInIEToolStripMenuItem_CheckStateChanged_OFF;
            this.AddLogEntry(string.Format("{0} {1}", Resources.Form1_office365LoginOverrideToolStripMenuItem_CheckStateChanged_Office365_login_override, state));

            this.UpdateOwaUrl();
        }

        /// <summary>
        ///     The open outlook tool strip menu item_ click.
        /// </summary>
        /// <param name="sender">
        ///     The sender.
        /// </param>
        /// <param name="e">
        ///     The e.
        /// </param>
        private void OpenOutlookToolStripMenuItemClick(object sender, EventArgs e)
        {
            try
            {
                Process.Start(Settings.Default.OutlookPath);
            }
            catch (Exception ex)
            {
                this.AddLogEntry(ex.Message, ex);
            }
        }

        /// <summary>
        ///     The open owa tool strip menu item_ click.
        /// </summary>
        /// <param name="sender">
        ///     The sender.
        /// </param>
        /// <param name="e">
        ///     The e.
        /// </param>
        private void OpenOwaToolStripMenuItemClick(object sender, EventArgs e)
        {
            this.ActivateOwa();
        }

        /// <summary>
        ///     The override autodiscovery validation tool strip menu item_ check state changed.
        /// </summary>
        /// <param name="sender">
        ///     The sender.
        /// </param>
        /// <param name="e">
        ///     The e.
        /// </param>
        private void OverrideAutodiscoveryValidationToolStripMenuItemCheckStateChanged(object sender, EventArgs e)
        {
            if (this.booting)
            {
                return;
            }

            this.connection.OverrideAutodiscoveryValidation =
                this.overrideAutodiscoveryValidationToolStripMenuItem.Checked;
            this.scenario.Save();

            var state = this.connection.OverrideAutodiscoveryValidation
                ? Resources.Form1_alwaysOpenOWAInIEToolStripMenuItem_CheckStateChanged_ON
                : Resources.Form1_alwaysOpenOWAInIEToolStripMenuItem_CheckStateChanged_OFF;
            this.AddLogEntry(string.Format("{0} {1}", Resources.Form1_overrideAutodiscoveryValidationToolStripMenuItem_CheckStateChanged_Autodiscovery_Validation_override_switched, state));
        }

        /// <summary>
        ///     The override certificate tool strip menu item_ check state changed.
        /// </summary>
        /// <param name="sender">
        ///     The sender.
        /// </param>
        /// <param name="e">
        ///     The e.
        /// </param>
        private void OverrideCertificateToolStripMenuItemCheckStateChanged(object sender, EventArgs e)
        {
            if (this.booting)
            {
                return;
            }

            this.connection.OverrideCertificate = this.overrideCertificateToolStripMenuItem.Checked;
            this.scenario.Save();

            var state = this.connection.OverrideCertificate
                ? Resources.Form1_alwaysOpenOWAInIEToolStripMenuItem_CheckStateChanged_ON
                : Resources.Form1_alwaysOpenOWAInIEToolStripMenuItem_CheckStateChanged_OFF;
            this.AddLogEntry(string.Format("{0} {1}", Resources.Form1_overrideCertificateToolStripMenuItem_CheckStateChanged_SSL_Certificate_override_switched, state));
        }

        /// <summary>
        ///     The play sound tool strip menu item_ check state changed.
        /// </summary>
        /// <param name="sender">
        ///     The sender.
        /// </param>
        /// <param name="e">
        ///     The e.
        /// </param>
        private void PlaySoundToolStripMenuItemCheckStateChanged(object sender, EventArgs e)
        {
            if (this.booting)
            {
                return;
            }

            Settings.Default.Bell = this.playSoundToolStripMenuItem.Checked;
            Settings.Default.Save();
            var state = Settings.Default.Bell
                ? Resources.Form1_alwaysOpenOWAInIEToolStripMenuItem_CheckStateChanged_ON
                : Resources.Form1_alwaysOpenOWAInIEToolStripMenuItem_CheckStateChanged_OFF;
            this.AddLogEntry(string.Format("{0} {1}", Resources.Form1_playSoundToolStripMenuItem_CheckStateChanged_Audible_notifications_switched, state));
        }

        /// <summary>
        ///     The pop toast.
        /// </summary>
        /// <param name="myTitle">
        ///     The my title.
        /// </param>
        /// <param name="myMessage">
        ///     The my message.
        /// </param>
        private void PopToast(string myTitle, string myMessage)
        {
            // Belt & Braces
            if (myTitle.Length == 0)
            {
                myTitle = string.Format("<{0}>", Resources.Form1_PopToast_No_Title);
            }

            if (myMessage.Length == 0)
            {
                myMessage = string.Format("<{0}>", Resources.Form1_PopToast_No_Subject);
            }

            this.AddLogEntry(myTitle);

            // Store for recall
            this.lastPopTitle = myTitle;
            this.lastPopMessage = myMessage;
            this.lastPopUrl = this.popUrl;

            // Balloon
            if (Settings.Default.Balloon)
            {
                this.notifyIcon1.Tag = this.popUrl;
                this.notifyIcon1.ShowBalloonTip(5000, myTitle, myMessage, ToolTipIcon.Info);
            }

            // Audible
            if (Settings.Default.Bell)
            {
                AudioHelper.Play(this.audioPath);
            }
        }

        /// <summary>
        ///     The recall last popup tool strip menu item_ click.
        /// </summary>
        /// <param name="sender">
        ///     The sender.
        /// </param>
        /// <param name="e">
        ///     The e.
        /// </param>
        private void RecallLastPopupToolStripMenuItemClick(object sender, EventArgs e)
        {
            if (this.lastPopMessage.Length <= 0 || this.lastPopTitle.Length <= 0)
            {
                return;
            }

            this.popUrl = this.lastPopUrl;
            this.PopToast(this.lastPopTitle, this.lastPopMessage);
        }

        /// <summary>
        ///     The reset tray icon tool strip menu item_ click.
        /// </summary>
        /// <param name="sender">
        ///     The sender.
        /// </param>
        /// <param name="e">
        ///     The e.
        /// </param>
        private void ResetTrayIconToolStripMenuItemClick(object sender, EventArgs e)
        {
            this.notifyIcon1.Icon = new Icon(this.emailIcon);
        }

        /// <summary>
        ///     The restore tool strip menu item_ click.
        /// </summary>
        /// <param name="sender">
        ///     The sender.
        /// </param>
        /// <param name="e">
        ///     The e.
        /// </param>
        private void RestoreToolStripMenuItemClick(object sender, EventArgs e)
        {
            this.allowVisible = true;
            this.Show();
            this.WindowState = FormWindowState.Normal;
        }

        /// <summary>
        ///     The run at startup.
        /// </summary>
        /// <param name="switchOn">
        ///     The switch on.
        /// </param>
        private void RunAtStartup(bool switchOn)
        {
            try
            {
                WindowsShortcut.Update(
                    Environment.SpecialFolder.Startup,
                    Application.ExecutablePath,
                    AssemblyHelpers.AssemblyTitle,
                    switchOn);
                this.AddLogEntry(
                    string.Format(
                        "{0} {1} {2}",
                        Resources.Form1_RunAtStartup_OWAtray_will,
                        switchOn ? string.Empty : Resources.Form1_RunAtStartup__not,
                        Resources.Form1_RunAtStartup_autostart_with_Windows));
            }
            catch (Exception ex)
            {
                this.AddLogEntry(ex.Message, ex);
            }
        }

        private void ScenarioScenarioEvent(string obj)
        {
            this.AddLogEntry(obj, Severity.Fail);
        }

        /// <summary>
        ///     Routes deep diagnostic messages (from a <see cref="Scenario"/> or connection's
        ///     <c>DebugMessage</c> event) straight to the file log - unlike <see cref="AddLogEntry(string, Severity)"/>,
        ///     this never touches the on-screen connection log.
        /// </summary>
        /// <param name="message">
        ///     The message.
        /// </param>
        private void DebugLogHandler(string message)
        {
            LoggerProxy.Debug(message);
        }

        /// <summary>
        ///     The select autodiscovery options.
        /// </summary>
        private void SelectAutodiscoveryOptions()
        {
            this.txtServer.Enabled = !this.connection.UseAutodiscovery;
            this.cbOverrideEWS.Enabled = !this.connection.UseAutodiscovery;
            this.cbOverrideOWA.Enabled = !this.connection.UseAutodiscovery;
            this.txtDomain.Enabled = !this.connection.UseAutodiscovery;
            this.overrideAutodiscoveryValidationToolStripMenuItem.Enabled = this.connection.UseAutodiscovery;
        }

        /// <summary>
        ///     The select domain options.
        /// </summary>
        private void SelectDomainOptions()
        {
            this.txtDomain.Enabled = !this.connection.OnWindowsDomain;
            this.txtPwd.Enabled = !this.connection.OnWindowsDomain;
            this.txtUser.Enabled = !this.connection.OnWindowsDomain;
        }

        /// <summary>
        ///     The shell autologin.
        /// </summary>
        private void ShellAutologin()
        {
            try
            {
                var runSvc = new ProcessStartInfo(this.shellPath)
                {
                    Arguments =
                        "autologin " + (this.connection.AutoLogin ? "Yes" : "No") + " "
                        + (this.connection.Office365 ? "Yes" : "No"),
                    WindowStyle = ProcessWindowStyle.Hidden,
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
                this.AddLogEntry(string.Format("{0}: {1}", Resources.Form1_ShellOwaUrl_Error, ex.Message), ex);
            }
        }

        /// <summary>
        ///     The shell browser version.
        /// </summary>
        private void ShellBrowserVersion()
        {
            try
            {
                var runSvc = new ProcessStartInfo(this.shellPath)
                {
                    Arguments = "browser " + (this.connection.AlwaysUseInternetExplorer ? "Yes" : "No"),
                    WindowStyle = ProcessWindowStyle.Hidden,
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
                this.AddLogEntry(string.Format("{0}: {1}", Resources.Form1_ShellOwaUrl_Error, ex.Message), ex);
            }
        }

        /// <summary>
        ///     The shell exchange version.
        /// </summary>
        private void ShellExchangeVersion()
        {
            try
            {
                var runSvc = new ProcessStartInfo(this.shellPath)
                {
                    Arguments = "exchange " + this.connection.Version,
                    WindowStyle = ProcessWindowStyle.Hidden,
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
                this.AddLogEntry(string.Format("{0}: {1}", Resources.Form1_ShellOwaUrl_Error, ex.Message), ex);
            }
        }

        /// <summary>
        ///     The shell owa url.
        /// </summary>
        private void ShellOwaUrl()
        {
            try
            {
                var runSvc = new ProcessStartInfo(this.shellPath)
                {
                    Arguments = "url " + this.connection.DerivedEmailUrl,
                    WindowStyle = ProcessWindowStyle.Hidden,
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
                this.AddLogEntry(string.Format("{0}: {1}", Resources.Form1_ShellOwaUrl_Error, ex.Message), ex);
            }
        }

        /// <summary>
        ///     The shell password.
        /// </summary>
        private void ShellPassword()
        {
            try
            {
                var runSvc = new ProcessStartInfo(this.shellPath)
                {
                    Arguments = "password " + this.connection.Password,
                    WindowStyle = ProcessWindowStyle.Hidden,
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
                this.AddLogEntry(string.Format("{0}: {1}", Resources.Form1_ShellOwaUrl_Error, ex.Message), ex);
            }
        }

        /// <summary>
        ///     The show log file tool strip menu item_ click.
        /// </summary>
        /// <param name="sender">
        ///     The sender.
        /// </param>
        /// <param name="e">
        ///     The e.
        /// </param>
        private void ShowLogFileToolStripMenuItemClick(object sender, EventArgs e)
        {
            try
            {
                Process.Start(LoggerProxy.Filename);
            }
            catch (Exception ex)
            {
                this.AddLogEntry(ex.Message, ex);
            }
        }

        /// <summary>
        ///     The shutdown.
        /// </summary>
        private void Shutdown()
        {
            this.allowClose = this.allowVisible = true;

            this.AddLogEntry(Resources.Form1_Form1_FormClosed_Terminating);
            if (this.bootOk)
            {
                this.UnwireConnectionEvents();
                this.DisconnectFromExchange();
            }

            this.Close();
        }

        /// <summary>
        ///     The support tool strip menu item_ click.
        /// </summary>
        /// <param name="sender">
        ///     The sender.
        /// </param>
        /// <param name="e">
        ///     The e.
        /// </param>
        private void SupportToolStripMenuItemClick(object sender, EventArgs e)
        {
            if (this.frmContact == null)
            {
                this.frmContact = new ContactUs();
            }

            this.frmContact.ShowDialog();
        }

        /// <summary>
        ///     The switch off tool strip menu item_ click.
        /// </summary>
        /// <param name="sender">
        ///     The sender.
        /// </param>
        /// <param name="e">
        ///     The e.
        /// </param>
        private void SwitchOffToolStripMenuItemClick(object sender, EventArgs e)
        {
            if (!IsUserAdministrator())
            {
                this.AddLogEntry(
                    string.Format(
                        "{0}. {1}.",
                        Resources.Form1_switchOffToolStripMenuItem_Click_You_are_not_an_Admin_user,
                        Resources.Form1_switchOffToolStripMenuItem_Click_Operation_may_fail),
                    Severity.Fail);
            }

            // Configure registry
            this.AddLogEntry(Resources.Form1_switchOffToolStripMenuItem_Click_Restoring_Mail_handlers);

            try
            {
                var runSvc = new ProcessStartInfo(this.shellPath)
                {
                    Arguments = "restore",
                    WindowStyle = ProcessWindowStyle.Hidden,
                };
                if (Environment.OSVersion.Version.Major >= 6)
                {
                    runSvc.Verb = "runas";
                }

                Process serviceProcess = Process.Start(runSvc);

                while (!serviceProcess.HasExited)
                {
                    Thread.Sleep(100);
                    Application.DoEvents();
                }
            }
            catch (Exception ex)
            {
                this.AddLogEntry(string.Format("{0}: {1}", Resources.Form1_ShellOwaUrl_Error, ex.Message), ex);
                return;
            }

            this.AddLogEntry("Mail handler restored to system default", Severity.Success);
        }

        /// <summary>
        ///     The system information tool strip menu item_ click.
        /// </summary>
        /// <param name="sender">
        ///     The sender.
        /// </param>
        /// <param name="e">
        ///     The e.
        /// </param>
        private void SystemInformationToolStripMenuItemClick(object sender, EventArgs e)
        {
            if (this.frmInfo == null)
            {
                this.frmInfo = new SysInfo();
            }

            this.frmInfo.ShowDialog();
        }

        /// <summary>
        ///     The timer 1_ tick.
        /// </summary>
        /// <param name="sender">
        ///     The sender.
        /// </param>
        /// <param name="e">
        ///     The e.
        /// </param>
        private void Timer1Tick(object sender, EventArgs e)
        {
            try
            {
                this.timer1.Enabled = false;
                this.timerLogging.Enabled = true;

                // Interlock for booting up
                this.booting = true;

                // Boot the various subsystems
                this.BootEnvironment();
                this.BootShell();
                this.BootAudio();
                this.BootScenario();
                this.BootIcons();

                // Connect if autostart is good to go
                if (Settings.Default.Autostart)
                {
                    this.ConnectToExchange();
                }
                else
                {
                    this.allowVisible = true;
                    this.Show();
                }

                // Only getting here means we've booted up ok
                this.bootOk = true;
            }
            catch (Exception ex)
            {
                this.AddLogEntry(string.Format("{0}", ex.Message), Severity.Fail);
            }
            finally
            {
                // Release boot interlock
                this.booting = false;
            }
        }

        /// <summary>
        ///     The timer logging_ tick.
        /// </summary>
        /// <param name="sender">
        ///     The sender.
        /// </param>
        /// <param name="e">
        ///     The e.
        /// </param>
        private void TimerLoggingTick(object sender, EventArgs e)
        {
            this.FlushOutput();
        }

        /// <summary>
        ///     The txt description_ validated.
        /// </summary>
        /// <param name="sender">
        ///     The sender.
        /// </param>
        /// <param name="e">
        ///     The e.
        /// </param>
        private void TxtDescriptionValidated(object sender, EventArgs e)
        {
            this.connection.Description = this.txtDescription.Text;
            this.scenario.Save();
            this.notifyIcon1.Text = this.NotificationText();
        }

        /// <summary>
        ///     The txt domain_ validated.
        /// </summary>
        /// <param name="sender">
        ///     The sender.
        /// </param>
        /// <param name="e">
        ///     The e.
        /// </param>
        private void TxtDomainValidated(object sender, EventArgs e)
        {
            this.connection.AccountDomain = this.txtDomain.Text;
            this.scenario.Save();
        }

        /// <summary>
        ///     The txt email_ validated.
        /// </summary>
        /// <param name="sender">
        ///     The sender.
        /// </param>
        /// <param name="e">
        ///     The e.
        /// </param>
        private void TxtEmailValidated(object sender, EventArgs e)
        {
            this.connection.EmailAddress = this.txtEmail.Text;
            this.scenario.Save();
            this.UpdateEmail();
        }

        /// <summary>
        ///     The txt interval_ validated.
        /// </summary>
        /// <param name="sender">
        ///     The sender.
        /// </param>
        /// <param name="e">
        ///     The e.
        /// </param>
        private void TxtIntervalValidated(object sender, EventArgs e)
        {
            this.connection.Interval = Convert.ToInt32(this.txtInterval.Text);
            this.scenario.Save();
        }

        /// <summary>
        ///     The txt interval_ validating.
        /// </summary>
        /// <param name="sender">
        ///     The sender.
        /// </param>
        /// <param name="e">
        ///     The e.
        /// </param>
        private void TxtIntervalValidating(object sender, CancelEventArgs e)
        {
            int result;

            if (int.TryParse(this.txtInterval.Text, out result))
            {
                if (result >= 1 && result <= MaxInterval)
                {
                    this.errorProvider1.SetError(this.txtInterval, string.Empty);
                    e.Cancel = false;
                }
                else
                {
                    string errorMessage = Resources.Form1_txtInterval_Validating_Must_be_a_numeric_value_between_1_and_
                        + MaxInterval.ToString();
                    this.errorProvider1.SetError(this.txtInterval, errorMessage);
                    e.Cancel = true;
                }
            }
            else
            {
                string errorMessage = Resources.Form1_txtInterval_Validating_Must_be_a_numeric_value_between_1_and_
                    + MaxInterval.ToString();
                this.errorProvider1.SetError(this.txtInterval, errorMessage);
                e.Cancel = true;
            }
        }

        /// <summary>
        ///     The txt owa edit_ validated.
        /// </summary>
        /// <param name="sender">
        ///     The sender.
        /// </param>
        /// <param name="e">
        ///     The e.
        /// </param>
        private void TxtOwaEditValidated(object sender, EventArgs e)
        {
            this.connection.EmailUrl = this.txtOWAEdit.Text;
            this.scenario.Save();
            this.UpdateOwaUrl();
        }

        /// <summary>
        ///     The txt pwd_ validated.
        /// </summary>
        /// <param name="sender">
        ///     The sender.
        /// </param>
        /// <param name="e">
        ///     The e.
        /// </param>
        private void TxtPwdValidated(object sender, EventArgs e)
        {
            this.connection.Password = this.txtPwd.Text;
            this.scenario.Save();
            this.ShellPassword();
        }

        /// <summary>
        ///     The txt server_ validated.
        /// </summary>
        /// <param name="sender">
        ///     The sender.
        /// </param>
        /// <param name="e">
        ///     The e.
        /// </param>
        private void TxtServerValidated(object sender, EventArgs e)
        {
            this.connection.EmailServer = this.txtServer.Text;
            this.scenario.Save();
            this.UpdateServiceUrl();
            this.UpdateOwaUrl();
        }

        /// <summary>
        ///     The txt url edit_ validated.
        /// </summary>
        /// <param name="sender">
        ///     The sender.
        /// </param>
        /// <param name="e">
        ///     The e.
        /// </param>
        private void TxtUrlEditValidated(object sender, EventArgs e)
        {
            this.connection.ServiceUrl = this.txtURLEdit.Text;
            this.scenario.Save();
            this.UpdateServiceUrl();
        }

        /// <summary>
        ///     The txt user_ validated.
        /// </summary>
        /// <param name="sender">
        ///     The sender.
        /// </param>
        /// <param name="e">
        ///     The e.
        /// </param>
        private void TxtUserValidated(object sender, EventArgs e)
        {
            this.connection.Username = this.txtUser.Text;
            this.scenario.Save();
            this.UpdateEmail();
        }

        /// <summary>
        ///     The unwire theConnection events.
        /// </summary>
        private void UnwireConnectionEvents()
        {
            foreach (IEmailInterface item in this.scenario.Connections.Where(item => item.AreEventsDefined))
            {
                item.LogMessage -= this.AddLogEntry;
                item.LogException -= this.AddLogEntry;
                item.DebugMessage -= this.DebugLogHandler;
                item.ConnectedStateChange -= this.ConnectedStateHandler;
                item.NewMail -= this.NewMailHandler;
                item.NewAppointment -= this.NewAppointmentHandler;
                item.MessageCount -= this.MailCountHandler;
            }
        }

        /// <summary>
        ///     The update email.
        /// </summary>
        private void UpdateEmail()
        {
            this.lblEmail.Text = this.EmailAddress;
        }

        /// <summary>
        ///     The update owa url.
        /// </summary>
        private void UpdateOwaUrl()
        {
            if (this.connection.OverrideOffice365Login)
            {
                this.lblOWAUrl.Text = Settings.Default.Office365OwaUrl + StripEmailDomain(this.lblEmail.Text);
            }
            else if (this.connection.UseAutodiscovery && this.connection.DiscoveredEmailUrl.Length > 0)
            {
                this.lblOWAUrl.Text = this.connection.DiscoveredEmailUrl;
            }
            else if (this.connection.OverrideEmailUrl && this.txtOWAEdit.Text.Length > 0)
            {
                this.lblOWAUrl.Text = this.txtOWAEdit.Text;
            }
            else if (this.txtServer.Text.Length > 0)
            {
                this.lblOWAUrl.Text = string.Format("{0}{1}{2}", "https://", this.txtServer.Text, "/owa/");
            }
            else
            {
                this.lblOWAUrl.Text = string.Empty;
            }

            this.connection.DerivedEmailUrl = this.lblOWAUrl.Text;
            this.ShellOwaUrl();
        }

        /// <summary>
        ///     The update service url.
        /// </summary>
        private void UpdateServiceUrl()
        {
            if (this.connection.UseAutodiscovery && this.connection.DiscoveredServiceUrl.Length > 0)
            {
                this.lblServiceUrl.Text = this.connection.DiscoveredServiceUrl;
            }
            else if (this.connection.OverrideServiceUrl && this.txtURLEdit.Text.Length > 0)
            {
                this.lblServiceUrl.Text = this.txtURLEdit.Text;
            }
            else if (this.txtServer.Text.Length > 0)
            {
                this.lblServiceUrl.Text = string.Format(
                    "{0}{1}{2}",
                    "https://",
                    this.txtServer.Text,
                    "/ews/exchange.asmx");
            }
            else
            {
                this.lblServiceUrl.Text = string.Empty;
            }

            this.connection.DerivedServiceUrl = this.lblServiceUrl.Text;
        }

        /// <summary>
        ///     The use default web proxy tool strip menu item_ click.
        /// </summary>
        /// <param name="sender">
        ///     The sender.
        /// </param>
        /// <param name="e">
        ///     The e.
        /// </param>
        private void UseDefaultWebProxyToolStripMenuItemClick(object sender, EventArgs e)
        {
            if (this.booting)
            {
                return;
            }

            Settings.Default.UseWebProxy = this.useDefaultWebProxyToolStripMenuItem.Checked;
            Settings.Default.Save();

            UpdateWebProxySettings();
        }

        /// <summary>
        ///     The window dressing.
        /// </summary>
        private void WindowDressing()
        {
            this.Text = string.Format(
                "{0} {1} {2}",
                AssemblyHelpers.AssemblyTitle,
                Resources.Form1_WindowDressing_freshly_baked_at,
                AssemblyHelpers.AssemblyCompany);
            this.notifyIcon1.Text = AssemblyHelpers.AssemblyTitle + Environment.NewLine
                                    + Resources.Form1_WindowDressing_Not_Connected_to_Exchange;
            foreach (TabPage tab in this.tabMain.TabPages)
            {
                tab.BackColor = SystemColors.Control;
            }

            InitEventView(this.lvStatus);
        }

        /// <summary>
        ///     The wire up theConnection events.
        /// </summary>
        private void WireUpConnectionEvents()
        {
            foreach (IEmailInterface item in this.scenario.Connections.Where(item => !item.AreEventsDefined))
            {
                item.LogMessage += this.AddLogEntry;
                item.LogException += this.AddLogEntry;
                item.DebugMessage += this.DebugLogHandler;
                item.ConnectedStateChange += this.ConnectedStateHandler;
                item.NewMail += this.NewMailHandler;
                item.NewAppointment += this.NewAppointmentHandler;
                item.MessageCount += this.MailCountHandler;
            }
        }

        #endregion
    }
}