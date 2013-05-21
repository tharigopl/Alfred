using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Amazon;
using Amazon.Route53;
using Amazon.Route53.Model;

namespace Bot.Infrastructure.AWS
{
    class Route53 : AwsClient
    {
        private AmazonRoute53 client;

        public Route53()
        {
            this.client = Client();
        }

        public void Something()
        {
            var domainName = "sometaro.tokyo.tradestation.io";
            var primaryIp = "176.34.55.115";
            var secondaryIp = "176.34.41.13";
            var healthCheckId = "0e890ebf-f6c3-47c8-8994-1d5781823191";
            var setIdPrefix = "insight";


        }

        private List<ResourceRecordSet> DefaultRecordSets(
            string domainName,
            string primaryIp,
            string secondaryIp,
            string healthCheckId,
            string setIdPrefix
            )
        {
            var primaryRecordSet = new ResourceRecordSet()
                .WithName(domainName)
                .WithType("A")
                .WithTTL(30)
                .WithResourceRecords(
                    new ResourceRecord().WithValue(primaryIp)
                )
                .WithHealthCheckId(healthCheckId)
                .WithFailover("PRIMARY")
                .WithSetIdentifier(setIdPrefix + "-primary");

            var secondaryRecordSet = new ResourceRecordSet()
                .WithName(domainName)
                .WithType("A")
                .WithTTL(30)
                .WithResourceRecords(
                    new ResourceRecord().WithValue(secondaryIp)
                )
                .WithFailover("SECONARY")
                .WithSetIdentifier(setIdPrefix + "-secondary");

            return new List<ResourceRecordSet> {
                primaryRecordSet,
                secondaryRecordSet
            };
        }

        private List<ResourceRecordSet> MaintenanceRecordSets(string domainName, string ip)
        {
            var primaryRecordSet = new ResourceRecordSet()
                .WithName(domainName)
                .WithType("A")
                .WithTTL(30)
                .WithResourceRecords(
                    new ResourceRecord().WithValue(ip)
                );

            return new List<ResourceRecordSet> {
                primaryRecordSet
            };
        } 

        private AmazonRoute53 Client()
        {
            return AWSClientFactory.CreateAmazonRoute53Client(
                this.Credentials,
                RegionEndpoint.APNortheast1
            );
        }
    }
}
