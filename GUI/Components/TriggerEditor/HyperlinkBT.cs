using BetterTriggers.Models.SaveableData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
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

            // Create an underline text decoration.
            TextDecoration underline = new TextDecoration();
            underline.PenOffset = 2; // Underline offset
            underline.PenThicknessUnit = TextDecorationUnit.FontRecommended;

            TextDecorationCollection decorations = new TextDecorationCollection();
            decorations.Add(underline);
            this.TextDecorations = decorations;

            this.GotFocus += HyperlinkParameter_GotFocus;
            this.LostFocus += HyperlinkParameter_LostFocus;

            this.FontFamily = new FontFamily("Verdana");
            RecolorHyperlink();
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
            if (this.IsFocused)
                this.Foreground = new SolidColorBrush(Color.FromRgb(0, 200, 0));
            else if (parameter is Constant ||
                     parameter is Function ||
                     parameter is VariableRef ||
                     parameter is TriggerRef ||
                     parameter is Value
                     )
                this.Foreground = new SolidColorBrush(Color.FromRgb(0, 200, 255));
            else
                this.Foreground = new SolidColorBrush(Color.FromRgb(255, 75, 75));
        }
    }
}
