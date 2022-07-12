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

        public static List<Types> GetGlobalTypes()
        {
            return types.
                Where(varType => varType.Value.CanBeGlobal).
                Select(varType => varType.Value).ToList();
        }

        internal static string GetBaseType(string type)
        {
            Types varType;
            types.TryGetValue(type, out varType);
            if (varType == null || varType.BaseType == null)
                return type;

            return varType.BaseType;
        }
    }
}
