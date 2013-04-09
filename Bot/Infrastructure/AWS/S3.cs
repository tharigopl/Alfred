using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Amazon;
using Amazon.S3;
using Amazon.S3.Model;

namespace Bot.Infrastructure.AWS
{
    public class S3 : AwsClient
    {
        private readonly AmazonS3 client;

        public S3()
        {
            this.client = S3Client();
        }

        public bool TryPut(string bucketName, string fileName, string contents, string contentType = "text/plain")
        {
            try
            {
                var response = this.client.PutObject(new PutObjectRequest() {
                    AutoCloseStream = true,
                    BucketName = bucketName,
                    Key = fileName,
                    ContentBody = contents,
                    ContentType = contentType,
                    CannedACL =  S3CannedACL.PublicRead
                });
            }
            catch (Exception ex)
            {
                // TODO: log
                Console.WriteLine(ex);
                return false;
            }

            return true;
        }

        private AmazonS3 S3Client()
        {
            return Amazon.AWSClientFactory.CreateAmazonS3Client(
                Credentials,
                RegionEndpoint.APNortheast1
            );
        }
    }
}
