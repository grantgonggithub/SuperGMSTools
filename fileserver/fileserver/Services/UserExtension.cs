// <copyright file="UserExtension.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
using SuperGMS.DB.EFEx;
using SuperGMS.DB.EFEx.GrantDbContext;
using SuperGMS.Protocol.RpcProtocol;
using SuperGMS.Rpc.Server;
using SuperGMS.UserSession;

namespace FileServer.Services
{
    /// <summary>
    /// 根据用户信息获取租户数据库
    /// </summary>
    public static class UserExtension
    {
        public static IDapperDbContext GetDapperContext(this UserContext context,string dbContextName)
        {
           return SuperGMSDBContext.GetDapperContext(
               new RpcContext(null, new Args<object>()
           {
               tk = context.Token,
               ct = context.ClientType,
               cv = context.ClientVersion,
           }), dbContextName);
        }
    }
}
