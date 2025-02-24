using BetterTriggers;
using BetterTriggers.WorldEdit;
using GUI.Components.ScriptEditor;
using GUI.Utility;
using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using ICSharpCode.AvalonEdit.Search;
using NuGet.Packaging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
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
        private bool isReadonly;

        public TextEditor(string content, ScriptLanguage language, bool isReadonly = false)
        {
            InitializeComponent();

            this.language = language;

            EditorSettings settings = EditorSettings.Load();
            this.avalonEditor.Margin = new Thickness(0, 0, 0, 0);
            this.avalonEditor.SetResourceReference(TextEditor.BackgroundProperty, "TexteditorBackground");
            this.avalonEditor.SetResourceReference(TextEditor.ForegroundProperty, "TexteditorForeground");
            this.avalonEditor.FontFamily = new FontFamily(settings.textEditorFontStyle);
            this.avalonEditor.ShowLineNumbers = true;
            this.avalonEditor.TextArea.FontSize = settings.textEditorFontSize;
            this.tooltip = new ToolTip();
            this.tooltip.FontFamily = new FontFamily(settings.textEditorFontStyle);

            ReloadTextEditorTheme();

            InitializeCompletionData();

            var searchPanel = SearchPanel.Install(avalonEditor);
            searchPanel.MarkerBrush = new SolidColorBrush(Color.FromArgb(150, 200, 100, 0));
            avalonEditor.Text = content;

            this.isReadonly = isReadonly;
            this.avalonEditor.IsReadOnly = isReadonly;
            if (!isReadonly)
            {
                // Autocomplete
                avalonEditor.TextArea.KeyDown += TextArea_KeyDown;
                avalonEditor.TextArea.TextEntering += TextArea_TextEntering;
                avalonEditor.TextArea.SelectionChanged += TextArea_SelectionChanged;

                // Hover over text
                this.avalonEditor.MouseHover += AvalonEditor_MouseHover;
                this.avalonEditor.MouseHoverStopped += AvalonEditor_MouseHoverStopped;

                // change font size
                this.avalonEditor.TextArea.MouseWheel += TextArea_MouseWheel;
            }
        }

        public void Dispose()
        {
            if (isReadonly)
            {
                return;
            }

            // Autocomplete
            avalonEditor.TextArea.KeyDown -= TextArea_KeyDown;
            avalonEditor.TextArea.TextEntering -= TextArea_TextEntering;
            avalonEditor.TextArea.SelectionChanged -= TextArea_SelectionChanged;
            // Hover over text
            this.avalonEditor.MouseHover -= AvalonEditor_MouseHover;
            this.avalonEditor.MouseHoverStopped -= AvalonEditor_MouseHoverStopped;
            // change font size
            this.avalonEditor.TextArea.MouseWheel -= TextArea_MouseWheel;

            CloseAutoCompletion();
        }

        public void ReloadTextEditorTheme()
        {
            EditorSettings settings = EditorSettings.Load();
            string uri = string.Empty;
            if (settings.editorAppearance == EditorAppearance.Light)
            {
                uri = language == ScriptLanguage.Jass ?
                    "Resources/SyntaxHighlighting/JassHighlightingLight.xml" :
                    "Resources/SyntaxHighlighting/LuaHighlightingLight.xml";
            }
            else
            {
                uri = language == ScriptLanguage.Jass ?
                    "Resources/SyntaxHighlighting/JassHighlighting.xml" :
                    "Resources/SyntaxHighlighting/LuaHighlighting.xml";
            }
            avalonEditor.TextArea.Caret.CaretBrush = (SolidColorBrush)Application.Current.Resources["TextBrush2"];

            // Sets syntax highlighting in the comment field
            using (Stream s = Application.GetResourceStream(new Uri(uri, UriKind.Relative)).Stream)
            {
                using (XmlTextReader reader = new XmlTextReader(s))
                {
                    this.avalonEditor.SyntaxHighlighting = HighlightingLoader.Load(reader, HighlightingManager.Instance);
                }
            }

        }

        public void ChangeFontSize()
        {
            EditorSettings settings = EditorSettings.Load();
            this.avalonEditor.TextArea.FontSize = settings.textEditorFontSize;
        }

        internal void ChangeFontStyle()
        {
            EditorSettings settings = EditorSettings.Load();
            this.avalonEditor.TextArea.FontFamily = new FontFamily(settings.textEditorFontStyle);
        }


        public void InitializeCompletionData(bool reload = false)
        {
            if ((completionCollection == null || reload) && !isLoadingScriptData)
            {
                TextFormatter.JassTypewordBrush = null;
                TextFormatter.JassKeywordBrush = null;
                TextFormatter.LuaKeywordBrush = null;
                TextFormatter.JassFunctionBrush = null;
                TextFormatter.JassVariableBrush = null;

                isLoadingScriptData = true;
                this.language = language;
                Thread newWindowThread = new Thread(AddCompletionDataThread);
                newWindowThread.SetApartmentState(ApartmentState.STA);
                newWindowThread.IsBackground = true;
                newWindowThread.Start();
            }
        }

        static bool isLoadingScriptData;
        ScriptLanguage language;
        private void AddCompletionDataThread()
        {
            this.Dispatcher.BeginInvoke(AddCompletionData, DispatcherPriority.Background);
        }

        private void AddCompletionData()
        {
            List<CompletionData> completionData = new List<CompletionData>();
            ScriptData.GetAll(this.language).ForEach(n =>
            {
                var completionItem = new CompletionData(n.displayText, n.description);
                completionData.Add(completionItem);
            });
            completionCollection = new CompletionDataCollection(completionData);
            isLoadingScriptData = false;
        }

        private void TextArea_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
            {
                EditorSettings settings = EditorSettings.Load();
                double change;
                if (e.Delta > 0)
                    change = 1;
                else
                    change = -1;

                ChangeFontSize(change, isDeltaChange: true);
                e.Handled = true;
            }
        }

        public static void ChangeFontSize(double change, bool isDeltaChange)
        {
            EditorSettings settings = EditorSettings.Load();
            bool doChange = false;
            if (isDeltaChange && (settings.textEditorFontSize + change) > 0)
            {
                doChange = true;
            }
            else if (!isDeltaChange && change > 0)
            {
                doChange = true;
            }

            if (!doChange)
            {
                return;
            }

            if (isDeltaChange)
                settings.textEditorFontSize += change;
            else
                settings.textEditorFontSize = change;

            EditorSettings.Save(settings);

            var mainWindow = MainWindow.GetMainWindow();
            var tabs = mainWindow.tabViewModel.Tabs.GetEnumerator();
            while (tabs.MoveNext())
            {
                var tab = tabs.Current;
                if (tab.Content is ScriptControl scriptControl)
                {
                    scriptControl.RefreshFontSize();
                }
                else if (tab.Content is RootControl rootControl)
                {
                    rootControl.RefreshFontSize();
                }
            }
        }

        private void TextArea_TextEntering(object sender, TextCompositionEventArgs e)
        {
            HandleKeyPress(e.Text);
        }

        private void TextArea_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                CloseAutoCompletion();
            }
            else if (e.Key == Key.Space && (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl)))
            {
                CloseAutoCompletion();
                HandleKeyPress();
                e.Handled = true; // prevents a space from being entered when opening autocomplete menu
            }
            else if (e.Key == Key.Space)
            {
                CloseAutoCompletion();
            }
        }

        private void CloseAutoCompletion()
        {
            if (completionWindow != null)
            {
                completionWindow.Close();
                completionWindow = null;
            }
        }

        private void HandleKeyPress(string toInsert = null)
        {
            var caret = avalonEditor.TextArea.Caret;
            int caretPos = caret.Offset - 1;
            int i = caretPos;
            string word = string.Empty;
            if (toInsert != null)
            {
                word = toInsert;
            }
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

            // cancel autocompletion window if the string is a number
            if (float.TryParse(word, out float value))
            {
                return;
            }

            // open completion window
            EditorSettings settings = EditorSettings.Load();
            if (completionWindow == null)
            {
                completionWindow = new CompletionWindow(avalonEditor.TextArea);
                completionWindow.ResizeMode = ResizeMode.NoResize;
                completionWindow.Background = (SolidColorBrush)Application.Current.Resources["TexteditorBackgroundAutocomplete"];
                completionWindow.BorderBrush = (SolidColorBrush)Application.Current.Resources["BorderBrush"];
                completionWindow.BorderThickness = new Thickness(0.3);
                completionWindow.Width = 400;
                completionWindow.FontFamily = new FontFamily(settings.textEditorFontStyle);
                completionWindow.UseLayoutRounding = true;
                completionWindow.CompletionList.InsertionRequested += CompletionList_InsertionRequested;
                completionWindow.KeyDown += CompletionWindow_KeyDown;

                // makes sure to replace the whole word for autocompletion, when it's only a partial word.
                int offset = completionWindow.StartOffset - 1;
                while(offset > 0)
                {
                    char c = avalonEditor.Document.GetCharAt(offset);
                    if (c == ' ')
                    {
                        offset++;
                        break;
                    }

                    offset--;
                }

                if (offset < 0) offset = 0;
                completionWindow.StartOffset = offset;

                completionWindow.Show();
            }


            IList<ICompletionData> data = completionWindow.CompletionList.CompletionData;
            var items = completionCollection.Search(word);
            data.Clear();
            data.AddRange(items);
            if (items.Count == 0)
            {
                CloseAutoCompletion();
                return;
            }
            completionWindow.CompletionList.SelectItem(word);
        }

        private void CompletionWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                CloseAutoCompletion();
            }
        }

        private void CompletionList_InsertionRequested(object? sender, EventArgs e)
        {
            CloseAutoCompletion();
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

        private void TextArea_SelectionChanged(object? sender, EventArgs e)
        {
            CloseAutoCompletion();
        }

    }
}
