﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using SLBS.Membership.Domain;
using SLBS.Membership.Web.Models;
using System.Configuration;

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

        public async Task<ActionResult> Send(EnumNoticeTypes noticeType)
        {
            //Send emails
            var sender = new EmailSender(EnumMode.Membership);
            var ids = (List<int>)Session["SelectedMemberIds"];

            var members = await db.Memberships.Where(m => ids.Contains(m.MembershipId)).ToListAsync();

            var sentCount = await sender.SendMail(members, noticeType);

            return Json(new {sentCount}, JsonRequestBehavior.AllowGet);
        }

        private SelectList BuildEnumList()
        {
            var list = EnumHelper.GetSelectList(typeof (EnumNoticeTypes), EnumNoticeTypes.PaymentStatus);
            return new SelectList(list,0);
        } 
    }
}