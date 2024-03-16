using BetterTriggers;
using BetterTriggers.Models.EditorData;
using BetterTriggers.WorldEdit;
using GUI.Components.Shared;
using GUI.Utility;
using NuGet.Packaging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Forms;

namespace GUI.Components.VariableEditor
{
    public class VariableControlViewModel : ViewModelBase
    {
        private Variable _variable;
        private War3Type _selectedItem;
        private static ObservableCollection<War3Type> _war3Types;
        private static War3Type defaultSelection;

        public Visibility IsLocal
        {
            get
            {
                if (_variable._isLocal)
                    return Visibility.Hidden;
                else
                    return Visibility.Visible;
            }
        }
        public string Identifier
        {
            get => _variable.GetIdentifierName();
        }
        public ObservableCollection<War3Type> War3Types { get => _war3Types; }
        public War3Type SelectedItem
        {
            get => _selectedItem;
            set
            {
                _selectedItem = value;
                OnPropertyChanged();
            }
        }
        public War3Type SelectedItemPrevious { get; set; }


        public bool IsArray
        {
            get => _variable.IsArray;
            set
            {
                _variable.IsArray = value;
                OnPropertyChanged();
            }
        }
        public int DimensionsIndex
        {
            get
            {
                return _variable.IsTwoDimensions ? 1 : 0;
            }
            set
            {
                _variable.IsTwoDimensions = value == 1;
                OnPropertyChanged();
            }
        }
        public int Size0
        {
            get => _variable.ArraySize[0];
            set
            {
                _variable.ArraySize[0] = value;
                OnPropertyChanged();
            }
        }
        public int Size1
        {
            get => _variable.ArraySize[1];
            set
            {
                _variable.ArraySize[1] = value;
                OnPropertyChanged();
            }
        }
        public bool IsTwoDimensions
        {
            get => _variable.IsTwoDimensions;
            set
            {
                _variable.IsTwoDimensions = value;
                OnPropertyChanged();
            }
        }
        public ObservableCollection<ExplorerElement> ReferenceTriggers { get; set; } = new();

        public VariableControlViewModel(Variable variable)
        {
            if (_war3Types == null)
            {
                _war3Types = new ObservableCollection<War3Type>();
                List<Types> types = TriggerData.LoadAllVariableTypes();
                for (int i = 0; i < types.Count; i++)
                {
                    string type = types[i].Key;
                    string displayName = Locale.Translate(types[i].DisplayName);
                    War3Type item = new War3Type(type, displayName);

                    _war3Types.Add(item);

                    if (type == "integer")
                        defaultSelection = item;
                }
            }

            foreach (War3Type item in _war3Types)
            {
                if (item.Type == variable.Type)
                {
                    SelectedItem = item;
                    break;
                }
            }
            if (SelectedItem == null)
            {
                SelectedItem = defaultSelection;
            }

            _variable = variable;
            variable.PropertyChanged += Variable_PropertyChanged;
            Variable_PropertyChanged(); // init
        }

        private void Variable_PropertyChanged()
        {
            OnPropertyChanged(nameof(Identifier));
        }
    }
}
