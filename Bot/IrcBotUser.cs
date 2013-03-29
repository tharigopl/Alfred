using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bot
{
    public class IrcBotUser
    {
        public string UserName { get; set; }
        public string NickName { get; set; }
        public bool IsAdmin { get; set; }
        public bool IsBotAdmin { get; set; }
    }
}
