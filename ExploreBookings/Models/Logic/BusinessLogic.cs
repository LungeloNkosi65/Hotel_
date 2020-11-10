using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace ExploreBookings.Models.Logic
{
    public class BusinessLogic
    {
        private static ApplicationDbContext db = new ApplicationDbContext();
        public static string GetRoomType(int roomId)
        {
            var roomBuilding = (from rb in db.Rooms
                                where rb.RoomId == roomId
                                select rb.roomTypes.Type).FirstOrDefault();
            return roomBuilding;
        }
        public static decimal GetRoomPrice(int roomId)
        {
            var roomPrice = (from rb in db.Rooms
                                where rb.RoomId == roomId
                                select rb.RoomPrice).FirstOrDefault();
            return roomPrice;
        }
        public static decimal GetRoomCapacity(int roomId)
        {
            var roomCapacity = (from rb in db.Rooms
                             where rb.RoomId == roomId
                             select rb.RoomCapacity).FirstOrDefault();
            return roomCapacity;
        }
        public static string GetHotelAddress(int roomId)
        {
            var hotelId = (from rb in db.Rooms
                                where rb.RoomId == roomId
                                select rb.HotelId).FirstOrDefault();

            var address = (from rb in db.Hotels
                           where rb.HotelId == hotelId
                           select rb.Address).FirstOrDefault();

            return address;
        }
        public static string GetHotelManagerEmail(int roomId)
        {
            var hotelId = (from rb in db.Rooms
                           where rb.RoomId == roomId
                           select rb.HotelId).FirstOrDefault();

            var email = (from rb in db.Hotels
                           where rb.HotelId == hotelId
                           select rb.ManagerEmail).FirstOrDefault();

            return email;
        }
        public static Int32 GetNumberDays(DateTime Check_in, DateTime Check_Out)
        {
            return ((Check_Out.Date - Check_in.Date).Days);
        }
        public static decimal calcTotalRoomCost(RoomBooking roomBooking)
        {
            return GetRoomPrice(roomBooking.RoomId) * GetNumberDays(roomBooking.CheckInDate, roomBooking.CheckOutDate);
        }    public static decimal calcTotalRoomCost1(CustomBooking roomBooking)
        {
            return GetRoomPrice(roomBooking.RoomId) * GetNumberDays(roomBooking.CheckInDate, roomBooking.CheckOutDate);
        }
        public static bool dateLessOutChecker(RoomBooking roomBooking)
        {
            bool check = false;            
            if (roomBooking.CheckInDate >= roomBooking.CheckOutDate)
            {
                check = true;
            }
            return check;
        }

        public static bool dateLessChecker(RoomBooking roomBooking)
        {
            bool check = false;
            if (roomBooking.CheckInDate < DateTime.Now)
            {
                check = true;
            }
            return check;
        }
        public static bool roomChecker(RoomBooking roomBooking)
        {
            bool check = false;
            DateTime outDate = (from r in db.RoomBookings
                           where r.RoomId == roomBooking.RoomId
                           select r.CheckOutDate
                         ).FirstOrDefault();
            if (roomBooking.CheckInDate <= outDate)
            {
                check = true;
            }
            return check;
        }   public static bool dateLessChecker1(CustomBooking roomBooking)
        {
            bool check = false;
            if (roomBooking.CheckInDate < DateTime.Now)
            {
                check = true;
            }
            return check;
        }
        public static bool roomChecker1(CustomBooking roomBooking)
        {
            bool check = false;
            DateTime outDate = (from r in db.RoomBookings
                           where r.RoomId == roomBooking.RoomId
                           select r.CheckOutDate
                         ).FirstOrDefault();
            if (roomBooking.CheckInDate <= outDate)
            {
                check = true;
            }
            return check;
        }
        public static void UpdateRoomsAvailable(int roomId)
        {
            var roomTypeId = (from rb in db.Rooms
                             where rb.RoomId == roomId
                             select rb.roomtypeId).FirstOrDefault();

            var roomsAvail = (from rb in db.RoomTypes
                              where rb.RoomtypeId == roomTypeId
                              select rb).FirstOrDefault();
            roomsAvail.RoomAvailable -= 1;
            db.Entry(roomsAvail).State = EntityState.Modified;
            db.SaveChanges();

        }
    }
}