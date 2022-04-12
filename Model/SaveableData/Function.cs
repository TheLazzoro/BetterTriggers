using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.SaveableData
{
    /// <summary>
    /// Things like 'CreateNUnitsAtLoc' or 'TriggerRegisterDeathEvent'
    /// </summary>
    public class Function : Parameter, ICloneable
    {
        public readonly int ParamType = 1; // DO NOT CHANGE
        public bool IsEnabled = true;
        public List<Parameter> parameters = new List<Parameter>();

        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}
