using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetterTriggers
{
    public class ProgramSettings
    {
        private static ProgramSettings instance;
        private static string filePath;

        public bool IgnoreNewVersion = false;

        private ProgramSettings() { }

        public static ProgramSettings Load()
        {
            if (instance != null)
                return instance;

            string dir = Directory.GetCurrentDirectory();
            filePath = Path.Combine(dir, "Settings/Settings.json");
            ProgramSettings settings;
            if (File.Exists(filePath))
            {
                var file = File.ReadAllText(filePath);
                settings = JsonConvert.DeserializeObject<ProgramSettings>(file);
            }
            else
            {
                settings = new ProgramSettings();
            }
            instance = settings;

            return settings;
        }

        public static void Save(ProgramSettings settings)
        {
            string dir = Path.GetDirectoryName(filePath);
            string fileName = Path.GetFileName(filePath);

            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            instance = settings;
            File.WriteAllText(Path.Combine(dir, fileName), JsonConvert.SerializeObject(instance, Formatting.Indented));
        }
    }
}
