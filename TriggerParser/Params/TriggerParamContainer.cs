using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TriggerParser.Params
{
    public static class TriggerParamContainer
    {
        public static List<TriggerParam> container = new List<TriggerParam>();

        public static TriggerParam FindByKey(string key)
        {
            TriggerParam constant = null;
            int i = 0;
            bool found = false;

            while (!found)
            {
                if (container.Count > i && container[i].key == key)
                {
                    found = true;
                    constant = container[i];
                }

                i++;
            }

            return constant;
        }
    }
}
