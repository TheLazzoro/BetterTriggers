using BetterTriggers;
using BetterTriggers.Models.EditorData;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace GUI.Components.Settings
{
    public partial class SettingsWindow : Window
    {
        EditorSettings settings;

        public SettingsWindow()
        {
            InitializeComponent();

            settings = EditorSettings.Load();

            comboboxDiff.SelectedIndex = settings.Difficulty;
            comboboxWindowMode.SelectedIndex = settings.WindowMode;
            comboboxAssetMode.SelectedIndex = settings.HD;
            comboboxTeenData.SelectedIndex = settings.Teen;
            textBoxPlayerProfile.Text = settings.PlayerProfile;
            checkBoxFixedSeed.IsChecked = settings.FixedRandomSeed;
            checkBoxNoWFPause.IsChecked = settings.NoWindowsFocusPause;
            textBoxCopiedMapFile.Text = settings.CopyLocation;
            comboboxTriggerStyle.SelectedIndex = settings.triggerEditorMode;
            comboboxEditorAppearance.SelectedIndex = settings.editorAppearance;
            checkBoxQuickStart.IsChecked = settings.useQuickStart;

            foreach (FontFamily fontFamily in Fonts.SystemFontFamilies)
            {
                // FontFamily.Source contains the font family name.
                int itemIndex = comboboxScriptFont.Items.Add(fontFamily.Source);
                if (settings.textEditorFontStyle == fontFamily.Source)
                    comboboxScriptFont.SelectedIndex = itemIndex;
            }
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            settings.Difficulty = comboboxDiff.SelectedIndex;
            settings.WindowMode = comboboxWindowMode.SelectedIndex;
            settings.HD = comboboxAssetMode.SelectedIndex;
            settings.Teen = comboboxTeenData.SelectedIndex;
            settings.PlayerProfile = textBoxPlayerProfile.Text;
            settings.FixedRandomSeed = (bool)checkBoxFixedSeed.IsChecked;
            settings.NoWindowsFocusPause = (bool)checkBoxNoWFPause.IsChecked;
            settings.CopyLocation = textBoxCopiedMapFile.Text;
            settings.triggerEditorMode = comboboxTriggerStyle.SelectedIndex;
            settings.textEditorFontStyle = comboboxScriptFont.Text;
            settings.useQuickStart = (bool)checkBoxQuickStart.IsChecked;

            EditorSettings.Save(settings);
            this.Close();

            // Refreshes editor tabs in case any relevant settings changed
            var mainWindow = MainWindow.GetMainWindow();
            var tabs = mainWindow.tabViewModel;
            for (int i = 0; i < tabs.Tabs.Count; i++)
            {
                if (tabs.Tabs[i].explorerElement.ElementType == ExplorerElementEnum.Trigger)
                    tabs.Tabs[i].explorerElement.Notify();
                if (tabs.Tabs[i].explorerElement.editor is ScriptControl scriptControl)
                    scriptControl.RefreshFontStyle();
            }
        }

        private void comboboxEditorAppearance_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.IsLoaded)
            {
                EditorTheme.Change((EditorThemeUnion)comboboxEditorAppearance.SelectedIndex);
            }
        }
    }
}
