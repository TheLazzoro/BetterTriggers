using BetterTriggers.WorldEdit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace BetterTriggers.Models.EditorData
{
    public class LocalVariable : TriggerElement
    {
        public Variable variable { get; }

        public LocalVariable(Variable variable)
        {
            this.variable = variable;
            variable._isLocal = true;
            variable.IsArray = false; // forces locals to be non-arrays
            variable.PropertyChanged += Variable_PropertyChanged;
            Variable_PropertyChanged(null, new PropertyChangedEventArgs(DisplayText)); // Init
        }

        private void Variable_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            var type = Types.Get(variable.Type);

            DisplayText = variable.Name;
            SuffixText = $"<{Locale.Translate(type.DisplayName)}>";
        }

        public override LocalVariable Clone()
        {
            LocalVariable clone = new LocalVariable(variable.Clone());
            clone.DisplayText = new string(DisplayText);
            clone.variable._isLocal = true;
            clone.ElementType = ElementType;
            clone.IconImage = new byte[IconImage.Length];
            IconImage.CopyTo(clone.IconImage, 0);

            return clone;
        }
    }
}
