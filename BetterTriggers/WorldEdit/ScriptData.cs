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

            public ScriptItem(string keyword, ScriptItemType type)
            {
                this.displayText = keyword;
                switch (type)
                {
                    case ScriptItemType.Native:
                        Natives.Add(keyword, this);
                        break;
                    case ScriptItemType.Constant:
                        Constants.Add(keyword, this);
                        break;
                    case ScriptItemType.Typeword:
                        TypewordsJass.Add(keyword, this);
                        break;
                    case ScriptItemType.Jass:
                        KeywordsJass.Add(keyword, this);
                        break;
                    case ScriptItemType.Lua:
                        KeywordsLua.Add(keyword, this);
                        break;
                    default:
                        break;
                }
            }
        }

        public enum ScriptItemType
        {
            Typeword,
            Native,
            Constant,
            Jass,
            Lua
        }

        public static Dictionary<string, ScriptItem> TypewordsJass = new();
        public static Dictionary<string, ScriptItem> KeywordsJass = new();
        public static Dictionary<string, ScriptItem> KeywordsLua = new();
        public static Dictionary<string, ScriptItem> Natives = new();
        public static Dictionary<string, ScriptItem> Constants = new();


        public static List<ScriptItem> GetAll(ScriptLanguage language)
        {
            List<ScriptItem> list = new List<ScriptItem>();
            if (language == ScriptLanguage.Jass)
            {
                list.AddRange(KeywordsJass.Select(x => x.Value).ToList());
                list.AddRange(TypewordsJass.Select(x => x.Value).ToList());
            }
            else
                list.AddRange(KeywordsLua.Select(x => x.Value).ToList());

            list.AddRange(Constants.Select(x => x.Value).ToList());
            list.AddRange(Natives.Select(x => x.Value).ToList());
            return list;
        }

        public static string GetDescription(string key)
        {
            ScriptItem item;
            if (Natives.TryGetValue(key, out item))
                return item.description;
            else if (Constants.TryGetValue(key, out item))
                return item.description;
            else if (TypewordsJass.TryGetValue(key, out item))
                return item.description;
            else if (KeywordsJass.TryGetValue(key, out item))
                return item.description;
            else if (KeywordsLua.TryGetValue(key, out item))
                return item.description;

            return string.Empty;
        }

        internal static void Load(bool isTest)
        {
            if (isTest)
                return;

            Natives.Clear();
            Constants.Clear();
            TypewordsJass.Clear();
            KeywordsJass.Clear();
            KeywordsLua.Clear();

            LoadJassPrimitives();
            LoadKeywords();
            LoadCommon();
            LoadBlizzardJ();
        }

        private static void LoadJassPrimitives()
        {
            string pathPrimitives = Path.Combine(Directory.GetCurrentDirectory(), "Resources/PrimitiveTypesJass.txt");
            string[] keywordsPrim = File.ReadAllLines(pathPrimitives);
            for (int i = 0; i < keywordsPrim.Length; i++)
            {
                new ScriptItem(keywordsPrim[i], ScriptItemType.Typeword);
            }
        }

        private static void LoadKeywords()
        {
            string pathJass = Path.Combine(Directory.GetCurrentDirectory(), "Resources/KeywordsJass.txt");
            string pathLua = Path.Combine(Directory.GetCurrentDirectory(), "Resources/KeywordsLua.txt");
            string[] keywordsJass = File.ReadAllLines(pathJass);
            string[] keywordsLua = File.ReadAllLines(pathLua);
            for (int i = 0; i < keywordsJass.Length; i++)
            {
                new ScriptItem(keywordsJass[i], ScriptItemType.Jass);
            }
            for (int i = 0; i < keywordsLua.Length; i++)
            {
                new ScriptItem(keywordsLua[i], ScriptItemType.Lua);
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
                new ScriptItem(type[1], ScriptItemType.Typeword)
                {
                    description = types[i],
                };
            }

            // Constant Natives
            for (int i = 0; i < constantNatives.Count; i++)
            {
                string[] constantNative = constantNatives[i].Split(" ");
                new ScriptItem(constantNative[2], ScriptItemType.Native)
                {
                    description = constantNatives[i],
                };
            }

            // Constants
            for (int i = 0; i < constants.Count; i++)
            {
                string[] constant = constants[i].Split(" ");
                new ScriptItem(constant[2], ScriptItemType.Constant)
                {
                    description = constants[i],
                };
            }

            // Natives
            for (int i = 0; i < natives.Count; i++)
            {
                string[] native = natives[i].Split(" ");
                new ScriptItem(native[1], ScriptItemType.Native)
                {
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
                new ScriptItem(constant[2], ScriptItemType.Constant)
                {
                    description = constants[i],
                };
            }

            // Functions
            for (int i = 0; i < functions.Count; i++)
            {
                string[] function = functions[i].Split(" ");
                new ScriptItem(function[1], ScriptItemType.Native)
                {
                    description = functions[i],
                };
            }
        }
    }
}
