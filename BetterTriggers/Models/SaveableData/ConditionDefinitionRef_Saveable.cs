using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Automation;

namespace BetterTriggers.Models.SaveableData
{
    public class ConditionDefinitionRef_Saveable : ECA_Saveable
    {
        public readonly int ElementType = 14; // DO NOT CHANGE
        public int ConditionDefinitionId;
        public List<Parameter_Saveable> Parameters = new();
    }
}
