using BetterTriggers.Containers;
using War3Net.Build.Extensions;
using War3Net.Build.Script;

namespace BetterTriggers.WorldEdit
{
    public class MapStrings
    {
        private TriggerStrings triggerStrings;

        internal string GetString(string trigStr)
        {
            string str;
            triggerStrings.TryGetValue(trigStr, out str);
            if (str == null)
                return string.Empty;

            return str;
        }
        
        internal void Load(Project project)
        {
            triggerStrings = project.MPQMap.TriggerStrings;
        }
    }
}
