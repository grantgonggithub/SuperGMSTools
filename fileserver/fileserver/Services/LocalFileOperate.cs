using FileServer.Services;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace fileserver.Services
{
    public class LocalFileOperate : IFileOperate
    {
        private FileServerInfo _info;
        public void SetServerInfo(FileServerInfo info)
        {
            _info = info;
            _info.RootPath = info.RootPath?.ToLower();
        }

        public bool Connected => true;

        public string CombineRootPath(string remoteFile)
        {
            return FileServerTools.CombineLocalPath(_info.RootPath, remoteFile);
        }

        public bool Connect()
        {
            return true;
        }

        public bool Delete(string remoteFile)
        {
            var dir = getDir(remoteFile);
            var filePath = Path.Combine(dir.Item1, dir.Item2);
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
            return true;
        }

        public bool DirExist(string dirName)
        {
           return Directory.Exists(dirName);
        }

        public void Disconnect()
        {
            return;
        }

        public void Dispose()
        {
            return;
        }

        public void Get(string fromFilePath, Stream streamOut)
        {
            var dirItem = getDir(fromFilePath);
            var filePath = Path.Combine(dirItem.Item1, dirItem.Item2);
            using var fileStream= new FileStream(filePath, FileMode.Open, FileAccess.Read);
            fileStream.CopyTo(streamOut);
        }

        public bool PutStream(Stream localFile, string remotePath)
        {
            var dir = getDir(remotePath);
            if (!Directory.Exists(dir.Item1)) Directory.CreateDirectory(dir.Item1);
            var filePath = Path.Combine(dir.Item1, dir.Item2);
            byte[] bytes = new byte[localFile.Length];
            localFile.Read(bytes, 0, bytes.Length);
            using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
            {
                using (BinaryWriter binaryWriter = new BinaryWriter(fileStream))
                {
                    binaryWriter.Write(bytes);
                    return true;
                }
            }
        }

        private (string,string) getDir(string path)
        {
            char split = Path.DirectorySeparatorChar;
            var pos = path.LastIndexOf(split.ToString());
            var dir = path.Substring(0, pos);
            var fileName = path.Substring(pos + 1, path.Length - (pos + 1));
            if (!Path.IsPathRooted(dir))
            {
                dir = Path.Combine(AppContext.BaseDirectory, dir);
            }
            return (dir, fileName);
        }

    }
}
