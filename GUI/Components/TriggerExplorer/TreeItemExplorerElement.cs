using BetterTriggers.Containers;
using BetterTriggers.Models.EditorData;
using GUI.Components.Shared;
using GUI.Controllers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using static GUI.Components.Shared.TreeItemHeader;

namespace GUI.Components
{
    public class TreeItemExplorerElement : TreeItemBT, IExplorerElementUI
    {
        public TabItemBT tabItem;
        public IExplorerElement Ielement;
        public IEditor editor;
        private string categoryName;

        public TreeItemExplorerElement(IExplorerElement explorerElement)
        {
            this.Ielement = explorerElement;

            if (Ielement is ExplorerElementRoot)
                categoryName = TriggerCategory.TC_MAP;
            else if (Ielement is ExplorerElementFolder)
                categoryName = TriggerCategory.TC_DIRECTORY;
            else if (Ielement is ExplorerElementTrigger)
                categoryName = TriggerCategory.TC_TRIGGER_NEW;
            else if (Ielement is ExplorerElementScript)
                categoryName = TriggerCategory.TC_SCRIPT;
            else if (Ielement is ExplorerElementVariable)
                categoryName = TriggerCategory.TC_SETVARIABLE;
            else
                categoryName = "TC_WTF"; // shouldn't happen

            TreeItemState state = Ielement.GetEnabled() == true ? TreeItemState.Normal : TreeItemState.Disabled;
            this.treeItemHeader = new TreeItemHeader(Ielement.GetName(), categoryName, state, Ielement.GetInitiallyOn());
            this.Header = treeItemHeader;
            this.KeyDown += TreeItemExplorerElement_KeyDown;
            this.treeItemHeader.RenameBox.KeyDown += RenameBox_KeyDown;
            this.MouseDoubleClick += TreeItemExplorerElement_MouseDoubleClick;

            ReloadElement();
            RefreshHeader();
        }


        public void Delete()
        {
            Application.Current.Dispatcher.Invoke(delegate
            {
                var parent = this.Parent as TreeViewItem;
                if (parent != null)
                {
                    parent.Items.Remove(this);
                    Project.CurrentProject.UnsavedFiles.RemoveFromUnsaved(this.Ielement);
                }

                if (tabItem != null)
                {
                    var tabControl = tabItem.Parent as TabViewModel;
                    if (tabControl != null)
                        tabControl.Tabs.Remove(tabItem);
                    tabItem = null;
                }
            });
        }

        public void ReloadElement()
        {
            if (this.Ielement == null)
                return;

            if (this.tabItem != null)
            {
                tabItem.Header = this.Ielement.GetName();
            }

            if( this.editor is TriggerControl)
            {
                var control = this.editor as TriggerControl;
                control.OnRemoteChange();
            }
            if (this.editor is VariableControl)
            {
                var control = this.editor as VariableControl;
                control.UpdateIdentifierText();
            }
            else if(this.editor is ScriptControl)
            {
                var control = this.editor as ScriptControl;
                control.OnRemoteChange();
            }
        }

        public void Reload()
        {
            Application.Current.Dispatcher.Invoke(delegate
            {
                ReloadElement();
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
            if (e.Key == Key.Enter)
            {
                try
                {
                    Project.CurrentProject.RenameElement(this.Ielement, renameText);
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
            RefreshHeader();

            if (this.tabItem != null)
                tabItem.Header = this.Ielement.GetName() + " *";

            Project.CurrentProject.UnsavedFiles.AddToUnsaved(this.Ielement);
        }

        public void OnSaved()
        {
            if (this.tabItem != null)
                tabItem.Header = this.Ielement.GetName();
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

        public void OnRemoteChange()
        {
            editor.OnRemoteChange();
        }

        public void RefreshHeader()
        {
            TreeItemState state = Ielement.GetEnabled() == true ? TreeItemState.Normal : TreeItemState.Disabled;
            if (Ielement is ExplorerElementTrigger)
            {
                int invalidCount = Project.CurrentProject.Triggers.VerifyParametersInTrigger(Ielement as ExplorerElementTrigger);
                if (state != TreeItemState.Disabled && invalidCount > 0)
                    state = TreeItemState.HasErrorsNoTextColor;
            }

            treeItemHeader.SetTextEnabled(state, Ielement.GetInitiallyOn());
            treeItemHeader.SetIcon(categoryName, state);
            treeItemHeader.SetDisplayText(Ielement.GetName());
        }

        private void TreeItemExplorerElement_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            /// Prevents treeitem from closing/expanding
            if (Ielement is ExplorerElementRoot)
                e.Handled = true;
        }
    }
}
