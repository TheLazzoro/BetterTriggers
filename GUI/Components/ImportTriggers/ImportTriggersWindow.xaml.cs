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
    public partial class ImportTriggersWindow : Window
    {
        public ImportTriggersWindow()
        {
            InitializeComponent();
        }

        private void btnSelect_Click(object sender, RoutedEventArgs e)
        {
            OpenWar3MapWindow window = new();
            window.ShowDialog();
            if (!string.IsNullOrEmpty(window.SelectedPath))
            {
                LoadTriggers(window.SelectedPath);
            }
        }

        private void LoadTriggers(string mapPath)
        {

        }
    }
}
