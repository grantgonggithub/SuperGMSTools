using SuperGMS.Config;
using System;

namespace FileServer.Services
{
    /// <summary>
    /// 配置文件解析，兼容老的配置
    /// ServerSetting.GetConstValue("ftpInfo")?.Value
    /// 老格式：192.168.100.121:21;ftpuser;ftppassword;
    /// 新格式：
    /// 
    /// </summary>
    public class FileServerInfoConfigParser
    {
        private string _oldFtpInfo = string.Empty;
        public  FileServerInfo Info { get; private set; }

        /// <summary>
        /// 配置格式错误，只能抛出异常
        /// </summary>
        public  void InitConfig()
        {
            Info = new FileServerInfo();
            _oldFtpInfo = ServerSetting.GetConstValue("ftpInfo")?.Value;
            if(string.IsNullOrEmpty(_oldFtpInfo))
            {
                ReadNewConfig();
                if (string.IsNullOrEmpty(Info.ServerIp))
                {
                    Info.ServerIp = "LocalHost";
                    Info.ServerType = FileServerType.LocalDir;
                    Info.RootPath = "FileDir";
                }
            }
            else
            { //兼容老的格式
                string[] fInfo = _oldFtpInfo.Split(";");
                if (fInfo == null || fInfo.Length < 3)
                {
                    throw new Exception($"本地ftp配置格式错误{_oldFtpInfo},如：192.168.100.121:21;ftpuser;ftppassword");
                }
                var ipAndPort = fInfo[0];
                Info.UserAccount = fInfo[1];
                Info.Password = fInfo[2];
         
                var arr = ipAndPort?.Split(':'); 
                if (arr != null && arr.Length > 0)
                {
                    var port = 21;
                    //获取配置的端口
                    if (arr.Length > 1) {
                        int.TryParse(arr[1], out port);
                        if (port <= 0)
                        {
                            port = 21;
                        }
                    }

                    Info.ServerIp = arr[0];
                    Info.Port = port;
                    Info.ServerType = FileServerType.FtpServer;
                    Info.FtpMode = "PASV";
                }
                else
                {
                    throw new Exception($"未配置本地ftp的地址");
                }
            }
        }

        private void ReadNewConfig()
        {
            var nPort = 0;
            var st = FileServerType.Unknow;
            Enum.TryParse<FileServerType>(ServerSetting.GetConstValue("ServerType")?.Value,true,out st);
            Info.ServerType = st;
            Info.ServerIp = ServerSetting.GetConstValue("ServerIp")?.Value;

            int.TryParse(ServerSetting.GetConstValue("Port")?.Value, out nPort);
            Info.Port = nPort;

            Info.UserAccount = ServerSetting.GetConstValue("UserAccount")?.Value;
            Info.Password = ServerSetting.GetConstValue("Password")?.Value;
            Info.RootPath = ServerSetting.GetConstValue("RootPath")?.Value;

            Info.FtpMode = ServerSetting.GetConstValue("FtpMode")?.Value;
            Info.PrivateKeyFile = ServerSetting.GetConstValue("PrivateKeyFile")?.Value;
            Info.OtherKey = ServerSetting.GetConstValue("OtherKey")?.Value;
           
            //默认潜规则
            if(Info.Port <= 0)
            {
                switch(st)
                {
                    case FileServerType.FtpServer:
                        Info.Port = 21;
                        break;
                    case FileServerType.Sftp:
                        Info.Port = 22;
                        break;
                    default:
                        break;
                }
            }

            Info.IsPrivateKeyValid = !string.IsNullOrEmpty(Info.PrivateKeyFile);
            Info.FtpMode = string.IsNullOrEmpty(Info.FtpMode) ? "PASV" : Info.FtpMode;            ;
        }

       
    }
}
