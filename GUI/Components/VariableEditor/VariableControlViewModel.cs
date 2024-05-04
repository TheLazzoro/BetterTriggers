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
using System.ComponentModel;
using ICSharpCode.Decompiler.CSharp.Syntax;

namespace GUI.Components.VariableEditor
{
    public class VariableControlViewModel : ViewModelBase
    {
        private string _identifier;
        private Variable _variable;
        private War3Type _selectedType;
        private bool _isArray;
        private bool _isTwoDimensions;
        private int _dimensionsIndex;
        private int _size0;
        private int _size1;

        public bool SuppressChangedEvents { get; private set; }


        public string Identifier
        {
            get => _identifier;
            set
            {
                _identifier = value;
                OnPropertyChanged();
            }
        }
        public ObservableCollection<War3Type> War3Types { get => War3Type.War3Types; }
        public War3Type War3Type
        {
            get => _selectedType;
            set
            {
                _selectedType = value;
                OnPropertyChanged();
            }
        }
        public War3Type SelectedItemPrevious { get; set; }


        public bool IsArray
        {
            get => _isArray;
            set
            {
                _isArray = value;
                OnPropertyChanged();
            }
        }
        public int DimensionsIndex
        {
            get => _dimensionsIndex;
            set
            {
                _dimensionsIndex = value;
                OnPropertyChanged();
            }
        }
        public int Size0
        {
            get => _size0;
            set
            {
                _size0 = value;
                OnPropertyChanged();
            }
        }
        public int Size1
        {
            get => _size1;
            set
            {
                _size1 = value;
                OnPropertyChanged();
            }
        }
        public bool IsTwoDimensions
        {
            get => _isTwoDimensions;
            set
            {
                _isTwoDimensions = value;
                OnPropertyChanged();
            }
        }
        public ObservableCollection<ExplorerElement> ReferenceTriggers { get; set; } = new();

        public VariableControlViewModel(Variable variable)
        {
            _variable = variable;
            variable.PropertyChanged += Variable_PropertyChanged;
            Variable_PropertyChanged(null, new PropertyChangedEventArgs(string.Empty)); // init
        }

        private void Variable_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            SuppressChangedEvents = true;
            Identifier = _variable.GetIdentifierName();
            War3Type = _variable.War3Type;
            SelectedItemPrevious = _variable.War3Type;
            IsArray = _variable.IsArray;
            IsTwoDimensions = _variable.IsTwoDimensions;
            DimensionsIndex = _variable.IsTwoDimensions ? 1 : 0;
            Size0 = _variable.ArraySize[0];
            Size1 = _variable.ArraySize[1];
            SuppressChangedEvents = false;
        }
    }
}
