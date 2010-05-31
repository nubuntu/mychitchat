using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyChitChat;


namespace MyChitChat.Test {
    class Program {
        static void Main(string[] args) {
            Plugin.Plugin myChitChat = new MyChitChat.Plugin.Plugin();
            myChitChat.ShowPlugin();
        }
    }
}
