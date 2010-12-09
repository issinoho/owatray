//------------------------------------------------------------------
// DrunkenBakery OWA Tray Monitor
// Main Form
//
// <copyright file="Form1.cs" company="The Drunken Bakery">
//     Copyright (c) 2009, 2010 The Drunken Bakery. All rights reserved.
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
        private static FlatFile myLog;
        bool alwaysIE;
        private Growl.Connector.Application application;
        bool calendarOff;
        bool autoLogin;
        private bool firstRun = true;
        private Form frmAbout;
        private Form frmChangeLog;
        private Form frmContact;
        private Form frmInfo;
        private Form frmMDAC;
        private Form frmNET;
        private GrowlConnector growl;
        private string iconPath;
        bool isBalloon;
        bool isBell;
        bool isDomain;
        bool isGrowl;
        bool isSnarl;
        private string lastLogEntry;
        private string lastPopMessage;
        private string lastPopTitle;
        private string lastPopUrl;
        List<ListViewItem> lvBuffer = new List<ListViewItem>();
        private ExchangeService myService;
        private string newIcon;
        private NotificationType newMail;
        bool overrideCert;
        bool overrideURL;
        bool resetFlag;
        private string shellPath;
        private DateTime TimeLastChecked = DateTime.Now;
        private string trayIcon;
        private string wavFile;
        private string _Domain;
        private string _ExchangeVersion;
        private int _InboxCount;
        private string _Interval;
        private SecureString _Pwd;
        private string _Server;
        private string _User;
        private string _Email;
        private string popUrl;

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

            // Start Logging
            InitLogger();

            // Initialise Event Views
            InitEventView(lvStatus);

            // Options
            _ExchangeVersion = Properties.Settings.Default.ExchangeVersion;
            switch (_ExchangeVersion)
            {
                case "2007":
                    exchange2007ToolStripMenuItem.SelectedIndex = 0;
                    break;

                case "2010":
                    exchange2007ToolStripMenuItem.SelectedIndex = 1;
                    break;

                case "2010SP1":
                    exchange2007ToolStripMenuItem.SelectedIndex = 2;
                    break;
            }
            txtServer.Text = Properties.Settings.Default.Server;
            _Server = txtServer.Text;
            txtUser.Text = Properties.Settings.Default.Username;
            _User = txtUser.Text;
            txtPwd.Text = ToInsecureString(DecryptString(Properties.Settings.Default.Password));
            _Pwd = ToSecureString(txtPwd.Text);
            txtEmail.Text = Properties.Settings.Default.EMail;
            _Email = txtEmail.Text;
            txtDomain.Text = Properties.Settings.Default.Domain;
            _Domain = txtDomain.Text;
            UpdateURL();
            txtInterval.Text = Properties.Settings.Default.UpdateInterval;
            _Interval = txtInterval.Text;
            _InboxCount = 0;

            // Form title bar
            this.Text = ThisApp + " freshly baked at " + ThisPublisher;

            // Logging
            AddLogEntry("--------------------------------------------------", LogType.Info);
            AddLogEntry("Welcome to the " + ThisApp + " v" + appVersionString, LogType.Info);
            AddLogEntry("Configured to communicate with Exchange " + _ExchangeVersion);
            notifyIcon1.Text = ThisApp + Environment.NewLine + "Not Connected to Exchange";

            // Startup Flag
            chkRunOnStartup.Checked = Link.Exists(Environment.SpecialFolder.Startup, ThisApp);

            // Notifications
            isBalloon = Properties.Settings.Default.Balloon == "Yes";
            balloonToolStripMenuItem.Checked = isBalloon;
            isGrowl = Properties.Settings.Default.Growl == "Yes";
            growlToolStripMenuItem.Checked = isGrowl;
            isSnarl = Properties.Settings.Default.Snarl == "Yes";
            snarlToolStripMenuItem.Checked = isSnarl;
            isBell = Properties.Settings.Default.Bell == "Yes";
            playSoundToolStripMenuItem.Checked = isBell;

            // Overrides
            overrideCert = Properties.Settings.Default.OverrideCert == "Yes";
            overrideToolStripMenuItem.Checked = overrideCert;
            overrideURL = Properties.Settings.Default.OverrideURL == "Yes";
            overrideServerURLToolStripMenuItem.Checked = overrideURL;
            alwaysIE = Properties.Settings.Default.AlwaysIE == "Yes";
            alwaysOpenOWAInIEToolStripMenuItem.Checked = alwaysIE;
            calendarOff = Properties.Settings.Default.DisableCalendar == "Yes";
            disableCalendarToolStripMenuItem.Checked = calendarOff;
            autoLogin = Properties.Settings.Default.AutoLogin == "Yes";
            loginAutomaticallyToolStripMenuItem.Checked = autoLogin;
            drawURL();

            // Override email address?
            chkOverride.Checked = Properties.Settings.Default.EmailOverride == "Yes";
            if (_Email.Length > 0 && !chkOverride.Checked) chkOverride.Checked = true;

            // Domain
            isDomain = Properties.Settings.Default.NetworkCredentials == "Yes";
            chkOnDomain.Checked = isDomain;

            // Special lockdown option
            restoreToolStripMenuItem.Enabled = (Properties.Settings.Default.LockDown == "Yes" ? false : true);

            // Icon
            iconPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + @"\email.png";
            trayIcon = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + @"\email.ico";
            newIcon = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + @"\comment_rect.ico";

            // Sound file
            wavFile = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + @"\notify.wav";

            // Tray icon (default)
            notifyIcon1.Icon = new Icon(trayIcon);
            lastPopTitle = "";
            lastPopMessage = "";
            popUrl = "";
            lastPopUrl = "";
            resetFlag = false;

            // Growl
            this.growl = new GrowlConnector();
            application = new Growl.Connector.Application(ThisApp);
            application.Icon = iconPath;
            this.newMail = new NotificationType("NEWMAIL", "New Mail");
            this.growl.Register(application, new NotificationType[] { newMail });

            // Snarl
            SnarlConnector.RegisterConfig(this.Handle, ThisApp, WindowsMessage.WM_MDIMAXIMIZE, iconPath);

            // Configure Shell
            ConfigureShell();

            // Configure Exchange
            if (ConfigureExchange())
            {
                AddLogEntry("Ready.", LogType.Info);
            }

            // Start Timers
            timerAppt.Interval = Convert.ToInt32(Properties.Settings.Default.ApptInterval) * 1000;
            timerUpdate.Interval = Convert.ToInt32(txtInterval.Text) * 1000;
            timerLogging.Enabled = true;

            // Window state
            if (Properties.Settings.Default.FirstTime != "Yes")
            {
                timer1.Enabled = true;
            }
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

            if (alwaysIE)
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
            alwaysIE = alwaysOpenOWAInIEToolStripMenuItem.Checked;
            AddLogEntry("Always use IE switched " + (alwaysIE ? "ON" : "OFF"), LogType.Info);
        }

        /// <summary>
        /// Handles the CheckStateChanged event of the balloonToolStripMenuItem control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void balloonToolStripMenuItem_CheckStateChanged(object sender, EventArgs e)
        {
            isBalloon = balloonToolStripMenuItem.Checked;
            AddLogEntry("Balloon notifications switched " + (isBalloon ? "ON" : "OFF"), LogType.Info);
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
            if (overrideCert)
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
                                popUrl = (_ExchangeVersion == "2007" ? "" : myAppt.WebClientReadFormQueryString);
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
            isDomain = chkOnDomain.Checked;
            txtDomain.Enabled = !isDomain;
            txtPwd.Enabled = !isDomain;
            txtUser.Enabled = !isDomain;
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
        /// Handles the Click event of the cmdForce control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void cmdForce_Click(object sender, EventArgs e)
        {
            GetUnreadCount();
        }

        /// <summary>
        /// Handles the Click event of the cmdSave control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void cmdSave_Click(object sender, EventArgs e)
        {
            try
            {
                Properties.Settings.Default.Server = _Server;
                Properties.Settings.Default.Username = _User;
                Properties.Settings.Default.Password = (_Pwd.Length > 0 ? EncryptString(_Pwd) : "");
                Properties.Settings.Default.Domain = _Domain;
                Properties.Settings.Default.EMail = _Email;
                Properties.Settings.Default.UpdateInterval = _Interval;
                Properties.Settings.Default.FirstTime = "No";
                Properties.Settings.Default.Balloon = isBalloon ? "Yes" : "No";
                Properties.Settings.Default.Growl = isGrowl ? "Yes" : "No";
                Properties.Settings.Default.Snarl = isSnarl ? "Yes" : "No";
                Properties.Settings.Default.NetworkCredentials = isDomain ? "Yes" : "No";
                Properties.Settings.Default.Bell = isBell ? "Yes" : "No";
                Properties.Settings.Default.OverrideURL = overrideURL ? "Yes" : "No";
                Properties.Settings.Default.OverrideCert = overrideCert ? "Yes" : "No";
                Properties.Settings.Default.ManualURL = txtURLEdit.Text;
                Properties.Settings.Default.AlwaysIE = alwaysIE ? "Yes" : "No";
                Properties.Settings.Default.DisableCalendar = calendarOff ? "Yes" : "No";
                Properties.Settings.Default.AutoLogin = autoLogin ? "Yes" : "No";
                Properties.Settings.Default.ExchangeVersion = _ExchangeVersion;
                Properties.Settings.Default.EmailOverride = chkOverride.Checked ? "Yes" : "No";
                Properties.Settings.Default.Save();

                AddLogEntry("Settings saved to file", LogType.Info);

                ConfigureShell();
            }
            catch (Exception ex)
            {
                AddLogEntry("Can't save settings: " + ex.ToString(), LogType.Fail);
            }
        }

        /// <summary>
        /// Handles the Click event of the cmdStart control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void cmdStart_Click(object sender, EventArgs e)
        {
            ConfigureExchange();
            // Start
            startMonitoring();
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
                string thisUri = "";

                // Validate the server certificate
                ServicePointManager.ServerCertificateValidationCallback = CertificateValidationCallBack;

                AddLogEntry("Binding to Exchange", LogType.Info);
                switch (_ExchangeVersion)
                {
                    case "2007":
                        myService = new ExchangeService(ExchangeVersion.Exchange2007_SP1);
                        break;

                    case "2010":
                        myService = new ExchangeService(ExchangeVersion.Exchange2010);
                        break;

                    case "2010SP1":
                        myService = new ExchangeService(ExchangeVersion.Exchange2010_SP1);
                        break;
                }
                if (overrideURL)
                {
                    thisUri = txtURLEdit.Text;
                }
                else
                {
                    thisUri = txtURL.Text;
                }
                Uri myUri = new Uri(thisUri);
                if (isDomain)
                {
                    myService.UseDefaultCredentials = true;
                }
                else
                {
                    myService.Credentials = new WebCredentials(txtUser.Text, txtPwd.Text, txtDomain.Text);
                }
                myService.Url = myUri;

                return true;
            }
            catch (Exception ex)
            {
                AddLogEntry("Error: " + ex.Message, LogType.Fail);
                return false;
            }
        }

        /// <summary>
        /// Configures the shell.
        /// </summary>
        private void ConfigureShell()
        {
            // Path to shell integration module
            shellPath = Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), Properties.Settings.Default.ShellIntegration);

            // Set OWA Url
            string owaUrl = "https://" + _Server + "/owa";
            AddLogEntry("Setting OWA url to " + owaUrl, LogType.Info);

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
            string userAccount = (chkOverride.Checked && _Email.Length > 0) ? _Email : _User;
            if (!userAccount.Contains("@"))
            {
                userAccount = userAccount + "@" + GetSubDomain(_Server);
            }
            AddLogEntry("Using user account: " + userAccount, LogType.Info);

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
            AddLogEntry("Configuring for Exchange " + _ExchangeVersion, LogType.Info);

            try
            {
                ProcessStartInfo RunSvc = new ProcessStartInfo(shellPath);
                RunSvc.Arguments = "exchange " + _ExchangeVersion;
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
                RunSvc.Arguments = "autologin " + (autoLogin ? "Yes" : "No");
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
                RunSvc.Arguments = "browser " + (alwaysIE ? "Yes" : "No");
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
            calendarOff = disableCalendarToolStripMenuItem.Checked;
            AddLogEntry("Calendar notifications switched " + (calendarOff ? "OFF" : "ON"), LogType.Info);
        }

        /// <summary>
        /// Draws the URL.
        /// </summary>
        private void drawURL()
        {
            if (overrideURL)
            {
                txtURL.Visible = false;
                txtURLEdit.Visible = true;
                txtURLEdit.Text = Properties.Settings.Default.ManualURL;
            }
            else
            {
                txtURL.Visible = true;
                txtURLEdit.Visible = false;
            }
        }

        private void exchange2007ToolStripMenuItem_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (exchange2007ToolStripMenuItem.SelectedIndex)
            {
                case 0:
                    _ExchangeVersion = "2007";
                    break;

                case 1:
                    _ExchangeVersion = "2010";
                    break;

                case 2:
                    _ExchangeVersion = "2010SP1";
                    break;
            }
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
                                slStatus.Text = lv.SubItems[1].Text;
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
            if (firstRun) TimeLastChecked = DateTime.Now.AddMinutes(-1);

            try
            {
                Folder myFolder = Folder.Bind(myService, WellKnownFolderName.Inbox);
                myCount = myFolder.UnreadCount;
                if (myCount > _InboxCount)
                {
                    if (firstRun)
                    {
                        PopToast("New Mail", "You have " + myCount + " unread email" + (myCount != 1 ? "s " : " ") + "in your inbox");                        
                    }
                    else
                    {
                        int count = PopUnreadEmail(myCount);
                        //if (count != myCount)
                        //{
                        //    AddLogEntry("Not all new mail has been notified", LogType.Fail);
                        //}
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
                string domainText = _Server + (isDomain ? "" : @"\" + _User);
                if (domainText.Length > charsLeft) domainText = domainText.Substring(0, charsLeft);
                string finalText = ThisApp + Environment.NewLine + domainText + Environment.NewLine + myCount + " unread email" + (myCount != 1 ? "s " : " ");
                notifyIcon1.Text = finalText;
                _InboxCount = myCount;
            }
            catch (Exception ex)
            {
                AddLogEntry("Error: " + ex.Message, LogType.Fail);
                myCount = 0;
            }

            firstRun = false;
            return myCount;
        }

        /// <summary>
        /// Handles the CheckStateChanged event of the growlToolStripMenuItem control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void growlToolStripMenuItem_CheckStateChanged(object sender, EventArgs e)
        {
            isGrowl = growlToolStripMenuItem.Checked;
            AddLogEntry("Growl notifications switched " + (isGrowl ? "ON" : "OFF"), LogType.Info);
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
            string path1 = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            myLog.LogFile = Path.Combine(path1, "owatray.log");
            myLog.DateOn = true;
            myLog.Verbose = true;
            myLog.LimitSize = true;
            myLog.Scavenge = true;
            myLog.Active = true;
        }

        /// <summary>
        /// Handles the CheckStateChanged event of the overrideServerURLToolStripMenuItem control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void overrideServerURLToolStripMenuItem_CheckStateChanged(object sender, EventArgs e)
        {
            overrideURL = overrideServerURLToolStripMenuItem.Checked;
            AddLogEntry("Server URL override switched " + (overrideURL ? "ON" : "OFF"), LogType.Info);

            drawURL();
        }

        /// <summary>
        /// Handles the CheckStateChanged event of the overrideToolStripMenuItem control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void overrideToolStripMenuItem_CheckStateChanged(object sender, EventArgs e)
        {
            overrideCert = overrideToolStripMenuItem.Checked;
            AddLogEntry("SSL Certificate override switched " + (overrideCert ? "ON" : "OFF"), LogType.Info);
        }

        /// <summary>
        /// Handles the CheckStateChanged event of the playSoundToolStripMenuItem control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void playSoundToolStripMenuItem_CheckStateChanged(object sender, EventArgs e)
        {
            isBell = playSoundToolStripMenuItem.Checked;
            AddLogEntry("Audible notifications switched " + (isBell ? "ON" : "OFF"), LogType.Info);
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
            if (isBalloon)
            {
                notifyIcon1.Tag = popUrl;
                notifyIcon1.ShowBalloonTip(5000, myTitle, myMessage, ToolTipIcon.Info);
            }

            // Growl
            if (isGrowl)
            {
                Notification notification = new Notification(this.application.Name, this.newMail.Name, "", myTitle, myMessage);
                this.growl.Notify(notification);
            }

            // Snarl
            if (isSnarl)
            {
                SnarlConnector.ShowMessage(myTitle, myMessage, 10, iconPath, this.Handle, (WindowsMessage)REPLY_MSG);
            }

            // Audible
            if (isBell)
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
            int count = 0;

            // Set the offset for the paged search.
            int offset = 0;

            // Set the page size.
            const int pageSize = 50;

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
                                popUrl = (_ExchangeVersion == "2007" ? "" : myEmail.WebClientReadFormQueryString);
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

            firstRun = false;
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
            isSnarl = snarlToolStripMenuItem.Checked;
            AddLogEntry("Snarl notifications switched " + (isSnarl ? "ON" : "OFF"), LogType.Info);
        }

        /// <summary>
        /// Starts the monitoring.
        /// </summary>
        private void startMonitoring()
        {
            // Start Timer
            timerAppt.Start();
            timerUpdate.Interval = Convert.ToInt32(_Interval) * 1000;
            timerUpdate.Start();
            AddLogEntry(_Interval + " second timer started", LogType.Info);

            // Minimise to tray
            this.WindowState = FormWindowState.Minimized;

            // Initial Check
            GetUnreadCount();
            if (!calendarOff)
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
            if (!calendarOff)
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
        /// Handles the TextChanged event of the txtDomain control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void txtDomain_TextChanged(object sender, EventArgs e)
        {
            _Domain = txtDomain.Text;
        }

        /// <summary>
        /// Handles the Validated event of the txtInterval control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void txtInterval_Validated(object sender, EventArgs e)
        {
            _Interval = txtInterval.Text;
            AddLogEntry("Update interval changed to " + _Interval + " seconds. Restart Timer to activate.", LogType.Info);
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
        /// Handles the TextChanged event of the txtPwd control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void txtPwd_TextChanged(object sender, EventArgs e)
        {
            _Pwd = ToSecureString(txtPwd.Text);
        }

        /// <summary>
        /// Handles the TextChanged event of the txtServer control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void txtServer_TextChanged(object sender, EventArgs e)
        {
            _Server = txtServer.Text;
            UpdateURL();
        }

        /// <summary>
        /// Handles the TextChanged event of the txtUser control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void txtUser_TextChanged(object sender, EventArgs e)
        {
            _User = txtUser.Text;
        }

        /// <summary>
        /// Updates the URL.
        /// </summary>
        private void UpdateURL()
        {
            txtURL.Text = "https://" + txtServer.Text + "/ews/exchange.asmx";
        }

        #endregion Methods

        /// <summary>
        /// Handles the TextChanged event of the txtEmail control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void txtEmail_TextChanged(object sender, EventArgs e)
        {
            _Email = txtEmail.Text;
        }

        /// <summary>
        /// Handles the CheckStateChanged event of the loginAutomaticallyToolStripMenuItem control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void loginAutomaticallyToolStripMenuItem_CheckStateChanged(object sender, EventArgs e)
        {
            autoLogin = loginAutomaticallyToolStripMenuItem.Checked;
            AddLogEntry("Automatic Login is switched " + (autoLogin ? "ON" : "OFF"), LogType.Info);
        }

        /// <summary>
        /// Handles the CheckedChanged event of the chkOverride control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void chkOverride_CheckedChanged(object sender, EventArgs e)
        {
            txtEmail.Enabled = chkOverride.Checked;
        }
    }
}