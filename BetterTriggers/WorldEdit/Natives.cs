using CASCLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetterTriggers.WorldEdit
{
    public static class Natives
    {
        internal static void Load()
        {
            string baseDir = System.IO.Directory.GetCurrentDirectory() + "/Resources/JassHelper/";
            string pathCommonJ = baseDir + "common.j";
            string pathBlizzardJ = baseDir + "Blizzard.j";

            if (!File.Exists(pathCommonJ)) {
                var units = (CASCFolder)Casc.GetWar3ModFolder().Entries["scripts"];

                CASCFile commonJ = (CASCFile)units.Entries["common.j"];
                Casc.SaveFile(commonJ, pathCommonJ);
            }

            if (!File.Exists(pathBlizzardJ))
            {
                var units = (CASCFolder)Casc.GetWar3ModFolder().Entries["scripts"];

                CASCFile blizzardJ = (CASCFile)units.Entries["Blizzard.j"];
                Casc.SaveFile(blizzardJ, pathBlizzardJ);
            }
        }
    }
}
