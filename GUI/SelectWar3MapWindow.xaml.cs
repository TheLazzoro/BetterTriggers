using BetterTriggers.Controllers;
using BetterTriggers.Utility;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
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
    public partial class SelectWar3MapWindow : Window
    {
        public bool OK = false;
        public string mapDir;

        public SelectWar3MapWindow()
        {
            InitializeComponent();
            Owner = MainWindow.GetMainWindow();
        }

        private void btnSelect_Click(object sender, RoutedEventArgs e)
        {
            ControllerProject controller = new ControllerProject();
            OpenWar3MapWindow window = new OpenWar3MapWindow();
            window.ShowDialog();
            if (!window.OK)
                return;

            controller.SetWar3MapPath(window.SelectedPath);
            if (controller.War3MapDirExists())
            {
                mapDir = window.SelectedPath;
                btnOK.IsEnabled = true;
                lblError.Visibility = Visibility.Hidden;
            }
            else
            {
                mapDir = null;
                btnOK.IsEnabled = false;
                lblError.Visibility = Visibility.Visible;
            }
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            ControllerProject controller = new();
            controller.SetWar3MapPath(mapDir);
            OK = true;
            this.Close();
        }
    }
}
