using System;
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
using SLBS.Membership.Web.Models;

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
                    ws.Cells[row, 2].Value = o.ContactName;
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

        public async Task<ActionResult> BsDetailsReport()
        {
            var fileDownloadName = "MembershipDetails.xlsx";
            var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

            using (var package = new ExcelPackage())
            {
                ExcelWorksheet wsBs = package.Workbook.Worksheets.Add("Membership Details");

                var list = await ReportRepository.GetMembershipDatails();

                AddMemberDetails(list, wsBs);

                var fileStream = new MemoryStream();
                package.SaveAs(fileStream);
                fileStream.Position = 0;

                var fsr = new FileStreamResult(fileStream, contentType);
                fsr.FileDownloadName = fileDownloadName;

                return fsr;
            }
        }

        public async Task<ActionResult> BsDsDetailsReport()
        {
            var fileDownloadName = "MembershipDetails.xlsx";
            var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

            using (var package = new ExcelPackage())
            {
                ExcelWorksheet wsBs = package.Workbook.Worksheets.Add("Members without DS Kids");
                ExcelWorksheet wsDs = package.Workbook.Worksheets.Add("Members with DS Kids");

                var list = await ReportRepository.GetMembershipDatails();
                var bsMembers = list.Where(m => !m.HasDsKids);
                var dsMembers = list.Where(m => m.HasDsKids);
                AddMemberDetails(bsMembers, wsBs);
                AddMemberDetails(dsMembers, wsDs);

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

          private void AddMemberDetails(IEnumerable<MembershipDetailsViewModel> list, ExcelWorksheet ws)
          {

              int row = 1;
              ws.Row(row).Style.Font.Bold = true;
              ws.Cells[row, 1].Value = "Membership Number";
              ws.Cells[row, 2].Value = "Membership Details";
              ws.Cells[row, 3].Value = "Paid Upto";

              ws.Cells[row, 4].Value = "Fathers Name";
              ws.Cells[row, 5].Value = "Fathers Mobile";
              ws.Cells[row, 6].Value = "Fathers Landphone";
              ws.Cells[row, 7].Value = "Fathers Email";

              ws.Cells[row, 8].Value = "Mothers Name";
              ws.Cells[row, 9].Value = "Mothers Mobile";
              ws.Cells[row, 10].Value = "Mothers Landphone";
              ws.Cells[row, 11].Value = "Mothers Email";

              row++;

              foreach (var o in list)
              {
                  ws.Cells[row, 1].Value = o.MembershipNumber;
                  ws.Cells[row, 2].Value = o.ContactName;
                  ws.Cells[row, 3].Value = o.PaidUpto;

                  ws.Cells[row, 4].Value = o.FathersName;
                  ws.Cells[row, 5].Value = o.FathersMobile;
                  ws.Cells[row, 6].Value = o.FathersLandphone;
                  ws.Cells[row, 7].Value = o.FathersEmail;

                  ws.Cells[row, 8].Value = o.MothersName;
                  ws.Cells[row, 9].Value = o.MothersMobile;
                  ws.Cells[row, 10].Value = o.MothersLandphone;
                  ws.Cells[row, 11].Value = o.MothersEmail;

                  row++;
              }

          }
    }
}