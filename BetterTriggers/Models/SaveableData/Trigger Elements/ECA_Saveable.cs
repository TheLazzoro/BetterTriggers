using BetterTriggers.JsonBaseConverter;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace BetterTriggers.Models.SaveableData
{
    public class ECA_Saveable : TriggerElement_Saveable
    {
        public bool isEnabled = true;
        public Function_Saveable function = new Function_Saveable();

        public ECA_Saveable() { }
        public ECA_Saveable(string value)
        {
            this.function.value = value;
        }

        public override ECA_Saveable Clone()
        {
            ECA_Saveable clone = new ECA_Saveable();
            clone.isEnabled = isEnabled;
            clone.function = function.Clone();

            return clone;
        }
    }
}
