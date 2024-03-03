using BetterTriggers;
using BetterTriggers.Models.EditorData;
using BetterTriggers.Models.Templates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;

namespace GUI.Components.TriggerEditor
{
    public class ListItemFunctionTemplate : TreeNodeBase
    {
        public ECA eca { get; }
        public string DisplayName { get; }
        public double IconWidth
        {
            get => _iconWidth;
            set
            {
                _iconWidth = value;
                OnPropertyChanged();
            }
        }

        private double _iconWidth = Double.NaN; // "auto" property in WPF

        public ListItemFunctionTemplate(FunctionTemplate template, Category category)
        {
            EditorSettings settings = EditorSettings.Load();
            string categoryStr = Locale.Translate(category.Name);
            if (categoryStr != "")
                categoryStr += " - ";

            string name = template.name != "" ? template.name : template.value;
            DisplayName = categoryStr + name;
            IconImage = category.Icon;
            IsIconVisible = settings.GUINewElementIcon ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
            eca = template.ToECA();
            eca.IconImage = category.Icon;

            PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(IsIconVisible))
                {
                    if (IsIconVisible == System.Windows.Visibility.Visible)
                        IconWidth = Double.NaN; // "auto" property in WPF
                    else
                        IconWidth = 0;
                }
            };
        }
    }
}
