using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using War3Net.Build.Extensions;
using War3Net.Build.Script;

namespace BetterTriggers.WorldEdit
{
    internal class MapStrings
    {
        private static TriggerStrings triggerStrings;

        internal static string GetString(string trigStr)
        {
            string str;
            triggerStrings.TryGetValue(trigStr, out str);
            if (str == null)
                return string.Empty;

            return str;
        }
        
        internal static void Load()
        {
            triggerStrings = CustomMapData.MPQMap.TriggerStrings;
        }
    }
}
