using BetterTriggers;
using BetterTriggers.Controllers;
using BetterTriggers.Models.EditorData;
using GUI.Utility;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace GUI.Components.Shared
{
    public class TreeItemHeader : StackPanel
    {
        internal TextBox RenameBox;
        private System.Windows.Shapes.Rectangle Icon;
        private TextBlock DisplayText;
        private string categoryName = string.Empty;

        public enum TreeItemState
        {
            Normal,
            Disabled,
            HasErrors,
            HasErrorsNoTextColor
        }


        public TreeItemHeader(string text, string categoryName, TreeItemState state = TreeItemState.Normal, bool isInitiallyOn = true)
        {
            this.Orientation = Orientation.Horizontal;
            this.Height = 18;
            this.Margin = new Thickness(-1, -1, 0, -1);

            Icon = new System.Windows.Shapes.Rectangle();
            Icon.Width = 16;
            Icon.Height = 16;

            DisplayText = new TextBlock();
            DisplayText.Margin = new Thickness(5, 0, 0, 0);
            DisplayText.FontFamily = new System.Windows.Media.FontFamily("Segoe UI");

            RenameBox = new TextBox();
            RenameBox.Margin = new Thickness(5, 0, 0, 0);

            Category category = Category.Get(categoryName);

            if (category.ShouldDisplay)
            {
                ControllerTriggerData controllerTriggerData = new ControllerTriggerData();
                this.categoryName = Locale.Translate(Category.Get(categoryName).Name) + " - ";
            }

            SetIcon(categoryName, state);
            SetDisplayText(text);
            SetTextEnabled(state, isInitiallyOn);

            this.Children.Add(Icon);
            this.Children.Add(DisplayText);

            this.RenameBox.LostFocus += RenameBox_LostFocus;
        }


        public void SetDisplayText(string text)
        {
            DisplayText.Text = categoryName + text;
            RenameBox.Text = text;
        }

        public string GetDisplayText()
        {
            return DisplayText.Text;
        }

        public void ShowRenameBox(bool doShow)
        {
            if (doShow)
            {
                this.Children.Remove(DisplayText);
                this.Children.Add(RenameBox);
                RenameBox.Focus();
                RenameBox.SelectAll();
            }
            else
            {
                this.Children.Remove(RenameBox);
                this.Children.Add(DisplayText);
            }
        }

        public void SetTextEnabled(TreeItemState state, bool isEnabled)
        {
            if (state == TreeItemState.HasErrors)
                DisplayText.Foreground = (SolidColorBrush)new BrushConverter().ConvertFromString("#C66");
            else if (isEnabled && state == TreeItemState.Normal)
                DisplayText.Foreground = (SolidColorBrush)new BrushConverter().ConvertFromString("#CCC");
            else if (isEnabled && state != TreeItemState.Normal)
                DisplayText.Foreground = (SolidColorBrush)new BrushConverter().ConvertFromString("#CCC");
            else
                DisplayText.Foreground = (SolidColorBrush)new BrushConverter().ConvertFromString("#888");
        }

        public string GetRenameText()
        {
            return RenameBox.Text;
        }


        private void RenameBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (RenameBox.Parent != null)
            {
                ShowRenameBox(false);
                SetDisplayText(DisplayText.Text);
            }
        }

        public void SetIcon(string category, TreeItemState state)
        {
            Bitmap icon = (Bitmap) Category.Get(category).Icon.Clone();
            Bitmap overlay = null;
            Graphics g = Graphics.FromImage(icon);

            if (state == TreeItemState.Disabled || state == TreeItemState.HasErrors)
                overlay = Category.Get(TriggerCategory.TC_ERROR).Icon;
            else if (state == TreeItemState.HasErrorsNoTextColor)
                overlay = Category.Get(TriggerCategory.TC_INVALID).Icon;

            if(overlay != null)
                g.DrawImage(overlay, 0, 0, icon.Width, icon.Height);

            var iconFinal = BitmapConverter.ToBitmapImage(icon);

            ImageBrush brush = new ImageBrush(iconFinal);
            Icon.Fill = brush;
        }

    }
}
