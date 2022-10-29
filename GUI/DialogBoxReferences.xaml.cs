using BetterTriggers.Models.EditorData;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;


namespace GUI
{
    public enum ExplorerAction
    {
        Reset,
        Delete
    }

    public partial class DialogBoxReferences : Window
    {
        public bool OK = false;
        
        public DialogBoxReferences(List<ExplorerElementTrigger> references, ExplorerAction action)
        {
            InitializeComponent();

            this.Title = "Confimation";

            string actionText = string.Empty;
            switch (action)
            {
                case ExplorerAction.Reset:
                    actionText = "Changing";
                    break;
                case ExplorerAction.Delete:
                    actionText = "Deleting";
                    break;
                default:
                    break;
            }


            string content = $"This element is still in use. {actionText} it will reset all references to it and cannot be undone. Do you wish to continue?\n\nUsed by:";
            foreach (var reference in references)
            {
                content += "\n<" + reference.GetName() + ">";
            }

            textBlockMessage.Text = content;
        }

        private void OKButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            OK = true;
            this.Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                this.Close();
            else if (e.Key == Key.Enter)
            {
                this.OK = true;
                this.Close();
            }
        }
    }
}
