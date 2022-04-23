using BetterTriggers.WorldEdit;
using CASCLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetterTriggers.Locales
{
    public static class Locale
    {
        private static Dictionary<string, string> WE_Strings = new Dictionary<string, string>();

        public static void Load()
        {
            Settings settings = Settings.Load();

            if(settings.language == "en") // placeholder for now
            {

            }

            /*
            var locales = (CASCFolder)Casc.GetWar3ModFolder().Entries["_locales"];
            // cannot locate _locales.
            // we just have to bundle the translated .txt files weith the project.

            var en_us = (CASCFolder)locales.Entries["enus.w3mod"];
            var ui = (CASCFolder)en_us.Entries["ui"];
            CASCFile worldEditStrings = (CASCFile)ui.Entries["worldeditstrings.txt"];
            var file = Casc.GetCasc().OpenFile(worldEditStrings.FullName);

            StreamReader reader = new StreamReader(file);
            string data = reader.ReadToEnd();
            string[] triggerData = data.Split("\r\n");

            AddEntries(triggerData);
            */
        }
        
        public static string GetTranslation(string key)
        {
            string value = string.Empty;
            WE_Strings.TryGetValue(key, out value);
            return value;
        }

        public static void AddEntries(string[] lines)
        {
            for (int i = 0; i < lines.Length; i++)
            {
                var line = lines[i];
                if (line.Length > 0 && line.Substring(0, 1) == "[")
                    return;
                if (line.Length > 0 && line.Substring(0, 2) == "//")
                    return;

                int j = 0;
                string key = string.Empty;
                bool keyFound = false;
                while (!keyFound && j < line.Length) // key
                {
                    char c = line[j];
                    if (c == '=')
                        keyFound = true;
                    else
                        key += c;

                    j++;
                }

                string value = string.Empty;
                while (j < line.Length) // value
                {
                    char c = line[j];
                    if (c != '"')
                        value += c;

                    j++;
                }

                WE_Strings.Add(key, value);
            }
        }
    }
}
