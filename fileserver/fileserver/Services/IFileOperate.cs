// <copyright file="IFileOperate.cs" company="PlaceholderCompany">
// Copyright (c) kj. All rights reserved.
// </copyright>
using System;
using System.Collections;
using System.IO;

namespace FileServer.Services
{
    /// <summary>
    /// 文件操作 公共接口
    /// </summary>
    public interface IFileOperate : IDisposable
    {

        /// <summary>
        /// Gets a value indicating whether 获取连接状态
        /// </summary>
        bool Connected { get; }

        /// <summary>
        /// 设置登录信息
        /// </summary>
        /// <param name="info">登录信息</param>
        void SetServerInfo(FileServerInfo info);

        /// <summary>
        /// 连接服务器
        /// </summary>
        /// <returns>成功与否</returns>
        bool Connect();

        /// <summary>
        /// 断开服务器连接
        /// </summary>
        void Disconnect();

        /// <summary>
        /// 上传文件流
        /// </summary>
        /// <param name="localFile">文件流</param>
        /// <param name="remotePath">远程路径</param>
        /// <returns>结果</returns>
        bool PutStream(System.IO.Stream localFile, string remotePath);

        /// <summary>
        /// 得到指定的路径文件
        /// </summary>
        /// <param name="fromFilePath">指定路径</param>
        /// <param name="streamOut">输出流</param>
        void Get(string fromFilePath, Stream streamOut);

        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="remoteFile">路径</param>
        /// <returns>结果</returns>
        bool Delete(string remoteFile);


        /// <summary>
        /// 组合根目录，供外部调用，其他接口所传递的路径必须是全路径
        /// </summary>
        /// <param name="remoteFile"></param>
        /// <returns></returns>
        string CombineRootPath(string remoteFile);
    }
}
