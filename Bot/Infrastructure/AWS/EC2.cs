using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Amazon;
using Amazon.EC2;
using Amazon.EC2.Model;

namespace Bot.Infrastructure.AWS
{
    public class EC2 : AwsClient
    {
        private readonly AmazonEC2 client;

        public EC2()
        {
            this.client = EC2Client();
        }

        public List<InstanceStatus> InstanceStatus(List<string> instanceIds)
        {
            var request = new DescribeInstanceStatusRequest().WithInstanceId(instanceIds.ToArray());
            var response = this.client.DescribeInstanceStatus(request);
            return response.DescribeInstanceStatusResult.InstanceStatus;
        }

        private AmazonEC2 EC2Client()
        {
            return AWSClientFactory.CreateAmazonEC2Client(
                this.Credentials, 
                RegionEndpoint.APNortheast1
            );
        }
    }
}
