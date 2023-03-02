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

            MapCameras mapCameras;
            mapCameras = CustomMapData.MPQMap.Cameras;
            if (mapCameras == null)
                return;

            mapCameras.Cameras.ForEach(c => cameras.Add(c));
        }
    }
}
