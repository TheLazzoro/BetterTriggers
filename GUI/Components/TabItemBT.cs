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

namespace GUI.Components
{
    public class TabItemBT : INotifyPropertyChanged
    {
        private string header;
        public string Header {
            get
            {
                return this.header;
            }
            set
            {
                if (value != this.header)
                {
                    this.header = value;
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
            this.Header = explorerElement.Ielement.GetName();
            this.Content = explorerElement.editor;
            this.Parent = parent;
        }

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void Close()
        {
            Parent.Tabs.Remove(this);
            if (this.explorerElement.editor is TriggerControl triggerControl)
                triggerControl.Dispose();
            this.explorerElement.editor = null;
            this.explorerElement.tabItem = null;
        }
    }
}
