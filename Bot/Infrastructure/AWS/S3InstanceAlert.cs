using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bot.Infrastructure.AWS
{
    public class S3InstanceAlert : IInstanceAlert
    {
        public bool TryUpdateAlert(int instancesOutOfService)
        {
            var s3 = new S3();
            return s3.TryPut(
                "instance-alerts", 
                "sometaro-elb.txt", 
                instancesOutOfService.ToString()
            );
        }
    }
}
