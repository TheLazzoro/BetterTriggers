using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Automation;

namespace BetterTriggers.Models.EditorData.TriggerEditor
{
    public class ActionDefinitionRef : ECA, IReferable
    {
        public int ActionDefinitionId;

        public override ActionDefinitionRef Clone()
        {
            var cloned = new ActionDefinitionRef();
            cloned.function = this.function.Clone();
            cloned.ActionDefinitionId = ActionDefinitionId;
            cloned.ElementType = ElementType;
            cloned.IconImage = new byte[IconImage.Length];
            IconImage.CopyTo(cloned.IconImage, 0);

            return cloned;
        }
    }
}
