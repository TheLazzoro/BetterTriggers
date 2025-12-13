using BetterTriggers.Models.EditorData;
using System;
using System.Windows;
using System.Windows.Controls;
using War3Net.Build.Info;

namespace GUI.Components
{
    public partial class ScriptControl : UserControl
    {
        internal TextEditor textEditor;
        private ExplorerElement explorerElementScript;
        private bool suppressStateChange = false;

        public ScriptControl(ExplorerElement explorerElementScript)
        {
            InitializeComponent();

            string extension = System.IO.Path.GetExtension(explorerElementScript.GetPath());
            textEditor = new TextEditor(explorerElementScript.script, extension == ".j" ? ScriptLanguage.Jass : ScriptLanguage.Lua);
            this.grid.Children.Add(textEditor);
            Grid.SetRow(textEditor, 1);

            this.explorerElementScript = explorerElementScript;
            DataContext = explorerElementScript;

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

            explorerElementScript.OnReload += ExplorerElementScript_OnReload;
            explorerElementScript.OnCloseEditor += ExplorerElementScript_OnCloseEditor;
        }

        private void ExplorerElementScript_OnReload()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                this.suppressStateChange = true;
                textEditor.avalonEditor.Document.Text = explorerElementScript.script;
            });
        }

        public void OnRemoteChange()
        {
            this.suppressStateChange = true;
            textEditor.avalonEditor.Document.Text = explorerElementScript.script;
        }

        public void SetElementEnabled(bool isEnabled)
        {
            checkBoxIsEnabled.IsChecked = isEnabled;
            explorerElementScript.IsEnabled = (bool)checkBoxIsEnabled.IsChecked;
            OnStateChange();
        }

        public void SetElementInitiallyOn(bool isInitiallyOn)
        {
            throw new NotImplementedException();
        }
        public void OnStateChange()
        {
            explorerElementScript.AddToUnsaved();
        }

        private void checkBoxIsEnabled_Click(object sender, RoutedEventArgs e)
        {
            explorerElementScript.IsEnabled = (bool)checkBoxIsEnabled.IsChecked;
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

        internal void ReloadTextEditorTheme()
        {
            textEditor.ReloadTextEditorTheme();
            textEditor.InitializeCompletionData(true);
        }


        private void ExplorerElementScript_OnCloseEditor()
        {
            textEditor.Dispose();

            explorerElementScript.OnReload -= ExplorerElementScript_OnReload;
            explorerElementScript.OnCloseEditor -= ExplorerElementScript_OnCloseEditor;
        }
    }
}
