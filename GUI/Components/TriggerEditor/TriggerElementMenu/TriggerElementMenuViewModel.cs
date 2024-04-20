using BetterTriggers;
using BetterTriggers.Models.EditorData;
using BetterTriggers.Models.Templates;
using BetterTriggers.Utility;
using BetterTriggers.WorldEdit;
using GUI.Components.Shared;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GUI.Components.TriggerEditor
{
    public class TriggerElementMenuViewModel : ViewModelBase
    {
        private ECA _selected;

        public ObservableCollection<ListItemFunctionTemplate> _ecas { get; } = new();
        public ECA Selected
        {
            get => _selected;
            set
            {
                _selected = value;
                OnPropertyChanged();
            }
        }
        public Searchables Searchables { get; }

        public TriggerElementMenuViewModel(TriggerElementType triggerElementType, ECA? previous)
        {
            
        }
    }
}
