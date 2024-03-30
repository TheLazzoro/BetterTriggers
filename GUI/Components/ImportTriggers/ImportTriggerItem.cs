using BetterTriggers.Models.EditorData;
using GUI.Components.Shared;
using GUI.Components.TriggerEditor.ParameterControls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    public class ImportTriggerItem : TreeNodeBase
    {
        private static bool _suppressEvents = false;
        private bool _isInitiallyOn;

        public bool IsInitiallyOn
        {
            get => _isInitiallyOn;
            set
            {
                if (!_suppressEvents)
                {
                    _isInitiallyOn = value;
                    OnPropertyChanged();
                }
            }
        }
        public ObservableCollection<ImportTriggerItem> ExplorerElements { get; set; } = new();
        public ExplorerElement explorerElement { get; }
        public ImportTriggerItem Parent { get; set; }

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

            var cat = Category.Get(category);
            DisplayText = explorerElement.GetName();
            IsEnabled = explorerElement.IsEnabled;
            IsInitiallyOn = explorerElement.IsInitiallyOn;
            HasErrors = explorerElement.HasErrors;
            IconImage = cat.Icon;
            CheckBoxVisibility = Visibility.Visible;
            CheckBoxWidth = 20;

            PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(IsChecked) && !_suppressEvents)
                {
                    _suppressEvents = true;
                    Checkbox_Click();
                    _suppressEvents = false;
                }
            };
        }

        private void Checkbox_Click()
        {
            ToggleCheckboxRecurse(this);
            ToggleCheckboxRecurseReverse(this, IsChecked);
        }

        /// <summary>
        /// All child items of a checked item are also affected.
        /// </summary>
        private void ToggleCheckboxRecurse(ImportTriggerItem parent)
        {
            foreach (ImportTriggerItem item in parent.ExplorerElements)
            {
                if (item is ImportTriggerItem treeItem)
                {
                    item.IsChecked = this.IsChecked;
                    if (treeItem.ExplorerElements.Count > 0)
                    {
                        ToggleCheckboxRecurse(treeItem);
                    }
                }
            }
        }

        /// <summary>
        /// All parent items of a checked item are affected.
        /// </summary>
        private void ToggleCheckboxRecurseReverse(ImportTriggerItem treeItem, bool isChecked)
        {
            var parent = treeItem.Parent;
            if (parent != null)
            {
                int checkedChildrenCount = 0;
                foreach (ImportTriggerItem child in parent.ExplorerElements)
                {
                    if (child.IsChecked)
                    {
                        checkedChildrenCount++;
                    }
                }

                if (checkedChildrenCount == 0 || isChecked)
                {
                    parent.IsChecked = isChecked;
                    ToggleCheckboxRecurseReverse(parent, isChecked);
                }
            }
        }
    }
}
