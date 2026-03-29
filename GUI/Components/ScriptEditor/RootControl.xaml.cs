using BetterTriggers.Containers;
using System;
using System.Windows.Controls;
using War3Net.Build.Info;

namespace GUI.Components
{
    public partial class RootControl : UserControl
    {
        public TextEditor textEditor;
        private Project _project;

        public RootControl(Project project)
        {
            InitializeComponent();

            _project = project;
            var root = project.GetRoot();
            string extension = System.IO.Path.GetExtension(root.GetPath());
            this.textEditor = new TextEditor(_project, project.war3project.Header, ScriptLanguage.Jass);
            this.grid.Children.Add(textEditor);
            Grid.SetColumn(textEditor, 0);
            Grid.SetRow(textEditor, 3);


            textEditor.avalonEditor.TextChanged += delegate
            {
                project.war3project.Header = textEditor.avalonEditor.Text;
                OnStateChange();
            };
        }

        public void OnStateChange()
        {
            
        }

        private void textBoxComment_TextChanged(object sender, TextChangedEventArgs e)
        {
            _project.war3project.Comment = textBoxComment.Text;
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
