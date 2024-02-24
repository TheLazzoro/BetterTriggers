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

        public ListItemFunctionTemplate(FunctionTemplate template, Category category)
        {
            EditorSettings settings = EditorSettings.Load();
            string categoryStr = Locale.Translate(category.Name);
            if (categoryStr != "")
                categoryStr += " - ";

            string name = template.name != "" ? template.name : template.value;
            DisplayName = categoryStr + name;
            IsIconVisible = settings.GUINewElementIcon;
            eca = template.ToECA();
        }
    }
}
