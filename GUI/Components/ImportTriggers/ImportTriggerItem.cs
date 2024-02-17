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
using Xceed.Wpf.Toolkit;
using static System.Net.Mime.MediaTypeNames;
using static System.Windows.Forms.AxHost;

namespace GUI.Components.ImportTriggers
{
    internal class ImportTriggerItem
    {
        internal ExplorerElement explorerElement { get; }

        public ImportTriggerItem(ExplorerElement explorerElement)
        {
            this.explorerElement = explorerElement;

            string category = string.Empty;
            switch (explorerElement.ElementType)
            {
                case ExplorerElementEnum.Root:
                    category = "TC_MAP";
                    break;
                case ExplorerElementEnum.Folder:
                    category = "TC_DIRECTORY";
                    break;
                case ExplorerElementEnum.Trigger:
                    category = "TC_TRIGGER_NEW";
                    break;
                case ExplorerElementEnum.Script:
                    category = "TC_SCRIPT";
                    break;
                case ExplorerElementEnum.GlobalVariable:
                    category = "TC_SETVARIABLE";
                    break;
                default:
                    break;
            }

            // TODO: BETTERTRIGGERS REFACTOR
            //treeItemHeader = new TreeItemHeaderCheckbox(explorerElement.GetName(), category);
            //this.Header = treeItemHeader;
            //treeItemHeader.checkbox.Click += Checkbox_Click;
        }

        private void Checkbox_Click(object sender, RoutedEventArgs e)
        {
            ToggleCheckboxRecurse(this);
            // TODO: BETTERTRIGGERS REFACTOR
            //ToggleCheckboxRecurseReverse(this, (bool)this.treeItemHeader.checkbox.IsChecked);
        }

        /// <summary>
        /// All child items of a checked item are also affected.
        /// </summary>
        private void ToggleCheckboxRecurse(ImportTriggerItem parent)
        {
            // TODO: BETTERTRIGGERS REFACTOR
            //foreach (var item in parent.Items)
            //{
            //    if (item is ImportTriggerItem treeItem)
            //    {
            //        var header = treeItem.Header as TreeItemHeaderCheckbox;
            //        header.checkbox.IsChecked = parent.treeItemHeader.checkbox.IsChecked;
            //        if (treeItem.Items.Count > 0)
            //        {
            //            ToggleCheckboxRecurse(treeItem);
            //        }
            //    }
            //}
        }

        /// <summary>
        /// All parent items of a checked item are affected.
        /// </summary>
        private void ToggleCheckboxRecurseReverse(ImportTriggerItem treeItem, bool isChecked)
        {
            // TODO: BETTERTRIGGERS REFACTOR
            //var parent = treeItem.Parent as ImportTriggerItem;
            //if (parent != null)
            //{
            //    var header = parent.Header as TreeItemHeaderCheckbox;
            //    int checkedChildrenCount = 0;
            //    foreach (ImportTriggerItem child in parent.Items)
            //    {
            //        if((bool)child.treeItemHeader.checkbox.IsChecked)
            //        {
            //            checkedChildrenCount++;
            //        }
            //    }

            //    if(checkedChildrenCount == 0 || isChecked)
            //    {
            //        header.checkbox.IsChecked = isChecked;
            //        ToggleCheckboxRecurseReverse(parent, isChecked);
            //    }
            //}
        }
    }
}
