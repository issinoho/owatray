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
            this.overrideServerURLToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.alwaysOpenOWAInIEToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.disableCalendarToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
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
            this.lvStatus = new System.Windows.Forms.ListView();
            this.button3 = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label7 = new System.Windows.Forms.Label();
            this.txtEmail = new System.Windows.Forms.TextBox();
            this.txtURLEdit = new System.Windows.Forms.TextBox();
            this.chkOnDomain = new System.Windows.Forms.CheckBox();
            this.chkRunOnStartup = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label6 = new System.Windows.Forms.Label();
            this.txtInterval = new System.Windows.Forms.TextBox();
            this.cmdSave = new System.Windows.Forms.Button();
            this.cmdStop = new System.Windows.Forms.Button();
            this.cmdStart = new System.Windows.Forms.Button();
            this.txtDomain = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.txtURL = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.txtPwd = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtUser = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtServer = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.cmdForce = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.timerLogging = new System.Windows.Forms.Timer(this.components);
            this.timerUpdate = new System.Windows.Forms.Timer(this.components);
            this.timerAppt = new System.Windows.Forms.Timer(this.components);
            this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
            this.statusStrip1.SuspendLayout();
            this.contextMenuStrip1.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
            this.SuspendLayout();
            // 
            // imLV
            // 
            this.imLV.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imLV.ImageStream")));
            this.imLV.TransparentColor = System.Drawing.Color.Transparent;
            this.imLV.Images.SetKeyName(0, "tick.png");
            this.imLV.Images.SetKeyName(1, "error.png");
            this.imLV.Images.SetKeyName(2, "information.png");
            this.imLV.Images.SetKeyName(3, "email.ico");
            this.imLV.Images.SetKeyName(4, "comment_rect.ico");
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.slStatus});
            this.statusStrip1.Location = new System.Drawing.Point(0, 421);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(646, 22);
            this.statusStrip1.TabIndex = 7;
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
            this.menuStrip1.TabIndex = 8;
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
            this.overrideServerURLToolStripMenuItem,
            this.alwaysOpenOWAInIEToolStripMenuItem,
            this.disableCalendarToolStripMenuItem});
            this.advancedToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("advancedToolStripMenuItem.Image")));
            this.advancedToolStripMenuItem.Name = "advancedToolStripMenuItem";
            this.advancedToolStripMenuItem.Size = new System.Drawing.Size(88, 20);
            this.advancedToolStripMenuItem.Text = "Advanced";
            // 
            // overrideToolStripMenuItem
            // 
            this.overrideToolStripMenuItem.CheckOnClick = true;
            this.overrideToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("overrideToolStripMenuItem.Image")));
            this.overrideToolStripMenuItem.Name = "overrideToolStripMenuItem";
            this.overrideToolStripMenuItem.Size = new System.Drawing.Size(197, 22);
            this.overrideToolStripMenuItem.Text = "Override Certificate";
            this.overrideToolStripMenuItem.CheckStateChanged += new System.EventHandler(this.overrideToolStripMenuItem_CheckStateChanged);
            // 
            // overrideServerURLToolStripMenuItem
            // 
            this.overrideServerURLToolStripMenuItem.CheckOnClick = true;
            this.overrideServerURLToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("overrideServerURLToolStripMenuItem.Image")));
            this.overrideServerURLToolStripMenuItem.Name = "overrideServerURLToolStripMenuItem";
            this.overrideServerURLToolStripMenuItem.Size = new System.Drawing.Size(197, 22);
            this.overrideServerURLToolStripMenuItem.Text = "Override Server URL";
            this.overrideServerURLToolStripMenuItem.CheckStateChanged += new System.EventHandler(this.overrideServerURLToolStripMenuItem_CheckStateChanged);
            // 
            // alwaysOpenOWAInIEToolStripMenuItem
            // 
            this.alwaysOpenOWAInIEToolStripMenuItem.CheckOnClick = true;
            this.alwaysOpenOWAInIEToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("alwaysOpenOWAInIEToolStripMenuItem.Image")));
            this.alwaysOpenOWAInIEToolStripMenuItem.Name = "alwaysOpenOWAInIEToolStripMenuItem";
            this.alwaysOpenOWAInIEToolStripMenuItem.Size = new System.Drawing.Size(197, 22);
            this.alwaysOpenOWAInIEToolStripMenuItem.Text = "Always open OWA in IE";
            this.alwaysOpenOWAInIEToolStripMenuItem.CheckStateChanged += new System.EventHandler(this.alwaysOpenOWAInIEToolStripMenuItem_CheckStateChanged);
            // 
            // disableCalendarToolStripMenuItem
            // 
            this.disableCalendarToolStripMenuItem.CheckOnClick = true;
            this.disableCalendarToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("disableCalendarToolStripMenuItem.Image")));
            this.disableCalendarToolStripMenuItem.Name = "disableCalendarToolStripMenuItem";
            this.disableCalendarToolStripMenuItem.Size = new System.Drawing.Size(197, 22);
            this.disableCalendarToolStripMenuItem.Text = "Disable Calendar";
            this.disableCalendarToolStripMenuItem.CheckStateChanged += new System.EventHandler(this.disableCalendarToolStripMenuItem_CheckStateChanged);
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
            this.groupBox3.Controls.Add(this.lvStatus);
            this.groupBox3.Controls.Add(this.button3);
            this.groupBox3.Location = new System.Drawing.Point(12, 180);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(622, 229);
            this.groupBox3.TabIndex = 9;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Logging";
            // 
            // lvStatus
            // 
            this.lvStatus.GridLines = true;
            this.lvStatus.Location = new System.Drawing.Point(68, 19);
            this.lvStatus.Name = "lvStatus";
            this.lvStatus.Size = new System.Drawing.Size(548, 204);
            this.lvStatus.SmallImageList = this.imLV;
            this.lvStatus.TabIndex = 5;
            this.lvStatus.UseCompatibleStateImageBehavior = false;
            this.lvStatus.View = System.Windows.Forms.View.Details;
            // 
            // button3
            // 
            this.button3.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button3.ForeColor = System.Drawing.SystemColors.Control;
            this.button3.Image = ((System.Drawing.Image)(resources.GetObject("button3.Image")));
            this.button3.Location = new System.Drawing.Point(16, 34);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(32, 32);
            this.button3.TabIndex = 4;
            this.button3.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Controls.Add(this.txtEmail);
            this.groupBox2.Controls.Add(this.txtURLEdit);
            this.groupBox2.Controls.Add(this.chkOnDomain);
            this.groupBox2.Controls.Add(this.chkRunOnStartup);
            this.groupBox2.Controls.Add(this.groupBox1);
            this.groupBox2.Controls.Add(this.cmdSave);
            this.groupBox2.Controls.Add(this.cmdStop);
            this.groupBox2.Controls.Add(this.cmdStart);
            this.groupBox2.Controls.Add(this.txtDomain);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.txtURL);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.txtPwd);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.txtUser);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.txtServer);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Controls.Add(this.cmdForce);
            this.groupBox2.Controls.Add(this.button2);
            this.groupBox2.Location = new System.Drawing.Point(12, 27);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(622, 147);
            this.groupBox2.TabIndex = 12;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Settings";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.ForeColor = System.Drawing.Color.Blue;
            this.label7.Location = new System.Drawing.Point(293, 43);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(18, 13);
            this.label7.TabIndex = 30;
            this.label7.Text = "@";
            // 
            // txtEmail
            // 
            this.txtEmail.Location = new System.Drawing.Point(313, 40);
            this.txtEmail.Name = "txtEmail";
            this.txtEmail.Size = new System.Drawing.Size(128, 20);
            this.txtEmail.TabIndex = 29;
            this.txtEmail.TextChanged += new System.EventHandler(this.txtEmail_TextChanged);
            // 
            // txtURLEdit
            // 
            this.txtURLEdit.Location = new System.Drawing.Point(160, 120);
            this.txtURLEdit.Name = "txtURLEdit";
            this.txtURLEdit.Size = new System.Drawing.Size(281, 20);
            this.txtURLEdit.TabIndex = 28;
            // 
            // chkOnDomain
            // 
            this.chkOnDomain.AutoSize = true;
            this.chkOnDomain.ForeColor = System.Drawing.Color.Blue;
            this.chkOnDomain.Location = new System.Drawing.Point(361, 15);
            this.chkOnDomain.Name = "chkOnDomain";
            this.chkOnDomain.Size = new System.Drawing.Size(79, 17);
            this.chkOnDomain.TabIndex = 27;
            this.chkOnDomain.Text = "On Domain";
            this.chkOnDomain.UseVisualStyleBackColor = true;
            this.chkOnDomain.CheckedChanged += new System.EventHandler(this.chkOnDomain_CheckedChanged);
            // 
            // chkRunOnStartup
            // 
            this.chkRunOnStartup.AutoSize = true;
            this.chkRunOnStartup.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.chkRunOnStartup.ForeColor = System.Drawing.Color.Blue;
            this.chkRunOnStartup.Location = new System.Drawing.Point(509, 122);
            this.chkRunOnStartup.Name = "chkRunOnStartup";
            this.chkRunOnStartup.Size = new System.Drawing.Size(95, 17);
            this.chkRunOnStartup.TabIndex = 26;
            this.chkRunOnStartup.Text = "Run at Startup";
            this.chkRunOnStartup.UseVisualStyleBackColor = true;
            this.chkRunOnStartup.CheckedChanged += new System.EventHandler(this.chkRunOnStartup_CheckedChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.txtInterval);
            this.groupBox1.Location = new System.Drawing.Point(456, 71);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(156, 45);
            this.groupBox1.TabIndex = 25;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Update Interval";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(101, 22);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(47, 13);
            this.label6.TabIndex = 1;
            this.label6.Text = "seconds";
            // 
            // txtInterval
            // 
            this.txtInterval.Location = new System.Drawing.Point(15, 19);
            this.txtInterval.Name = "txtInterval";
            this.txtInterval.Size = new System.Drawing.Size(80, 20);
            this.txtInterval.TabIndex = 0;
            this.txtInterval.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtInterval.Validated += new System.EventHandler(this.txtInterval_Validated);
            this.txtInterval.Validating += new System.ComponentModel.CancelEventHandler(this.txtInterval_Validating);
            // 
            // cmdSave
            // 
            this.cmdSave.Image = ((System.Drawing.Image)(resources.GetObject("cmdSave.Image")));
            this.cmdSave.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.cmdSave.Location = new System.Drawing.Point(456, 42);
            this.cmdSave.Name = "cmdSave";
            this.cmdSave.Size = new System.Drawing.Size(75, 23);
            this.cmdSave.TabIndex = 24;
            this.cmdSave.Text = "Sa&ve";
            this.cmdSave.UseVisualStyleBackColor = true;
            this.cmdSave.Click += new System.EventHandler(this.cmdSave_Click);
            // 
            // cmdStop
            // 
            this.cmdStop.Image = ((System.Drawing.Image)(resources.GetObject("cmdStop.Image")));
            this.cmdStop.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.cmdStop.Location = new System.Drawing.Point(537, 13);
            this.cmdStop.Name = "cmdStop";
            this.cmdStop.Size = new System.Drawing.Size(75, 23);
            this.cmdStop.TabIndex = 23;
            this.cmdStop.Text = "Sto&p";
            this.cmdStop.UseVisualStyleBackColor = true;
            this.cmdStop.Click += new System.EventHandler(this.cmdStop_Click);
            // 
            // cmdStart
            // 
            this.cmdStart.Image = ((System.Drawing.Image)(resources.GetObject("cmdStart.Image")));
            this.cmdStart.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.cmdStart.Location = new System.Drawing.Point(456, 13);
            this.cmdStart.Name = "cmdStart";
            this.cmdStart.Size = new System.Drawing.Size(75, 23);
            this.cmdStart.TabIndex = 22;
            this.cmdStart.Text = "&Start";
            this.cmdStart.UseVisualStyleBackColor = true;
            this.cmdStart.Click += new System.EventHandler(this.cmdStart_Click);
            // 
            // txtDomain
            // 
            this.txtDomain.Location = new System.Drawing.Point(159, 91);
            this.txtDomain.Name = "txtDomain";
            this.txtDomain.Size = new System.Drawing.Size(282, 20);
            this.txtDomain.TabIndex = 21;
            this.txtDomain.TextChanged += new System.EventHandler(this.txtDomain_TextChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.ForeColor = System.Drawing.Color.Blue;
            this.label5.Location = new System.Drawing.Point(65, 94);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(52, 13);
            this.label5.TabIndex = 20;
            this.label5.Text = "Domain : ";
            // 
            // txtURL
            // 
            this.txtURL.AutoSize = true;
            this.txtURL.ForeColor = System.Drawing.Color.Black;
            this.txtURL.Location = new System.Drawing.Point(156, 123);
            this.txtURL.Name = "txtURL";
            this.txtURL.Size = new System.Drawing.Size(89, 13);
            this.txtURL.TabIndex = 19;
            this.txtURL.Text = "Exchange URL : ";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.ForeColor = System.Drawing.Color.Blue;
            this.label4.Location = new System.Drawing.Point(65, 123);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(89, 13);
            this.label4.TabIndex = 18;
            this.label4.Text = "Exchange URL : ";
            // 
            // txtPwd
            // 
            this.txtPwd.Location = new System.Drawing.Point(159, 65);
            this.txtPwd.Name = "txtPwd";
            this.txtPwd.PasswordChar = '*';
            this.txtPwd.Size = new System.Drawing.Size(282, 20);
            this.txtPwd.TabIndex = 17;
            this.txtPwd.TextChanged += new System.EventHandler(this.txtPwd_TextChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.ForeColor = System.Drawing.Color.Blue;
            this.label3.Location = new System.Drawing.Point(65, 68);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(62, 13);
            this.label3.TabIndex = 16;
            this.label3.Text = "Password : ";
            // 
            // txtUser
            // 
            this.txtUser.Location = new System.Drawing.Point(159, 39);
            this.txtUser.Name = "txtUser";
            this.txtUser.Size = new System.Drawing.Size(128, 20);
            this.txtUser.TabIndex = 15;
            this.txtUser.TextChanged += new System.EventHandler(this.txtUser_TextChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.ForeColor = System.Drawing.Color.Blue;
            this.label2.Location = new System.Drawing.Point(65, 42);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(38, 13);
            this.label2.TabIndex = 14;
            this.label2.Text = "User : ";
            // 
            // txtServer
            // 
            this.txtServer.Location = new System.Drawing.Point(159, 13);
            this.txtServer.Name = "txtServer";
            this.txtServer.Size = new System.Drawing.Size(196, 20);
            this.txtServer.TabIndex = 13;
            this.txtServer.TextChanged += new System.EventHandler(this.txtServer_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.ForeColor = System.Drawing.Color.Blue;
            this.label1.Location = new System.Drawing.Point(65, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(88, 13);
            this.label1.TabIndex = 12;
            this.label1.Text = "Server Address : ";
            // 
            // cmdForce
            // 
            this.cmdForce.Image = ((System.Drawing.Image)(resources.GetObject("cmdForce.Image")));
            this.cmdForce.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.cmdForce.Location = new System.Drawing.Point(537, 42);
            this.cmdForce.Name = "cmdForce";
            this.cmdForce.Size = new System.Drawing.Size(75, 23);
            this.cmdForce.TabIndex = 11;
            this.cmdForce.Text = "&Force";
            this.cmdForce.UseVisualStyleBackColor = true;
            this.cmdForce.Click += new System.EventHandler(this.cmdForce_Click);
            // 
            // button2
            // 
            this.button2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button2.ForeColor = System.Drawing.SystemColors.Control;
            this.button2.Image = ((System.Drawing.Image)(resources.GetObject("button2.Image")));
            this.button2.Location = new System.Drawing.Point(16, 28);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(32, 32);
            this.button2.TabIndex = 3;
            this.button2.UseVisualStyleBackColor = true;
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
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(646, 443);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.menuStrip1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "OWA Tray Monitor";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Form1_FormClosed);
            this.Move += new System.EventHandler(this.Form1_Move);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.contextMenuStrip1.ResumeLayout(false);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
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
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button cmdForce;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtPwd;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtUser;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtServer;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label txtURL;
        private System.Windows.Forms.TextBox txtDomain;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button cmdStart;
        private System.Windows.Forms.Button cmdStop;
        private System.Windows.Forms.Button cmdSave;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txtInterval;
        private System.Windows.Forms.ToolStripMenuItem openOWAToolStripMenuItem;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.ToolStripMenuItem notificationsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem balloonToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem growlToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem snarlToolStripMenuItem;
        private System.Windows.Forms.CheckBox chkRunOnStartup;
        private System.Windows.Forms.ToolStripMenuItem supportToolStripMenuItem;
        private System.Windows.Forms.CheckBox chkOnDomain;
        private System.Windows.Forms.ToolStripMenuItem playSoundToolStripMenuItem;
        private System.Windows.Forms.Timer timerLogging;
        private System.Windows.Forms.Timer timerUpdate;
        private System.Windows.Forms.ToolStripMenuItem advancedToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem overrideToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem overrideServerURLToolStripMenuItem;
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
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox txtEmail;
    }
}

