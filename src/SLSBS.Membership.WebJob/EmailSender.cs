using System.Configuration;
using System.Text;
using Exceptions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using NLog;
using SendGrid;
using SLBS.Membership.Domain;
using System.Data.Entity;
using SLBS.Membership.Web;

namespace SLBS.Membership.WebJobEmail
{
    public class EmailSender
    {
        private const string MembershipTemplateId = "cbff537b-6b71-4d32-af3d-45363c5540a9";
        private const string MembershipPayStatusTemplateId = "0eccb659-e657-439b-a8e5-eb5f8cafceca";

        private const string BuildingTemplateId = "fce5f540-5873-438f-bd4b-f52cced63abf";
        private EnumMode _mode;

        private readonly bool IsProduction = false;

        private Logger log = LogManager.GetCurrentClassLogger();

        private SlsbsContext db = new SlsbsContext();

        public EmailSender(EnumMode mode)
        {
            _mode = mode;
            IsProduction = true;
        }

        public async Task<bool> SendMail(int membershipId, string email)
        {
            int count = 0;
            var existingMember = db.Memberships.SingleOrDefault(m => m.MembershipId == membershipId);

            if (existingMember == null) return false;

            var mailSentDate = DateTime.Now;

            if (await SendPayStatusEmail(existingMember, email))
            {
                count++;
                var memberDbInstance = db.Memberships.SingleOrDefault(m => m.MembershipNumber == existingMember.MembershipNumber);
                if (memberDbInstance != null)
                {
                    memberDbInstance.LastNotificationDate = mailSentDate;
                    db.SaveChanges();
                }

            }

            log.Info("{0} emails sent in total",count);
            return true;
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

        private async Task<bool> SendPayStatusEmail(Domain.Membership member, string email)
        {
        
            var myMessage = new SendGridMessage();

            var date = DateTime.Now;
            var lastDateOfMonth = DateTime.DaysInMonth(date.Year, date.Month - 1);
            var paymentStatusDate = new DateTime(date.Year, date.Month - 1, lastDateOfMonth);

            myMessage.From = new MailAddress("slsbsmembershipstatus@srilankanvihara.org.au", "SLSBS Treasurer"); //This needs to be a valid SLSBS email
            myMessage.Subject = string.Format("Your SLSBS Membership ({0}) Status as at {1}", member.MembershipNumber, paymentStatusDate.ToString("dd MMM yyyy"));

            myMessage.EnableTemplateEngine(MembershipPayStatusTemplateId);

            myMessage = HandleIfTest(myMessage, email);
          
            var payStatus = GetPaidUptoMonth(member.PaidUpTo);

            myMessage.AddSubstitution("-TREASURER-", new List<string> { EmailConfig.TreasurerName });
            myMessage.AddSubstitution("-NAME-", new List<string> { member.ContactName });
            myMessage.AddSubstitution("-PAYSTATUS-", new List<string> { payStatus });
            myMessage.AddSubstitution("-PAYCALCDATE-", new List<string> { paymentStatusDate.ToString("dd MMM yyyy") });
            myMessage.AddSubstitution("-MEMBERNO-", new List<string> { member.MembershipNumber });

            myMessage.Html = "Theruwan Saranai";
            myMessage.Text = "Theruwan Saranai";

            myMessage.AddSubstitution("-SENDER-", new List<string> { "Treasurer- SLSBS" });

            return await Send(myMessage);
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

            myMessage.AddSubstitution("-SECRETARY-", new List<string> { EmailConfig.TreasurerName });
            myMessage.AddSubstitution("-NAME-", new List<string> { member.FamilyName });
            myMessage.AddSubstitution("-AMOUNT-", new List<string> { amountString });

            await Send(myMessage);
        }

        private async Task<bool> Send(SendGrid.SendGridMessage message)
        {
            try
            {
                var apiKey = ConfigurationManager.AppSettings["SendgridKey"];
                var transportWeb = new SendGrid.Web(apiKey);
                await transportWeb.DeliverAsync(message);
                return true;
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

                log.Error("Error sending emails (InvalidApiRequest from Sendgrid): {0}", details);
                return false;
            }
            catch (Exception ex)
            {
                log.Error(ex,"Error sending email to {0}", message.To[0]);
                return false;
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
                return string.Format("paid up to {0}.",paidUpTo.Value.ToString("Y"));
            }
            else
            {
                return "payments are due.";
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
                myMessage.AddTo(email);

                //Add real to address as test message
                //var warnMessage = string.Format("### This is a test mail intended to be sent to {0} ###", email);
                var warnMessage = "Thank  you for opting in to be in the SLSBS email sending test phase.";
                myMessage.AddSubstitution("-FOOTER-", new List<string> { string.Empty });
            }
            return myMessage;
        }
    }
}