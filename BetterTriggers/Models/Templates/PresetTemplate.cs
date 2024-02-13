using BetterTriggers.Models.EditorData;
using System;

namespace BetterTriggers.Models.Templates
{
    public class PresetTemplate : ParameterTemplate
    {
        public string value;
        public string name;
        public string codeText;

        public override PresetTemplate Clone()
        {
            PresetTemplate clone = new PresetTemplate();
            clone.codeText = new string(codeText);
            clone.value = new string(value);
            clone.name = new string(name);
            clone.returnType = new string(returnType);

            return clone;
        }

        public override Preset ToParameter()
        {
            Preset constant = new Preset();
            constant.value = new string(this.value);
            return constant;
        }
    }
}
