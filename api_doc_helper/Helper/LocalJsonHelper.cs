using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SuperGMS.ApiHelper;

namespace Quantum.ApiDoc.Helper
{
  public class LocalJsonHelper
  {
    private string _ttid;
    private string _path;
    public LocalJsonHelper(string currPath, string ttid)
    {
      this._ttid = ttid;
      _path = Path.Combine(currPath, "assets");
    }
    Dictionary<string, List<ClassInfo>> _dict = new Dictionary<string, List<ClassInfo>>();
    private object _lockObject = new object();
    public async Task<Dictionary<string, List<ClassInfo>>> GetAllServieInterfaces(List<string> svrs)
    {
      lock (_lockObject)
      {
        if (_dict.Count > 0)
        {
          return _dict;
        }

        svrs.ForEach(svr =>
        {
          try
          {
            var txt = File.ReadAllText(Path.Combine(_path, $"{svr}.json"), Encoding.UTF8);
            _dict[svr] = JsonConvert.DeserializeObject<List<ClassInfo>>(txt);
          }
          catch (Exception e)
          {
            Trace.WriteLine(e.Message);
          }
        });

        return _dict;
      }
    }

    public void WriteJsonFile(ConcurrentDictionary<string, List<ClassInfo>> dict)
    {
      Parallel.ForEach(dict.Keys, svr =>
      {
        try
        {
          var json = JsonConvert.SerializeObject(dict[svr]
            ,
            Formatting.Indented,
            new JsonSerializerSettings
            {
              ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            });
          File.WriteAllText(Path.Combine(_path, $"{svr}.json"), json, Encoding.UTF8);
        }
        catch (Exception e)
        {
          Trace.WriteLine(e.Message);
        }
      });
    }
  }
}
