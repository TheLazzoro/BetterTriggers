using BetterTriggers;
using BetterTriggers.Controllers;
using BetterTriggers.Models.EditorData;
using GUI.Components.TriggerEditor;
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
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace GUI.Components.Shared
{
    public class TreeItemHeader : StackPanel
    {
        internal TextBox RenameBox;
        internal bool isRenaming { get; private set; }
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

        public TreeItemHeader() {
            this.Orientation = Orientation.Horizontal;
            this.Height = 18;
            this.Margin = new Thickness(-1, -1, 0, -1);

            Icon = new System.Windows.Shapes.Rectangle();
            Icon.Width = 16;
            Icon.Height = 16;
            this.Children.Add(Icon);

            DisplayText = new TextBlock();
            DisplayText.Margin = new Thickness(5, 0, 0, 0);
            DisplayText.FontFamily = TriggerEditorFont.GetTreeItemFont();
            DisplayText.FontSize = TriggerEditorFont.GetTreeItemFontSize();
            this.Children.Add(DisplayText);

            RenameBox = new TextBox();
            RenameBox.Margin = new Thickness(5, 0, 0, 0);
            this.RenameBox.LostFocus += RenameBox_LostFocus;

        }

        public TreeItemHeader(string text, string categoryName, TreeItemState state = TreeItemState.Normal, bool isInitiallyOn = true)
        {
            this.Orientation = Orientation.Horizontal;
            this.Height = 18;
            this.Margin = new Thickness(-1, -1, 0, -1);

            Icon = new System.Windows.Shapes.Rectangle();
            Icon.Width = 16;
            Icon.Height = 16;
            this.Children.Add(Icon);

            DisplayText = new TextBlock();
            DisplayText.Margin = new Thickness(5, 0, 0, 0);
            DisplayText.FontFamily = new System.Windows.Media.FontFamily("Segoe UI");
            this.Children.Add(DisplayText);

            RenameBox = new TextBox();
            RenameBox.Margin = new Thickness(5, 0, 0, 0);
            RenameBox.LostFocus += RenameBox_LostFocus;

            Refresh(text, categoryName, state, isInitiallyOn);
        }

        public void Refresh(string text, string categoryName, TreeItemState state = TreeItemState.Normal, bool isInitiallyOn = true)
        {
            _Refresh(categoryName, state, isInitiallyOn);
            SetDisplayText(text);
        }

        public void Refresh(List<Inline> inlines, string categoryName, TreeItemState state = TreeItemState.Normal, bool isInitiallyOn = true)
        {
            _Refresh(categoryName, state, isInitiallyOn);
            SetDisplayText(inlines);
        }

        private void _Refresh(string categoryName, TreeItemState state = TreeItemState.Normal, bool isInitiallyOn = true)
        {
            Category category = Category.Get(categoryName);
            if (category.ShouldDisplay)
            {
                this.categoryName = Locale.Translate(Category.Get(categoryName).Name) + " - ";
            }

            SetIcon(categoryName, state);
            SetTextEnabled(state, isInitiallyOn);
        }

        public void SetDisplayText(string text)
        {
            DisplayText.Text = categoryName + text;
            RenameBox.Text = text;
        }

        public void SetDisplayText(List<Inline> inlines)
        {
            DisplayText.Inlines.Clear();
            DisplayText.Inlines.Add(categoryName);
            DisplayText.Inlines.AddRange(inlines);
        }

        public string GetDisplayText()
        {
            return DisplayText.Text;
        }

        public void ShowRenameBox(bool doShow)
        {
            isRenaming = doShow;
            if (doShow && !this.Children.Contains(RenameBox))
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
                this.SetResourceReference(TextBlock.ForegroundProperty, EditorTheme.TreeItemTextColor());
            else if (isEnabled && state != TreeItemState.Normal)
                this.SetResourceReference(TextBlock.ForegroundProperty, EditorTheme.TreeItemTextColor());
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
