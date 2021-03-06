using GUI.Components.TextEditorExtensions;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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
        public ICSharpCode.AvalonEdit.TextEditor avalonEditor;

        public TextEditor(string content, ScriptLanguage language)
        {
            InitializeComponent();

            this.avalonEditor = new ICSharpCode.AvalonEdit.TextEditor();
            this.grid.Children.Add(avalonEditor);
            Grid.SetRow(avalonEditor, 0);
            this.avalonEditor.Margin = new Thickness(0, 0, 0, 0);
            this.avalonEditor.Background = (SolidColorBrush)new BrushConverter().ConvertFromString("#1E1E1E");
            this.avalonEditor.Foreground = (SolidColorBrush)new BrushConverter().ConvertFromString("#9CDCFE");
            this.avalonEditor.FontFamily = new FontFamily("Consolas");
            this.avalonEditor.ShowLineNumbers = true;
            // new AutoComplete(this.textEditor); TODO:

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

            avalonEditor.Text = content;
        }
    }
}
