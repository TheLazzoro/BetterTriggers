using System.Windows;
using System.Windows.Input;


namespace GUI.Components.Dialogs
{
    public partial class DialogBox : Window
    {
        public bool OK = false;
        
        public DialogBox(string caption, string message)
        {
            InitializeComponent();
            Owner = MainWindow.GetMainWindow();

            this.Title = caption;
            textBlockMessage.Text = message;
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
            else if(e.Key == Key.Enter)
            {
                this.OK = true;
                this.Close();
            }
        }
    }
}
