using BetterTriggers.JsonBaseConverter;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace BetterTriggers.Models.SaveableData
{
    public class ECA : TriggerElement
    {
        public bool isEnabled = true;
        public Function function = new Function();

        public ECA() { }
        public ECA(string value)
        {
            this.function.value = value;
        }

        public override ECA Clone()
        {
            ECA clone = new ECA();
            clone.isEnabled = isEnabled;
            clone.function = function.Clone();

            return clone;
        }
    }
}
