using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using BetterTriggers.Models.EditorData;
using BetterTriggers.Containers;

namespace GUI.Components.Tabs
{
    public class TabItemBT : INotifyPropertyChanged
    {
        private string header;
        public string Header
        {
            get
            {
                return header;
            }
            set
            {
                if (value != header)
                {
                    header = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public string ToolTip
        {
            get => explorerElement.GetPath();
        }

        public TabViewModel Parent;
        public ExplorerElement explorerElement;
        public UserControl Content { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public TabItemBT(ExplorerElement explorerElement, UserControl editor, TabViewModel parent)
        {
            this.explorerElement = explorerElement;
            explorerElement.OnChanged += ExplorerElement_OnChanged;
            explorerElement.OnSaved += ExplorerElement_OnSaved;
            explorerElement.OnDeleted += ExplorerElement_OnDeleted;

            Content = editor;
            Parent = parent;

            bool isUnsaved = Project.CurrentProject.UnsavedFiles.Contains(explorerElement);
            if (isUnsaved)
                ExplorerElement_OnChanged();
            else
                ExplorerElement_OnSaved();
        }

        private void ExplorerElement_OnDeleted()
        {
            Parent.Tabs.Remove(this);
        }

        private void ExplorerElement_OnChanged()
        {
            Header = explorerElement.GetName() + " *";
        }

        private void ExplorerElement_OnSaved()
        {
            Header = explorerElement.GetName();
        }

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void Close()
        {
            Parent.Tabs.Remove(this);
            explorerElement.OnChanged -= ExplorerElement_OnChanged;
            explorerElement.OnSaved -= ExplorerElement_OnSaved;
            if (Content is TriggerControl triggerControl)
                triggerControl.Dispose();

        }
    }
}
