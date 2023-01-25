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
        private static string filePath;

        public string war3root;
        public string CopyLocation         = "BetterTriggersTestMap";
        public int Difficulty              = 0;
        public bool FixedRandomSeed        = false;
        public int HD                      = 0;
        public bool NoWindowsFocusPause    = false;
        public string PlayerProfile        = "WorldEdit";
        public int WindowMode              = 0;
        public int Teen                    = 0;

        public bool mainWindowFullscreen   = false;
        public int mainWindowX             = 100;
        public int mainWindowY             = 100;
        public int mainWindowWidth         = 900;
        public int mainWindowHeight        = 600;

        public int triggerExplorerWidth    = 250;

        public int triggerWindowX          = 100;
        public int triggerWindowY          = 100;
        public int triggerWindowWidth      = 800;
        public int triggerWindowHeight     = 450;

        public int parameterWindowX        = 100;
        public int parameterWindowY        = 100;
        public int parameterWindowWidth    = 800;
        public int parameterWindowHeight   = 450;

        public int triggerEditorMode       = 0;

        private Settings() { }

        public static Settings Load()
        {
            if (instance != null)
                return instance;

            string appData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            filePath = Path.Combine(appData, "Better Triggers/settings.json");
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
            instance = settings;

            return settings;
        }

        public static void Save(Settings settings)
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
