using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bot
{
    public class IrcBotConfiguration
    {
        public string HostName { get; set; }
        public int Port { get; set; }
        public string Channel { get; set; }

        public string NickName { get; set; }
        public string UserName { get; set; }
        public string RealName { get; set; }
    }
}
