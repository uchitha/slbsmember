using Microsoft.Azure.WebJobs;
using Microsoft.WindowsAzure.Storage.Queue;
using SLBS.Membership.WebJobEmail;
using System.IO;

namespace SLSBS.Membership.WebJob
{
    public class Functions
    {
        public static void ProcessEmailQueue([QueueTrigger("slsbs-mailqueue")]CloudQueueMessage message, TextWriter log)
        {
            dynamic payload = Newtonsoft.Json.JsonConvert.DeserializeObject(message.AsString);

            var memberId = (int)payload.MembershipId.Value;
            var email = payload.Email.Value;
            var noticeType = (EnumNoticeTypes)(int)payload.NoticeType.Value;

            var sender = new EmailSender(EnumMode.Membership);
            sender.SendMail(memberId, email);

            log.WriteLine("Message processed : Member {0}, Email {1}",memberId, email);
        }



    }
}
