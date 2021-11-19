using GUI.Components.TriggerExplorer;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Xml;

namespace GUI.Controllers
{
    public class ControllerTriggerExplorer
    {
        public void CreateFolder(TriggerExplorer triggerExplorer) {
            triggerExplorer.CreateFolder();
        }
        public void CreateScript(Grid mainGrid, TriggerExplorer triggerExplorer)
        {

            var textEditor = new ICSharpCode.AvalonEdit.TextEditor();
            textEditor.Background = (SolidColorBrush)new BrushConverter().ConvertFromString("#1E1E1E");
            textEditor.Foreground = (SolidColorBrush)new BrushConverter().ConvertFromString("#9CDCFE");
            textEditor.FontFamily = new FontFamily("Consolas");
            textEditor.ShowLineNumbers = true;

            // Position editor
            mainGrid.Children.Add(textEditor);
            Grid.SetColumn(textEditor, 1);
            Grid.SetRow(textEditor, 3);

            // Sets syntax highlighting in the comment field
            using (Stream s = Application.GetResourceStream(new Uri("Resources/SyntaxHighlighting/JassHighlighting.xml", UriKind.Relative)).Stream)
            {
                using (XmlTextReader reader = new XmlTextReader(s))
                {
                    textEditor.SyntaxHighlighting = HighlightingLoader.Load(reader, HighlightingManager.Instance);
                }
            }

            triggerExplorer.CreateScript(textEditor);

            // folding text blocks?
            //foldingManager = FoldingManager.Install(textEditor.TextArea);
            //foldingStrategy.UpdateFoldings(foldingManager, textEditor.Document);
        }

        public void CreateVariable(Grid mainGrid, TriggerExplorer triggerExplorer)
        {
            var userControl = new VariableControl();

            // Position editor
            mainGrid.Children.Add(userControl);
            Grid.SetColumn(userControl, 1);
            Grid.SetRow(userControl, 3);

            triggerExplorer.CreateVariable(userControl);
        }

    }
}
