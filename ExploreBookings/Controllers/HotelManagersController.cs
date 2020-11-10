using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ExploreBookings.Models;
using Microsoft.AspNet.Identity;

namespace ExploreBookings.Controllers
{
    public class HotelManagersController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: HotelManagers
        public ActionResult Index()
        {
            var userName = User.Identity.GetUserName();
            if (User.IsInRole("HotelManager"))
            {
                return View(db.hotelManagers.ToList().Where(x => x.Email == userName));
            }
            else
            {
                return View(db.hotelManagers.ToList());
            }          
        }

        // GET: HotelManagers/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            HotelManager hotelManager = db.hotelManagers.Find(id);
            if (hotelManager == null)
            {
                return HttpNotFound();
            }
            return View(hotelManager);
        }

        // GET: HotelManagers/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: HotelManagers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "HotelManagerId,UserId,FullName,LastName,Email,Phone,Gender,Address")] HotelManager hotelManager)
        {
            if (ModelState.IsValid)
            {
                db.hotelManagers.Add(hotelManager);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(hotelManager);
        }

        // GET: HotelManagers/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            HotelManager hotelManager = db.hotelManagers.Find(id);
            if (hotelManager == null)
            {
                return HttpNotFound();
            }
            return View(hotelManager);
        }

        // POST: HotelManagers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "HotelManagerId,UserId,FullName,LastName,Email,Phone,Gender,Address")] HotelManager hotelManager)
        {
            if (ModelState.IsValid)
            {
                db.Entry(hotelManager).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(hotelManager);
        }

        // GET: HotelManagers/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            HotelManager hotelManager = db.hotelManagers.Find(id);
            if (hotelManager == null)
            {
                return HttpNotFound();
            }
            return View(hotelManager);
        }

        // POST: HotelManagers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            HotelManager hotelManager = db.hotelManagers.Find(id);
            db.hotelManagers.Remove(hotelManager);
            db.SaveChanges();
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
