using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace GUI.Components.Shared
{
    public class TreeItemBT : TreeViewItem
    {
        internal TreeItemHeader treeItemHeader { get; set; }

        public string GetHeaderText()
        {
            return treeItemHeader.GetDisplayText();
        }
    }
}
