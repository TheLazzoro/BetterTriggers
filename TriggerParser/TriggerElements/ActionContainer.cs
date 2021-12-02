using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TriggerParser.TriggerElements
{
    public static class ActionContainer
    {
        public static readonly int Id = 2;
        public static List<TriggerElement> container = new List<TriggerElement>();

        public static TriggerElement FindByKey(string key)
        {
            TriggerElement triggerAction = null;
            int i = 0;
            bool found = false;

            while (!found)
            {
                if (container.Count > i && container[i].key == key)
                {
                    found = true;
                    triggerAction = container[i];
                }
                
                i++;
            }

            return triggerAction;
        }
    }
}
