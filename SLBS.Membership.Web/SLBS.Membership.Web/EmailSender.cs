using SLBS.Membership.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Net.Http;
using System.Threading.Tasks;
using WebGrease.Css.Extensions;

namespace SLBS.Membership.Web
{
    public class EmailSender
    {
        private string ApiKey = "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";
        private List<string> _emailList;

        public EmailSender(List<string> emailList)
        {
            _emailList = emailList;
        }

        public async Task<int> SendAll()
        {
            int count = 0;
            foreach (var email in _emailList)
            {
                if (IsValidEmail(email))
                {
                    await Send(email);
                    count++;
                }
                
            }

            return count;
        }

        private async Task Send(string email)
        {
            var myMessage = new SendGrid.SendGridMessage();
            myMessage.AddTo("slbsmembershipstatus@gmail.com");
            myMessage.From = new MailAddress("slbsmembershipstatus@uchithar.net", "SLSBS Admin");
            myMessage.Subject = string.Format("Sending email to {0}", email);
            myMessage.Text = "Test mail from SLBS Membership";

            var transportWeb = new SendGrid.Web(ApiKey);
            await transportWeb.DeliverAsync(myMessage);
        }

        private bool IsValidEmail(string email)
        {
            return email.Contains("@") ;
        }
    }
}