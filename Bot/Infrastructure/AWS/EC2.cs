using System;
using System.Collections.Generic;
using System.Linq;
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

        public List<InstanceStatus> InstanceStatus(IEnumerable<string> instanceIds)
        {
            var request = new DescribeInstanceStatusRequest().WithInstanceId(instanceIds.ToArray());
            var response = this.client.DescribeInstanceStatus(request);
            return response.DescribeInstanceStatusResult.InstanceStatus;
        }

        public List<RunningInstance> InstanceDescriptions(IEnumerable<string> instanceIds)
        {
            try
            {
                var request = new DescribeInstancesRequest().WithInstanceId(instanceIds.ToArray());
                var response = this.client.DescribeInstances(request);

                return response
                    .DescribeInstancesResult
                    .Reservation
                    .SelectMany(reservation => reservation.RunningInstance)
                    .ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception occurred getting instance states: ");
                Console.WriteLine(ex.Message);
                return new List<RunningInstance>();
            }
        }

        public void RebootInstances(IEnumerable<string> instanceIds)
        {
            var request = new RebootInstancesRequest().WithInstanceId(instanceIds.ToArray());
            var response = this.client.RebootInstances(request);
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
