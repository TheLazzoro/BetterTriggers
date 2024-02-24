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
        public ECA Previous { get; set; }
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
            Previous = previous;
            var templates = new List<FunctionTemplate>();
            if (triggerElementType == TriggerElementType.Event)
            {
                templates = TriggerData.LoadAllEvents();
            }
            else if (triggerElementType == TriggerElementType.Condition)
            {
                templates = TriggerData.LoadAllConditions();
            }
            else if (triggerElementType == TriggerElementType.Action)
            {
                templates = TriggerData.LoadAllActions();
            }

            List<Searchable> objects = new List<Searchable>();
            for (int i = 0; i < templates.Count; i++)
            {
                var template = templates[i];
                Category category = Category.Get(template.category);
                ListItemFunctionTemplate listItem = new(template, category);
                _ecas.Add(listItem);

                objects.Add(new Searchable()
                {
                    Object = listItem,
                    Category = Locale.Translate(category.Name),
                    Words = new List<string>()
                    {
                        listItem.DisplayName.ToLower(),
                        template.value.ToLower()
                    },
                });

                // default selection
                if (Previous != null && Previous.function.value == template.value)
                {
                    Selected = Previous;
                }
                if(Selected == null)
                {
                    Selected = _ecas[0].eca;
                }

                Searchables = new Searchables(objects);
            }
        }
    }
}
