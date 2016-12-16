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
    public class NoticesController : Controller
    {

        private SlsbsContext db = new SlsbsContext();

        public async Task<ActionResult> Index()
        {
            if (TempData["SelectedMemberIds"] != null)
            {
                var ids = (List<int>)TempData["SelectedMemberIds"];

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
            await Task.Delay(500);
            return Json("OK", JsonRequestBehavior.AllowGet);
        }

        private SelectList BuildEnumList()
        {
            var list = EnumHelper.GetSelectList(typeof (EnumNoticeTypes), EnumNoticeTypes.PaymentStatusDhammaSchool);
            return new SelectList(list,0);
        } 
    }
}