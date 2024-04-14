using BetterTriggers;
using BetterTriggers.Models.EditorData;
using BetterTriggers.Models.EditorData.TriggerEditor;
using GUI.Components.Settings;
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

            var settings = EditorSettings.Load();

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
            this.SetResourceReference(Hyperlink.BackgroundProperty, EditorTheme.HyperlinkHoverColor());
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
            else if (parameter is Preset ||
                     parameter is Function ||
                     parameter is VariableRef ||
                     parameter is TriggerRef ||
                     parameter is ParameterDefinitionRef ||
                     parameter is Value
                     )
                this.SetResourceReference(Hyperlink.ForegroundProperty, EditorTheme.HyperlinkColor());
            else
                this.SetResourceReference(Hyperlink.ForegroundProperty, "HyperlinkErrorBrush");

        }
    }
}
