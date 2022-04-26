using Model.EditorData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.SaveableData
{
    public class ForGroupMultiple : Function
    {
        public readonly int ParamType = 8; // DO NOT CHANGE
        public List<TriggerElement> Actions = new List<TriggerElement>();
        
        public ForGroupMultiple Clone()
        {
            ForGroupMultiple forGroupMultiple = new ForGroupMultiple();
            forGroupMultiple.identifier = new string(identifier);
            forGroupMultiple.returnType = new string(returnType);
            forGroupMultiple.Actions = new List<TriggerElement>();
            Actions.ForEach(element => forGroupMultiple.Actions.Add(element.Clone()));

            return forGroupMultiple;
        }
    }
}
