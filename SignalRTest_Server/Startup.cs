using System;
using System.Threading.Tasks;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(SignalRTest_Server.Startup))]

namespace SignalRTest_Server
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            // 有关如何配置应用程序的详细信息，请访问 https://go.microsoft.com/fwlink/?LinkID=316888
            //设置可以跨域访问
            app.UseCors(Microsoft.Owin.Cors.CorsOptions.AllowAll);
            //映射到默认的管理
            app.MapSignalR();
        }
    }
}
