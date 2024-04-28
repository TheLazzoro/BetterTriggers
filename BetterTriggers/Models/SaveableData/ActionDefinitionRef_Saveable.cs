using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Automation;

namespace BetterTriggers.Models.SaveableData
{
    public class ActionDefinitionRef_Saveable : ECA_Saveable
    {
        public readonly int ElementType = 13; // DO NOT CHANGE
        public int ActionDefinitionId;
        public List<Parameter_Saveable> Parameters = new();
    }
}
