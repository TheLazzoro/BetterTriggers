using BetterTriggers.Commands;
using BetterTriggers.Controllers;
using GUI.Components.Shared;
using GUI.Components.TriggerExplorer;
using Model.EditorData;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace GUI.Components.TriggerEditor
{
    public class ContextMenuTrigger : ContextMenu
    {
        TreeViewTriggerElement treeItemTriggerElement;
        MenuItem menuItemCut = new ContextMenuItem("Cut");
        MenuItem menuItemReset = new ContextMenuItem("Copy");
        MenuItem menuItemPaste = new ContextMenuItem("Paste");
        MenuItem menuItemDelete = new ContextMenuItem("Delete");
        MenuItem menuItemEvent = new ContextMenuItem("New Event");
        MenuItem menuItemCondition = new ContextMenuItem("New Condition");
        MenuItem menuItemAction = new ContextMenuItem("New Action");
        MenuItem menuItemEnableFunction = new ContextMenuItem("Enable Function");

        public ContextMenuTrigger(TreeViewItem treeItem)
        {
            var background = (SolidColorBrush)new BrushConverter().ConvertFromString("#1E1E1E");
            this.Background = background;
            this.BorderThickness = new System.Windows.Thickness(0);

            if (treeItem is INode)
            {
                DisableNodeTypes((INode)treeItem);
                menuItemEnableFunction.IsEnabled = false;
            }
            else
            {
                DisableNodeTypes((INode)treeItem.Parent);
                treeItemTriggerElement = (TreeViewTriggerElement)treeItem;

                if(treeItemTriggerElement.triggerElement.isEnabled)
                {
                    Image img = new Image();
                    img.Source = new BitmapImage(new Uri(System.IO.Directory.GetCurrentDirectory() + "/Resources/Icons/green-check.png"));
                    menuItemEnableFunction.Icon = img;
                }
            }


            this.Items.Add(menuItemCut);
            this.Items.Add(menuItemReset);
            this.Items.Add(menuItemPaste);
            this.Items.Add(menuItemDelete);
            this.Items.Add(new ContextMenuSeperator());
            this.Items.Add(menuItemEvent);
            this.Items.Add(menuItemCondition);
            this.Items.Add(menuItemAction);
            this.Items.Add(new ContextMenuSeperator());
            this.Items.Add(menuItemEnableFunction);

            menuItemDelete.Click += delegate
            {
                treeItemTriggerElement.GetTriggerControl().DeleteTriggerElement();
            };
            menuItemEvent.Click += delegate
            {
                treeItemTriggerElement.GetTriggerControl().CreateEvent();
            };
            menuItemCondition.Click += delegate
            {
                treeItemTriggerElement.GetTriggerControl().CreateCondition();
            };
            menuItemAction.Click += delegate
            {
                treeItemTriggerElement.GetTriggerControl().CreateAction();
            };
            menuItemEnableFunction.Click += delegate
            {
                CommandTriggerElementEnableDisable command = new CommandTriggerElementEnableDisable(treeItemTriggerElement.triggerElement);
                command.Execute();
            };
        }

        private void DisableNodeTypes(INode node)
        {
            if (node is NodeEvent)
            {
                menuItemCondition.IsEnabled = false;
                menuItemAction.IsEnabled = false;
            }
            else if (node is NodeCondition)
            {
                menuItemEvent.IsEnabled = false;
                menuItemAction.IsEnabled = false;
            }
            else if (node is NodeAction)
            {
                menuItemEvent.IsEnabled = false;
                menuItemCondition.IsEnabled = false;
            }
        }
    }
}
