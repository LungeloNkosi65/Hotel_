using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ExploreBookings.Models;
using ExploreBookings.Models.Logic;
using Microsoft.AspNet.Identity;

namespace ExploreBookings.Controllers
{
    public class CustomBookingsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: CustomBookings
        public ActionResult Index()
        {
            var customBookings = db.CustomBookings.Include(c => c.Room);
            return View(customBookings.ToList());
        }

        // GET: CustomBookings/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CustomBooking customBooking = db.CustomBookings.Find(id);
            if (customBooking == null)
            {
                return HttpNotFound();
            }
            return View(customBooking);
        }

        // GET: CustomBookings/Create
        public ActionResult Create(int? id)
        {
            //ViewBag.Id = id;
            Session["RoomId"] = id;
            ViewBag.RoomId = new SelectList(db.Rooms, "RoomId", "roomDescription");
            return View();
        }

        // POST: RoomBookings/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "RoomBookingId,RoomType,RoomId,GuestEmail,RoomPrice,Total,CheckInDate,CheckOutDate,NumberOfDays,NumberOfPeople,NumberOfRooms,Status,HotelAddress,ManagerEmail")] CustomBooking roomBooking)
        {
            var userName = User.Identity.GetUserName();
            //roomBooking.RoomId = int.Parse(Session["RoomId"].ToString());

            var recordss = db.RoomBookings.Where(x => x.RoomId == roomBooking.RoomId).Select(x => x.CheckOutDate).FirstOrDefault();
            if (ModelState.IsValid)
            {
                if (roomBooking.CheckInDate >= recordss)
                {
                    if (BusinessLogic.dateLessChecker1(roomBooking) == false)
                    {
                        if (BusinessLogic.dateLessChecker1(roomBooking) == false)
                        {

                            roomBooking.GuestEmail = userName;
                            roomBooking.ManagerEmail = BusinessLogic.GetHotelManagerEmail(roomBooking.RoomId);
                            roomBooking.RoomPrice = BusinessLogic.GetRoomPrice(roomBooking.RoomId);
                            roomBooking.RoomType = BusinessLogic.GetRoomType(roomBooking.RoomId);
                            roomBooking.NumberOfDays = BusinessLogic.GetNumberDays(roomBooking.CheckInDate, roomBooking.CheckOutDate);
                            roomBooking.Status = "Not yet Checked In!!";
                            roomBooking.Total = BusinessLogic.calcTotalRoomCost1(roomBooking);
                            roomBooking.HotelAddress = BusinessLogic.GetHotelAddress(roomBooking.RoomId);

                            db.CustomBookings.Add(roomBooking);
                            db.SaveChanges();
                            Session["bookID"] = roomBooking.RoomBookingId;
                            ViewBag.RoomId = new SelectList(db.Rooms, "RoomId", "roomDescription", roomBooking.RoomId);
                            return RedirectToAction("Index");
                        }
                        else
                        {
                            ModelState.AddModelError("", "Check in date cannot be greater than checkout date!!");
                            ViewBag.RoomId = new SelectList(db.Rooms, "RoomId", "roomDescription", roomBooking.RoomId);
                            return View(roomBooking);
                        }

                    }
                    else
                    {

                        ModelState.AddModelError("", "Cannot book for a date that has passed!!");
                        ViewBag.RoomId = new SelectList(db.Rooms, "RoomId", "roomDescription", roomBooking.RoomId);
                        return View(roomBooking);
                    }
                }
                else
                {
                    ModelState.AddModelError("", $"Room already booked!! Please Choose date after {recordss}");
                    ViewBag.RoomId = new SelectList(db.Rooms, "RoomId", "roomDescription", roomBooking.RoomId);
                    return View(roomBooking);
                }

            }
            ViewBag.RoomId = new SelectList(db.Rooms, "RoomId", "roomDescription", roomBooking.RoomId);
            return View(roomBooking);
        }
        // GET: CustomBookings/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CustomBooking customBooking = db.CustomBookings.Find(id);
            if (customBooking == null)
            {
                return HttpNotFound();
            }
            ViewBag.RoomId = new SelectList(db.Rooms, "RoomId", "roomDescription", customBooking.RoomId);
            return View(customBooking);
        }

        // POST: CustomBookings/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "RoomBookingId,RoomType,RoomId,GuestEmail,RoomPrice,Total,CheckInDate,CheckOutDate,NumberOfDays,NumberOfPeople,NumberOfRooms,Status,HotelAddress,ManagerEmail")] CustomBooking customBooking)
        {
            if (ModelState.IsValid)
            {
                db.Entry(customBooking).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.RoomId = new SelectList(db.Rooms, "RoomId", "roomDescription", customBooking.RoomId);
            return View(customBooking);
        }

        // GET: CustomBookings/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CustomBooking customBooking = db.CustomBookings.Find(id);
            if (customBooking == null)
            {
                return HttpNotFound();
            }
            return View(customBooking);
        }

        // POST: CustomBookings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            CustomBooking customBooking = db.CustomBookings.Find(id);
            db.CustomBookings.Remove(customBooking);
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
