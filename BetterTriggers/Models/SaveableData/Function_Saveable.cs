using System.Collections.Generic;

namespace BetterTriggers.Models.SaveableData
{
    /// <summary>
    /// Things like 'CreateNUnitsAtLoc' or 'TriggerRegisterDeathEvent'
    /// </summary>
    public class Function_Saveable : Parameter_Saveable
    {
        public readonly int ParamType = 1; // DO NOT CHANGE
        public List<Parameter_Saveable> parameters = new List<Parameter_Saveable>();

        public override Function_Saveable Clone()
        {
            string value = null;
            if (this.value != null)
                value = new string(this.value);

            Function_Saveable f = new Function_Saveable();
            f.value = value;
            List<Parameter_Saveable> parameters = new List<Parameter_Saveable>();
            
            for (int i = 0; i < this.parameters.Count; i++)
            {
                /* This thing could be reduced to a simple interface,
                 * but JSON deserialization becomes a problem
                 * if we change the parameter list from 'Parameter' to 'IParameter',
                 * because the deserializer thinks it must create an 'IParameter'
                 * instance, which is illegal.
                 */
                Parameter_Saveable param = this.parameters[i];
                Parameter_Saveable cloned;
                if(param is Function_Saveable)
                {
                    var func = (Function_Saveable)param;
                    cloned = (Function_Saveable)func.Clone();
                }
                else if (param is Constant_Saveable)
                {
                    var constant = (Constant_Saveable)param;
                    cloned = (Constant_Saveable)constant.Clone();
                }
                else if (param is VariableRef_Saveable)
                {
                    var varRef = (VariableRef_Saveable)param;
                    cloned = (VariableRef_Saveable)varRef.Clone();
                }
                else if (param is TriggerRef_Saveable)
                {
                    var triggerRef = (TriggerRef_Saveable)param;
                    cloned = (TriggerRef_Saveable)triggerRef.Clone();
                }
                else if (param is Value_Saveable)
                {
                    var val = (Value_Saveable)param;
                    cloned = (Value_Saveable)val.Clone();
                }
                else
                    cloned = (Parameter_Saveable)param.Clone();

                parameters.Add(cloned);
            }
            f.parameters = parameters;

            return f;
        }
    }
}
