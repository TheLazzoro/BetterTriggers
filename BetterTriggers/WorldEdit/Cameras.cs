using Model.War3Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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

            Stream s = new FileStream(Path.Combine(CustomMapData.mapPath, "war3map.w3c"), FileMode.Open);
            BinaryReader reader = new BinaryReader(s);
            var mapCameras = BinaryReaderExtensions.ReadMapCameras(reader);

            mapCameras.Cameras.ForEach(c => cameras.Add(c));
        }
    }
}
