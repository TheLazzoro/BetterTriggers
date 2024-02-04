using BetterTriggers.Models.War3Data;
using BetterTriggers.WorldEdit;
using GUI.Utility;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace GUI.Components.TriggerEditor.ParameterControls
{
    public class ButtonUnitType : Button
    {
        public string UnitType { get; }
        public bool isSpecial;
        public string Category;
        private int Width = 32;
        private int Height = 32;
        private Brush defaultBorderBrush;

        public ButtonUnitType(UnitType unit)
        {
            this.UnitType = unit.Id;
            this.isSpecial = unit.isSpecial;
            Image img = new Image();
            img.Width = Width;
            img.Height = Height;
            img.Stretch = Stretch.Fill;
            img.Source = BitmapConverter.ByteToImage(unit.Image);



            this.Content = img;
            this.Background = (SolidColorBrush)new BrushConverter().ConvertFromString("#1E1E1E");
            this.defaultBorderBrush = this.BorderBrush;

            this.ToolTip = new ToolTip()
            {
                Content = $"[{unit.Id}] {UnitTypes.GetName(unit.Id)}",
            };

            this.Click += delegate
            {
                AddSelectedBorder();
            };
        }

        public void AddSelectedBorder()
        {
            this.BorderBrush = (SolidColorBrush)new BrushConverter().ConvertFromString("#00FF00");
        }

        public void RemoveSelectedBorder()
        {
            this.BorderBrush = defaultBorderBrush;
        }
    }
}
