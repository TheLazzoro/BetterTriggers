using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Automation;

namespace BetterTriggers.Models.SaveableData
{
    public class FunctionDefinitionRef_Saveable : Function_Saveable
    {
        public readonly int ParamType = 6; // DO NOT CHANGE
        public int FunctionDefinitionId;
    }
}
