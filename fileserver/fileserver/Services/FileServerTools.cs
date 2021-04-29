using System;
using System.IO;
using System.Text;

namespace FileServer.Services
{
    public class FileServerTools
    {
        /// <summary>
        /// 将ftp目录和文件名合并为一个相对根目录的相对路径
        /// </summary>
        /// <param name="forlder">目录</param>
        /// <param name="fileName">文件名</param>
        /// <returns>返回相对路径</returns>
        public static string CombineFtpPath(string forlder, string fileName = "")
        {
            return CombineBasePath(forlder, fileName, FileServerType.FtpServer);
        }

        /// <summary>
        /// 阿里云的路径前面没有/
        /// </summary>
        /// <param name="forlder"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static string CombineAliyunOssPath(string forlder, string fileName = "")
        {
            return CombineBasePath(forlder, fileName, FileServerType.AliyunOss);
        }

        /// <summary>
        /// 本地目录拼接
        /// </summary>
        /// <param name="folder">磁盘目录</param>
        /// <param name="fileName">文件名</param>
        /// <returns></returns>
        public static string CombineLocalPath(string folder, string fileName = "")
        {
            return CombineBasePath(folder, fileName, FileServerType.LocalDir);
        }

        /// <summary>
        /// 按照文件服务器实际的情况进行路径拼接
        /// </summary>
        /// <param name="folder"></param>
        /// <param name="fileName"></param>
        /// <param name="fsType"></param>
        /// <returns></returns>
        public static string CombineBasePath(string folder, string fileName, FileServerType fsType)
        {
            char split = Path.DirectorySeparatorChar; // 按系统来获取文件分隔字符

            if (fsType != FileServerType.LocalDir) // 其他都是虚拟目录用 /
                split = '/';

            if (string.IsNullOrEmpty(folder) && string.IsNullOrEmpty(fileName))
                return string.Empty;
            if (string.IsNullOrEmpty(folder))
            {
                folder = fileName;
                fileName = string.Empty;
            }

            StringBuilder sb = new StringBuilder();
            if ((fsType == FileServerType.FtpServer || fsType == FileServerType.Sftp) &&
                !folder.StartsWith(split)) // ftp前面是相对路径，需要加 /
            {
                sb.AppendFormat("{0}{1}", split, folder);
            }
            else if ((fsType == FileServerType.AliyunOss) && folder.StartsWith(split)) //  阿里云不需要 加 /
            {
                sb.AppendFormat(folder.Substring(1));
            }
            else
            {
                sb.Append(folder);
            }

            if (!string.IsNullOrEmpty(fileName))
            {

                if (!folder.EndsWith(split))
                {
                    sb.Append(split);
                }
                sb.Append(fileName.StartsWith(split) ? fileName.Substring(1) : fileName);
            }
            else
            {
                if (folder.EndsWith(split))
                {
                    return sb.ToString(0, sb.Length - 1);
                }
            }

            //兼容 /\
            var newSplit = fsType == FileServerType.LocalDir ? split.ToString() : "/";
            return getPath(sb.ToString(), newSplit);
        }

        private static string getPath(string fullPath,string newSplit)
        {
            //都转成小写，因为oss的key区分大小写，ftp不区分，所以oss的全部搞成小写就不存在找不到的问题了
            string[] ps = fullPath.ToLower().Split("\\/".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            return string.Join(newSplit, ps);
        }


    }
}
