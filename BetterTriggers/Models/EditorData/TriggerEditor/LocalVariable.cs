using BetterTriggers.Containers;
using BetterTriggers.WorldEdit;
using System.ComponentModel;

namespace BetterTriggers.Models.EditorData
{
    public class LocalVariable : TriggerElement
    {
        public Variable variable { get; }

        public LocalVariable(Project project, Variable variable) : base(project)
        {
            ElementType = TriggerElementType.LocalVariable;
            this.variable = variable;
            variable._isLocal = true;
            variable.IsArray = false; // forces locals to be non-arrays
            variable.PropertyChanged += Variable_PropertyChanged;
            Variable_PropertyChanged(null, new PropertyChangedEventArgs(DisplayText)); // Init
        }

        private void Variable_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            var type = Types.Get(variable.War3Type.Type);

            DisplayText = variable.Name;
            SuffixText = $"<{Locale.Translate(type.DisplayName)}>";
        }

        public override LocalVariable Clone()
        {
            LocalVariable clone = new LocalVariable(_project, variable.Clone());
            clone.DisplayText = new string(DisplayText);
            clone.variable._isLocal = true;
            clone.ElementType = ElementType;
            clone.IconImage = new byte[IconImage.Length];
            IconImage.CopyTo(clone.IconImage, 0);

            return clone;
        }
    }
}
