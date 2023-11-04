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

        public IEditor Content { get; set; }
        public TabViewModel Parent;
        public TreeItemExplorerElement explorerElement;

        public event PropertyChangedEventHandler PropertyChanged;

        public TabItemBT(TreeItemExplorerElement explorerElement, TabViewModel parent)
        {
            this.explorerElement = explorerElement;
            this.explorerElement.tabItem = this;
            Header = explorerElement.Ielement.GetName();
            Content = explorerElement.editor;
            Parent = parent;
        }

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void Close()
        {
            Parent.Tabs.Remove(this);
            if (explorerElement.editor is TriggerControl triggerControl)
                triggerControl.Dispose();
            explorerElement.editor = null;
            explorerElement.tabItem = null;
        }
    }
}
