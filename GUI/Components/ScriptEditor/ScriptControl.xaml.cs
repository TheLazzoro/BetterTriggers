using BetterTriggers.Models.EditorData;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using War3Net.Build.Info;

namespace GUI.Components
{
    public partial class ScriptControl : UserControl, IEditor
    {
        private TextEditor textEditor;
        private ExplorerElementScript explorerElementScript;
        private List<TreeItemExplorerElement> observers = new List<TreeItemExplorerElement>();
        private bool suppressStateChange = false;

        public ScriptControl(ExplorerElementScript explorerElementScript)
        {
            InitializeComponent();

            string extension = System.IO.Path.GetExtension(explorerElementScript.GetPath());
            textEditor = new TextEditor(explorerElementScript.script, extension == ".j" ? ScriptLanguage.Jass : ScriptLanguage.Lua);
            this.grid.Children.Add(textEditor);
            Grid.SetRow(textEditor, 1);

            checkBoxIsEnabled.IsChecked = explorerElementScript.GetEnabled();
            this.explorerElementScript = explorerElementScript;

            textEditor.avalonEditor.TextChanged += delegate
            {
                explorerElementScript.script = textEditor.avalonEditor.Text;
                if (this.suppressStateChange)
                {
                    this.suppressStateChange = false;
                    return;
                }

                OnStateChange();
            };
        }

        public void OnRemoteChange()
        {
            this.suppressStateChange = true;
            textEditor.avalonEditor.Document.Text = explorerElementScript.script;
        }

        public void SetElementEnabled(bool isEnabled)
        {
            checkBoxIsEnabled.IsChecked = isEnabled;
            explorerElementScript.SetEnabled((bool)checkBoxIsEnabled.IsChecked);
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
            explorerElementScript.SetEnabled((bool)checkBoxIsEnabled.IsChecked);
            OnStateChange();
        }

        public void RefreshFontSize()
        {
            textEditor.ChangeFontSize();
        }


        internal void RefreshFontStyle()
        {
            textEditor.ChangeFontStyle();
        }
    }
}
