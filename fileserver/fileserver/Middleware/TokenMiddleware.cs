// <copyright file="TokenMiddleware.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using  FileServer.Extensions;
using  FileServer.Models;

using SuperGMS.ExceptionEx;
using SuperGMS.Log;
using SuperGMS.Protocol.RpcProtocol;
using SuperGMS.UserSession;
using System;
using System.IO;
using System.Threading.Tasks;
using FileServer.Services;
using SuperGMS.Cache;

namespace FileServer.Middleware
{
    /// <summary>
    /// 验证token
    /// </summary>
    public class TokenMiddleware
    {
        private readonly static ILogger logger = LogFactory.CreateLogger<TokenMiddleware>();
        private readonly RequestDelegate next;
        private readonly IMemoryCache cache;

        /// <summary>
        /// Initializes a new instance of the <see cref="TokenMiddleware"/> class.
        /// 构造
        /// </summary>
        /// <param name="next">下一个操作</param>
        /// <param name="memoryCache">访问上下文</param>
        public TokenMiddleware(RequestDelegate next, IMemoryCache memoryCache)
        {
            this.next = next;
            this.cache = memoryCache;
        }

        /// <summary>
        /// 主要调用
        /// </summary>
        /// <param name="context">上下文</param>
        /// <returns>任务</returns>
        public Task Invoke(HttpContext context)
        {
            var path = context.Request.Path;
            var pathBase = context.Request.PathBase;
            var guid = Guid.NewGuid().ToString("N");
            context.Items["rid"] = guid;

            logger.LogInformation(new LogInfo()
            {
                ApiName = path,
                Desc = $"开始访问url:{path}",
                Parameters = context.Request.QueryString.ToString(),
                TransactionId = guid,
                ComputerIp = context.Request.Host.ToString(),
                ComputerName = Environment.MachineName,

            }.ToString());

            try
            {
                if (!path.Value.ToLower().StartsWith("/file_server/"))
                {
                    return this.next(context);
                }
                string token = string.Empty;
                if (path.Value.ToLower().Equals("/file_server/file/download") || path.Value.ToLower().StartsWith("/file_server/file/download?"))
                {
                    if (context.Request.Headers.ContainsKey("Token"))
                    {
                        token = context.Request.Headers["Token"];
                    }
                    if (context.Request.Query.ContainsKey("Token"))
                    {
                        token = context.Request.Query["Token"];
                    }
                }
                else if (path.Value.ToLower().StartsWith("/file_server/file/getimage/"))
                {
                    return this.next(context);
                }
                else
                {
                    var tokenString = context.Request.Headers["Token"];
                    if (tokenString.Count != 1 || string.IsNullOrWhiteSpace(tokenString[0]))
                    {
                        var exResult = string.Empty.ToResult(guid, new StatusCode(403, "Header中没有token"));
                        return this.SendStringRespond(context, exResult);
                    }

                    token = tokenString[0];
                }

                //if (string.IsNullOrEmpty(token))
                //{
                //    logger.LogError(new LogInfo()
                //    {
                //        ApiName = path,
                //        Parameters = context.Request.QueryString.ToString(),
                //        Desc = "访问失败，token为空",
                //        Code = StatusCode.Unauthorized.code,
                //        CodeMsg = StatusCode.Unauthorized.msg,
                //        TransactionId = guid,
                //    }.ToString());
                //    var exResult = string.Empty.ToResult(guid, new StatusCode(403, "token为空"));
                //    return this.SendStringRespond(context, exResult);
                //}

                var user= CacheManager.Get<UserContext>(token);
                //if (user == null)
                //{
                //    logger.LogError(new LogInfo()
                //    {
                //        ApiName = path,
                //        Desc = "访问失败，token无效",
                //        Parameters = context.Request.QueryString.ToString(),
                //        Code = StatusCode.Unauthorized.code,
                //        CodeMsg = StatusCode.Unauthorized.msg,
                //        TransactionId = guid,
                //    }.ToString());
                //    var exResult = string.Empty.ToResult(guid, new StatusCode(403,"token无效"));
                //    return this.SendStringRespond(context, exResult);
                //}

                context.Items["user"] = user;
                context.Items["token"] = token;
                logger.LogInformation(new LogInfo()
                {
                    ApiName = path,
                    Desc = $"结束访问url:{path}",
                    Parameters = context.Request.QueryString.ToString(),                
                    TransactionId = guid,
                }.ToString());
                return this.next(context);
            }
            catch (BusinessException e)
            {
                logger.LogError(new LogInfo()
                    {
                        ApiName = path,
                        Desc = $"结束访问url:{path}",
                        Parameters = context.Request.QueryString.ToString(),
                        Code = e.Code.code,
                        CodeMsg = e.Code.msg,
                        TransactionId = guid,
                    }.ToString(), e);
                var exResult = string.Empty.ToResult(guid, e.Code);
                return this.SendStringRespond(context, exResult);
            }
            catch (Exception e)
            {
                logger.LogError(new LogInfo()
                {
                    ApiName = path,
                    Desc = $"结束访问url:{path}",
                    Parameters = context.Request.QueryString.ToString(),
                    Code = e.InnerException?.HResult ?? 500,
                    CodeMsg = e.Message,
                    TransactionId = guid,
                }.ToString(), e);
                var exResult = string.Empty.ToResult(guid, new StatusCode(e.InnerException?.HResult??500, e.Message));
                return this.SendStringRespond(context, exResult);
            }
        }

        private Task SendStringRespond(HttpContext context, object content)
        {
            context.Response.StatusCode = 200;
            context.Response.ContentType += "application/json;charset=utf-8;";

            Task task = new Task(() =>
            {
                using (var strStream = new StreamWriter(context.Response.Body))
                {
                    var exString = JsonConvert.SerializeObject(content);
                    strStream.WriteAsync(exString);
                    strStream.FlushAsync();
                }
            });
            task.Start();
            return task;
        }
    }
}