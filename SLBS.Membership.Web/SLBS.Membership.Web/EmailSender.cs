using SLBS.Membership.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Net.Http;
using System.Threading.Tasks;

namespace SLBS.Membership.Web
{
    public class EmailSender
    {
        private string ApiKey = "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";
        private List<Member> _memberList;

        public EmailSender(List<Member> memberList)
        {
            _memberList = memberList;
        }

        public async Task SendAll()
        {
            await Send();
        }

        private async Task Send()
        {
            var myMessage = new SendGrid.SendGridMessage();
            myMessage.AddTo("slbsmembershipstatus@gmail.com");
            myMessage.From = new MailAddress("slbsmembershipstatus@uchithar.net", "First Last");
            myMessage.Subject = "Sending with SendGrid is Fun";
            myMessage.Text = "and easy to do anywhere, even with C#";

            var transportWeb = new SendGrid.Web(ApiKey);
            await transportWeb.DeliverAsync(myMessage);
        }
    }
}