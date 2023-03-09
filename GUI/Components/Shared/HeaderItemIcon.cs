using BetterTriggers.Models.EditorData;
using GUI.Utility;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace GUI.Components.Shared
{
    public class HeaderItemIcon : StackPanel
    {
        public HeaderItemIcon(string text, int iconIndex)
        {
            var icon = BitmapConverter.GetSystemIcon(iconIndex);
            var image = BitmapConverter.ToBitmapImage(icon);
            Create(text, image);
        }

        public HeaderItemIcon(string text, Category category)
        {
            var icon = category.Icon;
            var image = BitmapConverter.ToBitmapImage(icon);
            Create(text, image);
        }

        private void Create(string text, BitmapImage image)
        {
            this.Orientation = Orientation.Horizontal;
            this.Margin = new Thickness(-1, -1, 0, -1);
            this.Height = 17;

            var img = new Image();
            img.Width = 16;
            img.Height = 16;
            img.Source = image;
            img.VerticalAlignment = VerticalAlignment.Top;
            this.Children.Add(img);

            var separator = new Separator();
            separator.Width = 5;
            separator.Background = new SolidColorBrush(Colors.Transparent);
            this.Children.Add(separator);

            var textBlock = new TextBlock();
            textBlock.Text = text;
            this.Children.Add(textBlock);
        }
    }
}
