using BetterTriggers.Controllers;
using GUI.Components.Shared;
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

            MenuItem menuItemCut = new ContextMenuItem("Cut");
            MenuItem menuItemReset = new ContextMenuItem("Copy");
            MenuItem menuItemPaste = new ContextMenuItem("Paste");
            MenuItem menuItemDelete = new ContextMenuItem("Delete");
            MenuItem menuItemCategory = new ContextMenuItem("New Category");
            MenuItem menuItemTrigger = new ContextMenuItem("New Trigger");
            MenuItem menuItemTriggerComment = new ContextMenuItem("New Trigger Comment");
            MenuItem menuItemScript = new ContextMenuItem("New Script");
            MenuItem menuItemGlobalVariable = new ContextMenuItem("New Global Variable");
            MenuItem menuItemEnableTrigger = new ContextMenuItem("Enable Trigger");
            MenuItem menuItemInitiallyOn = new ContextMenuItem("Initially On");
            MenuItem menuItemOpenInExplorer = new ContextMenuItem("Open Containing Folder");

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
            this.Items.Add(new ContextMenuSeperator());
            this.Items.Add(menuItemCategory);
            this.Items.Add(menuItemTrigger);
            this.Items.Add(menuItemTriggerComment);
            this.Items.Add(menuItemScript);
            this.Items.Add(menuItemGlobalVariable);
            this.Items.Add(new ContextMenuSeperator());
            this.Items.Add(menuItemEnableTrigger);
            this.Items.Add(menuItemInitiallyOn);
            this.Items.Add(new ContextMenuSeperator());
            this.Items.Add(menuItemOpenInExplorer);

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
            menuItemOpenInExplorer.Click += delegate
            {
                ControllerFileSystem controller = new ControllerFileSystem();
                controller.OpenInExplorer(treeItem.Ielement.GetPath());
            };
        }
    }
}
