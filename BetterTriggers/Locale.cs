using BetterTriggers.Models.War3Data;
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
        private static Dictionary<string, string> WE_Strings;
        private static Dictionary<string, string> DisplayNames;
        private static Dictionary<string, string> EditorSuffixes;
        private static Dictionary<string, UnitName> Unit_Names;


        public static string Translate(string key)
        {
            if (key == null)
                return "";

            string translation;
            WE_Strings.TryGetValue(key, out translation);
            if (translation == null)
                WE_Strings.TryGetValue(key.ToUpper(), out translation);
            if (translation == null)
                    translation = key;

            return translation;
        }

        internal static UnitName GetUnitName(string unitcode)
        {
            UnitName name;
            Unit_Names.TryGetValue(unitcode, out name);
            return name;
        }

        internal static string GetEditorSuffix(string code)
        {
            if (code == null)
                return null;

            string text;
            EditorSuffixes.TryGetValue(code, out text);
            return text;
        }

        internal static string GetDisplayName(string code)
        {
            if (code == null)
                return null;

            string text;
            DisplayNames.TryGetValue(code, out text);
            return text;
        }

        public static void Load()
        {
            Settings settings = Settings.Load();
            WE_Strings = new Dictionary<string, string>();
            DisplayNames = new Dictionary<string, string>();
            EditorSuffixes = new Dictionary<string, string>();
            Unit_Names = new Dictionary<string, UnitName>();


            /*
            if (settings.language == "en") // placeholder for now
            {

            }

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

            List<string> genericData = new List<string>();
            List<string> unitData = new List<string>();
            unitData.Add(File.ReadAllText(System.IO.Directory.GetCurrentDirectory() + @"\Resources\WorldEditorData\campaignunitstrings.txt"));
            unitData.Add(File.ReadAllText(System.IO.Directory.GetCurrentDirectory() + @"\Resources\WorldEditorData\humanunitstrings.txt"));
            unitData.Add(File.ReadAllText(System.IO.Directory.GetCurrentDirectory() + @"\Resources\WorldEditorData\neutralunitstrings.txt"));
            unitData.Add(File.ReadAllText(System.IO.Directory.GetCurrentDirectory() + @"\Resources\WorldEditorData\nightelfunitstrings.txt"));
            unitData.Add(File.ReadAllText(System.IO.Directory.GetCurrentDirectory() + @"\Resources\WorldEditorData\orcunitstrings.txt"));
            unitData.Add(File.ReadAllText(System.IO.Directory.GetCurrentDirectory() + @"\Resources\WorldEditorData\undeadunitstrings.txt"));
            genericData.Add(File.ReadAllText(System.IO.Directory.GetCurrentDirectory() + @"\Resources\WorldEditorData\abilityskinstrings.txt"));
            genericData.Add(File.ReadAllText(System.IO.Directory.GetCurrentDirectory() + @"\Resources\WorldEditorData\campaignabilitystrings.txt"));
            genericData.Add(File.ReadAllText(System.IO.Directory.GetCurrentDirectory() + @"\Resources\WorldEditorData\campaignupgradestrings.txt"));
            genericData.Add(File.ReadAllText(System.IO.Directory.GetCurrentDirectory() + @"\Resources\WorldEditorData\commandskinstrings.txt"));
            genericData.Add(File.ReadAllText(System.IO.Directory.GetCurrentDirectory() + @"\Resources\WorldEditorData\commandstrings.txt"));
            genericData.Add(File.ReadAllText(System.IO.Directory.GetCurrentDirectory() + @"\Resources\WorldEditorData\commonabilitystrings.txt"));
            genericData.Add(File.ReadAllText(System.IO.Directory.GetCurrentDirectory() + @"\Resources\WorldEditorData\destructableskinstrings.txt"));
            genericData.Add(File.ReadAllText(System.IO.Directory.GetCurrentDirectory() + @"\Resources\WorldEditorData\humanabilitystrings.txt"));
            genericData.Add(File.ReadAllText(System.IO.Directory.GetCurrentDirectory() + @"\Resources\WorldEditorData\humanupgradestrings.txt"));
            genericData.Add(File.ReadAllText(System.IO.Directory.GetCurrentDirectory() + @"\Resources\WorldEditorData\itemabilitystrings.txt"));
            genericData.Add(File.ReadAllText(System.IO.Directory.GetCurrentDirectory() + @"\Resources\WorldEditorData\itemskinstrings.txt"));
            genericData.Add(File.ReadAllText(System.IO.Directory.GetCurrentDirectory() + @"\Resources\WorldEditorData\itemstrings.txt"));
            genericData.Add(File.ReadAllText(System.IO.Directory.GetCurrentDirectory() + @"\Resources\WorldEditorData\neutralabilitystrings.txt"));
            genericData.Add(File.ReadAllText(System.IO.Directory.GetCurrentDirectory() + @"\Resources\WorldEditorData\neutralupgradestrings.txt"));
            genericData.Add(File.ReadAllText(System.IO.Directory.GetCurrentDirectory() + @"\Resources\WorldEditorData\nightelfabilitystrings.txt"));
            genericData.Add(File.ReadAllText(System.IO.Directory.GetCurrentDirectory() + @"\Resources\WorldEditorData\nightelfupgradestrings.txt"));
            genericData.Add(File.ReadAllText(System.IO.Directory.GetCurrentDirectory() + @"\Resources\WorldEditorData\orcabilitystrings.txt"));
            genericData.Add(File.ReadAllText(System.IO.Directory.GetCurrentDirectory() + @"\Resources\WorldEditorData\orcupgradestrings.txt"));
            genericData.Add(File.ReadAllText(System.IO.Directory.GetCurrentDirectory() + @"\Resources\WorldEditorData\undeadabilitystrings.txt"));
            genericData.Add(File.ReadAllText(System.IO.Directory.GetCurrentDirectory() + @"\Resources\WorldEditorData\undeadupgradestrings.txt"));
            genericData.Add(File.ReadAllText(System.IO.Directory.GetCurrentDirectory() + @"\Resources\WorldEditorData\unitglobalstrings.txt"));
            genericData.Add(File.ReadAllText(System.IO.Directory.GetCurrentDirectory() + @"\Resources\WorldEditorData\unitskinstrings.txt"));
            genericData.Add(File.ReadAllText(System.IO.Directory.GetCurrentDirectory() + @"\Resources\WorldEditorData\upgradeskinstrings.txt"));

            genericData.ForEach(iniFile => AddGenericStringEntries(IniFileConverter.GetIniData(iniFile)));
            unitData.ForEach(iniFile => AddUnitStringEntries(IniFileConverter.GetIniData(iniFile)));


            AddWorldEditStrings(File.ReadAllLines(System.IO.Directory.GetCurrentDirectory() + @"\Resources\WorldEditorData\worldeditstrings.txt"));
            AddWorldEditStrings(File.ReadAllLines(System.IO.Directory.GetCurrentDirectory() + @"\Resources\WorldEditorData\worldeditgamestrings.txt"));
            AddWorldEditStrings(File.ReadAllLines(System.IO.Directory.GetCurrentDirectory() + @"\Resources\WorldEditorData\Custom\worldeditstrings_custom.txt"));

            AddTriggerHints(IniFileConverter.GetIniData(File.ReadAllText(System.IO.Directory.GetCurrentDirectory() + @"\Resources\WorldEditorData\ui\triggerstrings.txt")));
            AddTriggerHints(IniFileConverter.GetIniData(File.ReadAllText(System.IO.Directory.GetCurrentDirectory() + @"\Resources\WorldEditorData\Custom\triggerstrings_custom.txt")));
        }

        private static void AddTriggerHints(IniData iniData)
        {
            for (int i = 0; i < iniData.Sections.Count; i++)
            {
                var enumerator = iniData.Sections.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    var keyEnumerator = enumerator.Current.Keys.GetEnumerator();
                    while (keyEnumerator.MoveNext())
                    {
                        string key = keyEnumerator.Current.KeyName;
                        if (key.EndsWith("Hint"))
                        {
                            WE_Strings.TryAdd(key.Substring(0, key.Length - 4), keyEnumerator.Current.Value.Replace("\"", ""));
                        }
                    }
                }
            }
        }

        private static void AddGenericStringEntries(IniData iniData)
        {
            for (int i = 0; i < iniData.Sections.Count; i++)
            {
                var enumerator = iniData.Sections.GetEnumerator();
                string sectionName = null;
                while (enumerator.MoveNext())
                {
                    sectionName = enumerator.Current.SectionName;
                    string displayName = null;
                    if (enumerator.Current.Keys["Name"] != null)
                        displayName = enumerator.Current.Keys["Name"];
                    else if (enumerator.Current.Keys["Bufftip"] != null)
                        displayName = enumerator.Current.Keys["Bufftip"];
                    else if (enumerator.Current.Keys["EditorName"] != null)
                        displayName = enumerator.Current.Keys["EditorName"];

                    // Trim display name - upgrade name fields can contain multiple names (upgrade level 1, 2, 3 etc.)
                    if (displayName != null)
                    {
                        if (displayName.Contains(','))
                            displayName = displayName.Substring(0, displayName.IndexOf(',')); // finds the first index of char.
                        DisplayNames.TryAdd(sectionName, displayName);
                    }

                    if (enumerator.Current.Keys["EditorSuffix"] != null)
                        EditorSuffixes.TryAdd(sectionName, enumerator.Current.Keys["EditorSuffix"]);
                }

                DisplayNames.TryGetValue(sectionName, out string value);
                if (value == null || value == "")
                {
                    DisplayNames.Remove(sectionName);
                    DisplayNames.Add(sectionName, "Unknown");
                }
            }
        }

        private static void AddUnitStringEntries(IniData iniData) // TODO:
        {
            for (int i = 0; i < iniData.Sections.Count; i++)
            {
                var enumerator = iniData.Sections.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    var unitName = new UnitName()
                    {
                        Name = enumerator.Current.Keys["Name"],
                        Propernames = enumerator.Current.Keys["Propernames"],
                        EditorSuffix = enumerator.Current.Keys["EditorSuffix"],
                    };
                    Unit_Names.TryAdd(enumerator.Current.SectionName, unitName);
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
                string value = line.Substring(keyLength + 1, line.Length - 1 - keyLength).Replace("\"", "").Replace("&", "");

                WE_Strings.TryAdd(key, value);
            }
        }
    }
}
