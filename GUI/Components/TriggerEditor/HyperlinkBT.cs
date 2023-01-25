using BetterTriggers;
using BetterTriggers.Models.SaveableData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace GUI.Components.TriggerEditor
{
    public class HyperlinkBT : Hyperlink
    {
        internal Parameter parameter { get; }

        public HyperlinkBT(Parameter parameter, string text)
        {
            this.Inlines.Add(text);
            this.parameter = parameter;

            var settings = Settings.Load();

            // Create an underline text decoration.
            if (settings.triggerEditorMode == 0)
            {
                TextDecoration underline = new TextDecoration();
                underline.PenOffset = 2; // Underline offset
                underline.PenThicknessUnit = TextDecorationUnit.FontRecommended;

                TextDecorationCollection decorations = new TextDecorationCollection();
                decorations.Add(underline);
                this.TextDecorations = decorations;
            }
            else if (settings.triggerEditorMode == 1)
            {
                this.TextDecorations = new TextDecorationCollection(); // none
            }

            this.GotFocus += HyperlinkParameter_GotFocus;
            this.LostFocus += HyperlinkParameter_LostFocus;

            this.FontFamily = TriggerEditorFont.GetParameterFont();
            this.FontSize = TriggerEditorFont.GetParameterFontSize();
            this.MouseEnter += HyperlinkBT_MouseEnter;
            this.MouseLeave += HyperlinkBT_MouseLeave;
            RecolorHyperlink();
        }


        private void HyperlinkBT_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            this.Background = new SolidColorBrush(Color.FromRgb(80, 80, 80));
        }

        private void HyperlinkBT_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            this.Background = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));
        }

        /// <summary>
        /// Disables and greys out hyperlink.
        /// </summary>
        public void Disable()
        {
            this.IsEnabled = false;
            this.Inlines.Clear();
            this.Inlines.Add("Value");
            this.Foreground = new SolidColorBrush(Color.FromRgb(150, 150, 150));
        }

        private void HyperlinkParameter_LostFocus(object sender, RoutedEventArgs e)
        {
            RecolorHyperlink();
        }

        private void HyperlinkParameter_GotFocus(object sender, RoutedEventArgs e)
        {
            RecolorHyperlink();
        }

        private void RecolorHyperlink()
        {
            Settings settings = Settings.Load();
            byte r = settings.triggerEditorMode == 0 ? (byte)0 : (byte)105;
            byte g = settings.triggerEditorMode == 0 ? (byte)200 : (byte)172;
            byte b = settings.triggerEditorMode == 0 ? (byte)255 : (byte)65;

            if (this.IsFocused)
                this.Foreground = new SolidColorBrush(Color.FromRgb(0, 200, 0));
            else if (parameter is Constant ||
                     parameter is Function ||
                     parameter is VariableRef ||
                     parameter is TriggerRef ||
                     parameter is Value
                     )
                this.Foreground = new SolidColorBrush(Color.FromRgb(r, g, b));
            else
                this.Foreground = new SolidColorBrush(Color.FromRgb(255, 75, 75));
        }
    }
}
