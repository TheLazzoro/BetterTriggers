using BetterTriggers.Models.EditorData;
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
        internal IExplorerElement explorerElement { get; }

        internal TreeItemHeaderCheckbox treeItemHeader;

        public ImportTriggerItem(IExplorerElement explorerElement)
        {
            this.explorerElement = explorerElement;

            string category = string.Empty;
            switch (explorerElement)
            {
                case ExplorerElementRoot:
                    category = "TC_MAP";
                    break;
                case ExplorerElementFolder:
                    category = "TC_DIRECTORY";
                    break;
                case ExplorerElementTrigger:
                    category = "TC_TRIGGER_NEW";
                    break;
                case ExplorerElementScript:
                    category = "TC_SCRIPT";
                    break;
                case ExplorerElementVariable:
                    category = "TC_SETVARIABLE";
                    break;
                default:
                    break;
            }

            treeItemHeader = new TreeItemHeaderCheckbox(explorerElement.GetName(), category);
            this.Header = treeItemHeader;
            treeItemHeader.checkbox.Click += Checkbox_Click;
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
