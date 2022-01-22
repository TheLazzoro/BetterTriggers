using Model.Data;
using Model.Enums;
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
        public static void SetTreeViewItemAppearance(TreeViewItem treeViewitem, string text, Category iconCategory)
        {
            // create stack panel
            StackPanel stack = new StackPanel();
            stack.Orientation = Orientation.Horizontal;
            stack.Height = 18;
            stack.Margin = new Thickness(0, 0, 0, 0);

            // create Image
            Rectangle rect = new Rectangle();
            var img = GetIconImage(iconCategory);
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
                default:
                    break;

            }

            image = new BitmapImage(new Uri(Directory.GetCurrentDirectory() + "/Resources/Icons/" + path));

            return image;
        }
    }
}
