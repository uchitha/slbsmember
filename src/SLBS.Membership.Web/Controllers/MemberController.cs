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
using NLog;

namespace SLBS.Membership.Web.Controllers
{
    public class MemberController : Controller
    {
        private SlsbsContext db = new SlsbsContext();

        private Logger log = LogManager.GetCurrentClassLogger();

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Update()
        {
            if (Request.Files.Count > 0)
            {
                var file = Request.Files[0];
                if (file != null && file.ContentLength > 0)
                {
                    SafeUpdateMembers(file);
                }
            }

            return View("Mode");
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
                    if (file.FileName == "Members.xlsx")
                    {
                        ProcessMembershipFile(file);
                    }
                    if (file.FileName == "PayStatus.xlsx")
                    {
                        UpdatePayStatus(file);
                    }
                    if (file.FileName == "Children.xlsx")
                    {
                        ProcessChildrenFile(file);
                    }
                    if (file.FileName == "ChildrenWithoutMember.xlsx")
                    {
                        ProcessChildrenWithoutMemberFile(file);
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

        private void ProcessChildrenFile(HttpPostedFileBase file)
        {
            using (var package = new ExcelPackage(file.InputStream))
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets["Children"];
                for (int row = 4; worksheet.Cells[row, 1].Value != null; row++)
                {
                    var colIndex = 2; //starting column
                    var memberNo = worksheet.Cells[row, colIndex].Value.ToString();

                    if (string.IsNullOrEmpty(memberNo))
                    {
                        continue;
                    }

                    colIndex++;
                    var childName = worksheet.Cells[row, colIndex++].Value.ToString();
                    int? level = null;
                    if (worksheet.Cells[row, colIndex].Value != null)
                    {
                        level = worksheet.Cells[row, colIndex].Value.ToString() == "senior" ? 10 : int.Parse(worksheet.Cells[row, colIndex].Value.ToString());
                    }
                    colIndex++;
                    var ambulanceCover = worksheet.Cells[row, colIndex].Value != null && (worksheet.Cells[row, colIndex].Value.ToString() == "YES");
                    colIndex++;
                    var mediaConsent = worksheet.Cells[row, colIndex].Value != null && (worksheet.Cells[row, colIndex].Value.ToString() == "YES");


                    var existingMember = db.Memberships.SingleOrDefault(i => i.MembershipNumber == memberNo);

                    if (existingMember != null) //member found link kids to this membership
                    {
                        var c = new Domain.Child();
                        c.MembershipId = existingMember.MembershipId;
                        c.IsActive = true;
                        c.FullName = childName;
                        c.MediaConsent = mediaConsent;
                        c.AmbulanceCover = ambulanceCover;
                        if (level.HasValue) c.ClassLevel = (ClassLevelEnum)level.Value;
                        db.Children.Add(c);
                        db.SaveChanges();
                    }

                }

            } 
        }

        private void ProcessChildrenWithoutMemberFile(HttpPostedFileBase file)
        {
            using (var package = new ExcelPackage(file.InputStream))
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets["Children"];
                for (int row = 4; worksheet.Cells[row, 1].Value != null; row++)
                {
                    var colIndex = 2; //starting column
                    var memberNo = worksheet.Cells[row, colIndex].Value.ToString();

                    if (string.IsNullOrEmpty(memberNo))
                    {
                        continue;
                    }

                    var existingMember = db.Memberships.SingleOrDefault(i => i.MembershipNumber == memberNo);

                    if (existingMember == null)
                    {
                        //no member found //add dummy member
                        var m = new Domain.Membership();
                        m.MembershipNumber = memberNo;
                        m.ContactName = "Membership added temporarily";
                        db.Memberships.Add(m);
                    }

                    db.SaveChanges();

                }

            }
        }


        /// <summary>
        /// Add new members to the system
        /// </summary>
        /// <param name="file"></param>
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

                    var existingMember = db.Memberships.SingleOrDefault(i => i.MembershipNumber == memberNo);
                    if (existingMember != null)
                    {
                        continue;
                    }


                    var contactName = worksheet.Cells[row, 2].Value == null ? string.Empty : worksheet.Cells[row, 2].Value.ToString();

                    var statusString = worksheet.Cells[row, 3].Value == null ? string.Empty : worksheet.Cells[row, 3].Value.ToString();
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

                    db.Memberships.Add(m);
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

        /// <summary>
        /// Update the member information. Only update empty information. Never overwrite existing info
        /// </summary>
        /// <param name="file"></param>
        private void SafeUpdateMembers(HttpPostedFileBase file)
        {
            using (var package = new ExcelPackage(file.InputStream))
            {
                ExcelWorksheet mWorksheet = package.Workbook.Worksheets[SystemConfig.MembershipSheetName];
                for (int row = 2; mWorksheet.Cells[row, 1].Value != null; row++)
                {
                    var memberNo = mWorksheet.Cells[row, 1].Value.ToString();
                    if (string.IsNullOrEmpty(memberNo))
                    {
                        continue;
                    }
                    var existingMember = db.Memberships.SingleOrDefault(i => i.MembershipNumber == memberNo);
                    if (existingMember == null)
                    {
                        continue;
                    }

                    var existingFather = existingMember.Adults.FirstOrDefault(m => m.Role == MembershipRole.Father);
                    if (existingFather == null) continue;

                    if (string.IsNullOrEmpty(existingFather.FullName))
                    {
                        existingFather.FullName = mWorksheet.Cells[row, 3].Value == null ? null : mWorksheet.Cells[row, 3].Value.ToString();
                    }
                    if (string.IsNullOrEmpty(existingFather.Address))
                    {
                        existingFather.Address = mWorksheet.Cells[row, 4].Value == null ? null : mWorksheet.Cells[row, 4].Value.ToString();
                    }
                    if (string.IsNullOrEmpty(existingFather.MobilePhone))
                    {
                        existingFather.MobilePhone = mWorksheet.Cells[row, 5].Value == null ? null : mWorksheet.Cells[row, 5].Value.ToString();
                    }
                    if (string.IsNullOrEmpty(existingFather.LandPhone))
                    {
                        existingFather.LandPhone = mWorksheet.Cells[row, 6].Value == null ? null : mWorksheet.Cells[row, 6].Value.ToString();
                    }
                    if (string.IsNullOrEmpty(existingFather.Email))
                    {
                        existingFather.Email = mWorksheet.Cells[row, 7].Value == null ? null : mWorksheet.Cells[row, 7].Value.ToString();
                    }

                    var existingMother = existingMember.Adults.FirstOrDefault(m => m.Role == MembershipRole.Mother);
                    if (existingMother == null) continue;

                    if (string.IsNullOrEmpty(existingMother.FullName))
                    {
                        existingMother.FullName = mWorksheet.Cells[row, 8].Value == null ? null : mWorksheet.Cells[row, 8].Value.ToString();
                    }
                    if (string.IsNullOrEmpty(existingFather.Address))
                    {
                        existingMother.Address = mWorksheet.Cells[row, 9].Value == null ? null : mWorksheet.Cells[row, 9].Value.ToString();
                    }
                    if (string.IsNullOrEmpty(existingFather.MobilePhone))
                    {
                        existingMother.MobilePhone = mWorksheet.Cells[row, 10].Value== null ? null : mWorksheet.Cells[row, 10].Value.ToString();
                    }
                    if (string.IsNullOrEmpty(existingFather.LandPhone))
                    {
                        existingMother.LandPhone = mWorksheet.Cells[row, 11].Value == null ? null : mWorksheet.Cells[row, 11].Value.ToString();
                    }
                    if (string.IsNullOrEmpty(existingFather.Email))
                    {
                        existingMother.Email = mWorksheet.Cells[row, 12].Value == null ? null : mWorksheet.Cells[row, 12].Value.ToString();
                    }

                    db.SaveChanges();
                }
            }
        }

        private void UpdatePayStatus(HttpPostedFileBase file)
        {
            using (var package = new ExcelPackage(file.InputStream))
            {
                ExcelWorksheet mWorksheet = package.Workbook.Worksheets[SystemConfig.PayStatusSheetName];
                for (int row = 2; mWorksheet.Cells[row, 1].Value != null; row++)
                {
                    var memberNo = mWorksheet.Cells[row, 1].Value.ToString();
                    if (string.IsNullOrEmpty(memberNo))
                    {
                        continue;
                    }
                    var existingMember = db.Memberships.SingleOrDefault(i => i.MembershipNumber == memberNo);
                    if (existingMember == null)
                    {
                        continue;
                    }
                    
                    var payStatus = mWorksheet.Cells[row, 3].Value == null ? null : mWorksheet.Cells[row, 3].Value.ToString();

                    log.Debug("Updating payment status for {0}", memberNo);

                    var paidUpTo = DateTime.MinValue;

                    if ( DateTime.TryParse(payStatus, out paidUpTo) && paidUpTo != DateTime.MinValue ) 
                    {
                        existingMember.PaidUpTo = paidUpTo;
                        db.SaveChanges();
                        log.Debug("Updated payment status for {0} to {1}", memberNo, paidUpTo.ToShortDateString());
                    }
                  
                }
            }
        }
    }
}