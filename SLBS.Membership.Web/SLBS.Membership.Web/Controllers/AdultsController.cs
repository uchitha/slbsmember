using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using SLBS.Membership.Domain;

namespace SLBS.Membership.Web.Controllers
{
    [Authorize]
    public class AdultsController : Controller
    {
        private SlsbsContext db = new SlsbsContext();

        // GET: Adults
        public async Task<ActionResult> Index()
        {
            var adults = db.Adults.Include(a => a.Membership);
            return View(await adults.ToListAsync());
        }

        // GET: Adults for membership
        [ActionName("Members")]
        [HttpGet]
        public async Task<ActionResult> Membership(int id)
        {
            var adults = db.Adults.Include(a => a.Membership).Where(a => a.MembershipId == id);
            ViewBag.MembershipId = id;
            return View("Index",await adults.ToListAsync());
        }

        // GET: Adults/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Adult adult = await db.Adults.FindAsync(id);
            if (adult == null)
            {
                return HttpNotFound();
            }
            return View(adult);
        }

        // GET: Adults/Create
        [SimpleAuthorize(Roles = "BSEditor")]
        public ActionResult Create(int id)
        {
            ViewBag.MembershipList = new SelectList(db.Memberships, "MembershipId", "MembershipNumber");
            var adult = new Adult()
            {
                MembershipId = id
            };
            return View(adult);
        }

        // POST: Adults/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [SimpleAuthorize(Roles = "BSEditor")]
        public async Task<ActionResult> Create([Bind(Include = "AdultId,MembershipId,FullName,Address,Phone,Email,Role")] Adult adult)
        {
            if (ModelState.IsValid)
            {
                db.Adults.Add(adult);
                await db.SaveChangesAsync();
                return RedirectToAction("Members",new { id = adult.MembershipId});
            }

            ViewBag.MembershipId = new SelectList(db.Memberships, "MembershipId", "MembershipNumber", adult.MembershipId);
            return View(adult);
        }

        // GET: Adults/Edit/5
        [SimpleAuthorize(Roles = "BSEditor")]
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Adult adult = await db.Adults.FindAsync(id);
            if (adult == null)
            {
                return HttpNotFound();
            }
            ViewBag.MembershipId = adult.MembershipId;
            ViewBag.MembershipList = new SelectList(db.Memberships, "MembershipId", "MembershipNumber", adult.MembershipId);
            return View(adult);
        }

        // POST: Adults/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [SimpleAuthorize(Roles = "BSEditor")]
        public async Task<ActionResult> Edit([Bind(Include = "AdultId,MembershipId,FullName,Address,LandPhone,MobilePhone,Email,Role")] Adult adult)
        {
            if (ModelState.IsValid)
            {
                db.Entry(adult).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Members", new { id = adult.MembershipId });
            }
            ViewBag.MembershipId = new SelectList(db.Memberships, "MembershipId", "MembershipNumber", adult.MembershipId);
            return View(adult);
        }

        // GET: Adults/Delete/5
        [SimpleAuthorize(Roles = "BSEditor")]
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Adult adult = await db.Adults.FindAsync(id);
            if (adult == null)
            {
                return HttpNotFound();
            }
            return View(adult);
        }

        // POST: Adults/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [SimpleAuthorize(Roles = "BSEditor")]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Adult adult = await db.Adults.FindAsync(id);
            db.Adults.Remove(adult);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
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
