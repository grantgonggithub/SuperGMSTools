//-----------------------------------------------------------------------
// <copyright file="FileOperate.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using System;

namespace  FileServer.Models
{
    public partial class FileOperate
    {
        /// <summary>
        /// Gets or sets 操作标识
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Gets or sets 文件标识
        /// </summary>
        public string FileId { get; set; }

        /// <summary>
        /// Gets or sets 操作动作
        /// 0-建立
        /// 1-下载
        /// 2-删除
        /// 3-分享
        /// </summary>
        public byte Action { get; set; }

        /// <summary>
        /// Gets or sets 操作人id
        /// </summary>
        public string Operator { get; set; }

        /// <summary>
        /// Gets or sets 操作时间
        /// </summary>
        public DateTime OperateDate { get; set; }
    }
}