using SuperGMS.ExceptionEx;
using Renci.SshNet;
using Renci.SshNet.Common;
using System;
using System.Collections;
using System.IO;
using System.Text;

namespace FileServer.Services
{
    public class SFtpFileOperate : IFileOperate
    {
        static SFtpFileOperate()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        }
        /// <summary>
        /// 最大上传文件 支持 100M
        /// </summary>
        private readonly int MaxFileSize = FileServerTimeout.MaxFileSize;
        private FileServerInfo _info;
        private SftpClient sftpClient;
        public bool Connected => sftpClient.IsConnected;
        public void SetServerInfo(FileServerInfo info)
        {
            _info = info;
        }

        public bool Connect()
        {
            ConnectionInfo connectionInfo;
            if (_info.IsPrivateKeyValid)
            {
                connectionInfo = new PrivateKeyConnectionInfo(_info.ServerIp, _info.Port,
                    _info.UserAccount, new PrivateKeyFile(_info.PrivateKeyFile, _info.Password));
            }
            else
            {
                connectionInfo = new PasswordConnectionInfo(_info.ServerIp, _info.Port,
                    _info.UserAccount, _info.Password);
            }
            //connectionInfo.Encoding = Encoding.UTF8;
            connectionInfo.Encoding = Encoding.GetEncoding(936, new EncoderReplacementFallback(""), new DecoderReplacementFallback("_"));

            sftpClient = new SftpClient(connectionInfo);
            sftpClient.KeepAliveInterval = TimeSpan.FromMilliseconds(FileServerTimeout.SocketPollInterval);
            sftpClient.OperationTimeout = TimeSpan.FromMilliseconds(FileServerTimeout.SocketPollInterval);

            try
            {
                sftpClient.ErrorOccurred += SftpClient_ErrorOccurred;
                sftpClient.Connect();
                return true;
            }
            catch (SshAuthenticationException ex)
            {
                var e = new Exception("QtSftpClient.Connect,授权失败（authentication fails）", ex);
                IsError = true;
                throw ex;
            }
            catch (SshConnectionException ex)
            {
                var e = new Exception("QtSftpClient.Connect,连接超时（connection was terminated）", ex);
                IsError = true;
                throw ex;
            }
            catch (Exception ex)
            {
                var e = new Exception("QtSftpClient.Connect,连接异常", ex);
                IsError = true;
                throw ex;
            }
        }

        /// <summary>
        /// Error
        /// </summary>
        public bool IsError { get; set; }

        private void SftpClient_ErrorOccurred(object sender, ExceptionEventArgs e)
        {
            IsError = true;
        }

        protected bool KeepAlive()
        {
            sftpClient.OperationTimeout = sftpClient.KeepAliveInterval = TimeSpan.FromMilliseconds(FileServerTimeout.SocketPollInterval);
            return sftpClient.IsConnected;
        }
        /// <summary>
        /// 是否连接状态
        /// </summary>
        /// <returns></returns>
        public bool IsConnected()
        {
            return KeepAlive();
        }


        public void Disconnect()
        {
            try
            {
                sftpClient?.Disconnect();
                sftpClient?.Dispose();
            }
            catch (Exception ex)
            {
                var e = new Exception("QtSftpClient.Connect,Disconnect异常", ex);
                IsError = true;
            }
        }

        public string CombineRootPath(string remoteFile)
        {
            return FileServerTools.CombineFtpPath(_info.RootPath, remoteFile);
        }

        public bool PutStream(Stream localFile, string remotePath)
        {
            int nSize = (int)localFile.Length;
            if (nSize > this.MaxFileSize)
            {
                throw new BusinessException($"SFtp:Error Data Length > {this.MaxFileSize} byte"); // 客户端发送的数据不能超过500M
            }

            if (!this.Connected)
            {
                throw new BusinessException($"SFtp: {_info.ServerIp}:{_info.ServerIp} client aleady closed.");
            }

            if (!localFile.CanRead)
            {
                throw new BusinessException($"{remotePath} putstram param stream cantnot read.");
            }

            localFile.Seek(0, SeekOrigin.Begin);

            if (this.FileExists(remotePath))
            {
                this.Delete(remotePath);
            }
            else
            {
                //创建文件夹
                if (!string.IsNullOrEmpty(remotePath))
                {
                    var path = remotePath.Substring(0, remotePath.LastIndexOf('/'));
                    if (!this.DirExist(path))
                        this.Mkdir(path);
                }
            }

            sftpClient.UploadFile(localFile, remotePath, true);
            return true;
        }


        public void Get(string fromFilePath, Stream streamOut)
        {
            if (!FileExists(fromFilePath))
            {
                var e = new Exception($"Sftp Get:文件[{fromFilePath}]不存在.");
                throw e;
            }

            sftpClient.DownloadFile(fromFilePath, streamOut);
        }

        public bool Delete(string remoteFile)
        {
            try
            {
                if (this.FileExists(remoteFile))
                {
                    sftpClient.DeleteFile(remoteFile);
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                var e = new Exception("QtSftpClient.Delete.Error", ex);
                IsError = true;
                throw ex;
            }
        }

        public bool FileExists(string filePath)
        {
            return sftpClient.Exists(filePath);
        }

        private bool DirExist(string dirName)
        {
            return sftpClient.Exists(dirName);
        }

        private void Mkdir(string dirName)
        {
            //递归建目录
            if(string.IsNullOrEmpty(dirName))
            {
                return;
            }

            var arrPath = dirName.Split(new char[] { '/' });
            StringBuilder sb = new StringBuilder();
            for(var i=0;i<arrPath.Length;i++)
            {
                if (arrPath[i] == string.Empty && i == 0)
                    continue;

                sb.AppendFormat("/{0}", arrPath[i]);
                if(!DirExist(sb.ToString()))
                {
                    sftpClient.CreateDirectory(sb.ToString());
                }                
            }
            
        }

        /// <summary>
        /// 支持连接池处理
        /// </summary>
        public void Dispose()
        {
            RelaseDispose();
        }
        /// <summary>
        /// 放回连接池
        /// </summary>
        protected void RelaseDispose()
        {

            if (!IsError && KeepAlive()) // 本次使用没有错误， 并且连接正常才会放回连接池，否则扔掉
            {
                var cl = this as IFileOperate;
                FileServerFactory.DisposeClient(cl, this._info);
            }
            else
            {
                Disconnect();
            }
        }
    }
}
