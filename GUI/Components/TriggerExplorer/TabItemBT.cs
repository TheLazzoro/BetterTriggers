using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace GUI.Components.TriggerExplorer
{
    public class TabItemBT: TabItem
    {
        public TabItemBT(object content, string header)
        {
            this.Content = content;
            this.Header = header;
            this.Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 255));
            this.Background = new SolidColorBrush(Color.FromRgb(64, 64, 64));
            this.BorderBrush = new SolidColorBrush(Color.FromRgb(64, 64, 64));
        }
    }
}
