using BetterTriggers;
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

namespace GUI
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        Settings settings;

        public SettingsWindow()
        {
            InitializeComponent();

            settings = Settings.Load();

            comboboxDiff.SelectedIndex              = settings.Difficulty;
            comboboxWindowMode.SelectedIndex        = settings.WindowMode;
            comboboxAssetMode.SelectedIndex         = settings.HD;
            comboboxTeenData.SelectedIndex          = settings.Teen;
            textBoxPlayerProfile.Text               = settings.PlayerProfile;
            checkBoxFixedSeed.IsChecked             = settings.FixedRandomSeed;
            checkBoxNoWFPause.IsChecked             = settings.NoWindowsFocusPause;
            textBoxCopiedMapFile.Text               = settings.CopyLocation;
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            settings.Difficulty             = comboboxDiff.SelectedIndex;
            settings.WindowMode             = comboboxWindowMode.SelectedIndex;
            settings.HD                     = comboboxAssetMode.SelectedIndex;
            settings.Teen                   = comboboxTeenData.SelectedIndex;
            settings.PlayerProfile          = textBoxPlayerProfile.Text;
            settings.FixedRandomSeed        = (bool) checkBoxFixedSeed.IsChecked;
            settings.NoWindowsFocusPause    = (bool) checkBoxNoWFPause.IsChecked;
            settings.CopyLocation           = textBoxCopiedMapFile.Text;

            Settings.Save(settings);
            this.Close();
        }
    }
}
