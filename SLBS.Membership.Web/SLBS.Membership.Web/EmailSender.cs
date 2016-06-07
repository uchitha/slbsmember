using System.Text;
using Exceptions;
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
        private const string ApiKey = "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";
        private const string MembershipTemplateId = "cbff537b-6b71-4d32-af3d-45363c5540a9";
        private const string BuildingTemplateId = "fce5f540-5873-438f-bd4b-f52cced63abf";
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
            myMessage.AddTo("slbsmembershipstatus@gmail.com"); //member.Email
            myMessage.From = new MailAddress("slbsmembershipstatus@uchithar.net", "SLSBS Treasurer");
            myMessage.Subject = string.Format("THANK YOU - Acknowledgement of Payments for Membership ({0})", member.Email);
            myMessage.Text = "Much Merits to You and Your Family members.";

            myMessage.EnableTemplateEngine(MembershipTemplateId);
            var amountString = string.Format("${0}", member.Payment);

            myMessage.AddSubstitution("-SECRETARY-",new List<string>{ SystemConfig.SecretaryName});
            myMessage.AddSubstitution("-NAME-", new List<string> { member.MemberName });
            myMessage.AddSubstitution("-AMOUNT-", new List<string> { amountString });

            await Send(myMessage);
        }


        public async Task SendBuildingFundEmail(Member member)
        {
            var myMessage = new SendGrid.SendGridMessage();
            myMessage.AddTo("slbsmembershipstatus@gmail.com");
            myMessage.From = new MailAddress("slbsmembershipstatus@uchithar.net", "SLSBS Admin");
            myMessage.Subject = string.Format("THANK YOU - Acknowledgement of Payments for Building Fund ({0})", member.Email);
            myMessage.Text = "Much Merits to You and Your Family members.";

            myMessage.EnableTemplateEngine(BuildingTemplateId);
            var amountString = string.Format("${0}", member.Payment);

            myMessage.AddSubstitution("-SECRETARY-", new List<string> { SystemConfig.SecretaryName });
            myMessage.AddSubstitution("-NAME-", new List<string> { member.MemberName });
            myMessage.AddSubstitution("-AMOUNT-", new List<string> { amountString });

            await Send(myMessage);
        }



        private async Task Send(SendGrid.SendGridMessage message)
        {
            try
            {
                var transportWeb = new SendGrid.Web(ApiKey);
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
            return email.Contains("@") ;
        }
    }
}