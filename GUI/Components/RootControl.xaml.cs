using BetterTriggers.Controllers;
using BetterTriggers.Models.EditorData;
using GUI.Components;
using GUI.Components.TextEditorExtensions;
using GUI.Components.TriggerEditor;
using GUI.Components.TriggerExplorer;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
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

namespace GUI.Components
{
    public partial class RootControl : UserControl, IEditor
    {
        public ICSharpCode.AvalonEdit.TextEditor textEditor;
        private ExplorerElementRoot explorerElementRoot;
        List<TreeItemExplorerElement> observers = new List<TreeItemExplorerElement>();

        public RootControl(ExplorerElementRoot explorerElementRoot)
        {
            InitializeComponent();

            this.textEditor = new ICSharpCode.AvalonEdit.TextEditor();
            this.grid.Children.Add(textEditor);
            Grid.SetColumn(textEditor, 0);
            Grid.SetRow(textEditor, 3);
            this.textEditor.Margin = new Thickness(0, 0, 0, 0);
            this.textEditor.Background = (SolidColorBrush)new BrushConverter().ConvertFromString("#1E1E1E");
            this.textEditor.Foreground = (SolidColorBrush)new BrushConverter().ConvertFromString("#9CDCFE");
            this.textEditor.FontFamily = new FontFamily("Consolas");
            this.textEditor.ShowLineNumbers = true;
            new AutoComplete(this.textEditor);

            // Sets syntax highlighting in the comment field
            using (Stream s = Application.GetResourceStream(new Uri("Resources/SyntaxHighlighting/JassHighlighting.xml", UriKind.Relative)).Stream)
            {
                using (XmlTextReader reader = new XmlTextReader(s))
                {
                    this.textEditor.SyntaxHighlighting = HighlightingLoader.Load(reader, HighlightingManager.Instance);
                }
            }

            this.explorerElementRoot = explorerElementRoot;
            textBoxComment.Text = explorerElementRoot.project.Comment;
            textEditor.Text = explorerElementRoot.project.Header;

            textEditor.TextChanged += TextEditor_TextChanged;
        }

        public void SetElementEnabled(bool isEnabled)
        {
            throw new NotImplementedException();
        }

        public void SetElementInitiallyOn(bool isInitiallyOn)
        {
            throw new NotImplementedException();
        }

        public void Attach(TreeItemExplorerElement explorerElement)
        {
            this.observers.Add(explorerElement);
        }

        public void Detach(TreeItemExplorerElement explorerElement)
        {
            this.observers.Add(explorerElement);
        }

        public void OnStateChange()
        {
            foreach (var observer in observers)
            {
                observer.OnStateChange();
            }
        }

        private void textBoxComment_TextChanged(object sender, TextChangedEventArgs e)
        {
            explorerElementRoot.project.Comment = textBoxComment.Text;
            OnStateChange();
        }

        private void TextEditor_TextChanged(object sender, EventArgs e)
        {
            explorerElementRoot.project.Header = textEditor.Text;
            OnStateChange();
        }

        public void OnRemoteChange()
        {
            throw new NotImplementedException();
        }
    }
}
