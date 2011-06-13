namespace DrunkenBakery.OWAtray
{
    /// <summary>
    /// Designer class
    /// </summary>
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }

            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
			this.imLV = new System.Windows.Forms.ImageList(this.components);
			this.statusStrip1 = new System.Windows.Forms.StatusStrip();
			this.slStatus = new System.Windows.Forms.ToolStripStatusLabel();
			this.notifyIcon1 = new System.Windows.Forms.NotifyIcon(this.components);
			this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.restoreToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.blankToolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
			this.openOWAToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.openOutlookToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.dividerToolStripMenuItem = new System.Windows.Forms.ToolStripSeparator();
			this.recallLastPopupToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.resetTrayIconToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.blankToolStripMenuItem = new System.Windows.Forms.ToolStripSeparator();
			this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.menuStrip1 = new System.Windows.Forms.MenuStrip();
			this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.exitToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.nETVersionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.mDACVersionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.systemInformationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.notificationsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.balloonToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.growlToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.snarlToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.playSoundToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.advancedToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.overrideToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.alwaysOpenOWAInIEToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.disableCalendarToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.loginAutomaticallyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.overrideAutodiscoveryValidationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.office365LoginOverrideToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.exchangeVersionToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
			this.exchange2007ToolStripMenuItem = new System.Windows.Forms.ToolStripComboBox();
			this.shellIntegrationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.makeOWADefaultToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.switchOffToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.changeLogToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.supportToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.groupBox3 = new System.Windows.Forms.GroupBox();
			this.pictureBox4 = new System.Windows.Forms.PictureBox();
			this.lvStatus = new System.Windows.Forms.ListView();
			this.lblUrl = new System.Windows.Forms.Label();
			this.txtURLEdit = new System.Windows.Forms.TextBox();
			this.txtInterval = new System.Windows.Forms.TextBox();
			this.txtDomain = new System.Windows.Forms.TextBox();
			this.label5 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.txtServer = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.txtEmail = new System.Windows.Forms.TextBox();
			this.chkOnDomain = new System.Windows.Forms.CheckBox();
			this.cmdStop = new System.Windows.Forms.Button();
			this.cmdStart = new System.Windows.Forms.Button();
			this.txtPwd = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.txtUser = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.timer1 = new System.Windows.Forms.Timer(this.components);
			this.timerLogging = new System.Windows.Forms.Timer(this.components);
			this.timerUpdate = new System.Windows.Forms.Timer(this.components);
			this.timerAppt = new System.Windows.Forms.Timer(this.components);
			this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
			this.tabMain = new System.Windows.Forms.TabControl();
			this.tabPage1 = new System.Windows.Forms.TabPage();
			this.chkRunOnStartup = new System.Windows.Forms.CheckBox();
			this.pictureBox14 = new System.Windows.Forms.PictureBox();
			this.pictureBox15 = new System.Windows.Forms.PictureBox();
			this.label9 = new System.Windows.Forms.Label();
			this.chkAutodiscovery = new System.Windows.Forms.CheckBox();
			this.pictureBox3 = new System.Windows.Forms.PictureBox();
			this.tabPage2 = new System.Windows.Forms.TabPage();
			this.cbOverrideOWA = new System.Windows.Forms.CheckBox();
			this.txtOWAEdit = new System.Windows.Forms.TextBox();
			this.pictureBox5 = new System.Windows.Forms.PictureBox();
			this.label7 = new System.Windows.Forms.Label();
			this.cbOverrideEWS = new System.Windows.Forms.CheckBox();
			this.pictureBox2 = new System.Windows.Forms.PictureBox();
			this.pictureBox17 = new System.Windows.Forms.PictureBox();
			this.label8 = new System.Windows.Forms.Label();
			this.pictureBox16 = new System.Windows.Forms.PictureBox();
			this.pictureBox1 = new System.Windows.Forms.PictureBox();
			this.tabPage3 = new System.Windows.Forms.TabPage();
			this.lblOWAUrl = new System.Windows.Forms.Label();
			this.label13 = new System.Windows.Forms.Label();
			this.lblEmail = new System.Windows.Forms.Label();
			this.label6 = new System.Windows.Forms.Label();
			this.label10 = new System.Windows.Forms.Label();
			this.groupBox4 = new System.Windows.Forms.GroupBox();
			this.statusStrip1.SuspendLayout();
			this.contextMenuStrip1.SuspendLayout();
			this.menuStrip1.SuspendLayout();
			this.groupBox3.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox4)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
			this.tabMain.SuspendLayout();
			this.tabPage1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox14)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox15)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).BeginInit();
			this.tabPage2.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox5)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox17)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox16)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
			this.tabPage3.SuspendLayout();
			this.groupBox4.SuspendLayout();
			this.SuspendLayout();
			// 
			// imLV
			// 
			this.imLV.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imLV.ImageStream")));
			this.imLV.TransparentColor = System.Drawing.Color.Transparent;
			this.imLV.Images.SetKeyName(0, "apply.png");
			this.imLV.Images.SetKeyName(1, "info.png");
			this.imLV.Images.SetKeyName(2, "about.png");
			// 
			// statusStrip1
			// 
			this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.slStatus});
			this.statusStrip1.Location = new System.Drawing.Point(0, 421);
			this.statusStrip1.Name = "statusStrip1";
			this.statusStrip1.Size = new System.Drawing.Size(646, 22);
			this.statusStrip1.TabIndex = 4;
			this.statusStrip1.Text = "statusStrip1";
			// 
			// slStatus
			// 
			this.slStatus.Name = "slStatus";
			this.slStatus.Size = new System.Drawing.Size(42, 17);
			this.slStatus.Text = "Ready.";
			// 
			// notifyIcon1
			// 
			this.notifyIcon1.BalloonTipIcon = System.Windows.Forms.ToolTipIcon.Info;
			this.notifyIcon1.BalloonTipTitle = "OWA Tray Monitor";
			this.notifyIcon1.ContextMenuStrip = this.contextMenuStrip1;
			this.notifyIcon1.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon1.Icon")));
			this.notifyIcon1.Text = "OWA Tray Monitor";
			this.notifyIcon1.Visible = true;
			this.notifyIcon1.BalloonTipClicked += new System.EventHandler(this.notifyIcon1_BalloonTipClicked);
			this.notifyIcon1.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.notifyIcon1_MouseDoubleClick);
			// 
			// contextMenuStrip1
			// 
			this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.restoreToolStripMenuItem,
            this.blankToolStripMenuItem1,
            this.openOWAToolStripMenuItem,
            this.openOutlookToolStripMenuItem,
            this.dividerToolStripMenuItem,
            this.recallLastPopupToolStripMenuItem,
            this.resetTrayIconToolStripMenuItem,
            this.blankToolStripMenuItem,
            this.exitToolStripMenuItem});
			this.contextMenuStrip1.Name = "contextMenuStrip1";
			this.contextMenuStrip1.Size = new System.Drawing.Size(168, 154);
			// 
			// restoreToolStripMenuItem
			// 
			this.restoreToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("restoreToolStripMenuItem.Image")));
			this.restoreToolStripMenuItem.Name = "restoreToolStripMenuItem";
			this.restoreToolStripMenuItem.Size = new System.Drawing.Size(167, 22);
			this.restoreToolStripMenuItem.Text = "Options";
			this.restoreToolStripMenuItem.Click += new System.EventHandler(this.restoreToolStripMenuItem_Click);
			// 
			// blankToolStripMenuItem1
			// 
			this.blankToolStripMenuItem1.Name = "blankToolStripMenuItem1";
			this.blankToolStripMenuItem1.Size = new System.Drawing.Size(164, 6);
			// 
			// openOWAToolStripMenuItem
			// 
			this.openOWAToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("openOWAToolStripMenuItem.Image")));
			this.openOWAToolStripMenuItem.Name = "openOWAToolStripMenuItem";
			this.openOWAToolStripMenuItem.Size = new System.Drawing.Size(167, 22);
			this.openOWAToolStripMenuItem.Text = "Open OWA";
			this.openOWAToolStripMenuItem.Click += new System.EventHandler(this.openOWAToolStripMenuItem_Click);
			// 
			// openOutlookToolStripMenuItem
			// 
			this.openOutlookToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("openOutlookToolStripMenuItem.Image")));
			this.openOutlookToolStripMenuItem.Name = "openOutlookToolStripMenuItem";
			this.openOutlookToolStripMenuItem.Size = new System.Drawing.Size(167, 22);
			this.openOutlookToolStripMenuItem.Text = "Open Outlook";
			this.openOutlookToolStripMenuItem.Click += new System.EventHandler(this.openOutlookToolStripMenuItem_Click);
			// 
			// dividerToolStripMenuItem
			// 
			this.dividerToolStripMenuItem.Name = "dividerToolStripMenuItem";
			this.dividerToolStripMenuItem.Size = new System.Drawing.Size(164, 6);
			// 
			// recallLastPopupToolStripMenuItem
			// 
			this.recallLastPopupToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("recallLastPopupToolStripMenuItem.Image")));
			this.recallLastPopupToolStripMenuItem.Name = "recallLastPopupToolStripMenuItem";
			this.recallLastPopupToolStripMenuItem.Size = new System.Drawing.Size(167, 22);
			this.recallLastPopupToolStripMenuItem.Text = "Recall Last Popup";
			this.recallLastPopupToolStripMenuItem.Click += new System.EventHandler(this.recallLastPopupToolStripMenuItem_Click);
			// 
			// resetTrayIconToolStripMenuItem
			// 
			this.resetTrayIconToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("resetTrayIconToolStripMenuItem.Image")));
			this.resetTrayIconToolStripMenuItem.Name = "resetTrayIconToolStripMenuItem";
			this.resetTrayIconToolStripMenuItem.Size = new System.Drawing.Size(167, 22);
			this.resetTrayIconToolStripMenuItem.Text = "Reset Tray Icon";
			this.resetTrayIconToolStripMenuItem.Click += new System.EventHandler(this.resetTrayIconToolStripMenuItem_Click);
			// 
			// blankToolStripMenuItem
			// 
			this.blankToolStripMenuItem.Name = "blankToolStripMenuItem";
			this.blankToolStripMenuItem.Size = new System.Drawing.Size(164, 6);
			// 
			// exitToolStripMenuItem
			// 
			this.exitToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("exitToolStripMenuItem.Image")));
			this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
			this.exitToolStripMenuItem.Size = new System.Drawing.Size(167, 22);
			this.exitToolStripMenuItem.Text = "Exit";
			this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
			// 
			// menuStrip1
			// 
			this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.toolsToolStripMenuItem,
            this.notificationsToolStripMenuItem,
            this.advancedToolStripMenuItem,
            this.exchangeVersionToolStripMenuItem1,
            this.shellIntegrationToolStripMenuItem,
            this.helpToolStripMenuItem});
			this.menuStrip1.Location = new System.Drawing.Point(0, 0);
			this.menuStrip1.Name = "menuStrip1";
			this.menuStrip1.Size = new System.Drawing.Size(646, 24);
			this.menuStrip1.TabIndex = 0;
			this.menuStrip1.Text = "menuStrip1";
			// 
			// fileToolStripMenuItem
			// 
			this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.exitToolStripMenuItem1});
			this.fileToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("fileToolStripMenuItem.Image")));
			this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
			this.fileToolStripMenuItem.Size = new System.Drawing.Size(53, 20);
			this.fileToolStripMenuItem.Text = "File";
			// 
			// exitToolStripMenuItem1
			// 
			this.exitToolStripMenuItem1.Image = ((System.Drawing.Image)(resources.GetObject("exitToolStripMenuItem1.Image")));
			this.exitToolStripMenuItem1.Name = "exitToolStripMenuItem1";
			this.exitToolStripMenuItem1.Size = new System.Drawing.Size(92, 22);
			this.exitToolStripMenuItem1.Text = "Exit";
			this.exitToolStripMenuItem1.Click += new System.EventHandler(this.exitToolStripMenuItem1_Click);
			// 
			// toolsToolStripMenuItem
			// 
			this.toolsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.nETVersionsToolStripMenuItem,
            this.mDACVersionsToolStripMenuItem,
            this.systemInformationToolStripMenuItem});
			this.toolsToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("toolsToolStripMenuItem.Image")));
			this.toolsToolStripMenuItem.Name = "toolsToolStripMenuItem";
			this.toolsToolStripMenuItem.Size = new System.Drawing.Size(64, 20);
			this.toolsToolStripMenuItem.Text = "Tools";
			// 
			// nETVersionsToolStripMenuItem
			// 
			this.nETVersionsToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("nETVersionsToolStripMenuItem.Image")));
			this.nETVersionsToolStripMenuItem.Name = "nETVersionsToolStripMenuItem";
			this.nETVersionsToolStripMenuItem.Size = new System.Drawing.Size(178, 22);
			this.nETVersionsToolStripMenuItem.Text = ".NET Versions";
			this.nETVersionsToolStripMenuItem.Click += new System.EventHandler(this.nETVersionsToolStripMenuItem_Click);
			// 
			// mDACVersionsToolStripMenuItem
			// 
			this.mDACVersionsToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("mDACVersionsToolStripMenuItem.Image")));
			this.mDACVersionsToolStripMenuItem.Name = "mDACVersionsToolStripMenuItem";
			this.mDACVersionsToolStripMenuItem.Size = new System.Drawing.Size(178, 22);
			this.mDACVersionsToolStripMenuItem.Text = "MDAC Versions";
			this.mDACVersionsToolStripMenuItem.Click += new System.EventHandler(this.mDACVersionsToolStripMenuItem_Click);
			// 
			// systemInformationToolStripMenuItem
			// 
			this.systemInformationToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("systemInformationToolStripMenuItem.Image")));
			this.systemInformationToolStripMenuItem.Name = "systemInformationToolStripMenuItem";
			this.systemInformationToolStripMenuItem.Size = new System.Drawing.Size(178, 22);
			this.systemInformationToolStripMenuItem.Text = "System Information";
			this.systemInformationToolStripMenuItem.Click += new System.EventHandler(this.systemInformationToolStripMenuItem_Click);
			// 
			// notificationsToolStripMenuItem
			// 
			this.notificationsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.balloonToolStripMenuItem,
            this.growlToolStripMenuItem,
            this.snarlToolStripMenuItem,
            this.playSoundToolStripMenuItem});
			this.notificationsToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("notificationsToolStripMenuItem.Image")));
			this.notificationsToolStripMenuItem.Name = "notificationsToolStripMenuItem";
			this.notificationsToolStripMenuItem.Size = new System.Drawing.Size(103, 20);
			this.notificationsToolStripMenuItem.Text = "Notifications";
			// 
			// balloonToolStripMenuItem
			// 
			this.balloonToolStripMenuItem.Checked = true;
			this.balloonToolStripMenuItem.CheckOnClick = true;
			this.balloonToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
			this.balloonToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("balloonToolStripMenuItem.Image")));
			this.balloonToolStripMenuItem.Name = "balloonToolStripMenuItem";
			this.balloonToolStripMenuItem.Size = new System.Drawing.Size(133, 22);
			this.balloonToolStripMenuItem.Text = "Balloon";
			this.balloonToolStripMenuItem.CheckStateChanged += new System.EventHandler(this.balloonToolStripMenuItem_CheckStateChanged);
			// 
			// growlToolStripMenuItem
			// 
			this.growlToolStripMenuItem.CheckOnClick = true;
			this.growlToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("growlToolStripMenuItem.Image")));
			this.growlToolStripMenuItem.Name = "growlToolStripMenuItem";
			this.growlToolStripMenuItem.Size = new System.Drawing.Size(133, 22);
			this.growlToolStripMenuItem.Text = "Growl";
			this.growlToolStripMenuItem.CheckStateChanged += new System.EventHandler(this.growlToolStripMenuItem_CheckStateChanged);
			// 
			// snarlToolStripMenuItem
			// 
			this.snarlToolStripMenuItem.CheckOnClick = true;
			this.snarlToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("snarlToolStripMenuItem.Image")));
			this.snarlToolStripMenuItem.Name = "snarlToolStripMenuItem";
			this.snarlToolStripMenuItem.Size = new System.Drawing.Size(133, 22);
			this.snarlToolStripMenuItem.Text = "Snarl";
			this.snarlToolStripMenuItem.CheckStateChanged += new System.EventHandler(this.snarlToolStripMenuItem_CheckStateChanged);
			// 
			// playSoundToolStripMenuItem
			// 
			this.playSoundToolStripMenuItem.Checked = true;
			this.playSoundToolStripMenuItem.CheckOnClick = true;
			this.playSoundToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
			this.playSoundToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("playSoundToolStripMenuItem.Image")));
			this.playSoundToolStripMenuItem.Name = "playSoundToolStripMenuItem";
			this.playSoundToolStripMenuItem.Size = new System.Drawing.Size(133, 22);
			this.playSoundToolStripMenuItem.Text = "Play Sound";
			this.playSoundToolStripMenuItem.CheckStateChanged += new System.EventHandler(this.playSoundToolStripMenuItem_CheckStateChanged);
			// 
			// advancedToolStripMenuItem
			// 
			this.advancedToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.overrideToolStripMenuItem,
            this.alwaysOpenOWAInIEToolStripMenuItem,
            this.disableCalendarToolStripMenuItem,
            this.loginAutomaticallyToolStripMenuItem,
            this.overrideAutodiscoveryValidationToolStripMenuItem,
            this.office365LoginOverrideToolStripMenuItem});
			this.advancedToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("advancedToolStripMenuItem.Image")));
			this.advancedToolStripMenuItem.Name = "advancedToolStripMenuItem";
			this.advancedToolStripMenuItem.Size = new System.Drawing.Size(67, 20);
			this.advancedToolStripMenuItem.Text = "Expert";
			// 
			// overrideToolStripMenuItem
			// 
			this.overrideToolStripMenuItem.CheckOnClick = true;
			this.overrideToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("overrideToolStripMenuItem.Image")));
			this.overrideToolStripMenuItem.Name = "overrideToolStripMenuItem";
			this.overrideToolStripMenuItem.Size = new System.Drawing.Size(254, 22);
			this.overrideToolStripMenuItem.Text = "Override Certificate";
			this.overrideToolStripMenuItem.CheckStateChanged += new System.EventHandler(this.overrideToolStripMenuItem_CheckStateChanged);
			// 
			// alwaysOpenOWAInIEToolStripMenuItem
			// 
			this.alwaysOpenOWAInIEToolStripMenuItem.CheckOnClick = true;
			this.alwaysOpenOWAInIEToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("alwaysOpenOWAInIEToolStripMenuItem.Image")));
			this.alwaysOpenOWAInIEToolStripMenuItem.Name = "alwaysOpenOWAInIEToolStripMenuItem";
			this.alwaysOpenOWAInIEToolStripMenuItem.Size = new System.Drawing.Size(254, 22);
			this.alwaysOpenOWAInIEToolStripMenuItem.Text = "Always open OWA in IE";
			this.alwaysOpenOWAInIEToolStripMenuItem.CheckStateChanged += new System.EventHandler(this.alwaysOpenOWAInIEToolStripMenuItem_CheckStateChanged);
			// 
			// disableCalendarToolStripMenuItem
			// 
			this.disableCalendarToolStripMenuItem.CheckOnClick = true;
			this.disableCalendarToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("disableCalendarToolStripMenuItem.Image")));
			this.disableCalendarToolStripMenuItem.Name = "disableCalendarToolStripMenuItem";
			this.disableCalendarToolStripMenuItem.Size = new System.Drawing.Size(254, 22);
			this.disableCalendarToolStripMenuItem.Text = "Disable Calendar";
			this.disableCalendarToolStripMenuItem.CheckStateChanged += new System.EventHandler(this.disableCalendarToolStripMenuItem_CheckStateChanged);
			// 
			// loginAutomaticallyToolStripMenuItem
			// 
			this.loginAutomaticallyToolStripMenuItem.CheckOnClick = true;
			this.loginAutomaticallyToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("loginAutomaticallyToolStripMenuItem.Image")));
			this.loginAutomaticallyToolStripMenuItem.Name = "loginAutomaticallyToolStripMenuItem";
			this.loginAutomaticallyToolStripMenuItem.Size = new System.Drawing.Size(254, 22);
			this.loginAutomaticallyToolStripMenuItem.Text = "Login Automatically";
			this.loginAutomaticallyToolStripMenuItem.CheckStateChanged += new System.EventHandler(this.loginAutomaticallyToolStripMenuItem_CheckStateChanged);
			// 
			// overrideAutodiscoveryValidationToolStripMenuItem
			// 
			this.overrideAutodiscoveryValidationToolStripMenuItem.CheckOnClick = true;
			this.overrideAutodiscoveryValidationToolStripMenuItem.Image = global::DrunkenBakery.OWAtray.Properties.Resources.apply;
			this.overrideAutodiscoveryValidationToolStripMenuItem.Name = "overrideAutodiscoveryValidationToolStripMenuItem";
			this.overrideAutodiscoveryValidationToolStripMenuItem.Size = new System.Drawing.Size(254, 22);
			this.overrideAutodiscoveryValidationToolStripMenuItem.Text = "Override Autodiscovery Validation";
			this.overrideAutodiscoveryValidationToolStripMenuItem.CheckStateChanged += new System.EventHandler(this.overrideAutodiscoveryValidationToolStripMenuItem_CheckStateChanged);
			// 
			// office365LoginOverrideToolStripMenuItem
			// 
			this.office365LoginOverrideToolStripMenuItem.CheckOnClick = true;
			this.office365LoginOverrideToolStripMenuItem.Image = global::DrunkenBakery.OWAtray.Properties.Resources.office;
			this.office365LoginOverrideToolStripMenuItem.Name = "office365LoginOverrideToolStripMenuItem";
			this.office365LoginOverrideToolStripMenuItem.Size = new System.Drawing.Size(254, 22);
			this.office365LoginOverrideToolStripMenuItem.Text = "Office365 Login Override";
			this.office365LoginOverrideToolStripMenuItem.CheckStateChanged += new System.EventHandler(this.office365LoginOverrideToolStripMenuItem_CheckStateChanged);
			// 
			// exchangeVersionToolStripMenuItem1
			// 
			this.exchangeVersionToolStripMenuItem1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.exchange2007ToolStripMenuItem});
			this.exchangeVersionToolStripMenuItem1.Image = ((System.Drawing.Image)(resources.GetObject("exchangeVersionToolStripMenuItem1.Image")));
			this.exchangeVersionToolStripMenuItem1.Name = "exchangeVersionToolStripMenuItem1";
			this.exchangeVersionToolStripMenuItem1.Size = new System.Drawing.Size(127, 20);
			this.exchangeVersionToolStripMenuItem1.Text = "Exchange Version";
			// 
			// exchange2007ToolStripMenuItem
			// 
			this.exchange2007ToolStripMenuItem.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.exchange2007ToolStripMenuItem.Items.AddRange(new object[] {
            "Autodetect",
            "Exchange 2007 SP1",
            "Exchange 2010",
            "Exchange 2010 SP1"});
			this.exchange2007ToolStripMenuItem.Name = "exchange2007ToolStripMenuItem";
			this.exchange2007ToolStripMenuItem.Size = new System.Drawing.Size(152, 23);
			this.exchange2007ToolStripMenuItem.SelectedIndexChanged += new System.EventHandler(this.exchange2007ToolStripMenuItem_SelectedIndexChanged);
			// 
			// shellIntegrationToolStripMenuItem
			// 
			this.shellIntegrationToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.makeOWADefaultToolStripMenuItem,
            this.switchOffToolStripMenuItem});
			this.shellIntegrationToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("shellIntegrationToolStripMenuItem.Image")));
			this.shellIntegrationToolStripMenuItem.Name = "shellIntegrationToolStripMenuItem";
			this.shellIntegrationToolStripMenuItem.Size = new System.Drawing.Size(121, 20);
			this.shellIntegrationToolStripMenuItem.Text = "Shell Integration";
			// 
			// makeOWADefaultToolStripMenuItem
			// 
			this.makeOWADefaultToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("makeOWADefaultToolStripMenuItem.Image")));
			this.makeOWADefaultToolStripMenuItem.Name = "makeOWADefaultToolStripMenuItem";
			this.makeOWADefaultToolStripMenuItem.Size = new System.Drawing.Size(175, 22);
			this.makeOWADefaultToolStripMenuItem.Text = "Make OWA Default";
			this.makeOWADefaultToolStripMenuItem.Click += new System.EventHandler(this.makeOWADefaultToolStripMenuItem_Click);
			// 
			// switchOffToolStripMenuItem
			// 
			this.switchOffToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("switchOffToolStripMenuItem.Image")));
			this.switchOffToolStripMenuItem.Name = "switchOffToolStripMenuItem";
			this.switchOffToolStripMenuItem.Size = new System.Drawing.Size(175, 22);
			this.switchOffToolStripMenuItem.Text = "Switch Off";
			this.switchOffToolStripMenuItem.Click += new System.EventHandler(this.switchOffToolStripMenuItem_Click);
			// 
			// helpToolStripMenuItem
			// 
			this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.changeLogToolStripMenuItem,
            this.supportToolStripMenuItem,
            this.aboutToolStripMenuItem});
			this.helpToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("helpToolStripMenuItem.Image")));
			this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
			this.helpToolStripMenuItem.Size = new System.Drawing.Size(60, 20);
			this.helpToolStripMenuItem.Text = "Help";
			// 
			// changeLogToolStripMenuItem
			// 
			this.changeLogToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("changeLogToolStripMenuItem.Image")));
			this.changeLogToolStripMenuItem.Name = "changeLogToolStripMenuItem";
			this.changeLogToolStripMenuItem.Size = new System.Drawing.Size(138, 22);
			this.changeLogToolStripMenuItem.Text = "Change Log";
			this.changeLogToolStripMenuItem.Click += new System.EventHandler(this.changeLogToolStripMenuItem_Click);
			// 
			// supportToolStripMenuItem
			// 
			this.supportToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("supportToolStripMenuItem.Image")));
			this.supportToolStripMenuItem.Name = "supportToolStripMenuItem";
			this.supportToolStripMenuItem.Size = new System.Drawing.Size(138, 22);
			this.supportToolStripMenuItem.Text = "Support";
			this.supportToolStripMenuItem.Click += new System.EventHandler(this.supportToolStripMenuItem_Click);
			// 
			// aboutToolStripMenuItem
			// 
			this.aboutToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("aboutToolStripMenuItem.Image")));
			this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
			this.aboutToolStripMenuItem.Size = new System.Drawing.Size(138, 22);
			this.aboutToolStripMenuItem.Text = "About";
			this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
			// 
			// groupBox3
			// 
			this.groupBox3.Controls.Add(this.pictureBox4);
			this.groupBox3.Controls.Add(this.lvStatus);
			this.groupBox3.Location = new System.Drawing.Point(12, 199);
			this.groupBox3.Name = "groupBox3";
			this.groupBox3.Size = new System.Drawing.Size(622, 210);
			this.groupBox3.TabIndex = 3;
			this.groupBox3.TabStop = false;
			this.groupBox3.Text = "Logging";
			// 
			// pictureBox4
			// 
			this.pictureBox4.Image = global::DrunkenBakery.OWAtray.Properties.Resources.bookmark;
			this.pictureBox4.Location = new System.Drawing.Point(22, 30);
			this.pictureBox4.Name = "pictureBox4";
			this.pictureBox4.Size = new System.Drawing.Size(16, 16);
			this.pictureBox4.TabIndex = 62;
			this.pictureBox4.TabStop = false;
			// 
			// lvStatus
			// 
			this.lvStatus.GridLines = true;
			this.lvStatus.Location = new System.Drawing.Point(68, 19);
			this.lvStatus.Name = "lvStatus";
			this.lvStatus.Size = new System.Drawing.Size(548, 185);
			this.lvStatus.SmallImageList = this.imLV;
			this.lvStatus.TabIndex = 0;
			this.lvStatus.UseCompatibleStateImageBehavior = false;
			this.lvStatus.View = System.Windows.Forms.View.Details;
			// 
			// lblUrl
			// 
			this.lblUrl.AutoSize = true;
			this.lblUrl.ForeColor = System.Drawing.Color.Black;
			this.lblUrl.Location = new System.Drawing.Point(115, 61);
			this.lblUrl.Name = "lblUrl";
			this.lblUrl.Size = new System.Drawing.Size(50, 13);
			this.lblUrl.TabIndex = 3;
			this.lblUrl.Text = "unknown";
			// 
			// txtURLEdit
			// 
			this.txtURLEdit.Location = new System.Drawing.Point(158, 110);
			this.txtURLEdit.Name = "txtURLEdit";
			this.txtURLEdit.Size = new System.Drawing.Size(362, 21);
			this.txtURLEdit.TabIndex = 11;
			this.txtURLEdit.Validated += new System.EventHandler(this.txtURLEdit_Validated);
			// 
			// txtInterval
			// 
			this.txtInterval.Location = new System.Drawing.Point(158, 47);
			this.txtInterval.Name = "txtInterval";
			this.txtInterval.Size = new System.Drawing.Size(80, 21);
			this.txtInterval.TabIndex = 3;
			this.txtInterval.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.txtInterval.Validating += new System.ComponentModel.CancelEventHandler(this.txtInterval_Validating);
			this.txtInterval.Validated += new System.EventHandler(this.txtInterval_Validated);
			// 
			// txtDomain
			// 
			this.txtDomain.Location = new System.Drawing.Point(386, 47);
			this.txtDomain.Name = "txtDomain";
			this.txtDomain.Size = new System.Drawing.Size(134, 21);
			this.txtDomain.TabIndex = 5;
			this.txtDomain.Validated += new System.EventHandler(this.txtDomain_Validated);
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.ForeColor = System.Drawing.Color.Blue;
			this.label5.Location = new System.Drawing.Point(282, 50);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(98, 13);
			this.label5.TabIndex = 4;
			this.label5.Text = "Windows Domain : ";
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.ForeColor = System.Drawing.Color.Blue;
			this.label4.Location = new System.Drawing.Point(63, 113);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(61, 13);
			this.label4.TabIndex = 10;
			this.label4.Text = "EWS URL : ";
			// 
			// txtServer
			// 
			this.txtServer.Location = new System.Drawing.Point(158, 17);
			this.txtServer.Name = "txtServer";
			this.txtServer.Size = new System.Drawing.Size(362, 21);
			this.txtServer.TabIndex = 1;
			this.txtServer.Validated += new System.EventHandler(this.txtServer_Validated);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.ForeColor = System.Drawing.Color.Blue;
			this.label1.Location = new System.Drawing.Point(61, 20);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(91, 13);
			this.label1.TabIndex = 0;
			this.label1.Text = "Server Address : ";
			// 
			// txtEmail
			// 
			this.txtEmail.Location = new System.Drawing.Point(150, 35);
			this.txtEmail.Name = "txtEmail";
			this.txtEmail.Size = new System.Drawing.Size(366, 21);
			this.txtEmail.TabIndex = 2;
			this.txtEmail.Validated += new System.EventHandler(this.txtEmail_Validated);
			// 
			// chkOnDomain
			// 
			this.chkOnDomain.AutoSize = true;
			this.chkOnDomain.ForeColor = System.Drawing.Color.Blue;
			this.chkOnDomain.Location = new System.Drawing.Point(64, 66);
			this.chkOnDomain.Name = "chkOnDomain";
			this.chkOnDomain.Size = new System.Drawing.Size(185, 17);
			this.chkOnDomain.TabIndex = 3;
			this.chkOnDomain.Text = "Use Windows Domain Credentials";
			this.chkOnDomain.UseVisualStyleBackColor = true;
			this.chkOnDomain.CheckedChanged += new System.EventHandler(this.chkOnDomain_CheckedChanged);
			// 
			// cmdStop
			// 
			this.cmdStop.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.cmdStop.Location = new System.Drawing.Point(6, 51);
			this.cmdStop.Name = "cmdStop";
			this.cmdStop.Size = new System.Drawing.Size(57, 23);
			this.cmdStop.TabIndex = 1;
			this.cmdStop.Text = "Sto&p";
			this.cmdStop.UseVisualStyleBackColor = true;
			this.cmdStop.Click += new System.EventHandler(this.cmdStop_Click);
			// 
			// cmdStart
			// 
			this.cmdStart.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.cmdStart.Location = new System.Drawing.Point(6, 22);
			this.cmdStart.Name = "cmdStart";
			this.cmdStart.Size = new System.Drawing.Size(57, 23);
			this.cmdStart.TabIndex = 0;
			this.cmdStart.Text = "&Start";
			this.cmdStart.UseVisualStyleBackColor = true;
			this.cmdStart.Click += new System.EventHandler(this.cmdStart_Click);
			// 
			// txtPwd
			// 
			this.txtPwd.Location = new System.Drawing.Point(150, 113);
			this.txtPwd.Name = "txtPwd";
			this.txtPwd.PasswordChar = '*';
			this.txtPwd.Size = new System.Drawing.Size(366, 21);
			this.txtPwd.TabIndex = 8;
			this.txtPwd.Validated += new System.EventHandler(this.txtPwd_Validated);
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.ForeColor = System.Drawing.Color.Blue;
			this.label3.Location = new System.Drawing.Point(61, 116);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(63, 13);
			this.label3.TabIndex = 7;
			this.label3.Text = "Password : ";
			// 
			// txtUser
			// 
			this.txtUser.Location = new System.Drawing.Point(150, 87);
			this.txtUser.Name = "txtUser";
			this.txtUser.Size = new System.Drawing.Size(366, 21);
			this.txtUser.TabIndex = 6;
			this.txtUser.Validated += new System.EventHandler(this.txtUser_Validated);
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.ForeColor = System.Drawing.Color.Blue;
			this.label2.Location = new System.Drawing.Point(61, 90);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(39, 13);
			this.label2.TabIndex = 5;
			this.label2.Text = "User : ";
			// 
			// timer1
			// 
			this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
			// 
			// timerLogging
			// 
			this.timerLogging.Interval = 500;
			this.timerLogging.Tick += new System.EventHandler(this.timerLogging_Tick);
			// 
			// timerUpdate
			// 
			this.timerUpdate.Tick += new System.EventHandler(this.timerUpdate_Tick);
			// 
			// timerAppt
			// 
			this.timerAppt.Tick += new System.EventHandler(this.timerAppt_Tick);
			// 
			// errorProvider1
			// 
			this.errorProvider1.ContainerControl = this;
			// 
			// tabMain
			// 
			this.tabMain.Controls.Add(this.tabPage1);
			this.tabMain.Controls.Add(this.tabPage2);
			this.tabMain.Controls.Add(this.tabPage3);
			this.tabMain.Location = new System.Drawing.Point(80, 27);
			this.tabMain.Name = "tabMain";
			this.tabMain.SelectedIndex = 0;
			this.tabMain.Size = new System.Drawing.Size(548, 166);
			this.tabMain.TabIndex = 2;
			// 
			// tabPage1
			// 
			this.tabPage1.Controls.Add(this.chkRunOnStartup);
			this.tabPage1.Controls.Add(this.pictureBox14);
			this.tabPage1.Controls.Add(this.chkOnDomain);
			this.tabPage1.Controls.Add(this.txtEmail);
			this.tabPage1.Controls.Add(this.pictureBox15);
			this.tabPage1.Controls.Add(this.label9);
			this.tabPage1.Controls.Add(this.chkAutodiscovery);
			this.tabPage1.Controls.Add(this.pictureBox3);
			this.tabPage1.Controls.Add(this.txtPwd);
			this.tabPage1.Controls.Add(this.label3);
			this.tabPage1.Controls.Add(this.txtUser);
			this.tabPage1.Controls.Add(this.label2);
			this.tabPage1.Location = new System.Drawing.Point(4, 22);
			this.tabPage1.Name = "tabPage1";
			this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
			this.tabPage1.Size = new System.Drawing.Size(540, 140);
			this.tabPage1.TabIndex = 0;
			this.tabPage1.Text = "Basic Settings";
			this.tabPage1.UseVisualStyleBackColor = true;
			// 
			// chkRunOnStartup
			// 
			this.chkRunOnStartup.AutoSize = true;
			this.chkRunOnStartup.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.chkRunOnStartup.ForeColor = System.Drawing.Color.Blue;
			this.chkRunOnStartup.Location = new System.Drawing.Point(419, 64);
			this.chkRunOnStartup.Name = "chkRunOnStartup";
			this.chkRunOnStartup.Size = new System.Drawing.Size(97, 17);
			this.chkRunOnStartup.TabIndex = 4;
			this.chkRunOnStartup.Text = "Run at Startup";
			this.chkRunOnStartup.UseVisualStyleBackColor = true;
			this.chkRunOnStartup.CheckedChanged += new System.EventHandler(this.chkRunOnStartup_CheckedChanged);
			// 
			// pictureBox14
			// 
			this.pictureBox14.Image = global::DrunkenBakery.OWAtray.Properties.Resources.unlock;
			this.pictureBox14.Location = new System.Drawing.Point(24, 67);
			this.pictureBox14.Name = "pictureBox14";
			this.pictureBox14.Size = new System.Drawing.Size(16, 16);
			this.pictureBox14.TabIndex = 61;
			this.pictureBox14.TabStop = false;
			// 
			// pictureBox15
			// 
			this.pictureBox15.Image = global::DrunkenBakery.OWAtray.Properties.Resources.mail;
			this.pictureBox15.Location = new System.Drawing.Point(24, 38);
			this.pictureBox15.Name = "pictureBox15";
			this.pictureBox15.Size = new System.Drawing.Size(16, 16);
			this.pictureBox15.TabIndex = 60;
			this.pictureBox15.TabStop = false;
			// 
			// label9
			// 
			this.label9.AutoSize = true;
			this.label9.ForeColor = System.Drawing.Color.Blue;
			this.label9.Location = new System.Drawing.Point(61, 38);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(83, 13);
			this.label9.TabIndex = 1;
			this.label9.Text = "Email Address : ";
			// 
			// chkAutodiscovery
			// 
			this.chkAutodiscovery.AutoSize = true;
			this.chkAutodiscovery.ForeColor = System.Drawing.Color.Blue;
			this.chkAutodiscovery.Location = new System.Drawing.Point(64, 10);
			this.chkAutodiscovery.Name = "chkAutodiscovery";
			this.chkAutodiscovery.Size = new System.Drawing.Size(170, 17);
			this.chkAutodiscovery.TabIndex = 0;
			this.chkAutodiscovery.Text = "Attempt to use Autodiscovery";
			this.chkAutodiscovery.UseVisualStyleBackColor = true;
			this.chkAutodiscovery.CheckedChanged += new System.EventHandler(this.chkAutodiscovery_CheckedChanged);
			// 
			// pictureBox3
			// 
			this.pictureBox3.Image = global::DrunkenBakery.OWAtray.Properties.Resources.star;
			this.pictureBox3.Location = new System.Drawing.Point(24, 10);
			this.pictureBox3.Name = "pictureBox3";
			this.pictureBox3.Size = new System.Drawing.Size(16, 16);
			this.pictureBox3.TabIndex = 45;
			this.pictureBox3.TabStop = false;
			// 
			// tabPage2
			// 
			this.tabPage2.Controls.Add(this.cbOverrideOWA);
			this.tabPage2.Controls.Add(this.txtOWAEdit);
			this.tabPage2.Controls.Add(this.pictureBox5);
			this.tabPage2.Controls.Add(this.label7);
			this.tabPage2.Controls.Add(this.cbOverrideEWS);
			this.tabPage2.Controls.Add(this.txtURLEdit);
			this.tabPage2.Controls.Add(this.pictureBox2);
			this.tabPage2.Controls.Add(this.label4);
			this.tabPage2.Controls.Add(this.pictureBox17);
			this.tabPage2.Controls.Add(this.txtDomain);
			this.tabPage2.Controls.Add(this.txtInterval);
			this.tabPage2.Controls.Add(this.label5);
			this.tabPage2.Controls.Add(this.label8);
			this.tabPage2.Controls.Add(this.pictureBox16);
			this.tabPage2.Controls.Add(this.pictureBox1);
			this.tabPage2.Controls.Add(this.txtServer);
			this.tabPage2.Controls.Add(this.label1);
			this.tabPage2.Location = new System.Drawing.Point(4, 22);
			this.tabPage2.Name = "tabPage2";
			this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
			this.tabPage2.Size = new System.Drawing.Size(540, 140);
			this.tabPage2.TabIndex = 1;
			this.tabPage2.Text = "Advanced Settings";
			this.tabPage2.UseVisualStyleBackColor = true;
			// 
			// cbOverrideOWA
			// 
			this.cbOverrideOWA.AutoSize = true;
			this.cbOverrideOWA.Location = new System.Drawing.Point(46, 82);
			this.cbOverrideOWA.Name = "cbOverrideOWA";
			this.cbOverrideOWA.Size = new System.Drawing.Size(15, 14);
			this.cbOverrideOWA.TabIndex = 6;
			this.cbOverrideOWA.UseVisualStyleBackColor = true;
			this.cbOverrideOWA.CheckedChanged += new System.EventHandler(this.cbOverrideOWA_CheckedChanged);
			// 
			// txtOWAEdit
			// 
			this.txtOWAEdit.Location = new System.Drawing.Point(158, 79);
			this.txtOWAEdit.Name = "txtOWAEdit";
			this.txtOWAEdit.Size = new System.Drawing.Size(362, 21);
			this.txtOWAEdit.TabIndex = 8;
			this.txtOWAEdit.Validated += new System.EventHandler(this.txtOWAEdit_Validated);
			// 
			// pictureBox5
			// 
			this.pictureBox5.Image = global::DrunkenBakery.OWAtray.Properties.Resources.exchange;
			this.pictureBox5.Location = new System.Drawing.Point(24, 80);
			this.pictureBox5.Name = "pictureBox5";
			this.pictureBox5.Size = new System.Drawing.Size(16, 16);
			this.pictureBox5.TabIndex = 73;
			this.pictureBox5.TabStop = false;
			// 
			// label7
			// 
			this.label7.AutoSize = true;
			this.label7.ForeColor = System.Drawing.Color.Blue;
			this.label7.Location = new System.Drawing.Point(63, 82);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(64, 13);
			this.label7.TabIndex = 7;
			this.label7.Text = "OWA URL : ";
			// 
			// cbOverrideEWS
			// 
			this.cbOverrideEWS.AutoSize = true;
			this.cbOverrideEWS.Location = new System.Drawing.Point(46, 114);
			this.cbOverrideEWS.Name = "cbOverrideEWS";
			this.cbOverrideEWS.Size = new System.Drawing.Size(15, 14);
			this.cbOverrideEWS.TabIndex = 9;
			this.cbOverrideEWS.UseVisualStyleBackColor = true;
			this.cbOverrideEWS.CheckedChanged += new System.EventHandler(this.cbOverrideEWS_CheckedChanged);
			// 
			// pictureBox2
			// 
			this.pictureBox2.Image = global::DrunkenBakery.OWAtray.Properties.Resources.exchange;
			this.pictureBox2.Location = new System.Drawing.Point(24, 112);
			this.pictureBox2.Name = "pictureBox2";
			this.pictureBox2.Size = new System.Drawing.Size(16, 16);
			this.pictureBox2.TabIndex = 69;
			this.pictureBox2.TabStop = false;
			// 
			// pictureBox17
			// 
			this.pictureBox17.Image = global::DrunkenBakery.OWAtray.Properties.Resources.win;
			this.pictureBox17.Location = new System.Drawing.Point(260, 50);
			this.pictureBox17.Name = "pictureBox17";
			this.pictureBox17.Size = new System.Drawing.Size(16, 16);
			this.pictureBox17.TabIndex = 68;
			this.pictureBox17.TabStop = false;
			// 
			// label8
			// 
			this.label8.AutoSize = true;
			this.label8.ForeColor = System.Drawing.Color.Blue;
			this.label8.Location = new System.Drawing.Point(61, 50);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(93, 13);
			this.label8.TabIndex = 2;
			this.label8.Text = "Update Interval : ";
			// 
			// pictureBox16
			// 
			this.pictureBox16.Image = global::DrunkenBakery.OWAtray.Properties.Resources.history;
			this.pictureBox16.Location = new System.Drawing.Point(24, 50);
			this.pictureBox16.Name = "pictureBox16";
			this.pictureBox16.Size = new System.Drawing.Size(16, 16);
			this.pictureBox16.TabIndex = 65;
			this.pictureBox16.TabStop = false;
			// 
			// pictureBox1
			// 
			this.pictureBox1.Image = global::DrunkenBakery.OWAtray.Properties.Resources.computer;
			this.pictureBox1.Location = new System.Drawing.Point(24, 17);
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.Size = new System.Drawing.Size(16, 16);
			this.pictureBox1.TabIndex = 46;
			this.pictureBox1.TabStop = false;
			// 
			// tabPage3
			// 
			this.tabPage3.Controls.Add(this.lblOWAUrl);
			this.tabPage3.Controls.Add(this.label13);
			this.tabPage3.Controls.Add(this.lblEmail);
			this.tabPage3.Controls.Add(this.label6);
			this.tabPage3.Controls.Add(this.label10);
			this.tabPage3.Controls.Add(this.lblUrl);
			this.tabPage3.Location = new System.Drawing.Point(4, 22);
			this.tabPage3.Name = "tabPage3";
			this.tabPage3.Size = new System.Drawing.Size(540, 140);
			this.tabPage3.TabIndex = 2;
			this.tabPage3.Text = "URLs";
			this.tabPage3.UseVisualStyleBackColor = true;
			// 
			// lblOWAUrl
			// 
			this.lblOWAUrl.AutoSize = true;
			this.lblOWAUrl.ForeColor = System.Drawing.Color.Black;
			this.lblOWAUrl.Location = new System.Drawing.Point(115, 82);
			this.lblOWAUrl.Name = "lblOWAUrl";
			this.lblOWAUrl.Size = new System.Drawing.Size(50, 13);
			this.lblOWAUrl.TabIndex = 5;
			this.lblOWAUrl.Text = "unknown";
			// 
			// label13
			// 
			this.label13.AutoSize = true;
			this.label13.ForeColor = System.Drawing.Color.Blue;
			this.label13.Location = new System.Drawing.Point(24, 82);
			this.label13.Name = "label13";
			this.label13.Size = new System.Drawing.Size(64, 13);
			this.label13.TabIndex = 4;
			this.label13.Text = "OWA URL : ";
			// 
			// lblEmail
			// 
			this.lblEmail.AutoSize = true;
			this.lblEmail.ForeColor = System.Drawing.Color.Black;
			this.lblEmail.Location = new System.Drawing.Point(115, 40);
			this.lblEmail.Name = "lblEmail";
			this.lblEmail.Size = new System.Drawing.Size(50, 13);
			this.lblEmail.TabIndex = 1;
			this.lblEmail.Text = "unknown";
			// 
			// label6
			// 
			this.label6.AutoSize = true;
			this.label6.ForeColor = System.Drawing.Color.Blue;
			this.label6.Location = new System.Drawing.Point(24, 40);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(83, 13);
			this.label6.TabIndex = 0;
			this.label6.Text = "Email Address : ";
			// 
			// label10
			// 
			this.label10.AutoSize = true;
			this.label10.ForeColor = System.Drawing.Color.Blue;
			this.label10.Location = new System.Drawing.Point(24, 61);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size(61, 13);
			this.label10.TabIndex = 2;
			this.label10.Text = "EWS URL : ";
			// 
			// groupBox4
			// 
			this.groupBox4.Controls.Add(this.cmdStart);
			this.groupBox4.Controls.Add(this.cmdStop);
			this.groupBox4.Location = new System.Drawing.Point(4, 27);
			this.groupBox4.Name = "groupBox4";
			this.groupBox4.Size = new System.Drawing.Size(70, 166);
			this.groupBox4.TabIndex = 1;
			this.groupBox4.TabStop = false;
			this.groupBox4.Text = "Controls";
			// 
			// Form1
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(646, 443);
			this.Controls.Add(this.groupBox3);
			this.Controls.Add(this.statusStrip1);
			this.Controls.Add(this.groupBox4);
			this.Controls.Add(this.tabMain);
			this.Controls.Add(this.menuStrip1);
			this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "Form1";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "OWA Tray Monitor";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
			this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Form1_FormClosed);
			this.Move += new System.EventHandler(this.Form1_Move);
			this.statusStrip1.ResumeLayout(false);
			this.statusStrip1.PerformLayout();
			this.contextMenuStrip1.ResumeLayout(false);
			this.menuStrip1.ResumeLayout(false);
			this.menuStrip1.PerformLayout();
			this.groupBox3.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.pictureBox4)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
			this.tabMain.ResumeLayout(false);
			this.tabPage1.ResumeLayout(false);
			this.tabPage1.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox14)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox15)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).EndInit();
			this.tabPage2.ResumeLayout(false);
			this.tabPage2.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox5)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox17)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox16)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
			this.tabPage3.ResumeLayout(false);
			this.tabPage3.PerformLayout();
			this.groupBox4.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

        }
        #endregion

        private System.Windows.Forms.ImageList imLV;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel slStatus;
        private System.Windows.Forms.NotifyIcon notifyIcon1;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem restoreToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem toolsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem nETVersionsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mDACVersionsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem systemInformationToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.ListView lvStatus;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtPwd;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtUser;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtServer;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblUrl;
        private System.Windows.Forms.TextBox txtDomain;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button cmdStart;
        private System.Windows.Forms.Button cmdStop;
        private System.Windows.Forms.TextBox txtInterval;
        private System.Windows.Forms.ToolStripMenuItem openOWAToolStripMenuItem;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.ToolStripMenuItem notificationsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem balloonToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem growlToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem snarlToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem supportToolStripMenuItem;
        private System.Windows.Forms.CheckBox chkOnDomain;
        private System.Windows.Forms.ToolStripMenuItem playSoundToolStripMenuItem;
        private System.Windows.Forms.Timer timerLogging;
        private System.Windows.Forms.Timer timerUpdate;
        private System.Windows.Forms.ToolStripMenuItem advancedToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem overrideToolStripMenuItem;
        private System.Windows.Forms.TextBox txtURLEdit;
        private System.Windows.Forms.ToolStripMenuItem changeLogToolStripMenuItem;
        private System.Windows.Forms.Timer timerAppt;
        private System.Windows.Forms.ToolStripMenuItem alwaysOpenOWAInIEToolStripMenuItem;
        private System.Windows.Forms.ErrorProvider errorProvider1;
        private System.Windows.Forms.ToolStripMenuItem recallLastPopupToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem resetTrayIconToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openOutlookToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator blankToolStripMenuItem1;
        private System.Windows.Forms.ToolStripSeparator blankToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exchangeVersionToolStripMenuItem1;
        private System.Windows.Forms.ToolStripComboBox exchange2007ToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator dividerToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem shellIntegrationToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem makeOWADefaultToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem switchOffToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem disableCalendarToolStripMenuItem;
        private System.Windows.Forms.TextBox txtEmail;
        private System.Windows.Forms.ToolStripMenuItem loginAutomaticallyToolStripMenuItem;
        private System.Windows.Forms.TabControl tabMain;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.CheckBox chkAutodiscovery;
        private System.Windows.Forms.PictureBox pictureBox3;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.PictureBox pictureBox14;
        private System.Windows.Forms.PictureBox pictureBox15;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.PictureBox pictureBox17;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.PictureBox pictureBox16;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.Label lblOWAUrl;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label lblEmail;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.ToolStripMenuItem overrideAutodiscoveryValidationToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem office365LoginOverrideToolStripMenuItem;
        private System.Windows.Forms.PictureBox pictureBox4;
        private System.Windows.Forms.CheckBox cbOverrideEWS;
        private System.Windows.Forms.CheckBox chkRunOnStartup;
        private System.Windows.Forms.CheckBox cbOverrideOWA;
        private System.Windows.Forms.TextBox txtOWAEdit;
        private System.Windows.Forms.PictureBox pictureBox5;
        private System.Windows.Forms.Label label7;
    }
}

