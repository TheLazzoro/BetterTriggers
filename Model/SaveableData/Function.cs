using System.Collections.Generic;

namespace Model.SaveableData
{
    /// <summary>
    /// Things like 'CreateNUnitsAtLoc' or 'TriggerRegisterDeathEvent'
    /// </summary>
    public class Function : Parameter
    {
        public readonly int ParamType = 1; // DO NOT CHANGE
        public List<Parameter> parameters = new List<Parameter>();

        public new Function Clone()
        {
            string identifier = null;
            if (this.identifier != null)
                identifier = new string(this.identifier);

            Function f = new Function();
            f.identifier = identifier;
            f.returnType = new string(returnType);
            List<Parameter> parameters = new List<Parameter>();
            
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
                if(param is Function)
                {
                    var func = (Function)param;
                    cloned = (Function)func.Clone();
                }
                else if (param is Constant)
                {
                    var constant = (Constant)param;
                    cloned = (Constant)constant.Clone();
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
                    var value = (Value)param;
                    cloned = (Value)value.Clone();
                }
                else
                    cloned = (Parameter)param.Clone();

                parameters.Add(cloned);
            }
            f.parameters = parameters;

            return f;
        }

        public string GetIdentifier()
        {
            return identifier;
        }

        public string GetReturnType()
        {
            return returnType;
        }
    }
}
