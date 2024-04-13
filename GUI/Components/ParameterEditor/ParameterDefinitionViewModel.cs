using BetterTriggers.Models.EditorData;
using GUI.Components.Shared;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GUI.Components.ParameterEditor
{
    public class ParameterDefinitionViewModel : ViewModelBase
    {
        private ParameterDefinition _definition;
        private War3Type _selectedItem;

        public event Action OnChanged;
        public string IdentifierName
        {
            get => _definition.GetIdentifierName();
        }
        public ObservableCollection<War3Type> War3Types { get => War3Type.War3Types; }
        public War3Type SelectedItem
        {
            get => _selectedItem;
            set
            {
                _selectedItem = value;
                _definition.ReturnType = value;
                OnPropertyChanged();
                OnChanged?.Invoke();
            }
        }

        public ParameterDefinitionViewModel(ParameterDefinition definition)
        {
            _definition = definition;
            for (int i = 0; i < War3Types.Count; i++)
            {
                var type = War3Types[i];
                if (definition.ReturnType.Type == type.Type)
                {
                    SelectedItem = War3Types[i];
                    break;
                }
            }
        }
    }
}
