using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;


namespace GUI
{

    public partial class AboutBox : Window
    {
        public AboutBox()
        {
            InitializeComponent();
            this.Owner = MainWindow.GetMainWindow();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape || e.Key == Key.Enter)
                this.Close();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
