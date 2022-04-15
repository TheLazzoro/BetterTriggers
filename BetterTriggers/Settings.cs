using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetterTriggers
{
    public class Settings
    {
        private static Settings instance;
        private static string filePath = Directory.GetCurrentDirectory() + @"\settings\settings.json";

        public string war3root { get; set; }
        public string language { get; set; }

        public static Settings Load()
        {
            if (instance != null)
                return instance;

            Settings settings;
            if (File.Exists(filePath))
            {
                var file = File.ReadAllText(filePath);
                settings = JsonConvert.DeserializeObject<Settings>(file);
            }
            else
            {
                settings = new Settings();
            }

            return settings;
        }

        public static void Save(Settings settings)
        {
            string dir = Path.GetDirectoryName(filePath);
            string fileName = Path.GetFileName(filePath);

            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            instance = settings;
            File.WriteAllText(Path.Combine(dir, fileName), JsonConvert.SerializeObject(instance));
        }
    }
}
