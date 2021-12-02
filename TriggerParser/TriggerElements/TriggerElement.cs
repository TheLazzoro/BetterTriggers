using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TriggerParser.Types;

namespace TriggerParser.TriggerElements
{
    public class TriggerElement
    {
        public string key;
        public int version;
        public string displayName;
        public string paramText;
        public List<TriggerType> arguments;
        public string defaultParams; // should be a list of params with default value or just a string?
        // public List<float> limits // for example, limits time of day to float between 00.00 and 24.00. Maybe needs type int as indicated by 'triggerdata.txt' ?
        public string category;
    }
}
