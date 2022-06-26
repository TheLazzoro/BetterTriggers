using System.Collections.Generic;

namespace BetterTriggers.Models.SaveableData
{
    public class OrMultiple : Function
    {
        public readonly int ParamType = 7; // DO NOT CHANGE
        public List<TriggerElement> Or = new List<TriggerElement>();
        
        public new OrMultiple Clone()
        {
            OrMultiple orMultiple = new OrMultiple();
            orMultiple.identifier = new string(identifier);
            orMultiple.returnType = new string(returnType);
            orMultiple.Or = new List<TriggerElement>();
            Or.ForEach(element => orMultiple.Or.Add(element.Clone()));

            return orMultiple;
        }
    }
}
