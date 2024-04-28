using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetterTriggers.WorldEdit
{
    public class Types
    {
        private static Dictionary<string, Types> types = new Dictionary<string, Types>();
        
        public string Key { get; }
        public bool CanBeGlobal { get; }
        public bool CanBeCompared { get; }
        public string DisplayName { get; }
        public string BaseType { get; }
        public string Extends { get; internal set; } = string.Empty;

        public Types(string key, bool canBeGlobal, bool canBeCompared, string displayName, string baseType)
        {
            this.Key = key;
            this.CanBeGlobal = canBeGlobal;
            this.CanBeCompared = canBeCompared;
            this.DisplayName = displayName;
            this.BaseType = baseType;
        }

        public static void Create(string key, bool canBeGlobal, bool canBeCompared, string displayName, string baseType)
        {
            // Some basetypes from triggerdata.txt are wrong. Gj Blizz.
            if (key == "unitstatemethod" ||
                key == "checkingignoringoption"
                ) 
                baseType = key;

            Types variableType = new Types(key, canBeGlobal, canBeCompared, displayName, baseType);
            types.Add(key, variableType);
        }

        public static Types Get(string type)
        {
            Types t;
            types.TryGetValue(type, out t);
            return t;
        }

        public static bool IsHandle(string type)
        {
            bool isHandle = false;
            Types t;
            types.TryGetValue(type, out t);
            while(t != null)
            {
                if (t.Extends == "handle")
                {
                    isHandle = true;
                    break;
                }

                types.TryGetValue(t.Extends, out t);
            }

            return isHandle;
        }
        
        public static List<Types> GetGlobalTypes()
        {
            return types.
                Where(varType => varType.Value.CanBeGlobal).
                Select(varType => varType.Value).ToList();
        }

        public static string GetDisplayName(string type)
        {
            string displayName = "MISSING STRING!";
            Types t;
            types.TryGetValue(type, out t);
            return Locale.Translate(t.DisplayName);
        }

        public static string GetBaseType(string type)
        {
            Types varType;
            types.TryGetValue(type, out varType);
            if (varType == null || varType.BaseType == null)
                return type;

            return varType.BaseType;
        }

        /// <summary>
        /// Only used for testing.
        /// </summary>
        internal static void Clear()
        {
            types.Clear();
        }

        public static bool AreTypesEqual(string type1, string type2)
        {
            bool areEqual = false;
            if ((type1 == "VarAsString_Real" && type2 == "real") || (type2 == "VarAsString_Real" && type1 == "real"))
                areEqual = true;
            else
                areEqual = type1 == type2;

            return areEqual;
        }
    }
}
