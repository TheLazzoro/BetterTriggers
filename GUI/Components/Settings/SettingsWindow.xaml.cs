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
            comboboxTriggerStyle.SelectedIndex = (int)settings.triggerEditorMode;
            comboboxEditorAppearance.SelectedIndex = (int)settings.editorAppearance;
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
            settings.triggerEditorMode = (TriggerEditorMode)comboboxTriggerStyle.SelectedIndex;
            settings.textEditorFontStyle = comboboxScriptFont.Text;
            settings.useQuickStart = (bool)checkBoxQuickStart.IsChecked;

            EditorSettings.Save(settings);
            this.Close();

            // Refreshes editor tabs in case any relevant settings changed
            var mainWindow = MainWindow.GetMainWindow();
            var tabs = mainWindow.tabViewModel;
            for (int i = 0; i < tabs.Tabs.Count; i++)
            {
                var element = tabs.Tabs[i].explorerElement;
                if (element.ElementType == ExplorerElementEnum.Trigger
                    || element.ElementType == ExplorerElementEnum.ActionDefinition
                    || element.ElementType == ExplorerElementEnum.ConditionDefinition
                    || element.ElementType == ExplorerElementEnum.FunctionDefinition)
                {
                    element.ShouldRefreshUIElements = true;
                    element.Notify();
                }
                if (element.editor is ScriptControl scriptControl)
                    scriptControl.RefreshFontStyle();
            }
        }

        private void comboboxEditorAppearance_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.IsLoaded)
            {
                EditorTheme.Change((EditorAppearance)comboboxEditorAppearance.SelectedIndex);
            }
        }
    }
}
