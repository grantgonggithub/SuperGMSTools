using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Quantum.ApiDoc.Helper
{
    public class ScmParse
    {
        private const string HttpProxyName = "HttpProxy";
        private const string CONSTKEYVALUE = "ConstKeyValue";
        public static Dictionary<string, ConstKeyValue> ConstDics = new Dictionary<string, ConstKeyValue>();
        public static string[] Services = null;

        public static void Init()
        {
            ConstDics.Clear();
            string path = Path.Combine(AppContext.BaseDirectory, "scm.config");
            XElement root = XElement.Load(path);
            if (root == null || root.Element(HttpProxyName) == null)
                throw new System.Exception("你不配置后端服务列表，ApiHelp犯傻了~~~");

            XElement cstValue = root.Element(CONSTKEYVALUE);
            if (cstValue != null) //keyvalue解析
            {
                updateConstKeyValue(cstValue);
            }


            var items = root.Element("HttpProxy")?.Elements("Item")?.GetEnumerator();
            if (items == null) throw new System.Exception("你不配置后端服务列表Item，ApiHelp犯傻了~~~");

            List<string> li = new List<string>();
            while (items.MoveNext())
            {
                li.Add(items.Current.Attribute("name")?.Value);
            }
            Services = li.ToArray();


        }


        private static void updateConstKeyValue(XElement cstValue)
        {
            try
            {
                var items = cstValue.Elements("item").GetEnumerator();
                while (items.MoveNext())
                {
                    if (items.Current != null)
                    {
                        if (items.Current.Attribute("key") != null)
                        {
                            ConstKeyValue kv = new ConstKeyValue(items.Current.Attribute("key").Value.ToLower(),
                                items.Current.Attribute("value")?.Value);
                            if (!ConstDics.ContainsKey(kv.Key)) ConstDics.Add(kv.Key, kv);
                            else ConstDics[kv.Key] = kv;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(new Exception("ConfigManager.updateConstKeyValue.Error", ex));
            }
        }
    }
}
