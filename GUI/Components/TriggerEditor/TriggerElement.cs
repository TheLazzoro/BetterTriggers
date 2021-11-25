using DataAccess.Data;
using GUI.Components.Utility;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace GUI.Components.TriggerEditor
{
    public abstract class TriggerElement : TreeViewItem
    {
        public TextBlock paramTextBlock;
        protected List <DataAccess.Natives.Parameter> parameters;
        protected string paramText;

        public TriggerElement()
        {
            this.paramTextBlock = new TextBlock();
            this.paramTextBlock.FontSize = 18;
            this.paramTextBlock.Margin = new Thickness(0, 0, 5, 0);
            this.paramTextBlock.Foreground = Brushes.White;
            
        }

        protected void FormatParameterText(TextBlock textBlock)
        {
            bool isParam = false;
            string textNormal = string.Empty;
            string textParam = string.Empty;
            for (int i = 0; i < paramText.Length; i++)
            {
                if (paramText[i] == '%' && !isParam)
                {
                    isParam = true;
                }
                else if (paramText[i] == '%' && isParam)
                {
                    isParam = false;
                    Run run = new Run(textParam); // idk why it's called run
                    Hyperlink hyperlink = new Hyperlink(run);
                    hyperlink.Click += Hyperlink_Click;
                    textBlock.Inlines.Add(hyperlink);
                    textParam = string.Empty; // reset param text
                }

                if (!isParam && paramText[i] != '%')
                    textBlock.Inlines.Add(paramText[i].ToString());
                else if (isParam && paramText[i] != '%')
                {
                    textParam += paramText[i];
                }
            }
        }

        private void Hyperlink_Click(object sender, RoutedEventArgs e)
        {
            var window = new ParameterWindow(parameters[0].returnType.type); // Temporary. Only index 0 parameter return type is being passed in atm.
            window.ShowDialog();
        }
    }
}
