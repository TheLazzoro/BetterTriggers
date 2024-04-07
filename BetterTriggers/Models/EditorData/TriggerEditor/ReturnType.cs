using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetterTriggers.Models.EditorData
{
    public class ReturnType : TriggerElement
    {
        private War3Type _type;

        public War3Type War3Type
        {
            get => _type;
            set
            {
                _type = value;
                OnPropertyChanged();
                DisplayText = "Return Type: " + _type.DisplayName;
            }
        }

        public ReturnType(string type = null)
        {
            if(type == null)
                War3Type = War3Type.Get("integer");
            else
                War3Type = War3Type.Get(type);

            ElementType = TriggerElementType.None;
            IconImage = Category.Get(TriggerCategory.TC_SCRIPT).Icon;
        }

        public override ReturnType Clone()
        {
            var cloned = new ReturnType();
            cloned.War3Type = War3Type;
            return cloned;
        }
    }
}
