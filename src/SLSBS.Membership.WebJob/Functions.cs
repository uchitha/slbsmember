using Microsoft.Azure.WebJobs;
using Microsoft.WindowsAzure.Storage.Queue;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SLSBS.Membership.WebJob
{
    public class Functions
    {
        public static void ProcessEmailQueue([QueueTrigger("slsbs-mailqueue")]CloudQueueMessage message, TextWriter log)
        {
            dynamic payload = Newtonsoft.Json.JsonConvert.DeserializeObject(message.AsString);

            log.WriteLine("Message processed");
        }
    }
}
