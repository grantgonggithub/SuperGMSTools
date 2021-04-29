//-----------------------------------------------------------------------
// <copyright file="FileInfo.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using System;

namespace  FileServer.Models
{
    public partial class DbFileInfo
    {
        /// <summary>
        /// Gets or sets guid
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets 存储租户id
        /// </summary>
        public string Ttid { get; set; }

        /// <summary>
        /// Gets or sets 存储路径
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// Gets or sets 原始名称
        /// </summary>
        public string OriginalName { get; set; }

        /// <summary>
        /// Gets or sets mD5编码
        /// </summary>
        public string Md5 { get; set; }

        /// <summary>
        /// Gets or sets sHA编码
        /// </summary>
        public string Sha256 { get; set; }

        /// <summary>
        /// Gets or sets mIME格式
        /// </summary>
        public string Mime { get; set; }

        /// <summary>
        /// Gets or sets 文件大小
        /// </summary>
        public long Size { get; set; }

        /// <summary>
        /// Gets or sets 如果是图片，则缩略图路径
        /// </summary>
        public string ThumbnailPath { get; set; }

        /// <summary>
        /// Gets or sets 下载次数
        /// </summary>
        public long DownloadNum { get; set; }

        /// <summary>
        /// Gets or sets 是否已删除
        /// </summary>
        public byte IsDelete { get; set; }

        /// <summary>
        /// Gets or sets 是否临时文件
        /// </summary>
        public byte IsTemp { get; set; }

        /// <summary>
        /// Gets or sets 是否图片
        /// </summary>
        public byte IsImage { get; set; }

        /// <summary>
        /// Gets or sets 最后修改日期
        /// </summary>
        public DateTime UpdateDate { get; set; }
    }
}