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

        public string IdentifierName
        {
            get => _definition.GetIdentifierName();
        }
        public ObservableCollection<War3Type> War3Types { get => War3Type.War3Types; }
        public War3Type SelectedItem
        {
            get => _definition.ReturnType;
        }

        public ParameterDefinitionViewModel(ParameterDefinition definition)
        {
            _definition = definition;
            _definition.PropertyChanged += _definition_PropertyChanged;
        }

        private void _definition_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(_definition.ReturnType))
                OnPropertyChanged(nameof(SelectedItem));
        }
    }
}
