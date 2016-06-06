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
        private List<Member> _memeberList;
        private EnumMode _mode;

        public EmailSender(EnumMode mode)
        {
            _mode = mode;
        }

        public async Task<int> SendAll(List<Member> memberList)
        {
            int count = 0;
            foreach (var member in memberList)
            {
                if (IsValidEmail(member.Email))
                {
                    if (_mode == EnumMode.Membership)
                    {
                        await SendMembershipEmail(member);
                        count++;
                    }
                    else if (_mode == EnumMode.BuildingFund)
                    {
                        await SendBuildingFundEmail(member);
                        count++;
                    } 
                }
            }

            return count;
        }


        public async Task SendMembershipEmail(Member member)
        {
            var myMessage = new SendGrid.SendGridMessage();
            myMessage.AddTo("slbsmembershipstatus@gmail.com");
            myMessage.From = new MailAddress("slbsmembershipstatus@uchithar.net", "SLSBS Admin");
            myMessage.Subject = string.Format("Sending membership email to {0}", member.Email);
            myMessage.Text = string.Format("Received ${0} from {1} as SLSBS membership. Thank you very much for your support",member.Payment,member.MemberName);

            await Send(myMessage);
        }


        public async Task SendBuildingFundEmail(Member member)
        {
            var myMessage = new SendGrid.SendGridMessage();
            myMessage.AddTo("slbsmembershipstatus@gmail.com");
            myMessage.From = new MailAddress("slbsmembershipstatus@uchithar.net", "SLSBS Admin");
            myMessage.Subject = string.Format("Sending building fund email to {0}", member.Email);
            myMessage.Text = string.Format("Received ${0} from {1} for SLSBS building fund. Thank you very much for your support", member.Payment, member.MemberName);

            await Send(myMessage);
        }



        private async Task Send(SendGrid.SendGridMessage message)
        {
            var transportWeb = new SendGrid.Web(ApiKey);
            await transportWeb.DeliverAsync(message);
        }

        private bool IsValidEmail(string email)
        {
            return email.Contains("@") ;
        }
    }
}