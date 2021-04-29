// <copyright file="InsertFileInfo.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace  FileServer.Models
{
    /// <summary>
    /// 插入文件模型
    /// </summary>
    public class InsertFileInfo : DbFileInfo
    {
        /// <summary>
        /// Gets or sets 用户id
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// Gets or sets 文件操作
        /// </summary>
        public int Action { get; set; }
    }
}
