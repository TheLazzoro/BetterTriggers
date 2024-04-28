using BetterTriggers;
using BetterTriggers.Containers;
using BetterTriggers.Models.EditorData;
using BetterTriggers.WorldEdit;
using GUI.Components.Shared;
using GUI.Components.VariableEditor;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GUI.Components.Return
{
    public class ReturnTypeViewModel : ViewModelBase
    {
        private ReturnType _returnType;
        public ObservableCollection<War3Type> War3Types { get => War3Type.War3Types; }
        public War3Type SelectedItem
        {
            get => _returnType.War3Type;
        }

        public ReturnTypeViewModel(ReturnType returnType)
        {
            _returnType = returnType;
            _returnType.PropertyChanged += _returnType_PropertyChanged;
        }

        private void _returnType_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(_returnType.War3Type))
                OnPropertyChanged(nameof(SelectedItem));
        }
    }
}
