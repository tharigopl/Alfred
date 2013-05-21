using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Amazon;
using Amazon.ElasticLoadBalancing;
using Amazon.ElasticLoadBalancing.Model;

namespace Bot.Infrastructure.AWS
{
    public class ELB : AwsClient
    {
        private readonly AmazonElasticLoadBalancing client;

        public ELB()
        {
            this.client = ElbClient();
        }

        public List<LoadBalancerDescription> List(string loadBalancerName = null)
        {
            var request = new DescribeLoadBalancersRequest();

            if (!string.IsNullOrEmpty(loadBalancerName))
                request.LoadBalancerNames.Add(loadBalancerName);

            var response = client.DescribeLoadBalancers(request);
            return response.DescribeLoadBalancersResult.LoadBalancerDescriptions;
        }

        public List<Instance> Instances(string loadBalancerName)
        {
            if (string.IsNullOrEmpty(loadBalancerName))
                throw new ArgumentNullException("loadBalancerName");

            return List(loadBalancerName)
                    .Select(d => d.Instances)
                    .FirstOrDefault();
        }

        public List<InstanceState> InstanceState(string loadBalancerName)
        {
            if (string.IsNullOrEmpty(loadBalancerName))
                throw new ArgumentNullException("loadBalancerName");

            try
            {
                var result = client.DescribeInstanceHealth(new DescribeInstanceHealthRequest(loadBalancerName));
                return result.DescribeInstanceHealthResult.InstanceStates;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception occurred getting instance states: ");
                Console.WriteLine(ex.Message);
                return new List<InstanceState>();
            }
        } 

        private AmazonElasticLoadBalancing ElbClient()
        {
            return AWSClientFactory.CreateAmazonElasticLoadBalancingClient(
                this.Credentials, 
                RegionEndpoint.APNortheast1
            );
        }
    }
}
