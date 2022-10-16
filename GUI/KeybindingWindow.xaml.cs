using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace GUI
{
    public partial class KeybindingWindow : Window
    {
        public bool OK { get; internal set; }
        public Keybindings keybindings { get; }

        public KeybindingWindow(Keybindings keybindings)
        {
            this.Owner = MainWindow.GetMainWindow();
            InitializeComponent();

            this.keybindings = keybindings;
            foreach (PropertyInfo property in keybindings.GetType().GetProperties())
            {
                Keybinding keybinding = (Keybinding)property.GetValue(keybindings);


                Grid grid = new Grid();
                RowDefinition rowDefinition = new RowDefinition() { Height = new GridLength(30) };
                ColumnDefinition columnDefinition0 = new ColumnDefinition() { Width = new GridLength(50, GridUnitType.Star) };
                ColumnDefinition columnDefinition1 = new ColumnDefinition() { Width = new GridLength(110) };
                ColumnDefinition columnDefinition2 = new ColumnDefinition() { Width = new GridLength(100) };
                grid.RowDefinitions.Add(rowDefinition);
                grid.ColumnDefinitions.Add(columnDefinition0);
                grid.ColumnDefinitions.Add(columnDefinition1);
                grid.ColumnDefinitions.Add(columnDefinition2);
                stackPanelVertical.Children.Add(grid);
                if (stackPanelVertical.Children.Count % 2 == 0)
                    grid.Background = (SolidColorBrush)new BrushConverter().ConvertFromString("#444");

                Label label = new Label();
                Grid.SetColumn(label, 0);
                label.Foreground = (SolidColorBrush)new BrushConverter().ConvertFromString("#CCC");
                label.Content = GetKeybindingName(property.Name);
                grid.Children.Add(label);

                ComboBox comboBox = new ComboBox();
                foreach (ModifierKeys modifier in Enum.GetValues(typeof(ModifierKeys)))
                {
                    ComboBoxItem item = new ComboBoxItem();
                    item.Content = modifier.ToString();
                    item.Tag = modifier;
                    comboBox.Items.Add(item);
                    if(keybinding.modifier == modifier)
                        comboBox.SelectedItem = item;
                }
                comboBox.Height = 22;
                Grid.SetColumn(comboBox, 1);
                grid.Children.Add(comboBox);
                comboBox.SelectionChanged += delegate
                {
                    ComboBoxItem selected = (ComboBoxItem)comboBox.SelectedItem;
                    keybinding.modifier = (ModifierKeys)selected.Tag;
                };

                Button button = new Button();
                button.Height = 22;
                button.Width = 75;
                button.Content = keybinding.key.ToString();
                Grid.SetColumn(button, 2);
                grid.Children.Add(button);
                button.Click += delegate
                {
                    SetKeyWindow window = new SetKeyWindow();
                    window.ShowDialog();
                    if (window.ok == false)
                        return;
                    keybinding.key = window.key;
                    button.Content = keybinding.key;
                };
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
                case "NewEvent":
                    memberName = "New Event";
                    break;
                case "NewCondition":
                    memberName = "New Condition";
                    break;
                case "NewAction":
                    memberName = "New Action";
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
