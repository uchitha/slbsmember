using System.Globalization;
using OfficeOpenXml;
using SLBS.Membership.Domain;
using SLBS.Membership.Web.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Member = SLBS.Membership.Web.Models.Member;

namespace SLBS.Membership.Web.Controllers
{
    public class MemberController : Controller
    {
        private SlsbsContext db = new SlsbsContext();

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Load()
        {
            var list = new List<Member>();

            if (Request.Files.Count > 0)
            {
                var file = Request.Files[0];
              

                if (file != null && file.ContentLength > 0)
                {
                    if (Path.GetExtension(file.FileName) == ".xlsx")
                    {
                        ProcessMembershipFile(file);
                    }
                }
            }

            return View("Mode");
        }

        public ActionResult Show(EnumMode mode)
        {
            var list = new List<Member>();
            if (mode.Equals(EnumMode.Membership))
            {
                list = (List<Member>)Session["MembershipList"];
               
            }
            else if (mode.Equals(EnumMode.BuildingFund))
            {
                list = (List<Member>)Session["BuildingFundList"];
            }
            ViewBag.Mode = mode;
            return View("Show", list);
        }

        [HttpPost]
        public async Task<ActionResult> Send(List<string> selectedMemberNumbers,EnumMode mode)
        {
            var memberSendList = new List<Member>();
            var membershipList = new List<Member>();
            var sender = new EmailSender(EnumMode.None);
            int sendCount = 0;
            if (mode.Equals(EnumMode.Membership))
            {
                membershipList = (List<Member>) Session["MembershipList"];
                sender = new EmailSender(EnumMode.Membership);
            }
            else if (mode.Equals(EnumMode.BuildingFund))
            {
                membershipList = (List<Member>) Session["BuildingFundList"];
                sender = new EmailSender(EnumMode.BuildingFund);
            }
         
            if (membershipList.Count > 0)
            {
                foreach (var memberNo in selectedMemberNumbers) 
                {
                    var member = membershipList.FirstOrDefault(m => m.MemberNo == memberNo);
                    if (member != null)
                    {
                        memberSendList.Add(member);
                    }
                }
            }

            //ViewBag.SentCount = await sender.SendAll(memberSendList);
            return View("SendReport");

        }

        private void ProcessMembershipFile(HttpPostedFileBase file)
        {
            using (var package = new ExcelPackage(file.InputStream))
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets[SystemConfig.MembershipSheetName];
                var listMembership = new List<Member>();
                var listFathers = new List<Adult>();
                for (int row = 5; worksheet.Cells[row, 1].Value != null; row++)
                {
                    var memberNo = worksheet.Cells[row, 1].Value.ToString();

                    if (string.IsNullOrEmpty(memberNo))
                    {
                        continue;
                    }

                    var contactName = worksheet.Cells[row, 2].Value.ToString();
                    var statusString = worksheet.Cells[row, 3].Value.ToString();
                    DateTime? payStatus = null;

                    if (!string.IsNullOrEmpty(statusString))
                    {
                        DateTime payDate;
                        if (DateTime.TryParse(statusString, out payDate))
                        {
                            payStatus = payDate;
                        }
                        //var items = statusString.Split(new[] {'/'});
                    }

                    var m = new Domain.Membership
                    {
                        MembershipNumber = memberNo,
                        ContactName = contactName,
                        PaidUpTo = payStatus
                    };

                    var existingMember = db.Memberships.SingleOrDefault(i => i.MembershipNumber == memberNo);
                    if (existingMember != null)
                    {
                        m.MembershipId = existingMember.MembershipId;
                        existingMember.PaidUpTo = m.PaidUpTo;
                        existingMember.ContactName = m.ContactName;
                    }
                    else
                    {
                        db.Memberships.Add(m);
                    }
                    db.SaveChanges();

                    var fathersName = worksheet.Cells[row, 4].Value == null ? null: worksheet.Cells[row, 4].Value.ToString();
                    if (!string.IsNullOrEmpty(fathersName))
                    {
                        var f = new Adult();
                        f.FullName = fathersName;
                        f.MembershipId = m.MembershipId;
                        f.Address = worksheet.Cells[row, 5].Value == null ? null : worksheet.Cells[row, 5].Value.ToString();
                        f.MobilePhone = worksheet.Cells[row, 6].Value == null ? null : worksheet.Cells[row, 6].Value.ToString();
                        f.LandPhone = worksheet.Cells[row, 7].Value == null ? null : worksheet.Cells[row, 7].Value.ToString();
                        
                        f.Email = worksheet.Cells[row, 8].Value == null ? null : worksheet.Cells[row, 8].Value.ToString();

                        f.Role = MembershipRole.Father;

                        db.Adults.Add(f);
                    }
               

                    var mothersName = worksheet.Cells[row, 9].Value == null ? null : worksheet.Cells[row, 9].Value.ToString();

                    if (!string.IsNullOrEmpty(mothersName))
                    {
                        var mother = new Adult();
                        mother.FullName = mothersName;
                        mother.MembershipId = m.MembershipId;
                        mother.Address = worksheet.Cells[row, 10].Value == null ? null : worksheet.Cells[row, 10].Value.ToString();
                        mother.MobilePhone = worksheet.Cells[row, 11].Value == null ? null : worksheet.Cells[row, 11].Value.ToString();
                        mother.LandPhone = worksheet.Cells[row, 12].Value == null ? null : worksheet.Cells[row, 12].Value.ToString();
                        mother.Email = worksheet.Cells[row, 13].Value == null ? null : worksheet.Cells[row, 13].Value.ToString();
                       
                        mother.Role = MembershipRole.Mother;
                        db.Adults.Add(mother);

                    }

                    db.SaveChanges();
                }
            
            } 
        }
    }
}