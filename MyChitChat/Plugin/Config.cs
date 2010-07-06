using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using MyChitChat.Jabber;
using MediaPortal.UserInterface.Controls;
using nJim;
using MyChitChat.Gui;
using System.Diagnostics;
using System.Text;

namespace MyChitChat.Plugin {
    public partial class Config : MPConfigForm {
       
        public Config() {
            InitializeComponent();
            this.Load += new EventHandler(MyChitChatConfig_Load);
        }

        void MyChitChatConfig_Load(object sender, EventArgs e) {

            Settings.Load();

            textBoxUsername.Text = Settings.username;
            textBoxServer.Text = Settings.server;
            textBoxResource.Text = Settings.resource;
            textBoxPassword.Text = Settings.password;

            comboBoxStartupStatus.DataSource = Translations.EnumToList<Enums.StatusType>();
            textBoxStartupStatus.Text = Settings.defaultStatusMessage;
            comboBoxStartupStatus.SelectedIndex = (int)Settings.defaultStatusType;            
            comboBoxStartupStatus.SelectionChangeCommitted += new EventHandler(comboBoxStartupStatus_SelectionChangeCommitted);
            
            comboBoxAutoIdleStatus.DataSource = Translations.EnumToList<Enums.StatusType>();
            textBoxAutoIdleStatus.Text = Settings.autoIdleStatusMessage;
            comboBoxAutoIdleStatus.SelectedIndex = (int)Settings.autoIdleStatusType;
            comboBoxAutoIdleStatus.SelectedIndexChanged += new EventHandler(comboBoxAutoIdleStatus_SelectedIndexChanged);
            
            comboBoxStartupActivity.DataSource = Translations.EnumToList<Enums.ActivityType>();
            textBoxStartupActivity.Text = Settings.defaultActivityMessage;
            comboBoxStartupActivity.SelectedIndex = (int)Settings.defaultActivityType;
            comboBoxStartupActivity.SelectionChangeCommitted += new EventHandler(comboBoxStartupActivity_SelectionChangeCommitted);
            
            comboBoxStartupMood.DataSource = Translations.EnumToList<Enums.MoodType>();
            textBoxStartupMood.Text = Settings.defaultMoodMessage;
            comboBoxStartupMood.SelectedIndex = (int)Settings.defaultMoodType;
            comboBoxStartupMood.SelectionChangeCommitted += new EventHandler(comboBoxStartupMood_SelectionChangeCommitted);
            
            comboBoxWindowSize.DataSource = Translations.EnumToList<Helper.PLUGIN_NOTIFY_WINDOWS>();
            comboBoxWindowSize.SelectionChangeCommitted += new EventHandler(comboBoxWindowSize_SelectionChangeCommitted);
            comboBoxWindowSize.SelectedIndex = (int)Settings.notifyWindowType;

            numericUpDownIdleTimeOut.Value = Settings.autoIdleTimeOut;
            numericUpDownIdleTimeOut.Leave += new EventHandler(numericUpDownIdleTimeOut_Leave);

            checkBoxSelectOnStartup.Checked = Settings.selectStatusOnStartup;
            checkBoxMessageGlobally.Checked = Settings.notifyOnMessageGlobally;
            checkBoxMessagePlugin.Checked = Settings.notifyOnMessagePlugin;
            checkBoxStatusGlobally.Checked = Settings.notifyOnStatusGlobally;
            checkBoxStatusPlugin.Checked = Settings.notifyOnStatusPlugin;
            checkBoxActivityGlobally.Checked = Settings.notifyOnActivityGlobally;
            checkBoxActivityPlugin.Checked = Settings.notifyOnActivityPlugin;
            checkBoxMoodGlobally.Checked = Settings.notifyOnMoodGlobally;
            checkBoxMoodPlugin.Checked = Settings.notifyOnMoodPlugin;
            checkBoxErrorGlobally.Checked = Settings.notifyOnErrorGlobally;
            checkBoxErrorPlugin.Checked = Settings.notifyOnErrorPlugin;
            checkBoxTuneGlobally.Checked = Settings.notifyOnTuneGlobally;
            checkBoxTunePlugin.Checked = Settings.notifyOnTunePlugin;

            comboBoxKeyboardType.DataSource = Translations.EnumToList<Dialog.KeyBoardTypes>();
            comboBoxKeyboardType.SelectionChangeCommitted += new EventHandler(comboBoxKeyboardType_SelectionChangeCommitted);
            comboBoxKeyboardType.SelectedIndex = (int)Dialog.KeyBoardTypes.Default;

            comboBoxLanguage.DataSource = Translations.GetCultureLanguages();
            comboBoxLanguage.SelectedItem = Translations.GetCurrentCultureLanguage();

        }

        void comboBoxAutoIdleStatus_SelectedIndexChanged(object sender, EventArgs e) {
            Settings.autoIdleStatusType = ((KeyValuePair<string, Enums.StatusType>)((ComboBox)sender).SelectedItem).Value;
            textBoxAutoIdleStatus.Text = ((KeyValuePair<string, Enums.StatusType>)((ComboBox)sender).SelectedItem).Key;
        }

        void comboBoxKeyboardType_SelectionChangeCommitted(object sender, EventArgs e) {
            Settings.defaultKeyboardType = ((KeyValuePair<string, Dialog.KeyBoardTypes>)((ComboBox)sender).SelectedItem).Value;
        }

        void numericUpDownIdleTimeOut_Leave(object sender, EventArgs e) {
            Settings.autoIdleTimeOut = (int)((NumericUpDown)sender).Value;
        }

        void comboBoxWindowSize_SelectionChangeCommitted(object sender, EventArgs e) {
            Settings.notifyWindowType = ((KeyValuePair<string, Helper.PLUGIN_NOTIFY_WINDOWS>)((ComboBox)sender).SelectedItem).Value;
        }

        void comboBoxStartupStatus_SelectionChangeCommitted(object sender, EventArgs e) {
            Settings.defaultStatusType = ((KeyValuePair<string, Enums.StatusType>)((ComboBox)sender).SelectedItem).Value;
            textBoxStartupStatus.Text = ((KeyValuePair<string, Enums.StatusType>)((ComboBox)sender).SelectedItem).Key;
        }

        void comboBoxStartupActivity_SelectionChangeCommitted(object sender, EventArgs e) {
            Settings.defaultActivityType = ((KeyValuePair<string, Enums.ActivityType>)((ComboBox)sender).SelectedItem).Value;
            textBoxStartupActivity.Text = ((KeyValuePair<string, Enums.ActivityType>)((ComboBox)sender).SelectedItem).Key;
        }

        void comboBoxStartupMood_SelectionChangeCommitted(object sender, EventArgs e) {
            Settings.defaultMoodType = ((KeyValuePair<string, Enums.MoodType>)((ComboBox)sender).SelectedItem).Value;
            textBoxStartupMood.Text = ((KeyValuePair<string, Enums.MoodType>)((ComboBox)sender).SelectedItem).Key;
        }

        /// <summary>
        /// Open the jabber FAQ in the default browser
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void linkFAQ_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            System.Diagnostics.Process.Start(new UriBuilder("http://www.jabber.org/index.php/faq/").ToString());
        }

        /// <summary>
        /// Open the jabber.org register account page
        /// in the default browser.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void linkLabelCreateAccount_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            System.Diagnostics.Process.Start(new UriBuilder("http://register.jabber.org/").ToString());
        }

        

       
        delegate void RefreshGuiComponentsCallBack(TestEventArgs response);
        


        /// <summary>
        /// Form is closing, save data to mediaportal config
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SetupForm_FormClosing(object sender, FormClosingEventArgs e) {
            Settings.Save();
        }

        /// <summary>
        /// Update the username setting
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBoxUsername_Leave(object sender, EventArgs e) {
            Settings.username = textBoxUsername.Text;
        }

        /// <summary>
        /// Update the server setting
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBoxServer_Leave(object sender, EventArgs e) {
            Settings.server = textBoxServer.Text;
        }

        /// <summary>
        /// Update the password setting
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBoxPassword_Leave(object sender, EventArgs e) {
            Settings.password = textBoxPassword.Text;
        }

        /// <summary>
        /// Update the resource setting
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBoxResource_Leave(object sender, EventArgs e) {
            Settings.resource = textBoxResource.Text;
        }

        private void textBoxStartupStatus_TextChanged(object sender, EventArgs e) {
            Settings.defaultStatusMessage = textBoxStartupStatus.Text;
        }

        private void textBoxAutoIdleStatus_TextChanged(object sender, EventArgs e) {
            Settings.autoIdleStatusMessage = textBoxAutoIdleStatus.Text;
        }

        private void textBoxStartupActivity_TextChanged(object sender, EventArgs e) {
            Settings.defaultActivityMessage = textBoxStartupActivity.Text;
        }

        private void textBoxStartupMood_TextChanged(object sender, EventArgs e) {
            Settings.defaultMoodMessage = textBoxStartupMood.Text;
        }

        private void checkBoxSelectOnStartup_CheckedChanged(object sender, EventArgs e) {
            Settings.selectStatusOnStartup = checkBoxSelectOnStartup.Checked;
        }

        private void checkBoxMessageGlobally_CheckedChanged(object sender, EventArgs e) {
            Settings.notifyOnMessageGlobally = checkBoxMessageGlobally.Checked;
        }

        private void checkBoxMessagePlugin_CheckedChanged(object sender, EventArgs e) {
            Settings.notifyOnMessagePlugin = checkBoxMessagePlugin.Checked;
        }

        private void checkBoxStatusGlobally_CheckedChanged(object sender, EventArgs e) {
            Settings.notifyOnStatusGlobally = checkBoxStatusGlobally.Checked;
        }

        private void checkBoxStatusPlugin_CheckedChanged(object sender, EventArgs e) {
            Settings.notifyOnStatusPlugin = checkBoxStatusPlugin.Checked;
        }

        private void checkBoxActivityGlobally_CheckedChanged(object sender, EventArgs e) {
            Settings.notifyOnActivityGlobally = checkBoxActivityGlobally.Checked;
        }

        private void checkBoxActivityPlugin_CheckedChanged(object sender, EventArgs e) {
            Settings.notifyOnActivityPlugin = checkBoxActivityPlugin.Checked;
        }

        private void checkBoxMoodGlobally_CheckedChanged(object sender, EventArgs e) {
            Settings.notifyOnMoodGlobally = checkBoxMoodGlobally.Checked;
        }

        private void checkBoxMoodPlugin_CheckedChanged(object sender, EventArgs e) {
            Settings.notifyOnMoodPlugin = checkBoxMoodPlugin.Checked;
        }

        private void checkBoxErrorGlobally_CheckedChanged(object sender, EventArgs e) {
            Settings.notifyOnErrorGlobally = checkBoxErrorGlobally.Checked;
        }

        private void checkBoxErrorPlugin_CheckedChanged(object sender, EventArgs e) {
            Settings.notifyOnErrorPlugin = checkBoxErrorPlugin.Checked;
        }

        private void checkBoxTuneGlobally_CheckedChanged(object sender, EventArgs e) {
            Settings.notifyOnTuneGlobally = checkBoxTuneGlobally.Checked;
        }

        private void checkBoxTunePlugin_CheckedChanged(object sender, EventArgs e) {
            Settings.notifyOnTunePlugin = checkBoxTunePlugin.Checked;
        }

        private void button1_Click(object sender, EventArgs e) {
            string file = Translations.CreateTranslationTemplate(comboBoxLanguage.Text);
            StringBuilder infoMessage = new StringBuilder();
            infoMessage.AppendFormat("Creating new '{0}' language file: \n({1})", comboBoxLanguage.Text, file);
            infoMessage.Append(Environment.NewLine);
            infoMessage.Append(Environment.NewLine);
            infoMessage.AppendFormat("A new Issue for '{0}' on Google Code will be opened \nPlease remember to attach the new language file. \n\nKKTHXBY, \n{1}", Helper.PLUGIN_NAME, Helper.PLUGIN_AUTHOR);
            MessageBox.Show(infoMessage.ToString(), Helper.PLUGIN_NAME + " - Language file creation");

            System.Diagnostics.Process.Start(file);
            System.Diagnostics.Process.Start("iexplore",Helper.PLUGIN_LINK_NEW_ISSUE);
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            System.Diagnostics.Process.Start("iexplore", Helper.PLUGIN_LINK_NEW_ISSUE);


        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            System.Diagnostics.Process.Start("iexplore", Helper.PLUGIN_LINK_FORUM);
        }

        private void checkBoxCurrentTuneInfo_CheckedChanged(object sender, EventArgs e) {
            Settings.publishTuneInfo = checkBoxCurrentTuneInfo.Checked;
        }

        private void checkBoxPublishActivityRadio_CheckedChanged(object sender, EventArgs e) {
            Settings.publishActivityRadio = checkBoxPublishActivityRadio.Checked;
        }

        private void checkBoxPublishActivityMusic_CheckedChanged(object sender, EventArgs e) {
            Settings.publishActivityMusic = checkBoxPublishActivityMusic.Checked;
        }

        private void checkBoxPublishActivityMovie_CheckedChanged(object sender, EventArgs e) {
            Settings.publishActivityMovie = checkBoxPublishActivityMovie.Checked;
        }

        private void checkBoxPublishActivityTV_CheckedChanged(object sender, EventArgs e) {
            Settings.publishActivityTV = checkBoxPublishActivityTV.Checked;
        }

        private void checkBoxPublishActivityRecording_CheckedChanged(object sender, EventArgs e) {
            Settings.publishActivityRecording = checkBoxPublishActivityRecording.Checked;
        }

    }
    /// <summary>
    /// Event args for the test event
    /// </summary>
    public class TestEventArgs : EventArgs {
        /// <summary>
        /// Was the test successful?
        /// </summary>
        public bool Success {
            get { return _success; }
            set { _success = value; }
        }
        private bool _success;

        public nJim.Enums.ErrorType? ErrorType {
            get { return _errorType; }
            set { _errorType = value; }
        }

        private nJim.Enums.ErrorType? _errorType = null;

        public string ErrorMessage {
            get { return _errorMessage; }
            set { _errorMessage = value; }
        }

        private string _errorMessage = String.Empty;

        public TestEventArgs(bool successful)
            : this(successful, null, String.Empty) {
        }

        public TestEventArgs(bool successful, nJim.Enums.ErrorType? errorType, string ErrorMessage) {
            _success = successful;
            _errorType = errorType;
            _errorMessage = ErrorMessage;
        }
    }
}
