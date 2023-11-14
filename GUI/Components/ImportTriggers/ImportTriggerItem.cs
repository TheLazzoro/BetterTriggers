﻿using GUI.Components.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using War3Net.Build.Script;

namespace GUI.Components.ImportTriggers
{
    internal class ImportTriggerItem : TreeViewItem
    {
        internal TriggerItem triggerItem { get; }
        internal bool IsValid { get; }
        internal List<ImportTriggerItem> Children;

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
                this.Header = new TreeItemHeader(name, category);
            }
        }
    }
}