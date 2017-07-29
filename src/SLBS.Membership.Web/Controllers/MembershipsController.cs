using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web.Mvc;
using SLBS.Membership.Domain;
using NLog;

namespace SLBS.Membership.Web.Controllers
{
    [Authorize]
    public class MembershipsController : Controller
    {
        private SlsbsContext db = new SlsbsContext();

        private Logger log = LogManager.GetCurrentClassLogger();

        // GET: Memberships
        [SimpleAuthorize(Roles = "BSEditor,Viewer")]
        public async Task<ActionResult> Index()
        {
            log.Debug("Loading members");
            return View(await db.Memberships.ToListAsync());
        }

        // GET: Memberships/Details/5
        [SimpleAuthorize(Roles = "BSEditor,Viewer")]
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var membership = await db.Memberships.FindAsync(id);
            if (membership == null)
            {
                return HttpNotFound();
            }
            return View(membership);
        }

        // GET: Memberships/Create
        [SimpleAuthorize(Roles = "BSEditor")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Memberships/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [SimpleAuthorize(Roles = "BSEditor")]
        public async Task<ActionResult> Create([Bind(Include = "MembershipNumber,ContactName,PaidUpTo,ApplicationDate,BlockEmails")] Domain.Membership membership)
        {
            var memberKey = membership.MembershipNumber;
            if (!Char.IsLetter(memberKey, 0))
            {
                ModelState.AddModelError("MembershipNumber", "Please provide a letter for membership key");
            }

            if (ModelState.IsValid)
            {
                membership.MembershipNumber = GenerateMembershipNumber(memberKey.ToUpper());
                db.Memberships.Add(membership);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(membership);
        }

        private string GenerateMembershipNumber(string membershipKey)
        {
            var memberNumbers = db.Memberships.Where(m => m.MembershipNumber.StartsWith(membershipKey)).Select(m => m.MembershipNumber).ToList();
            
            if (!memberNumbers.Any()) return string.Format("{0}0001", membershipKey);

            var memberNumberOrdered = memberNumbers.Select(m => int.Parse(m.Remove(0, 1))).OrderByDescending(m => m).ToList();

            var latestMember = memberNumberOrdered.First();

            return string.Format("{0}{1}", membershipKey, (latestMember + 1).ToString("D4"));

        }

        // GET: Memberships/Edit/5
        [SimpleAuthorize(Roles = "BSEditor")]
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var membership = await db.Memberships.FindAsync(id);
            //var comments = await db.Memberships
            if (membership == null)
            {
                return HttpNotFound();
            }
            return View(membership);
        }

        // POST: Memberships/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [SimpleAuthorize(Roles = "BSEditor")]
        public async Task<ActionResult> Edit([Bind(Include = "MembershipId,MembershipNumber,ContactName,PaidUpTo,ApplicationDate,BlockEmails")] Domain.Membership membership, string comment)
        {
            if (ModelState.IsValid)
            {
                var membershipComment = new MembershipComment();
                membershipComment.Comment = comment;
                membershipComment.CommentedOn = DateTime.Now;
                membershipComment.StatusUpdatedOn = DateTime.Now;
                membershipComment.MembershipId = membership.MembershipId;

                membership.MembershipComments.Add(membershipComment);

                db.Entry(membership).State = EntityState.Modified;
                db.Entry(membershipComment).State = EntityState.Added;

                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(membership);
        }

        // GET: Memberships/Delete/5
        [SimpleAuthorize(Roles = "Admin,BSEditor")]
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var membership = await db.Memberships.FindAsync(id);
            if (membership == null)
            {
                return HttpNotFound();
            }
            return View(membership);
        }

        // POST: Memberships/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [SimpleAuthorize(Roles = "BSEditor")]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            var membership = await db.Memberships.FindAsync(id);
            db.Memberships.Remove(membership);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        [HttpPost, ActionName("SaveSendList")]
        [ValidateAntiForgeryToken]
        [SimpleAuthorize(Roles = "Sender")]
        public async Task<ActionResult> SaveSendList(List<int> ids)
        {
            Session["SelectedMemberIds"] = null; //clear
            Session["SelectedMemberIds"] = ids;
            return Json("OK", JsonRequestBehavior.AllowGet);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
