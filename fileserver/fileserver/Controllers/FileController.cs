// <copyright file="FileController.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using FileServer.Extensions;
using FileServer.Models;
using FileServer.Services;
using SuperGMS.Extend.FileServer;
using SuperGMS.Log;
using SuperGMS.Protocol.RpcProtocol;
using SuperGMS.UserSession;
using StackExchange.Profiling;
using System;
using System.Data;
using System.DrawingCore;
using System.DrawingCore.Drawing2D;
using System.DrawingCore.Imaging;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SuperGMS.Cache;
using System.Net;

namespace FileServer.Controllers
{
    /// <summary>
    /// 文件控制器
    /// </summary>
    // [Consumes("application/json", "multipart/form-data")] // 此处为新增
    [Produces("application/json")]
    [Route("file_server/[controller]/[action]")]
    [Route("file_server/[controller]/[action]/{token}")]
    [EnableCors("AllowAll")]
    public class FileController : Controller
    {
        /// <summary>
        /// Gets or sets 日志仓储
        /// </summary>
        private readonly static ILogger Logger = LogFactory.CreateLogger<FileController>();
        private readonly IMemoryCache cache;
        private string tid = string.Empty;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileController"/> class.
        /// 构造函数
        /// </summary>
        /// <param name="memoryCache">缓存</param>
        public FileController(IMemoryCache memoryCache)
        {
            this.cache = memoryCache;
        }

        /// <summary>
        /// 下载文件 ，form形式，前端url转为 form格式提交
        /// </summary>
        /// <param name="downFileInfo">下载文件信息</param>
        /// <returns>文件流或 错误定位页面</returns>
        [HttpGet]
        public async Task<IActionResult> Download(string file)
        {
            return await this.DownloadInner(file, null, null);
        }
        /// <summary>
        /// 下载文件
        /// </summary>
        /// <param name="file">远程路径</param>
        /// <param name="fileName">原始名称</param>
        /// <param name="errUri">错误后定位路径</param>
        /// <returns>文件流,若错误，则返回json</returns>
        public async Task<IActionResult> DownloadInner(string file, string fileName, string errUri)
        {            

            tid = HttpContext.Items["rid"]?.ToString();

            if (string.IsNullOrEmpty(file))
            {
                Logger.LogInformation($"download:路径未指定");
                return new RedirectResult(errUri, false);
            }
            var temp = file;
            //前端系统bug兼容
            if (file.StartsWith("//") || file.StartsWith("\\\\") ||  file.StartsWith("/\\"))
            {
                file = file.Substring(1);
            }

            var user = (UserContext)this.HttpContext.Items["user"];
            var name = fileName ?? this.GetFileName(file);
            //if (user == null)
            //{
            //    Logger.LogInformation("download:用户非法或过期");
            //    return new RedirectResult(errUri, false);
            //}

            //var info = new FileCheckInfo()
            //{
            //    Action = FileOperateAction.FileUpload,
            //    BussinessType = this.Request.Headers.ContainsKey("BussinessType")
            //        ? this.Request.Headers["BussinessType"].ToString()
            //        : string.Empty,
            //    FileName = file,
            //    FileSize = -1,
            //    Guid = Guid.NewGuid().ToString("N"),
            //    Token = user.Token,
            //};

            var i = 0;
        Label:
            IFileOperate fileServer = null;
            try
            {
                using (fileServer = user.GetFileServer(this.cache))
                {
                    if (i == 0) file = fileServer.CombineRootPath(file);
                    var stream = new MemoryStream();
                    fileServer.Get(file, stream);
                    stream.Seek(0, SeekOrigin.Begin);
                    // name = HttpUtility.UrlEncode(name, System.Text.Encoding.UTF8); // 对文件名编码
                    // name = name.Replace("+", "%20"); // 解决空格被编码为"+"号的问题
                    return this.File(stream, this.GetMime(file), name);
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"download:访问文件{file} 失败。");
                //if (i<1)
                //{
                //    i += 1;
                //if (++i < Startup.WebConfig.RetryNum)
                //{
                //    Thread.Sleep(Startup.WebConfig.RetrySleep);
                //var f = temp.Split("\\/".ToCharArray(),StringSplitOptions.RemoveEmptyEntries);
                //if (f!=null&&f.Length>0)
                //{
                //    file = $"{temp}/{f[f.Length - 1]}";
                //    i += 1;
                //    goto Label;
                //}
                //}
                //return new RedirectResult(errUri, false);
                //}
                if (this.IsImage(name))
                    return this.File("/images/noimg.png", "image/png");
                else
                {
                    if (ex is DirectoryNotFoundException ||ex is FileNotFoundException)
                        return NotFound(ex.Message);
                    else
                    {
                        throw ex;
                    }
                }
            }            
        }

       /// <summary>
       /// 上传文件
       /// </summary>
       /// <param name="mime">类型</param>
       /// <param name="files">文件集合</param>
       /// <returns>文件名</returns>
        [HttpPost,DisableRequestSizeLimit]
        public Result<FileUploadInfo> Upload(string mime,IFormCollection files)
        {
            var uploadInfo = new FileUploadInfo();
            try
            {
                var user = (UserContext)this.HttpContext.Items["user"];
                tid = this.HttpContext.Items["rid"].ToString();
                if (user == null)
                {
                    Logger.LogInformation($"upload:用户不存在或过期");
                    return uploadInfo.ToResult(tid, SuperGMS.Protocol.RpcProtocol.StatusCode.LoginFailed);
                }
                if (files == null || files.Files == null)
                {
                    Logger.LogInformation($"files=null?{files==null},files.Files==null?{files.Files==null}");
                    return uploadInfo.ToResult(tid, new SuperGMS.Protocol.RpcProtocol.StatusCode(401,"上传文件内容为空"));
                }
                if (files.Files.Count > 0)
                {
                    var f = files.Files[0];
                    uploadInfo.Guid = Guid.NewGuid().ToString("N");
                    var i = 0;

                Label:
                    IFileOperate fileServer = null;
                    try
                    {
                        using (fileServer = user.GetFileServer(this.cache))
                        {
                            var dir = user.GetUserDir();
                            var filePath = string.Empty;
                            var rtn = true;
                            //仅上传一个文件
                            var file = files.Files.FirstOrDefault();
                            if (file == null)
                            {
                                Logger.LogInformation($"upload:没有上传文件。");
                                return uploadInfo.ToResult(tid, new SuperGMS.Protocol.RpcProtocol.StatusCode(902, "没有上传文件流"));
                            }

                            var ext = new FileInfo(file.FileName);
                            filePath = $"{dir}{Guid.NewGuid():N}{ext.Extension}";
                            using (var stream = file.OpenReadStream())
                            {
                                var tmpPath = fileServer.CombineRootPath(filePath);
                                rtn = fileServer.PutStream(stream, tmpPath);
                                try
                                {
                                    this.CreateThumbnail(fileServer, stream, tmpPath);
                                }
                                catch (Exception ex1)
                                {
                                    Logger.LogError(ex1, $"upload: {user.UserInfo.UserId} CreateThumbnail failed.");
                                }
                            }
                            uploadInfo.FileName = filePath;
                            return uploadInfo.ToResult(tid);
                        }

                    }
                    catch (Exception ex)
                    {
                        Logger.LogError(ex, $"upload:{user.UserInfo.UserId}上传文件失败");
                        if (++i < Startup.WebConfig.RetryNum)
                        {
                            Thread.Sleep(Startup.WebConfig.RetrySleep);
                            goto Label;
                        }
                        return uploadInfo.ToResult(tid, new StatusCode(901, ex.Message));
                    }
                }
                else
                {
                    Logger.LogInformation($"upload:没有选择文件!");
                    return uploadInfo.ToResult(tid, new SuperGMS.Protocol.RpcProtocol.StatusCode(402, "没有选择文件"));
                }
            }
            catch (Exception ex1)
            {
                Logger.LogError(ex1, $"上传文件失败");
                return uploadInfo.ToResult(tid, new SuperGMS.Protocol.RpcProtocol.StatusCode(405, $"上传文件失败:{ex1.Message},{ex1?.InnerException?.Message}"));
            }
        }

        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="file">远程文件路径和名称</param>
        /// <returns>删除名称</returns>
        [HttpGet]
        public Result<string> Delete(string file)
        {
            var user = (UserContext)this.HttpContext.Items["user"];
            tid = this.HttpContext.Items["rid"].ToString();
            if (user == null)
            {
                Logger.LogInformation($"用户为空");
                return string.Empty.ToResult(tid, SuperGMS.Protocol.RpcProtocol.StatusCode.LoginFailed);
            }

            IFileOperate fileServer = null;
            try
            {
                using (fileServer = user.GetFileServer(this.cache))
                {
                    var tmpPath = fileServer.CombineRootPath(file);
                    var ret = fileServer.Delete(tmpPath);
                    var fileExt = new FileInfo(tmpPath).Extension;
                    if (this.IsImage(fileExt))
                    {
                        var path = tmpPath.Substring(0, tmpPath.Length - fileExt.Length) + "_t" + fileExt;
                        fileServer.Delete(path);
                    }
                    return string.Empty.ToResult(tid, ret ? null : new StatusCode(903, $"删除文件失败"));
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"删除文件失败");
                return string.Empty.ToResult(tid, new StatusCode(901, ex.Message));
            }            
        }

        /// <summary>
        /// 返回图片
        /// </summary>
        /// <param name="token">token</param>
        /// <param name="file">图片路径</param>
        /// <returns>文件流</returns>
        [HttpGet]
        public async Task<IActionResult> GetImage(string token, string file)
        {
            var rid = this.HttpContext.Items["rid"].ToString();
            tid = rid;
            if (string.IsNullOrEmpty(file))
            {
                Logger.LogInformation("访问失败,文件参数为空");                
                return this.File("/images/noimg.png", "image/png");
            }

            var user = CacheManager.Get<UserContext>(token);
            if (user == null)
            {
                Logger.LogInformation("访问失败，token无效");                
                return this.File("/images/noimg.png", "image/png");
            }

            var i = 0;
            Label:
            IFileOperate fileServer = null;
            try
            {
                using (fileServer = user.GetFileServer(this.cache))
                {
                    if(i==0)file = fileServer.CombineRootPath(file);
                    var stream = new MemoryStream();
                    fileServer.Get(file, stream);
                    stream.Seek(0, SeekOrigin.Begin);
                    if (stream.Length <= 0)
                    {
                        Logger.LogInformation("访问失败，文件流为空");                        
                        return this.File("/images/noimg.png", "image/png");
                    }

                    return this.File(stream, this.GetMime(file));
                }
                
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"访问({i})失败,未知原因");
               
                if (++i < Startup.WebConfig.RetryNum)
                {
                    Thread.Sleep(Startup.WebConfig.RetrySleep);
                    goto Label;
                }
                return this.File("/images/noimg.png", "image/png");
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
                return "application/octet-stream";
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "获取mime失败,赋予默认值");
                return "application/octet-stream";
            }
        }

        /// <summary>
        /// 得到文件扩展名
        /// </summary>
        /// <param name="file">文件路径</param>
        /// <returns>扩展名</returns>
        private string GetFileName(string file)
        {
            try
            {
                FileInfo fi = new FileInfo(file);
                return fi.Name;
            }
            catch (Exception ex)
            {
                return Guid.NewGuid().ToString("N");
            }
        }

        /// <summary>
        /// 建立缩略图
        /// </summary>
        /// <param name="fileServer">后台文件操作</param>
        /// <param name="file">文件</param>
        /// <param name="fileNewName">新名称</param>
        private void CreateThumbnail(IFileOperate fileServer, Stream file, string fileNewName)
        {
            double targetWidth = 140;
            double targetHeight = 100;
            var fileExt = new FileInfo(fileNewName).Extension;
            if (this.IsImage(fileExt))
            {
                file.Seek(0, SeekOrigin.Begin);
                var path = fileNewName.Substring(0, fileNewName.Length - fileExt.Length) + "_t" + fileExt;
                using (Image initImage = Image.FromStream(file, true, true))
                {
                    // 原图宽高均小于模版，不作处理，直接保存
                    if (initImage.Width <= targetWidth && initImage.Height <= targetHeight)
                    {
                        // 保存缩略图
                        using (MemoryStream temp = new MemoryStream())
                        {
                            initImage.Save(temp, ImageFormat.Jpeg);
                            fileServer.PutStream(temp, path);
                        }
                    }
                    else
                    {
                        // 宽大于高或宽等于高（横图或正方）
                        if (initImage.Width >= initImage.Height )
                        {
                            // 如果宽大于模版
                            if (initImage.Width > targetWidth)
                            {
                                // 宽按模版，高按比例缩放
                                targetHeight = initImage.Height * (targetWidth / initImage.Width);
                            }
                        }
                        else
                        {
                            // 如果高大于模版
                            if (initImage.Height > targetHeight)
                            {
                                // 高按模版，宽按比例缩放
                                targetWidth = initImage.Width * (targetHeight / initImage.Height);
                            }
                        }

                        // 生成新图
                        using (var newImage = new Bitmap((int) targetWidth, (int) targetHeight))
                        {
                            using (var g = Graphics.FromImage(newImage))
                            {
                                // 设置质量
                                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                                g.SmoothingMode = SmoothingMode.HighQuality;

                                // 置背景色
                                g.Clear(Color.Transparent);
                                g.DrawImage(initImage, 0, 0, (int)targetWidth, (int)targetHeight);
                            }

                            using (MemoryStream temp = new MemoryStream())
                            {
                                newImage.Save(temp, ImageFormat.Jpeg);
                                fileServer.PutStream(temp, path);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 根据文件名判断是否是图片类型
        /// </summary>
        /// <param name="fileName">文件名</param>
        /// <returns>是否图片</returns>
        private bool IsImage(string fileName)
        {
            if (fileName.IndexOf('.') == -1)
            {
                return false;
            }

            switch (fileName.Substring(fileName.LastIndexOf('.')).ToLower())
            {
                case ".gif":
                case ".jpg":
                case ".png":
                case ".jpeg":
                    return true;
                default: return false;
            }
        }
    }
}
