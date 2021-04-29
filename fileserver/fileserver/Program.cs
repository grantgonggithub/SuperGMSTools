using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using FileServer.Models;
using SuperGMS.Config;
using System.Threading;

namespace FileServer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            ThreadPool.SetMinThreads(200, 200);
            ThreadPool.SetMaxThreads(1000, 1000);
            ServerSetting.Initlize(WebConfig.serverName,1);
            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseUrls($"http://*:{ServerSetting.GetRpcServer(WebConfig.serverName).Port}/")
                .UseStartup<Startup>()
                .Build();
    }
}
