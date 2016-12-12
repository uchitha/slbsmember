using OfficeOpenXml;
using SLBS.Membership.Web.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace SLBS.Membership.Web.Controllers
{
    public class MemberController : Controller
    {
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
                ExcelWorksheet worksheet = package.Workbook.Worksheets[SystemConfig.SheetName];
                var listMembership = new List<Member>();
                var listBuildingFund = new List<Member>();
                for (int row = 6; worksheet.Cells[row, SystemConfig.PayColumnIndex].Value != null; row++)
                {
                    var email = worksheet.Cells[row, SystemConfig.EmailColumnIndex].Value != null ? worksheet.Cells[row, SystemConfig.EmailColumnIndex].Value.ToString() : string.Empty;
                    var memberNo = worksheet.Cells[row, SystemConfig.MemberNumberColumnIndex].Value.ToString();
                    var memberName = worksheet.Cells[row, SystemConfig.MemberNameColumnIndex].Value.ToString();
                    var paymentStatus = worksheet.Cells[row, SystemConfig.PayColumnIndex].Value.ToString();
                    var membershipPay = worksheet.Cells[row, SystemConfig.MembershipPayStatusColumnIndex].Value;
                    var buildingPay = worksheet.Cells[row, SystemConfig.BuildingFundPayStatusColumnIndex].Value;

                    var m = new Member
                    {
                        MemberNo = memberNo,
                        Email = email,
                        MemberName = memberName,
                    };

                    if (membershipPay != null &&  !string.IsNullOrEmpty(membershipPay.ToString()))
                    {
                        decimal value = 0;
                        if (decimal.TryParse(membershipPay.ToString(), out value))
                        {
                            if (value != 0)
                            {
                                m.Payment = value;
                                listMembership.Add(m);
                            }
                        }
                    }

                    if (buildingPay != null && !string.IsNullOrEmpty(buildingPay.ToString()))
                    {
                        decimal value = 0;
                        if (decimal.TryParse(buildingPay.ToString(), out value))
                        {
                            if (value != 0)
                            {
                                m.Payment = value;
                                listBuildingFund.Add(m);
                            }
                        }
                    }
                   
                }
                Session.Add("MembershipList",listMembership);
                Session.Add("BuildingFundList", listBuildingFund);
            } 
        }
    }
}