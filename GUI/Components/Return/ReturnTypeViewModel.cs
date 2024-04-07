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
        private War3Type _selectedItem;
        private ReturnType _returnType;
        public ObservableCollection<War3Type> War3Types { get => War3Type.War3Types; }
        public War3Type SelectedItem
        {
            get => _selectedItem;
            set
            {
                _selectedItem = value;
                _returnType.War3Type = value;
                OnPropertyChanged();
            }
        }

        public ReturnTypeViewModel(ReturnType returnType)
        {
            _returnType = returnType;
            for (int i = 0; i < War3Types.Count; i++)
            {
                var type = War3Types[i];
                if(returnType.War3Type.Type == type.Type)
                {
                    SelectedItem = War3Types[i];
                    break;
                }
            }
        }
    }
}
