using DataAccess.Data;
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
        public static void SetTreeViewItemAppearance(TreeViewItem treeViewitem, string text, EnumCategory iconCategory)
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

        private static BitmapImage GetIconImage(EnumCategory iconCategory)
        {
            BitmapImage image = null;
            string path = string.Empty;

            switch (iconCategory)
            {
                case EnumCategory.Map:
                    path += "map.png";
                    break;
                case EnumCategory.Folder:
                    path += "ui-editoricon-triggercategories_folder.png";
                    break;
                case EnumCategory.Trigger:
                    path += "ui-editoricon-triggercategories_element.png";
                    break;
                case EnumCategory.Event:
                    path += "editor-triggerevent.png";
                    break;
                case EnumCategory.Condition:
                    path += "editor-triggercondition.png";
                    break;
                case EnumCategory.LocalVariable:
                    path += "actions-setvariables.png";
                    break;
                case EnumCategory.Action:
                    path += "editor-triggeraction.png";
                    break;
                case EnumCategory.Ability:
                    path += "actions-ability.png";
                    break;
                case EnumCategory.AI:
                    path += "actions-ai.png";
                    break;
                case EnumCategory.Animation:
                    path += "actions-animation.png";
                    break;
                case EnumCategory.Camera:
                    path += "actions-camera.png";
                    break;
                case EnumCategory.Comment:
                    path += "actions-comment.png";
                    break;
                case EnumCategory.Destructible:
                    path += "actions-destructibles.png";
                    break;
                case EnumCategory.Dialog:
                    path += "actions-dialog.png";
                    break;
                case EnumCategory.Environment:
                    path += "actions-environment.png";
                    break;
                case EnumCategory.Game:
                    path += "actions-game.png";
                    break;
                case EnumCategory.Goldmine:
                    path += "actions-goldmine.png";
                    break;
                case EnumCategory.Hero:
                    path += "actions-hero.png";
                    break;
                case EnumCategory.Item:
                    path += "actions-item.png";
                    break;
                case EnumCategory.Logical:
                    path += "actions-logical.png";
                    break;
                case EnumCategory.Melee:
                    path += "actions-melee.png";
                    break;
                case EnumCategory.Nothing:
                    path += "actions-nothing.png";
                    break;
                case EnumCategory.Player:
                    path += "actions-player.png";
                    break;
                case EnumCategory.PlayerGroup:
                    path += "actions-playergroup.png";
                    break;
                case EnumCategory.Quest:
                    path += "actions-quest.png";
                    break;
                case EnumCategory.Region:
                    path += "actions-region.png";
                    break;
                case EnumCategory.SetVariable:
                    path += "actions-setvariables.png";
                    break;
                case EnumCategory.Sound:
                    path += "actions-sound.png";
                    break;
                case EnumCategory.Timer:
                    path += "events-time.png";
                    break;
                case EnumCategory.Unit:
                    path += "actions-unit.png";
                    break;
                case EnumCategory.UnitGroup:
                    path += "actions-unitgroup.png";
                    break;
                case EnumCategory.UnitSelection:
                    path += "actions-unitselection.png";
                    break;
                case EnumCategory.Visibility:
                    path += "actions-visibility.png";
                    break;
                case EnumCategory.Wait:
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
