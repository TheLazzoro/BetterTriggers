using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Automation;

namespace BetterTriggers.Models.SaveableData
{
    public class ParameterDefinitionRef_Saveable : Parameter_Saveable
    {
        public readonly int ParamType = 7; // DO NOT CHANGE
        public int ParameterDefinitionId;
    }
}
