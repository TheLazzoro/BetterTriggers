using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetterTriggers.Models.EditorData
{
    public class ReturnType : TriggerElement
    {
        private string _type;

        public string Type
        {
            get => _type;
            set
            {
                _type = value;
                OnPropertyChanged();
                DisplayText = "Return Type: " + Locale.Translate(_type);
            }
        }

        public ReturnType(string type = null)
        {
            if(type == null)
                Type = "integer";
            else
                Type = type;

            ElementType = TriggerElementType.None;
            IconImage = Category.Get(TriggerCategory.TC_SCRIPT).Icon;
        }

        public override ReturnType Clone()
        {
            var cloned = new ReturnType();
            cloned.Type = new string(Type);
            return cloned;
        }
    }
}
