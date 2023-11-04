using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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

namespace GUI.Components.Keybindings
{
    public partial class SetKeyWindow : Window
    {
        public Key key;
        public bool ok;

        public SetKeyWindow()
        {
            Owner = MainWindow.GetMainWindow();
            InitializeComponent();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                this.Close();
                return;
            }

            key = e.Key;
            ok = true;
            this.Close();
        }
    }
}
