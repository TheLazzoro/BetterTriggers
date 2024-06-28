using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Automation;

namespace BetterTriggers.Models.EditorData.TriggerEditor
{
    public class ConditionDefinitionRef : ECA, IReferable
    {
        public int ConditionDefinitionId;

        public override ConditionDefinitionRef Clone()
        {
            var cloned = new ConditionDefinitionRef();
            cloned.function = this.function.Clone();
            cloned.ConditionDefinitionId = ConditionDefinitionId;
            cloned.ElementType = ElementType;
            cloned.IconImage = new byte[IconImage.Length];
            IconImage.CopyTo(cloned.IconImage, 0);

            return cloned;
        }
    }
}
