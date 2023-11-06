using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

using SuperGMS.ApiDoc;
using SuperGMS.ApiHelper;
using SuperGMS.Config;
using SuperGMS.Log;
using SuperGMS.Protocol.RpcProtocol;
using SuperGMS.Rpc;

namespace Quantum.ApiDoc.Helper
{
  public class InterfaceHelper
  {
    private HttpApiHelper _helper;
    public InterfaceHelper(HttpApiHelper helper)
    {
      this._helper = helper;
    }
    private readonly static ILogger logger = LogFactory.CreateLogger<InterfaceHelper>();

    public async Task<List<string>> GetAllServices()
    {
      var svrs = await _helper.GetApiContent<Nullables, List<string>>($"GetAllServices", Nullables.NullValue, false);
      svrs = svrs?.Where(x => x.ToLower() != "httpproxy").ToList();
      Program.Svrs = svrs;
      return svrs;
    }

    public async Task<ConcurrentDictionary<string, List<ClassInfo>>> GetAllServieInterfaces()
    {
      var svrs = Program.Svrs;
      //todo debug
      //svrs = new List<string> { "SCMOMSOrderCenterService" };
      // var res =  await _helper.GetApiContent<GetResourceArgs, GetResourceResult>("SCMGlobalToolsService/GetResource", new GetResourceArgs());
    

      if (svrs != null)
      {
        svrs.ForEach(svr =>
          {
            Task.Run(() => {
              try
              {
                //if (svr == "SXSupplyService")
                //  this._helper.Host = "http://192.168.7.223:20001/v2_api/";
                //else
                //  this._helper.Host = "http://192.168.7.207/v2_api/";
                var c = GetServiceInterfaces(svr).Result;
                Program.Dict.TryAdd(svr, c);
              }
              catch (Exception ex)
              {
                logger.LogError(ex, "");
              }
            });
          });
        
        return Program.Dict;
      }

      return null;
    }

    /// <summary>
    /// 获取单个服务的接口
    /// </summary>
    /// <param name="serverName">服务名</param>
    /// <returns></returns>
    public Task<List<ClassInfo>> GetServiceInterfaces(string serverName)
    {
       return GetApiContent($"{this._helper.Host}{serverName}/GetApiHelp", null);
    }



    /// <summary>
    /// 异步获取指定地址的api接口帮助
    /// </summary>
    /// <param name="uri">指定微服务地址</param>
    /// <param name="res">资源词典</param>
    /// <returns>api接口信息</returns>
    public async Task<List<ClassInfo>> GetApiContent(string uri,GetResourceResult res)
    {

      var infos = await _helper.GetApiContent<Nullables, List<ClassInfo>>(uri, Nullables.NullValue);
      //if (infos != null)
      //{

      //}
      //infos?.ForEach(x =>
      //{
      //  if (x.PropertyInfo.Count >= 2)
      //  {
      //    try
      //    {
      //      if (x.PropertyInfo[0]!= null)
      //      {
      //        x.PropertyInfo[0].LimitDesc = GetLimitJson(x.PropertyInfo[0].ApiClassInfo, res?.Dict);
      //        x.PropertyInfo[1].LimitDesc = GetLimitJson(x.PropertyInfo[1].ApiClassInfo, res?.Dict);
      //        x.PropertyInfo[0].ApiClassInfo = null;
      //        x.PropertyInfo[1].ApiClassInfo = null;
      //      }
      //    }
      //    catch (Exception ex)
      //    {
      //      logger.LogError(ex, $"{x.Name} 解析出错");
      //    }
      //  }
      //});

      return infos ?? new List<ClassInfo>();
    }

    private object _lockUdfModel = new object();
    private Dictionary<string, List<FieldDescInfo>> _dictCacheUdfModel = new Dictionary<string, List<FieldDescInfo>>();

  }

  #region 接口定义类
  public class GetResourceArgs
  {
    /// <summary>
    /// 语言
    /// </summary>
    public string Lang { get; set; }
  }

  public class GetResourceResult
  {
    /// <summary>
    /// 词典列表
    /// </summary>
    public Dictionary<string, string> Dict { get; set; }
  }

  public class GetEditUIConfigArgs
  {
    public string EditGuid { get; set; }
    public string SysId { get; set; }
    public string ModelName { get; set; }
  }

  public class GetEditUIConfigResult
  {
    public string FieldName { get; set; }
    public int SortId { get; set; }
    public string ControlType { get; set; }
    public string GroupName { get; set; }
    public int IsHidden { get; set; }
    public string DefaultValue { get; set; }
    public int ReadOnly { get; set; }
    public int EditReadOnly { get; set; }
    public int CanHidden { get; set; }
    public int IsRequired { get; set; }

    public string QuickSelectColumn { get; set; }
    public string QuickSelectComplate { get; set; }
    public string QuickSelectModel { get; set; }
    public int QuickSelectIsMatch { get; set; }
    public string QuickSelectPageApi { get; set; }
    public string QuickSelectTextApi { get; set; }
    public string CheckedValue { get; set; }
    public string ReferColumnName { get; set; }
    public int DropdownAllowEmpty { get; set; }
    public string ConvertData { get; set; }
    public string DropdownFormat { get; set; }
    public string ValidateRule { get; set; }
    public int MaxLength { get; set; }
    public decimal NumberMax { get; set; }
    public decimal NumberMin { get; set; }
    public int IsDigits { get; set; }
    public int DecimalDigits { get; set; }
    public string ProvinceColumnName { get; set; }
    public string CityColumnName { get; set; }
    public string CountyColumnName { get; set; }
    public string DateFormat { get; set; }
    public string DateDefaultTime { get; set; }
    public string CreatedBy { get; set; }
    public DateTime CreatedDate { get; set; }
    public string UpdatedBy { get; set; }
    public DateTime? UpdatedDate { get; set; }

  }
  #endregion
}
