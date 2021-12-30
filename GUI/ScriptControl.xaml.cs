using GUI.Components.TextEditor;
using GUI.Components.TriggerExplorer;
using System;
using System.Collections.Generic;
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

namespace GUI
{
    /// <summary>
    /// Interaction logic for ScriptControl.xaml
    /// </summary>
    public partial class ScriptControl : UserControl, IExplorerElement
    {
        ICSharpCode.AvalonEdit.TextEditor textEditor;

        public ScriptControl()
        {
            InitializeComponent();

            this.textEditor = new ICSharpCode.AvalonEdit.TextEditor();
            this.textEditor.Margin = new Thickness(0, 5, 5, 5);
            new AutoComplete(this.textEditor);
        }

        public void OnElementClick()
        {
            if (ExplorerElement.currentExplorerElement != null)
                ExplorerElement.currentExplorerElement.Hide();

            this.Show();

            ExplorerElement.currentExplorerElement = this;
        }

        public void Show()
        {
            this.Visibility = Visibility.Visible;
        }

        public void Hide()
        {
            this.Visibility = Visibility.Hidden;
        }

        public string GetScript()
        {
            return this.textEditor.Text;
        }

        public string GetSaveString()
        {
            return this.textEditor.Text;
        }

        public UserControl GetControl()
        {
            throw new NotImplementedException();
        }
    }
}
