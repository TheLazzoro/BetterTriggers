using BetterTriggers.Controllers;
using GUI.Components;
using GUI.Components.TextEditor;
using GUI.Components.TriggerExplorer;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using Model.EditorData;
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
    /// Interaction logic for ScriptControl.xaml
    /// </summary>
    public partial class ScriptControl : UserControl, IEditor
    {
        public ICSharpCode.AvalonEdit.TextEditor textEditor;
        private ExplorerElementScript explorerElementScript;
        private List<TreeItemExplorerElement> observers = new List<TreeItemExplorerElement>();
        private bool suppressStateChange = false;

        public ScriptControl(ExplorerElementScript explorerElementScript)
        {
            InitializeComponent();

            this.textEditor = new ICSharpCode.AvalonEdit.TextEditor();
            this.grid.Children.Add(textEditor);
            Grid.SetRow(textEditor, 1);
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

            this.explorerElementScript = explorerElementScript;
            textEditor.Text = explorerElementScript.script;

            textEditor.TextChanged += delegate
            {
                explorerElementScript.script = textEditor.Text;
                if (this.suppressStateChange)
                {
                    this.suppressStateChange = false;
                    return;
                }

                OnStateChange();
            };
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

        public void OnRemoteChange()
        {
            throw new NotImplementedException();
        }

        public void Refresh()
        {
            this.suppressStateChange = true;
            ControllerScript controller = new ControllerScript();
            textEditor.Text = controller.LoadScriptFromFile(explorerElementScript.GetPath());
        }

        public void SetElementEnabled(bool isEnabled)
        {
            checkBoxIsEnabled.IsChecked = isEnabled;
            ControllerProject controller = new ControllerProject();
            controller.SetElementEnabled(explorerElementScript, (bool)checkBoxIsEnabled.IsChecked);
            OnStateChange();
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

        private void checkBoxIsEnabled_Click(object sender, RoutedEventArgs e)
        {
            ControllerProject controller = new ControllerProject();
            controller.SetElementEnabled(explorerElementScript, (bool)checkBoxIsEnabled.IsChecked);
            OnStateChange();
        }
    }
}
