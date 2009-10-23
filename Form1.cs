//------------------------------------------------------------------
// Cygnet OWA Tray Monitor
// Main Form
//
// <copyright file="Form1.cs" company="Cygnet Solutions Ltd">
//     Copyright (c) 2009 Cygnet Solutions Ltd. All rights reserved.
// </copyright>
//
// Monitors Exchange email for OWA users
// Main application form which drives all functionality.
//
// Author: IRS
// $Revision: 1.13 $
//------------------------------------------------------------------

namespace Cygnet.OWAtray
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Net;
    using System.Security;
    using System.Timers;
    using System.Windows.Forms;
    using Growl.Connector;
    using Microsoft.Exchange.WebServices.Data;
    using Snarl;

    /// <summary>
    /// Main application form which drives all functionality.
    /// </summary>
    public partial class Form1 : Form
    {
        #region Enums
        /// <summary>
        /// Severity of logging entry
        /// </summary>
        private enum LogType { Success, Fail, Info }
        #endregion

        #region Constants
        const int ScreenRefresh = 1;
        const int ScreenLines = 1000;
        const string ThisApp = "OWA Tray Monitor";
        const string ThisPublisher = "The Drunken Bakery";
        private const Int32 REPLY_MSG = 0x400 + 100;
        #endregion

        #region Class Variables
        static byte[] entropy = System.Text.Encoding.Unicode.GetBytes("Salt Is Not A Password");
        private Form frmNET;
        private Form frmMDAC;
        private Form frmInfo;
        private Form frmAbout;
        private Form frmContact;
        private ExchangeService myService;
        List<ListViewItem> lvitems = new List<ListViewItem>();
        private System.Threading.Timer _ScreenLogTimer;
        private System.Threading.TimerCallback _ScreenLogTimerCallback;
        static bool overRideClose = false;
        private static System.Timers.Timer updateTimer;
        private string _Server;
        private string _User;
        private SecureString _Pwd;
        private string _Domain;
        private string _Interval;
        private int _InboxCount;
        // Notifications
        bool isBalloon;
        bool isGrowl;
        bool isSnarl;
        bool isBell;
        private string trayIcon;
        private string newIcon;
        // Growl
        private string iconPath;
        private GrowlConnector growl;
        private NotificationType newMail;
        private Growl.Connector.Application application;
        // Domain
        bool isDomain;
        private string wavFile;
        #endregion

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

            // Initialise Event Views
            InitEventView(lvStatus);

            // Options
            txtServer.Text = Properties.Settings.Default.Server;
            txtUser.Text = Properties.Settings.Default.Username;
            txtPwd.Text = ToInsecureString(DecryptString(Properties.Settings.Default.Password));
            txtDomain.Text = Properties.Settings.Default.Domain;
            txtInterval.Text = Properties.Settings.Default.UpdateInterval;
            _Interval = txtInterval.Text;
            _InboxCount = 0;

            // Form title bar
            this.Text = ThisApp + " freshly baked at " + ThisPublisher;

            // Logging
            AddLogEntry("--------------------------------------------------", LogType.Info);
            AddLogEntry("Welcome to the " + ThisApp + " v" + appVersionString, LogType.Info);
            AddLogEntry("Configured to communicate with Exchange " + Properties.Settings.Default.ExchangeVersion);
            AddLogEntry("Ready.");
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

            // Domain
            isDomain = Properties.Settings.Default.NetworkCredentials == "Yes";
            chkOnDomain.Checked = isDomain;

            // Icon
            iconPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + @"\email.png";
            trayIcon = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + @"\email.ico";
            newIcon = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + @"\comment_rect.ico";

            // Sound file
            wavFile = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + @"\notify.wav";

            // Tray icon (default)
            notifyIcon1.Icon = new Icon(trayIcon);

            // Growl
            this.growl = new GrowlConnector();
            application = new Growl.Connector.Application(ThisApp);
            application.Icon = iconPath;
            this.newMail = new NotificationType("NEWMAIL", "New Mail");
            this.growl.Register(application, new NotificationType[] { newMail });

            // Snarl
            SnarlConnector.RegisterConfig(this.Handle, ThisApp, WindowsMessage.WM_MDIMAXIMIZE, iconPath);

            // Validate the server certificate
            ServicePointManager.ServerCertificateValidationCallback = CertificateValidationCallBack;

            // Start Timers
            _ScreenLogTimerCallback = new System.Threading.TimerCallback(_ScreenLogTimer_Elapsed);
            _ScreenLogTimer = new System.Threading.Timer(_ScreenLogTimerCallback, null, (Convert.ToInt32(ScreenRefresh) * 1000), System.Threading.Timeout.Infinite);
            updateTimer = new System.Timers.Timer(Convert.ToInt32(txtInterval.Text) * 1000);
            updateTimer.Elapsed += new ElapsedEventHandler(updateTimer_Elapsed);
            updateTimer.Stop();

            // Window state
            if (Properties.Settings.Default.FirstTime != "Yes")
            {
                timer1.Enabled = true;
            }
        }

        /// <summary>
        /// Handles the Elapsed event of the updateTimer control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Timers.ElapsedEventArgs"/> instance containing the event data.</param>
        void updateTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            GetUnreadCount();
        }

        /// <summary>
        /// Updates the status view by flushing the buffer.
        /// </summary>
        /// <param name="sender">The sender.</param>
        private void _ScreenLogTimer_Elapsed(object sender)
        {
            CycleStatusView();
            _ScreenLogTimer.Change((Convert.ToInt32(ScreenRefresh) * 1000), System.Threading.Timeout.Infinite);
        }

        /// <summary>
        /// Connects to Exchange.
        /// </summary>
        private void ConnectToExchange()
        {
            try
            {
                AddLogEntry("Connecting to Exchange...");
                myService = new ExchangeService(Properties.Settings.Default.ExchangeVersion == "2010" ? ExchangeVersion.Exchange2010 : ExchangeVersion.Exchange2007_SP1);
                Uri myUri = new Uri(txtURL.Text);
                if (isDomain)
                {
                    myService.UseDefaultCredentials = true;
                }
                else
                {
                    myService.Credentials = new WebCredentials(txtUser.Text, txtPwd.Text, txtDomain.Text);
                }
                myService.Url = myUri;
                AddLogEntry("Connected");
            }
            catch (Exception ex)
            {
                AddLogEntry("Error: " + ex.Message, LogType.Fail);
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

            try
            {
                Folder myFolder = Folder.Bind(myService, WellKnownFolderName.Inbox);
                myCount = myFolder.UnreadCount;
                if (myCount > _InboxCount)
                {
                    PopToast("New Mail", "You have " + myCount + " unread email" + (myCount > 1 ? "s " : " ") + "in your inbox");
                }

                notifyIcon1.Icon = new Icon((myCount > 0 ? newIcon : trayIcon));
                notifyIcon1.Text = ThisApp + Environment.NewLine + myCount + " unread email" + (myCount > 1 ? "s " : " ");
                _InboxCount = myCount;
            }
            catch (Exception ex)
            {
                AddLogEntry("Error: " + ex.Message, LogType.Fail);
                myCount = 0;
            }

            return myCount;
        }

        /// <summary>
        /// Pops the toast.
        /// </summary>
        /// <param name="myTitle">My title.</param>
        /// <param name="myMessage">My message.</param>
        private void PopToast(string myTitle, string myMessage)
        {
            AddLogEntry(myTitle, LogType.Info);

            //Balloon
            if (isBalloon)
            {
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
        /// Cycles the status view.
        /// </summary>
        private void CycleStatusView()
        {
            PauseOutput(lvStatus);
            FlushOutput(lvStatus);
            ResumeOutput(lvStatus);
        }

        private delegate void FlushOutputDelegate(ListView lv);

        /// <summary>
        /// Flushes the output.
        /// </summary>
        /// <param name="lv">The lv.</param>
        private void FlushOutput(ListView lv)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new FlushOutputDelegate(FlushOutput), new object[] { lv });
                return;
            }

            if (lvitems.Count > 0)
            {
                if (lv.Items.Count >= Convert.ToInt32(ScreenLines)) lv.Items.Clear();
                lv.BeginUpdate();
                lv.Items.AddRange(lvitems.ToArray());
                lv.EnsureVisible(lv.Items.Count - 1);
                lv.EndUpdate();
                lvitems.Clear();
            }
        }

        private delegate void PauseOutputDelegate(ListView lv);

        /// <summary>
        /// Pauses the output.
        /// </summary>
        /// <param name="lv">The lv.</param>
        private void PauseOutput(ListView lv)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new PauseOutputDelegate(PauseOutput), new object[] { lv });
                return;
            }

            lv.BeginUpdate();
        }

        private delegate void ResumeOutputDelegate(ListView lv);
        /// <summary>
        /// Resumes the output.
        /// </summary>
        /// <param name="lv">The lv.</param>
        private void ResumeOutput(ListView lv)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new ResumeOutputDelegate(ResumeOutput), new object[] { lv });
                return;
            }

            lv.EndUpdate();
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
            switch (whichLog)
            {
                case LogType.Success:
                    lvitems.Add(new ListViewItem(DateTime.Now.ToString(), 0));
                    break;

                case LogType.Fail:
                    lvitems.Add(new ListViewItem(DateTime.Now.ToString(), 1));
                    break;

                case LogType.Info:
                    lvitems.Add(new ListViewItem(DateTime.Now.ToString(), 2));
                    break;
            }

            int i = (lvitems.Count - 1);
            lvitems[i].SubItems.Add(newEntry);
            slStatus.Text = newEntry;
        }

        /// <summary>
        /// Certificates the validation call back.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="certificate">The certificate.</param>
        /// <param name="chain">The chain.</param>
        /// <param name="sslPolicyErrors">The SSL policy errors.</param>
        /// <returns>Is Certifcate Valid?</returns>
        private static bool CertificateValidationCallBack(
                 object sender,
                 System.Security.Cryptography.X509Certificates.X509Certificate certificate,
                 System.Security.Cryptography.X509Certificates.X509Chain chain,
                 System.Net.Security.SslPolicyErrors sslPolicyErrors)
            {
            // If the certificate is a valid, signed certificate, return true.
            if (sslPolicyErrors == System.Net.Security.SslPolicyErrors.None)
            {
                return true;
            }

            // If thre are errors in the certificate chain, look at each error to determine the cause.
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
        /// Handles the Click event of the cmdUnread control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void cmdUnread_Click(object sender, EventArgs e)
        {
            GetUnreadCount();
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
        /// Handles the BalloonTipClicked event of the notifyIcon1 control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void notifyIcon1_BalloonTipClicked(object sender, EventArgs e)
        {
            if (Control.MouseButtons == MouseButtons.Left)
            {
                activateOWA();
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

            activateOWA();
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
        /// Handles the TextChanged event of the txtPwd control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void txtPwd_TextChanged(object sender, EventArgs e)
        {
            _Pwd = ToSecureString(txtPwd.Text);
        }

        /// <summary>
        /// Updates the URL.
        /// </summary>
        private void UpdateURL()
        {
            txtURL.Text = "https://" + txtServer.Text + "/ews/exchange.asmx";
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
        /// Handles the Click event of the cmdStart control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void cmdStart_Click(object sender, EventArgs e)
        {
            // Start
            startMonitoring();
        }

        /// <summary>
        /// Starts the monitoring.
        /// </summary>
        private void startMonitoring()
        {
            // Connect to Exchange Web Services
            ConnectToExchange();

            // Initial check
            GetUnreadCount();

            // Start Timer
            updateTimer.Interval = Convert.ToInt32(_Interval) * 1000;
            updateTimer.Start();
            AddLogEntry(_Interval + " second timer started", LogType.Info);

            // Minimise to tray
            this.WindowState = FormWindowState.Minimized;
        }

        /// <summary>
        /// Handles the Click event of the cmdStop control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void cmdStop_Click(object sender, EventArgs e)
        {
            updateTimer.Stop();
            AddLogEntry("Timer stopped", LogType.Info);
            notifyIcon1.Text = ThisApp + Environment.NewLine + "Not Connected to Exchange";
        }

        /// <summary>
        /// Handles the Click event of the cmdSave control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void cmdSave_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.Server = _Server;
            Properties.Settings.Default.Username = _User;
            Properties.Settings.Default.Password = EncryptString(_Pwd);
            Properties.Settings.Default.Domain = _Domain;
            Properties.Settings.Default.UpdateInterval = _Interval;
            Properties.Settings.Default.FirstTime = "No";
            Properties.Settings.Default.Balloon = isBalloon ? "Yes" : "No";
            Properties.Settings.Default.Growl = isGrowl ? "Yes" : "No";
            Properties.Settings.Default.Snarl = isSnarl ? "Yes" : "No";
            Properties.Settings.Default.NetworkCredentials = isDomain ? "Yes" : "No";
            Properties.Settings.Default.Bell = isBell ? "Yes" : "No";
            Properties.Settings.Default.Save();

            AddLogEntry("Settings saved to file", LogType.Info);
        }

        [System.Runtime.InteropServices.DllImport("winmm.DLL", EntryPoint = "PlaySound", SetLastError = true)]
        private static extern bool PlaySound(string szSound, System.IntPtr hMod, PlaySoundFlags flags);

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
        /// Handles the Click event of the openOWAToolStripMenuItem control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void openOWAToolStripMenuItem_Click(object sender, EventArgs e)
        {
            activateOWA();
        }

        /// <summary>
        /// Activates the OWA.
        /// </summary>
        private void activateOWA()
        {
            System.Diagnostics.Process.Start("https://" + _Server + "/owa");
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
        /// Handles the Leave event of the txtInterval control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void txtInterval_Leave(object sender, EventArgs e)
        {
            int result;

            if (txtInterval.Text == _Interval)
            {
                return;
            }

            if (int.TryParse(txtInterval.Text, out result))
            {
                if (result <= 60)
                {
                    _Interval = txtInterval.Text;
                    AddLogEntry("Update interval changed to " + _Interval + " seconds. Restart Timer to activate.", LogType.Info);
                }
                else
                {
                    AddLogEntry("Update interval illegal. Ignored.", LogType.Fail);
                }
            }
            else
            {
                AddLogEntry("Update interval illegal. Ignored.", LogType.Fail);
            }
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
    }
}
