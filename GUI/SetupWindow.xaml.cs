using BetterTriggers;
using BetterTriggers.WorldEdit;
using System.Windows;

namespace GUI
{
    /// <summary>
    /// Interaction logic for SetupWindow.xaml
    /// </summary>
    public partial class SetupWindow : Window
    {
        public SetupWindow()
        {
            InitializeComponent();

            Settings settings = Settings.Load();
            textBoxRoot.Text = settings.war3root;
        }

        private void btnSelectWar3Dir_Click(object sender, RoutedEventArgs e)
        {
            using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
            {
                System.Windows.Forms.DialogResult result = dialog.ShowDialog();
                if(dialog.SelectedPath != "")
                    textBoxRoot.Text = dialog.SelectedPath;
            }
        }

        private void btnContinue_Click(object sender, RoutedEventArgs e)
        {
            Settings settings = Settings.Load();
            settings.war3root = textBoxRoot.Text;
            Settings.Save(settings);

            if(Casc.Load())
            {
                LoadingCascWindow window = new LoadingCascWindow();
                window.Show();
                Close();
            }
            else
                lblError.Visibility = Visibility.Visible;
        }
    }
}
