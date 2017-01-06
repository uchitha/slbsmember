using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using SLBS.Membership.Domain;
using SLBS.Membership.Web.Models;

namespace SLBS.Membership.Web.Controllers
{
    [SimpleAuthorize(Roles = "Sender")]
    public class NoticesController : Controller
    {

        private SlsbsContext db = new SlsbsContext();

        public async Task<ActionResult> Index()
        {
            if (Session["SelectedMemberIds"] != null)
            {
                var ids = (List<int>)Session["SelectedMemberIds"];

                var members = await db.Memberships.Where(m => ids.Contains(m.MembershipId)).ToListAsync();

                var model = new NoticeViewModel
                {
                    Receipients = members,
                    NoticeType = EnumNoticeTypes.PaymentStatusDhammaSchool //Default
                };

                return View(model);
            }
            return View(new NoticeViewModel());
        }

        public async Task<ActionResult> Send()
        {
            //Send emails
            var sender = new EmailSender(EnumMode.Membership);
            var ids = (List<int>)Session["SelectedMemberIds"];
            var members = await db.Memberships.Where(m => ids.Contains(m.MembershipId)).ToListAsync();

            var sentCount = await sender.SendMail(members, EnumNoticeTypes.PaymentStatusDhammaSchool);

            return Json(new {sentCount}, JsonRequestBehavior.AllowGet);
        }

        private SelectList BuildEnumList()
        {
            var list = EnumHelper.GetSelectList(typeof (EnumNoticeTypes), EnumNoticeTypes.PaymentStatusDhammaSchool);
            return new SelectList(list,0);
        } 
    }
}