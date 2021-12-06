using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Folding;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using System.Windows.Media;

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
                    completionWindow.ResizeMode = System.Windows.ResizeMode.NoResize;
                    completionWindow.Foreground = (SolidColorBrush)new BrushConverter().ConvertFromString("#DDDDDD");
                    completionWindow.Background = (SolidColorBrush)new BrushConverter().ConvertFromString("#333333");
                    completionWindow.FontFamily = new FontFamily("Consolas");
                    completionWindow.BorderThickness = new System.Windows.Thickness(0);

                    // Autocomplete data
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
