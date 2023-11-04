using BetterTriggers.Containers;
using GUI.Components.NewProject;
using GUI.Controllers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using War3Net.Build.Info;

namespace GUI.Components.NewProject
{
    public partial class EmptyProjectWindow : UserControl, INewProjectControl
    {
        public string ProjectLocation { get; set; }

        public event Action OnOpenProject;
        
        public EmptyProjectWindow()
        {
            InitializeComponent();

            radBtnJass.IsChecked = true;
        }

        private void btnProjectDestination_Click(object sender, RoutedEventArgs e)
        {
            using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
            {
                System.Windows.Forms.DialogResult result = dialog.ShowDialog();
                if(dialog.SelectedPath != "")
                {
                    lblProjectDestination.Content = dialog.SelectedPath;
                    EnableButton();
                }
            }
        }

        private void EnableButton()
        {
            if(Directory.Exists(lblProjectDestination.Content as string) && textBoxProjectName.Text.Length > 0)
                btnCreate.IsEnabled = true;
            else
                btnCreate.IsEnabled = false;
        }

        private void btnCreate_Click(object sender, RoutedEventArgs e)
        {
            ScriptLanguage language = ScriptLanguage.Jass;
            if ((bool)radBtnJass.IsChecked)
                language = ScriptLanguage.Jass;
            else if ((bool)radBtnLua.IsChecked)
                language = ScriptLanguage.Lua;

            ProjectLocation = Project.Create(language, textBoxProjectName.Text, lblProjectDestination.Content.ToString());
            OnOpenProject?.Invoke();
        }

        private void textBoxProjectName_TextChanged(object sender, TextChangedEventArgs e)
        {
            EnableButton();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                OnOpenProject?.Invoke();
        }
    }
}
