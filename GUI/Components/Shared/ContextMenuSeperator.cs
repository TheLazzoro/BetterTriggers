using System.Windows.Controls;
using System.Windows.Media;

namespace GUI.Components.Shared
{
    public class ContextMenuSeperator: Separator
    {
        public ContextMenuSeperator()
        {
            this.BorderThickness = new System.Windows.Thickness(0);
            var background = (SolidColorBrush)new BrushConverter().ConvertFromString("#000000");
            this.Background = background;
            this.Foreground = background;
        }
    }
}
