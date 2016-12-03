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
    public class MothersController : Controller
    {
        private SlsbsContext db = new SlsbsContext();

        // GET: Mothers
        public async Task<ActionResult> Index()
        {
            var mothers = db.Mothers.Include(m => m.Member);
            return View(await mothers.ToListAsync());
        }

        // GET: Mothers/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Mother mother = await db.Mothers.FindAsync(id);
            if (mother == null)
            {
                return HttpNotFound();
            }
            return View(mother);
        }

        // GET: Mothers/Create
        public ActionResult Create()
        {
            ViewBag.Id = new SelectList(db.Members, "Id", "MemberNo");
            return View();
        }

        // POST: Mothers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,MemberId,Name,Phone,Email")] Mother mother)
        {
            if (ModelState.IsValid)
            {
                db.Mothers.Add(mother);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.Id = new SelectList(db.Members, "Id", "MemberNo", mother.Id);
            return View(mother);
        }

        // GET: Mothers/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Mother mother = await db.Mothers.FindAsync(id);
            if (mother == null)
            {
                return HttpNotFound();
            }
            ViewBag.Id = new SelectList(db.Members, "Id", "MemberNo", mother.Id);
            return View(mother);
        }

        // POST: Mothers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,MemberId,Name,Phone,Email")] Mother mother)
        {
            if (ModelState.IsValid)
            {
                db.Entry(mother).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.Id = new SelectList(db.Members, "Id", "MemberNo", mother.Id);
            return View(mother);
        }

        // GET: Mothers/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Mother mother = await db.Mothers.FindAsync(id);
            if (mother == null)
            {
                return HttpNotFound();
            }
            return View(mother);
        }

        // POST: Mothers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Mother mother = await db.Mothers.FindAsync(id);
            db.Mothers.Remove(mother);
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
