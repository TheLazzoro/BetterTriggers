using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using BetterTriggers.Models.SaveableData;

namespace BetterTriggers.Controllers
{
    public class ControllerImports
    {
        public static List<Value> GetImportsByReturnType(string returnType)
        {
            List<Value> imports = new List<Value>();
            string mapDir = CustomMapData.mapPath + "/";
            string[] files = Directory.GetFiles(mapDir, "*", SearchOption.AllDirectories);

            foreach (var file in files)
            {
                if (returnType == "skymodelstring" && (file.ToLower().EndsWith(".mdx") || file.ToLower().EndsWith(".mdl")))
                    imports.Add(new Value()
                    {
                        identifier = file.Substring(mapDir.Length, file.Length - mapDir.Length),
                    });
                else if(returnType == "musictheme" && (file.ToLower().EndsWith(".mp3") || file.ToLower().EndsWith(".wav") || file.ToLower().EndsWith(".flac")))
                    imports.Add(new Value()
                    {
                        identifier = file.Substring(mapDir.Length, file.Length - mapDir.Length),
                    });
            }

            return imports;
        }
    }
}