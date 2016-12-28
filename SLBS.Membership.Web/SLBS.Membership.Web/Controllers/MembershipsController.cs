using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web.Mvc;
using SLBS.Membership.Domain;

namespace SLBS.Membership.Web.Controllers
{
    public class MembershipsController : Controller
    {
        private SlsbsContext db = new SlsbsContext();

        // GET: Memberships
        public async Task<ActionResult> Index()
        {
            return View(await db.Memberships.ToListAsync());
        }

        // GET: Memberships/Details/5
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
        public ActionResult Create()
        {
            return View();
        }

        // POST: Memberships/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "ContactName,PaidUpTo")] Domain.Membership membership)
        {
            if (ModelState.IsValid)
            {
                membership.MembershipNumber = GenerateMembershipNumber(membership.ContactName[0].ToString().ToUpper());

                db.Memberships.Add(membership);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(membership);
        }

        private string GenerateMembershipNumber(string membershipKey)
        {
            var members = db.Memberships.Where(m => m.MembershipNumber.StartsWith(membershipKey)).ToList();

            if (!members.Any()) return string.Format("{0}0001", membershipKey);

            var latestMember = members.OrderByDescending(m => m.MembershipNumber).First();

            var id =  int.Parse(latestMember.MembershipNumber.Substring(1));

            return string.Format("{0}{1}", membershipKey, (id + 1).ToString("D4"));

        }

        // GET: Memberships/Edit/5
        public async Task<ActionResult> Edit(int? id)
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

        // POST: Memberships/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "MembershipId,MembershipNumber,ContactName,PaidUpTo")] Domain.Membership membership)
        {
            if (ModelState.IsValid)
            {
                db.Entry(membership).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(membership);
        }

        // GET: Memberships/Delete/5
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
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            var membership = await db.Memberships.FindAsync(id);
            db.Memberships.Remove(membership);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        [HttpPost, ActionName("SaveSendList")]
        [ValidateAntiForgeryToken]
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
