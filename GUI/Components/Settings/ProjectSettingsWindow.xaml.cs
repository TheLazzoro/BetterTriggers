using BetterTriggers.Containers;
using System.IO;
using System.Windows;

namespace GUI.Components.Settings
{
    public partial class ProjectSettingsWindow : Window
    {
        private Project _project;

        public ProjectSettingsWindow(Project project)
        {
            this.Owner = MainWindow.GetMainWindow();

            InitializeComponent();

            _project = project;
            if (project.war3project.Language == "jass")
                comboboxMapScript.SelectedIndex = 0;
            else
                comboboxMapScript.SelectedIndex = 1;

            checkboxRelativeMapPath.IsChecked = project.war3project.UseRelativeMapDirectory;
            checkboxCompressFiles.IsChecked = project.war3project.CompressProjectFiles;
            checkboxGenerateAllObjects.IsChecked = project.war3project.GenerateAllObjectVariables;
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            var war3Project = _project.war3project;
            if (comboboxMapScript.SelectedIndex == 0)
                war3Project.Language = "jass";
            else
                war3Project.Language = "lua";

            war3Project.UseRelativeMapDirectory = (bool)checkboxRelativeMapPath.IsChecked;
            war3Project.CompressProjectFiles = (bool)checkboxCompressFiles.IsChecked;
            war3Project.GenerateAllObjectVariables = (bool)checkboxGenerateAllObjects.IsChecked;
            if (war3Project.UseRelativeMapDirectory)
                war3Project.War3MapDirectory = Path.GetFileName(war3Project.War3MapDirectory);
            else
            {
                war3Project.War3MapDirectory = _project.GetFullMapPath();
            }

            var root = _project.GetRoot();
            _project.UnsavedFiles.AddToUnsaved(root);

            this.Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
