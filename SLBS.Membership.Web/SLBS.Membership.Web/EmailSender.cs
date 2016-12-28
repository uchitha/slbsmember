using System.Configuration;
using System.Text;
using Exceptions;
using SLBS.Membership.Web.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using NLog;
using SendGrid;

namespace SLBS.Membership.Web
{
    public class EmailSender
    {
        private const string MembershipTemplateId = "cbff537b-6b71-4d32-af3d-45363c5540a9";
        private const string MembershipPayStatusTemplateId = "0eccb659-e657-439b-a8e5-eb5f8cafceca";

        private const string BuildingTemplateId = "fce5f540-5873-438f-bd4b-f52cced63abf";
        private List<Member> _memeberList;
        private EnumMode _mode;

        private readonly bool IsProduction = false;

        private Logger log = LogManager.GetCurrentClassLogger();

        public EmailSender(EnumMode mode)
        {
            _mode = mode;
            IsProduction = ConfigurationManager.AppSettings["Environment"] == "Production";
        }

        public async Task SendMail(List<Domain.Membership> members, EnumNoticeTypes noticeType)
        {
            int count = 0;
            foreach (var member in members)
            {
                var validEmails = member.Adults.Where(a => !string.IsNullOrEmpty(a.Email) && IsValidEmail(a.Email)).Select(a => a.Email).ToList();

                log.Debug("Found {0} emails for membership {1}",validEmails.Count(),member.MembershipNumber);

                foreach (var email in validEmails)
                {
                    if (noticeType == EnumNoticeTypes.PaymentStatusDhammaSchool &&  member.PaidUpTo.HasValue)
                    {
                        await SendPayStatusEmail(member, email);
                        count++;
                    }
                }
            }

            log.Info("Sent {0} emails",count);

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
                        //await SendPayStatusEmail(member);
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


        //public async Task SendPayStatusEmail(Member member)
        //{
        //    var myMessage = new SendGrid.SendGridMessage();
        //    myMessage.AddTo("slbsmembershipstatus@gmail.com"); //member.Email
        //    myMessage.From = new MailAddress("slbsmembershipstatus@uchithar.net", "SLSBS Treasurer");
        //    myMessage.Subject = string.Format("THANK YOU - Acknowledgement of Payments for Membership ({0})", member.Email);
        //    myMessage.Text = "Much Merits to You and Your Family members.";

        //    myMessage.EnableTemplateEngine(MembershipTemplateId);
        //    var amountString = string.Format("${0}", member.Payment);

        //    myMessage.AddSubstitution("-SECRETARY-",new List<string>{ SystemConfig.TreasurerName});
        //    myMessage.AddSubstitution("-NAME-", new List<string> { member.MemberName });
        //    myMessage.AddSubstitution("-AMOUNT-", new List<string> { amountString });

        //    await Send(myMessage);
        //}

        public async Task SendPayStatusEmail(Domain.Membership member, string email)
        {
        

            var myMessage = new SendGridMessage();

            myMessage.From = new MailAddress("slbsmembershipstatus@uchithar.net", "SLSBS Treasurer"); //This needs to be a valid SLSBS email
            myMessage.Subject = string.Format("Dhamma School - Membership Status for {0}", member.MembershipNumber);

            myMessage.EnableTemplateEngine(MembershipPayStatusTemplateId);

            myMessage = HandleIfTest(myMessage, email);
          
            var payStatus = GetPaidUptoMonth(member.PaidUpTo);

            myMessage.AddSubstitution("-TREASURER-", new List<string> { SystemConfig.TreasurerName });
            myMessage.AddSubstitution("-NAME-", new List<string> { member.ContactName });
            myMessage.AddSubstitution("-PAYSTATUS-", new List<string> { payStatus });
            myMessage.AddSubstitution("-MEMBERNO-", new List<string> { member.MembershipNumber });

            myMessage.Html = "<i>Theruwan Saranai, SLSBS</i>";
            myMessage.Text = "Theruwan Saranai, SLSBS";

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
            var amountString = string.Format("${0}", GetPaidUptoMonth(member.PaidUpTo));

            myMessage.AddSubstitution("-SECRETARY-", new List<string> { SystemConfig.TreasurerName });
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

                int i = 1;
                foreach (var error in apiEx.Errors)
                {
                    details.Append(" -- Error #" + i + " : " + error);
                    i++;
                }

                log.Error("Error sending emails : {0}", details);

            }
            catch (Exception ex)
            {
                log.Error(ex,"Error sending email to {0}", message.To[0]);
            }
        }

        private bool IsValidEmail(string email)
        {
            if (string.IsNullOrEmpty(email)) return false;
            return email.Contains("@") ;
        }

        private string GetPaidUptoMonth(DateTime? paidUpTo)
        {
            if (paidUpTo.HasValue)
            {
                return paidUpTo.Value.ToString("Y");
            }
            else
            {
                return "-";
            }
        }

        private SendGridMessage HandleIfTest(SendGridMessage myMessage,string email)
        {
            if (IsProduction)
            {
                myMessage.AddTo(email); //member email
                myMessage.AddSubstitution("-FOOTER-", new List<string> { string.Empty });
            }
            else
            {
                var to = ConfigurationManager.AppSettings["TestEmailReceipients"].Split(',');
                myMessage.AddTo(to);

                //Add real to address as test message
                var warnMessage = string.Format("### This is a test mail intended to be sent to {0} ###", email);
                myMessage.AddSubstitution("-FOOTER-", new List<string> { warnMessage });
            }
            return myMessage;
        }
    }
}