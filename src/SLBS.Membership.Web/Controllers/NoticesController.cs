using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using SLBS.Membership.Domain;
using SLBS.Membership.Web.Models;
using System.Configuration;
using NLog;
using System.Net;
using System.Text;

namespace SLBS.Membership.Web.Controllers
{
    [SimpleAuthorize(Roles = "Sender")]
    public class NoticesController : Controller
    {
        private Logger log = LogManager.GetCurrentClassLogger();

        private SlsbsContext db = new SlsbsContext();

        public async Task<ActionResult> Index()
        {
            if (Session["SelectedMemberIds"] != null)
            {
                var ids = (List<int>)Session["SelectedMemberIds"];

                var members = await db.Memberships.Where(m => ids.Contains(m.MembershipId)).ToListAsync();
                ////Do filtering for pilot
                //var pilotMembers = ConfigurationManager.AppSettings["PilotEmailsMemberList"].Split(',');
                //var pilotMemberIdList = await db.Memberships.Where(m => pilotMembers.Contains(m.MembershipNumber)).Select(m => m.MembershipId).ToListAsync();

                //var list = members.Where(m => ids.Contains(m.MembershipId)).ToList();

                Session["SelectedMemberIds"] = members.Select(m => m.MembershipId).ToList();

                var model = new NoticeViewModel
                {
                    Receipients = members,
                    NoticeType = EnumNoticeTypes.PaymentStatus //Default
                };

                return View(model);
            }
            return View(new NoticeViewModel());
        }

        public async Task<ActionResult> Send()
        {
            //Send emails
            var sender = new QueueSender();
            var ids = (List<int>)Session["SelectedMemberIds"];

            var members = await db.Memberships.Where(m => ids.Contains(m.MembershipId)).ToListAsync();
            try
            {
                log.Debug("About to send mail request to the queue");
                var sentCount = await sender.QueueMail(members, EnumNoticeTypes.PaymentStatus);
                log.Debug("Send mail request to the queue completed");

                log.Debug("Triggering the web job to start sending emails");
                await StartMailSendingWebJob();
                log.Debug("Triggering the web job completed");

                return Json(new { sentCount }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                log.Error(e.Message);
                log.Error(e.StackTrace);
                throw;
            }
          
        }

        private async Task StartMailSendingWebJob()
        {
            var url = string.Format("https://{0}.scm.azurewebsites.net/api/triggeredwebjobs/{1}/run", 
                ConfigurationManager.AppSettings["AppServiceName"], 
                ConfigurationManager.AppSettings["WebJobName"]);
            log.Debug(string.Format("Triggering web job at {0}", url));
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
            request.Method = "POST";
            var creds = ConfigurationManager.AppSettings["EmailJobCredentials"];
            var byteArray = Encoding.ASCII.GetBytes(creds); //we could find user name and password in Azure web app publish profile 
            request.Headers.Add("Authorization", "Basic " + Convert.ToBase64String(byteArray));
            request.ContentLength = 0;
            try
            {
                await request.GetResponseAsync();
            }
            catch (Exception e)
            {
                log.Error(e, "Triggering web job failed");
                throw e;
            }
        }

        private SelectList BuildEnumList()
        {
            var list = EnumHelper.GetSelectList(typeof (EnumNoticeTypes), EnumNoticeTypes.PaymentStatus);
            return new SelectList(list,0);
        } 
    }
}