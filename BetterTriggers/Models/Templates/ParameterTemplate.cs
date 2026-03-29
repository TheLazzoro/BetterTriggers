using BetterTriggers.Containers;
using BetterTriggers.Models.EditorData;

namespace BetterTriggers.Models.Templates
{
    public class ParameterTemplate
    {
        public string returnType;

        public virtual ParameterTemplate Clone()
        {
            ParameterTemplate clone = new ParameterTemplate();
            clone.returnType = new string(this.returnType);
            return clone;
        }

        public virtual Parameter ToParameter(Project project)
        {
            return new Parameter();
        }
    }
}
