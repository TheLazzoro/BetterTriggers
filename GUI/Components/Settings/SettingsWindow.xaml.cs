using BetterTriggers;
using BetterTriggers.Containers;
using BetterTriggers.Models.EditorData;
using BetterTriggers.WorldEdit.GameDataReader;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace GUI.Components.Settings
{
    public partial class SettingsWindow : Window
    {
        private EditorSettings settings;
        private string _fontSizePreviousValue;

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
            textboxFontSize.Text = settings.textEditorFontSize.ToString();
            comboboxEditorAppearance.SelectedIndex = (int)settings.editorAppearance;
            checkBoxShowGlobalDetail.IsChecked = settings.globalSuffixVisibility;
            checkBoxSingleClickExplorerElements.IsChecked = settings.singleClickExplorerElement;
            checkBoxQuickStart.IsChecked = settings.useQuickStart;

            foreach (FontFamily fontFamily in Fonts.SystemFontFamilies)
            {
                // FontFamily.Source contains the font family name.
                int itemIndex = comboboxScriptFont.Items.Add(fontFamily.Source);
                if (settings.textEditorFontStyle == fontFamily.Source)
                    comboboxScriptFont.SelectedIndex = itemIndex;
            }

            if(WarcraftStorageReader.GameVersion < WarcraftVersion._1_32)
            {
                lblError.Visibility = Visibility.Visible;
                comboboxDiff.IsEnabled = false;
                comboboxAssetMode.IsEnabled = false;
                comboboxTeenData.IsEnabled = false;
                textBoxPlayerProfile.IsEnabled = false;
                checkBoxFixedSeed.IsEnabled = false;

                lblDiff.Foreground = (SolidColorBrush)Application.Current.FindResource("TextBrushDim");
                lblAssetMode.Foreground = (SolidColorBrush)Application.Current.FindResource("TextBrushDim");
                lblTeenData.Foreground = (SolidColorBrush)Application.Current.FindResource("TextBrushDim");
                lblPlayerProfile.Foreground = (SolidColorBrush)Application.Current.FindResource("TextBrushDim");
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
            settings.textEditorFontSize = double.Parse(textboxFontSize.Text);
            settings.globalSuffixVisibility = (bool)checkBoxShowGlobalDetail.IsChecked;
            settings.singleClickExplorerElement = (bool)checkBoxSingleClickExplorerElements.IsChecked;
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

                var mainWindow = MainWindow.GetMainWindow();
                var tabs = mainWindow.tabViewModel;
                for (int i = 0; i < tabs.Tabs.Count; i++)
                {
                    var element = tabs.Tabs[i].explorerElement;
                    if (element.editor is ScriptControl scriptControl)
                        scriptControl.ReloadTextEditorTheme();
                }
            }
        }

        private void checkBoxShowGlobalDetail_Click(object sender, RoutedEventArgs e)
        {
            if (this.IsLoaded)
            {
                var project = Project.CurrentProject;
                foreach (var element in project.Variables.variableContainer)
                {
                    bool isVisible = checkBoxShowGlobalDetail.IsChecked == true;
                    element.SuffixVisibility = isVisible ? Visibility.Visible : Visibility.Collapsed;
                }
            }
        }

        private void textboxFontSize_TextChanged(object sender, TextChangedEventArgs e)
        {
            bool valid = int.TryParse(textboxFontSize.Text, out int size);
            if(!valid)
            {
                textboxFontSize.Text = _fontSizePreviousValue;
            }
            else
            {
                _fontSizePreviousValue = textboxFontSize.Text;
                TextEditor.ChangeFontSize(size, isDeltaChange: false);
            }
        }

        private void textboxFontSize_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            _fontSizePreviousValue = textboxFontSize.Text;
        }
    }
}
