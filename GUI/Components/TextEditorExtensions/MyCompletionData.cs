using BetterTriggers.WorldEdit;
using GUI.Utility;
using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using War3Net.Build.Info;

namespace GUI.Components.TextEditorExtensions
{
    public class MyCompletionData : ICompletionData
    {
        public MyCompletionData(string text, string description)
        {
            this.Text = text;
            this.header = new TextBlock();
            this.description = new TextBlock();
            this.header.Inlines.AddRange(TextFormatter.CodeColor(text, Info.GetLanguage()));
            this.description.Inlines.AddRange(TextFormatter.CodeColor(description, ScriptLanguage.Jass));

            BitmapImage b = new BitmapImage();
            if(ScriptData.KeywordsJass.ContainsKey(text) || ScriptData.KeywordsLua.ContainsKey(text))
                b = new BitmapImage(new Uri(@"/Resources/Icons/Keyword_16x.png", UriKind.Relative));
            else if (ScriptData.TypewordsJass.ContainsKey(text))
                b = new BitmapImage(new Uri(@"/Resources/Icons/Class_16x.png", UriKind.Relative));
            else if (ScriptData.Constants.ContainsKey(text))
                b = new BitmapImage(new Uri(@"/Resources/Icons/Variable_16x.png", UriKind.Relative));
            else if (ScriptData.Natives.ContainsKey(text))
                b = new BitmapImage(new Uri(@"/Resources/Icons/Method_16x.png", UriKind.Relative));

            this.Image = b;
        }

        public System.Windows.Media.ImageSource Image { get; private set; }

        public string Text { get; private set; }
        public TextBlock header { get; private set; }
        public TextBlock description { get; private set; }

        public object Content
        {
            get { return this.header; }
        }

        public object Description
        {
            get
            {
                if (description.Inlines.Count == 0)
                    return null;

                return this.description;
            }
        }

        public double Priority => 0;

        public void Complete(TextArea textArea, ISegment completionSegment,
            EventArgs insertionRequestEventArgs)
        {
            textArea.Document.Replace(completionSegment, this.Text);
        }
    }
}
