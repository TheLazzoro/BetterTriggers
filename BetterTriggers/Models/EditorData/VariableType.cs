using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetterTriggers.Models.EditorData
{
    public class VariableType
    {
        private static List<VariableType> variableTypes = new List<VariableType>();
        
        public string Key { get; }
        public bool CanBeGlobal { get; }
        public bool CanBeCompared { get; }
        public string DisplayName { get; }
        public string BaseType { get; }

        public VariableType(string key, bool canBeGlobal, bool canBeCompared, string displayName, string baseType)
        {
            this.Key = key;
            this.CanBeGlobal = canBeGlobal;
            this.CanBeCompared = canBeCompared;
            this.DisplayName = displayName;
            this.BaseType = baseType;
        }

        public static void Create(string key, bool canBeGlobal, bool canBeCompared, string displayName, string baseType)
        {
            if (!canBeGlobal)
                return;

            VariableType variableType = new VariableType(key, canBeGlobal, canBeCompared, displayName, baseType);
            variableTypes.Add(variableType);
        }

        public static List<VariableType> GetAll()
        {
            return variableTypes;
        }
    }
}
