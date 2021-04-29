using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Quantum.ApiDoc.Helper
{
    /// <summary>
    /// ConstKeyValue
    /// </summary>
    public class ConstKeyValue
    {
        private string key;

        public string Key { get { return key; } }

        private string _value;

        public string Value { get { return _value; } }

        public ConstKeyValue(string key, string value)
        {
            this.key = key;
            this._value = value;
        }
    }
}
