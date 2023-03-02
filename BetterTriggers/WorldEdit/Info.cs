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
            return MapInfo.ScriptLanguage;
        }

        internal static void Load()
        {
            MapInfo = CustomMapData.MPQMap.Info;
        }
    }
}
