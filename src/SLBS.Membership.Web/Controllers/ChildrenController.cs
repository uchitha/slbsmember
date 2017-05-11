using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web.Mvc;
using SLBS.Membership.Domain;

namespace SLBS.Membership.Web.Controllers
{
    [Authorize]
    [Route("Children")]
    public class ChildrenController : Controller
    {
        private SlsbsContext db = new SlsbsContext();

        // GET: Children
        public async Task<ActionResult> Index()
        {
            var children = db.Children.Include(c => c.Membership);
            return View(await children.ToListAsync());
        }
        
        // GET: Children for membership
        [ActionName("Members")]
        public async Task<ActionResult> Membership(int id)
        {
            var children = db.Children.Include(c => c.Membership).Where(i => i.MembershipId == id);
            ViewBag.MembershipId = id;
            return View("Index",await children.ToListAsync());
        }

        // GET: Children/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Child child = await db.Children.Include(c => c.Comments).SingleOrDefaultAsync(c => c.ChildId == id);
            if (child == null)
            {
                return HttpNotFound();
            }
            return View(child);
        }

        // GET: Children/Create
        [SimpleAuthorize(Roles = "DSEditor")]
        public ActionResult Create(int? id)
        {
            ViewBag.MembershipList = new SelectList(db.Memberships, "MembershipId", "MembershipNumber");
            var child = new Child();
            if (id.HasValue)
            {
                child.MembershipId = id.Value;
            }
            return View(child);
        }

        // POST: Children/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [SimpleAuthorize(Roles = "DSEditor")]
        public async Task<ActionResult> Create([Bind(Include = "ChildId,MembershipId,FullName,ClassLevel,MediaConsent,AmbulanceCover")] Child child)
        {
            if (ModelState.IsValid)
            {
                db.Children.Add(child);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.MembershipId = new SelectList(db.Memberships, "MembershipId", "MembershipNumber", child.MembershipId);
            return View(child);
        }

        // GET: Children/Edit/5
         [SimpleAuthorize(Roles = "DSEditor")]
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Child child = await db.Children.FindAsync(id);
            if (child == null)
            {
                return HttpNotFound();
            }
            ViewBag.MembershipId = new SelectList(db.Memberships, "MembershipId", "MembershipNumber", child.MembershipId);
            return View(child);
        }

        // POST: Children/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [SimpleAuthorize(Roles = "DSEditor")]
        public async Task<ActionResult> Edit([Bind(Include = "ChildId,MembershipId,FullName,ClassLevel,MediaConsent,AmbulanceCover")] Child child)
        {
            if (ModelState.IsValid)
            {
                db.Entry(child).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.MembershipId = new SelectList(db.Memberships, "MembershipId", "MembershipNumber", child.MembershipId);
            return View(child);
        }

        // GET: Children/Delete/5
         [SimpleAuthorize(Roles = "DSEditor")]
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Child child = await db.Children.FindAsync(id);
            if (child == null)
            {
                return HttpNotFound();
            }
            return View(child);
        }

        // POST: Children/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [SimpleAuthorize(Roles = "DSEditor")]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Child child = await db.Children.FindAsync(id);
            db.Children.Remove(child);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        [HttpPost, ActionName("AddComment")]
        public JsonResult AddComment(ChildComment childComment)
        {
            Child child = db.Children.Find(childComment.ChildId);
            childComment.CreatedOn = System.DateTime.Now;
            child.Comments.Add(childComment);

            db.SaveChanges();

            return Json(new { result = "OK" });
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
