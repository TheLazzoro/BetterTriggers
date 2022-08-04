using BetterTriggers.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace GUI
{
    /// <summary>
    /// Interaction logic for OnCloseWindow.xaml
    /// </summary>
    public partial class OnCloseWindow : Window
    {
        public bool Yes = false;
        public bool No = false;

        public OnCloseWindow()
        {
            InitializeComponent();
            this.Owner = MainWindow.GetMainWindow();

            ControllerProject controller = new ControllerProject();
            lblMessage.Text = $"Save changes to '{controller.GetProjectRoot().GetName()}'?";
        }

        private void btnYes_Click(object sender, RoutedEventArgs e)
        {
            Yes = true;
            this.Close();
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            No = true;
            this.Close();
        }
    }
}
