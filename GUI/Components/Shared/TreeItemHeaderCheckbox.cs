using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace GUI.Components.Shared
{
    public class TreeItemHeaderCheckbox : TreeItemHeader
    {
        public CheckBox checkbox;

        public TreeItemHeaderCheckbox(string text, string categoryName, TreeItemState state = TreeItemState.Normal, bool initiallyOn = true, bool isChecked = false)
            : base (text, categoryName, state, initiallyOn)
        {
            ColumnDefinition column0 = new ColumnDefinition();
            column0.Width = GridLength.Auto;
            this.ColumnDefinitions.Insert(0, column0);

            checkbox = new CheckBox();
            checkbox.IsChecked = isChecked;
            this.Children.Insert(0, checkbox); // inserts into the grid, not the stackpanel

            // shift all other elements in the header to the right
            Grid.SetColumn(this.Icon, 1);
            Grid.SetColumn(this.IconOverlay, 1);
            Grid.SetColumn(this.stackPanel, 2);
        }
    }
}
