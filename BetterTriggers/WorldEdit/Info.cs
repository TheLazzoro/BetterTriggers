using BetterTriggers.Containers;
using War3Net.Build.Info;

namespace BetterTriggers.WorldEdit
{
    public class Info
    {
        internal static MapInfo MapInfo;

        public static ScriptLanguage GetLanguage(Project project)
        {
            if(project.war3project.Language == "jass")
                return ScriptLanguage.Jass;
            else
                return ScriptLanguage.Lua;
        }

        internal static void Load()
        {
            MapInfo = CustomMapData.MPQMap.Info;
        }
    }
}
