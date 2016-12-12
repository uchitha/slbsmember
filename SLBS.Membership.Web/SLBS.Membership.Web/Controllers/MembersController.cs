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
using SLBS.Membership.Web.Models;
using Member = SLBS.Membership.Domain.Member;

namespace SLBS.Membership.Web.Controllers
{
    public class MembersController : Controller
    {
        private SlsbsContext db = new SlsbsContext();

        // GET: Members
        public async Task<ActionResult> Index()
        {
            var members = db.Members.Include(m => m.Father).Include(m => m.Mother);
            return View(await members.ToListAsync());
        }

        // GET: Members/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Member member = await db.Members.FindAsync(id);
            if (member == null)
            {
                return HttpNotFound();
            }
            return View(member);
        }

        // GET: Members/Create
        public ActionResult Create()
        {
            ViewBag.Id = new SelectList(db.Fathers, "Id", "Name");
            ViewBag.Id = new SelectList(db.Mothers, "Id", "Name");
            return View();
        }

        // POST: Members/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,MotherId,FatherId,MemberNo,FamilyName,PaidUpTo")] Member member)
        {
            if (ModelState.IsValid)
            {
                db.Members.Add(member);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.Id = new SelectList(db.Fathers, "Id", "Name", member.Id);
            ViewBag.Id = new SelectList(db.Mothers, "Id", "Name", member.Id);
            return View(member);
        }

        // GET: Members/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Member member = await db.Members.Include(m => m.Mother).Include(m => m.Father).FirstOrDefaultAsync(m => m.Id == id);

            if (member == null)
            {
                return HttpNotFound();
            }

            ViewBag.Fathers = new SelectList(db.Fathers, "Id", "Name", member.MotherId);
            ViewBag.Mothers = new SelectList(db.Mothers, "Id", "Name", member.FatherId);

            return View(member);
        }

        // POST: Members/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,MotherId,FatherId,MemberNo,FamilyName,PaidUpTo")] Member member)
        {
            if (ModelState.IsValid)
            {
                db.Entry(member).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.Id = new SelectList(db.Fathers, "Id", "Name", member.Id);
            ViewBag.Id = new SelectList(db.Mothers, "Id", "Name", member.Id);
            return View(member);
        }

        // GET: Members/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Member member = await db.Members.FindAsync(id);
            if (member == null)
            {
                return HttpNotFound();
            }
            return View(member);
        }

        // POST: Members/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Member member = await db.Members.FindAsync(id);
            db.Members.Remove(member);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        [HttpPost, ActionName("Send")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Send(List<int> ids)
        {
            //var ids = new List<int>();
            var members = await db.Members.Where(m => ids.Contains(m.Id)).Include(m => m.Father).Include(m => m.Mother).ToListAsync();
            var sender = new EmailSender(EnumMode.Membership);

            ViewBag.SentCount = await sender.SendAll(members);
            var sentCount = ViewBag.SentCount;
            ViewBag.UserMessage = string.Format("Emails sent to {0} members",sentCount);

            return View("Index");
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
