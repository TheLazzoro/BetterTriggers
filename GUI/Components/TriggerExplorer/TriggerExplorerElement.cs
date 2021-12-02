﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace GUI.Components.TriggerExplorer
{
    public abstract class TriggerExplorerElement
    {
        public string Name;
        public TreeViewItem treeViewItem;
        internal static ITriggerExplorerElement currentTriggerElement;

        public TriggerExplorerElement(TreeViewItem treeViewItem)
        {
            this.treeViewItem = treeViewItem;
            this.treeViewItem.Tag = this;

            // click event
            treeViewItem.Selected += new RoutedEventHandler(delegate (object sender, RoutedEventArgs e)
            {
                var thisElement = (ITriggerExplorerElement) this;
                thisElement.OnElementClick();

                // prevents event from firing up the parent items
                e.Handled = true;
            });
        }
    }
}