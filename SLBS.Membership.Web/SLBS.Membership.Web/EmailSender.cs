using System.Configuration;
using System.Text;
using Exceptions;
using SLBS.Membership.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;

namespace SLBS.Membership.Web
{
    public class EmailSender
    {
        private const string MembershipTemplateId = "cbff537b-6b71-4d32-af3d-45363c5540a9";
        private const string BuildingTemplateId = "fce5f540-5873-438f-bd4b-f52cced63abf";
        private List<Member> _memeberList;
        private EnumMode _mode;

        public EmailSender(EnumMode mode)
        {
            _mode = mode;
        }

        public async Task SendMail(List<Domain.Membership> members, EnumNoticeTypes noticeType)
        {
            int count = 0;
            foreach (var member in members)
            {
                var emails = member.Adults.Select(a => a.Email);

                var toList = emails.Where(IsValidEmail).ToList();

                if (noticeType == EnumNoticeTypes.PaymentStatusDhammaSchool)
                {
                    await SendMembershipEmail(member);
                }
            }
        }

        public async Task<int> SendAll(List<Domain.Member> memberList)
        {
            int count = 0;
            foreach (var member in memberList)
            {
                if (IsValidEmail(member.Mother.Email))
                {
                    if (_mode == EnumMode.Membership)
                    {
                        //await SendMembershipEmail(member);
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


        //public async Task SendMembershipEmail(Member member)
        //{
        //    var myMessage = new SendGrid.SendGridMessage();
        //    myMessage.AddTo("slbsmembershipstatus@gmail.com"); //member.Email
        //    myMessage.From = new MailAddress("slbsmembershipstatus@uchithar.net", "SLSBS Treasurer");
        //    myMessage.Subject = string.Format("THANK YOU - Acknowledgement of Payments for Membership ({0})", member.Email);
        //    myMessage.Text = "Much Merits to You and Your Family members.";

        //    myMessage.EnableTemplateEngine(MembershipTemplateId);
        //    var amountString = string.Format("${0}", member.Payment);

        //    myMessage.AddSubstitution("-SECRETARY-",new List<string>{ SystemConfig.SecretaryName});
        //    myMessage.AddSubstitution("-NAME-", new List<string> { member.MemberName });
        //    myMessage.AddSubstitution("-AMOUNT-", new List<string> { amountString });

        //    await Send(myMessage);
        //}

        public async Task SendMembershipEmail(Domain.Membership member)
        {
            var myMessage = new SendGrid.SendGridMessage();
            myMessage.AddTo("slbsmembershipstatus@gmail.com"); //member.Email
            myMessage.From = new MailAddress("slbsmembershipstatus@uchithar.net", "SLSBS Treasurer");
            myMessage.Subject = string.Format("Dhamma School - Membership Payment Status for ({0})", member.MembershipNumber);
            myMessage.Text = "Much Merits to You and Your Family members.";

            myMessage.EnableTemplateEngine(MembershipTemplateId);
            var amountString = string.Format("${0}", member.PaidUpTo.ToString());

            myMessage.AddSubstitution("-SECRETARY-", new List<string> { SystemConfig.SecretaryName });
            myMessage.AddSubstitution("-NAME-", new List<string> { member.ContactName });
            myMessage.AddSubstitution("-AMOUNT-", new List<string> { amountString });

            await Send(myMessage);
        }


        public async Task SendBuildingFundEmail(Domain.Member member)
        {
            var myMessage = new SendGrid.SendGridMessage();
            myMessage.AddTo("uchitha.r@gmail.com");
            myMessage.From = new MailAddress("slbsmembershipstatus@uchithar.net", "SLSBS Admin");
            myMessage.Subject = string.Format("THANK YOU - Acknowledgement of Payments for Building Fund ({0})", member.MemberNo);
            myMessage.Text = "Much Merits to You and Your Family members.";

            myMessage.EnableTemplateEngine(BuildingTemplateId);
            var amountString = string.Format("${0}", member.PaidUpTo);

            myMessage.AddSubstitution("-SECRETARY-", new List<string> { SystemConfig.SecretaryName });
            myMessage.AddSubstitution("-NAME-", new List<string> { member.FamilyName });
            myMessage.AddSubstitution("-AMOUNT-", new List<string> { amountString });

            await Send(myMessage);
        }



        private async Task Send(SendGrid.SendGridMessage message)
        {
            try
            {
                var apiKey = ConfigurationManager.AppSettings["SendgridKey"];
                var transportWeb = new SendGrid.Web(apiKey);
                await transportWeb.DeliverAsync(message);
            }
            catch (InvalidApiRequestException apiEx)
            {
                var details = new StringBuilder();

                details.Append("ResponseStatusCode: " + apiEx.ResponseStatusCode + ".   ");
                for (int i = 1; i <= apiEx.Errors.Count(); i++)
                {
                    details.Append(" -- Error #" + i + " : " + apiEx.Errors[i]);
                }

                throw new ApplicationException(details.ToString(), apiEx);
            }
            
        }

        private bool IsValidEmail(string email)
        {
            if (string.IsNullOrEmpty(email)) throw new ApplicationException("Invalid email");
            return email.Contains("@") ;
        }
    }
}