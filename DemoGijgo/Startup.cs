using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(DemoGijgo.Startup))]
namespace DemoGijgo
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
