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
using SLBS.Membership.Domain;
using System.Data.Entity;

namespace SLBS.Membership.Web
{
    public class QueueSender
    {
        private readonly bool IsProduction = false;

        private Logger log = LogManager.GetCurrentClassLogger();

        private SlsbsContext db = new SlsbsContext();

        public QueueSender()
        {
            IsProduction = ConfigurationManager.AppSettings["Environment"] == "Production";
        }

        public async Task<int> QueueMail(List<Domain.Membership> members, EnumNoticeTypes noticeType)
        {
            int count = 0;
            List<int> membershipIdsMailSentTo = new List<int>();

            var mailSentDate = DateTime.Now;
            var queueManager = new QueueManager();

            log.Debug(string.Format("Received {0} members",members.Count));
            foreach (var member in members)
            {
                if (member.BlockEmails.HasValue && member.BlockEmails.Value) continue;

                var validEmails = member.Adults.Where(a => !string.IsNullOrEmpty(a.Email) && IsValidEmail(a.Email)).Select(a => a.Email).ToList();

                log.Debug("Found {0} emails for membership {1}",validEmails.Count(),member.MembershipNumber);

                foreach (var email in validEmails)
                {
                    log.Debug("About to insert to queue");
                    queueManager.InsertMail(member.MembershipId,email,noticeType);
                    count++;
                }
            }

            log.Info("{0} emails queued to be sent in total",count);
            return count;
        }

        private bool IsValidEmail(string email)
        {
            if (string.IsNullOrEmpty(email)) return false;
            return email.Contains("@") ;
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