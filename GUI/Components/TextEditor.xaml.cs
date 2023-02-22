using BetterTriggers;
using BetterTriggers.Models.EditorData;
using BetterTriggers.WorldEdit;
using GUI.Components.TextEditorExtensions;
using GUI.Utility;
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
using System.Windows.Threading;
using System.Xml;
using War3Net.Build.Info;

namespace GUI.Components
{
    public partial class TextEditor : UserControl
    {
        CompletionWindow completionWindow;
        static CompletionDataCollection completionCollection;
        ToolTip tooltip;

        public TextEditor(string content, ScriptLanguage language)
        {
            InitializeComponent();

            Settings settings = Settings.Load();
            this.avalonEditor.Margin = new Thickness(0, 0, 0, 0);
            this.avalonEditor.Background = (SolidColorBrush)new BrushConverter().ConvertFromString("#1E1E1E");
            this.avalonEditor.Foreground = (SolidColorBrush)new BrushConverter().ConvertFromString("#9CDCFE");
            this.avalonEditor.FontFamily = new FontFamily(settings.textEditorFontStyle);
            this.avalonEditor.ShowLineNumbers = true;
            this.avalonEditor.TextArea.FontSize = settings.textEditorFontSize;
            this.tooltip = new ToolTip();
            this.tooltip.FontFamily = new FontFamily(settings.textEditorFontStyle);

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
                ScriptData.GetAll(language).ForEach(n =>
                {
                    completionData.Add(new MyCompletionData(n.displayText, n.description));
                });
                completionCollection = new CompletionDataCollection(completionData);
            }

            // Hover over text
            this.avalonEditor.MouseHover += AvalonEditor_MouseHover;
            this.avalonEditor.MouseHoverStopped += AvalonEditor_MouseHoverStopped;

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
                    while (tabs.MoveNext())
                    {
                        var tab = tabs.Current;
                        if (tab.explorerElement.editor is ScriptControl scriptControl)
                        {
                            scriptControl.RefreshFontSize();
                        }
                        else if (tab.explorerElement.editor is RootControl rootControl)
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

        internal void ChangeFontStyle()
        {
            Settings settings = Settings.Load();
            this.avalonEditor.TextArea.FontFamily = new FontFamily(settings.textEditorFontStyle);
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
                if (!Char.IsLetterOrDigit(c) && c != '_')
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
            Settings settings = Settings.Load();
            completionWindow = new CompletionWindow(avalonEditor.TextArea);
            completionWindow.ResizeMode = ResizeMode.NoResize;
            completionWindow.Foreground = (SolidColorBrush)new BrushConverter().ConvertFromString("#CCC");
            completionWindow.Background = (SolidColorBrush)new BrushConverter().ConvertFromString("#333");
            completionWindow.BorderBrush = (SolidColorBrush)new BrushConverter().ConvertFromString("#444");
            completionWindow.BorderThickness = new Thickness(0.3);
            completionWindow.Width = 400;
            completionWindow.FontFamily = new FontFamily(settings.textEditorFontStyle);
            completionWindow.UseLayoutRounding = true;
            IList<ICompletionData> data = completionWindow.CompletionList.CompletionData;
            var items = completionCollection.Search(word);
            data.AddRange(items);
            if (items.Count == 0)
            {
                completionWindow = null;
                return;
            }

            completionWindow.Show();
            completionWindow.CompletionList.SelectItem(word);
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

        private void AvalonEditor_MouseHover(object sender, MouseEventArgs e)
        {
            var pos = avalonEditor.GetPositionFromPoint(e.GetPosition(avalonEditor));
            if (pos != null)
            {
                // determine word
                string hoveredWord = string.Empty;
                int offset = 0;
                for (int i = 0; i < pos.Value.Line - 1; i++)
                {
                    // Need to append 2 to length per line. Probably because of '\n\r' characters when jumping to new line.
                    offset += avalonEditor.Document.Lines[i].Length + 2;
                }
                offset += pos.Value.VisualColumn;
                bool lookback = true;

                // first looks at letters behind the cursor position, then in front.
                while (offset < avalonEditor.Document.TextLength)
                {
                    char c = avalonEditor.Document.GetCharAt(offset);
                    if (!lookback && ((!Char.IsLetterOrDigit(c) && c != '_') || offset == avalonEditor.Document.TextLength))
                    {
                        break;
                    }
                    else if ((!Char.IsLetterOrDigit(c) && c != '_') || offset == 0)
                    {
                        lookback = false;
                        if (offset == 0)
                        {
                            hoveredWord = hoveredWord.Insert(0, c.ToString());
                            offset += hoveredWord.Length;
                        }
                        else
                            offset += hoveredWord.Length + 1;

                        continue;
                    }

                    if (lookback)
                    {
                        hoveredWord = hoveredWord.Insert(0, c.ToString());
                        offset--;
                    }
                    else
                    {
                        hoveredWord += c;
                        offset++;
                    }
                }

                string description = ScriptData.GetDescription(hoveredWord);
                TextBlock tooltipContent = new TextBlock();
                Run title = new Run(hoveredWord);
                title.FontWeight = FontWeights.Bold;
                tooltipContent.Inlines.Add(title);
                tooltipContent.Inlines.Add(new Run(Environment.NewLine));
                tooltipContent.Inlines.Add(new Run(Environment.NewLine));
                tooltipContent.Inlines.AddRange(TextFormatter.CodeColor(description, ScriptLanguage.Jass));
                tooltip.PlacementTarget = this; // required for property inheritance
                tooltip.Content = tooltipContent;
                if (!string.IsNullOrEmpty(description))
                {
                    tooltip.IsOpen = true;
                    e.Handled = true;
                }
            }
        }

        private void AvalonEditor_MouseHoverStopped(object sender, MouseEventArgs e)
        {
            tooltip.IsOpen = false;
        }

    }
}
