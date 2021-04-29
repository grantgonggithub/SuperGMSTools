// <copyright file="WebConfig.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
using Microsoft.Extensions.Logging;
using SuperGMS.Config;
using SuperGMS.Log;
using System;

namespace FileServer.Models
{
    /// <summary>
    /// 配置路径参数
    /// </summary>
    public class WebConfig
    {
        public const string serverName = "FileServer";
        private readonly static ILogger logger = LogFactory.CreateLogger<WebConfig>();
        /// <summary>
        /// Gets or sets api基地址 形如 http://xxxx/api/
        /// </summary>
        public string BaseUri
        {
            get
            {
                var httpUri = ServerSetting.GetConstValue("HttpProxy")?.Value;
                if (string.IsNullOrEmpty(httpUri))
                {
                    logger.LogError("配置中的常量 [HttpProxy]没有配置! ");
                }

                return httpUri;
            }
        }

        /// <summary>
        /// 重试次数，默认5
        /// </summary>
        public int RetryNum
        {
            get
            {
                var strNum = ServerSetting.GetConstValue("FtpRetry")?.Value;
                if (string.IsNullOrEmpty(strNum))
                {
                    return 5;
                }

                int num = 0;
                int.TryParse(strNum, out num);
                return num;
            }
        }

        /// <summary>
        /// 重试间隔，默认1s
        /// </summary>
        public int RetrySleep
        {
            get
            {
                var strNum = ServerSetting.GetConstValue("RetrySleep")?.Value;
                var r = new Random(DateTime.Now.Millisecond);
                if (string.IsNullOrEmpty(strNum))
                {
                    return r.Next(700,1200);
                }

                int num = 0;
                int.TryParse(strNum, out num);
                return Math.Abs(r.Next(num-200, num+800));
            }
        }

        /// <summary>
        /// ftpMode 默认，pasv
        /// </summary>
        public string FtpMode
        {
            get
            {
                var strMode = ServerSetting.GetConstValue("FtpMode")?.Value;
                return strMode;
            }
        }

        /// <summary>
        /// 获取登录验证uri
        /// </summary>
        /// <returns>登录验证uri</returns>
        public static string GetLoginUri()
        {
            return "BaseService/GetUserContext";
        }

        /// <summary>
        /// 获取ftp信息uri
        /// </summary>
        /// <returns>uri</returns>
        public static string GetFtpUri()
        {
            return "SCMConfigrationService/GetFtpInfo";
        }

        /// <summary>
        /// 检查文件权限
        /// </summary>
        /// <returns>uri</returns>
        public static string GetCheckFileUri()
        {
            return "SCMBaseService/CheckFileRight";
        }
    }
}
