using BetterTriggers.Controllers;
using GUI.Commands;
using GUI.Components;
using GUI.Components.TextEditor;
using GUI.Components.TriggerEditor;
using GUI.Components.TriggerExplorer;
using GUI.Utility;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using Model.EditorData;
using Model.SaveableData;
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
    /// <summary>
    /// Interaction logic for TriggerControl.xaml
    /// </summary>
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
            richTextBoxComment.Document.Blocks.Clear();
            richTextBoxComment.Document.Blocks.Add(new Paragraph(new Run(explorerElementRoot.project.Comment)));
            textEditor.Text = explorerElementRoot.project.Header;
        }

        public void Refresh()
        {
            
        }

        
        public string GetSaveString()
        {
            ControllerProject controller = new ControllerProject();
            War3Project project = controller.GetCurrentProject();
            project.Comment = new TextRange(richTextBoxComment.Document.ContentStart, richTextBoxComment.Document.ContentEnd).Text;
            project.Header = textEditor.Text;

            return JsonConvert.SerializeObject(project);
        }

        public UserControl GetControl()
        {
            return this;
        }


        public void OnElementRename(string name)
        {
            throw new NotImplementedException();
        }

        public void OnRemoteChange()
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
    }
}
