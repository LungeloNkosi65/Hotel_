using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace ExploreBookings.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit https://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
        public DbSet<Admin> Admins { get; set; }
        public DbSet<Guest> guests { get; set; }
        public DbSet<HotelManager> hotelManagers { get; set; }

        public System.Data.Entity.DbSet<ExploreBookings.Models.Rooms> Rooms { get; set; }

        public System.Data.Entity.DbSet<ExploreBookings.Models.RoomType> RoomTypes { get; set; }

        public System.Data.Entity.DbSet<ExploreBookings.Models.Hotel> Hotels { get; set; }

        public System.Data.Entity.DbSet<ExploreBookings.Models.RoomBooking> RoomBookings { get; set; }

        public System.Data.Entity.DbSet<ExploreBookings.Models.CustomBooking> CustomBookings { get; set; }
    }
}