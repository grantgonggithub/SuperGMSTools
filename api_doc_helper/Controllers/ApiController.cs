using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using SuperGMS.ApiDoc.Models;

using SuperGMS.ApiHelper;
using SuperGMS.Config;

namespace SuperGMS.ApiDoc.Controllers
{
  [Produces("application/json")]
  [Route("[controller]/[action]")]
  public  class ApiController : Controller
  {
    [HttpPost]
    public List<ClassInfo> GetApiInfo([FromBody]ServerApiParam serverName)
    {
      try
      {
        if (string.IsNullOrEmpty(serverName?.SvrName))
        {
          return null;
        }

        if (Program.Dict.ContainsKey(serverName.SvrName))
        {
          return Program.Dict[serverName.SvrName];
        }
        //else
        //{
        //  var rst = Program.helper.GetServiceInterfaces(serverName.SvrName).Result;
        //  Program.Dict[serverName.SvrName] = rst;
        //}
      }
      catch {

      }
      return new List<ClassInfo>();
      
    }

    public List<string> GetSvrList()
    {
      return Program.Svrs;
    }
    public string GetHttpProxy()
    {
      return ServerSetting.GetConstValue("HttpProxy")?.Value ?? "http://192.168.7.207/v2_api/";
    }
  }
}
