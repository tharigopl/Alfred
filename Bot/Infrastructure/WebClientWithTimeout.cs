using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Bot.Infrastructure
{
    public class WebClientWithTimeout : WebClient
    {
        private readonly int timeout;

        public WebClientWithTimeout(int timeout = 5000)
        {
            this.timeout = timeout;
        }

        protected override WebRequest GetWebRequest(Uri address)
        {
            var req = base.GetWebRequest(address);

            if (req != null)
                req.Timeout = this.timeout;

            return req;
        }
    }
}
