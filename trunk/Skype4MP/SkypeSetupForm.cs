using System;
using System.Drawing;
using System.ComponentModel;
using System.Windows.Forms;
using System.Collections.Generic;

using MediaPortal.GUI.Library;

namespace maniac.MediaPortal.Skype
{
  /// <summary>
  /// Description résumée de SkypeSetupForm.
  /// </summary>
  public class SkypeSetupForm : System.Windows.Forms.Form
  {
    private System.Windows.Forms.Button button1;
    private System.Windows.Forms.Button button2;
    private System.Windows.Forms.CheckBox cBStartWithMP;
    private System.Windows.Forms.CheckBox cBStopWithMP;

    //private System.Windows.Forms.CheckBox cBDNDWhilePlaying;
    private System.Windows.Forms.TabControl tabControl1;
    private System.Windows.Forms.TabPage tPGeneral;
    private System.Windows.Forms.TabPage tPAbout;
    private System.Windows.Forms.LinkLabel linkLabel3;
    private System.Windows.Forms.Label label14;
    private System.Windows.Forms.LinkLabel linkLabel2;
    private System.Windows.Forms.Label label13;
    private System.Windows.Forms.Label label10;
    private System.Windows.Forms.LinkLabel linkLabel1;
    private System.Windows.Forms.Label label12;
    private System.Windows.Forms.Label label11;
    private LinkLabel linkLabel4;
    private Label label1;
    private Label label3;
    private Label label2;
    private TabPage tPSkypeCredit;
    private CheckBox cBAllowSMS;
    private CheckBox cBAllowSkypeOut;
    private CheckBox cbUseSilentMode;
    private TabPage tpVoiceVideo;
    private CheckBox cBPauseForCalls;
    private CheckBox cBHandleVoiceCalls;
    private CheckBox cBHangupIfDND;
    private TabPage tpChat;
    private CheckBox cBPauseForChats;
    private CheckBox cBHandleChats;
    private CheckBox cBIgnoreIncomingCalls;
    /// <summary>
    /// Variable nécessaire au concepteur.
    /// </summary>
    private System.ComponentModel.Container components = null;

    /// <summary>
    /// options is the list of options and their current values
    /// </summary>
    private OptionTypeList options = new OptionTypeList();
    /// <summary>
    /// opts links the option to its button on the form
    /// </summary>
    Dictionary<string, CheckBox> opts = new Dictionary<string, CheckBox>();

    public SkypeSetupForm()
    {
      //
      // Requis pour la prise en charge du Concepteur Windows Forms
      //
      InitializeComponent();
    }

    /// <summary>
    /// Nettoyage des ressources utilisées.
    /// </summary>
    protected override void Dispose(bool disposing)
    {
      if (disposing)
      {
        if (components != null)
        {
          components.Dispose();
        }
      }
      base.Dispose(disposing);
    }


    #region Code généré par le Concepteur Windows Form
    /// <summary>
    /// Méthode requise pour la prise en charge du concepteur - ne modifiez pas
    /// le contenu de cette méthode avec l'éditeur de code.
    /// </summary>
    private void InitializeComponent()
    {
      this.button1 = new System.Windows.Forms.Button();
      this.cBStartWithMP = new System.Windows.Forms.CheckBox();
      this.cBStopWithMP = new System.Windows.Forms.CheckBox();
      this.button2 = new System.Windows.Forms.Button();
      this.tabControl1 = new System.Windows.Forms.TabControl();
      this.tPGeneral = new System.Windows.Forms.TabPage();
      this.cbUseSilentMode = new System.Windows.Forms.CheckBox();
      this.tpVoiceVideo = new System.Windows.Forms.TabPage();
      this.cBIgnoreIncomingCalls = new System.Windows.Forms.CheckBox();
      this.cBPauseForCalls = new System.Windows.Forms.CheckBox();
      this.cBHandleVoiceCalls = new System.Windows.Forms.CheckBox();
      this.cBHangupIfDND = new System.Windows.Forms.CheckBox();
      this.tpChat = new System.Windows.Forms.TabPage();
      this.cBPauseForChats = new System.Windows.Forms.CheckBox();
      this.cBHandleChats = new System.Windows.Forms.CheckBox();
      this.tPSkypeCredit = new System.Windows.Forms.TabPage();
      this.cBAllowSMS = new System.Windows.Forms.CheckBox();
      this.cBAllowSkypeOut = new System.Windows.Forms.CheckBox();
      this.tPAbout = new System.Windows.Forms.TabPage();
      this.label3 = new System.Windows.Forms.Label();
      this.label2 = new System.Windows.Forms.Label();
      this.label1 = new System.Windows.Forms.Label();
      this.linkLabel4 = new System.Windows.Forms.LinkLabel();
      this.linkLabel1 = new System.Windows.Forms.LinkLabel();
      this.label12 = new System.Windows.Forms.Label();
      this.label11 = new System.Windows.Forms.Label();
      this.label14 = new System.Windows.Forms.Label();
      this.label13 = new System.Windows.Forms.Label();
      this.label10 = new System.Windows.Forms.Label();
      this.linkLabel3 = new System.Windows.Forms.LinkLabel();
      this.linkLabel2 = new System.Windows.Forms.LinkLabel();
      this.tabControl1.SuspendLayout();
      this.tPGeneral.SuspendLayout();
      this.tpVoiceVideo.SuspendLayout();
      this.tpChat.SuspendLayout();
      this.tPSkypeCredit.SuspendLayout();
      this.tPAbout.SuspendLayout();
      this.SuspendLayout();
      // 
      // button1
      // 
      this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.button1.Location = new System.Drawing.Point(208, 248);
      this.button1.Name = "button1";
      this.button1.Size = new System.Drawing.Size(75, 23);
      this.button1.TabIndex = 0;
      this.button1.Text = "&Cancel";
      this.button1.Click += new System.EventHandler(this.button1_Click);
      // 
      // cBStartWithMP
      // 
      this.cBStartWithMP.Location = new System.Drawing.Point(19, 20);
      this.cBStartWithMP.Name = "cBStartWithMP";
      this.cBStartWithMP.Size = new System.Drawing.Size(248, 24);
      this.cBStartWithMP.TabIndex = 1;
      this.cBStartWithMP.Text = "Run Skype when MediaPortal starts.";
      // 
      // cBStopWithMP
      // 
      this.cBStopWithMP.Location = new System.Drawing.Point(19, 45);
      this.cBStopWithMP.Name = "cBStopWithMP";
      this.cBStopWithMP.Size = new System.Drawing.Size(240, 24);
      this.cBStopWithMP.TabIndex = 2;
      this.cBStopWithMP.Text = "Close Skype when MediaPortal stops.";
      // 
      // button2
      // 
      this.button2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.button2.Location = new System.Drawing.Point(104, 248);
      this.button2.Name = "button2";
      this.button2.Size = new System.Drawing.Size(75, 23);
      this.button2.TabIndex = 3;
      this.button2.Text = "&Ok";
      this.button2.Click += new System.EventHandler(this.button2_Click);
      // 
      // tabControl1
      // 
      this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.tabControl1.Controls.Add(this.tPGeneral);
      this.tabControl1.Controls.Add(this.tpVoiceVideo);
      this.tabControl1.Controls.Add(this.tpChat);
      this.tabControl1.Controls.Add(this.tPSkypeCredit);
      this.tabControl1.Controls.Add(this.tPAbout);
      this.tabControl1.Location = new System.Drawing.Point(8, 8);
      this.tabControl1.Name = "tabControl1";
      this.tabControl1.SelectedIndex = 0;
      this.tabControl1.Size = new System.Drawing.Size(368, 232);
      this.tabControl1.TabIndex = 4;
      // 
      // tPGeneral
      // 
      this.tPGeneral.Controls.Add(this.cbUseSilentMode);
      this.tPGeneral.Controls.Add(this.cBStartWithMP);
      this.tPGeneral.Controls.Add(this.cBStopWithMP);
      this.tPGeneral.Location = new System.Drawing.Point(4, 22);
      this.tPGeneral.Name = "tPGeneral";
      this.tPGeneral.Size = new System.Drawing.Size(360, 206);
      this.tPGeneral.TabIndex = 0;
      this.tPGeneral.Text = "General";
      this.tPGeneral.UseVisualStyleBackColor = true;
      // 
      // cbUseSilentMode
      // 
      this.cbUseSilentMode.AutoSize = true;
      this.cbUseSilentMode.Location = new System.Drawing.Point(19, 72);
      this.cbUseSilentMode.Name = "cbUseSilentMode";
      this.cbUseSilentMode.Size = new System.Drawing.Size(103, 17);
      this.cbUseSilentMode.TabIndex = 7;
      this.cbUseSilentMode.Text = "Use Silent mode";
      this.cbUseSilentMode.UseVisualStyleBackColor = true;
      // 
      // tpVoiceVideo
      // 
      this.tpVoiceVideo.Controls.Add(this.cBIgnoreIncomingCalls);
      this.tpVoiceVideo.Controls.Add(this.cBPauseForCalls);
      this.tpVoiceVideo.Controls.Add(this.cBHandleVoiceCalls);
      this.tpVoiceVideo.Controls.Add(this.cBHangupIfDND);
      this.tpVoiceVideo.Location = new System.Drawing.Point(4, 22);
      this.tpVoiceVideo.Name = "tpVoiceVideo";
      this.tpVoiceVideo.Size = new System.Drawing.Size(360, 206);
      this.tpVoiceVideo.TabIndex = 3;
      this.tpVoiceVideo.Text = "Voice/Video";
      this.tpVoiceVideo.UseVisualStyleBackColor = true;
      // 
      // cBIgnoreIncomingCalls
      // 
      this.cBIgnoreIncomingCalls.Location = new System.Drawing.Point(38, 91);
      this.cBIgnoreIncomingCalls.Name = "cBIgnoreIncomingCalls";
      this.cBIgnoreIncomingCalls.Size = new System.Drawing.Size(290, 24);
      this.cBIgnoreIncomingCalls.TabIndex = 9;
      this.cBIgnoreIncomingCalls.Text = "Ignore incoming calls (no dialog)";
      // 
      // cBPauseForCalls
      // 
      this.cBPauseForCalls.Location = new System.Drawing.Point(38, 45);
      this.cBPauseForCalls.Name = "cBPauseForCalls";
      this.cBPauseForCalls.Size = new System.Drawing.Size(290, 24);
      this.cBPauseForCalls.TabIndex = 8;
      this.cBPauseForCalls.Text = "Pause media for incoming calls";
      // 
      // cBHandleVoiceCalls
      // 
      this.cBHandleVoiceCalls.AutoSize = true;
      this.cBHandleVoiceCalls.Location = new System.Drawing.Point(19, 22);
      this.cBHandleVoiceCalls.Name = "cBHandleVoiceCalls";
      this.cBHandleVoiceCalls.Size = new System.Drawing.Size(113, 17);
      this.cBHandleVoiceCalls.TabIndex = 7;
      this.cBHandleVoiceCalls.Text = "Handle voice calls";
      this.cBHandleVoiceCalls.UseVisualStyleBackColor = true;
      this.cBHandleVoiceCalls.CheckedChanged += new System.EventHandler(this.cBHandleVoiceCalls_CheckedChanged);
      // 
      // cBHangupIfDND
      // 
      this.cBHangupIfDND.Location = new System.Drawing.Point(38, 68);
      this.cBHangupIfDND.Name = "cBHangupIfDND";
      this.cBHangupIfDND.Size = new System.Drawing.Size(290, 24);
      this.cBHangupIfDND.TabIndex = 6;
      this.cBHangupIfDND.Text = "Refuse incoming calls while Do Not Disturb";
      // 
      // tpChat
      // 
      this.tpChat.Controls.Add(this.cBPauseForChats);
      this.tpChat.Controls.Add(this.cBHandleChats);
      this.tpChat.Location = new System.Drawing.Point(4, 22);
      this.tpChat.Name = "tpChat";
      this.tpChat.Size = new System.Drawing.Size(360, 206);
      this.tpChat.TabIndex = 4;
      this.tpChat.Text = "Chat";
      this.tpChat.UseVisualStyleBackColor = true;
      // 
      // cBPauseForChats
      // 
      this.cBPauseForChats.Location = new System.Drawing.Point(38, 45);
      this.cBPauseForChats.Name = "cBPauseForChats";
      this.cBPauseForChats.Size = new System.Drawing.Size(290, 24);
      this.cBPauseForChats.TabIndex = 8;
      this.cBPauseForChats.Text = "Pause media for incoming chats";
      // 
      // cBHandleChats
      // 
      this.cBHandleChats.AutoSize = true;
      this.cBHandleChats.Location = new System.Drawing.Point(19, 22);
      this.cBHandleChats.Name = "cBHandleChats";
      this.cBHandleChats.Size = new System.Drawing.Size(134, 17);
      this.cBHandleChats.TabIndex = 7;
      this.cBHandleChats.Text = "Handle chat messages";
      this.cBHandleChats.UseVisualStyleBackColor = true;
      this.cBHandleChats.CheckedChanged += new System.EventHandler(this.cBHandleChats_CheckedChanged);
      // 
      // tPSkypeCredit
      // 
      this.tPSkypeCredit.Controls.Add(this.cBAllowSMS);
      this.tPSkypeCredit.Controls.Add(this.cBAllowSkypeOut);
      this.tPSkypeCredit.Location = new System.Drawing.Point(4, 22);
      this.tPSkypeCredit.Name = "tPSkypeCredit";
      this.tPSkypeCredit.Size = new System.Drawing.Size(360, 206);
      this.tPSkypeCredit.TabIndex = 2;
      this.tPSkypeCredit.Text = "Skype Credit";
      this.tPSkypeCredit.UseVisualStyleBackColor = true;
      // 
      // cBAllowSMS
      // 
      this.cBAllowSMS.AutoSize = true;
      this.cBAllowSMS.Location = new System.Drawing.Point(35, 54);
      this.cBAllowSMS.Name = "cBAllowSMS";
      this.cBAllowSMS.Size = new System.Drawing.Size(120, 17);
      this.cBAllowSMS.TabIndex = 1;
      this.cBAllowSMS.Text = "Allow sending SMS ";
      this.cBAllowSMS.UseVisualStyleBackColor = true;
      // 
      // cBAllowSkypeOut
      // 
      this.cBAllowSkypeOut.AutoSize = true;
      this.cBAllowSkypeOut.Location = new System.Drawing.Point(35, 31);
      this.cBAllowSkypeOut.Name = "cBAllowSkypeOut";
      this.cBAllowSkypeOut.Size = new System.Drawing.Size(125, 17);
      this.cBAllowSkypeOut.TabIndex = 0;
      this.cBAllowSkypeOut.Text = "Allow SkypeOut calls";
      this.cBAllowSkypeOut.UseVisualStyleBackColor = true;
      // 
      // tPAbout
      // 
      this.tPAbout.Controls.Add(this.label3);
      this.tPAbout.Controls.Add(this.label2);
      this.tPAbout.Controls.Add(this.label1);
      this.tPAbout.Controls.Add(this.linkLabel4);
      this.tPAbout.Controls.Add(this.linkLabel1);
      this.tPAbout.Controls.Add(this.label12);
      this.tPAbout.Controls.Add(this.label11);
      this.tPAbout.Controls.Add(this.label14);
      this.tPAbout.Controls.Add(this.label13);
      this.tPAbout.Controls.Add(this.label10);
      this.tPAbout.Location = new System.Drawing.Point(4, 22);
      this.tPAbout.Name = "tPAbout";
      this.tPAbout.Size = new System.Drawing.Size(360, 206);
      this.tPAbout.TabIndex = 1;
      this.tPAbout.Text = "About";
      this.tPAbout.UseVisualStyleBackColor = true;
      // 
      // label3
      // 
      this.label3.Location = new System.Drawing.Point(80, 131);
      this.label3.Name = "label3";
      this.label3.Size = new System.Drawing.Size(272, 14);
      this.label3.TabIndex = 19;
      this.label3.Text = "Thanks to the MediaPortal team for a great product:";
      // 
      // label2
      // 
      this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label2.Location = new System.Drawing.Point(8, 30);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(130, 22);
      this.label2.TabIndex = 18;
      this.label2.Text = "Updated by:";
      this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
      // 
      // label1
      // 
      this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label1.Location = new System.Drawing.Point(136, 30);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(213, 22);
      this.label1.TabIndex = 17;
      this.label1.Text = "TesterBoy";
      this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
      // 
      // linkLabel4
      // 
      this.linkLabel4.AutoSize = true;
      this.linkLabel4.Location = new System.Drawing.Point(80, 104);
      this.linkLabel4.Name = "linkLabel4";
      this.linkLabel4.Size = new System.Drawing.Size(151, 13);
      this.linkLabel4.TabIndex = 16;
      this.linkLabel4.TabStop = true;
      this.linkLabel4.Text = "http://www.mediaportal-fr.com";
      // 
      // linkLabel1
      // 
      this.linkLabel1.Location = new System.Drawing.Point(80, 155);
      this.linkLabel1.Name = "linkLabel1";
      this.linkLabel1.Size = new System.Drawing.Size(272, 22);
      this.linkLabel1.TabIndex = 15;
      this.linkLabel1.TabStop = true;
      this.linkLabel1.Text = "http://www.team-mediaportal.com";
      // 
      // label12
      // 
      this.label12.Location = new System.Drawing.Point(80, 80);
      this.label12.Name = "label12";
      this.label12.Size = new System.Drawing.Size(272, 24);
      this.label12.TabIndex = 14;
      this.label12.Text = "Thanks Scubefr from MediaPortal French forum:";
      // 
      // label11
      // 
      this.label11.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label11.Location = new System.Drawing.Point(8, 80);
      this.label11.Name = "label11";
      this.label11.Size = new System.Drawing.Size(56, 24);
      this.label11.TabIndex = 13;
      this.label11.Text = "Credits :";
      // 
      // label14
      // 
      this.label14.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label14.Location = new System.Drawing.Point(8, 8);
      this.label14.Name = "label14";
      this.label14.Size = new System.Drawing.Size(130, 22);
      this.label14.TabIndex = 11;
      this.label14.Text = "Original Author:";
      this.label14.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
      // 
      // label13
      // 
      this.label13.Location = new System.Drawing.Point(8, 177);
      this.label13.Name = "label13";
      this.label13.Size = new System.Drawing.Size(344, 22);
      this.label13.TabIndex = 9;
      this.label13.Text = "This plugin uses, but is not endorsed or certified by, Skype";
      this.label13.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
      // 
      // label10
      // 
      this.label10.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label10.Location = new System.Drawing.Point(136, 8);
      this.label10.Name = "label10";
      this.label10.Size = new System.Drawing.Size(216, 22);
      this.label10.TabIndex = 8;
      this.label10.Text = "Sony Tricoire as Maniac\'s";
      this.label10.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
      // 
      // linkLabel3
      // 
      this.linkLabel3.Location = new System.Drawing.Point(80, 40);
      this.linkLabel3.Name = "linkLabel3";
      this.linkLabel3.Size = new System.Drawing.Size(272, 23);
      this.linkLabel3.TabIndex = 12;
      this.linkLabel3.TabStop = true;
      this.linkLabel3.Text = "http://www.prodigia.org/digimatrix";
      // 
      // linkLabel2
      // 
      this.linkLabel2.Location = new System.Drawing.Point(0, 0);
      this.linkLabel2.Name = "linkLabel2";
      this.linkLabel2.Size = new System.Drawing.Size(100, 23);
      this.linkLabel2.TabIndex = 0;
      // 
      // SkypeSetupForm
      // 
      this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
      this.ClientSize = new System.Drawing.Size(386, 280);
      this.ControlBox = false;
      this.Controls.Add(this.tabControl1);
      this.Controls.Add(this.button2);
      this.Controls.Add(this.button1);
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = "SkypeSetupForm";
      this.ShowInTaskbar = false;
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
      this.Text = "Skype Plugin Configuration";
      this.Load += new System.EventHandler(this.SkypeSetupForm_Load);
      this.tabControl1.ResumeLayout(false);
      this.tPGeneral.ResumeLayout(false);
      this.tPGeneral.PerformLayout();
      this.tpVoiceVideo.ResumeLayout(false);
      this.tpVoiceVideo.PerformLayout();
      this.tpChat.ResumeLayout(false);
      this.tpChat.PerformLayout();
      this.tPSkypeCredit.ResumeLayout(false);
      this.tPSkypeCredit.PerformLayout();
      this.tPAbout.ResumeLayout(false);
      this.tPAbout.PerformLayout();
      this.ResumeLayout(false);

    }
    #endregion

    private void button1_Click(object sender, System.EventArgs e)
    {
      this.Close();
    }

    /// <summary>
    /// Defines the link between a Settings file parameter and the button on the SetupForm.
    /// </summary>
    private void InitOptionList()
    {
      opts.Add("StartWithMP", cBStartWithMP);
      opts.Add("StopWithMP", cBStopWithMP);
      opts.Add("HangupIfDND", cBHangupIfDND);
      opts.Add("HandleChats", cBHandleChats);
      opts.Add("HandleVoiceCalls", cBHandleVoiceCalls);
      opts.Add("AllowSkypeOutCalls", cBAllowSkypeOut);
      opts.Add("AllowSMS", cBAllowSMS);
      opts.Add("PauseForIncomingCalls", cBPauseForCalls);
      opts.Add("PauseForIncomingChats", cBPauseForChats);
      opts.Add("UseSilentMode", cbUseSilentMode);
      opts.Add("IgnoreIncomingCalls", cBIgnoreIncomingCalls);
    }

    private void SkypeSetupForm_Load(object sender, System.EventArgs e)
    {
      // Get values from settings file
      if (options.Count == 0)
      {
        options.LoadList();
      }

      // Link buttons to settings
      if (opts.Count == 0)
      {
        InitOptionList();
      }

      // Set button values appropriately
      foreach (string ot in options.Keys)
      {
        opts[ot].Checked = options[ot];
      }

      // Enabled/disable the sub-options
      cBHandleVoiceCalls_CheckedChanged(null, null);
      cBHandleChats_CheckedChanged(null, null);
    }

    private void button2_Click(object sender, System.EventArgs e)
    {
      Save();
      Close();
    }

    /// <summary>
    /// Save the current vaklues of the settings to the xml file
    /// </summary>
    public void Save()
    {
      // Copy the current button values to the settings
      foreach (string ot in opts.Keys)
      {
        options[ot] = opts[ot].Checked;
      }
      options.SaveList();

    }

    private void cBHandleVoiceCalls_CheckedChanged(object sender, EventArgs e)
    {
      cBHangupIfDND.Enabled = cBHandleVoiceCalls.Checked;
      cBPauseForCalls.Enabled = cBHandleVoiceCalls.Checked;
      cBIgnoreIncomingCalls.Enabled = cBHandleVoiceCalls.Checked;
    }

    private void cBHandleChats_CheckedChanged(object sender, EventArgs e)
    {
      cBPauseForChats.Enabled = cBHandleChats.Checked;
    }

  }
}
