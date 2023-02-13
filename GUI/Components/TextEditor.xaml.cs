using BetterTriggers;
using BetterTriggers.Models.EditorData;
using BetterTriggers.WorldEdit;
using GUI.Components.TextEditorExtensions;
using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using ICSharpCode.AvalonEdit.Search;
using NuGet.Packaging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml;
using War3Net.Build.Info;

namespace GUI.Components
{
    public partial class TextEditor : UserControl
    {
        CompletionWindow completionWindow;
        static CompletionDataCollection completionCollection;

        public TextEditor(string content, ScriptLanguage language)
        {
            InitializeComponent();

            Settings settings = Settings.Load();
            this.avalonEditor.Margin = new Thickness(0, 0, 0, 0);
            this.avalonEditor.Background = (SolidColorBrush)new BrushConverter().ConvertFromString("#1E1E1E");
            this.avalonEditor.Foreground = (SolidColorBrush)new BrushConverter().ConvertFromString("#9CDCFE");
            this.avalonEditor.FontFamily = new FontFamily("Consolas");
            this.avalonEditor.ShowLineNumbers = true;
            this.avalonEditor.TextArea.FontSize = settings.textEditorFontSize;

            string uri = language == ScriptLanguage.Jass ?
                "Resources/SyntaxHighlighting/JassHighlighting.xml" :
                "Resources/SyntaxHighlighting/LuaHighlighting.xml";

            // Sets syntax highlighting in the comment field
            using (Stream s = Application.GetResourceStream(new Uri(uri, UriKind.Relative)).Stream)
            {
                using (XmlTextReader reader = new XmlTextReader(s))
                {
                    this.avalonEditor.SyntaxHighlighting = HighlightingLoader.Load(reader, HighlightingManager.Instance);
                }
            }

            var searchPanel = SearchPanel.Install(avalonEditor);
            avalonEditor.Text = content;

            // Autocomplete
            avalonEditor.TextArea.KeyDown += TextArea_KeyDown;
            avalonEditor.TextArea.Document.Changed += Document_Changed;

            if (completionCollection == null)
            {
                List<MyCompletionData> completionData = new List<MyCompletionData>();
                ScriptData.Natives.ForEach(n =>
                {
                    completionData.Add(new MyCompletionData(n.displayText, n.description));
                });
                completionCollection = new CompletionDataCollection(completionData);
            }

            // change font size
            this.avalonEditor.TextArea.MouseWheel += TextArea_MouseWheel;
        }

        private void TextArea_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
            {
                Settings settings = Settings.Load();
                double change;
                if (e.Delta > 0)
                    change = 1;
                else
                    change = -1;

                if ((settings.textEditorFontSize + change) != 0)
                {
                    settings.textEditorFontSize += change;
                    Settings.Save(settings);

                    var mainWindow = MainWindow.GetMainWindow();
                    var tabs = mainWindow.vmd.Tabs.GetEnumerator();
                    while(tabs.MoveNext())
                    {
                        var tab = tabs.Current;
                        if(tab.explorerElement.editor is ScriptControl scriptControl)
                        {
                            scriptControl. RefreshFontSize();
                        }
                        else if(tab.explorerElement.editor is RootControl rootControl)
                        {
                            rootControl.RefreshFontSize();
                        }
                    }
                }

                e.Handled = true;
            }
        }

        public void ChangeFontSize()
        {
            Settings settings = Settings.Load();
            this.avalonEditor.TextArea.FontSize = settings.textEditorFontSize;
        }

        private void TextArea_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space && (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl)))
            {
                ShowAutoCompletion();
                e.Handled = true; // prevents a space from being entered when opening autocomplete menu
            }
        }

        private void Document_Changed(object sender, ICSharpCode.AvalonEdit.Document.DocumentChangeEventArgs e)
        {

        }

        private void ShowAutoCompletion()
        {
            var caret = avalonEditor.TextArea.Caret;
            int caretPos = caret.Offset - 1;
            int i = caretPos;
            string word = string.Empty;
            bool wordFound = false;
            while (i > 0 && !wordFound)
            {
                char c = avalonEditor.Document.GetCharAt(i);
                if (c == ' ' || c == '\t' || c == '\n')
                {
                    wordFound = true;
                }
                else
                {
                    word += c;
                    i--;
                }
            }

            // flip word
            char[] charArray = word.ToCharArray();
            Array.Reverse(charArray);
            word = new string(charArray);

            // open completion window
            completionWindow = new CompletionWindow(avalonEditor.TextArea);
            completionWindow.ResizeMode = ResizeMode.NoResize;
            completionWindow.Foreground = (SolidColorBrush)new BrushConverter().ConvertFromString("#CCC");
            completionWindow.Background = (SolidColorBrush)new BrushConverter().ConvertFromString("#333");
            completionWindow.BorderBrush = (SolidColorBrush)new BrushConverter().ConvertFromString("#444");
            completionWindow.BorderThickness = new Thickness(0.3);
            IList<ICompletionData> data = completionWindow.CompletionList.CompletionData;
            var items = completionCollection.Search(word);
            data.AddRange(items);
            if (items.Count == 0)
            {
                completionWindow = null;
                return;
            }

            completionWindow.Show();
            completionWindow.Closed += delegate
            {
                completionWindow = null;
            };
            completionWindow.CompletionList.SizeChanged += delegate
            {
                if (completionWindow.CompletionList.ListBox.Items.Count == 0)
                {
                    completionWindow.Close();
                }
            };
        }
    }
}
