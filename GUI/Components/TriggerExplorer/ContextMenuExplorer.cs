using Facades.Controllers;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;

namespace GUI.Components.TriggerExplorer
{
    public class ContextMenuExplorer : ContextMenu
    {
        public ContextMenuExplorer(TreeItemExplorerElement treeItem)
        {
            MenuItem menuItemCut = new MenuItem { Header = "Cut" };
            MenuItem menuItemReset = new MenuItem { Header = "Copy" };
            MenuItem menuItemPaste = new MenuItem { Header = "Paste" };
            MenuItem menuItemDelete = new MenuItem { Header = "Delete" };
            MenuItem menuItemCategory = new MenuItem { Header = "New Category" };
            MenuItem menuItemTrigger = new MenuItem { Header = "New Trigger" };
            MenuItem menuItemTriggerComment = new MenuItem { Header = "New Trigger Comment" };
            MenuItem menuItemScript = new MenuItem { Header = "New Script" };
            MenuItem menuItemGlobalVariable = new MenuItem { Header = "New Global Variable" };
            MenuItem menuItemEnableTrigger = new MenuItem { Header = "Enable Trigger" };
            MenuItem menuItemInitiallyOn = new MenuItem { Header = "Initially On" };

            this.Items.Add(menuItemCut);
            this.Items.Add(menuItemReset);
            this.Items.Add(menuItemPaste);
            this.Items.Add(menuItemDelete);
            this.Items.Add(new Separator());
            this.Items.Add(menuItemCategory);
            this.Items.Add(menuItemTrigger);
            this.Items.Add(menuItemTriggerComment);
            this.Items.Add(menuItemScript);
            this.Items.Add(menuItemGlobalVariable);
            this.Items.Add(new Separator());
            this.Items.Add(menuItemEnableTrigger);
            this.Items.Add(menuItemInitiallyOn);
            //menuItemReplace.Click += ReplaceTexture;
            //menuItemReset.Click += ResetTexture;

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
        }
    }
}
