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
        public static List<Value> GetImportsAll(string returnType)
        {
            List<Value> models = new List<Value>();
            string mapDir = CustomMapData.mapPath + "/";
            string[] files = Directory.GetFiles(mapDir, "*", SearchOption.AllDirectories);

            foreach (var file in files)
            {
                if(file.EndsWith(".mdx") || file.EndsWith(".mdl"))
                models.Add(new Value() {
                    identifier = file.Substring(mapDir.Length, file.Length - mapDir.Length),
                    returnType = returnType,
                });
            }

            return models;
        }
    }
}