using fileserver.Services;
using SuperGMS.Tools;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace FileServer.Services
{
    /// <summary>
    /// 文件服务器工厂
    /// </summary>
    public class FileServerFactory
    {
        private static readonly Dictionary<string, List<ComboxClass<DateTime, IFileOperate>>> clients = new Dictionary<string, List<ComboxClass<DateTime, IFileOperate>>>();
        private static readonly object root = new object();
        private static readonly Timer timer = new Timer(new TimerCallback(o => { checkTimeOut(); }), null, 0, 10 * 1000);

        /// <summary>
        /// 工厂方法
        /// </summary>
        /// <param name="info">信息</param>
        /// <returns>操作接口</returns>
        public static IFileOperate GetFileServer(FileServerInfo info)
        {            
            return createInstance(info); 
        }

        private static IFileOperate createInstance(FileServerInfo client)
        {
            if (client == null) throw new Exception("FileServerInfo is null");
            if (string.IsNullOrEmpty(client.ServerIp)) throw new Exception("FileServerInfo Ip is null or Empty");
            Trace.WriteLine($"开始连接：clientInfo={client.ToString()}");
            string key = client.Key;
            if (clients.ContainsKey(key))
            {
                lock (root)
                {
                    if (clients.ContainsKey(key))
                    {
                        var cl = clients[key];
                        if (cl.Count > 0)
                        {
                            var c = cl[0].V2;
                            cl.Remove(cl[0]);
                            if (c.Connected) // 连接状态的才会返回，否则重建
                                return c;
                        }
                    }
                }
            }

            IFileOperate fo = null;
            switch (client?.ServerType)
            {
                case FileServerType.FtpServer:
                    fo = new FtpFileOperate();
                    fo.SetServerInfo(client);
                    break;
                case FileServerType.Sftp:
                    fo = new SFtpFileOperate();
                    fo.SetServerInfo(client);
                    break;
                case FileServerType.AliyunOss:
                    fo = new OssFileOperate();
                    fo.SetServerInfo(client);
                    break;
                case FileServerType.LocalDir:
                    fo = new LocalFileOperate();
                    fo.SetServerInfo(client);
                    break;
                default:
                    throw new NotImplementedException(client?.ToString());
                    //break;
            }
            fo.Connect();
            return fo;
        }

        /// <summary>
        /// 是否会连接池，这里能放回来，一定是可用的连接
        /// </summary>
        /// <param name="client">连接对象</param>
        /// <param name="clientInfo">连接信息</param>
        internal static void DisposeClient(IFileOperate client, FileServerInfo clientInfo)
        {
            string key = clientInfo.Key;
            lock (root)
            {
                if (!clients.ContainsKey(key))
                {
                    clients[key] = new List<ComboxClass<DateTime, IFileOperate>>();
                }
                clients[key].Add(new ComboxClass<DateTime, IFileOperate> { V1 = DateTime.Now, V2 = client });
            }
        }

        /// <summary>
        /// 检查超时或者断开连接的 默认10分钟
        /// </summary>
        private static void checkTimeOut()
        {
            lock (root)
            {
                if (clients == null || clients.Count < 1) return;
                string[] keys = clients.Keys.ToArray();
                foreach (string k in keys)
                {
                    Trace.WriteLine($"池子{k}: {clients[k].Count}");
                    try
                    {
                        clients[k].RemoveAll(x => DateTime.Now.Subtract(x.V1).TotalMilliseconds > 10 * 60 * 1000 || !x.V2.Connected);
                    }
                    catch (Exception ex)
                    {
                        Trace.WriteLine(ex.Message);
                    }
                }
            }
        }
    }
}
