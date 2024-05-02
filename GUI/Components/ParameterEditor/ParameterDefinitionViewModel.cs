using BetterTriggers.Models.EditorData;
using GUI.Components.Shared;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xceed.Wpf.Toolkit.Primitives;

namespace GUI.Components.ParameterEditor
{
    public class ParameterDefinitionViewModel : ViewModelBase
    {
        private ParameterDefinition _definition;
        private string _name;
        private War3Type _selectedItem;

        public bool SuppressChangedEvents { get; private set; }

        public string IdentifierName
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged();
            }
        }
        public ObservableCollection<War3Type> War3Types { get => War3Type.War3Types; }
        public War3Type SelectedItem
        {
            get => _selectedItem;
            set
            {
                _selectedItem = value;
                OnPropertyChanged();
            }
        }

        public ParameterDefinitionViewModel(ParameterDefinition definition)
        {
            _definition = definition;
            _definition.PropertyChanged += _definition_PropertyChanged;
            _definition_PropertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(string.Empty)); // init
        }

        private void _definition_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            SuppressChangedEvents = true;
            _name = _definition.GetIdentifierName();
            SelectedItem = _definition.ReturnType;
            SuppressChangedEvents = false;
        }
    }
}
