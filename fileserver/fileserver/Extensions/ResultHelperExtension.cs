// <copyright file="ResultHelperExtension.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SuperGMS.Log;
using SuperGMS.Protocol.RpcProtocol;
using System;

namespace FileServer.Extensions
{
    /// <summary>
    /// 扩展类
    /// </summary>
    public static class ResultHelperExtension
    {
        private readonly static ILogger logger = LogFactory.CreateLogger(typeof(ResultHelperExtension).FullName);
        /// <summary>
        /// 生成result
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="theClass">实例</param>
        /// <param name="code">code</param>
        /// <returns>result类型</returns>
        private static Result<T> ToResult<T>(this T theClass, StatusCode code = null)
        {
            return theClass.ToResult(Guid.NewGuid().ToString("N"), code);
        }

        /// <summary>
        /// 指定返回的 rid
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="theClass"></param>
        /// <param name="rid"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        public static Result<T> ToResult<T>(this T theClass, string rid, StatusCode code = null)
        {
            var rst = new Result<T>();
            if (code == null)
            {
                code = StatusCode.OK;
            }

            rst.c = code.code;
            rst.msg = code.msg;
            rst.rid = rid;
            rst.icp = false;
            rst.v = theClass;
            logger.LogInformation(new LogInfo()
            {
                Code = code.code,
                CodeMsg = code.msg,
                TransactionId = rid,
                Desc = "访问返回结果:" + JsonConvert.SerializeObject(theClass),
            }.ToString());
            return rst;
        }

    }
}
