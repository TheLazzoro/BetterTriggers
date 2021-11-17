using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Folding;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace GUI.Components.TextEditor
{
    public class AutoComplete
    {
        XmlFoldingStrategy foldingStrategy = new XmlFoldingStrategy();
        FoldingManager foldingManager;
        CompletionWindow completionWindow;

        public AutoComplete(ICSharpCode.AvalonEdit.TextEditor textEditor)
        {
            foldingManager = FoldingManager.Install(textEditor.TextArea);
            foldingStrategy.UpdateFoldings(foldingManager, textEditor.Document);

            textEditor.KeyDown += new KeyEventHandler(delegate (object sender, KeyEventArgs e) {
                foldingStrategy.UpdateFoldings(foldingManager, textEditor.Document);

                if (!(Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl) || Keyboard.IsKeyDown(Key.LeftAlt)))
                {
                    // Open code completion after the user has pressed dot:
                    completionWindow = new CompletionWindow(textEditor.TextArea);
                    IList<ICompletionData> data = completionWindow.CompletionList.CompletionData;
                    data.Add(new MyCompletionData("Item1"));
                    data.Add(new MyCompletionData("Item2"));
                    data.Add(new MyCompletionData("Item3"));
                    completionWindow.Show();
                    completionWindow.Closed += delegate
                    {
                        completionWindow = null;
                    };
                }
            });
        }
    }
}
