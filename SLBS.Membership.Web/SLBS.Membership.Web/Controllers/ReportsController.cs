﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using OfficeOpenXml;
using SLBS.Membership.Domain;
using SLBS.Membership.Web.Data;

namespace SLBS.Membership.Web.Controllers
{
      [Authorize]
    public class ReportsController : Controller
    {
        private SlsbsContext db = new SlsbsContext();

        // GET: Reports
        public ActionResult Index()
        {
            return View();
        }

        [SimpleAuthorize(Roles = "BSEditor")]
        public async Task<ActionResult> PaymentStatusReport()
        {
            var fileDownloadName = "PaymentStatus.xlsx";
            var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

            
            using (var package = new ExcelPackage())
            {
                ExcelWorksheet ws = package.Workbook.Worksheets.Add("PaymentStatus");

                var list = await ReportRepository.GetPayStatusReport();
                int row = 1;
                ws.Row(row).Style.Font.Bold = true;
                ws.Cells[row, 1].Value = "Membership Number";
                ws.Cells[row, 2].Value = "Membership Details";
                ws.Cells[row, 3].Value = "Paid Upto";

                row++;

                foreach (var o in list)
                {
                    ws.Cells[row, 1].Value = o.MembershipNumber;
                    ws.Cells[row, 2].Value = o.MembershipDetails;
                    ws.Cells[row, 3].Value = o.PaidUpto;

                    row++;
                }

                var fileStream = new MemoryStream();
                package.SaveAs(fileStream);
                fileStream.Position = 0;

                var fsr = new FileStreamResult(fileStream, contentType);
                fsr.FileDownloadName = fileDownloadName;

                return fsr;
            }
        }

        public async Task<ActionResult> ChildDetailsReport()
        {
            var fileDownloadName = "DSChildDetailsReport.xlsx";
            var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";


            using (var package = new ExcelPackage())
            {
                ExcelWorksheet ws = package.Workbook.Worksheets.Add("ChldDetails");

                var list = await ReportRepository.GetChildrenDetails();
                int row = 1;
                int col = 1;
                ws.Row(row).Style.Font.Bold = true;
                ws.Cells[row, col++].Value = "Class";
                ws.Cells[row, col++].Value = "Membership Number";
                ws.Cells[row, col++].Value = "Child name";
                ws.Cells[row, col++].Value = "Amblance Cover";
                ws.Cells[row, col++].Value = "Media Consent";
                ws.Cells[row, col++].Value = "Pay Status";
                ws.Cells[row, col++].Value = "Fathers Name";
                ws.Cells[row, col++].Value = "Mothers Name";
                ws.Cells[row, col++].Value = "Fathers Email";
                ws.Cells[row, col++].Value = "Fathers Mobile";
                ws.Cells[row, col++].Value = "Fathers Landphone";
                ws.Cells[row, col++].Value = "Mothers Email";
                ws.Cells[row, col++].Value = "Mothers Mobile";
                ws.Cells[row, col].Value = "Mother Landphone";

                row++;
                foreach (var o in list)
                {
                    col = 1;
                    ws.Cells[row, col++].Value = o.ClassName;
                    ws.Cells[row, col++].Value = o.MembershipNumber;
                    ws.Cells[row, col++].Value = o.ChildName;
                    ws.Cells[row, col++].Value = o.AmbulanceCover;
                    ws.Cells[row, col++].Value = o.MediaConsent;
                    ws.Cells[row, col++].Value = o.PaymentStatus;
                    ws.Cells[row, col++].Value = o.FatherName;
                    ws.Cells[row, col++].Value = o.MotherName;
                    ws.Cells[row, col++].Value = o.FatherEmail;
                    ws.Cells[row, col++].Value = o.FatherMobile;
                    ws.Cells[row, col++].Value = o.FatherLandphone;
                    ws.Cells[row, col++].Value = o.MotherEmail;
                    ws.Cells[row, col++].Value = o.MotherMobile;
                    ws.Cells[row, col].Value = o.MotherLandphone;

                    row++;
                }

                var fileStream = new MemoryStream();
                package.SaveAs(fileStream);
                fileStream.Position = 0;

                var fsr = new FileStreamResult(fileStream, contentType);
                fsr.FileDownloadName = fileDownloadName;

                return fsr;
            }
        }
    }
}