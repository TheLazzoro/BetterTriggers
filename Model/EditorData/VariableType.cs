using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.EditorData
{
    public class VariableType
    {
        public string displayname { get; }
        public string key { get; }
        public VariableType(string displayname, string key)
        {
            this.displayname = displayname;
            this.key = key;
        }
    }
}
