using BetterTriggers.Models.SaveableData;
using System;

namespace BetterTriggers.Models.Templates
{
    public class ConstantTemplate : ParameterTemplate
    {
        public string value;
        public string name;
        public string codeText;

        public override ConstantTemplate Clone()
        {
            ConstantTemplate clone = new ConstantTemplate();
            clone.codeText = new string(codeText);
            clone.value = new string(value);
            clone.name = new string(name);
            clone.returnType = new string(returnType);

            return clone;
        }

        public override Constant ToParameter()
        {
            Constant constant = new Constant();
            constant.value = new string(this.value);
            return constant;
        }
    }
}
