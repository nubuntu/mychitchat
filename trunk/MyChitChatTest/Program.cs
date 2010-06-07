using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyChitChat;
using MyChitChat.Plugin;
using System.Windows.Forms;

namespace MyChitChat.Test {
    class Program {

        [STAThread]
        static void Main(string[] args) {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Config());            
        }
    }
}
