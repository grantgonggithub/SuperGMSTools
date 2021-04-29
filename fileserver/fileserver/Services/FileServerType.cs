namespace FileServer.Services
{
    /// <summary>
    /// 文件服务器类型
    /// </summary>
    public enum FileServerType
    {
        /// <summary>
        /// 未知
        /// </summary>
        Unknow = 0,        

        /// <summary>
        /// ftp服务器
        /// </summary>
        FtpServer = 1,

        /// <summary>
        /// ftps
        /// </summary>
        /// 
        Ftps = 2,

        /// <summary>
        /// Sftp
        /// </summary>
        Sftp = 3,

        /// <summary>
        /// oss服务
        /// </summary>
        AliyunOss = 4,      

        /// <summary>
        /// 本地磁盘
        /// </summary>
        LocalDir = 5,

        /// <summary>
        /// fileServer
        /// </summary>
        FileServer = 6,
    }
}
