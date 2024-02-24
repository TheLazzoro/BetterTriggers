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
        private string _identifier;
        private bool _size1Enabled;
        private bool _dimensions;
        private static ObservableCollection<War3Type> _war3Types;

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
            set
            {
                if (_variable._isLocal)
                    _identifier = "udl_" + value;
                else
                    _identifier = "udg_" + value;
                OnPropertyChanged();
            }
        }
        public ObservableCollection<War3Type> War3Types { get => _war3Types; }
        public int SelectedIndex { get; set; }
        public bool IsArray
        {
            get => _variable.IsArray;
            set
            {
                _variable.IsArray = value;
                OnPropertyChanged();
            }
        }
        public int Dimensions
        {
            get
            {
                if (_variable.IsArray && _variable.IsTwoDimensions)
                    return 2;
                else
                    return 1;
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
        public bool Size1Enabled { get => Dimensions == 2; }
        public ObservableCollection<Inline> InitialValueParamText { get; private set; } = new();
        public ObservableCollection<TreeNodeBase> ReferenceTriggers { get; set; } = new();

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
                }
            }

            foreach (War3Type item in _war3Types)
            {
                if (item.Type == variable.Type)
                {
                    SelectedIndex = _war3Types.IndexOf(item);
                    break;
                }
            }

            _variable = variable;
            variable.PropertyChanged += Variable_PropertyChanged;
            Variable_PropertyChanged(); // init
        }

        private void Variable_PropertyChanged()
        {
            OnPropertyChanged();

            Identifier = _variable.Name;


            InitialValueParamText.Clear();
            ParamTextBuilder paramTextBuilder = new ParamTextBuilder();
            var inlines = paramTextBuilder.GenerateParamText(_variable);
            InitialValueParamText.AddRange(inlines);
        }
    }
}
