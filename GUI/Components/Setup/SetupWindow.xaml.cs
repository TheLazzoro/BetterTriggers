using BetterTriggers;
using BetterTriggers.WorldEdit.GameDataReader;
using GUI.Components.Dialogs;
using GUI.Components.Loading;
using System;
using System.Threading.Tasks;
using System.Windows;

namespace GUI.Components.Setup
{
    public partial class SetupWindow : Window
    {
        public SetupWindow()
        {
            InitializeComponent();

            EditorSettings settings = EditorSettings.Load();
            textBoxRoot.Text = settings.war3root;
            Init.HasLoaded = false;
            Task.Run(() => VersionCheck.VersionCheck.CheckVersion_PopupWindow(this));
        }

        private void btnSelectWar3Dir_Click(object sender, RoutedEventArgs e)
        {
            using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
            {
                System.Windows.Forms.DialogResult result = dialog.ShowDialog();
                if (dialog.SelectedPath != "")
                    textBoxRoot.Text = dialog.SelectedPath;
            }
        }

        private void btnContinue_Click(object sender, RoutedEventArgs e)
        {
            EditorSettings settings = EditorSettings.Load();
            settings.war3root = textBoxRoot.Text;
            EditorSettings.Save(settings);

            bool validCasc;
            string errorMsg;
            (validCasc, errorMsg) = WarcraftStorageReader.Load();
            if (validCasc)
            {
                LoadingCascWindow window = new LoadingCascWindow();
                window.Show();
                Close();
            }
            else
            {
                string hint = $"{Environment.NewLine}{Environment.NewLine}Hint: If this is the correct game directory, you can simply launch WC3, then close the game and re-launch Better Triggers. Sometimes a WC3 config file needs to be regenerated.";
                var messageBox = new Dialogs.MessageBox("Error", "Error: " + errorMsg + hint, this);
                messageBox.ShowDialog();
                lblError.Visibility = Visibility.Visible;
            }
        }
    }
}
