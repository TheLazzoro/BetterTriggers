using Model.EditorData;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;


namespace GUI
{

    public partial class MessageBox : Window
    {
        public bool OK = false;
        
        public MessageBox(string caption, string content)
        {
            InitializeComponent();

            this.Title = caption;
            textBlockMessage.Text = content;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Window parentWindow = Application.Current.MainWindow;
            this.Top = parentWindow.Top + parentWindow.Height / 2 - this.Height / 2;
            this.Left = parentWindow.Left + parentWindow.Width / 2 - this.Width / 2;
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
