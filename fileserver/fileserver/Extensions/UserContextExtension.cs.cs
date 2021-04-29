// <copyright file="UserContextExtension.cs.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using Microsoft.Extensions.Caching.Memory;
using FileServer.Models;
using FileServer.Services;
using SuperGMS.Extend.FileServer;
using SuperGMS.UserSession;
using System;

namespace FileServer.Extensions
{
    /// <summary>
    /// 扩展类
    /// </summary>
    public static class UserContextExtension
    {
        /// <summary>
        /// 根据类型生成路径，默认ftp路径
        /// </summary>
        /// <param name="context">用户信息</param>
        /// <param name="type">类型</param>
        /// <returns>路径</returns>
        public static string GetUserDir(this UserContext context, int type = 0)
        {
            var dt = DateTime.Now;
            return $"/{dt:yyyy}/{dt:MMdd}/";
        }


        private static FileServerInfo _info = null;
        private static FileServerInfo GetConfig()
        {
            if(_info == null)
            {
                var fip = new FileServerInfoConfigParser();
                fip.InitConfig();
                _info = fip.Info;
            }
            return _info;
        }

        /// <summary>
        /// 获取实际的文件服务器
        /// </summary>
        /// <param name="context">用户信息</param>
        /// <param name="cache">缓存</param>
        /// <returns>ftp</returns>
        public static IFileOperate GetFileServer(this UserContext context, IMemoryCache cache)
        {
            var helper = new ApiHelper(cache);
            var info = GetConfig();
            ClientInfo ftpInfo = null;

            //如果没有定义本地的配置，则使用远程配置
            if (info.ServerType == FileServerType.Unknow)
            {
                ftpInfo = new ClientInfo();

                info = ftpInfo.ToFileServerInfo();
            }            
           
            var fso = FileServerFactory.GetFileServer(info);
            return fso;
        }
    }
}
