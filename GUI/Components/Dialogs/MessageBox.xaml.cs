using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;


namespace GUI.Components.Dialogs
{

    public partial class MessageBox : Window
    {
        public bool OK = false;
        
        public MessageBox(string caption, string content)
        {
            InitializeComponent();
            this.Owner = MainWindow.GetMainWindow();

            this.Title = caption;
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
            if (e.Key == Key.Escape || e.Key == Key.Enter)
                this.Close();
        }
    }
}
