﻿using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using MyChitChat.Jabber;
using MediaPortal.UserInterface.Controls;

namespace MyChitChat.Plugin
{
    public partial class Config : MPConfigForm
    {
        private Client _testClient = null;
        private Thread backgroundWorker = null;

        public Config()
        {
            InitializeComponent();
            this.Load += new EventHandler(MyChitChatConfig_Load);
            this.Text += Helper.PLUGIN_NAME;            
        }

        void MyChitChatConfig_Load(object sender, EventArgs e) {           

            Settings.Load();

            textBoxUsername.Text = Settings.username;
            textBoxServer.Text = Settings.server;
            textBoxResource.Text = Settings.resource;
            textBoxPassword.Text = Settings.password;
        }

        /// <summary>
        /// Open the jabber FAQ in the default browser
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void linkFAQ_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("http://www.jabber.org/index.php/faq/");
        }

        /// <summary>
        /// Open the jabber.org register account page
        /// in the default browser.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void linkLabelCreateAccount_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("http://register.jabber.org/");
        }

        /// <summary>
        /// Test the settings
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonTest_Click(object sender, EventArgs e)
        {
            buttonTest.Text = "Testing ...";
            buttonTest.Enabled = false;
            labelStatus.Visible = false;

            try
            {
                // Test the jabber credentials in a background thread
                if (backgroundWorker != null && backgroundWorker.IsAlive)
                {
                    backgroundWorker.Abort();
                }
                backgroundWorker = new Thread(TestJabber);
                backgroundWorker.IsBackground = true;
                backgroundWorker.Start();
            }
            catch (Exception ex) 
            {
                MediaPortal.GUI.Library.Log.Error(String.Format("Error spawning test thread: {0}", ex.Message));
            }
        }

        /// <summary>
        /// Method to do the jabber testing
        /// </summary>
        private void TestJabber()
        {
            _testClient = Client.Instance;
            _testClient.OnError += new OnErrorEventHandler(_testClient_OnError);
            _testClient.OnLogin += new OnLoginEventHandler(_testClient_OnLogin);
            _testClient.Login();
        }

        void _testClient_OnLogin(object sender) {
            SetTestResponse(new TestEventArgs(true));
        }

        void _testClient_OnError(nJim.Enums.ErrorType type, string message) {
            SetTestResponse(new TestEventArgs(false, type, message));
        }

        delegate void RefreshGuiComponentsCallBack(TestEventArgs response);
        /// <summary>
        /// Display the test response in the setup form
        /// </summary>
        /// <param name="response"></param>  
        
        private void SetTestResponse(TestEventArgs response)
        {
            // Update the GUI thread-safe
            if (labelStatus.InvokeRequired || buttonTest.InvokeRequired) {
                RefreshGuiComponentsCallBack refreshGuiComponentsCallBack = new RefreshGuiComponentsCallBack(SetTestResponse);
                this.Invoke(refreshGuiComponentsCallBack, new object[] { });
            }
            if (response.Success)
            {
                labelStatus.ForeColor = Color.Green;
                labelStatus.Text = "Everything OK!";
            }
            else
            {
                labelStatus.ForeColor = Color.Red;

                // Auth error
                if (response.ErrorType.HasValue)
                {
                    labelStatus.Text = "An error occured! Login may be ok ... " +response.ErrorMessage;
                }               
            }

            labelStatus.Visible = true;
            buttonTest.Text = "Test";
            buttonTest.Enabled = true;
         }
       

        /// <summary>
        /// Form is closing, save data to mediaportal config
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SetupForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Settings.Save();
        }

        /// <summary>
        /// Update the username setting
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBoxUsername_Leave(object sender, EventArgs e)
        {
            Settings.username = textBoxUsername.Text;
        }

        /// <summary>
        /// Update the server setting
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBoxServer_Leave(object sender, EventArgs e)
        {
            Settings.server = textBoxServer.Text;
        }

        /// <summary>
        /// Update the password setting
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBoxPassword_Leave(object sender, EventArgs e)
        {
            Settings.password = textBoxPassword.Text;
        }

        /// <summary>
        /// Update the resource setting
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBoxResource_Leave(object sender, EventArgs e)
        {
            Settings.resource = textBoxResource.Text;
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

            public TestEventArgs(bool successful) : this(successful, null, String.Empty){                 
            }
            
            public TestEventArgs(bool successful, nJim.Enums.ErrorType? errorType, string ErrorMessage ) {
                _success = successful;
                _errorType = errorType;
                _errorMessage = ErrorMessage;
            }
        }
}