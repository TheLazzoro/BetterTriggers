using System.Collections.Generic;

namespace BetterTriggers.Models.SaveableData
{
    /// <summary>
    /// Things like 'CreateNUnitsAtLoc' or 'TriggerRegisterDeathEvent'
    /// </summary>
    public class Function_Saveable : Parameter_Saveable
    {
        public readonly int ParamType = 1; // DO NOT CHANGE
        public List<Parameter_Saveable> parameters = new List<Parameter_Saveable>();
    }
}
