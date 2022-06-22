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


        internal static void Load()
        {
            string filePath = "war3map.w3i";
            if (!File.Exists(Path.Combine(CustomMapData.mapPath, filePath)))
                return;

            while (CustomMapData.IsMapSaving())
            {
                Thread.Sleep(1000);
            }

            using (Stream s = new FileStream(Path.Combine(CustomMapData.mapPath, filePath), FileMode.Open, FileAccess.Read))
            {
                BinaryReader reader = new BinaryReader(s);
                MapInfo = BinaryReaderExtensions.ReadMapInfo(reader);
            }
        }

    }
}
