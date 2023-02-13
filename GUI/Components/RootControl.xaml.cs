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
using War3Net.Build.Info;

namespace GUI.Components
{
    public partial class RootControl : UserControl, IEditor
    {
        public TextEditor textEditor;
        private ExplorerElementRoot explorerElementRoot;
        List<TreeItemExplorerElement> observers = new List<TreeItemExplorerElement>();

        public RootControl(ExplorerElementRoot explorerElementRoot)
        {
            InitializeComponent();

            this.explorerElementRoot = explorerElementRoot;
            string extension = System.IO.Path.GetExtension(explorerElementRoot.GetPath());
            this.textEditor = new TextEditor(explorerElementRoot.project.Header, ScriptLanguage.Jass);
            this.grid.Children.Add(textEditor);
            Grid.SetColumn(textEditor, 0);
            Grid.SetRow(textEditor, 3);


            textEditor.avalonEditor.TextChanged += delegate
            {
                this.explorerElementRoot.project.Header = textEditor.avalonEditor.Text;
                OnStateChange();
            };
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

        public void OnRemoteChange()
        {
            throw new NotImplementedException();
        }

        internal void RefreshFontSize()
        {
            textEditor.ChangeFontSize();
        }

        internal void RefreshFontStyle()
        {
            textEditor.ChangeFontStyle();
        }
    }
}
