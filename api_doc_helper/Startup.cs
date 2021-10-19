using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SpaServices.AngularCli;
using System.Net.Http;
using System.Threading;
using Quantum.ApiDoc.Helper;
using SuperGMS.Config;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Serialization;

namespace SuperGMS.ApiDoc
{
  public class Startup
  {
    public Startup(IWebHostEnvironment configuration)
    {
    }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
      //services.AddMemoryCache();           
      services.AddMvc(options =>
      {
        options.EnableEndpointRouting = false;
      })
      .AddNewtonsoftJson(options => { options.SerializerSettings.ContractResolver = new DefaultContractResolver(); });

      services.AddSpaStaticFiles(configuration =>
      {
        configuration.RootPath = "wwwroot";
      });

      var uri =
        $"http://{ServerSetting.GetRpcServer(Program.ServerName).Ip}:{ServerSetting.GetRpcServer(Program.ServerName).Port}/";
      services.AddHttpClient("local", c =>
      {
        c.BaseAddress = new Uri(uri);
        c.DefaultRequestHeaders.Add("Accept", "application/json");
        c.DefaultRequestHeaders.Add("User-Agent", "HttpClient");
              //超时设定为 60s
              c.Timeout = new TimeSpan(0, 1, 0);
        c.MaxResponseContentBufferSize = 1000000;
      });

      var apiUri = ServerSetting.GetConstValue("HttpProxy")?.Value??"http://192.168.7.207/v2_api/";
      services.AddHttpClient("httpProxy", c =>
      {
        c.BaseAddress = new Uri(apiUri);
        c.DefaultRequestHeaders.Add("Accept", "application/json");
        c.DefaultRequestHeaders.Add("User-Agent", "HttpClient");
            //超时设定为 60s
            c.Timeout = new TimeSpan(0, 1, 0);
        c.MaxResponseContentBufferSize = 10000000;
      });


      //services.AddHttpClient("httpProxy2", c =>
      //{
      //  c.BaseAddress = new Uri("http://192.168.100.121/api2/");
      //  c.DefaultRequestHeaders.Add("Accept", "application/json");
      //  c.DefaultRequestHeaders.Add("User-Agent", "HttpClient");
      //      //超时设定为 60s
      //      c.Timeout = new TimeSpan(0, 1, 0);
      //  c.MaxResponseContentBufferSize = 10000000;
      //});

      services.AddSession();
    }
    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
      Thread th = new Thread(InitHelpDoc)
      {
        IsBackground = true
      };
      th.Start(app);

      var root = env.WebRootPath;
      if (env.IsDevelopment())
      {
        app.UseDeveloperExceptionPage();
      }
      else
      {
        app.UseExceptionHandler("/Home/Error");
        app.UseHsts();
      }
      app.UseHttpsRedirection();
      app.UseDefaultFiles();
      app.UseStaticFiles();
      app.UseSpaStaticFiles();
      app.UseMvc();
      app.UseSpa(spa =>
      {
        spa.Options.SourcePath = "ClientApp";
        if (env.IsDevelopment())
        {
          spa.UseAngularCliServer(npmScript: "start");
        }
      });
    }

    private void InitHelpDoc(object appObject)
    {
      var app = appObject as IApplicationBuilder;
      var host = ServerSetting.GetConstValue("HttpProxy")?.Value??"http://192.168.7.207/v2_api/";
      var user = "admin";
      var pwd = SuperGMS.Tools.EncryptionTools.HashMd5("sx123456");
      var localHelpDoc = false; //(ServerSetting.GetConstValue("ApiHelpType")?.Value?.ToLower() ?? "local") == "local";
      var openApi = false; //(ServerSetting.GetConstValue("OpenApi")?.Value?.ToLower() ?? "0") == "1";
      var sp = app?.ApplicationServices;

     // while (sp != null)
      //{
        try
        {
          var hostService = sp.GetService(typeof(IWebHostEnvironment)) as IWebHostEnvironment;
          var currPath = hostService?.WebRootPath;
          var cf = sp.GetService(typeof(IHttpClientFactory)) as IHttpClientFactory;
          var client = cf.CreateClient("httpProxy");
         client.Timeout =TimeSpan.FromSeconds(10 * 60 * 1000);
          //var clien2 = cf.CreateClient("httpProxy2");
          //var ah = new HttpApiHelper(clien2, "6a85d414-e2a8-421a-99d4-050b02a199d8");

          var apiHelper = new HttpApiHelper(client,"", host);
          if (!openApi)
          {
            apiHelper.ClientType = "webapi";
          }
          Program.helper = new InterfaceHelper(apiHelper);
          var svrs = Program.helper.GetAllServices().Result;
          lock (Program.lockObject)
          {
            Program.Svrs = svrs;
          }

          //string token = string.Empty;

          //if (!localHelpDoc)
          //{
          //  token = apiHelper.InitToken(host, user, pwd).Result;
          //}

          //if (localHelpDoc || string.IsNullOrEmpty(token))
          //{
          //  if (svrs?.Count > 0)
          //  {
          //    var dict = new LocalJsonHelper(currPath, "").GetAllServieInterfaces(svrs).Result;
          //    //过滤非openApi
          //    if (openApi)
          //    {
          //      dict?.Any(x =>
          //      {
          //        dict[x.Key] = x.Value.Where(o => (o?.IsPublic ?? false)).ToList();
          //        return false;
          //      });
          //    }
          //    lock (Program.lockObject)
          //    {
          //      Program.Dict = dict;
          //    }
          //  }
          //}
          //else
          //{
            var dict = Program.helper.GetAllServieInterfaces().Result;
            if (dict.Count > 0)
            {
              try
              {
                //写文件
                new LocalJsonHelper(currPath, "").WriteJsonFile(dict);
              }
              catch (Exception ex)
              {
                Trace.WriteLine(ex.Message);
              }
              if (openApi)
              {
                dict?.Any(x =>
                {
                  dict[x.Key] = x.Value.Where(o => (o?.IsPublic ?? false)).ToList();
                  return false;
                });
              }
              lock (Program.lockObject)
              {
                Program.Dict = dict;
              }
            }
          //}

         // Thread.Sleep(60*1000);
        }
        catch (Exception e)
        {
          Trace.WriteLine(e.Message);
          //Thread.Sleep(60000);
        }
      //}


    }
  }
}
