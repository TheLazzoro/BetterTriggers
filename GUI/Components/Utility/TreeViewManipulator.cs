using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace GUI.Components.Utility
{
    public static class TreeViewManipulator
    {
        public static void SetTreeViewItemAppearance(TreeViewItem treeViewitem, string text, string imagePath)
        {
            // create stack panel
            StackPanel stack = new StackPanel();
            stack.Orientation = Orientation.Horizontal;
            stack.Height = 18;
            stack.Margin = new Thickness(0, 0, 0, 0);

            // create Image
            Rectangle rect = new Rectangle();
            var img = new BitmapImage(new Uri(Directory.GetCurrentDirectory() + "/" + imagePath));
            ImageBrush brush = new ImageBrush(img);
            rect.Fill = brush;
            rect.Width = 16;
            rect.Height = 16;

            // TextBlock
            TextBlock txtBlock = new TextBlock();
            txtBlock.Text = text;
            txtBlock.Foreground = (SolidColorBrush)new BrushConverter().ConvertFromString("#FFFFFF");
            txtBlock.Margin = new Thickness(5, 0, 0, 0);

            // Add into stack
            stack.Children.Add(rect);
            stack.Children.Add(txtBlock);

            // assign stack to header
            treeViewitem.Header = stack;
        }
    }
}
