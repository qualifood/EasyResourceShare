using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(IamUsingIt.Startup))]
namespace IamUsingIt
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
