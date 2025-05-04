using BetterTriggers.Containers;
using BetterTriggers.Models.EditorData;
using System;
using System.Collections.Generic;
using System.Windows.Controls;
using War3Net.Build.Info;

namespace GUI.Components
{
    public partial class RootControl : UserControl
    {
        public TextEditor textEditor;

        public RootControl()
        {
            InitializeComponent();

            var root = Project.CurrentProject.GetRoot();
            string extension = System.IO.Path.GetExtension(root.GetPath());
            this.textEditor = new TextEditor(Project.CurrentProject.war3project.Header, ScriptLanguage.Jass);
            this.grid.Children.Add(textEditor);
            Grid.SetColumn(textEditor, 0);
            Grid.SetRow(textEditor, 3);


            textEditor.avalonEditor.TextChanged += delegate
            {
                Project.CurrentProject.war3project.Header = textEditor.avalonEditor.Text;
                OnStateChange();
            };
        }

        public void OnStateChange()
        {
            
        }

        private void textBoxComment_TextChanged(object sender, TextChangedEventArgs e)
        {
            Project.CurrentProject.war3project.Comment = textBoxComment.Text;
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
