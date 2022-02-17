using Facades.Controllers;
using GUI.Utility;
using Model.Data;
using Model.EditorData;
using Model.EditorData.Enums;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace GUI.Components.TriggerExplorer
{
    public class TreeItemExplorerElement : TreeViewItem, IExplorerElementObserver
    {
        public TabItemBT tabItem;
        public IExplorerElement Ielement;
        public IEditor editor;

        public TreeItemExplorerElement(IExplorerElement explorerElement)
        {
            this.Ielement = explorerElement;

            RefreshElement();
        }

        public TreeItemExplorerElement(IExplorerElement explorerElement, bool isRoot)
        {
            this.Ielement = explorerElement;
            TreeViewManipulator.SetTreeViewItemAppearance(this, Ielement.GetName(), Category.Map);
        }

        public void Delete()
        {
            Application.Current.Dispatcher.Invoke(delegate
            {
                var parent = this.Parent as TreeViewItem;
                if (parent != null)
                    parent.Items.Remove(this);

                if (tabItem != null)
                {
                    var tabControl = tabItem.Parent as TabControl;
                    if (tabControl != null)
                        tabControl.Items.Remove(tabItem);
                }
            });
        }

        public void RefreshElement()
        {
            Category category;
            switch (Path.GetExtension(this.Ielement.GetPath()))
            {
                case "":
                    category = Category.Folder;
                    break;
                case ".trg":
                    category = Category.Trigger;
                    break;
                case ".j":
                    category = Category.AI;
                    break;
                case ".var":
                    category = Category.SetVariable;
                    break;
                default:
                    category = Category.Trigger;
                    break;
            }

            TreeViewManipulator.SetTreeViewItemAppearance(this, this.Ielement.GetName(), category);

            if (this.tabItem != null)
                tabItem.RefreshHeader(this.Ielement.GetName());

            //if(Ielement != null)
            //Ielement.OnElementRename();
        }

        public void Save()
        {
            string saveableString = editor.GetSaveString();
            Ielement.SaveInMemory(saveableString);

            ControllerFileSystem controller = new ControllerFileSystem();
            controller.SaveFile(Ielement.GetPath(), saveableString);
        }

        public void Update(IExplorerElement subject)
        {
            Application.Current.Dispatcher.Invoke(delegate
            {
                RefreshElement();
            });
        }
    }
}
