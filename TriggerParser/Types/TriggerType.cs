using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TriggerParser.Types
{
    public class TriggerType
    {
        public string key;
        public int version;
        public bool globalVariable;
        public bool canCompare;
        public string displayName;
        public string baseType; // only used for custom types, i.e. and 'extends'. E.g. 'unitcode' extends integer.
        public string importType; // for strings which represent files (optional)
        public bool flag; // indicating to treat this type as the base type in the editor
    }
}
