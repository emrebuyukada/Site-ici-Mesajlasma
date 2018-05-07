using System;
using System.Threading.Tasks;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(WebApplication7.Startup))]

namespace WebApplication7
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
 app.MapSignalR();
        }
    }
}
