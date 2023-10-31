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
            if(ContainerProject.project.Language == "jass")
                SetLanguage(ScriptLanguage.Jass);
            else
                SetLanguage(ScriptLanguage.Lua);


            return MapInfo.ScriptLanguage;
        }

        public static void SetLanguage(ScriptLanguage language)
        {
            if (MapInfo != null)
            {
                MapInfo.ScriptLanguage = language;
            }
        }

        internal static void Load()
        {
            MapInfo = CustomMapData.MPQMap.Info;
        }
    }
}
