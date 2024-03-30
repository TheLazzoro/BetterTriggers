using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using BetterTriggers.Models.SaveableData;
using War3Net.IO.Mpq;
using War3Net.Build;
using BetterTriggers.Containers;

namespace BetterTriggers.WorldEdit
{
    public class Imports
    {
        public static List<Value_Saveable> GetImportsByReturnType(string returnType)
        {
            var project = Project.CurrentProject;
            string fullMapPath = project.GetFullMapPath();
            bool isMapMPQ = File.Exists(fullMapPath);
            List<Value_Saveable> imports = new List<Value_Saveable>();
            List<string> files = new List<string>();
            string mapDir = fullMapPath + "/";

            if (isMapMPQ)
            {
                if (CustomMapData.MPQMap.ImportedFiles != null)
                {
                    var mpqFiles = CustomMapData.MPQMap.ImportedFiles.Files;
                    mpqFiles.ForEach(f =>
                    {
                        files.Add(f.FullPath);
                    });
                }
            }
            else
            {
                string[] entries = Directory.GetFiles(mapDir, "*", SearchOption.AllDirectories);
                for (int i = 0; i < entries.Length; i++)
                {
                    files.Add(entries[i]);
                }
            }

            foreach (var file in files)
            {
                string fileName = string.Empty;
                if (
                    (returnType == "modelfile" || returnType == "skymodelstring") && (file.ToLower().EndsWith(".mdx") || file.ToLower().EndsWith(".mdl") ||
                    returnType == "musictheme" && (file.ToLower().EndsWith(".mp3") || file.ToLower().EndsWith(".wav") || file.ToLower().EndsWith(".flac"))
                   ))
                {
                    fileName = file;
                    if (!isMapMPQ)
                        fileName = file.Substring(mapDir.Length, file.Length - mapDir.Length);

                    imports.Add(new Value_Saveable()
                    {
                        value = fileName
                    });
                }
            }

            return imports;
        }
    }
}