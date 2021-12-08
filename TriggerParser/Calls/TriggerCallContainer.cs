using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TriggerParser.Calls
{
    public static class TriggerCallContainer
    {
        public static List<TriggerCall> container = new List<TriggerCall>();

        public static TriggerCall FindByKey(string key)
        {
            TriggerCall call = null;
            int i = 0;
            bool found = false;

            while (!found)
            {
                if (container.Count > i && container[i].key == key)
                {
                    found = true;
                    call = container[i];
                }

                i++;
            }

            return call;
        }
    }
}
