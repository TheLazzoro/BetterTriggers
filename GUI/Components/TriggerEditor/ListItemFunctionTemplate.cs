using BetterTriggers;
using BetterTriggers.Containers;
using BetterTriggers.Models.EditorData;
using BetterTriggers.Models.Templates;
using System;

namespace GUI.Components.TriggerEditor
{
    public class ListItemFunctionTemplate : TreeNodeBase
    {
        public ECA eca { get; }
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

        public ListItemFunctionTemplate(Project project, FunctionTemplate template, Category category)
        {
            EditorSettings settings = EditorSettings.Load();
            string categoryStr = Locale.Translate(category.Name);
            if (categoryStr != "")
                categoryStr += " - ";

            string name = template.name != "" ? template.name : template.value;
            DisplayText = categoryStr + name;
            IconImage = category.Icon;
            eca = template.ToECA(project);
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

            // Important to set visibility after 'PropertyChanged' event listener.
            IsIconVisible = settings.GUINewElementIcon ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
        }
    }
}
