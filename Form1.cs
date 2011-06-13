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
namespace DrunkenBakery.OWAtray
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Drawing;
    using System.IO;
    using System.Net;
    using System.Security;
    using System.Security.Principal;
    using System.Windows.Forms;
    using Growl.Connector;
    using Microsoft.Exchange.WebServices.Data;
    using Microsoft.Exchange.WebServices.Autodiscover;
    using Snarl;

    /// <summary>
    /// Main application form which drives all functionality.
    /// </summary>
    public partial class Form1 : Form
    {
        #region Fields

        const int MaxInterval = 3600;
        const Int32 REPLY_MSG = 0x400 + 100;
        const int ScreenLines = 1000;
        const int ScreenRefresh = 1;
        const string ThisApp = "OWA Tray Monitor";
        const string ThisPublisher = "The Drunken Bakery";

        static byte[] entropy = System.Text.Encoding.Unicode.GetBytes("Salt Is Not A Password");
        static bool overRideClose = false;
        static FlatFile myLog;
        Growl.Connector.Application growlApp;
        GrowlConnector growl;
        bool firstRun = true;
        Form frmAbout;
        Form frmChangeLog;
        Form frmContact;
        Form frmInfo;
        Form frmMDAC;
        Form frmNET;
        string iconPath;
        string lastLogEntry;
        string lastPopMessage;
        string lastPopTitle;
        string lastPopUrl;
        List<ListViewItem> lvBuffer = new List<ListViewItem>();
        ExchangeService myService;
        string newIcon;
        NotificationType newMail;
        bool resetFlag;
        string shellPath;
        DateTime TimeLastChecked = DateTime.Now;
        string trayIcon;
        string wavFile;
        int inboxCount;
        string popUrl;
        ExchangeVersion reportedVersion = ExchangeVersion.Exchange2007_SP1;
        string reportedEwsUrl = "";
        string reportedOwaUrl = "";
        string reportedMailboxServer = "";
        string reportedUserName = "";
        bool startingUp;
        string Office365Account = "";

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Form1"/> class.
        /// </summary>
        public Form1()
        {
            InitializeComponent();

            // Upgrade settings from older version
            System.Reflection.Assembly a = System.Reflection.Assembly.GetExecutingAssembly();
            Version appVersion = a.GetName().Version;
            string appVersionString = appVersion.ToString();

            if (Properties.Settings.Default.ApplicationVersion != appVersion.ToString())
            {
                Properties.Settings.Default.Upgrade();
                Properties.Settings.Default.ApplicationVersion = appVersionString;
            }

            // Interlock for starting up
            startingUp = true;

            // Start Logging
            InitLogger();

            // Logging
            AddLogEntry("--------------------------------------------------", LogType.Info);
            AddLogEntry("Welcome to the " + ThisApp + " v" + appVersionString, LogType.Info);
            notifyIcon1.Text = ThisApp + Environment.NewLine + "Not Connected to Exchange";

            // Initialise Event Views
            InitEventView(lvStatus);

            // Tabs
            foreach (TabPage _tab in tabMain.TabPages)
            {
                _tab.BackColor = SystemColors.Control;
            }

            // Options
            exchange2007ToolStripMenuItem.SelectedIndex = 0;
            switch (Properties.Settings.Default.ExchangeVersion)
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

            txtEmail.Text = Properties.Settings.Default.EMail;
            txtServer.Text = Properties.Settings.Default.Server;
            txtUser.Text = Properties.Settings.Default.Username;
            txtPwd.Text = ToInsecureString(DecryptString(Properties.Settings.Default.Password));
            txtDomain.Text = Properties.Settings.Default.Domain;
            txtInterval.Text = Properties.Settings.Default.UpdateInterval.ToString();
            txtURLEdit.Text = Properties.Settings.Default.ManualURL;
            txtOWAEdit.Text = Properties.Settings.Default.ManualOWAUrl;

            // Form title bar
            this.Text = ThisApp + " freshly baked at " + ThisPublisher;

            // Startup Flag
            chkRunOnStartup.Checked = Link.Exists(Environment.SpecialFolder.Startup, ThisApp);

            // Notifications
            balloonToolStripMenuItem.Checked = Properties.Settings.Default.Balloon;
            growlToolStripMenuItem.Checked = Properties.Settings.Default.Growl;
            snarlToolStripMenuItem.Checked = Properties.Settings.Default.Snarl;
            playSoundToolStripMenuItem.Checked = Properties.Settings.Default.Bell;

            // Overrides
            overrideToolStripMenuItem.Checked = Properties.Settings.Default.OverrideCert;
            cbOverrideEWS.Checked = Properties.Settings.Default.OverrideURL;
            cbOverrideOWA.Checked = Properties.Settings.Default.OverrideOWAUrl;
            alwaysOpenOWAInIEToolStripMenuItem.Checked = Properties.Settings.Default.AlwaysIE;
            disableCalendarToolStripMenuItem.Checked = Properties.Settings.Default.DisableCalendar;
            loginAutomaticallyToolStripMenuItem.Checked = Properties.Settings.Default.AutoLogin;
            office365LoginOverrideToolStripMenuItem.Checked = Properties.Settings.Default.UseOffice365;
            overrideAutodiscoveryValidationToolStripMenuItem.Checked = Properties.Settings.Default.Autodiscovery;

            // Domain
            chkOnDomain.Checked = Properties.Settings.Default.NetworkCredentials;
            SelectDomainOptions();

            // Autodiscover?
            chkAutodiscovery.Checked = Properties.Settings.Default.Autodiscovery;
            SelectAutodiscoveryOptions();

            // Special lockdown option
            restoreToolStripMenuItem.Enabled = (Properties.Settings.Default.LockDown ? false : true);

            // Icon
            iconPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + @"\email.png";
            trayIcon = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + @"\email.ico";
            newIcon = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + @"\comment_rect.ico";

            // Sound file
            wavFile = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + @"\notify.wav";

            // Tray icon (default)
            notifyIcon1.Icon = new Icon(trayIcon);

            // Path to shell integration module
            shellPath = Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), Properties.Settings.Default.ShellIntegration);

            // A few flags
            lastPopTitle = "";
            lastPopMessage = "";
            popUrl = "";
            lastPopUrl = "";
            resetFlag = false;
            inboxCount = 0;

            // URLs
            UpdateURL();
			UpdateEWSField();
            UpdateOwaUrl();
            UpdateEmail();
			UpdateOffice365Field();

            // Growl
            this.growl = new GrowlConnector();
            growlApp = new Growl.Connector.Application(ThisApp);
            growlApp.Icon = iconPath;
            this.newMail = new NotificationType("NEWMAIL", "New Mail");
            this.growl.Register(growlApp, new NotificationType[] { newMail });

            // Snarl
            SnarlConnector.RegisterConfig(this.Handle, ThisApp, WindowsMessage.WM_MDIMAXIMIZE, iconPath);

            // Start Timers
            timerAppt.Interval = Properties.Settings.Default.ApptInterval * 1000;
            timerUpdate.Interval = Properties.Settings.Default.UpdateInterval * 1000;
            timerLogging.Enabled = true;

            // Now decide what to do based on whether this is the first run or not
            if (!Properties.Settings.Default.FirstTime)
            {
                // Set up Exchange
                if (ConfigureExchange())
                {
                    // Start main timer
                    timer1.Enabled = true;
                }
            }

            // Release interlock
            startingUp = false;
            AddLogEntry("Ready.", LogType.Info);
        }

        #endregion Constructors

        #region Enumerations

        /// <summary>
        /// Flags to control sound playback
        /// </summary>
        [System.Flags]
        public enum PlaySoundFlags : int
        {
            /// <summary>
            /// Sound attribute
            /// </summary>
            SND_SYNC = 0x0000,
            /// <summary>
            /// Sound attribute
            /// </summary>
            SND_ASYNC = 0x0001,
            /// <summary>
            /// Sound attribute
            /// </summary>
            SND_NODEFAULT = 0x0002,
            /// <summary>
            /// Sound attribute
            /// </summary>
            SND_LOOP = 0x0008,
            /// <summary>
            /// Sound attribute
            /// </summary>
            SND_NOSTOP = 0x0010,
            /// <summary>
            /// Sound attribute
            /// </summary>
            SND_NOWAIT = 0x00002000,
            /// <summary>
            /// Sound attribute
            /// </summary>
            SND_FILENAME = 0x00020000,
            /// <summary>
            /// Sound attribute
            /// </summary>
            SND_RESOURCE = 0x00040004
        }

        /// <summary>
        /// Severity of logging entry
        /// </summary>
        private enum LogType
        {
            Success, Fail, Info
        }

        #endregion Enumerations

        #region Delegates

        private delegate void FlushOutputDelegate();

        #endregion Delegates

        #region Methods

        /// <summary>
        /// Updates the EWS field.
        /// </summary>
        private void UpdateEWSField()
        {
            txtURLEdit.Enabled = Properties.Settings.Default.OverrideURL;
        }

        /// <summary>
        /// Selects the domain options.
        /// </summary>
        private void SelectDomainOptions()
        {
            txtDomain.Enabled = !chkOnDomain.Checked;
            txtPwd.Enabled = !chkOnDomain.Checked;
            txtUser.Enabled = !chkOnDomain.Checked;
        }

        /// <summary>
        /// Selects the autodiscovery options.
        /// </summary>
        private void SelectAutodiscoveryOptions()
        {
            txtServer.Enabled = !Properties.Settings.Default.Autodiscovery;
            cbOverrideEWS.Enabled = !Properties.Settings.Default.Autodiscovery;
            cbOverrideOWA.Enabled = !Properties.Settings.Default.Autodiscovery;
            txtURLEdit.Enabled = !Properties.Settings.Default.Autodiscovery;
            txtOWAEdit.Enabled = !Properties.Settings.Default.Autodiscovery;
            txtDomain.Enabled = !Properties.Settings.Default.Autodiscovery;
            overrideAutodiscoveryValidationToolStripMenuItem.Enabled = Properties.Settings.Default.Autodiscovery;
        }

        /// <summary>
        /// Updates the owa URL.
        /// </summary>
        private void UpdateOwaUrl()
        {
			txtOWAEdit.Enabled = cbOverrideOWA.Checked;

            if (Properties.Settings.Default.UseOffice365 && Office365Account.Length > 0)
            {
                lblOWAUrl.Text = Properties.Settings.Default.Office365OwaUrl.Replace(
                    Properties.Settings.Default.Office365AccountTemplate,
                    Office365Account);
            }
            else if (Properties.Settings.Default.Autodiscovery && reportedOwaUrl.Length > 0)
            {
                lblOWAUrl.Text = reportedOwaUrl;
            }
            else if (Properties.Settings.Default.OverrideOWAUrl && txtOWAEdit.Text.Length > 0)
            {
                lblOWAUrl.Text = txtOWAEdit.Text;
            }
            else if (txtServer.Text.Length > 0)
            {
                lblOWAUrl.Text = "https://" + txtServer.Text + "/owa/";
            }
            else
            {
                lblOWAUrl.Text = "unknown";
            }

            if (!startingUp)
            {
                // Update shell parameters
                ConfigureShell();
            }
        }

        /// <summary>
        /// Determines whether [is user administrator].
        /// </summary>
        /// <returns>
        /// 	<c>true</c> if [is user administrator]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsUserAdministrator()
        {
            //bool value to hold our return value
            bool isAdmin;
            try
            {
                //get the currently logged in user
                WindowsIdentity user = WindowsIdentity.GetCurrent();
                WindowsPrincipal principal = new WindowsPrincipal(user);
                isAdmin = principal.IsInRole(WindowsBuiltInRole.Administrator);
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
        /// Decrypts the string.
        /// </summary>
        /// <param name="encryptedData">The encrypted data.</param>
        /// <returns></returns>
        private static SecureString DecryptString(string encryptedData)
        {
            try
            {
                byte[] decryptedData = System.Security.Cryptography.ProtectedData.Unprotect(
                    Convert.FromBase64String(encryptedData),
                    entropy,
                    System.Security.Cryptography.DataProtectionScope.CurrentUser);
                return ToSecureString(System.Text.Encoding.Unicode.GetString(decryptedData));
            }
            catch
            {
                return new SecureString();
            }
        }

        /// <summary>
        /// Encrypts the string.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns></returns>
        private static string EncryptString(System.Security.SecureString input)
        {
            byte[] encryptedData = System.Security.Cryptography.ProtectedData.Protect(
                System.Text.Encoding.Unicode.GetBytes(ToInsecureString(input)),
                entropy,
                System.Security.Cryptography.DataProtectionScope.CurrentUser);
            return Convert.ToBase64String(encryptedData);
        }

        /// <summary>
        /// Retrieves the subdomain from the specified URL.
        /// </summary>
        /// <param name="domain">The domain.</param>
        /// <returns>
        /// The subdomain if it exist, otherwise null.
        /// </returns>
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

        [System.Runtime.InteropServices.DllImport("winmm.DLL", EntryPoint = "PlaySound", SetLastError = true)]
        private static extern bool PlaySound(string szSound, System.IntPtr hMod, PlaySoundFlags flags);

        /// <summary>
        /// Toes the insecure string.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns></returns>
        private static string ToInsecureString(SecureString input)
        {
            string returnValue = string.Empty;
            IntPtr ptr = System.Runtime.InteropServices.Marshal.SecureStringToBSTR(input);
            try
            {
                returnValue = System.Runtime.InteropServices.Marshal.PtrToStringBSTR(ptr);
            }
            finally
            {
                System.Runtime.InteropServices.Marshal.ZeroFreeBSTR(ptr);
            }
            return returnValue;
        }

        /// <summary>
        /// Toes the secure string.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns></returns>
        private static SecureString ToSecureString(string input)
        {
            SecureString secure = new SecureString();
            foreach (char c in input)
            {
                secure.AppendChar(c);
            }
            secure.MakeReadOnly();
            return secure;
        }

        /// <summary>
        /// Handles the Click event of the aboutToolStripMenuItem control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (frmAbout == null) frmAbout = new AboutBox1();
            frmAbout.ShowDialog();
        }

        /// <summary>
        /// Activates the OWA.
        /// </summary>
        private void activateOWA()
        {
            ProcessStartInfo RunSvc = new ProcessStartInfo(shellPath);
            RunSvc.WindowStyle = ProcessWindowStyle.Hidden;

            if (Properties.Settings.Default.AlwaysIE)
            {
                RunSvc.Arguments = "owa" + ((popUrl.Length > 0) ? " " + popUrl : "");
            }
            else
            {
                RunSvc.Arguments = "shell" + ((popUrl.Length > 0) ? " " + popUrl : "");
            }

            Process ServiceProcess = Process.Start(RunSvc);
        }

        /// <summary>
        /// Adds the log entry.
        /// </summary>
        /// <param name="newEntry">The new entry.</param>
        private void AddLogEntry(string newEntry)
        {
            AddLogEntry(newEntry, LogType.Success);
        }

        /// <summary>
        /// Adds the log entry.
        /// </summary>
        /// <param name="newEntry">The new entry.</param>
        /// <param name="whichLog">The which log.</param>
        private void AddLogEntry(string newEntry, LogType whichLog)
        {
            if (newEntry == lastLogEntry && whichLog == LogType.Fail)
            {
            }
            else
            {
                try
                {
                    lastLogEntry = newEntry;
                    lvBuffer.Add(new ListViewItem(DateTime.Now.ToString(), Convert.ToInt32(whichLog)));
                    lvBuffer[lvBuffer.Count - 1].SubItems.Add(newEntry);
                    myLog.AddEntry(newEntry);
                }
                catch (Exception)
                {
                    // Can't do anything for obvious reasons!
                }
            }
        }

        /// <summary>
        /// Handles the CheckStateChanged event of the alwaysOpenOWAInIEToolStripMenuItem control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void alwaysOpenOWAInIEToolStripMenuItem_CheckStateChanged(object sender, EventArgs e)
        {
            if (startingUp) return;

            Properties.Settings.Default.AlwaysIE = alwaysOpenOWAInIEToolStripMenuItem.Checked;
            Properties.Settings.Default.Save();
            AddLogEntry("Always use IE switched " + (Properties.Settings.Default.AlwaysIE ? "ON" : "OFF"), LogType.Info);

            ConfigureShell();
        }

        /// <summary>
        /// Handles the CheckStateChanged event of the balloonToolStripMenuItem control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void balloonToolStripMenuItem_CheckStateChanged(object sender, EventArgs e)
        {
            if (startingUp) return;

            Properties.Settings.Default.Balloon = balloonToolStripMenuItem.Checked;
            Properties.Settings.Default.Save();
            AddLogEntry("Balloon notifications switched " + (Properties.Settings.Default.Balloon ? "ON" : "OFF"), LogType.Info);
        }

        /// <summary>
        /// Certificates the validation call back.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="certificate">The certificate.</param>
        /// <param name="chain">The chain.</param>
        /// <param name="sslPolicyErrors">The SSL policy errors.</param>
        /// <returns>Is Certifcate Valid?</returns>
        private bool CertificateValidationCallBack(
            object sender,
            System.Security.Cryptography.X509Certificates.X509Certificate certificate,
            System.Security.Cryptography.X509Certificates.X509Chain chain,
            System.Net.Security.SslPolicyErrors sslPolicyErrors)
        {
            // If the override has been set then just return true
            if (Properties.Settings.Default.OverrideCert)
            {
                return true;
            }

            // If the certificate is a valid, signed certificate, return true.
            if (sslPolicyErrors == System.Net.Security.SslPolicyErrors.None)
            {
                return true;
            }

            // If there are errors in the certificate chain, look at each error to determine the cause.
            if ((sslPolicyErrors & System.Net.Security.SslPolicyErrors.RemoteCertificateChainErrors) != 0)
            {
                if (chain != null && chain.ChainStatus != null)
                {
                    foreach (System.Security.Cryptography.X509Certificates.X509ChainStatus status in chain.ChainStatus)
                    {
                        if ((certificate.Subject == certificate.Issuer) &&
                           (status.Status == System.Security.Cryptography.X509Certificates.X509ChainStatusFlags.UntrustedRoot))
                        {
                            // Self-signed certificates with an untrusted root are valid.
                            continue;
                        }
                        else
                        {
                            if (status.Status != System.Security.Cryptography.X509Certificates.X509ChainStatusFlags.NoError)
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
            else
            {
                // In all other cases, return false.
                return false;
            }
        }

        /// <summary>
        /// Handles the Click event of the changeLogToolStripMenuItem control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void changeLogToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (frmChangeLog == null) frmChangeLog = new ChangeLog(Properties.Settings.Default.RSSFeed);
            frmChangeLog.ShowDialog();
        }

        /// <summary>
        /// Checks for appointments.
        /// </summary>
        private void CheckForAppointments()
        {
            try
            {
                // Interrogate default Calendar
                CalendarView cView = new CalendarView(DateTime.Now, DateTime.Now.AddMinutes(Convert.ToDouble(Properties.Settings.Default.ApptWindow)));
                cView.PropertySet = PropertySet.FirstClassProperties;
                FindItemsResults<Appointment> findResults = myService.FindAppointments(WellKnownFolderName.Calendar, cView);

                // Process each item.
                int count = 0;
                bool allDone = false;
                foreach (Item myItem in findResults.Items)
                {
                    if (++count > Convert.ToInt32(Properties.Settings.Default.MaxNotify))
                    {
                        if (!allDone)
                        {
                            PopToast("Too many appointments!", "There are " + (findResults.Items.Count - Convert.ToInt32(Properties.Settings.Default.MaxNotify)) + " others");
                            allDone = true;
                        }
                    }
                    else
                    {
                        if (myItem is Appointment)
                        {
                            string myLocation = "<No Location>";
                            string mySubject = "<No Subject>";
                            string myStart = "unknown";
                            string myTime = "unknown";
                            int duration = 0;

                            Appointment myAppt = (Appointment)myItem;
                            PropertySet ps = new PropertySet(BasePropertySet.FirstClassProperties);
                            myAppt.Load(ps);
                            myLocation = myAppt.Location;
                            mySubject = (myAppt.Subject == null ? "<No Subject>" : myAppt.Subject);
                            TimeSpan span = myAppt.Start.Subtract(DateTime.Now);
                            duration = (int)Math.Floor(span.TotalMinutes);
                            myStart = duration.ToString();
                            myTime = myAppt.Start.ToString("HH:mm");

                            if (duration > 0)
                            {
                                popUrl = (reportedVersion == ExchangeVersion.Exchange2007_SP1 ? "" : myAppt.WebClientReadFormQueryString);
                                PopToast("You have an appointment in " + myStart + (duration != 1 ? " mins" : " min"), myTime + " - " + mySubject + " (" + myLocation + ")");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                AddLogEntry(ex.ToString(), LogType.Fail);
            }
        }

        /// <summary>
        /// Handles the CheckedChanged event of the chkOnDomain control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void chkOnDomain_CheckedChanged(object sender, EventArgs e)
        {
            if (startingUp) return;

            Properties.Settings.Default.NetworkCredentials = chkOnDomain.Checked;
            Properties.Settings.Default.Save();

            // Switch off some options when domain authentication selected
            SelectDomainOptions();
        }

        /// <summary>
        /// Handles the CheckedChanged event of the chkRunOnStartup control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void chkRunOnStartup_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                Link.Update(Environment.SpecialFolder.Startup, System.Windows.Forms.Application.ExecutablePath, ThisApp, chkRunOnStartup.Checked);
                AddLogEntry("OWAtray will" + (chkRunOnStartup.Checked ? " " : " not ") + "autostart with Windows", LogType.Info);
            }
            catch (Exception ex)
            {
                AddLogEntry(ex.Message, LogType.Fail);
            }
        }

        /// <summary>
        /// Handles the Click event of the cmdStart control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void cmdStart_Click(object sender, EventArgs e)
        {
            if (ConfigureExchange())
            {
                // Start
                startMonitoring();
            }
        }

        /// <summary>
        /// Handles the Click event of the cmdStop control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void cmdStop_Click(object sender, EventArgs e)
        {
            timerUpdate.Stop();
            timerAppt.Stop();
            AddLogEntry("Timer stopped", LogType.Info);
            notifyIcon1.Text = ThisApp + Environment.NewLine + "Not Connected to Exchange";
        }

        /// <summary>
        /// Handles the Click event of the cmdUnread control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void cmdUnread_Click(object sender, EventArgs e)
        {
            GetUnreadCount();
        }

        /// <summary>
        /// Configures Exchange.
        /// </summary>
        /// <returns>False if any errors</returns>
        private bool ConfigureExchange()
        {
            try
            {
                // Cursor
                this.Cursor = Cursors.WaitCursor;

                // Validate the server certificate
                ServicePointManager.ServerCertificateValidationCallback = CertificateValidationCallBack;

                AddLogEntry("Binding to Exchange...", LogType.Info);
                switch (Properties.Settings.Default.ExchangeVersion)
                {
                    case "Autodetect":
                        myService = new ExchangeService();
                        break;

                    case "Exchange2007_SP1":
                        myService = new ExchangeService(ExchangeVersion.Exchange2007_SP1);
                        break;

                    case "Exchange2010":
                        myService = new ExchangeService(ExchangeVersion.Exchange2010);
                        break;

                    case "Exchange2010_SP1":
                        myService = new ExchangeService(ExchangeVersion.Exchange2010_SP1);
                        break;
                }

                // Credentials
                if (chkOnDomain.Checked)
                {
                    myService.UseDefaultCredentials = true;
                }
                else
                {
                    if (txtUser.Text.Length == 0 || txtPwd.Text.Length == 0)
                    {
                        AddLogEntry("Please supply a valid username and password.", LogType.Fail);
                        return false;
                    }
                    if (txtDomain.Text.Length > 0)
                    {
                        myService.Credentials = new WebCredentials(txtUser.Text, txtPwd.Text, txtDomain.Text);
                    }
                    else
                    {
                        myService.Credentials = new WebCredentials(txtUser.Text, txtPwd.Text);
                    }
                }

                // If autodiscover is on then that overrides the URI
                if (Properties.Settings.Default.Autodiscovery)
                {
                    if (lblEmail.Text.Length == 0)
                    {
                        AddLogEntry("Autodiscovery requires an Email address to be specified. Please check settings.", LogType.Fail);
                        return false;
                    }
                    else
                    {
                        AddLogEntry("Starting Autodiscovery", LogType.Info);
                        if (Properties.Settings.Default.OverrideValidation)
                        {
                            myService.AutodiscoverUrl(lblEmail.Text, delegate(string url) { return true; });
                        }
                        else
                        {
                            myService.AutodiscoverUrl(lblEmail.Text);
                        }

                        // Update server settings
                        reportedVersion = myService.RequestedServerVersion;
                        AddLogEntry("Connected to " + reportedVersion.ToString(), LogType.Success);

                        // Probe for autodiscover information
                        AutodiscoverService autodiscoverService = new AutodiscoverService(myService.RequestedServerVersion);

                        // Credentials
                        if (chkOnDomain.Checked)
                        {
                            autodiscoverService.UseDefaultCredentials = true;
                        }
                        else
                        {
                            if (txtDomain.Text.Length > 0)
                            {
                                autodiscoverService.Credentials = new WebCredentials(txtUser.Text, txtPwd.Text, txtDomain.Text);
                            }
                            else
                            {
                                autodiscoverService.Credentials = new WebCredentials(txtUser.Text, txtPwd.Text);
                            }
                        }

                        // Redirection Callback
                        if (Properties.Settings.Default.OverrideValidation)
                        {
                            autodiscoverService.RedirectionUrlValidationCallback = delegate(string url) { return true; };
                        }

                        // Is this Internal or External ?
                        if (autodiscoverService.IsExternal == false)
                        {
                            // Internal
                            AddLogEntry("Endpoint is INSIDE corporate network", LogType.Info);

                            // Probe for values
                            GetUserSettingsResponse userresponse = autodiscoverService.GetUserSettings(lblEmail.Text,
                                UserSettingName.InternalWebClientUrls,
                                UserSettingName.InternalEwsUrl,
                                UserSettingName.InternalMailboxServer,
                                UserSettingName.UserDisplayName);

                            // OWA Url
                            WebClientUrlCollection col = (WebClientUrlCollection)userresponse.Settings[UserSettingName.InternalWebClientUrls];
                            WebClientUrl owaUrl = col.Urls[0];
                            reportedOwaUrl = owaUrl.Url;
                            UpdateOwaUrl();
                            AddLogEntry("Autodiscovered OWA Url: " + reportedOwaUrl, LogType.Success);

                            // EWS Url
                            reportedEwsUrl = (string)userresponse.Settings[UserSettingName.InternalEwsUrl];
                            UpdateURL();
                            AddLogEntry("Autodiscovered EWS Url: " + reportedEwsUrl, LogType.Success);

                            // Mailbox
                            reportedMailboxServer = (string)userresponse.Settings[UserSettingName.InternalMailboxServer];
                            AddLogEntry("Autodiscovered Mailbox Server: " + reportedMailboxServer, LogType.Success);

                            // User Name
                            reportedUserName = (string)userresponse.Settings[UserSettingName.UserDisplayName];
                            AddLogEntry("Autodiscovered User Name: " + reportedUserName, LogType.Success);
                        }
                        else
                        {
                            // External (default)
                            AddLogEntry("Endpoint is OUTSIDE corporate network", LogType.Info);

                            // Probe for values
                            GetUserSettingsResponse userresponse = autodiscoverService.GetUserSettings(lblEmail.Text,
                                UserSettingName.ExternalWebClientUrls,
                                UserSettingName.ExternalEwsUrl,
                                UserSettingName.ExternalMailboxServer,
                                UserSettingName.UserDisplayName);

                            // OWA Url
                            WebClientUrlCollection owaCollection = (WebClientUrlCollection)userresponse.Settings[UserSettingName.ExternalWebClientUrls];
                            WebClientUrl owaUrl = owaCollection.Urls[0];
                            reportedOwaUrl = owaUrl.Url;
                            UpdateOwaUrl();
                            AddLogEntry("Autodiscovered OWA Url: " + reportedOwaUrl, LogType.Success);

                            // EWS Url
                            reportedEwsUrl = (string)userresponse.Settings[UserSettingName.ExternalEwsUrl];
                            UpdateURL();
                            AddLogEntry("Autodiscovered EWS Url: " + reportedEwsUrl, LogType.Success);

                            // Mailbox
                            reportedMailboxServer = (string)userresponse.Settings[UserSettingName.ExternalMailboxServer];
                            AddLogEntry("Autodiscovered Mailbox Server: " + reportedMailboxServer, LogType.Success);

                            // User Name
                            reportedUserName = (string)userresponse.Settings[UserSettingName.UserDisplayName];
                            AddLogEntry("Autodiscovered User Name: " + reportedUserName, LogType.Success);
                        }
                    }
                }
                else
                {
                    if (lblUrl.Text.Length == 0)
                    {
                        AddLogEntry("Can't establish a valid URL for Exchange. Please check settings.", LogType.Fail);
                    }
                    else
                    {
                        Uri myUri = new Uri(lblUrl.Text);
                        myService.Url = myUri;

                        // Update server settings
                        reportedVersion = myService.RequestedServerVersion;
                        AddLogEntry("Connected to " + reportedVersion.ToString(), LogType.Success);

                        // Update properties
                        reportedMailboxServer = txtServer.Text;
                        reportedUserName = (chkOnDomain.Checked ? "" : txtUser.Text);
                    }
                }

                // Set a flag to indicate that subsequent runs can autostart
                Properties.Settings.Default.FirstTime = false;
                Properties.Settings.Default.Save();

                // All clear
                return true;
            }
            catch (Exception ex)
            {
                AddLogEntry("Error: " + ex.Message, LogType.Fail);
                return false;
            }
            finally
            {
                // Cursor
                this.Cursor = Cursors.Default;
            }
        }

        /// <summary>
        /// Configures the shell.
        /// </summary>
        private void ConfigureShell()
        {
            //AddLogEntry("Configuring Shell Integration", LogType.Info);

            // Set OWA Url
            string owaUrl = lblOWAUrl.Text;
            //AddLogEntry("Setting OWA url to " + owaUrl, LogType.Info);

            try
            {
                ProcessStartInfo RunSvc = new ProcessStartInfo(shellPath);
                RunSvc.Arguments = "url " + owaUrl;
                RunSvc.WindowStyle = ProcessWindowStyle.Hidden;
                Process ServiceProcess = Process.Start(RunSvc);

                while (!(ServiceProcess.HasExited == true))
                {
                    System.Threading.Thread.Sleep(100);
                    System.Windows.Forms.Application.DoEvents();
                }
            }
            catch (Exception ex)
            {
                AddLogEntry("Error - " + ex.Message, LogType.Fail);
                return;
            }

            // Set account name
            string userAccount = GetEmailAddress();
            //AddLogEntry("Using user account: " + userAccount, LogType.Info);

            try
            {
                ProcessStartInfo RunSvc = new ProcessStartInfo(shellPath);
                RunSvc.Arguments = "account " + userAccount;
                RunSvc.WindowStyle = ProcessWindowStyle.Hidden;
                Process ServiceProcess = Process.Start(RunSvc);

                while (!(ServiceProcess.HasExited == true))
                {
                    System.Threading.Thread.Sleep(100);
                    System.Windows.Forms.Application.DoEvents();
                }
            }
            catch (Exception ex)
            {
                AddLogEntry("Error - " + ex.Message, LogType.Fail);
                return;
            }

            // Set Exchange Version
            //AddLogEntry("Configuring for Exchange " + reportedVersion.ToString(), LogType.Info);

            try
            {
                ProcessStartInfo RunSvc = new ProcessStartInfo(shellPath);
                RunSvc.Arguments = "exchange " + reportedVersion.ToString();
                RunSvc.WindowStyle = ProcessWindowStyle.Hidden;
                Process ServiceProcess = Process.Start(RunSvc);

                while (!(ServiceProcess.HasExited == true))
                {
                    System.Threading.Thread.Sleep(100);
                    System.Windows.Forms.Application.DoEvents();
                }
            }
            catch (Exception ex)
            {
                AddLogEntry("Error - " + ex.Message, LogType.Fail);
                return;
            }

            // Set Password
            try
            {
                ProcessStartInfo RunSvc = new ProcessStartInfo(shellPath);
                RunSvc.Arguments = "password " + txtPwd.Text;
                RunSvc.WindowStyle = ProcessWindowStyle.Hidden;
                Process ServiceProcess = Process.Start(RunSvc);

                while (!(ServiceProcess.HasExited == true))
                {
                    System.Threading.Thread.Sleep(100);
                    System.Windows.Forms.Application.DoEvents();
                }
            }
            catch (Exception ex)
            {
                AddLogEntry("Error - " + ex.Message, LogType.Fail);
                return;
            }

            // Set Autologin
            try
            {
                ProcessStartInfo RunSvc = new ProcessStartInfo(shellPath);
                RunSvc.Arguments = "autologin " + (Properties.Settings.Default.AutoLogin ? "Yes" : "No");
                RunSvc.WindowStyle = ProcessWindowStyle.Hidden;
                Process ServiceProcess = Process.Start(RunSvc);

                while (!(ServiceProcess.HasExited == true))
                {
                    System.Threading.Thread.Sleep(100);
                    System.Windows.Forms.Application.DoEvents();
                }
            }
            catch (Exception ex)
            {
                AddLogEntry("Error - " + ex.Message, LogType.Fail);
                return;
            }

            // Set Browser
            try
            {
                ProcessStartInfo RunSvc = new ProcessStartInfo(shellPath);
                RunSvc.Arguments = "browser " + (Properties.Settings.Default.AlwaysIE ? "Yes" : "No");
                RunSvc.WindowStyle = ProcessWindowStyle.Hidden;
                Process ServiceProcess = Process.Start(RunSvc);

                while (!(ServiceProcess.HasExited == true))
                {
                    System.Threading.Thread.Sleep(100);
                    System.Windows.Forms.Application.DoEvents();
                }
            }
            catch (Exception ex)
            {
                AddLogEntry("Error - " + ex.Message, LogType.Fail);
                return;
            }
        }

        /// <summary>
        /// Handles the CheckStateChanged event of the disableCalendarToolStripMenuItem control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void disableCalendarToolStripMenuItem_CheckStateChanged(object sender, EventArgs e)
        {
            if (startingUp) return;

            Properties.Settings.Default.DisableCalendar = disableCalendarToolStripMenuItem.Checked;
            Properties.Settings.Default.Save();
            AddLogEntry("Calendar notifications switched " + (Properties.Settings.Default.DisableCalendar ? "OFF" : "ON"), LogType.Info);
        }

        /// <summary>
        /// Handles the SelectedIndexChanged event of the exchange2007ToolStripMenuItem control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void exchange2007ToolStripMenuItem_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (startingUp) return;

            switch (exchange2007ToolStripMenuItem.SelectedIndex)
            {
                case 0:
                    Properties.Settings.Default.ExchangeVersion = "Autodetect";
                    break;

                case 1:
                    Properties.Settings.Default.ExchangeVersion = "Exchange2007_SP1";
                    break;

                case 2:
                    Properties.Settings.Default.ExchangeVersion = "Exchange2010";
                    break;

                case 3:
                    Properties.Settings.Default.ExchangeVersion = "Exchange2010_SP1";
                    break;
            }

            Properties.Settings.Default.Save();
        }

        /// <summary>
        /// Handles the Click event of the exitToolStripMenuItem1 control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void exitToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            overRideClose = true;
            this.Close();
        }

        /// <summary>
        /// Handles the Click event of the exitToolStripMenuItem control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            overRideClose = true;
            this.Close();
        }

        /// <summary>
        /// Flushes the output.
        /// </summary>
        private void FlushOutput()
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new FlushOutputDelegate(FlushOutput), new object[] { });
                return;
            }
            else
            {
                if (lvBuffer.Count > 0)
                {
                    if (lvStatus.Items.Count >= Convert.ToInt32(ScreenLines))
                    {
                        lvStatus.Items.Clear();
                    }

                    try
                    {
                        // Pause output
                        lvStatus.BeginUpdate();

                        // Add new records - use Add rather than AddRange to avoid bug in .Net that causes NullReferenceException
                        foreach (ListViewItem lv in lvBuffer)
                        {
                            if (lv != null)
                            {
                                lvStatus.Items.Add(lv);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        // This won't appear on screen but will go to the log file
                        AddLogEntry("Logging error 1 - " + ex.Message, LogType.Fail);
                    }
                    finally
                    {
                        if (lvStatus.Items.Count > 0)
                        {
                            try
                            {
                                // Make the latest addition visible
                                lvStatus.EnsureVisible(lvStatus.Items.Count - 1);

                                // Update status strip
                                ListViewItem lv = lvStatus.Items[lvStatus.Items.Count - 1];
                                slStatus.Text = lv.SubItems[1].Text.Substring(0, (lv.SubItems[1].Text.Length > 120 ? 120 : lv.SubItems[1].Text.Length));
                            }
                            catch (Exception ex)
                            {
                                // This won't appear on screen but will go to the log file
                                AddLogEntry("Logging error 2 - " + ex.Message, LogType.Fail);
                            }
                        }
                        // Clear down buffer
                        lvBuffer.Clear();
                        // Resume output
                        lvStatus.EndUpdate();
                        // Repaint control
                        lvStatus.Refresh();
                        this.Refresh();
                    }
                }
            }
        }

        /// <summary>
        /// Handles the FormClosed event of the Form1 control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.FormClosedEventArgs"/> instance containing the event data.</param>
        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            this.FormClosed -= new FormClosedEventHandler(Form1_FormClosed);
            AddLogEntry("Terminating");
            myLog.Active = false;
        }

        /// <summary>
        /// Handles the FormClosing event of the Form1 control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.FormClosingEventArgs"/> instance containing the event data.</param>
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            //There are several ways to close an application.
            //We are trying to find the click of the X in the upper right hand corner
            //We will only allow the closing of this app if it is minimized.
            if (this.WindowState != FormWindowState.Minimized && overRideClose == false)
            {
                //we don't close the app...
                e.Cancel = true;
                this.WindowState = FormWindowState.Minimized;
            }
            else
            {
                this.FormClosing -= new FormClosingEventHandler(Form1_FormClosing);
                M_RESULT result = SnarlConnector.RevokeConfig(this.Handle);
                System.Windows.Forms.Application.Exit(e);
            }
        }

        /// <summary>
        /// Handles the Move event of the Form1 control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void Form1_Move(object sender, EventArgs e)
        {
            if (this == null)
            {
                return;
            }

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
        /// Times the of newest email.
        /// </summary>
        /// <returns></returns>
        private DateTime TimeOfNewestEmail()
        {
            DateTime myTime = DateTime.Now;

            // Define filters collection
            SearchFilter.SearchFilterCollection filters = new SearchFilter.SearchFilterCollection(LogicalOperator.And);
            filters.Add(new SearchFilter.IsEqualTo(EmailMessageSchema.IsRead, false));

            // Item view
            ItemView view = new ItemView(10, 0, OffsetBasePoint.Beginning);
            view.PropertySet = new PropertySet(BasePropertySet.IdOnly);
            view.PropertySet.Add(ItemSchema.DateTimeReceived);
            view.OrderBy.Add(ItemSchema.DateTimeReceived, SortDirection.Descending);

            try
            {
                // Now search
                FindItemsResults<Item> findResults = myService.FindItems(WellKnownFolderName.Inbox, filters, view);

                // Process each item.
                foreach (Item myItem in findResults.Items)
                {
                    if (myItem is EmailMessage)
                    {
                        EmailMessage myEmail = (EmailMessage)myItem;
                        PropertySet ps = new PropertySet(BasePropertySet.FirstClassProperties);
                        myEmail.Load(ps);
                        myTime = myEmail.DateTimeReceived;
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                AddLogEntry("Error when getting email properties - " + ex.Message, LogType.Fail);
            }

            //AddLogEntry("Newest email is: " + myTime.ToString());
            return myTime;
        }

        /// <summary>
        /// Gets the unread count from the Inbox
        /// </summary>
        /// <returns>Email count</returns>
        private int GetUnreadCount()
        {
            int myCount;

            if (myService == null)
            {
                AddLogEntry("Not connnected to Exchange", LogType.Fail);
                notifyIcon1.Text = ThisApp + Environment.NewLine + "Not Connected to Exchange";
                return 0;
            }

            // Set time for initial run only
            if (firstRun) TimeLastChecked = TimeOfNewestEmail().AddSeconds(1);

            try
            {
                // Is there new mail?
                Folder myFolder = Folder.Bind(myService, WellKnownFolderName.Inbox);
                myCount = myFolder.UnreadCount;
                if (myCount > inboxCount)
                {
                    if (firstRun)
                    {
                        PopToast("New Mail", "You have " + myCount + " unread email" + (myCount != 1 ? "s " : " ") + "in your inbox");
                    }
                    else
                    {
                        int count = PopUnreadEmail(myCount);
                    }

                    resetFlag = false;
                }

                if (!resetFlag)
                {
                    notifyIcon1.Icon = new Icon((myCount > 0 ? newIcon : trayIcon));
                }
                string text1 = ThisApp + Environment.NewLine + Environment.NewLine + myCount + " unread email" + (myCount != 1 ? "s " : " ");
                const int MaxTipLength = 63;
                int charsLeft = MaxTipLength - text1.Length;
                string domainText = reportedMailboxServer + @"\" + reportedUserName;
                if (domainText.Length > charsLeft) domainText = domainText.Substring(0, charsLeft);
                string finalText = ThisApp + Environment.NewLine + domainText + Environment.NewLine + myCount + " unread email" + (myCount != 1 ? "s " : " ");
                notifyIcon1.Text = finalText;
                inboxCount = myCount;
            }
            catch (Exception ex)
            {
                AddLogEntry("Error: " + ex.Message, LogType.Fail);
                myCount = 0;
            }
            finally
            {
                firstRun = false;
            }

            return myCount;
        }

        /// <summary>
        /// Handles the CheckStateChanged event of the growlToolStripMenuItem control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void growlToolStripMenuItem_CheckStateChanged(object sender, EventArgs e)
        {
            if (startingUp) return;

            Properties.Settings.Default.Growl = growlToolStripMenuItem.Checked;
            Properties.Settings.Default.Save();
            AddLogEntry("Growl notifications switched " + (Properties.Settings.Default.Growl ? "ON" : "OFF"), LogType.Info);
        }

        /// <summary>
        /// Inits the event view.
        /// </summary>
        /// <param name="lvX">The lv X.</param>
        private void InitEventView(ListView lvX)
        {
            lvX.Columns.Add("Time", 140, HorizontalAlignment.Left);
            lvX.Columns.Add("Event Details", 1000, HorizontalAlignment.Left);
            lvX.Items.Clear();
        }

        /// <summary>
        /// Handles the Click event of the makeOWADefaultToolStripMenuItem control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void makeOWADefaultToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Update shell parameters
            ConfigureShell();

            if (!IsUserAdministrator())
            {
                AddLogEntry("You are not an Admin user. Operation may fail.", LogType.Fail);
            }

            // Configure registry
            AddLogEntry("Setting up Mail handlers", LogType.Info);

            try
            {
                ProcessStartInfo RunSvc = new ProcessStartInfo(shellPath);
                RunSvc.Arguments = "registry";
                RunSvc.WindowStyle = ProcessWindowStyle.Hidden;
                if (System.Environment.OSVersion.Version.Major >= 6)
                    RunSvc.Verb = "runas";
                Process ServiceProcess = Process.Start(RunSvc);

                while (!(ServiceProcess.HasExited == true))
                {
                    System.Threading.Thread.Sleep(100);
                    System.Windows.Forms.Application.DoEvents();
                }
            }
            catch (Exception ex)
            {
                AddLogEntry("Error - " + ex.Message, LogType.Fail);
                return;
            }

            AddLogEntry("Mail functions will now be handled by OWA", LogType.Success);
        }

        /// <summary>
        /// Handles the Click event of the mDACVersionsToolStripMenuItem control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void mDACVersionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (frmMDAC == null) frmMDAC = new MDACversions();
            frmMDAC.ShowDialog();
        }

        /// <summary>
        /// Handles the Click event of the nETVersionsToolStripMenuItem control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void nETVersionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (frmNET == null) frmNET = new NETversions();
            frmNET.ShowDialog();
        }

        /// <summary>
        /// Handles the BalloonTipClicked event of the notifyIcon1 control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void notifyIcon1_BalloonTipClicked(object sender, EventArgs e)
        {
            if (Control.MouseButtons == MouseButtons.Left)
            {
                //AddLogEntry("PopURL - " + notifyIcon1.Tag);
                activateOWA();
                popUrl = "";
            }
        }

        /// <summary>
        /// Handles the MouseDoubleClick event of the notifyIcon1 control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.MouseEventArgs"/> instance containing the event data.</param>
        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            //if (this.WindowState == FormWindowState.Minimized)
            //{
            //    this.Show();
            //    this.WindowState = FormWindowState.Normal;
            //}

            //// Activate the form.
            //this.Activate();
            //this.Focus();

            popUrl = "";
            activateOWA();
        }

        /// <summary>
        /// Handles the Click event of the openOutlookToolStripMenuItem control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void openOutlookToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start(Properties.Settings.Default.OutlookPath);
            }
            catch(Exception ex)
            {
                AddLogEntry(ex.Message, LogType.Fail);
            }
        }

        /// <summary>
        /// Handles the Click event of the openOWAToolStripMenuItem control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void openOWAToolStripMenuItem_Click(object sender, EventArgs e)
        {
            popUrl = "";
            activateOWA();
        }

        /// <summary>
        /// Inits the logger.
        /// </summary>
        private static void InitLogger()
        {
            myLog = new FlatFile();
            myLog.LogFile = Path.Combine(System.Windows.Forms.Application.LocalUserAppDataPath, "owatray.log");
            myLog.DateOn = true;
            myLog.Verbose = true;
            myLog.LimitSize = true;
            myLog.Scavenge = true;
            myLog.Active = true;
        }

        /// <summary>
        /// Handles the CheckStateChanged event of the overrideToolStripMenuItem control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void overrideToolStripMenuItem_CheckStateChanged(object sender, EventArgs e)
        {
            if (startingUp) return;

            Properties.Settings.Default.OverrideCert = overrideToolStripMenuItem.Checked;
            Properties.Settings.Default.Save();
            AddLogEntry("SSL Certificate override switched " + (Properties.Settings.Default.OverrideCert ? "ON" : "OFF"), LogType.Info);
        }

        /// <summary>
        /// Handles the CheckStateChanged event of the playSoundToolStripMenuItem control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void playSoundToolStripMenuItem_CheckStateChanged(object sender, EventArgs e)
        {
            if (startingUp) return;

            Properties.Settings.Default.Bell = playSoundToolStripMenuItem.Checked;
            Properties.Settings.Default.Save();
            AddLogEntry("Audible notifications switched " + (Properties.Settings.Default.Bell ? "ON" : "OFF"), LogType.Info);
        }

        /// <summary>
        /// Pops the toast.
        /// </summary>
        /// <param name="myTitle">My title.</param>
        /// <param name="myMessage">My message.</param>
        private void PopToast(string myTitle, string myMessage)
        {
            // Belt & Braces
            if (myTitle.Length == 0) myTitle = "<No Title>";
            if (myMessage.Length == 0) myMessage = "<No Subject>";

            AddLogEntry(myTitle, LogType.Info);

            // Store for recall
            lastPopTitle = myTitle;
            lastPopMessage = myMessage;
            lastPopUrl = popUrl;

            //Balloon
            if (Properties.Settings.Default.Balloon)
            {
                notifyIcon1.Tag = popUrl;
                notifyIcon1.ShowBalloonTip(5000, myTitle, myMessage, ToolTipIcon.Info);
            }

            // Growl
            if (Properties.Settings.Default.Growl)
            {
                Notification notification = new Notification(this.growlApp.Name, this.newMail.Name, "", myTitle, myMessage);
                this.growl.Notify(notification);
            }

            // Snarl
            if (Properties.Settings.Default.Snarl)
            {
                SnarlConnector.ShowMessage(myTitle, myMessage, 10, iconPath, this.Handle, (WindowsMessage)REPLY_MSG);
            }

            // Audible
            if (Properties.Settings.Default.Bell)
            {
                PlaySound(wavFile, new System.IntPtr(), PlaySoundFlags.SND_SYNC);
            }
        }

        /// <summary>
        /// Pops the unread email.
        /// </summary>
        /// <param name="unreadCount">The unread count.</param>
        /// <returns></returns>
        private int PopUnreadEmail(int unreadCount)
        {
            //AddLogEntry("Checking for mail after: " + TimeLastChecked.ToString());

            // Set the offset for the paged search.
            int offset = 0;
            int count = 0;

            // Set the page size.
            int pageSize = Properties.Settings.Default.PageSize;

            // Set the flag that indicates whether to continue iterating through additional pages.
            bool MoreItems = true;

            // Continue paging while there are more items to page.
            while (MoreItems)
            {
                // Define filters collection
                SearchFilter.SearchFilterCollection filters = new SearchFilter.SearchFilterCollection(LogicalOperator.And);
                filters.Add(new SearchFilter.IsEqualTo(EmailMessageSchema.IsRead, false));
                filters.Add(new SearchFilter.IsGreaterThan(EmailMessageSchema.DateTimeReceived, TimeLastChecked));

                // Item view
                ItemView view = new ItemView(pageSize, offset, OffsetBasePoint.Beginning);
                view.PropertySet = new PropertySet(BasePropertySet.IdOnly);
                view.PropertySet.Add(ItemSchema.Subject);
                view.PropertySet.Add(ItemSchema.DateTimeReceived);
                view.OrderBy.Add(ItemSchema.DateTimeReceived, SortDirection.Descending);

                // Now search
                FindItemsResults<Item> findResults = myService.FindItems(WellKnownFolderName.Inbox, filters, view);

                // Process each item.
                bool allDone = false;
                bool isFlagged = false;
                foreach (Item myItem in findResults.Items)
                {
                    if (++count > Convert.ToInt32(Properties.Settings.Default.MaxNotify))
                    {
                        if (!allDone)
                        {
                            PopToast("Too much mail!", "There are " + (unreadCount - Convert.ToInt32(Properties.Settings.Default.MaxNotify)) + " other new emails");
                            allDone = true;
                        }
                    }
                    else
                    {
                        if (myItem is EmailMessage)
                        {
                            string mySender = "<No Sender>";
                            string mySubject = "<No Subject>";
                            DateTime myTime = DateTime.Now;

                            try
                            {
                                EmailMessage myEmail = (EmailMessage)myItem;
                                PropertySet ps = new PropertySet(BasePropertySet.FirstClassProperties);
                                myEmail.Load(ps);
                                mySender = myEmail.Sender.Name;
                                mySubject = (myEmail.Subject == null ? "<No Subject>" : myEmail.Subject);
                                myTime = myEmail.DateTimeReceived;
                                popUrl = (reportedVersion == ExchangeVersion.Exchange2007_SP1 ? "" : myEmail.WebClientReadFormQueryString);
                                PopToast("New Mail from " + mySender, mySubject);
                            }
                            catch (Exception ex)
                            {
                                AddLogEntry("Error when getting email properties - " + ex.Message, LogType.Fail);
                            }

                            // Update flag
                            if (!isFlagged)
                            {
                                TimeLastChecked = myTime.AddSeconds(1);
                                isFlagged = true;
                            }
                        }
                    }
                }

                // Set the flag to discontinue paging.
                if (!findResults.MoreAvailable)
                    MoreItems = false;

                // Update the offset if there are more items to page.
                if (MoreItems)
                    offset = offset + pageSize;
            }

            return count;
        }

        /// <summary>
        /// Handles the Click event of the recallLastPopupToolStripMenuItem control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void recallLastPopupToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (lastPopMessage.Length > 0 && lastPopTitle.Length > 0)
            {
                popUrl = lastPopUrl;
                PopToast(lastPopTitle, lastPopMessage);
            }
        }

        /// <summary>
        /// Handles the Click event of the resetTrayIconToolStripMenuItem control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void resetTrayIconToolStripMenuItem_Click(object sender, EventArgs e)
        {
            notifyIcon1.Icon = new Icon(trayIcon);
            resetFlag = true;
        }

        /// <summary>
        /// Handles the Click event of the restoreToolStripMenuItem control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void restoreToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                this.Show();
                this.WindowState = FormWindowState.Normal;
            }

            // Activate the form.
            this.Activate();
            this.Focus();
        }

        /// <summary>
        /// Handles the CheckStateChanged event of the snarlToolStripMenuItem control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void snarlToolStripMenuItem_CheckStateChanged(object sender, EventArgs e)
        {
            if (startingUp) return;

            Properties.Settings.Default.Snarl = snarlToolStripMenuItem.Checked;
            Properties.Settings.Default.Save();
            AddLogEntry("Snarl notifications switched " + (Properties.Settings.Default.Snarl ? "ON" : "OFF"), LogType.Info);
        }

        /// <summary>
        /// Starts the monitoring.
        /// </summary>
        private void startMonitoring()
        {
            // Start Timer
            timerAppt.Start();
            timerUpdate.Interval = Properties.Settings.Default.UpdateInterval * 1000;
            timerUpdate.Start();
            AddLogEntry(txtInterval.Text + " second timer started", LogType.Info);

            // Minimise to tray
            this.WindowState = FormWindowState.Minimized;

            // Configure Shell
            ConfigureShell();

            // Initial Check
            GetUnreadCount();
            if (!Properties.Settings.Default.DisableCalendar)
            {
                CheckForAppointments();
            }
        }

        /// <summary>
        /// Handles the Click event of the supportToolStripMenuItem control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void supportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (frmContact == null) frmContact = new ContactUs();
            frmContact.ShowDialog();
        }

        /// <summary>
        /// Handles the Click event of the switchOffToolStripMenuItem control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void switchOffToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!IsUserAdministrator())
            {
                AddLogEntry("You are not an Admin user. Operation may fail.", LogType.Fail);
            }

            // Configure registry
            AddLogEntry("Restoring Mail handlers", LogType.Info);

            try
            {
                ProcessStartInfo RunSvc = new ProcessStartInfo(shellPath);
                RunSvc.Arguments = "restore";
                RunSvc.WindowStyle = ProcessWindowStyle.Hidden;
                if (System.Environment.OSVersion.Version.Major >= 6)
                    RunSvc.Verb = "runas";
                Process ServiceProcess = Process.Start(RunSvc);

                while (!(ServiceProcess.HasExited == true))
                {
                    System.Threading.Thread.Sleep(100);
                    System.Windows.Forms.Application.DoEvents();
                }
            }
            catch (Exception ex)
            {
                AddLogEntry("Error - " + ex.Message, LogType.Fail);
                return;
            }

            AddLogEntry("Mail handler restored to system default", LogType.Success);
        }

        /// <summary>
        /// Handles the Click event of the systemInformationToolStripMenuItem control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void systemInformationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (frmInfo == null) frmInfo = new SysInfo();
            frmInfo.ShowDialog();
        }

        /// <summary>
        /// Handles the Tick event of the timer1 control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Enabled = false;
            startMonitoring();
        }

        /// <summary>
        /// Handles the Tick event of the timerAppt control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void timerAppt_Tick(object sender, EventArgs e)
        {
            // Check for appointments
            if (!Properties.Settings.Default.DisableCalendar)
            {
                CheckForAppointments();
            }
        }

        /// <summary>
        /// Handles the Tick event of the timerLogging control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void timerLogging_Tick(object sender, EventArgs e)
        {
            // Update logging view
            FlushOutput();
        }

        /// <summary>
        /// Handles the Tick event of the timerUpdate control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void timerUpdate_Tick(object sender, EventArgs e)
        {
            // Look for new email
            GetUnreadCount();
        }

        /// <summary>
        /// Handles the Validated event of the txtInterval control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void txtInterval_Validated(object sender, EventArgs e)
        {
            Properties.Settings.Default.UpdateInterval = Convert.ToInt32(txtInterval.Text);
            Properties.Settings.Default.Save();
        }

        /// <summary>
        /// Handles the Validating event of the txtInterval control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.ComponentModel.CancelEventArgs"/> instance containing the event data.</param>
        private void txtInterval_Validating(object sender, System.ComponentModel.CancelEventArgs e)
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
                    errorProvider1.SetError(txtInterval, "Must be a numeric value between 1 and " + MaxInterval.ToString());
                    e.Cancel = true;
                }
            }
            else
            {
                errorProvider1.SetError(txtInterval, "Must be a numeric value between 1 and " + MaxInterval.ToString());
                e.Cancel = true;
            }
        }

        /// <summary>
        /// Updates the URL.
        /// </summary>
        private void UpdateURL()
        {
            if (Properties.Settings.Default.Autodiscovery && reportedEwsUrl.Length > 0)
            {
                lblUrl.Text = reportedEwsUrl;
            }
            else if (Properties.Settings.Default.OverrideURL && txtURLEdit.Text.Length > 0)
            {
                lblUrl.Text = txtURLEdit.Text;
            }
            else if (txtServer.Text.Length > 0)
            {
                lblUrl.Text = "https://" + txtServer.Text + "/ews/exchange.asmx";
            }
            else
            {
                lblUrl.Text = "unknown";
            }
        }

        /// <summary>
        /// Gets the email address.
        /// </summary>
        /// <returns></returns>
        private string GetEmailAddress()
        {
            string userAccount = (txtEmail.Text.Length > 0) ? txtEmail.Text : txtUser.Text;
            if (userAccount.Length > 0 && !userAccount.Contains("@"))
            {
                userAccount = userAccount + "@" + GetSubDomain(txtServer.Text);
            }

            return userAccount;
        }

        /// <summary>
        /// Updates the email.
        /// </summary>
        private void UpdateEmail()
        {
            lblEmail.Text = GetEmailAddress();

            if (!startingUp)
            {
                ConfigureShell();
            }
        }

        #endregion Methods

        /// <summary>
        /// Handles the CheckStateChanged event of the loginAutomaticallyToolStripMenuItem control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void loginAutomaticallyToolStripMenuItem_CheckStateChanged(object sender, EventArgs e)
        {
            if (startingUp) return;

            Properties.Settings.Default.AutoLogin = loginAutomaticallyToolStripMenuItem.Checked;
            Properties.Settings.Default.Save();
            AddLogEntry("Automatic Login is switched " + (Properties.Settings.Default.AutoLogin ? "ON" : "OFF"), LogType.Info);

            ConfigureShell();
        }

        /// <summary>
        /// Handles the Validated event of the txtDomain control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void txtDomain_Validated(object sender, EventArgs e)
        {
            Properties.Settings.Default.Domain = txtDomain.Text;
            Properties.Settings.Default.Save();
        }

        /// <summary>
        /// Handles the Validated event of the txtPwd control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void txtPwd_Validated(object sender, EventArgs e)
        {
            Properties.Settings.Default.Password = (txtPwd.Text.Length > 0 ? EncryptString(ToSecureString(txtPwd.Text)) : "");
            Properties.Settings.Default.Save();
        }

        /// <summary>
        /// Handles the Validated event of the txtServer control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void txtServer_Validated(object sender, EventArgs e)
        {
            Properties.Settings.Default.Server = txtServer.Text;
            Properties.Settings.Default.Save();
            UpdateURL();
            UpdateOwaUrl();
        }

        /// <summary>
        /// Handles the Validated event of the txtUser control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void txtUser_Validated(object sender, EventArgs e)
        {
            Properties.Settings.Default.Username = txtUser.Text;
            Properties.Settings.Default.Save();
            UpdateEmail();
        }

        /// <summary>
        /// Handles the Validated event of the txtEmail control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void txtEmail_Validated(object sender, EventArgs e)
        {
            Properties.Settings.Default.EMail = txtEmail.Text;
            Properties.Settings.Default.Save();
            UpdateEmail();
            if (txtUser.Text.Length == 0)
            {
                txtUser.Text = txtEmail.Text;
                Properties.Settings.Default.Username = txtUser.Text;
                Properties.Settings.Default.Save();
            }
        }

        /// <summary>
        /// Handles the CheckStateChanged event of the overrideAutodiscoveryValidationToolStripMenuItem control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void overrideAutodiscoveryValidationToolStripMenuItem_CheckStateChanged(object sender, EventArgs e)
        {
            if (startingUp) return;

            Properties.Settings.Default.OverrideValidation = overrideAutodiscoveryValidationToolStripMenuItem.Checked;
            Properties.Settings.Default.Save();
            AddLogEntry("Autodiscovery Validation override switched " + (Properties.Settings.Default.OverrideValidation ? "ON" : "OFF"), LogType.Info);
        }

        /// <summary>
        /// Updates the office365 field.
        /// </summary>
        private void UpdateOffice365Field()
        {
            Office365Account = StripOffice365Account(Properties.Settings.Default.EMail);
            loginAutomaticallyToolStripMenuItem.Enabled = !Properties.Settings.Default.UseOffice365;
        }

        /// <summary>
        /// Strips the office365 account.
        /// </summary>
        /// <param name="email">The email.</param>
        /// <returns></returns>
        private string StripOffice365Account(string email)
        {
            string sub = "";
            int start = email.IndexOf("@");
            if (start > 0)
            {
                string body = email.Substring(start + 1);
                int end = body.IndexOf(".");
                if (end > 0) sub = body.Substring(0, end);
            }
            return sub;
        }

        /// <summary>
        /// Handles the CheckStateChanged event of the office365LoginOverrideToolStripMenuItem control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void office365LoginOverrideToolStripMenuItem_CheckStateChanged(object sender, EventArgs e)
        {
            if (startingUp) return;

            Properties.Settings.Default.UseOffice365 = office365LoginOverrideToolStripMenuItem.Checked;
            Properties.Settings.Default.Save();
            UpdateOffice365Field();
            UpdateOwaUrl();
            AddLogEntry("Office365 login override " + (Properties.Settings.Default.UseOffice365 ? "ON" : "OFF"), LogType.Info);
        }

        /// <summary>
        /// Handles the Validated event of the txtURLEdit control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void txtURLEdit_Validated(object sender, EventArgs e)
        {
            Properties.Settings.Default.ManualURL = txtURLEdit.Text;
            Properties.Settings.Default.Save();
            UpdateURL();
        }

        /// <summary>
        /// Handles the CheckedChanged event of the cbOverrideEWS control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void cbOverrideEWS_CheckedChanged(object sender, EventArgs e)
        {
            if (startingUp) return;

            Properties.Settings.Default.OverrideURL = cbOverrideEWS.Checked;
            Properties.Settings.Default.Save();
            UpdateEWSField();
            AddLogEntry("EWS URL override switched " + (Properties.Settings.Default.OverrideURL ? "ON" : "OFF"), LogType.Info);
        }

        /// <summary>
        /// Handles the CheckedChanged event of the chkAutodiscovery control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void chkAutodiscovery_CheckedChanged(object sender, EventArgs e)
        {
            if (startingUp) return;

            Properties.Settings.Default.Autodiscovery = chkAutodiscovery.Checked;
            Properties.Settings.Default.Save();
            AddLogEntry("Autodiscovery is switched " + (chkAutodiscovery.Checked ? "ON" : "OFF"), LogType.Info);

            // Switch off some options when Autodiscovery is checked
            SelectAutodiscoveryOptions();

            // Re-evaluate settings
            UpdateURL();
            UpdateOwaUrl();
            UpdateEmail();
        }

        /// <summary>
        /// Handles the CheckedChanged event of the cbOverrideOWA control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void cbOverrideOWA_CheckedChanged(object sender, EventArgs e)
        {
            if (startingUp) return;

            Properties.Settings.Default.OverrideOWAUrl = cbOverrideOWA.Checked;
            Properties.Settings.Default.Save();
            UpdateOwaUrl();
            AddLogEntry("OWA URL override switched " + (Properties.Settings.Default.OverrideOWAUrl ? "ON" : "OFF"), LogType.Info);
        }

        /// <summary>
        /// Handles the Validated event of the txtOWAEdit control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void txtOWAEdit_Validated(object sender, EventArgs e)
        {
            Properties.Settings.Default.ManualOWAUrl = txtOWAEdit.Text;
            Properties.Settings.Default.Save();
            UpdateOwaUrl();
        }
    }
}