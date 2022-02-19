using Model.SaveableData;
using System;
using System.Collections.Generic;
using System.Text;

namespace Model.SaveableData
{
    public class Trigger
    {
        public int Id;
        public List<Function> Events = new List<Function>();
        public List<Function> Conditions = new List<Function>();
        public List<Function> Actions = new List<Function>();
    }
}
