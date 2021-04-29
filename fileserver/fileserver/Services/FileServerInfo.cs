// <copyright file="FileServerInfo.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace FileServer.Services
{
    /// <summary>
    /// 文件服务器登录信息
    /// 支持oss的改造，sftp支持
    /// </summary>
    public class FileServerInfo
    {
        /// <summary>
        /// Gets or sets 服务类型
        /// </summary>
        public FileServerType ServerType { get; set; }

        /// <summary>
        /// Gets or sets ip
        /// </summary>
        public string ServerIp { get; set; }
        /// <summary>
        /// Gets or sets port
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// Gets or sets 用户
        /// </summary>
        public string UserAccount { get; set; }

        /// <summary>
        /// Gets or sets 密码
        /// </summary>
        public string Password { get; set; }


        /// <summary>
        /// 根目录,保存和提取文件时会自动增加该父目录，
        /// 对外输出路径时 隐藏该目录
        /// </summary>
        public string RootPath { get; set; }

        public string FtpMode { get; set; }

        /// <summary>
        /// 是否sftp特有的 使用私钥
        /// </summary>
        public bool IsPrivateKeyValid { get; set; }
        /// <summary>
        /// 当IsPrivateKeyValid=true 需要提供秘钥文件路径
        /// </summary>
        public string PrivateKeyFile { get; set; }

        /// <summary>
        /// Gets or sets Key
        /// </summary>
        public string OtherKey { get; set; }

        /// <summary>
        /// 当前client的唯一标识，标记一个连接
        /// </summary>
        public string Key
        {
            get
            {
                return string.Format("{0}_{1}_{2}_{3}", ServerIp, Port, UserAccount, ServerType.ToString());
            }
        }

        public override string ToString()
        {
            if (ServerType == FileServerType.Sftp)
            {
                if (!IsPrivateKeyValid)
                {
                    return $"{ServerType}://{ServerIp}:{Port}/RootPath {UserAccount}@{Password}";
                }
                else
                {
                    return $"{ServerType}://{ServerIp}:{Port}/RootPath {UserAccount}@{PrivateKeyFile}";
                }
            }
            else
            {
                return $"{ServerType}://{ServerIp}:{Port}/RootPath {UserAccount}@{Password}";
            }
        }
    }
}
