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

        public TabViewModel Parent;
        public ExplorerElement explorerElement;
        public UserControl Content { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public TabItemBT(ExplorerElement explorerElement, UserControl editor, TabViewModel parent)
        {
            this.explorerElement = explorerElement;
            Header = explorerElement.GetName();
            Content = editor;
            Parent = parent;
        }

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void Close()
        {
            Parent.Tabs.Remove(this);
            if (Content is TriggerControl triggerControl)
                triggerControl.Dispose();
            
        }
    }
}
