using Newtonsoft.Json;
using NuGet.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetterTriggers
{
    public class LastOpenedTabs
    {
        public string[] Tabs;
        private const string tabhistoryDir = "Better Triggers/tabhistory";

        private LastOpenedTabs() { }

        public static LastOpenedTabs Load(string mapName)
        {
            string appData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string dir = Path.Combine(appData, tabhistoryDir);
            string fullPath = Path.Combine(dir, mapName + ".json");
            LastOpenedTabs tabs;
            if (File.Exists(fullPath))
            {
                var file = File.ReadAllText(fullPath);
                tabs = JsonConvert.DeserializeObject<LastOpenedTabs>(file);
            }
            else
            {
                tabs = new LastOpenedTabs();
            }

            return tabs;
        }

        public static void Save(string mapName, string[] tabs)
        {
            string appData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string dir = Path.Combine(appData, tabhistoryDir);
            string fullPath = Path.Combine(dir, mapName + ".json");
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            LastOpenedTabs lastOpenedTabs = new LastOpenedTabs
            {
                Tabs = tabs,
            };
            File.WriteAllText(Path.Combine(dir, fullPath), JsonConvert.SerializeObject(lastOpenedTabs, Formatting.Indented));
        }
    }
}
