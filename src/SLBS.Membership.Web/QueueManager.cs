using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SLBS.Membership.Domain;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;
using SLBS.Membership.Web.Models;
using NLog;

namespace SLBS.Membership.Web
{
    
    public class QueueManager
    {
        private const string _queueName = "slsbs-mailqueue";
        private CloudQueueClient _queueClient;
        private Logger log = LogManager.GetCurrentClassLogger();

        public QueueManager()
        {
            var storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("StorageConnectionString"));
            _queueClient = storageAccount.CreateCloudQueueClient();
            var queue = _queueClient.GetQueueReference(_queueName);
            queue.CreateIfNotExists();
        }

        public void InsertMail(int membershipId, string email, EnumNoticeTypes noticeType)
        {
            CloudQueue queue = _queueClient.GetQueueReference(_queueName);

            // Create the queue if it doesn't already exist.
            log.Debug("About to create queue if not exists");
            queue.CreateIfNotExists();

            // Create a message and add it to the queue.
            var emailMessage = JsonConvert.SerializeObject(new { MembershipId = membershipId, Email = email, NoticeType = noticeType });
            CloudQueueMessage message = new CloudQueueMessage(emailMessage);
            queue.AddMessage(message);
            log.Debug(string.Format("Added message {0} to queue ", emailMessage));
        }

    }
}