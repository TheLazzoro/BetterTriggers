using BetterTriggers.WorldEdit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetterTriggers.Models.EditorData
{
    public class LocalVariable : TriggerElement
    {
        public Variable variable = new Variable();

        public LocalVariable()
        {
            variable._isLocal = true;
            variable.IsArray = false; // forces locals to be non-arrays
            variable.PropertyChanged += Variable_PropertyChanged;
        }

        private void Variable_PropertyChanged()
        {
            var type = Types.Get(variable.Type);

            DisplayText = variable.Name;
            SuffixText = $"<{Locale.Translate(type.DisplayName)}>";
        }

        public override LocalVariable Clone()
        {
            LocalVariable clone = new LocalVariable();
            clone.DisplayText = new string(DisplayText);
            clone.variable = variable.Clone();
            clone.variable._isLocal = true;
            clone.ElementType = ElementType;
            clone.IconImage = new byte[IconImage.Length];
            IconImage.CopyTo(clone.IconImage, 0);

            return clone;
        }
    }
}
