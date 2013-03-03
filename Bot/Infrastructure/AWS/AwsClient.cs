using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Amazon.Runtime;

namespace Bot.Infrastructure.AWS
{
    public class AwsClient
    {
        private readonly string key;
        private readonly string secret;
        private readonly AWSCredentials credentials;

        public AwsClient()
        {
            this.key = ConfigurationManager.AppSettings["AWSKey"];
            this.secret = ConfigurationManager.AppSettings["AWSSecret"];

            if (string.IsNullOrEmpty(this.key))
                throw new ArgumentNullException("key", "Missing AWSKey appSettings entry.");

            if (string.IsNullOrEmpty(this.secret))
                throw new ArgumentNullException("secret", "Missing AWSSecret appSettings entry.");

            this.credentials = new BasicAWSCredentials(this.key, this.secret);
        }

        protected AWSCredentials Credentials { get { return this.credentials; } }
    }
}
