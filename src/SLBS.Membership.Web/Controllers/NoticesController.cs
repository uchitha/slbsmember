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

            log.Debug("About to send mail request to the queue");
            var sentCount = await sender.QueueMail(members, EnumNoticeTypes.PaymentStatus);
            log.Debug("Send mail request to the queue completed");
            return Json(new {sentCount}, JsonRequestBehavior.AllowGet);
        }

        private SelectList BuildEnumList()
        {
            var list = EnumHelper.GetSelectList(typeof (EnumNoticeTypes), EnumNoticeTypes.PaymentStatus);
            return new SelectList(list,0);
        } 
    }
}