using System;
using nJim;
namespace MyChitChat.Plugin
{
    partial class Config
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
            this.labelStatus = new System.Windows.Forms.Label();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPageJabber = new System.Windows.Forms.TabPage();
            this.linkLabelCreateAccount = new System.Windows.Forms.LinkLabel();
            this.label4 = new System.Windows.Forms.Label();
            this.textBoxResource = new System.Windows.Forms.TextBox();
            this.textBoxPassword = new System.Windows.Forms.TextBox();
            this.textBoxServer = new System.Windows.Forms.TextBox();
            this.textBoxUsername = new System.Windows.Forms.TextBox();
            this.linkFAQ = new System.Windows.Forms.LinkLabel();
            this.buttonTest = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.tabPageDefault = new System.Windows.Forms.TabPage();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.label11 = new System.Windows.Forms.Label();
            this.textBoxStartupMood = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.comboBoxStartupMood = new System.Windows.Forms.ComboBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label6 = new System.Windows.Forms.Label();
            this.textBoxStartupStatus = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.comboBoxStartupStatus = new System.Windows.Forms.ComboBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label7 = new System.Windows.Forms.Label();
            this.textBoxAutoIdleStatus = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.comboBoxAutoIdleStatus = new System.Windows.Forms.ComboBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.label9 = new System.Windows.Forms.Label();
            this.textBoxStartupActivity = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.comboBoxStartupActivity = new System.Windows.Forms.ComboBox();
            this.tabPageNotification = new System.Windows.Forms.TabPage();
            this.statusTypeBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.activityTypeBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.numericUpDownIdleTimeOut = new System.Windows.Forms.NumericUpDown();
            this.label13 = new System.Windows.Forms.Label();
            this.tabControl1.SuspendLayout();
            this.tabPageJabber.SuspendLayout();
            this.tabPageDefault.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.statusTypeBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.activityTypeBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownIdleTimeOut)).BeginInit();
            this.SuspendLayout();
            // 
            // labelStatus
            // 
            this.labelStatus.AutoSize = true;
            this.labelStatus.BackColor = System.Drawing.Color.White;
            this.labelStatus.Location = new System.Drawing.Point(203, 196);
            this.labelStatus.Name = "labelStatus";
            this.labelStatus.Size = new System.Drawing.Size(0, 13);
            this.labelStatus.TabIndex = 0;
            this.labelStatus.Visible = false;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPageJabber);
            this.tabControl1.Controls.Add(this.tabPageDefault);
            this.tabControl1.Controls.Add(this.tabPageNotification);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.tabControl1.Location = new System.Drawing.Point(0, 80);
            this.tabControl1.Multiline = true;
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(634, 368);
            this.tabControl1.TabIndex = 3;
            // 
            // tabPageJabber
            // 
            this.tabPageJabber.BackColor = System.Drawing.SystemColors.Menu;
            this.tabPageJabber.Controls.Add(this.linkLabelCreateAccount);
            this.tabPageJabber.Controls.Add(this.label4);
            this.tabPageJabber.Controls.Add(this.textBoxResource);
            this.tabPageJabber.Controls.Add(this.textBoxPassword);
            this.tabPageJabber.Controls.Add(this.textBoxServer);
            this.tabPageJabber.Controls.Add(this.textBoxUsername);
            this.tabPageJabber.Controls.Add(this.linkFAQ);
            this.tabPageJabber.Controls.Add(this.buttonTest);
            this.tabPageJabber.Controls.Add(this.label3);
            this.tabPageJabber.Controls.Add(this.label2);
            this.tabPageJabber.Controls.Add(this.label1);
            this.tabPageJabber.Location = new System.Drawing.Point(4, 22);
            this.tabPageJabber.Name = "tabPageJabber";
            this.tabPageJabber.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageJabber.Size = new System.Drawing.Size(626, 342);
            this.tabPageJabber.TabIndex = 0;
            this.tabPageJabber.Text = "Jabber Credentials";
            // 
            // linkLabelCreateAccount
            // 
            this.linkLabelCreateAccount.AutoSize = true;
            this.linkLabelCreateAccount.ForeColor = System.Drawing.SystemColors.ControlText;
            this.linkLabelCreateAccount.Location = new System.Drawing.Point(7, 159);
            this.linkLabelCreateAccount.Name = "linkLabelCreateAccount";
            this.linkLabelCreateAccount.Size = new System.Drawing.Size(81, 13);
            this.linkLabelCreateAccount.TabIndex = 18;
            this.linkLabelCreateAccount.TabStop = true;
            this.linkLabelCreateAccount.Text = "Create Account";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(195, 61);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(53, 13);
            this.label4.TabIndex = 11;
            this.label4.Text = "Resource";
            // 
            // textBoxResource
            // 
            this.textBoxResource.Location = new System.Drawing.Point(198, 78);
            this.textBoxResource.Name = "textBoxResource";
            this.textBoxResource.Size = new System.Drawing.Size(160, 20);
            this.textBoxResource.TabIndex = 15;
            this.textBoxResource.Text = "MediaPortal";
            // 
            // textBoxPassword
            // 
            this.textBoxPassword.Location = new System.Drawing.Point(8, 78);
            this.textBoxPassword.Name = "textBoxPassword";
            this.textBoxPassword.Size = new System.Drawing.Size(160, 20);
            this.textBoxPassword.TabIndex = 14;
            this.textBoxPassword.UseSystemPasswordChar = true;
            // 
            // textBoxServer
            // 
            this.textBoxServer.Location = new System.Drawing.Point(198, 32);
            this.textBoxServer.Name = "textBoxServer";
            this.textBoxServer.Size = new System.Drawing.Size(169, 20);
            this.textBoxServer.TabIndex = 13;
            // 
            // textBoxUsername
            // 
            this.textBoxUsername.Location = new System.Drawing.Point(8, 32);
            this.textBoxUsername.Name = "textBoxUsername";
            this.textBoxUsername.Size = new System.Drawing.Size(160, 20);
            this.textBoxUsername.TabIndex = 12;
            // 
            // linkFAQ
            // 
            this.linkFAQ.AutoSize = true;
            this.linkFAQ.ForeColor = System.Drawing.SystemColors.ControlText;
            this.linkFAQ.Location = new System.Drawing.Point(7, 135);
            this.linkFAQ.Name = "linkFAQ";
            this.linkFAQ.Size = new System.Drawing.Size(63, 13);
            this.linkFAQ.TabIndex = 17;
            this.linkFAQ.TabStop = true;
            this.linkFAQ.Text = "Jabber FAQ";
            // 
            // buttonTest
            // 
            this.buttonTest.Location = new System.Drawing.Point(198, 135);
            this.buttonTest.Name = "buttonTest";
            this.buttonTest.Size = new System.Drawing.Size(169, 37);
            this.buttonTest.TabIndex = 16;
            this.buttonTest.Text = "Test";
            this.buttonTest.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(7, 61);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(53, 13);
            this.label3.TabIndex = 10;
            this.label3.Text = "Password";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(174, 35);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(18, 13);
            this.label2.TabIndex = 9;
            this.label2.Text = "@";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(7, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(161, 13);
            this.label1.TabIndex = 8;
            this.label1.Text = "Username (example@jabber.org)";
            // 
            // tabPageDefault
            // 
            this.tabPageDefault.BackColor = System.Drawing.SystemColors.Menu;
            this.tabPageDefault.Controls.Add(this.tableLayoutPanel1);
            this.tabPageDefault.Location = new System.Drawing.Point(4, 22);
            this.tabPageDefault.Name = "tabPageDefault";
            this.tabPageDefault.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageDefault.Size = new System.Drawing.Size(626, 342);
            this.tabPageDefault.TabIndex = 1;
            this.tabPageDefault.Text = "Status / Activity / Mood";
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Controls.Add(this.groupBox4, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.groupBox1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.groupBox2, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.groupBox3, 0, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(620, 336);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.label11);
            this.groupBox4.Controls.Add(this.textBoxStartupMood);
            this.groupBox4.Controls.Add(this.label12);
            this.groupBox4.Controls.Add(this.comboBoxStartupMood);
            this.groupBox4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox4.Location = new System.Drawing.Point(313, 171);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(304, 162);
            this.groupBox4.TabIndex = 4;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Startup Mood";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(6, 49);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(50, 13);
            this.label11.TabIndex = 3;
            this.label11.Text = "Message";
            // 
            // textBoxStartupMood
            // 
            this.textBoxStartupMood.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxStartupMood.Location = new System.Drawing.Point(84, 46);
            this.textBoxStartupMood.Multiline = true;
            this.textBoxStartupMood.Name = "textBoxStartupMood";
            this.textBoxStartupMood.Size = new System.Drawing.Size(214, 64);
            this.textBoxStartupMood.TabIndex = 2;
            this.textBoxStartupMood.TextChanged += new System.EventHandler(this.textBoxStartupMood_TextChanged);
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(25, 22);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(31, 13);
            this.label12.TabIndex = 1;
            this.label12.Text = "Type";
            // 
            // comboBoxStartupMood
            // 
            this.comboBoxStartupMood.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxStartupMood.FormattingEnabled = true;
            this.comboBoxStartupMood.Location = new System.Drawing.Point(84, 19);
            this.comboBoxStartupMood.Name = "comboBoxStartupMood";
            this.comboBoxStartupMood.Size = new System.Drawing.Size(214, 21);
            this.comboBoxStartupMood.TabIndex = 0;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.textBoxStartupStatus);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.comboBoxStartupStatus);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(3, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(304, 162);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Startup Status";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(6, 49);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(50, 13);
            this.label6.TabIndex = 3;
            this.label6.Text = "Message";
            // 
            // textBoxStartupStatus
            // 
            this.textBoxStartupStatus.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxStartupStatus.Location = new System.Drawing.Point(84, 46);
            this.textBoxStartupStatus.Multiline = true;
            this.textBoxStartupStatus.Name = "textBoxStartupStatus";
            this.textBoxStartupStatus.Size = new System.Drawing.Size(214, 64);
            this.textBoxStartupStatus.TabIndex = 2;
            this.textBoxStartupStatus.TextChanged += new System.EventHandler(this.textBoxStartupStatus_TextChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(25, 22);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(31, 13);
            this.label5.TabIndex = 1;
            this.label5.Text = "Type";
            // 
            // comboBoxStartupStatus
            // 
            this.comboBoxStartupStatus.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxStartupStatus.FormattingEnabled = true;
            this.comboBoxStartupStatus.Location = new System.Drawing.Point(84, 19);
            this.comboBoxStartupStatus.Name = "comboBoxStartupStatus";
            this.comboBoxStartupStatus.Size = new System.Drawing.Size(214, 21);
            this.comboBoxStartupStatus.TabIndex = 0;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label13);
            this.groupBox2.Controls.Add(this.numericUpDownIdleTimeOut);
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Controls.Add(this.textBoxAutoIdleStatus);
            this.groupBox2.Controls.Add(this.label8);
            this.groupBox2.Controls.Add(this.comboBoxAutoIdleStatus);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox2.Location = new System.Drawing.Point(313, 3);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(304, 162);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Auto Idle  Status";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(6, 49);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(50, 13);
            this.label7.TabIndex = 3;
            this.label7.Text = "Message";
            // 
            // textBoxAutoIdleStatus
            // 
            this.textBoxAutoIdleStatus.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxAutoIdleStatus.Location = new System.Drawing.Point(84, 46);
            this.textBoxAutoIdleStatus.Multiline = true;
            this.textBoxAutoIdleStatus.Name = "textBoxAutoIdleStatus";
            this.textBoxAutoIdleStatus.Size = new System.Drawing.Size(214, 64);
            this.textBoxAutoIdleStatus.TabIndex = 2;
            this.textBoxAutoIdleStatus.TextChanged += new System.EventHandler(this.textBoxAutoIdleStatus_TextChanged);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(25, 22);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(31, 13);
            this.label8.TabIndex = 1;
            this.label8.Text = "Type";
            // 
            // comboBoxAutoIdleStatus
            // 
            this.comboBoxAutoIdleStatus.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxAutoIdleStatus.FormattingEnabled = true;
            this.comboBoxAutoIdleStatus.Location = new System.Drawing.Point(84, 19);
            this.comboBoxAutoIdleStatus.Name = "comboBoxAutoIdleStatus";
            this.comboBoxAutoIdleStatus.Size = new System.Drawing.Size(214, 21);
            this.comboBoxAutoIdleStatus.TabIndex = 0;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.label9);
            this.groupBox3.Controls.Add(this.textBoxStartupActivity);
            this.groupBox3.Controls.Add(this.label10);
            this.groupBox3.Controls.Add(this.comboBoxStartupActivity);
            this.groupBox3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox3.Location = new System.Drawing.Point(3, 171);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(304, 162);
            this.groupBox3.TabIndex = 2;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Startup Activity";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(6, 49);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(50, 13);
            this.label9.TabIndex = 3;
            this.label9.Text = "Message";
            // 
            // textBoxStartupActivity
            // 
            this.textBoxStartupActivity.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxStartupActivity.Location = new System.Drawing.Point(84, 46);
            this.textBoxStartupActivity.Multiline = true;
            this.textBoxStartupActivity.Name = "textBoxStartupActivity";
            this.textBoxStartupActivity.Size = new System.Drawing.Size(214, 64);
            this.textBoxStartupActivity.TabIndex = 2;
            this.textBoxStartupActivity.TextChanged += new System.EventHandler(this.textBoxStartupActivity_TextChanged);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(25, 22);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(31, 13);
            this.label10.TabIndex = 1;
            this.label10.Text = "Type";
            // 
            // comboBoxStartupActivity
            // 
            this.comboBoxStartupActivity.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxStartupActivity.FormattingEnabled = true;
            this.comboBoxStartupActivity.Location = new System.Drawing.Point(84, 19);
            this.comboBoxStartupActivity.Name = "comboBoxStartupActivity";
            this.comboBoxStartupActivity.Size = new System.Drawing.Size(214, 21);
            this.comboBoxStartupActivity.TabIndex = 0;
            // 
            // tabPageNotification
            // 
            this.tabPageNotification.BackColor = System.Drawing.SystemColors.Menu;
            this.tabPageNotification.Location = new System.Drawing.Point(4, 22);
            this.tabPageNotification.Name = "tabPageNotification";
            this.tabPageNotification.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageNotification.Size = new System.Drawing.Size(626, 342);
            this.tabPageNotification.TabIndex = 2;
            this.tabPageNotification.Text = "Behaviour & Notification";
            // 
            // statusTypeBindingSource
            // 
            this.statusTypeBindingSource.DataSource = typeof(nJim.Enums.StatusType);
            // 
            // numericUpDownIdleTimeOut
            // 
            this.numericUpDownIdleTimeOut.Location = new System.Drawing.Point(209, 116);
            this.numericUpDownIdleTimeOut.Maximum = new decimal(new int[] {
            120,
            0,
            0,
            0});
            this.numericUpDownIdleTimeOut.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDownIdleTimeOut.Name = "numericUpDownIdleTimeOut";
            this.numericUpDownIdleTimeOut.Size = new System.Drawing.Size(46, 20);
            this.numericUpDownIdleTimeOut.TabIndex = 4;
            this.numericUpDownIdleTimeOut.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDownIdleTimeOut.ValueChanged += new System.EventHandler(this.numericUpDownIdleTimeOut_ValueChanged);
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(81, 118);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(97, 13);
            this.label13.TabIndex = 5;
            this.label13.Text = "Idle TimeOut (mins)";
            // 
            // Config
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.BackgroundImage = global::MyChitChat.Properties.Resources.Config_Stripes;
            this.ClientSize = new System.Drawing.Size(634, 448);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.labelStatus);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "Config";
            this.ShowIcon = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "MyChitChat Configuration (0.1.0)";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.SetupForm_FormClosing);
            this.tabControl1.ResumeLayout(false);
            this.tabPageJabber.ResumeLayout(false);
            this.tabPageJabber.PerformLayout();
            this.tabPageDefault.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.statusTypeBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.activityTypeBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownIdleTimeOut)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelStatus;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPageJabber;
        private System.Windows.Forms.LinkLabel linkLabelCreateAccount;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox textBoxResource;
        private System.Windows.Forms.TextBox textBoxPassword;
        private System.Windows.Forms.TextBox textBoxServer;
        private System.Windows.Forms.TextBox textBoxUsername;
        private System.Windows.Forms.LinkLabel linkFAQ;
        private System.Windows.Forms.Button buttonTest;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TabPage tabPageDefault;
        private System.Windows.Forms.TabPage tabPageNotification;
        private System.Windows.Forms.BindingSource activityTypeBindingSource;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox textBoxStartupStatus;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox comboBoxStartupStatus;
        private System.Windows.Forms.BindingSource statusTypeBindingSource;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.TextBox textBoxStartupMood;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.ComboBox comboBoxStartupMood;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox textBoxAutoIdleStatus;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.ComboBox comboBoxAutoIdleStatus;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox textBoxStartupActivity;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.ComboBox comboBoxStartupActivity;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.NumericUpDown numericUpDownIdleTimeOut;
    }
}