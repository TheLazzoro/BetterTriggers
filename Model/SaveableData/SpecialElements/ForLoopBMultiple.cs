using Model.EditorData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.SaveableData
{
    public class ForLoopBMultiple : Function
    {
        public readonly int ParamType = 11; // DO NOT CHANGE
        public List<TriggerElement> Actions = new List<TriggerElement>();
        
        public ForLoopBMultiple Clone()
        {
            ForLoopBMultiple forLoopBMultiple = new ForLoopBMultiple();
            forLoopBMultiple.identifier = new string(identifier);
            forLoopBMultiple.returnType = new string(returnType);
            forLoopBMultiple.Actions = new List<TriggerElement>();
            Actions.ForEach(element => forLoopBMultiple.Actions.Add(element.Clone()));

            return forLoopBMultiple;
        }
    }
}
