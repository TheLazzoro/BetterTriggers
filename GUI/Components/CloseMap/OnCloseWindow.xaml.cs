using BetterTriggers.Containers;
using System.Windows;

namespace GUI.Components.CloseMap
{
    public partial class OnCloseWindow : Window
    {
        public bool Yes = false;
        public bool No = false;

        public OnCloseWindow(Project project)
        {
            InitializeComponent();
            this.Owner = MainWindow.GetMainWindow();

            lblMessage.Text = $"Save changes to '{project.MapName}'?";
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
