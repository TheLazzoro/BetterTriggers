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
    /// Interaction logic for ConvertTriggersWindow.xaml
    /// </summary>
    public partial class ConvertTriggersWindow : Window
    {
        public string ProjectLocation;
        public bool doOpen;

        public ConvertTriggersWindow()
        {
            Owner = MainWindow.GetMainWindow();
            InitializeComponent();
            control.OnOpenProject += Control_OnOpenProject;
        }

        private void Control_OnOpenProject()
        {
            ProjectLocation = control.ProjectLocation;
            doOpen = true;
            this.Close();
        }
    }
}
