using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ExploreBookings.Models
{
    public class HotelManager
    {
        [Key]
        public int HotelManagerId { get; set; }
        public string UserId { get; set; }

        [RegularExpression(pattern: @"^[a-zA-Z''-'\s]{1,40}$", ErrorMessage = "Numbers and special characters are not allowed.")]
        [StringLength(maximumLength: 228, ErrorMessage = "First Name must be atleast 3 characters long", MinimumLength = 3)]
        [DisplayName("Full Name")]
        public string FullName { get; set; }

        [RegularExpression(pattern: @"^[a-zA-Z''-'\s]{1,40}$", ErrorMessage = "Numbers and special characters are not allowed.")]
        [StringLength(maximumLength: 228, ErrorMessage = "Last Name must be atleast 3 characters long", MinimumLength = 3)]
        [DisplayName("Last Name")]
        public string LastName { get; set; }

        [Display(Name = "Email")]
        [DataType(dataType: DataType.EmailAddress)]
        [RegularExpression(pattern: @"^\w+[\w-\.]*\@\w+((-\w+)|(\w*))\.[a-z]{2,3}$", ErrorMessage = "Email not valid")]
        public string Email { get; set; }

        [Display(Name = "Phone Number")]
        [DataType(DataType.PhoneNumber)]
        [RegularExpression(pattern: @"^\(?([0]{1})\)?[-. ]?([1-9]{1})[-. ]?([0-9]{8})$", ErrorMessage = "Entered Phone format is not valid.")]
        [StringLength(maximumLength: 10, ErrorMessage = "SA Contact Number must be exactly 10 digits long", MinimumLength = 10)]
        public string Phone { get; set; }
        public string Gender { get; set; }
        public string Address { get; set; }
    }
}