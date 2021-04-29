using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using FileServer.Middleware;
using FileServer.Models;
using SuperGMS.Config;
using SuperGMS.Log;
using System;
using SuperGMS.Cache;

namespace FileServer
{
    /// <summary>
    /// 系统启动类
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Startup"/> class.
        /// 启动程序
        /// </summary>
        /// <param name="configuration">配置项</param>
        public Startup(IConfiguration configuration)
        {
            this.Configuration = configuration;
            var rpc = ServerSetting.GetRpcServer(WebConfig.serverName);
            ServerSetting.RegisterRouter(ServerSetting.AppName,rpc.Ip,rpc.Port,rpc.Enable,rpc.TimeOut);
            WebConfig = new WebConfig();
            // this.Configuration.GetSection("Cfg").Bind(WebConfig);
        }

        /// <summary>
        /// Gets or sets 配置量
        /// </summary>
        public static WebConfig WebConfig { get; set; }
        /// <summary>
        /// Gets or sets 配置项
        /// </summary>
        public IConfiguration Configuration { get; set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMemoryCache(options =>
            {
                // options.SizeLimit = 1024 * 1024 * 500; // 500Mb
            });

            services.Configure<FormOptions>(o => {
                o.ValueLengthLimit = int.MaxValue;
                o.MultipartBodyLengthLimit = int.MaxValue;
                o.MemoryBufferThreshold = int.MaxValue;
            });            
            services.AddMvc(options=> 
            {
                options.EnableEndpointRouting = false;
            });

            services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", p => p.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            var config = ServerSetting.GetConfiguration();
            SuperGMS.Redis.RedisConfig.Initlize(config.RedisConfig);
            CacheManager.Initlize(new RedisCache());
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                //app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStatusCodePagesWithReExecute("/Home/{0}");
            app.UseStaticFiles(new StaticFileOptions
            {
                ServeUnknownFileTypes = true,
            });

            app.UseMiddleware<TokenMiddleware>();
            app.UseCors("AllowAll");

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }

    }
}
