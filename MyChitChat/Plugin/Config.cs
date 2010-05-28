using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using MyChitChat.Jabber;
using MediaPortal.UserInterface.Controls;

namespace MyChitChat.Plugin
{
    public partial class Config : MPConfigForm
    {
        delegate void SetTestReponse(TestEventArgs response);

        private Client jabber = null;
        private Thread backgroundWorker = null;

        public Config()
        {
            InitializeComponent();
            this.Load += new EventHandler(MyChitChatConfig_Load);
            this.Text += Helper.PLUGIN_NAME;            
        }

        void MyChitChatConfig_Load(object sender, EventArgs e) {           

            Settings.Load();

            textBoxUsername.Text = Settings.Username;
            textBoxServer.Text = Settings.Server;
            textBoxResource.Text = Settings.Resource;
            textBoxPassword.Text = Settings.Password;
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
            jabber = new Client();

            jabber.TestCompleted += new TestCompletedEventHandler(jabber_TestCompleted);
            jabber.TestSettings();
            jabber.Connect();
        }

        /// <summary>
        /// Display the test response in the setup form
        /// </summary>
        /// <param name="response"></param>
        private void setTestResponse(TestEventArgs response)
        {
            if (response.Success)
            {
                labelStatus.ForeColor = Color.Green;
                labelStatus.Text = "Everything OK!";
            }
            else
            {
                labelStatus.ForeColor = Color.Red;

                // Auth error
                if (response.Exception == null)
                {
                    labelStatus.Text = "Login not OK!";
                }
                // Exception while testing connection
                else
                {
                    labelStatus.Text = "An error occured! Login may be ok ...";
                }
            }

            labelStatus.Visible = true;
            buttonTest.Text = "Test";
            buttonTest.Enabled = true;
         }

        /// <summary>
        /// Event is raised when the credential test has been completed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void jabber_TestCompleted(object sender, TestEventArgs e)
        {
            jabber.Disconnect();

            // Update the GUI thread-safe
            if (labelStatus.InvokeRequired || buttonTest.InvokeRequired)
            {
                SetTestReponse setResponseDelegate = new SetTestReponse(setTestResponse);
                this.Invoke(setResponseDelegate, new Object[] { e });
            }
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
            Settings.Username = textBoxUsername.Text;
        }

        /// <summary>
        /// Update the server setting
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBoxServer_Leave(object sender, EventArgs e)
        {
            Settings.Server = textBoxServer.Text;
        }

        /// <summary>
        /// Update the password setting
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBoxPassword_Leave(object sender, EventArgs e)
        {
            Settings.Password = textBoxPassword.Text;
        }

        /// <summary>
        /// Update the resource setting
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBoxResource_Leave(object sender, EventArgs e)
        {
            Settings.Resource = textBoxResource.Text;
        }
    }
}
