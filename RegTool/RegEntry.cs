using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegTool
{
    public class RegEntry
    {
        public string Key;
        public string SubKey;
        public object Value;

        public bool IsString
        {
            get { return Value is string; }
        }

        public override string ToString()
        {
            return string.Format("{0} = {1} ({2})", Key, SubKey, Value);
        }
    }
}
