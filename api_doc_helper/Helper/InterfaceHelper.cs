using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
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
      return svrs;
    }

    public async Task<Dictionary<string, List<ClassInfo>>> GetAllServieInterfaces()
    {
      var dict = new Dictionary<string, List<ClassInfo>>();
      var svrs = await GetAllServices();
      //todo debug
      //svrs = new List<string> { "SCMOMSOrderCenterService" };
      // var res =  await _helper.GetApiContent<GetResourceArgs, GetResourceResult>("SCMGlobalToolsService/GetResource", new GetResourceArgs());
    

      if (svrs != null)
      {
        svrs.ForEach(svr =>
          {
            try
            {
              var c = GetApiContent($"{this._helper.Host}{svr}/GetApiHelp", null).Result;
              dict.Add(svr, c);
            }
            catch (Exception ex)
            {
              logger.LogError(ex, "");
            }
          });
        
        return dict;
      }

      return null;
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
      if (infos != null)
      {

      }
      infos?.ForEach(x =>
      {
        if (x.PropertyInfo.Count >= 2)
        {
          try
          {
            if (x.PropertyInfo[0].ApiClassInfo != null)
            {
              //x.PropertyInfo[0].LimitDesc = GetLimitJson(x.PropertyInfo[0].ApiClassInfo, res?.Dict);
              //x.PropertyInfo[1].LimitDesc = GetLimitJson(x.PropertyInfo[1].ApiClassInfo, res?.Dict);
              x.PropertyInfo[0].ApiClassInfo = null;
              x.PropertyInfo[1].ApiClassInfo = null;
            }
          }
          catch (Exception ex)
          {
            logger.LogError(ex, $"{x.Name} 解析出错");
          }
        }
      });

      return infos ?? new List<ClassInfo>();
    }

    /// <summary>
    /// 获取描述类 json
    /// </summary>
    /// <param name="info"></param>
    /// <param name="uri"></param>
    /// <param name="dict"></param>
    /// <returns></returns>
    private string GetLimitJson(ApiClassInfo info, Dictionary<string, string> dict)
    {
      return info?.ToJson(GetUdfModelDefine, dict);
    }

    private object _lockUdfModel = new object();
    private Dictionary<string, List<FieldDescInfo>> _dictCacheUdfModel = new Dictionary<string, List<FieldDescInfo>>();

    /// <summary>
    /// 得到自定义模型信息
    /// </summary>
    /// <param name="info"></param>
    /// <returns></returns>
    private async Task<List<FieldDescInfo>> GetUdfModelDefine(ApiClassInfo info)
    {
      if (info?.UdfModel == null)
      {
        return null;
      }

      var key = $"{info.UdfModel.SysId}-{info.UdfModel.ModelName}";

      lock (_lockUdfModel)
      {
        if (_dictCacheUdfModel.ContainsKey(key))
        {
          return _dictCacheUdfModel[key];
        }
      }

      var ms = await _helper.GetApiContent< GetEditUIConfigArgs, List<GetEditUIConfigResult>>(
        "SCMUserDefineService/GetEditUIConfig",
        new GetEditUIConfigArgs
        {
            SysId = info.UdfModel.SysId,
            ModelName = info.UdfModel.ModelName,
        });

      var dict = ms?.Select(m => new FieldDescInfo
      {
        CanHidden = m.CanHidden,
        ControlType = m.ControlType,
        DateDefaultTime = m.DateDefaultTime,
        DateFormat = m.DateFormat,
        DecimalDigits = m.DecimalDigits,
        DefaultValue = m.DefaultValue,
        DicSource = null,
        EditReadOnly = m.EditReadOnly,
        FieldName = m.FieldName,
        GroupName = m.GroupName,
        IsDigits = m.IsDigits,
        IsHidden = m.IsHidden,
        IsRequired = m.IsRequired,
        MaxLength = m.MaxLength,
        NumberMax = m.NumberMax,
        NumberMin = m.NumberMin,
        ReadOnly = m.ReadOnly,
        ValidateRule = m.ValidateRule,
      }).ToList();
      lock (_lockUdfModel)
      {
        _dictCacheUdfModel[key] = dict; 
      }
      return dict;
    }
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
