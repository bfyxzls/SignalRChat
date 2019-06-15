using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(SignalR.Core.Startup))]
namespace SignalR.Core
{

    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.MapSignalR();
        }

    }
}