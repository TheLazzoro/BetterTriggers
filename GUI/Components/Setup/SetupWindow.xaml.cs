using BetterTriggers;
using BetterTriggers.WorldEdit;
using GUI.Components.Loading;
using System.Windows;

namespace GUI.Components.Setup
{
    /// <summary>
    /// Interaction logic for SetupWindow.xaml
    /// </summary>
    public partial class SetupWindow : Window
    {
        public SetupWindow()
        {
            InitializeComponent();

            EditorSettings settings = EditorSettings.Load();
            textBoxRoot.Text = settings.war3root;
            Init.HasLoaded = false;
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
            EditorSettings settings = EditorSettings.Load();
            settings.war3root = textBoxRoot.Text;
            EditorSettings.Save(settings);

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
