using BetterTriggers.Models.EditorData;
using GUI.Utility;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Xceed.Wpf.Toolkit;

namespace GUI.Components.TriggerEditor.ParameterControls
{
    public partial class ValueControlString : UserControl, IValueControl
    {
        string returnType;
        static string color0 = "FFFFFF";
        static string color1 = "000000";

        public event EventHandler SelectionChanged;
        public event EventHandler OK;

        public ValueControlString(string returnType)
        {
            InitializeComponent();

            this.returnType = returnType;

            colorPicker0.ShowAvailableColors = false;
            colorPicker1.ShowAvailableColors = false;

            colorPicker0.SelectedColor = (Color)ColorConverter.ConvertFromString("#" + color0);
            colorPicker1.SelectedColor = (Color)ColorConverter.ConvertFromString("#" + color1);
            UpdateGradientBar();

            ObservableCollection<ColorItem> collection = new ObservableCollection<ColorItem>();
            collection.Add(new ColorItem((Color)ColorConverter.ConvertFromString("#FFFFFF"), "White"));
            collection.Add(new ColorItem((Color)ColorConverter.ConvertFromString("#000000"), "Black"));
            collection.Add(new ColorItem((Color)ColorConverter.ConvertFromString("#808080"), "Gray"));
            collection.Add(new ColorItem((Color)ColorConverter.ConvertFromString("#FF0000"), "Red"));
            collection.Add(new ColorItem((Color)ColorConverter.ConvertFromString("#00FF00"), "Green"));
            collection.Add(new ColorItem((Color)ColorConverter.ConvertFromString("#0000FF"), "Blue"));
            collection.Add(new ColorItem((Color)ColorConverter.ConvertFromString("#FFFF00"), "Yellow"));
            collection.Add(new ColorItem((Color)ColorConverter.ConvertFromString("#FF00FF"), "Magenta"));
            collection.Add(new ColorItem((Color)ColorConverter.ConvertFromString("#00FFFF"), "Teal"));
            collection.Add(new ColorItem((Color)ColorConverter.ConvertFromString("#6F2583"), "Purple"));
            collection.Add(new ColorItem((Color)ColorConverter.ConvertFromString("#D45E19"), "Orange"));
            collection.Add(new ColorItem((Color)ColorConverter.ConvertFromString("#FF8080"), "Pink"));

            collection.Add(new ColorItem((Color)ColorConverter.ConvertFromString("#FF0402"), "Player 1 (Red)"));
            collection.Add(new ColorItem((Color)ColorConverter.ConvertFromString("#0042FF"), "Player 2 (Blue)"));
            collection.Add(new ColorItem((Color)ColorConverter.ConvertFromString("#1BE6BA"), "Player 3 (Teal)"));
            collection.Add(new ColorItem((Color)ColorConverter.ConvertFromString("#550081"), "Player 4 (Purple)"));
            collection.Add(new ColorItem((Color)ColorConverter.ConvertFromString("#FFFB00"), "Player 5 (Yellow)"));
            collection.Add(new ColorItem((Color)ColorConverter.ConvertFromString("#FF8A0D"), "Player 6 (Orange)"));
            collection.Add(new ColorItem((Color)ColorConverter.ConvertFromString("#20BF00"), "Player 7 (Green)"));
            collection.Add(new ColorItem((Color)ColorConverter.ConvertFromString("#E35BAF"), "Player 8 (Pink)"));
            collection.Add(new ColorItem((Color)ColorConverter.ConvertFromString("#949697"), "Player 9 (Gray)"));
            collection.Add(new ColorItem((Color)ColorConverter.ConvertFromString("#7EBFF1"), "Player 10 (Light Blue)"));
            collection.Add(new ColorItem((Color)ColorConverter.ConvertFromString("#106247"), "Player 11 (Dark Green)"));
            collection.Add(new ColorItem((Color)ColorConverter.ConvertFromString("#4F2B05"), "Player 12 (Brown)"));
            collection.Add(new ColorItem((Color)ColorConverter.ConvertFromString("#9C0000"), "Player 13 (Maroon)"));
            collection.Add(new ColorItem((Color)ColorConverter.ConvertFromString("#0000C2"), "Player 14 (Navy)"));
            collection.Add(new ColorItem((Color)ColorConverter.ConvertFromString("#00EBFF"), "Player 15 (Turquoise)"));
            collection.Add(new ColorItem((Color)ColorConverter.ConvertFromString("#BD00FF"), "Player 16 (Violet)"));
            collection.Add(new ColorItem((Color)ColorConverter.ConvertFromString("#ECCC86"), "Player 17 (Wheat)"));
            collection.Add(new ColorItem((Color)ColorConverter.ConvertFromString("#F7A48B"), "Player 18 (Peach)"));
            collection.Add(new ColorItem((Color)ColorConverter.ConvertFromString("#BFFF80"), "Player 19 (Mint)"));
            collection.Add(new ColorItem((Color)ColorConverter.ConvertFromString("#DBB8EC"), "Player 20 (Lavender)"));
            collection.Add(new ColorItem((Color)ColorConverter.ConvertFromString("#4F4F55"), "Player 21 (Coal)"));
            collection.Add(new ColorItem((Color)ColorConverter.ConvertFromString("#ECF0FF"), "Player 22 (Snow)"));
            collection.Add(new ColorItem((Color)ColorConverter.ConvertFromString("#00781E"), "Player 23 (Emerald)"));
            collection.Add(new ColorItem((Color)ColorConverter.ConvertFromString("#A46F34"), "Player 24 (Peanut)"));
            collection.Add(new ColorItem((Color)ColorConverter.ConvertFromString("#2E2D2E"), "Player Hostile (Black)"));

            colorPicker0.StandardColors = collection;
            colorPicker1.StandardColors = collection;

            this.Loaded += ValueControlString_Loaded;
            this.KeyDown += ValueControlString_KeyDown;
        }

        private void ValueControlString_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter)
                return;

            EventHandler handler = OK;
            handler?.Invoke(this, e);
        }

        private void ValueControlString_Loaded(object sender, RoutedEventArgs e)
        {
            EventHandler handler = SelectionChanged;
            handler?.Invoke(this, e);
        }

        public void SetDefaultSelection(Parameter parameter)
        {
            textBoxString.Text = parameter.value;
        }

        public int GetElementCount()
        {
            return 1;
        }

        public Parameter GetSelected()
        {
            Value value = new Value()
            {
                value = textBoxString.Text,
            };

            return value;
        }

        private void colorPicker0_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            string color = colorPicker0.SelectedColor.ToString();
            color0 = color.Substring(3, color.Length - 3);
            UpdateGradientBar();
        }

        private void colorPicker1_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            string color = colorPicker1.SelectedColor.ToString();
            color1 = color.Substring(3, color.Length - 3);
            UpdateGradientBar();
        }

        private void textBoxString_TextChanged(object sender, TextChangedEventArgs e)
        {
            textBlockPreview.Inlines.Clear();
            textBlockPreview.Inlines.AddRange(TextFormatter.War3ColoredText(textBoxString.Text));
        }

        private void btnApplyColor0_Click(object sender, RoutedEventArgs e)
        {
            InsertColor("|cff" + color0);
        }

        private void btnApplyColor1_Click(object sender, RoutedEventArgs e)
        {
            InsertColor("|cff" + color1);
        }


        private void btnNewLine_Click(object sender, RoutedEventArgs e)
        {
            int index = textBoxString.CaretIndex;
            textBoxString.Text = textBoxString.Text.Insert(index, "|n");
            textBoxString.CaretIndex = index;
        }

        private void InsertColor(string colorCode)
        {
            int start = textBoxString.SelectionStart;
            int length = textBoxString.SelectionLength + colorCode.Length;
            textBoxString.Text = textBoxString.Text.Insert(start, colorCode);
            textBoxString.Text = textBoxString.Text.Insert(start + length, "|r");

            textBoxString.Focus();
            textBoxString.Select(start, length + 2);
        }

        private void UpdateGradientBar()
        {
            gradient0.Color = (Color)ColorConverter.ConvertFromString("#" + color0);
            gradient1.Color = (Color)ColorConverter.ConvertFromString("#" + color1);
        }

        private void InsertGradient()
        {
            int start = textBoxString.SelectionStart;
            int length = textBoxString.SelectionLength;
            //gradientBrush.

            throw new NotImplementedException();
        }
    }
}
