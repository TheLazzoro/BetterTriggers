using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TriggerParser.TriggerElements
{
    public static class TriggerElementContainer
    {
        public static List<TriggerElement> container = new List<TriggerElement>();

        public static TriggerElement FindByKey(string key)
        {
            TriggerElement triggerAction = null;
            int i = 0;
            bool found = false;

            while (!found)
            {
                if (i < container.Count && container[i].key == key)
                {
                    found = true;
                    triggerAction = container[i];
                } else if(i >= container.Count)
                {
                    break;
                }

                i++;
            }

            if (!found)
            {
                Console.WriteLine($"Didn't find any matching name for {key}");
                throw new Exception();
            }

            return triggerAction;
        }
    }
}
