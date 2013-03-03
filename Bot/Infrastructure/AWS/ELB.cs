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
           //         .Where(d => d.LoadBalancerName.ToLower() == loadBalancerName.ToLower())
                    .Select(d => d.Instances)
                    .FirstOrDefault();
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
