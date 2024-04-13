using BetterTriggers.Models.EditorData;
using BetterTriggers.WorldEdit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetterTriggers.Models.EditorData
{
    public class ParameterDefinition : TriggerElement, IReferable
    {
        public int Id;
        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged();
                var type = Types.Get(ReturnType);
                DisplayText = _name;
                SuffixText = $"<{Locale.Translate(type.DisplayName)}>";
            }
        }
        public string ReturnType;

        private string _name;

        public ParameterDefinition()
        {
            ReturnType = "integer";
            ElementType = TriggerElementType.ParameterDef;
            IconImage = Category.Get(TriggerCategory.TC_PARAMETER).Icon;
        }

        public override ParameterDefinition Clone()
        {
            var clone = new ParameterDefinition();
            clone.ReturnType = new string(ReturnType);
            clone.DisplayText = new string(DisplayText);
            IconImage.CopyTo(clone.IconImage, 0);

            return clone;
        }
    }
}
