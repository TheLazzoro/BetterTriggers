using GUI.Components.Dialogs;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace GUI.Components.Keybindings
{
    public partial class KeybindingWindow : Window
    {
        public bool OK { get; internal set; }
        public Keybindings keybindings { get; }

        private List<Button> buttons = new List<Button>();
        private List<ComboBox> comboBoxes = new List<ComboBox>();

        // Styles
        SolidColorBrush textColor;
        SolidColorBrush backgroundLight;

        class ComboBoxKeybindTag
        {
            public Keybinding keybinding;
            public ModifierKeys modifier;
            public ComboBoxItem PreviousSelected;
        }

        public KeybindingWindow(Keybindings keybindings)
        {
            this.Owner = MainWindow.GetMainWindow();
            InitializeComponent();
            textColor = (SolidColorBrush)Application.Current.FindResource("TextBrush");
            backgroundLight = (SolidColorBrush)Application.Current.FindResource("BackgroundLight");

            this.keybindings = keybindings;
            foreach (PropertyInfo property in keybindings.GetType().GetProperties())
            {
                Keybinding keybinding = (Keybinding)property.GetValue(keybindings);


                Grid grid = new Grid();
                RowDefinition rowDefinition = new RowDefinition() { Height = new GridLength(30) };
                ColumnDefinition columnDefinition0 = new ColumnDefinition() { Width = new GridLength(50, GridUnitType.Star) };
                ColumnDefinition columnDefinition1 = new ColumnDefinition() { Width = new GridLength(110) };
                ColumnDefinition columnDefinition2 = new ColumnDefinition() { Width = new GridLength(110) };
                ColumnDefinition columnDefinition3 = new ColumnDefinition() { Width = new GridLength(100) };
                grid.RowDefinitions.Add(rowDefinition);
                grid.ColumnDefinitions.Add(columnDefinition0);
                grid.ColumnDefinitions.Add(columnDefinition1);
                grid.ColumnDefinitions.Add(columnDefinition2);
                grid.ColumnDefinitions.Add(columnDefinition3);
                stackPanelVertical.Children.Add(grid);
                if (stackPanelVertical.Children.Count % 2 == 0)
                    grid.Background = backgroundLight;

                Label label = new Label();
                Grid.SetColumn(label, 0);
                label.Foreground = textColor;
                label.Content = GetKeybindingName(property.Name);
                grid.Children.Add(label);

                if(property.Name == "BuildMap" || property.Name == "TestMap" || property.Name == "ValidateTriggers")
                {
                    CheckBox checkBox = new CheckBox();
                    checkBox.Content = "Global Key";
                    checkBox.IsChecked = keybinding.global;
                    checkBox.Foreground = textColor;
                    checkBox.VerticalAlignment = VerticalAlignment.Center;
                    checkBox.ToolTip = "Hotkey presses register without the window being in focus.";
                    Grid.SetColumn(checkBox, 1);
                    grid.Children.Add(checkBox);
                    checkBox.Click += delegate
                    {
                        keybinding.global = (bool)checkBox.IsChecked;
                    };
                }

                ComboBox comboBox = new ComboBox();
                var keybindTag = new ComboBoxKeybindTag();
                foreach (ModifierKeys modifier in Enum.GetValues(typeof(ModifierKeys)))
                {
                    ComboBoxItem item = new ComboBoxItem();
                    item.Content = modifier.ToString();
                    item.Tag = modifier;
                    comboBox.Items.Add(item);
                    if (keybinding.modifier == modifier)
                    {
                        comboBox.SelectedItem = item;
                        keybindTag.PreviousSelected = item;
                    }
                }
                comboBox.Height = 22;
                comboBox.Tag = keybindTag;
                Grid.SetColumn(comboBox, 2);
                grid.Children.Add(comboBox);
                comboBoxes.Add(comboBox);
                comboBox.SelectionChanged += new SelectionChangedEventHandler(delegate (object sender, SelectionChangedEventArgs e)
                {
                    ComboBoxItem selected = (ComboBoxItem)comboBox.SelectedItem;
                    ModifierKeys modifier = (ModifierKeys)selected.Tag;

                    if (selected == keybindTag.PreviousSelected)
                        return;

                    if (keybindings.IsKeybindingAlreadySet(modifier, keybinding.key))
                    {
                        DialogBox dialog = new DialogBox("Confirm keybinding",
                            $"This keybinding '{Keybindings.GetModifierText(modifier)}+{keybinding.key}' is already set. Do you wish to rebind it?");
                        dialog.ShowDialog();
                        if (!dialog.OK)
                        {
                            comboBox.SelectedItem = keybindTag.PreviousSelected;
                            return;
                        }
                    }

                    keybindTag.PreviousSelected = selected;
                    keybindings.UnbindKeybinding(modifier, keybinding.key);
                    keybinding.modifier = modifier;
                    RefreshUI();
                });

                Button button = new Button();
                button.Height = 22;
                button.Width = 75;
                button.Content = keybinding.key.ToString();
                button.Tag = keybinding;
                Grid.SetColumn(button, 3);
                grid.Children.Add(button);
                buttons.Add(button);
                button.Click += delegate
                {
                    SetKeyWindow window = new SetKeyWindow();
                    window.ShowDialog();
                    if (window.ok == false)
                        return;

                    if (keybindings.IsKeybindingAlreadySet(keybinding.modifier, window.key))
                    {
                        DialogBox dialog = new DialogBox("Confirm keybinding",
                            $"This keybinding '{Keybindings.GetModifierText(keybinding.modifier)}+{window.key}' is already set. Do you wish to overwrite?");
                        dialog.ShowDialog();
                        if (!dialog.OK)
                            return;
                    }

                    keybindings.UnbindKeybinding(keybinding.modifier, window.key);
                    keybinding.key = window.key;
                    RefreshUI();
                };
            }
        }

        private void RefreshUI()
        {
            foreach (var button in buttons)
            {
                Keybinding keybinding = button.Tag as Keybinding;
                button.Content = keybinding.key;
            }
        }


        private string GetKeybindingName(string memberName)
        {
            switch (memberName)
            {
                case "NewProject":
                    memberName = "New Project";
                    break;
                case "OpenProject":
                    memberName = "Open Project";
                    break;
                case "SaveProject":
                    memberName = "Save Project";
                    break;
                case "Undo":
                    memberName = "Undo";
                    break;
                case "Redo":
                    memberName = "Redo";
                    break;
                case "NewCategory":
                    memberName = "New Category";
                    break;
                case "NewTrigger":
                    memberName = "New Trigger";
                    break;
                case "NewScript":
                    memberName = "New Script";
                    break;
                case "NewGlobalVariable":
                    memberName = "New Global Variable";
                    break;
                case "NewLocalVariable":
                    memberName = "New Local Variable";
                    break;
                case "NewEvent":
                    memberName = "New Event";
                    break;
                case "NewCondition":
                    memberName = "New Condition";
                    break;
                case "NewAction":
                    memberName = "New Action";
                    break;
                case "EnableDisableFunctions":
                    memberName = "Enable/Disable Functions";
                    break;
                case "ValidateTriggers":
                    memberName = "Validate Map Triggers";
                    break;
                case "TestMap":
                    memberName = "Test Map";
                    break;
                case "BuildMap":
                    memberName = "Build Map";
                    break;
                default:
                    memberName = "MISSING STRING!!!";
                    break;
            }

            return memberName;
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            OK = true;
            this.Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
