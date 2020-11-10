using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ExploreBookings.Models
{
    public class RoomBooking
    {
        [Key]
        public int RoomBookingId { get; set; }
        [DisplayName("Room Type")]
        public string RoomType { get; set; }
        public int RoomId { get; set; }
        [DisplayName("Guest Email")]
        public string GuestEmail { get; set; }        
        [DisplayName("Room Price"), DataType(DataType.Currency)]
        public decimal RoomPrice { get; set; }
        public decimal Total { get; set; }
        [DisplayName("Check-In-Date"), DataType(DataType.Date)]
        public DateTime CheckInDate { get; set; }
        [DisplayName("Check-Out-Date"), DataType(DataType.Date)]
        public DateTime CheckOutDate { get; set; }
        [DisplayName("Number Of Days")]
        public int NumberOfDays { get; set; }
        [DisplayName("Number Of People")]
        public int NumberOfPeople { get; set; }
        public string Status { get; set; }
        public string HotelAddress { get; set; }
        public string ManagerEmail { get; set; }
        public virtual Rooms Room { get; set; }

        
    }
}