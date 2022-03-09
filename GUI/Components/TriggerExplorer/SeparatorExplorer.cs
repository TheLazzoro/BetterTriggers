using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;
using System.Windows.Media;

namespace GUI.Components.TriggerExplorer
{
    public class SeparatorExplorer: Separator
    {
        public SeparatorExplorer()
        {
            this.BorderThickness = new System.Windows.Thickness(0);
            var background = (SolidColorBrush)new BrushConverter().ConvertFromString("#000000");
            this.Background = background;
            this.Foreground = background;
        }
    }
}
