using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Egnyte.Api.WebSample.Startup))]
namespace Egnyte.Api.WebSample
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
