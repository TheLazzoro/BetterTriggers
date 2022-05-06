using Model.SaveableData;
using System;
using System.Collections.Generic;
using System.Text;

namespace Model.SaveableData
{
    public class Trigger
    {
        public int Id;
        public List<TriggerElement> Events = new List<TriggerElement>();
        public List<TriggerElement> Conditions = new List<TriggerElement>();
        public List<TriggerElement> Actions = new List<TriggerElement>();

        internal Trigger Clone()
        {
            Trigger cloned = new Trigger();
            this.Events.ForEach(element => cloned.Events.Add(element.Clone()));
            this.Conditions.ForEach(element => cloned.Conditions.Add(element.Clone()));
            this.Actions.ForEach(element => cloned.Actions.Add(element.Clone()));

            return cloned;
        }
    }
}
