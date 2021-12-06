using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TriggerParser.Types;

namespace TriggerParser.Conditions
{
    public class TriggerCondition
    {
        public string key;
        public int version;
        public string displayName;
        public List<TriggerType> arguments;
        public string paramText;
        public string defaultParams;
        /*
        public string defaultFirstArgument; // should be a list of params with default value or just a string?
        public string defaultOperator;
        public string defaultSecondArgument;
        */
        public string category;
    }
}
