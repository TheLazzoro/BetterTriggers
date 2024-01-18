using BetterTriggers.Containers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using War3Net.Build.Audio;
using War3Net.Build.Extensions;
using War3Net.Build.Info;

namespace BetterTriggers.WorldEdit
{
    public class Info
    {
        internal static MapInfo MapInfo;

        public static ScriptLanguage GetLanguage()
        {
            var project = Project.CurrentProject;
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
