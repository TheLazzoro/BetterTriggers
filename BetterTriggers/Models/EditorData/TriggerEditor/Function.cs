using BetterTriggers.Containers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetterTriggers.Models.EditorData
{
    /// <summary>
    /// Things like 'CreateNUnitsAtLoc' or 'TriggerRegisterDeathEvent'
    /// </summary>
    public class Function : Parameter
    {
        public List<Parameter> parameters { get; set; } = new();

        public override Function Clone()
        {
            string value = null;
            if (this.value != null)
                value = new string(this.value);

            Function f = new Function();
            f.value = value;
            List<Parameter> parameters = new();

            for (int i = 0; i < this.parameters.Count; i++)
            {
                Parameter param = this.parameters[i];
                Parameter cloned;
                if (param is Function func)
                {
                    cloned = func.Clone();
                }
                else if (param is Preset preset)
                {
                    cloned = preset.Clone();
                }
                else if (param is VariableRef varRef)
                {
                    cloned = varRef.Clone();
                }
                else if (param is TriggerRef triggerRef)
                {
                    cloned = triggerRef.Clone();
                }
                else if (param is Value val)
                {
                    cloned = val.Clone();
                }
                else
                    cloned = param.Clone();

                parameters.Add(cloned);
            }
            f.parameters = parameters;

            return f;
        }
    }
}
