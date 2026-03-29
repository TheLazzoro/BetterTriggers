using BetterTriggers.Containers;
using GUI.Components.OpenMap;
using System.Windows;

namespace GUI.Components.SelectMap
{
    public partial class SelectWar3MapWindow : Window
    {
        public bool OK = false;
        public string mapDir;
        private Project _project;

        public SelectWar3MapWindow(Project project)
        {
            InitializeComponent();

            _project = project;
            Owner = MainWindow.GetMainWindow();
        }

        private void btnSelect_Click(object sender, RoutedEventArgs e)
        {
            OpenWar3MapWindow window = new OpenWar3MapWindow(_project);
            window.ShowDialog();
            if (!window.OK)
                return;

            _project.SetWar3MapPath(window.SelectedPath);
            if (_project.War3MapDirExists())
            {
                mapDir = window.SelectedPath;
                lblPath.Content = mapDir;
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
            _project.SetWar3MapPath(mapDir);
            OK = true;
            this.Close();
        }
    }
}
