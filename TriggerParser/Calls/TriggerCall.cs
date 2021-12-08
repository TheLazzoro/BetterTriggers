using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TriggerParser.Types;

namespace TriggerParser.Calls
{
    public class TriggerCall
    {
        public string key;
        public int version;
        public int canBeUsedInEvent;
        public List<TriggerType> arguments;
        public string returnType;
        public string displayName;
        public string paramText;
        public string defaultParams;
        public string category;
    }
}
