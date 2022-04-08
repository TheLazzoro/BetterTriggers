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
    public class Cameras
    {
        public static List<Camera> Load()
        {
            Stream s = new FileStream(@"C:\Users\Lasse Dam\Desktop\test2.w3x\war3map.w3c", FileMode.Open);
            BinaryReader reader = new BinaryReader(s);
            var cameras = BinaryReaderExtensions.ReadMapCameras(reader);
            return cameras.Cameras;
        }
    }
}
