using BetterTriggers.Containers;

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

        public ReturnType(Project project, string type = null) : base(project)
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
            var cloned = new ReturnType(_project);
            cloned.War3Type = War3Type;
            return cloned;
        }
    }
}
