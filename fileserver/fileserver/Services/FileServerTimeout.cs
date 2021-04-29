namespace FileServer.Services
{
    public static class FileServerTimeout
    {
        public static int SocketPollInterval = 5 * 1000;

        public static int ConnectTimeout = 5 * 1000;

        public static int ReadTimeout = 5 * 1000;

        public static int DataConnectionConnectTimeout = 5 * 1000;

        public static int DataConnectionReadTimeout = 5 * 1000;

        /// <summary>
        /// 最大上传文件 支持 100M
        /// </summary>
        public static readonly int MaxFileSize = 500 * 1024 * 1024;

    }
}
