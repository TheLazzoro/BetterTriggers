using BetterTriggers.Utility;
using BetterTriggers.WorldEdit;
using CASCLib;
using IniParser.Model;
using IniParser.Parser;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetterTriggers
{
    public static class Locale
    {
        private static Dictionary<string, string> WE_Strings = new Dictionary<string, string>();

        public static string Translate(string key)
        {
            string translation;
            WE_Strings.TryGetValue(key, out translation);
            return translation;
        }

        internal static void Load()
        {
            Settings settings = Settings.Load();

            if (settings.language == "en") // placeholder for now
            {

            }

            /*
            var locales = (CASCFolder)Casc.GetWar3ModFolder().Entries["_locales"];
            // cannot locate _locales.
            // we just have to bundle the translated .txt files with the project.

            var en_us = (CASCFolder)locales.Entries["enus.w3mod"];
            var ui = (CASCFolder)en_us.Entries["ui"];
            CASCFile worldEditStrings = (CASCFile)ui.Entries["worldeditstrings.txt"];
            var file = Casc.GetCasc().OpenFile(worldEditStrings.FullName);

            StreamReader reader = new StreamReader(file);
            string data = reader.ReadToEnd();
            string[] triggerData = data.Split("\r\n");

            AddEntries(triggerData);
            */

            //var iniFile = IniFileConverter.Convert(File.ReadAllText(System.IO.Directory.GetCurrentDirectory() + @"\Resources\abilityskinstring.txt"));
            var iniFile = IniFileConverter.Convert(File.ReadAllText(System.IO.Directory.GetCurrentDirectory() + @"\Resources\WorldEditorData\campaignunitstrings.txt"));
            var abilitySkinString = IniFileHelper.Parse(iniFile);

            AddUnitStringEntries(abilitySkinString);

            AddWorldEditStrings(File.ReadAllLines(System.IO.Directory.GetCurrentDirectory() + @"\Resources\WorldEditorData\worldeditstrings.txt"));
        }

        private static void AddUnitStringEntries(IniData iniData)
        {
            for (int i = 0; i < iniData.Sections.Count; i++)
            {
                var enumerator = iniData.Sections.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    if (enumerator.Current.Keys["Name"] != null)
                        WE_Strings.TryAdd(enumerator.Current.SectionName, enumerator.Current.Keys["Name"]);
                    else if (enumerator.Current.Keys["Bufftip"] != null)
                        WE_Strings.TryAdd(enumerator.Current.SectionName, enumerator.Current.Keys["Bufftip"]);
                }
            }
        }

        private static void AddWorldEditStrings(string[] content)
        {
            content[0] = ""; // First is an ini file section

            for (int i = 0; i < content.Length; i++)
            {
                string line = content[i];
                if (line.Length == 0 || line.Substring(0, 2) == "//")
                    continue;


                int keyLength = 0;
                for (int k = 0; k < line.Length; k++)
                {
                    if (line[k] == '=')
                    {
                        keyLength = k;
                        break;
                    }
                }

                string key = line.Substring(0, keyLength);
                string value = line.Substring(keyLength + 1, line.Length - 1 - keyLength).Replace("\"", "");

                WE_Strings.TryAdd(key, value);
            }
        }
    }
}
