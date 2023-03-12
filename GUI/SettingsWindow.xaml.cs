using BetterTriggers;
using BetterTriggers.Models.EditorData;
using GUI.Components;
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
            comboboxTriggerStyle.SelectedIndex      = settings.triggerEditorMode;
            comboboxEditorAppearance.SelectedIndex  = settings.editorApperance;

            foreach (FontFamily fontFamily in Fonts.SystemFontFamilies)
            {
                // FontFamily.Source contains the font family name.
                int itemIndex = comboboxScriptFont.Items.Add(fontFamily.Source);
                if(settings.textEditorFontStyle == fontFamily.Source)
                    comboboxScriptFont.SelectedIndex = itemIndex;
            }
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
            settings.triggerEditorMode      = comboboxTriggerStyle.SelectedIndex;
            settings.textEditorFontStyle    = comboboxScriptFont.Text;

            Settings.Save(settings);
            this.Close();

            var mainWindow = MainWindow.GetMainWindow();
            var tabs = mainWindow.vmd;
            for (int i = 0; i < tabs.Tabs.Count; i++)
            {
                if (tabs.Tabs[i].explorerElement.Ielement is ExplorerElementTrigger)
                    tabs.Tabs[i].explorerElement.Reload();
                if(tabs.Tabs[i].explorerElement.editor is ScriptControl scriptControl)
                    scriptControl.RefreshFontStyle();
            }
        }

        private void comboboxEditorAppearance_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            EditorTheme.Change((EditorThemeUnion)comboboxEditorAppearance.SelectedIndex);
        }
    }
}
