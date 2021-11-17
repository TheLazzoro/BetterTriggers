using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace GUI.Components.TriggerExplorer
{
    public abstract class TriggerElement
    {
        public string Name;
        public TreeViewItem treeViewItem;
        internal static ITriggerElement currentTriggerElement;

        public TriggerElement(TreeViewItem treeViewItem)
        {
            this.treeViewItem = treeViewItem;

            // click event
            treeViewItem.Selected += new RoutedEventHandler(delegate (object sender, RoutedEventArgs e)
            {
                var thisElement = (ITriggerElement) this;
                thisElement.OnElementClick();

                // prevents event from firing up the parent items
                e.Handled = true;
            });
        }

    }
}
