using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(ExploreBookings.Startup))]
namespace ExploreBookings
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
