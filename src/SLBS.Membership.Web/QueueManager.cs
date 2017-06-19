using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SLBS.Membership.Domain;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;

namespace SLBS.Membership.Web
{
    
    public class QueueManager
    {
        private const string _queueName = "slsbs-mailqueue";
        private CloudQueueClient _queueClient;

        public QueueManager()
        {
            var storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("StorageConnectionString"));
            _queueClient = storageAccount.CreateCloudQueueClient();
            var queue = _queueClient.GetQueueReference(_queueName);
            queue.CreateIfNotExists();
        }

        public void InsertMail(SLBS.Membership.Domain.Membership member, string email)
        {
            CloudQueue queue = _queueClient.GetQueueReference(_queueName);

            // Create the queue if it doesn't already exist.
            queue.CreateIfNotExists();

            // Create a message and add it to the queue.
            var emailMessage = new SlsbsEmailObject(member, email);
            CloudQueueMessage message = CloudQueueMessageExtensions.Serialize(emailMessage);
            queue.AddMessage(message);
        }

    }
}