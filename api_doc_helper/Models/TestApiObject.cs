using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SuperGMS.ApiDoc.Models
{
    public class TestApiObject
    {
        public string Name { get; set; }
        public string Desc { get; set; }
        public string InputParam { get; set; }
        public string ApiUri { get; set; }
        public bool HasToken { get; set; }
    }
}
