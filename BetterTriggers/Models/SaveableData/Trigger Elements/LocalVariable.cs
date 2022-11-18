using BetterTriggers.JsonBaseConverter;
using BetterTriggers.Models.EditorData;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace BetterTriggers.Models.SaveableData
{
    public class LocalVariable : TriggerElement
    {
        public readonly int LocalVar = 1; // DO NOT CHANGE
        public Variable variable = new Variable();
        
        [JsonIgnore]
        private List<ITriggerElementUI> triggerElementUIs = new List<ITriggerElementUI>();

        public LocalVariable() { }

        public virtual LocalVariable Clone()
        {
            LocalVariable clone = new LocalVariable();
            clone.variable = (Variable)variable.Clone();

            return clone;
        }

        public void Attach(ITriggerElementUI elementUI)
        {
            triggerElementUIs.Add(elementUI);
        }

        public void Detach(ITriggerElementUI elementUI)
        {
            triggerElementUIs.Remove(elementUI);
        }

        
    }
}
