using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SuperGMS.ApiHelper;
using SuperGMS.Config;

namespace SuperGMS.ApiDoc
{
  public class Program
  {
    public static object lockObject = new object();
    public const string ServerName = "ApiDocument";
    public static Dictionary<string, List<ClassInfo>> Dict = new Dictionary<string, List<ClassInfo>>();
    public static List<string> Svrs = new List<string>();
    public static void Main(string[] args)
    {
      //var path = Directory.GetCurrentDirectory();
      //Console.WriteLine($"{path}");
      ////先执行拷贝文件的命令
      //if (File.Exists(Path.Combine(path, "scm.json")))
      //{
      //  File.Delete(Path.Combine(path, "wwwroot/assets/scm.json"));
      //  File.Copy(Path.Combine(path, "scm.json"), Path.Combine(path, "wwwroot/assets/scm.json"));
      //}
      ServerSetting.Initlize(ServerName, 1);

      // ScmParse.Init();
      CreateWebHostBuilder(args).Build().Run();
    }

    public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
      WebHost.CreateDefaultBuilder(args)
        .UseUrls($"http://{ServerSetting.GetRpcServer(ServerName).Ip}:{ServerSetting.GetRpcServer(ServerName).Port}/")
        .UseStartup<Startup>();
  }

}
