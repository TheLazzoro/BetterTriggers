using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;
using System.Windows.Media;

namespace GUI.Components.TriggerExplorer
{
    public class ContextMenuItem : MenuItem
    {
        public ContextMenuItem(string header)
        {
            var background = (SolidColorBrush)new BrushConverter().ConvertFromString("#1E1E1E");
            var foreground = (SolidColorBrush)new BrushConverter().ConvertFromString("#FFFFFF");
            this.Background = background;
            this.Foreground = foreground;
            this.Header = header;
            this.BorderThickness = new System.Windows.Thickness(0);
        }
    }
}
