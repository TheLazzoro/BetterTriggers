using Model.War3Data;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace GUI.Components.TriggerEditor.ParameterControls
{
    public class ButtonUnitType : Button
    {
        private int Width = 32;
        private int Height = 32;

        public ButtonUnitType(UnitType unit)
        {
            Image img = new Image();
            img.Width = Width;
            img.Height = Height;
            BitmapImage bitmap = new BitmapImage();
            bitmap.BeginInit();
            unit.Image.Seek(0, System.IO.SeekOrigin.Begin); // reset back to start of stream
            bitmap.StreamSource = unit.Image;
            bitmap.CacheOption = BitmapCacheOption.OnLoad;
            bitmap.EndInit();
            img.Source = bitmap;

            this.Content = unit.Id;
            this.Content = img;

            this.ToolTip = new ToolTip()
            {
                Content = unit.Id,
            };
        }
    }
}
