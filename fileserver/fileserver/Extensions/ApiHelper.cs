// <copyright file="ApiHelper.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SuperGMS.Cache;
using SuperGMS.Log;
using SuperGMS.Protocol.RpcProtocol;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace FileServer.Services
{
    /// <summary>
    /// 初始化其他代理服务
    /// </summary>
    internal class ApiHelper
    {
        private IMemoryCache cache;

        /// <summary>
        /// Initializes a new instance of the <see cref="ApiHelper"/> class.
        /// 构造
        /// </summary>
        /// <param name="memoryCache">cache</param>
        public ApiHelper(IMemoryCache memoryCache)
        {
            this.cache = memoryCache;
            
        }

        /// <summary>
        /// 日志记录
        /// </summary>
        private readonly static ILogger logger = LogFactory.CreateLogger<ApiHelper>();
        /// <summary>
        /// 发送Post api请求其他微服务，需要设定代理类地址
        /// </summary>
        /// <typeparam name="TA">参数类型</typeparam>
        /// <typeparam name="T">返回类型</typeparam>
        /// <param name="args">参数</param>
        /// <param name="uri">相对路径 形如：Service/interface </param>
        /// <param name="token">token参数</param>
        /// <param name="actionCallback">成功回调</param>
        /// <returns>api返回值</returns>
        public static async Task<T> SendAsync<TA, T>(TA args, string uri, string token, Action<string> actionCallback = null)
            where T : class
        {
            using (var client = new HttpClient {BaseAddress = new Uri(Startup.WebConfig?.BaseUri) })
            {
                var apiUri = $"{Startup.WebConfig?.BaseUri}{uri}";
                var sendArgs = args == null
                ? "{}"
                : JsonConvert.SerializeObject(args, new JsonSerializerSettings()
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                });
                HttpContent ctx = new StringContent($"{{\"tk\": \"{token}\",\"v\":{sendArgs}}}", Encoding.UTF8, "application/json");
                var msg = await client.PostAsync(uri, ctx);
                var content = await msg.Content.ReadAsStringAsync();
                if (!string.IsNullOrEmpty(content))
                {
                    try
                    {
                        var c = JsonConvert.DeserializeObject<Result<T>>(content);
                        logger.LogInformation($"service[{apiUri}]:{c?.c},{c?.msg}");
                        actionCallback?.Invoke(content);
                        return c?.v;
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, $"service[{apiUri}]");
                    }
                }
                else
                {
                    logger.LogInformation($"service[{apiUri}]:没有内容");
                }
                return default(T);
            }      
            
        }

        /// <summary>
        /// 保存缓存
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="value">val</param>
        private void SetCache(string key, string value)
        {
            var rnd = new Random((int)DateTime.Now.Ticks);
            var tp = new MemoryCacheEntryOptions
            {
                SlidingExpiration = TimeSpan.FromSeconds(rnd.Next(30,60)), //1-2分钟内未命中过期
                AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(rnd.Next(60, 90))
            };
            this.cache?.Set(key, value, tp);
        }
    }
}
