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
    public class TriggerEvent : TreeViewItem
    {
        private DataAccess.Natives.Event _event;
        public TextBlock eventTextBlock;
        
        public TriggerEvent(DataAccess.Natives.Event _event)
        {
            this._event = _event;
            TreeViewManipulator.SetTreeViewItemAppearance(this, "Action", "Resources/editor-triggeraction.png");
            
            this.eventTextBlock = new TextBlock();
            this.eventTextBlock.FontSize = 18;
            this.eventTextBlock.Margin = new Thickness(0, 0, 5, 0);
            this.eventTextBlock.Foreground = Brushes.White;
            FormatEventText(eventTextBlock);
        }

        private void FormatEventText(TextBlock textBlock)
        {
            var eventText = _event.eventText;

            bool isParam = false;
            string textNormal = string.Empty;
            string textParam = string.Empty;
            for (int i = 0; i < eventText.Length; i++)
            {
                if (eventText[i] == '%' && !isParam)
                {
                    isParam = true;
                }
                else if(eventText[i] == '%' && isParam)
                {
                    isParam = false;
                    Run run = new Run(textParam); // idk why it's called run
                    Hyperlink hyperlink = new Hyperlink(run);
                    textBlock.Inlines.Add(hyperlink);
                    textParam = string.Empty; // reset param text
                }

                if (!isParam && eventText[i] != '%')
                    textBlock.Inlines.Add(eventText[i].ToString());
                else if(isParam && eventText[i] != '%')
                {
                    textParam += eventText[i];
                }
            }
        }
    }
}
