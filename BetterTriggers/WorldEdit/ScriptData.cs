using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using War3Net.Build.Info;

namespace BetterTriggers.WorldEdit
{
    public static class ScriptData
    {
        public class ScriptItem
        {
            public string displayText { get; internal set; }
            public string description { get; internal set; }

            public ScriptItem(ScriptItemType type)
            {
                switch (type)
                {
                    case ScriptItemType.All:
                        Natives.Add(this);
                        break;
                    case ScriptItemType.Jass:
                        KeywordsJass.Add(this);
                        break;
                    case ScriptItemType.Lua:
                        KeywordsLua.Add(this);
                        break;
                    default:
                        break;
                }

            }
        }

        public enum ScriptItemType
        {
            All,
            Jass,
            Lua
        }

        private static List<ScriptItem> KeywordsJass = new List<ScriptItem>();
        private static List<ScriptItem> KeywordsLua = new List<ScriptItem>();
        private static List<ScriptItem> Natives = new List<ScriptItem>();

        public static List<ScriptItem> Get(ScriptLanguage language)
        {
            List<ScriptItem> list = new List<ScriptItem>();
            if (language == ScriptLanguage.Jass)
                list.AddRange(KeywordsJass);
            else
                list.AddRange(KeywordsLua);

            list.AddRange(Natives);
            return list;
        }

        internal static void Load(bool isTest)
        {
            if (isTest)
                return;

            LoadKeywords();
            LoadCommon();
            LoadBlizzardJ();
        }

        private static void LoadKeywords()
        {
            string pathJass = Path.Combine(Directory.GetCurrentDirectory(), "Resources/KeywordsJass.txt");
            string pathLua = Path.Combine(Directory.GetCurrentDirectory(), "Resources/KeywordsLua.txt");
            string[] keywordsJass = File.ReadAllLines(pathJass);
            string[] keywordsLua = File.ReadAllLines(pathLua);
            for (int i = 0; i < keywordsJass.Length; i++)
            {
                new ScriptItem(ScriptItemType.Jass)
                {
                    displayText = keywordsJass[i]
                };
            }
            for (int i = 0; i < keywordsLua.Length; i++)
            {
                new ScriptItem(ScriptItemType.Lua)
                {
                    displayText = keywordsLua[i]
                };
            }
        }

        private static void LoadCommon()
        {
            string[] commonJ = File.ReadAllLines(TriggerData.pathCommonJ);
            List<string> types = new List<string>();
            List<string> constantNatives = new List<string>();
            List<string> constants = new List<string>();
            List<string> natives = new List<string>();
            for (int i = 0; i < commonJ.Length; i++)
            {
                commonJ[i] = Regex.Replace(commonJ[i], @"\s+", " ").Trim();
                if (commonJ[i].StartsWith("type"))
                {
                    types.Add(commonJ[i]);
                }
                else if (commonJ[i].StartsWith("constant native"))
                {
                    constantNatives.Add(commonJ[i]);
                }
                else if (commonJ[i].StartsWith("constant"))
                {
                    constants.Add(commonJ[i]);
                }
                else if (commonJ[i].StartsWith("native"))
                {
                    natives.Add(commonJ[i]);
                }
            }

            // Types
            for (int i = 0; i < types.Count; i++)
            {
                string[] type = types[i].Split(" ");
                new ScriptItem(ScriptItemType.Jass)
                {
                    displayText = type[1],
                    description = types[i],
                };
            }

            // Constant Natives
            for (int i = 0; i < constantNatives.Count; i++)
            {
                string[] constantNative = constantNatives[i].Split(" ");
                new ScriptItem(ScriptItemType.All)
                {
                    displayText = constantNative[2],
                    description = constantNatives[i],
                };
            }

            // Constants
            for (int i = 0; i < constants.Count; i++)
            {
                string[] constant = constants[i].Split(" ");
                new ScriptItem(ScriptItemType.All)
                {
                    displayText = constant[2],
                    description = constants[i],
                };
            }

            // Natives
            for (int i = 0; i < natives.Count; i++)
            {
                string[] native = natives[i].Split(" ");
                new ScriptItem(ScriptItemType.All)
                {
                    displayText = native[1],
                    description = natives[i],
                };
            }
        }

        private static void LoadBlizzardJ()
        {
            string[] blizzardJ = File.ReadAllLines(TriggerData.pathBlizzardJ);
            List<string> constants = new List<string>();
            List<string> functions = new List<string>();
            for (int i = 0; i < blizzardJ.Length; i++)
            {
                blizzardJ[i] = Regex.Replace(blizzardJ[i], @"\s+", " ").Trim();
                if (blizzardJ[i].StartsWith("constant"))
                {
                    constants.Add(blizzardJ[i]);
                }
                else if (blizzardJ[i].StartsWith("function"))
                {
                    functions.Add(blizzardJ[i]);
                }
            }

            // Constants
            for (int i = 0; i < constants.Count; i++)
            {
                string[] constant = constants[i].Split(" ");
                new ScriptItem(ScriptItemType.All)
                {
                    displayText = constant[2],
                    description = constants[i],
                };
            }

            // Functions
            for (int i = 0; i < functions.Count; i++)
            {
                string[] function = functions[i].Split(" ");
                new ScriptItem(ScriptItemType.All)
                {
                    displayText = function[1],
                    description = functions[i],
                };
            }
        }
    }
}
