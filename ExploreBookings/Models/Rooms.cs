using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExploreBookings.Models
{
    public class Rooms
    {
        [Key]
        public int RoomId { get; set; }
        [DisplayName("Hotel")]
        public int HotelId { get; set; }
        [DisplayName("Room Type")]
        public int roomtypeId { get; set; }
        [DisplayName("Room Capacity")]
        public int RoomCapacity { get; set; }
        [DisplayName("Room Description")]
        public string roomDescription { get; set; }
        public byte[] RoomPicture { get; set; }
        [DisplayName("Room Price"), DataType(DataType.Currency)]
        public decimal RoomPrice { get; set; }
        [DisplayName("Room Status")]
        public string Status { get; set; }
        public string RoomNumber { get; set; }
        public virtual Hotel hotels { get; set; }
        public virtual RoomType roomTypes { get; set; }
    }
}