using GUI.Components.Shared;
using GUI.Components.TriggerEditor.ParameterControls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using War3Net.Build.Script;
using static System.Net.Mime.MediaTypeNames;
using static System.Windows.Forms.AxHost;

namespace GUI.Components.ImportTriggers
{
    internal class ImportTriggerItem : TreeViewItem
    {
        internal TriggerItem triggerItem { get; }
        internal bool IsValid { get; }
        internal List<ImportTriggerItem> Children;

        private TreeItemHeaderCheckbox treeItemHeader;

        public ImportTriggerItem(TriggerItem triggerItem)
        {
            this.triggerItem = triggerItem;

            string name = triggerItem.Name;
            string category = string.Empty;
            switch (triggerItem.Type)
            {
                case TriggerItemType.RootCategory:
                    category = "TC_MAP";
                    Children = new List<ImportTriggerItem>();
                    break;
                case TriggerItemType.Category:
                    category = "TC_DIRECTORY";
                    Children = new List<ImportTriggerItem>();
                    break;
                case TriggerItemType.Gui:
                    category = "TC_TRIGGER_NEW";
                    break;
                case TriggerItemType.Script:
                    category = "TC_SCRIPT";
                    break;
                case TriggerItemType.Variable:
                    category = "TC_SETVARIABLE";
                    break;
                default:
                    break;
            }

            if(triggerItem is not DeletedTriggerItem && !string.IsNullOrEmpty(category))
            {
                IsValid = true;
                treeItemHeader = new TreeItemHeaderCheckbox(name, category);
                this.Header = treeItemHeader;

                treeItemHeader.checkbox.Click += Checkbox_Click;
            }
        }

        private void Checkbox_Click(object sender, RoutedEventArgs e)
        {
            ToggleCheckboxRecurse(this);
        }

        private void ToggleCheckboxRecurse(ImportTriggerItem parent)
        {
            foreach (var item in parent.Items)
            {
                if (item is ImportTriggerItem treeItem)
                {
                    var header = treeItem.Header as TreeItemHeaderCheckbox;
                    header.checkbox.IsChecked = parent.treeItemHeader.checkbox.IsChecked;
                    if (treeItem.Items.Count > 0)
                    {
                        ToggleCheckboxRecurse(treeItem);
                    }
                }
            }
        }
    }
}
