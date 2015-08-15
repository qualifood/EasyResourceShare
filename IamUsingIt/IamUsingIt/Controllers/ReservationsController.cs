using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using IamUsingIt.Models;
using Microsoft.AspNet.Identity;

namespace IamUsingIt.Controllers
{
    public class ReservationsController : Controller
    {
        private readonly ApplicationDbContext _db = new ApplicationDbContext();

        // GET: Reservations
        [Authorize]
        public async Task<ActionResult> Index(int? resourceId)
        {
            if (resourceId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var reservations = _db.Reservations.Include(r => r.Resource).Where(r=>r.ResourceId==resourceId);
            return View(await reservations.ToListAsync());
        }


        // GET: Reservations/Create
        public ActionResult Create(int? resourceId)
        {
            if (resourceId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var resource = _db.Resources.Find(resourceId);

            var model = new Reservation
            {
                Resource = resource,
                Begin = DateTime.Now,
                End = DateTime.Now.AddHours(3)
            };
            return View(model);
        }

        // POST: Reservations/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize(Roles=("User"))]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "ReservationId,ResourceId,Begin,End")] Reservation reservation)
        {
            var userId = User.Identity.GetUserId();
            var currentUser = _db.Users.Find(userId);
            reservation.User = currentUser;
            reservation.UserId = userId;
            if (ModelState.IsValid)
            {
                _db.Reservations.Add(reservation);
                await _db.SaveChangesAsync();
                return RedirectToAction("Index", "Resources");
            }

            ViewBag.ResourceId = new SelectList(_db.Resources, "ResourceId", "Name", reservation.ResourceId);
            return View(reservation);
        }



        // GET: Reservations/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Reservation reservation = _db.Reservations.Include(r=>r.Resource).FirstOrDefault(r=>r.ReservationId==id);
            if (reservation == null)
            {
                return HttpNotFound();
            }
            return View(reservation);
        }

        // POST: Reservations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Reservation reservation = await _db.Reservations.FindAsync(id);
            _db.Reservations.Remove(reservation);
            await _db.SaveChangesAsync();
            return RedirectToAction("Index", "Resources");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
