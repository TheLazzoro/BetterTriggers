using BetterTriggers.JsonBaseConverter;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace BetterTriggers.Models.SaveableData
{
    public class TriggerRef_Saveable : Parameter_Saveable
    {
        public readonly int ParamType = 4; // DO NOT CHANGE
        public int TriggerId;
    }
}
