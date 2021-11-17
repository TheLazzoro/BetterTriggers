using GUI.Components.TextEditor;
using GUI.Containers;
using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Folding;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace GUI.Components.TriggerExplorer
{
    public class Script : TriggerElement, ITriggerElement
    {
        public bool IsEnabled;
        ICSharpCode.AvalonEdit.TextEditor textEditor;
        

        public Script(string name, TreeViewItem treeViewItem, ICSharpCode.AvalonEdit.TextEditor textEditor) : base(treeViewItem )
        {
            this.Name = name;
            this.textEditor = textEditor;
            this.textEditor.Margin = new Thickness(0, 5, 5, 5);
            new AutoComplete(this.textEditor);

            ContainerITriggerElements.AddTriggerElement(this);
        }

        public void OnElementClick()
        {
            if (currentTriggerElement != null)
                currentTriggerElement.Hide();

            this.Show();

            currentTriggerElement = this;
        }

        public void Show()
        {
            textEditor.Visibility = Visibility.Visible;
        }

        public void Hide()
        {
            textEditor.Visibility = Visibility.Hidden;
        }

        public string GetScript()
        {
            return this.textEditor.Text;
        }
    }
}
