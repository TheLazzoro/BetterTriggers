using BetterTriggers.Containers;
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
using War3Net.Build.Info;

namespace GUI.Components.Settings
{
    public partial class ProjectSettingsWindow : Window
    {
        public ProjectSettingsWindow()
        {
            this.Owner = MainWindow.GetMainWindow();

            InitializeComponent();

            var project = Project.CurrentProject.war3project;
            if (project.Language == "jass")
                comboboxMapScript.SelectedIndex = 0;
            else
                comboboxMapScript.SelectedIndex = 1;

            checkboxRelativeMapPath.IsChecked = project.UseRelativeMapDirectory;
            checkboxCompressFiles.IsChecked = project.CompressProjectFiles;
            checkboxGenerateAllObjects.IsChecked = project.GenerateAllObjectVariables;
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            var project = Project.CurrentProject.war3project;
            if (comboboxMapScript.SelectedIndex == 0)
                project.Language = "jass";
            else
                project.Language = "lua";

            project.UseRelativeMapDirectory = (bool)checkboxRelativeMapPath.IsChecked;
            project.CompressProjectFiles = (bool)checkboxCompressFiles.IsChecked;
            project.GenerateAllObjectVariables = (bool)checkboxGenerateAllObjects.IsChecked;
            if (project.UseRelativeMapDirectory)
                project.War3MapDirectory = Path.GetFileName(project.War3MapDirectory);
            else
            {
                project.War3MapDirectory = Project.CurrentProject.GetFullMapPath();
            }

            var root = Project.CurrentProject.GetRoot();
            Project.CurrentProject.UnsavedFiles.AddToUnsaved(root);

            this.Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
