using BetterTriggers.Controllers;
using GUI.Components.Shared;
using GUI.Controllers;
using Model.Data;
using Model.EditorData;
using Model.EditorData.Enums;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace GUI.Components.TriggerExplorer
{
    public class TreeItemExplorerElement : TreeViewItem, IExplorerElementUI
    {
        public TabItemBT tabItem;
        public IExplorerElement Ielement;
        public IEditor editor;
        private TreeItemHeader treeItemHeader;
        private Category category;

        public TreeItemExplorerElement(IExplorerElement explorerElement)
        {
            this.Ielement = explorerElement;

            if (Ielement is ExplorerElementRoot)
                category = Category.Map;
            else if (Ielement is ExplorerElementFolder)
                category = Category.Folder;
            else if (Ielement is ExplorerElementTrigger)
                category = Category.Trigger;
            else if (Ielement is ExplorerElementScript)
                category = Category.AI;
            else if (Ielement is ExplorerElementVariable)
                category = Category.SetVariable;
            else
                category = Category.Wait;

            this.treeItemHeader = new TreeItemHeader(explorerElement.GetName(), category, Ielement.GetEnabled(), Ielement.GetInitiallyOn());
            this.Header = treeItemHeader;
            this.KeyDown += TreeItemExplorerElement_KeyDown;
            this.treeItemHeader.RenameBox.KeyDown += RenameBox_KeyDown;

            RefreshElement();
        }


        public void Delete()
        {
            Application.Current.Dispatcher.Invoke(delegate
            {
                var parent = this.Parent as TreeViewItem;
                if (parent != null)
                {
                    parent.Items.Remove(this);
                    ControllerExplorerElement controller = new ControllerExplorerElement();
                    controller.RemoveFromUnsaved(this.Ielement);
                }

                if (tabItem != null)
                {
                    var tabControl = tabItem.Parent as TabControl;
                    if (tabControl != null)
                        tabControl.Items.Remove(tabItem);
                    tabItem = null;
                }
            });
        }

        public void RefreshElement()
        {
            if (this.Ielement == null)
                return;

            treeItemHeader.SetIcon(category, Ielement.GetEnabled());

            if (this.tabItem != null)
                tabItem.RefreshHeader(this.Ielement.GetName());

            if (this.editor is VariableControl)
            {
                var control = this.editor as VariableControl;
                control.Rename(Ielement.GetName());
            }

            if (this.editor != null)
                this.editor.Refresh();
        }

        public void Update(IExplorerElement subject)
        {
            Application.Current.Dispatcher.Invoke(delegate
            {
                RefreshElement();
            });
        }

        public void ShowRenameBox()
        {
            this.treeItemHeader.ShowRenameBox(true);
        }

        private void TreeItemExplorerElement_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.F2)
            {
                ShowRenameBox();
                e.Handled = true;
            }

        }

        private void RenameBox_KeyDown(object sender, KeyEventArgs e)
        {
            string renameText = this.treeItemHeader.GetRenameText();
            ControllerProject controller = new ControllerProject();
            if (e.Key == Key.Enter)
            {
                try
                {
                    controller.RenameElement(this.Ielement, renameText);
                    this.treeItemHeader.ShowRenameBox(false);
                }
                catch (Exception ex)
                {
                    MessageBox messageBox = new MessageBox("Error", ex.Message);
                    messageBox.ShowDialog();
                }
            }
            else if (e.Key == Key.Escape)
            {
                this.treeItemHeader.ShowRenameBox(false);
                this.treeItemHeader.SetDisplayText(this.Ielement.GetName());
                this.Focus();
            }
        }

        /// <summary>
        /// Gets invoked when an action in the 'editor' field triggers a state of change
        /// E.g. new letters in script were typed (ScriptControl), new action gets added (TriggerControl).
        /// </summary>
        public void OnStateChange()
        {
            treeItemHeader.SetIcon(category, Ielement.GetEnabled());

            if (this.tabItem != null)
                tabItem.RefreshHeader(this.Ielement.GetName() + " *");

            ControllerExplorerElement controller = new ControllerExplorerElement();
            controller.AddToUnsaved(this.Ielement);
        }

        public void UpdatePosition()
        {
            Application.Current.Dispatcher.Invoke(delegate
            {
                ControllerTriggerExplorer controller = new ControllerTriggerExplorer();
                int insertIndex = Ielement.GetParent().GetExplorerElements().IndexOf(Ielement);
                controller.OnMoveElement(controller.GetCurrentExplorer(), Ielement.GetPath(), insertIndex);
                this.treeItemHeader.SetDisplayText(this.Ielement.GetName());

                this.IsSelected = true;
                this.Focus();
            });
        }

        public void OnCreated(int insertIndex)
        {
            ControllerTriggerExplorer controller = new ControllerTriggerExplorer();
            var dir = Path.GetDirectoryName(this.Ielement.GetPath());
            var parent = controller.FindTreeNodeDirectory(Path.GetDirectoryName(this.Ielement.GetPath()));
            parent.Items.Insert(insertIndex, this);

            this.IsSelected = true;
            this.Focus();
        }
    }
}
