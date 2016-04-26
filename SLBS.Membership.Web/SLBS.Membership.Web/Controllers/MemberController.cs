using OfficeOpenXml;
using SLBS.Membership.Web.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
                        list = ProcessMembershipFile(file);
                    }
                }
            }

            return Show(list);
        }

        public ActionResult Show(List<Member> list)
        {
            return View("Show",list);
        }

        private List<Member> ProcessMembershipFile(HttpPostedFileBase file)
        {
            using (var package = new ExcelPackage(file.InputStream))
            {
                // get the first worksheet in the workbook
                ExcelWorksheet worksheet = package.Workbook.Worksheets["MEM_BF"];
                int col = 12;
                var list = new List<Member>();
                for (int row = 6; worksheet.Cells[row, col].Value != null; row++)
                {
                    var email = worksheet.Cells[row, col - 3].Value != null ? worksheet.Cells[row, col - 3].Value.ToString() : string.Empty;
                    var memberNo = worksheet.Cells[row, col - 2].Value.ToString();
                    var paymentStatus = worksheet.Cells[row, col].Value.ToString();
                    if (paymentStatus == "Pay")
                    {
                        list.Add(new Member
                        {
                            MemberNo = memberNo,
                            Email = email,
                            PaymentStatus = paymentStatus
                        });
                    }
                   
                }
                return list;
            } 

        }
    }
}