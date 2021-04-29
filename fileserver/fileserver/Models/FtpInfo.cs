// <copyright file="FtpInfo.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace  FileServer.Models
{
    /// <summary>
    /// ftp信息
    /// </summary>
    public class FtpInfo : ClientInfo
    {
        /// <summary>
        /// Gets or sets 别名
        /// </summary>
        public string Alias { get; set; }

        /// <summary>
        /// Gets or sets 密码
        /// </summary>
        public string FtpPassword { get; set; }

        /// <summary>
        /// Gets or sets ip
        /// </summary>
        public string FtpServerIP { get; set; }

        /// <summary>
        /// Gets or sets 用户
        /// </summary>
        public string FtpUserID { get; set; }
    }


    public class ClientInfo
    {
        /// <summary>
        /// Ip或者Url 主要根据ClientType的类型来确定，如果是Ip就要提供Prot，如果是Url就不需要
        /// </summary>
        public string Ip { get; set; }

        /// <summary>
        /// Port
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 密码或者口令
        /// </summary>
        public string PassWord { get; set; }


        private bool _useConnectPool = false;

        /// <summary>
        /// 是否启用连接池
        /// </summary>
        public bool UseConnectPool
        {
            get { return _useConnectPool; }
            set { _useConnectPool = value; }
        }

        private string rootFolder;

        /// <summary>
        /// 工作跟目录，阿里云oss这个值是bucket
        /// </summary>
        public string RootFoler
        {
            get { return rootFolder; }
            set { rootFolder = value; }
        }

        /// <summary>
        /// 文件存储类型
        /// </summary>
        public FileClientType ClientType { get; set; }

        #region ftp特有

        private FtpDataConnectionType connectionType = FtpDataConnectionType.PASV;
        /// <summary>
        /// 数据连接建立方式，主动，被动
        /// </summary>
        public FtpDataConnectionType ConnectionType
        {
            get { return connectionType; }
            set { connectionType = value; }
        }

        private SslTslNone sslTslNone = SslTslNone.None;
        /// <summary>
        /// 分别对应ssl,tsl,默认是none
        /// </summary>
        public SslTslNone SslTslNone { get; set; }

        #endregion


        #region sftp 特有配置

        /// <summary>
        /// 是否sftp
        /// </summary>
        public bool IsPrivateKeyValid { get; set; }
        /// <summary>
        /// 当IsPrivateKeyValid=true 需要提供秘钥文件路径
        /// </summary>
        public string PrivateKeyFile { get; set; }

        #endregion


    }

    /// <summary>
    /// ClientType
    /// </summary>
    public enum FileClientType
    {
        /// <summary>
        /// 未知
        /// </summary>
        Unknow = 0,
        /// <summary>
        /// ftp
        /// </summary>
        /// 
        Ftp = 1,
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
        /// oss
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

    public enum FtpDataConnectionType
    {
        /// <summary>
        /// This type of data connection attempts to use the EPSV command
        /// and if the server does not support EPSV it falls back to the
        /// PASV command before giving up unless you are connected via IPv6
        /// in which case the PASV command is not supported.
        /// </summary>
        AutoPassive,
        /// <summary>
        /// Passive data connection. EPSV is a better
        /// option if it's supported. Passive connections
        /// connect to the IP address dictated by the server
        /// which may or may not be accessible by the client
        /// for example a server behind a NAT device may
        /// give an IP address on its local network that
        /// is inaccessible to the client. Please note that IPv6
        /// does not support this type data connection. If you
        /// ask for PASV and are connected via IPv6 EPSV will
        /// automatically be used in its place.
        /// </summary>
        PASV,
        /// <summary>
        /// Same as PASV except the host supplied by the server is ignored
        /// and the data connection is made to the same address that the control
        /// connection is connected to. This is useful in scenarios where the
        /// server supplies a private/non-routable network address in the
        /// PASV response. It's functionally identical to EPSV except some
        /// servers may not implement the EPSV command. Please note that IPv6
        /// does not support this type data connection. If you
        /// ask for PASV and are connected via IPv6 EPSV will
        /// automatically be used in its place.
        /// </summary>
        PASVEX,
        /// <summary>
        /// Extended passive data connection, recommended. Works
        /// the same as a PASV connection except the server
        /// does not dictate an IP address to connect to, instead
        /// the passive connection goes to the same address used
        /// in the control connection. This type of data connection
        /// supports IPv4 and IPv6.
        /// </summary>
        EPSV,
        /// <summary>
        /// This type of data connection attempts to use the EPRT command
        /// and if the server does not support EPRT it falls back to the
        /// PORT command before giving up unless you are connected via IPv6
        /// in which case the PORT command is not supported.
        /// </summary>
        AutoActive,
        /// <summary>
        /// Active data connection, not recommended unless
        /// you have a specific reason for using this type.
        /// Creates a listening socket on the client which
        /// requires firewall exceptions on the client system
        /// as well as client network when connecting to a
        /// server outside of the client's network. In addition
        /// the IP address of the interface used to connect to the
        /// server is the address the server is told to connect to
        /// which, if behind a NAT device, may be inaccessible to
        /// the server. This type of data connection is not supported
        /// by IPv6. If you specify PORT and are connected via IPv6
        /// EPRT will automatically be used instead.
        /// </summary>
        PORT,
        /// <summary>
        /// Extended active data connection, not recommended
        /// unless you have a specific reason for using this
        /// type. Creates a listening socket on the client
        /// which requires firewall exceptions on the client
        /// as well as client network when connecting to a
        /// server outside of the client's network. The server
        /// connects to the IP address it sees the client coming
        /// from. This type of data connection supports IPv4 and IPv6.
        /// </summary>
        EPRT,
    }

    public enum SslTslNone
    {
        /// <summary>
        /// 默认
        /// </summary>
        None = 0,

        /// <summary>
        /// Ssl
        /// </summary>
        Ssl = 1,

        /// <summary>
        /// Tsl
        /// </summary>
        Tsl = 2,

    }
}
