using GUI.Components.TextEditor;
using GUI.Components.TriggerExplorer;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;
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

namespace GUI
{
    /// <summary>
    /// Interaction logic for ScriptControl.xaml
    /// </summary>
    public partial class ScriptControl : UserControl, IExplorerElement
    {
        public ICSharpCode.AvalonEdit.TextEditor textEditor;

        public ScriptControl()
        {
            InitializeComponent();

            this.textEditor = new ICSharpCode.AvalonEdit.TextEditor();
            this.grid.Children.Add(textEditor);
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
        }

        public void OnElementClick()
        {
            ExplorerElement.currentExplorerElement = this;
        }

        public string GetSaveString()
        {
            return this.textEditor.Text;
        }

        public UserControl GetControl()
        {
            throw new NotImplementedException();
        }

        public void OnElementRename(string name)
        {
            throw new NotImplementedException();
        }
    }
}
