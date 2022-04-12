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
    }
}
