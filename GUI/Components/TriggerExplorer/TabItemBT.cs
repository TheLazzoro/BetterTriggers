using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace GUI.Components.TriggerExplorer
{
    public class TabItemBT : TabItem
    {
        public TabItemBT(object content, string header)
        {
            this.Style = Application.Current.FindResource("TabItemGlobal") as Style;
            this.Content = content;
            RefreshHeader(header);
        }

        public void RefreshHeader(string header)
        {
            StackPanel panel = new StackPanel();
            panel.Orientation = Orientation.Horizontal;
            panel.Background = (SolidColorBrush)new BrushConverter().ConvertFromString("#00000000");
            this.Background = (SolidColorBrush)new BrushConverter().ConvertFromString("#444");

            TextBlock textBlock = new TextBlock();
            textBlock.Inlines.Add(new Run(header));

            Button btnCloseTab = new Button();
            btnCloseTab.Content = "X";
            btnCloseTab.Width = 12;
            btnCloseTab.Height = 12;

            panel.Children.Add(textBlock);
            panel.Children.Add(btnCloseTab);


            this.Header = panel;
        }
    }
}
