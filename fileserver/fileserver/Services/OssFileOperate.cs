using Aliyun.OSS;
using Aliyun.OSS.Common;
using System;
using System.Collections;
using System.IO;

namespace FileServer.Services
{
    public class OssFileOperate : IFileOperate
    {
        private FileServerInfo _info;
        private IOss ossClient;
        public bool Connected => true;
        public void SetServerInfo(FileServerInfo info)
        {
            _info = info;
            _info.RootPath = info.RootPath?.ToLower();
        }

        public bool Connect()
        {
            ossClient = new OssClient(_info.ServerIp, _info.UserAccount, _info.Password);
            return ossClient != null;
        }

        public void Disconnect()
        {
            ossClient = null;
        }

        public bool PutStream(Stream localFile, string remotePath)
        {
            try
            {
                using (var memory = new MemoryStream())
                {
                    localFile.Seek(0, SeekOrigin.Begin);
                    //此处会释放原来的流，因此不使用原来的流
                    localFile.CopyTo(memory);
                    memory.Seek(0, SeekOrigin.Begin);
                    localFile.Seek(0, SeekOrigin.Begin);
                    PutObjectResult r = ossClient.PutObject(_info.RootPath, remotePath?.ToLower(), memory);
                    return r.HttpStatusCode == System.Net.HttpStatusCode.OK;
                }
            }
            catch (Exception ex)
            {
                var e = new Exception("AliyunOssClient.UpLoad(Stream fileStream, string remoteName, string remoteFolder).Error", ex);
                throw ex;
            }
        }

        public void Get(string fromFilePath, Stream streamOut)
        {
            try
            {
                if (string.IsNullOrEmpty(fromFilePath)) return;
                fromFilePath = fromFilePath.ToLower();
                string process = string.Empty;
                if (fromFilePath.IndexOf('?') >= 0)
                {
                    var idx = fromFilePath.IndexOf('?');
                    process = fromFilePath.Substring(idx + 1);
                    fromFilePath = fromFilePath.Substring(0, idx);
                }
                if (!this.FileExists(fromFilePath)) throw new Exception(string.Format("文件{0}不存在", fromFilePath));
                if (string.IsNullOrEmpty(process))
                {
                    var rtn = ossClient.GetObject(new GetObjectRequest(_info.RootPath, fromFilePath), streamOut);
                }
                else
                {
                    var rtn = ossClient.GetObject(new GetObjectRequest(_info.RootPath, fromFilePath, process), streamOut);

                }

                //return rtn.ContentLength;
            }
            catch (OssException ex)
            {
                var e = new Exception($"AliyunOssClient.DownLoad({fromFilePath},).Error.OssException", ex);
                throw ex;
            }
            catch (Exception ex)
            {
                var e = new Exception($"AliyunOssClient.DownLoad({fromFilePath}, ).Error", ex);
                throw ex;
            }

        }

        public bool Delete(string remoteFile)
        {
            try
            {
                remoteFile = remoteFile?.ToLower();
                if (!this.FileExists(remoteFile)) throw new Exception(string.Format("文件{0}不存在", remoteFile));
                ossClient.DeleteObject(_info.RootPath, remoteFile);
                return true;
            }
            catch (Exception ex)
            {
                var e = new Exception("AliyunOssClient.CreateFolder.Delete", ex);
                throw ex;
            }
        }

        public bool FileExists(string filePath)
        {
            filePath = filePath?.ToLower();
            return ossClient.DoesObjectExist(_info.RootPath, filePath);
        }

        public void Dispose()
        {
            //Disconnect();            
            var cl = this as IFileOperate;
            FileServerFactory.DisposeClient(cl, this._info);
        }

        public string CombineRootPath(string remoteFile)
        {
            return FileServerTools.CombineAliyunOssPath("", remoteFile);
        }
    }
}
