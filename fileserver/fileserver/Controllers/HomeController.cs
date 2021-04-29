using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Net.Http.Headers;
using FileServer.Extensions;
using FileServer.Models;
using FileServer.Services;

using SuperGMS.UserSession;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using SuperGMS.Cache;

namespace FileServer.Controllers
{
    /// <summary>
    /// 首页面控制器
    /// </summary>
    public class HomeController : Controller
    {
        private IHostingEnvironment _env;
        private readonly IMemoryCache cache;

        public HomeController(IHostingEnvironment env, IMemoryCache memoryCache)
        {
            _env = env;
            this.cache = memoryCache;
        }

        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 返回图片
        /// </summary>
        /// <param name="token">token</param>
        /// <param name="file">图片路径</param>
        /// <returns>文件流</returns>
        [HttpGet]
        [Route("Home/GetImage/{token}")]
        public async Task<IActionResult> GetImage(string token, string file)
        {

            if (string.IsNullOrEmpty(file))
            {
                return this.File("/images/noimg.png", "image/png");
            }

            var user = CacheManager.Get<UserContext>(token);
            if (user == null)
            {
                return this.File("/images/noimg.png", "image/png");
            }

            IFileOperate fileServer = null;
            try
            {
                fileServer = user.GetFileServer(this.cache);
                fileServer.Connect();
                var stream = new MemoryStream();
                fileServer.Get(file, stream);
                stream.Seek(0, SeekOrigin.Begin);
                fileServer.Disconnect();
                if (stream.Length <= 0)
                {
                    return this.File("/images/noimg.png", "image/png");
                }

                return this.File(stream, this.GetMime(file));
            }
            catch (Exception ex)
            {
                return this.File("/images/noimg.png", "image/png");
            }
            finally
            {
                fileServer?.Disconnect();
            }
        }

        /// <summary>
        /// 得到文件的mime
        /// </summary>
        /// <param name="file">文件路径</param>
        /// <returns>mime</returns>
        private string GetMime(string file)
        {
            try
            {
                FileInfo fi = new FileInfo(file);
                var ext = fi.Extension;
                var provider = new FileExtensionContentTypeProvider();
                if (provider.Mappings.ContainsKey(ext))
                {
                    var mime = provider.Mappings[ext];
                    return mime;
                }
                return "text / plain";
            }
            catch (Exception ex)
            {
                return "application/octet-stream";
            }
        }


        [Route("Home/Error")]
        public IActionResult Error(string msg)
        {
            return View(new ErrorViewModel { Message = msg, RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [Route("Home/Download")]
        public IActionResult Download(string file)
        {
            if (string.IsNullOrEmpty(file))
            {
                return new RedirectResult("/333/333/333", false);
            }

            var stream = new FileStream("E:\\MyWeb.zip", FileMode.Open);
            return this.File(stream, "application/octet-stream", "abc.zip");
        }

        [Route("Home/{statusCode}")]
        public IActionResult CustomError(int statusCode)
        {
            var filePath = $"{_env.WebRootPath}/errors/{(statusCode == 404 ? 404 : 500)}.html";
            return new PhysicalFileResult(filePath, new MediaTypeHeaderValue("text/html"));
        }
    }
}
