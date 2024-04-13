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
                DisplayText = _name;
                SuffixText = $"<{Locale.Translate(ReturnType.DisplayName)}>";
            }
        }
        public War3Type ReturnType
        {
            get => _returnType;
            set
            {
                _returnType = value;
                OnPropertyChanged();
                SuffixText = $"<{Locale.Translate(ReturnType.DisplayName)}>";
            }
        }

        private string _name;
        private War3Type _returnType;

        public ParameterDefinition()
        {
            ReturnType = War3Type.Get("integer");
            ElementType = TriggerElementType.ParameterDef;
            IconImage = Category.Get(TriggerCategory.TC_PARAMETER).Icon;
        }

        public override ParameterDefinition Clone()
        {
            var clone = new ParameterDefinition();
            clone.ReturnType = ReturnType;
            clone.DisplayText = new string(DisplayText);
            IconImage.CopyTo(clone.IconImage, 0);

            return clone;
        }

        public string GetIdentifierName()
        {
            return "param_" + Name.Replace(" ", "_");
        }
    }
}
