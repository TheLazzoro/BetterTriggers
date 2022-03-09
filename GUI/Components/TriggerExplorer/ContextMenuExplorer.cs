using BetterTriggers.Controllers;
using Model.EditorData;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace GUI.Components.TriggerExplorer
{
    public class ContextMenuExplorer : ContextMenu
    {
        public ContextMenuExplorer(TreeItemExplorerElement treeItem)
        {
            var background = (SolidColorBrush)new BrushConverter().ConvertFromString("#1E1E1E");
            this.Background = background;
            this.BorderThickness = new System.Windows.Thickness(0);

            MenuItem menuItemCut = new MenuItemExplorer("Cut");
            MenuItem menuItemReset = new MenuItemExplorer("Copy");
            MenuItem menuItemPaste = new MenuItemExplorer("Paste");
            MenuItem menuItemDelete = new MenuItemExplorer("Delete");
            MenuItem menuItemCategory = new MenuItemExplorer("New Category");
            MenuItem menuItemTrigger = new MenuItemExplorer("New Trigger");
            MenuItem menuItemTriggerComment = new MenuItemExplorer("New Trigger Comment");
            MenuItem menuItemScript = new MenuItemExplorer("New Script");
            MenuItem menuItemGlobalVariable = new MenuItemExplorer("New Global Variable");
            MenuItem menuItemEnableTrigger = new MenuItemExplorer("Enable Trigger");
            MenuItem menuItemInitiallyOn = new MenuItemExplorer("Initially On");

            var element = treeItem.Ielement;
            if (element is ExplorerElementRoot || element is ExplorerElementFolder || element is ExplorerElementVariable)
            {
                menuItemEnableTrigger.IsEnabled = false;
                menuItemInitiallyOn.IsEnabled = false;
            }
            else if (element is ExplorerElementScript)
                menuItemInitiallyOn.IsEnabled = false;

            if (element.GetEnabled())
            {
                Image img = new Image();
                img.Source = new BitmapImage(new Uri(System.IO.Directory.GetCurrentDirectory() + "/Resources/Icons/green-check.png"));
                menuItemEnableTrigger.Icon = img;
            }
            if (element.GetInitiallyOn())
            {
                Image img = new Image();
                img.Source = new BitmapImage(new Uri(System.IO.Directory.GetCurrentDirectory() + "/Resources/Icons/green-check.png"));
                menuItemInitiallyOn.Icon = img;
            }

            this.Items.Add(menuItemCut);
            this.Items.Add(menuItemReset);
            this.Items.Add(menuItemPaste);
            this.Items.Add(menuItemDelete);
            this.Items.Add(new SeparatorExplorer());
            this.Items.Add(menuItemCategory);
            this.Items.Add(menuItemTrigger);
            this.Items.Add(menuItemTriggerComment);
            this.Items.Add(menuItemScript);
            this.Items.Add(menuItemGlobalVariable);
            this.Items.Add(new SeparatorExplorer());
            this.Items.Add(menuItemEnableTrigger);
            this.Items.Add(menuItemInitiallyOn);

            menuItemDelete.Click += delegate
            {
                var controller = new ControllerFileSystem();
                controller.DeleteElement(treeItem.Ielement.GetPath());
            };
            menuItemCategory.Click += delegate
            {
                var controller = new ControllerFolder();
                controller.CreateFolder();
            };
            menuItemTrigger.Click += delegate
            {
                var controller = new ControllerTrigger();
                controller.CreateTrigger();
            };
            menuItemScript.Click += delegate
            {
                var controller = new ControllerScript();
                controller.CreateScript();
            };
            menuItemGlobalVariable.Click += delegate
            {
                var controller = new ControllerVariable();
                controller.CreateVariable();
            };
            menuItemEnableTrigger.Click += delegate
            {
                treeItem.editor.SetElementEnabled(!treeItem.Ielement.GetEnabled());
            };
            menuItemInitiallyOn.Click += delegate
            {
                treeItem.editor.SetElementInitiallyOn(!treeItem.Ielement.GetInitiallyOn());

            };
        }
    }
}
