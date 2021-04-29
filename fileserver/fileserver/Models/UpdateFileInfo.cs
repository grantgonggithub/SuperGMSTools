// <copyright file="UpdateFileInfo.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
using System;

namespace FileServer.Models
{
    /// <summary>
    /// 下载文件、删除文件、分享文件dto
    /// </summary>
    public class UpdateFileInfo
    {
        /// <summary>
        /// Gets or sets guid
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets 用户id
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// Gets or sets 最后修改日期
        /// </summary>
        public DateTime UpdateDate { get; set; }
    }
}
