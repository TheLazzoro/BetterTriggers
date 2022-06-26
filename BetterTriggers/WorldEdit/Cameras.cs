using BetterTriggers.Models.War3Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using War3Net.Build.Environment;
using War3Net.Build.Extensions;

namespace BetterTriggers.WorldEdit
{
    internal class Cameras
    {
        private static List<Camera> cameras = new List<Camera>();

        internal static List<Camera> GetAll()
        {
            return cameras;
        }

        internal static void Load()
        {
            cameras.Clear();

            string filePath = "war3map.w3c";
            if (!File.Exists(Path.Combine(CustomMapData.mapPath, filePath)))
                return;

            while(CustomMapData.IsMapSaving())
            {
                Thread.Sleep(1000);
            }

            using (Stream s = new FileStream(Path.Combine(CustomMapData.mapPath, filePath), FileMode.Open, FileAccess.Read))
            {
                BinaryReader reader = new BinaryReader(s);
                var mapCameras = BinaryReaderExtensions.ReadMapCameras(reader);

                mapCameras.Cameras.ForEach(c => cameras.Add(c));
            }
        }
    }
}
