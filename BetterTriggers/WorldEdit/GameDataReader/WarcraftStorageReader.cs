using NuGet.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetterTriggers.WorldEdit.GameDataReader
{
    public class WarcraftStorageReader
    {
        private static GameMpq mpq = null;

        public static bool IsReforged { get; set; } = false;
        public static Version GameVersion = new Version();

        public static (bool, string) Load()
        {
            EditorSettings settings = EditorSettings.Load();
            IsReforged = File.Exists(Path.Combine(settings.war3root, @"Data\data\data.000"));
            if (IsReforged)
            {
                var result = Casc.Load();
                bool success = result.Item1;
                string errorMsg = result.Item2;
                if (success)
                {
                    GameVersion = Casc.GameVersion;
                    return (true, errorMsg);
                }
                return (false, errorMsg);
            }
            mpq = new GameMpq();
            return mpq.Load(settings.war3root);
        }

        public static bool FileExists(string path)
        {
            if (IsReforged)
            {
                var file = Casc.GetWar3ModFolder();
                path = Path.Combine(file.Name, path);
                return Casc.GetCasc().FileExists(path);
            }
            return mpq.FileExists(path);
        }

        public static Stream OpenFile(string path, string archiveName = null)
        {
            if (IsReforged)
            {
                var file = Casc.GetWar3ModFolder();
                path = Path.Combine(file.Name, path);
                return Casc.GetCasc().OpenFile(path);
            }

            return mpq.Open(path, archiveName);
        }

        public static Stream OpenFile_x86(string path)
        {
            if (IsReforged)
            {
                var file = Casc.Getx86Folder();
                path = Path.Combine(file.Name, path);
                return Casc.GetCasc().OpenFile(path);
            }

            return mpq.Open(path);
        }

        public static string ReadAllText(string path, string archiveName = null)
        {
            var stream = OpenFile(path, archiveName);
            if (stream != null)
            {
                return new StreamReader(stream).ReadToEnd();
            }

            return string.Empty;
        }

        public static string[] ReadAllLines(string path, string archiveName = null)
        {
            var stream = OpenFile(path, archiveName);
            if (stream != null)
            {
                var liens = new List<string>();
                var reader = new StreamReader(stream);

                while (true)
                {
                    var line = reader.ReadLine();
                    if (line == null)
                    {
                        break;
                    }
                    liens.Add(line);
                }

                return liens.ToArray();
            }

            return null;
        }

        public static void Export(string srcPath, string destPath)
        {
            var stream = OpenFile(srcPath);
            using var destStream = File.Create(destPath);
            stream.CopyTo(destStream);
        }

        public static void Export_x86(string srcPath, string destPath)
        {
            var stream = OpenFile_x86(srcPath);
            using var destStream = File.Create(destPath);
            stream.CopyTo(destStream);
        }


    }
}
