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
                /* This thing could be reduced to a simple interface,
                 * but JSON deserialization becomes a problem
                 * if we change the parameter list from 'Parameter' to 'IParameter',
                 * because the deserializer thinks it must create an 'IParameter'
                 * instance, which is illegal.
                 */
                Parameter param = this.parameters[i];
                Parameter cloned;
                if (param is Function)
                {
                    var func = (Function)param;
                    cloned = (Function)func.Clone();
                }
                else if (param is Preset)
                {
                    var constant = (Preset)param;
                    cloned = (Preset)constant.Clone();
                }
                else if (param is VariableRef)
                {
                    var varRef = (VariableRef)param;
                    cloned = (VariableRef)varRef.Clone();
                }
                else if (param is TriggerRef)
                {
                    var triggerRef = (TriggerRef)param;
                    cloned = (TriggerRef)triggerRef.Clone();
                }
                else if (param is Value)
                {
                    var val = (Value)param;
                    cloned = (Value)val.Clone();
                }
                else
                    cloned = (Parameter)param.Clone();

                parameters.Add(cloned);
            }
            f.parameters = parameters;

            return f;
        }
    }
}
