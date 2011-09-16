namespace DrunkenBakery.OWAtray
{
    partial class ContactUs
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ContactUs));
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.lvX = new System.Windows.Forms.ListView();
			this.Language = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.Author = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.imLarge = new System.Windows.Forms.ImageList(this.components);
			this.cmdOK = new System.Windows.Forms.Button();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.linkEmail = new System.Windows.Forms.LinkLabel();
			this.button4 = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.linkBakery = new System.Windows.Forms.LinkLabel();
			this.button3 = new System.Windows.Forms.Button();
			this.button1 = new System.Windows.Forms.Button();
			this.groupBox1.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.SuspendLayout();
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.lvX);
			this.groupBox1.Location = new System.Drawing.Point(12, 103);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(327, 220);
			this.groupBox1.TabIndex = 0;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Translations";
			// 
			// lvX
			// 
			this.lvX.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.Language,
            this.Author});
			this.lvX.GridLines = true;
			this.lvX.Location = new System.Drawing.Point(6, 19);
			this.lvX.Name = "lvX";
			this.lvX.Size = new System.Drawing.Size(312, 195);
			this.lvX.SmallImageList = this.imLarge;
			this.lvX.TabIndex = 0;
			this.lvX.UseCompatibleStateImageBehavior = false;
			this.lvX.View = System.Windows.Forms.View.Details;
			// 
			// Language
			// 
			this.Language.Text = "Language";
			this.Language.Width = 118;
			// 
			// Author
			// 
			this.Author.Text = "Author";
			this.Author.Width = 190;
			// 
			// imLarge
			// 
			this.imLarge.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imLarge.ImageStream")));
			this.imLarge.TransparentColor = System.Drawing.Color.Transparent;
			this.imLarge.Images.SetKeyName(0, "catalonia.png");
			this.imLarge.Images.SetKeyName(1, "de.png");
			this.imLarge.Images.SetKeyName(2, "es.png");
			// 
			// cmdOK
			// 
			this.cmdOK.Location = new System.Drawing.Point(264, 329);
			this.cmdOK.Name = "cmdOK";
			this.cmdOK.Size = new System.Drawing.Size(75, 23);
			this.cmdOK.TabIndex = 1;
			this.cmdOK.Text = global::DrunkenBakery.OWAtray.OWAtray.OK;
			this.cmdOK.UseVisualStyleBackColor = true;
			this.cmdOK.Click += new System.EventHandler(this.cmdOK_Click);
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.linkEmail);
			this.groupBox2.Controls.Add(this.button4);
			this.groupBox2.Controls.Add(this.label1);
			this.groupBox2.Controls.Add(this.linkBakery);
			this.groupBox2.Controls.Add(this.button3);
			this.groupBox2.Controls.Add(this.button1);
			this.groupBox2.Location = new System.Drawing.Point(12, 12);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(327, 85);
			this.groupBox2.TabIndex = 2;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Contact Details";
			// 
			// linkEmail
			// 
			this.linkEmail.AutoSize = true;
			this.linkEmail.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.linkEmail.Location = new System.Drawing.Point(201, 55);
			this.linkEmail.Name = "linkEmail";
			this.linkEmail.Size = new System.Drawing.Size(117, 13);
			this.linkEmail.TabIndex = 12;
			this.linkEmail.TabStop = true;
			this.linkEmail.Text = "support@owatray.com";
			this.linkEmail.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkEmail_LinkClicked);
			// 
			// button4
			// 
			this.button4.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.button4.ForeColor = System.Drawing.SystemColors.Control;
			this.button4.Image = ((System.Drawing.Image)(resources.GetObject("button4.Image")));
			this.button4.Location = new System.Drawing.Point(169, 45);
			this.button4.Name = "button4";
			this.button4.Size = new System.Drawing.Size(32, 32);
			this.button4.TabIndex = 8;
			this.button4.UseVisualStyleBackColor = true;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(35, 29);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(106, 13);
			this.label1.TabIndex = 7;
			this.label1.Text = "The Drunken Bakery";
			// 
			// linkBakery
			// 
			this.linkBakery.AutoSize = true;
			this.linkBakery.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.linkBakery.Location = new System.Drawing.Point(35, 55);
			this.linkBakery.Name = "linkBakery";
			this.linkBakery.Size = new System.Drawing.Size(130, 13);
			this.linkBakery.TabIndex = 6;
			this.linkBakery.TabStop = true;
			this.linkBakery.Text = "http://www.owatray.com";
			this.linkBakery.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkBakery_LinkClicked);
			// 
			// button3
			// 
			this.button3.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.button3.ForeColor = System.Drawing.SystemColors.Control;
			this.button3.Image = ((System.Drawing.Image)(resources.GetObject("button3.Image")));
			this.button3.Location = new System.Drawing.Point(6, 45);
			this.button3.Name = "button3";
			this.button3.Size = new System.Drawing.Size(32, 32);
			this.button3.TabIndex = 5;
			this.button3.UseVisualStyleBackColor = true;
			// 
			// button1
			// 
			this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.button1.ForeColor = System.Drawing.SystemColors.Control;
			this.button1.Image = ((System.Drawing.Image)(resources.GetObject("button1.Image")));
			this.button1.Location = new System.Drawing.Point(6, 19);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(32, 32);
			this.button1.TabIndex = 3;
			this.button1.UseVisualStyleBackColor = true;
			// 
			// ContactUs
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(351, 364);
			this.Controls.Add(this.groupBox2);
			this.Controls.Add(this.cmdOK);
			this.Controls.Add(this.groupBox1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "ContactUs";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Support Information";
			this.groupBox1.ResumeLayout(false);
			this.groupBox2.ResumeLayout(false);
			this.groupBox2.PerformLayout();
			this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Button cmdOK;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.LinkLabel linkBakery;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.LinkLabel linkEmail;
		private System.Windows.Forms.ImageList imLarge;
		private System.Windows.Forms.ListView lvX;
		private System.Windows.Forms.ColumnHeader Language;
		private System.Windows.Forms.ColumnHeader Author;
    }
}