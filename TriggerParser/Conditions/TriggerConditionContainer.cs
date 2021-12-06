using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TriggerParser.Conditions
{
    public static class TriggerConditionContainer
    {
        public static List<TriggerCondition> container = new List<TriggerCondition>();

        public static TriggerCondition FindByKey(string key)
        {
            TriggerCondition constant = null;
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
