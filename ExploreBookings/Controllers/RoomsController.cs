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
    public class RoomsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Rooms
        public ActionResult Index()
        {
            var rooms = db.Rooms.Include(r => r.hotels).Include(r => r.roomTypes);
            return View(rooms.ToList());
        }
        public ActionResult RoomBooking()
        {
            var rooms = db.Rooms.Include(r => r.hotels).Include(r => r.roomTypes);
            return View(rooms.ToList());
        }
        public ActionResult RoomBooking1(int? id)
        {
            var rooms = db.Rooms.Include(r => r.hotels).Include(r => r.roomTypes);
            return View(rooms.ToList().Where(x=>x.HotelId == id));
        }
        // GET: Rooms/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Rooms rooms = db.Rooms.Find(id);
            if (rooms == null)
            {
                return HttpNotFound();
            }
            return View(rooms);
        }

        // GET: Rooms/Create
        public ActionResult Create()
        {
            var userName = User.Identity.GetUserName();
            ViewBag.HotelId = new SelectList(db.Hotels.Where(m=>m.ManagerEmail == userName), "HotelId", "HotelName");
            ViewBag.roomtypeId = new SelectList(db.RoomTypes, "RoomtypeId", "Type");
            return View();
        }

        // POST: Rooms/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "RoomId,HotelId,roomtypeId,RoomCapacity,roomDescription,RoomPicture,RoomPrice,Status")] Rooms rooms, HttpPostedFileBase photoUpload)
        {

            byte[] photo = null;
            photo = new byte[photoUpload.ContentLength];
            photoUpload.InputStream.Read(photo, 0, photoUpload.ContentLength);
            rooms.RoomPicture = photo;

            if (ModelState.IsValid)
            {
                db.Rooms.Add(rooms);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.HotelId = new SelectList(db.Hotels, "HotelId", "HotelName", rooms.HotelId);
            ViewBag.roomtypeId = new SelectList(db.RoomTypes, "RoomtypeId", "Type", rooms.roomtypeId);
            return View(rooms);
        }

        // GET: Rooms/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Rooms rooms = db.Rooms.Find(id);
            if (rooms == null)
            {
                return HttpNotFound();
            }
            ViewBag.HotelId = new SelectList(db.Hotels, "HotelId", "HotelName", rooms.HotelId);
            ViewBag.roomtypeId = new SelectList(db.RoomTypes, "RoomtypeId", "Type", rooms.roomtypeId);
            return View(rooms);
        }

        // POST: Rooms/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "RoomId,HotelId,roomtypeId,RoomCapacity,roomDescription,RoomPicture,RoomPrice,Status")] Rooms rooms)
        {
            if (ModelState.IsValid)
            {
                db.Entry(rooms).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.HotelId = new SelectList(db.Hotels, "HotelId", "HotelName", rooms.HotelId);
            ViewBag.roomtypeId = new SelectList(db.RoomTypes, "RoomtypeId", "Type", rooms.roomtypeId);
            return View(rooms);
        }

        // GET: Rooms/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Rooms rooms = db.Rooms.Find(id);
            if (rooms == null)
            {
                return HttpNotFound();
            }
            return View(rooms);
        }

        // POST: Rooms/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Rooms rooms = db.Rooms.Find(id);
            db.Rooms.Remove(rooms);
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
