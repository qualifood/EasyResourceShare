using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Security;
using IamUsingIt.Models;
using Microsoft.AspNet.Identity;
using NodaTime;

namespace IamUsingIt.Controllers
{
    [Authorize]
    public class ReservationsController : Controller
    {
        private readonly ApplicationDbContext _db = new ApplicationDbContext();

        // GET: Reservations
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
            if (IsReservationConflicted(reservation)) ModelState.AddModelError("ErrorMessage", "This reservation overlaps with an already existing reservation!");
            if (ModelState.IsValid)
            {
                _db.Reservations.Add(reservation);
                await _db.SaveChangesAsync();
                return RedirectToAction("Index", "Resources");
            }

            reservation.Resource = _db.Resources.Find(reservation.ResourceId);
            return View(reservation);
        }

        private bool IsReservationConflicted(Reservation reservation)
        {
            var resInterval = new Interval(new Instant(reservation.Begin.Ticks), new Instant(reservation.End.Ticks));
            var existingIntervals =
                _db.Reservations.Where(r => r.ResourceId == reservation.ResourceId).ToList()
                    .Select(r => new Interval(new Instant(r.Begin.Ticks), new Instant(r.End.Ticks)))
                    .ToList();
            return existingIntervals.Any(i => Overlaps(i, resInterval));
        }

        private bool Overlaps(Interval interval, Interval resInterval)
        {
            if (interval.Contains(resInterval.Start)) return true;
            if (interval.Contains(resInterval.End)) return true;
            if (resInterval.Contains(interval.Start)) return true;
            if (resInterval.Contains(interval.End)) return true;
            return false;
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
        public async Task<ActionResult> DeleteConfirmed(int? id)
        {
            var reservation = _db.Reservations.Include(r=>r.Resource).FirstOrDefault(r=>r.ResourceId == id);
            if (reservation == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var userId = User.Identity.GetUserId();

            var isAdmin = User.IsInRole("Admin");
            if (reservation.UserId != userId && !isAdmin) ModelState.AddModelError("ErrorMessage", "This Reservation does not belong to you!");
            if (ModelState.IsValid)
            {
                _db.Reservations.Remove(reservation);
                await _db.SaveChangesAsync();
                return RedirectToAction("Index", "Resources");
            }
            else
            {
                return View(reservation);
            }
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
