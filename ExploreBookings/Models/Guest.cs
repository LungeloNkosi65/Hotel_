using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ExploreBookings.Models
{
    public class Guest
    {
        [Key]
        public int GuestId { get; set; }
        public string UserId { get; set; }
    
        [DisplayName("Full Name")]
        public string FullName { get; set; }
     
        [DisplayName("Last Name")]
        public string LastName { get; set; }
        [Display(Name = "Email")]
        public string Email { get; set; }
        [Display(Name = "Phone Number")]
        [DataType(DataType.PhoneNumber)]
    
        public string Phone { get; set; }
        public string Gender { get; set; }
        public string Address { get; set; }

    }
}