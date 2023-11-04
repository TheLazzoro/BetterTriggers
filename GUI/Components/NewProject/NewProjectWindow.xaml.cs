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
using GUI.Components.NewProject;

namespace GUI.Components.NewProject
{
    public partial class NewProjectWindow : Window
    {
        INewProjectControl control;
        public string projectPath;
        public bool doOpen;

        public NewProjectWindow()
        {
            this.Owner = MainWindow.GetMainWindow();
            InitializeComponent();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                this.Close();
        }

        private void btnEmptyProject_Click(object sender, RoutedEventArgs e)
        {
            EmptyProjectWindow control = new EmptyProjectWindow();
            this.control = control;
            control.OnOpenProject += OnOpenProject;
            grid.Children.Add(control);
            Grid.SetColumnSpan(control, 2);
        }


        private void btnConvert_Click(object sender, RoutedEventArgs e)
        {
            ConvertTriggersControl control = new ConvertTriggersControl();
            this.control = control;
            control.OnOpenProject += OnOpenProject;
            grid.Children.Add(control);
            Grid.SetColumnSpan(control, 2);
            Grid.SetRowSpan(control, 4);
        }

        private void OnOpenProject()
        {
            this.projectPath = control.ProjectLocation;
            this.doOpen = true;
            this.Close();
        }
    }
}
