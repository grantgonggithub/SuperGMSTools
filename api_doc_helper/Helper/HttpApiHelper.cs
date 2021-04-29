using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Extensions.Logging;
using SuperGMS.Log;
using SuperGMS.Protocol.RpcProtocol;

namespace Quantum.ApiDoc.Helper
{
  public class HttpApiHelper
  {
    public HttpClient Client { get; set; }
    public string Token { get; set; }

    public string Lang { get; set; }
    public string ClientType { get; set; }

    public string Host { get; set; }
    public HttpApiHelper(HttpClient client, string token,string host)
    {
      this.Client = client;
      this.Token = token;
      this.Lang = "zh_cn";
      this.ClientType = string.Empty;
      this.Host = host;
    }

    private readonly static ILogger logger = LogFactory.CreateLogger<HttpApiHelper>();

    /// <summary>
    /// 访问api，并返回结果
    /// </summary>
    /// <typeparam name="TA"></typeparam>
    /// <typeparam name="TR"></typeparam>
    /// <param name="uri">相对Api的路径 eg: 服务名/api名</param>
    /// <param name="inputParam"></param>
    /// <param name="bPost"></param>
    /// <returns></returns>
    public async Task<TR> GetApiContent<TA, TR>(string uri, TA inputParam, bool bPost = true)
      where TR : class, new()
    {
      try
      {
        var args = new Args<TA>()
        {
          v = inputParam,
          tk = Token,
          ct = "api_doc_helper",
          cv = "1.0.1"
        };
        var sJson = Newtonsoft.Json.JsonConvert.SerializeObject(args);
        HttpResponseMessage apiContent;
        if (bPost)
        {
          HttpContent ctx = new StringContent(sJson, Encoding.UTF8, "application/json");
          apiContent = await Client.PostAsync(uri, ctx);
        }
        else
        {
          apiContent = await Client.GetAsync($"{uri}?args={sJson}");
        }

        if (apiContent.IsSuccessStatusCode)
        {
          var content = await apiContent.Content.ReadAsStringAsync();
          if (!string.IsNullOrEmpty(content))
          {
            try
            {
              var c = Newtonsoft.Json.JsonConvert.DeserializeObject<Result<TR>>(content);
              if (c == null)
              {
                logger.LogInformation($"访问 {uri} 返回了内容，解析为null ");
                return default(TR);
              }
              else
              {
                if (c.c != StatusCode.OK.code)
                {
                  logger.LogInformation($"访问 {uri} 返回了内容，返回状态：{c.c}:{c.msg} ");
                  return null;
                }

                return c.v;
              }
            }
            catch (Exception ex)
            {
              logger.LogError(ex, $"访问 {uri} 返回了内容，但解析失败");
              return null;
            }
          }
          else
          {
            logger.LogInformation($"访问 {uri} 没有内容返回! ");
            return null;
          }
        }
        else
        {
          logger.LogInformation($"访问 {uri} 失败! 状态:{apiContent.StatusCode}");
          return null;
        }
      }
      catch (Exception e)
      {
        logger.LogError(e, $"访问 {uri} 失败! ");
        return null;
      }

    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="uri">http://192.168.7.207/</param>
    /// <param name="userid"></param>
    /// <param name="password"></param>
    /// <returns></returns>
    private async Task<string> GetSsid(string uri, string userid,string password)
    {
      // password  md5(user.userid + md5(user.password))
      try
      {
        var md5 = MD5.Create();
        var bytes = md5.ComputeHash(Encoding.UTF8.GetBytes(password));
        string byte2String = null;
        for (int i = 0; i < bytes.Length; i++)
        {
          byte2String += bytes[i].ToString("x2");
        }
        
        bytes = md5.ComputeHash(Encoding.UTF8.GetBytes(userid + byte2String));
        byte2String = string.Empty;
        for (int i = 0; i < bytes.Length; i++)
        {
          byte2String += bytes[i].ToString("x2");
        }

        NameValueCollection dict = HttpUtility.ParseQueryString(String.Empty);
        dict.Add("userid", userid);
        dict.Add("password", byte2String);
        dict.Add("token", "");
        byte[] byteArray = Encoding.UTF8.GetBytes(dict.ToString());

        WebRequest request = WebRequest.CreateHttp(uri + "/Login/Login");
        request.Proxy = null;
        request.Method = "POST";
        request.ContentType = "application/x-www-form-urlencoded;";
        request.ContentLength = byteArray.Length;
        using (Stream newStream = request.GetRequestStream())
        {
          newStream.Write(byteArray, 0, byteArray.Length);
          newStream.Close();
        }

        var resp = await request.GetResponseAsync();
        string sResp = "";
        using (Stream s = resp.GetResponseStream())
        {
          using (var rs = new MemoryStream())
          {
            await s.CopyToAsync(rs, 1024);
            sResp = Encoding.UTF8.GetString(rs.ToArray());
          }

          s.Close();
        }

        if (sResp.Contains("\"flag\":true"))
        {
          string cookie = resp.Headers.Get("Set-Cookie");
          if (!string.IsNullOrEmpty(cookie))
          {
            var n = cookie.IndexOf("SSID=");
            cookie = cookie.Substring(n + 5, cookie.IndexOf(";") - n - 5);
            //CookieContainer cc = new CookieContainer();
            //cc.SetCookies(new Uri(Program.Config.Login),cookie);
            var ssid = cookie;
            return ssid.ToString();
          }
        }

        return "";
      }
      catch (Exception ex)
      {
        logger.LogError(ex, "");
        return "";
      }
    }

    public async Task<string> InitToken(string uri, string userid, string password)
    {
      var token = await this.GetApiContent<object, GetTokenBySsidResult>(uri + "TravelingsmoothlyAuthService/EmployeeLoginApi",
        new {  LoginName=userid, PassWord=password });
      this.Token = token?.token;
      if (string.IsNullOrEmpty(this.Token))
      {
        logger.LogInformation("未能获取Token!");
      }
      return token?.token;
    }

    private class GetTokenBySsidResult
    {
      public string token { get; set; }
      public string EmployeeId { get; set; }
      public string loginname { get; set; }
      public string TtId { get; set; }
  }
  }
}
