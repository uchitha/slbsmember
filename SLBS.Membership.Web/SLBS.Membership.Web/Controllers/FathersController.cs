using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SLBS.Membership.Domain;

namespace SLBS.Membership.Web.Controllers
{
    public class FathersController : Controller
    {
        private SlsbsContext db = new SlsbsContext();

        // GET: Fathers
        public async Task<ActionResult> Index()
        {
            var fathers = db.Fathers.Include(f => f.Member);
            return View(await fathers.ToListAsync());
        }

        // GET: Fathers/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Father father = await db.Fathers.FindAsync(id);
            if (father == null)
            {
                return HttpNotFound();
            }
            return View(father);
        }

        // GET: Fathers/Create
        public ActionResult Create()
        {
            ViewBag.Id = new SelectList(db.Members, "Id", "MemberNo");
            return View();
        }

        // POST: Fathers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,MemberId,Name,Phone,Email")] Father father)
        {
            if (ModelState.IsValid)
            {
                db.Fathers.Add(father);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.Id = new SelectList(db.Members, "Id", "MemberNo", father.Id);
            return View(father);
        }

        // GET: Fathers/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Father father = await db.Fathers.FindAsync(id);
            if (father == null)
            {
                return HttpNotFound();
            }
            ViewBag.Id = new SelectList(db.Members, "Id", "MemberNo", father.Id);
            return View(father);
        }

        // POST: Fathers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,MemberId,Name,Phone,Email")] Father father)
        {
            if (ModelState.IsValid)
            {
                db.Entry(father).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.Id = new SelectList(db.Members, "Id", "MemberNo", father.Id);
            return View(father);
        }

        // GET: Fathers/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Father father = await db.Fathers.FindAsync(id);
            if (father == null)
            {
                return HttpNotFound();
            }
            return View(father);
        }

        // POST: Fathers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Father father = await db.Fathers.FindAsync(id);
            db.Fathers.Remove(father);
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
