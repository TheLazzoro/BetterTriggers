using BetterTriggers;
using BetterTriggers.Models.EditorData;
using GUI.Components.TriggerEditor;
using GUI.Utility;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    public class TreeItemHeader : Grid
    {
        internal TextBox RenameBox;
        internal bool isRenaming { get; private set; }
        private StackPanel stackPanel;
        private System.Windows.Shapes.Rectangle Icon;
        private System.Windows.Shapes.Rectangle IconOverlay;
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
            this.Height = 18;
            this.Margin = new Thickness(-1, -1, 0, -1);
            ColumnDefinition column0 = new ColumnDefinition();
            ColumnDefinition column1 = new ColumnDefinition();
            column0.Width = new GridLength(16);
            column1.Width = GridLength.Auto;
            this.ColumnDefinitions.Add(column0);
            this.ColumnDefinitions.Add(column1);

            int size = 16;
            Icon = new System.Windows.Shapes.Rectangle();
            Icon.Width = size;
            Icon.Height = size;
            this.Children.Add(Icon);
            IconOverlay = new System.Windows.Shapes.Rectangle();
            IconOverlay.Width = size;
            IconOverlay.Height = size;
            this.Children.Add(IconOverlay);

            stackPanel = new StackPanel();
            stackPanel.Orientation = Orientation.Horizontal;
            this.Children.Add(stackPanel);
            Grid.SetColumn(stackPanel, 1);

            DisplayText = new TextBlock();
            DisplayText.Margin = new Thickness(5, 0, 0, 0);
            DisplayText.FontFamily = TriggerEditorFont.GetTreeItemFont();
            DisplayText.FontSize = TriggerEditorFont.GetTreeItemFontSize();
            stackPanel.Children.Add(DisplayText);

            RenameBox = new TextBox();
            RenameBox.Margin = new Thickness(5, 0, 0, 0);
            Grid.SetColumn(RenameBox, 1);
            this.RenameBox.LostFocus += RenameBox_LostFocus;

        }

        public TreeItemHeader(string text, string categoryName, TreeItemState state = TreeItemState.Normal, bool isInitiallyOn = true)
        {
            this.Height = 18;
            this.Margin = new Thickness(-1, -1, 0, -1);
            ColumnDefinition column0 = new ColumnDefinition();
            ColumnDefinition column1 = new ColumnDefinition();
            column0.Width = new GridLength(16);
            column1.Width = GridLength.Auto;
            this.ColumnDefinitions.Add(column0);
            this.ColumnDefinitions.Add(column1);

            int size = 16;
            Icon = new System.Windows.Shapes.Rectangle();
            Icon.Width = size;
            Icon.Height = size;
            this.Children.Add(Icon);
            IconOverlay = new System.Windows.Shapes.Rectangle();
            IconOverlay.Width = size;
            IconOverlay.Height = size;
            this.Children.Add(IconOverlay);

            stackPanel = new StackPanel();
            stackPanel.Orientation = Orientation.Horizontal;
            this.Children.Add(stackPanel);
            Grid.SetColumn(stackPanel, 1);

            DisplayText = new TextBlock();
            DisplayText.Margin = new Thickness(5, 0, 0, 0);
            DisplayText.FontFamily = new System.Windows.Media.FontFamily("Segoe UI");
            stackPanel.Children.Add(DisplayText);

            RenameBox = new TextBox();
            RenameBox.Margin = new Thickness(5, 0, 0, 0);
            Grid.SetColumn(RenameBox, 1);
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
                stackPanel.Children.Remove(DisplayText);
                this.Children.Add(RenameBox);
                RenameBox.Focus();
                RenameBox.SelectAll();
            }
            else
            {
                this.Children.Remove(RenameBox);
                stackPanel.Children.Add(DisplayText);
            }
        }

        public void SetTextEnabled(TreeItemState state, bool isEnabled)
        {
            if (state == TreeItemState.HasErrors)
                DisplayText.SetResourceReference(TextBlock.ForegroundProperty, "HyperlinkErrorBrush");
            else if (isEnabled && state == TreeItemState.Normal)
                DisplayText.SetResourceReference(TextBlock.ForegroundProperty, EditorTheme.TreeItemTextColor());
            else if (isEnabled && state != TreeItemState.Normal)
                DisplayText.SetResourceReference(TextBlock.ForegroundProperty, EditorTheme.TreeItemTextColor());
            else
                DisplayText.SetResourceReference(TextBlock.ForegroundProperty, "TreeItemDisabled");
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
            BitmapImage icon = CategoryExtension.getImageByCategory(category);
            BitmapImage overlay = null;
            IconOverlay.Fill = null;

            if (state == TreeItemState.Disabled || state == TreeItemState.HasErrors)
                overlay = CategoryExtension.getImageByCategory(TriggerCategory.TC_ERROR);
            else if (state == TreeItemState.HasErrorsNoTextColor)
                overlay = CategoryExtension.getImageByCategory(TriggerCategory.TC_INVALID);

            if(overlay != null)
            {
                var overlayFinal = overlay;
                ImageBrush overlayBrush = new ImageBrush(overlayFinal);
                IconOverlay.Fill = overlayBrush;
            }

            var iconFinal = icon;

            ImageBrush brush = new ImageBrush(iconFinal);
            Icon.Fill = brush;
        }

    }
}
