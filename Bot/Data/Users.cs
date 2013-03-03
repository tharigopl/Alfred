using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bot.Data
{
    public class Users : IModelBase
    {
        private string id;
        public string Id 
        { 
            get { return "users"; } 
            set { id = value; }
        }

        public List<User> Items { get; set; }

        public Users()
        {
            this.Items = new List<User>();
        }
    }
}
