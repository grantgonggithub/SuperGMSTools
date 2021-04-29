// <copyright file="FtpFileOperate.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
using FluentFTP;
using Microsoft.Extensions.Logging;
using SuperGMS.ExceptionEx;
using SuperGMS.Log;
using System;
using System.Collections;
using System.IO;
using System.Net;

namespace FileServer.Services
{
    /// <summary>
    /// ftp实现
    /// </summary>
    public class FtpFileOperate : IFileOperate
    {
        /// <summary>
        /// 最大上传文件 支持 100M
        /// </summary>
        private readonly int MaxFileSize = FileServerTimeout.MaxFileSize;
        private readonly static ILogger logger = LogFactory.CreateLogger<FtpFileOperate>();
        /// <summary>
        /// ftp客户端
        /// </summary>
        private FtpClient client; 

        FileServerInfo _info;
        /// <summary>
        /// Gets or sets 日志记录
        /// </summary>

        /// <inheritdoc/>
        public bool Connected
        {
            get{
                try
                {
                    if (client == null) return false;
                    FtpReply r = client.Execute("NOOP");
                    return r.Success;
                }
                catch
                {
                    return false;
                }
            }
        }     

        /// <inheritdoc/>
        public void SetServerInfo(FileServerInfo info)
        {
            _info = info;
            this.client = new FtpClient();
            if (string.IsNullOrEmpty(info.OtherKey) || info.OtherKey.ToLower() == "pasv")
            {
                this.client.DataConnectionType = FtpDataConnectionType.PASV;
            }

            // 初始化超时时间 10s
            this.client.SocketPollInterval = 10000;
            this.client.ConnectTimeout = 10000;
            this.client.ReadTimeout = 10000;
            this.client.DataConnectionConnectTimeout = 10000;
            this.client.DataConnectionReadTimeout = 10000;
        }

        /// <inheritdoc/>
        public bool Connect()
        {
            if (this.client == null)
            {
                return false;
            }

            var i = 0;
            try
            {
                
                this.client.Host = _info.ServerIp;
                this.client.Port = _info.Port;
                this.client.Credentials = new NetworkCredential(_info.UserAccount, _info.Password);
                this.client.Connect();
                return true;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "ftp连接失败");
                return false;
            }
              
            
        }

        /// <inheritdoc/>
        public void Disconnect()
        {
            try
            {
                this.client?.Disconnect();
                this.client?.Dispose();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "ftp关闭连接失败");
            }
            finally
            {
                this.client?.Dispose();
            }
        }

        /// <inheritdoc/>
        public bool PutStream(Stream localFile, string remotePath)
        {
            int nSize = (int)localFile.Length;
            if (nSize > this.MaxFileSize)
            {
                logger.LogInformation($"{_info.ServerIp}:{_info.Port} Error Data Length > {this.MaxFileSize} byte");
                throw new BusinessException($"Error Data Length > {this.MaxFileSize} byte"); // 客户端发送的数据不能超过500M
            }

            if (!this.Connected)
            {
                logger.LogInformation($"ftp {_info.ServerIp}:{_info.Port} client aleady closed.");
                throw new BusinessException($"ftp {_info.ServerIp}:{_info.Port} client aleady closed.");
            }

            if (!localFile.CanRead)
            {
                logger.LogInformation($"ftp {_info.ServerIp}:{_info.Port} putstram param stream cant read.");
                throw new BusinessException($"ftp {_info.ServerIp}:{_info.Port} putstram param stream cant read.");
            }

            if (this.client.FileExists(remotePath))
            {
                logger.LogInformation($"ftp {_info.ServerIp}:{_info.Port} delete file {remotePath} when upload.");
                this.Delete(remotePath);
            }

            // 限制速度为 10Mb/s
            this.client.UploadRateLimit = 102400;
            return this.client.Upload(localFile, remotePath, FtpExists.Overwrite, true);

            /*
            using (Stream s = this.client.OpenWrite(remotePath, FtpDataType.Binary))
            {
                try
                {
                    localFile.CopyTo(s);
                }
                finally
                {
                    s.Close();
                    this.client.GetReply();
                }
            }*/
        }

        /// <inheritdoc/>
        public bool Delete(string remoteFile)
        {
            try
            {
                this.client?.DeleteFile(remoteFile);
                return true;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "ftp关闭连接失败");
                return false;
            }
        }


        /// <inheritdoc/>
        public void Get(string fromFilePath, Stream streamOut)
        {
            if (!this.Connected)
            {
                throw new BusinessException($"ftp {_info.ServerIp}:{_info.Port} client aleady closed.");
            }

            if (!this.client.FileExists(fromFilePath))
            {
                throw new BusinessException($"ftp {_info.ServerIp}:{_info.Port} file {fromFilePath} dont exists.");
            }

            // 限制下载 10Mb/s
            this.client.DownloadRateLimit = 102400;
            this.client.Download(streamOut, fromFilePath);
        }

        public void Dispose()
        {
            if (this.Connected)
            {
                var cl = this as IFileOperate;
                FileServerFactory.DisposeClient(cl, this._info);
            }
            else
            {
                this.Disconnect();
            }
        }

        public string CombineRootPath(string remoteFile)
        {
            return FileServerTools.CombineFtpPath(_info.RootPath, remoteFile);
        }
    }
}
