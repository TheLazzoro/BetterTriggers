using BetterTriggers.Controllers;
using GUI.Controllers;
using System;
using System.Collections.Generic;
using System.Text;
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
    /// Interaction logic for NewProjectWindow.xaml
    /// </summary>
    public partial class NewProjectWindow : Window
    {
        public bool Ok = false;
        public string projectFilePath;
        
        public NewProjectWindow()
        {
            InitializeComponent();

            radBtnJass.IsChecked = true;
        }

        private void radBtnJass_Checked(object sender, RoutedEventArgs e)
        {
            string hint = string.Empty;
            if ((bool)radBtnJass.IsChecked)
            {
                hint += "vJass benefits:\n";
                hint += "- Type checking\n";
                hint += "- Compatibility with 2 decades of Jass resources\n";
            }

            lblLanguageHint.Text = hint;
        }

        private void radBtnLua_Checked(object sender, RoutedEventArgs e)
        {
            string hint = string.Empty;
            if ((bool)radBtnLua.IsChecked)
            {
                hint += "Lua benefits:\n";
                hint += "- General-purpose language\n";
                hint += "- More advanced features\n";
            }

            lblLanguageHint.Text = hint;
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
            if(lblProjectDestination.Content != null && textBoxProjectName.Text.Length > 0)
                btnCreate.IsEnabled = true;
            else
                btnCreate.IsEnabled = false;
        }

        private void btnCreate_Click(object sender, RoutedEventArgs e)
        {
            string language = string.Empty;
            if ((bool)radBtnJass.IsChecked)
                language = "jass";
            else if ((bool)radBtnLua.IsChecked)
                language = "lua";

            ControllerProject controller = new ControllerProject();
            projectFilePath = controller.CreateProject(language, textBoxProjectName.Text, lblProjectDestination.Content.ToString());

            this.Ok = true;
            this.Close();
        }

        private void textBoxProjectName_TextChanged(object sender, TextChangedEventArgs e)
        {
            EnableButton();
        }
    }
}
