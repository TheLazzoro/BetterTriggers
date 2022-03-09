using Model.Data;
using Model.EditorData.Enums;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace GUI.Utility
{
    public static class TreeViewManipulator
    {
        public static void SetTreeViewItemAppearance(TreeViewItem treeViewitem, string text, Category iconCategory, bool isValid = true, bool isInitiallyOn = true)
        {
            // create stack panel
            StackPanel stack = new StackPanel();
            stack.Orientation = Orientation.Horizontal;
            stack.Height = 18;
            stack.Margin = new Thickness(0, 0, 0, 0);

            // create category Image
            var group = new DrawingGroup();
            Rectangle rect = new Rectangle();
            var img = GetIconImage(iconCategory);

            // Red cross over image
            if (!isValid)
            {
                img = OverlapImage(img, GetIconImage(Category.Error));
            }

            ImageBrush brush = new ImageBrush(img);
            rect.Fill = brush;
            rect.Width = 16;
            rect.Height = 16;

            // TextBlock
            TextBlock txtBlock = new TextBlock();
            txtBlock.Text = text;
            if(isInitiallyOn)
                txtBlock.Foreground = (SolidColorBrush)new BrushConverter().ConvertFromString("#FFFFFF");
            else
            txtBlock.Foreground = (SolidColorBrush)new BrushConverter().ConvertFromString("#AAAAAA");
            txtBlock.Margin = new Thickness(5, 0, 0, 0);

            // Add into stack
            stack.Children.Add(rect);
            stack.Children.Add(txtBlock);

            // assign stack to header
            treeViewitem.Header = stack;
        }

        private static BitmapImage OverlapImage(BitmapImage source, BitmapImage toOverlap)
        {
            //Assume we have two bitmaps
            WriteableBitmap bitmap1, bitmap2;
            bitmap1 = new WriteableBitmap(source);
            bitmap2 = new WriteableBitmap(toOverlap);

            //Get the pixel arrays of both bitmaps
            int width = bitmap1.PixelWidth;
            int height = bitmap1.PixelHeight;
            int stride = bitmap1.BackBufferStride;

            int[] pixels1 = new int[width * height];
            int[] pixels2 = new int[pixels1.Length];

            bitmap1.CopyPixels(pixels1, stride, 0);
            bitmap2.CopyPixels(pixels2, stride, 0);

            //Detect the rectangle that has difference
            int top = 0, bottom = 0, left = 0, right = 0;
            for (int i = 0; i < pixels1.Length; i++)
            {
                if (pixels1[i] != pixels2[i])
                {
                    int row = i / width;
                    int col = i % width;
                    top = Math.Min(top, row);
                    bottom = Math.Max(bottom, row);
                    left = Math.Min(left, col);
                    right = Math.Max(right, col);
                }
            }
            Int32Rect rect = new Int32Rect(left, top, right - left + 1, bottom - top + 1);

            //Copy pixels of the different rectangle in the second bitmap into another array
            int[] pixelDiff = new int[rect.Width * rect.Height];
            for (int i = 0; i < rect.Width; i++)
            {
                for (int j = 0; j < rect.Height; j++)
                {
                    var pixelOfMerger = pixels2[i + j * rect.Width];
                    if (pixelOfMerger > 60000 || pixelOfMerger < -60000) // here is my addition to this atrocious image merger function
                        pixelDiff[i + j * rect.Width] = pixels2[rect.X + i + (rect.Y + j) * width];
                    else
                        pixelDiff[i + j * rect.Width] = pixels1[rect.X + i + (rect.Y + j) * width];
                }
            }

            //Write the new pixels into the first bitmap
            bitmap1.WritePixels(rect, pixelDiff, stride * rect.Width / width, 0);

            BitmapImage bmImage = new BitmapImage();
            using (MemoryStream stream = new MemoryStream())
            {
                PngBitmapEncoder encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(bitmap1));
                encoder.Save(stream);
                bmImage.BeginInit();
                bmImage.CacheOption = BitmapCacheOption.OnLoad;
                bmImage.StreamSource = stream;
                bmImage.EndInit();
                bmImage.Freeze();
            }

            return bmImage;
        }

        private static BitmapImage GetIconImage(Category iconCategory)
        {
            BitmapImage image = null;
            string path = string.Empty;

            switch (iconCategory)
            {
                case Category.Map:
                    path += "map.png";
                    break;
                case Category.Folder:
                    path += "ui-editoricon-triggercategories_folder.png";
                    break;
                case Category.Trigger:
                    path += "ui-editoricon-triggercategories_element.png";
                    break;
                case Category.Event:
                    path += "editor-triggerevent.png";
                    break;
                case Category.Condition:
                    path += "editor-triggercondition.png";
                    break;
                case Category.LocalVariable:
                    path += "actions-setvariables.png";
                    break;
                case Category.Action:
                    path += "editor-triggeraction.png";
                    break;
                case Category.Ability:
                    path += "actions-ability.png";
                    break;
                case Category.AI:
                    path += "actions-ai.png";
                    break;
                case Category.Animation:
                    path += "actions-animation.png";
                    break;
                case Category.Camera:
                    path += "actions-camera.png";
                    break;
                case Category.Comment:
                    path += "actions-comment.png";
                    break;
                case Category.Destructible:
                    path += "actions-destructibles.png";
                    break;
                case Category.Dialog:
                    path += "actions-dialog.png";
                    break;
                case Category.Environment:
                    path += "actions-environment.png";
                    break;
                case Category.Game:
                    path += "actions-game.png";
                    break;
                case Category.Goldmine:
                    path += "actions-goldmine.png";
                    break;
                case Category.Hero:
                    path += "actions-hero.png";
                    break;
                case Category.Item:
                    path += "actions-item.png";
                    break;
                case Category.Logical:
                    path += "actions-logical.png";
                    break;
                case Category.Melee:
                    path += "actions-melee.png";
                    break;
                case Category.Nothing:
                    path += "actions-nothing.png";
                    break;
                case Category.Player:
                    path += "actions-player.png";
                    break;
                case Category.PlayerGroup:
                    path += "actions-playergroup.png";
                    break;
                case Category.Quest:
                    path += "actions-quest.png";
                    break;
                case Category.Region:
                    path += "actions-region.png";
                    break;
                case Category.SetVariable:
                    path += "actions-setvariables.png";
                    break;
                case Category.Sound:
                    path += "actions-sound.png";
                    break;
                case Category.Timer:
                    path += "events-time.png";
                    break;
                case Category.Unit:
                    path += "actions-unit.png";
                    break;
                case Category.UnitGroup:
                    path += "actions-unitgroup.png";
                    break;
                case Category.UnitSelection:
                    path += "actions-unitselection.png";
                    break;
                case Category.Visibility:
                    path += "actions-visibility.png";
                    break;
                case Category.Wait:
                    path += "actions-wait.png";
                    break;
                case Category.Error:
                    path += "trigger-error.png";
                    break;
                default:
                    break;

            }

            image = new BitmapImage(new Uri(Directory.GetCurrentDirectory() + "/Resources/Icons/" + path));

            return image;
        }
    }
}
