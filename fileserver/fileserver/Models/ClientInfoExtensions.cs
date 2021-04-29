using FileServer.Services;

using FileServer.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace  FileServer.Models
{
    public static class ClientInfoExtensions
    {
        /// <summary>
        /// 转换到 FileServerInfo 信息内
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public static FileServerInfo ToFileServerInfo(this ClientInfo info)
        {
            var fsi = new FileServerInfo
            {
                ServerType = (FileServerType)info.ClientType,               
                ServerIp = info.Ip,
                Port = info.Port,
                UserAccount = info.UserName,
                Password = info.PassWord,
                RootPath = info.RootFoler,

                //ftp专用
                FtpMode = info.ConnectionType.ToString(),

                //sftp
                IsPrivateKeyValid =info.IsPrivateKeyValid,
                PrivateKeyFile = info.PrivateKeyFile,
                //OtherKey =
            };

            if(fsi.ServerType == FileServerType.Unknow)
            {
                //兼容老系统错误的接口返回值~~~~~
                fsi.ServerType = FileServerType.FtpServer;
            }
            return fsi;
        }
    }
}
