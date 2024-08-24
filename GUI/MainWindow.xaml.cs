using BetterTriggers;
using BetterTriggers.Containers;
using BetterTriggers.Logging;
using BetterTriggers.Models.EditorData;
using BetterTriggers.Models.SaveableData;
using BetterTriggers.TestMap;
using BetterTriggers.Utility;
using BetterTriggers.WorldEdit.GameDataReader;
using GUI.Components;
using GUI.Components.About;
using GUI.Components.BuildMap;
using GUI.Components.CloseMap;
using GUI.Components.Dialogs;
using GUI.Components.Keybindings;
using GUI.Components.Loading;
using GUI.Components.NewProject;
using GUI.Components.SaveMap;
using GUI.Components.SelectMap;
using GUI.Components.Settings;
using GUI.Components.Setup;
using GUI.Components.Tabs;
using GUI.Components.UserReports;
using GUI.Components.VariableList;
using GUI.Components.VerifyTriggers;
using GUI.Components.VersionCheck;
using Microsoft.Win32;
using NuGet.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Forms.Design;
using System.Windows.Input;
using System.Windows.Media;

namespace GUI
{
    public partial class MainWindow : Window
    {
        static MainWindow instance;
        TriggerExplorer triggerExplorer;
        public TabViewModel tabViewModel;

        public MainWindow()
        {
            InitializeComponent();
            instance = this;

            EditorSettings settings = EditorSettings.Load();
            this.Width = settings.mainWindowWidth;
            this.Height = settings.mainWindowHeight;
            this.Left = settings.mainWindowX;
            this.Top = settings.mainWindowY;
            this.WindowState = settings.mainWindowFullscreen ? WindowState.Maximized : WindowState.Normal;
            rowTriggerExplorer.Width = new GridLength(settings.triggerExplorerWidth);

            tabViewModel = new TabViewModel();
            tabControl.ItemsSource = tabViewModel.Tabs;

            CustomMapData.OnSaving += CustomMapData_OnSaving;

            btnNewMap.ToolTip = Locale.Translate(btnNewMap.ToolTip as string);
            //btnOpenMap.ToolTip          = Locale.Translate(btnOpenMap.ToolTip as string);

            btnUndo.ToolTip = Locale.Translate(btnUndo.ToolTip as string);
            btnRedo.ToolTip = Locale.Translate(btnRedo.ToolTip as string);
            // TODO:
            //btnCut.ToolTip              = Locale.Translate(btnCut.ToolTip as string);
            //btnCopy.ToolTip             = Locale.Translate(btnCopy.ToolTip as string);
            //btnPaste.ToolTip            = Locale.Translate(btnPaste.ToolTip as string);

            btnVariableMenu.ToolTip = Locale.Translate(btnVariableMenu.ToolTip as string);

            btnCreateFolder.ToolTip = Locale.Translate(btnCreateFolder.ToolTip as string);
            btnCreateTrigger.ToolTip = Locale.Translate(btnCreateTrigger.ToolTip as string);
            btnCreateScript.ToolTip = Locale.Translate(btnCreateScript.ToolTip as string);
            btnCreateVariable.ToolTip = Locale.Translate(btnCreateVariable.ToolTip as string);
            btnCreateActionDef.ToolTip = Locale.Translate(btnCreateActionDef.ToolTip as string);
            btnCreateConditionDef.ToolTip = Locale.Translate(btnCreateConditionDef.ToolTip as string);
            btnCreateFunctionDef.ToolTip = Locale.Translate(btnCreateFunctionDef.ToolTip as string);

            btnCreateEvent.ToolTip = Locale.Translate(btnCreateEvent.ToolTip as string);
            btnCreateCondition.ToolTip = Locale.Translate(btnCreateCondition.ToolTip as string);
            btnCreateAction.ToolTip = Locale.Translate(btnCreateAction.ToolTip as string);
            btnSaveScript.ToolTip = Locale.Translate(btnSaveScript.ToolTip as string);

            btnTestMap.ToolTip = Locale.Translate(btnTestMap.ToolTip as string);
            btnBuildMap.ToolTip = Locale.Translate(btnBuildMap.ToolTip as string);


            menuFile.Header = Locale.Translate(menuFile.Header as string);
            //menuOpen.Header             = Locale.Translate(menuOpen.Header as string);


            menuEdit.Header = Locale.Translate(menuEdit.Header as string);


            menuNew.Header = Locale.Translate(menuNew.Header as string);
            menuNewCategory.Header = Locale.Translate(menuNewCategory.Header as string);
            menuNewTrigger.Header = Locale.Translate(menuNewTrigger.Header as string);
            menuNewScript.Header = Locale.Translate(menuNewScript.Header as string);
            menuNewVariable.Header = Locale.Translate(menuNewVariable.Header as string);

            menuNewEvent.Header = Locale.Translate(menuNewEvent.Header as string);
            menuNewCondition.Header = Locale.Translate(menuNewCondition.Header as string);
            menuNewAction.Header = Locale.Translate(menuNewAction.Header as string);

            // TODO:
            //menuJassHelper.Header       = Locale.Translate(menuJassHelper.Header as string);
            //menuEnableJassHelper.Header = Locale.Translate(menuEnableJassHelper.Header as string);
            //menuEnableVJass.Header      = Locale.Translate(menuEnableVJass.Header as string);
            //menuEnableDebugMode.Header  = Locale.Translate(menuEnableDebugMode.Header as string);
            //menuEnableOptimizer.Header  = Locale.Translate(menuEnableOptimizer.Header as string);


            menuTools.Header = Locale.Translate(menuTools.Header as string);

            btnGameVersion.Content = "Game Version: " + WarcraftStorageReader.GameVersion;

            // Load keybindings
            Keybindings keybindings = Keybindings.Load();
            SetKeybindings(keybindings);

            RecentFiles.isTest = false; // hack

            Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            EditorSettings settings = EditorSettings.Load();
            if (settings.useQuickStart)
            {
                string lastOpenedProject = RecentFiles.GetRecentFiles().FirstOrDefault();
                if (!string.IsNullOrEmpty(lastOpenedProject))
                {
                    OpenProject(lastOpenedProject);
                }
            }

            ProgramSettings programSettings = ProgramSettings.Load();
            if (programSettings.IgnoreNewVersion == false)
            {
                Task.Run(CheckVersionOnStart);
            }

            LoggingService service = new LoggingService();
            service.SubmitSession();
        }

        private async Task CheckVersionOnStart()
        {
            var versionCheck = new VersionCheck();
            var version = await versionCheck.GetNewestVersionAsync();
            if (version.VersionCheckEnum == VersionCheckEnum.NewerExists)
            {
                Application.Current.Dispatcher.Invoke(delegate
                {
                    var window = new NewVersionWindow_OnStart(version);
                    window.ShowDialog();
                });
            }
        }

        /// <summary>
        /// This function only exists because of WPF wizardry.
        /// With a debug build 'Application.Current.MainWindow' works, but not in release.
        /// </summary>
        public static MainWindow GetMainWindow()
        {
            return instance;
        }

        public void SetKeybindings(Keybindings keybindings)
        {
            if (keybindings == null)
                return;

            keybindingNewProject.Key = keybindings.NewProject.key;
            keybindingNewProject.Modifiers = keybindings.NewProject.modifier;
            keybindingOpenProject.Key = keybindings.OpenProject.key;
            keybindingOpenProject.Modifiers = keybindings.OpenProject.modifier;
            keybindingSaveProject.Key = keybindings.SaveProject.key;
            keybindingSaveProject.Modifiers = keybindings.SaveProject.modifier;
            keybindingUndo.Key = keybindings.Undo.key;
            keybindingUndo.Modifiers = keybindings.Undo.modifier;
            keybindingRedo.Key = keybindings.Redo.key;
            keybindingRedo.Modifiers = keybindings.Redo.modifier;
            keybindingNewCategory.Key = keybindings.NewCategory.key;
            keybindingNewCategory.Modifiers = keybindings.NewCategory.modifier;
            keybindingNewTrigger.Key = keybindings.NewTrigger.key;
            keybindingNewTrigger.Modifiers = keybindings.NewTrigger.modifier;
            keybindingNewScript.Key = keybindings.NewScript.key;
            keybindingNewScript.Modifiers = keybindings.NewScript.modifier;
            keybindingNewVariable.Key = keybindings.NewGlobalVariable.key;
            keybindingNewVariable.Modifiers = keybindings.NewGlobalVariable.modifier;
            keybindingNewEvent.Key = keybindings.NewEvent.key;
            keybindingNewEvent.Modifiers = keybindings.NewEvent.modifier;
            keybindingNewCondition.Key = keybindings.NewCondition.key;
            keybindingNewCondition.Modifiers = keybindings.NewCondition.modifier;
            keybindingNewAction.Key = keybindings.NewAction.key;
            keybindingNewAction.Modifiers = keybindings.NewAction.modifier;
            keybindingNewLocalVariable.Key = keybindings.NewLocalVariable.key;
            keybindingNewLocalVariable.Modifiers = keybindings.NewLocalVariable.modifier;
            keybindingValidateTriggers.Key = keybindings.ValidateTriggers.key;
            keybindingValidateTriggers.Modifiers = keybindings.ValidateTriggers.modifier;
            keybindingTestMap.Key = keybindings.TestMap.key;
            keybindingTestMap.Modifiers = keybindings.TestMap.modifier;
            keybindingBuildMap.Key = keybindings.BuildMap.key;
            keybindingBuildMap.Modifiers = keybindings.BuildMap.modifier;

            menuNewProject.InputGestureText = Keybindings.GetModifierText(keybindingNewProject.Modifiers) + "+" + keybindingNewProject.Key;
            menuOpen.InputGestureText = Keybindings.GetModifierText(keybindingOpenProject.Modifiers) + "+" + keybindingOpenProject.Key;
            menuSave.InputGestureText = Keybindings.GetModifierText(keybindingSaveProject.Modifiers) + "+" + keybindingSaveProject.Key;
            menuItemUndo.InputGestureText = Keybindings.GetModifierText(keybindingUndo.Modifiers) + "+" + keybindingUndo.Key;
            menuItemRedo.InputGestureText = Keybindings.GetModifierText(keybindingRedo.Modifiers) + "+" + keybindingRedo.Key;
            menuNewCategory.InputGestureText = Keybindings.GetModifierText(keybindingNewCategory.Modifiers) + "+" + keybindingNewCategory.Key;
            menuNewTrigger.InputGestureText = Keybindings.GetModifierText(keybindingNewTrigger.Modifiers) + "+" + keybindingNewTrigger.Key;
            menuNewScript.InputGestureText = Keybindings.GetModifierText(keybindingNewScript.Modifiers) + "+" + keybindingNewScript.Key;
            menuNewVariable.InputGestureText = Keybindings.GetModifierText(keybindingNewVariable.Modifiers) + "+" + keybindingNewVariable.Key;
            menuNewEvent.InputGestureText = Keybindings.GetModifierText(keybindingNewEvent.Modifiers) + "+" + keybindingNewEvent.Key;
            menuNewCondition.InputGestureText = Keybindings.GetModifierText(keybindingNewCondition.Modifiers) + "+" + keybindingNewCondition.Key;
            menuNewAction.InputGestureText = Keybindings.GetModifierText(keybindingNewAction.Modifiers) + "+" + keybindingNewAction.Key;
            menuNewLocalVariable.InputGestureText = Keybindings.GetModifierText(keybindingNewLocalVariable.Modifiers) + "+" + keybindingNewLocalVariable.Key;


            if (globalValidateTriggers != null)
            {
                globalValidateTriggers.Dispose();
                globalValidateTriggers = null;
            }
            if (globalTestMap != null)
            {
                globalTestMap.Dispose();
                globalTestMap = null;
            }
            if (globalBuildMap != null)
            {
                globalBuildMap.Dispose();
                globalBuildMap = null;
            }

            if (keybindings.ValidateTriggers.global)
                globalValidateTriggers = new HotKey(keybindingValidateTriggers.Key, keybindingValidateTriggers.Modifiers, GlobalHotkeyValidateTriggers);
            if (keybindings.TestMap.global)
                globalTestMap = new HotKey(keybindingTestMap.Key, keybindingTestMap.Modifiers, GlobalHotkeyTestMap);
            if (keybindings.BuildMap.global)
                globalBuildMap = new HotKey(keybindingBuildMap.Key, keybindingBuildMap.Modifiers, GlobalHotkeyBuildMap);
        }

        HotKey globalValidateTriggers;
        HotKey globalTestMap;
        HotKey globalBuildMap;

        private void GlobalHotkeyValidateTriggers(HotKey hotKey)
        {
            if (IsProjectActive())
            {
                Builder builder = new();
                builder.GenerateScript();
            }
        }
        private void GlobalHotkeyTestMap(HotKey hotKey)
        {
            if (IsProjectActive())
                TestMap();
        }
        private void GlobalHotkeyBuildMap(HotKey hotKey)
        {
            if (IsProjectActive())
                BuildMap();
        }

        public Keybindings GetKeybindings()
        {
            Keybindings keybindings = new Keybindings();

            keybindings.NewProject = new Keybinding() { key = keybindingNewProject.Key, modifier = keybindingNewProject.Modifiers };
            keybindings.OpenProject = new Keybinding() { key = keybindingOpenProject.Key, modifier = keybindingOpenProject.Modifiers };
            keybindings.SaveProject = new Keybinding() { key = keybindingSaveProject.Key, modifier = keybindingSaveProject.Modifiers };
            keybindings.Undo = new Keybinding() { key = keybindingUndo.Key, modifier = keybindingUndo.Modifiers };
            keybindings.Redo = new Keybinding() { key = keybindingRedo.Key, modifier = keybindingRedo.Modifiers };
            keybindings.NewCategory = new Keybinding() { key = keybindingNewCategory.Key, modifier = keybindingNewCategory.Modifiers };
            keybindings.NewTrigger = new Keybinding() { key = keybindingNewTrigger.Key, modifier = keybindingNewTrigger.Modifiers };
            keybindings.NewScript = new Keybinding() { key = keybindingNewScript.Key, modifier = keybindingNewScript.Modifiers };
            keybindings.NewGlobalVariable = new Keybinding() { key = keybindingNewVariable.Key, modifier = keybindingNewVariable.Modifiers };
            keybindings.NewEvent = new Keybinding() { key = keybindingNewEvent.Key, modifier = keybindingNewEvent.Modifiers };
            keybindings.NewCondition = new Keybinding() { key = keybindingNewCondition.Key, modifier = keybindingNewCondition.Modifiers };
            keybindings.NewAction = new Keybinding() { key = keybindingNewAction.Key, modifier = keybindingNewAction.Modifiers };
            keybindings.NewLocalVariable = new Keybinding() { key = keybindingNewLocalVariable.Key, modifier = keybindingNewLocalVariable.Modifiers };
            keybindings.ValidateTriggers = new Keybinding() { key = keybindingValidateTriggers.Key, modifier = keybindingValidateTriggers.Modifiers, global = globalValidateTriggers != null };
            keybindings.TestMap = new Keybinding() { key = keybindingTestMap.Key, modifier = keybindingTestMap.Modifiers, global = globalTestMap != null };
            keybindings.BuildMap = new Keybinding() { key = keybindingBuildMap.Key, modifier = keybindingBuildMap.Modifiers, global = globalBuildMap != null };

            return keybindings;
        }

        private void CustomMapData_OnSaving()
        {
            Application.Current.Dispatcher.Invoke(delegate
            {
                var window = new SavingMapWindow();
                window.WindowStartupLocation = WindowStartupLocation.Manual;
                window.Top = this.Top + this.Height / 2 - window.Height / 2;
                window.Left = this.Left + this.Width / 2 - window.Width / 2;
                window.ShowDialog();

                VerifyTriggerData();
            });
        }

        private void VerifyTriggerData()
        {
            VerifyingTriggersWindow window = new VerifyingTriggersWindow();
            window.OnCloseProject += Window_OnCloseProject;
            window.ShowDialog();
            window.OnCloseProject -= Window_OnCloseProject;
        }

        private void Window_OnCloseProject()
        {
            CloseProject(true);
        }

        private void OnSelectTab(ExplorerElement selectedItem, TabViewModel tabViewModel, TabControl tabControl)
        {
            if (selectedItem.ElementType == ExplorerElementEnum.Trigger)
            {
                ExplorerElement.CurrentToRender = selectedItem;
                EnableECAButtons(true);
                EnableParameterButton(false);
            }
            else if(selectedItem.ElementType == ExplorerElementEnum.ActionDefinition
                    || selectedItem.ElementType == ExplorerElementEnum.ConditionDefinition
                    || selectedItem.ElementType == ExplorerElementEnum.FunctionDefinition)
            {
                ExplorerElement.CurrentToRender = selectedItem;
                EnableECAButtons(true);
                EnableParameterButton(true);
            }
            else
            {
                EnableECAButtons(false);
                EnableParameterButton(false);
            }

            if (!tabViewModel.Contains(selectedItem))
            {
                UserControl editor = null;
                switch (selectedItem.ElementType)
                {
                    case ExplorerElementEnum.Folder:
                        return;
                    case ExplorerElementEnum.GlobalVariable:
                        var variableControl = new VariableControl(selectedItem, selectedItem.variable);
                        editor = variableControl;
                        break;
                    case ExplorerElementEnum.Root:
                        var rootControl = new RootControl();
                        editor = rootControl;
                        break;
                    case ExplorerElementEnum.Script:
                        var scriptControl = new ScriptControl(selectedItem);
                        editor = scriptControl;
                        break;
                    case ExplorerElementEnum.Trigger:
                    case ExplorerElementEnum.ActionDefinition:
                    case ExplorerElementEnum.ConditionDefinition:
                    case ExplorerElementEnum.FunctionDefinition:
                        var triggerControl = new TriggerControl(selectedItem);
                        editor = triggerControl;
                        break;
                    default:
                        break;
                }

                selectedItem.editor = editor;

                TabItemBT tabItem = new TabItemBT(selectedItem, editor, tabViewModel);
                tabViewModel.Tabs.Add(tabItem);
            }

            tabControl.SelectedIndex = tabViewModel.IndexOf(selectedItem);
        }

        private void TreeViewTriggerExplorer_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            var selected = e.NewValue as ExplorerElement;
            if (selected == null)
                return;

            Project.CurrentProject.currentSelectedElement = selected.GetPath();
        }

        private void TriggerExplorer_OnOpenExplorerElement(ExplorerElement opened)
        {
            if (opened == null)
                return;

            OnSelectTab(opened, tabViewModel, tabControl);
        }

        private void tabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (tabControl.Items.Count == 0)
                return;

            TabItemBT tabItem = tabControl.SelectedItem as TabItemBT;
            if (tabItem == null) // it crashes when we don't do this?
                return;

            OnSelectTab(tabItem.explorerElement, tabViewModel, tabControl);
        }

        private void ButtonTabItem_Click(object sender, RoutedEventArgs e)
        {
            var btn = (Button)e.Source;
            TabItemBT tabItem = (TabItemBT)btn.DataContext;
            tabItem.Close();

            if (tabControl.Items.Count == 0)
            {
                EnableECAButtons(false);
                EnableParameterButton(false);
            }
        }

        private void btnNewMap_Click(object sender, RoutedEventArgs e)
        {
            NewProject();
        }

        private void btnOpenMap_Click(object sender, RoutedEventArgs e)
        {
            OpenMap();
        }

        private void btnSaveAll_Click(object sender, RoutedEventArgs e)
        {
            var project = Project.CurrentProject;
            if(project == null) return;

            project.Save();
        }

        private void btnUndo_Click(object sender, RoutedEventArgs e)
        {
            Project.CurrentProject.CommandManager.Undo();
        }

        private void btnRedo_Click(object sender, RoutedEventArgs e)
        {
            Project.CurrentProject.CommandManager.Redo();
        }

        private void btnCreateFolder_Click(object sender, RoutedEventArgs e)
        {
            Project.CurrentProject.Folders.Create();
        }

        private void btnCreateTrigger_Click(object sender, RoutedEventArgs e)
        {
            Project.CurrentProject.Triggers.Create();
        }

        private void btnCreateScript_Click(object sender, RoutedEventArgs e)
        {
            Project.CurrentProject.Scripts.Create();
        }

        private void btnCreateVariable_Click(object sender, RoutedEventArgs e)
        {
            Project.CurrentProject.Variables.Create();
        }

        private void btnCreateActionDefinition_Click(object sender, RoutedEventArgs e)
        {
            Project.CurrentProject.ActionDefinitions.Create();
        }

        private void btnCreateConditionDefinition_Click(object sender, RoutedEventArgs e)
        {
            Project.CurrentProject.ConditionDefinitions.Create();
        }

        private void btnCreateFunctionDefinition_Click(object sender, RoutedEventArgs e)
        {
            Project.CurrentProject.FunctionDefinitions.Create();
        }

        private void btnCreateEvent_Click(object sender, RoutedEventArgs e)
        {
            var selected = tabControl.SelectedItem as TabItemBT;
            var triggerControl = selected.explorerElement.editor as TriggerControl;
            triggerControl.CreateEvent();
        }

        private void btnCreateCondition_Click(object sender, RoutedEventArgs e)
        {
            var selected = tabControl.SelectedItem as TabItemBT;
            var triggerControl = selected.explorerElement.editor as TriggerControl;
            triggerControl.CreateCondition();
        }

        private void btnCreateAction_Click(object sender, RoutedEventArgs e)
        {
            var selected = tabControl.SelectedItem as TabItemBT;
            var triggerControl = selected.explorerElement.editor as TriggerControl;
            triggerControl.CreateAction();
        }

        private void btnSaveScript_Click(object sender, RoutedEventArgs e)
        {
            Builder builder = new Builder();
            builder.GenerateScript();
        }

        private void btnTestMap_Click(object sender, RoutedEventArgs e)
        {
            TestMap();

        }

        private void btnBuildMap_Click(object sender, RoutedEventArgs e)
        {
            BuildMap();
        }


        private void MenuItem_SubmenuOpened(object sender, RoutedEventArgs e)
        {
            bool isProjectOpen = Project.CurrentProject != null;

            bool canUndo = false;
            bool canRedo = false;
            string nameCommandToUndo = string.Empty;
            string nameCommandToRedo = string.Empty;
            if (isProjectOpen)
            {
                canUndo = Project.CurrentProject.CommandManager.CanUndo();
                canRedo = Project.CurrentProject.CommandManager.CanRedo();
                nameCommandToUndo = Project.CurrentProject.CommandManager.GetNameCommandToUndo();
                nameCommandToRedo = Project.CurrentProject.CommandManager.GetNameCommandToRedo();
            }

            menuItemUndo.IsEnabled = canUndo;
            menuItemRedo.IsEnabled = canRedo;
            if (canUndo)
                menuItemUndo.Header = $"Undo '{nameCommandToUndo}'";
            else
                menuItemUndo.Header = "Undo";

            if (canRedo)
                menuItemRedo.Header = $"Redo '{nameCommandToRedo}'";
            else
                menuItemRedo.Header = "Redo";
        }


        private void menuItemOptions_Click(object sender, RoutedEventArgs e)
        {
            SettingsWindow settings = new SettingsWindow();
            settings.WindowStartupLocation = WindowStartupLocation.Manual;
            settings.Top = this.Top + this.Height / 2 - settings.Height / 2;
            settings.Left = this.Left + this.Width / 2 - settings.Width / 2;
            settings.ShowDialog();
        }

        private void NewProject()
        {
            NewProjectWindow window = new();
            window.WindowStartupLocation = WindowStartupLocation.Manual;
            window.Top = this.Top + this.Height / 2 - window.Height / 2;
            window.Left = this.Left + this.Width / 2 - window.Width / 2;
            window.ShowDialog();

            if (window.doOpen && DoCloseProject())
                OpenProject(window.projectPath);
        }

        private void OpenMap()
        {

            OpenFileDialog dialog = new OpenFileDialog();
            dialog.DefaultExt = ".json";
            dialog.Filter = "JSON Files (*.json)|*.json";
            if (dialog.ShowDialog() == true && DoCloseProject())
            {
                OpenProject(dialog.FileName);
            }
        }

        private void menuRecentFiles_MouseEnter(object sender, MouseEventArgs e)
        {
            menuRecentFiles.Items.Clear();
            List<string> recentFiles = RecentFiles.GetRecentFiles();

            for (int i = 0; i < recentFiles.Count; i++)
            {
                int index = i;
                MenuItem item = new MenuItem();
                item.Header = recentFiles[i];
                item.Click += delegate
                {
                    OpenProject(recentFiles[index]);
                };
                menuRecentFiles.Items.Add(item);
            }
        }

        private bool IsProjectActive()
        {
            return triggerExplorer != null;
        }

        private void OpenProject(string file)
        {
            War3Project project = null;
            LoadingProjectFilesWindow loadingFilesWindow = new LoadingProjectFilesWindow(file);
            loadingFilesWindow.ShowDialog();
            project = loadingFilesWindow.project;
            if (project == null)
                return;

            if (!Project.CurrentProject.War3MapDirExists())
            {
                SelectWar3MapWindow window = new SelectWar3MapWindow();
                window.ShowDialog();
                if (!window.OK)
                {
                    return;
                }
            }

            tabViewModel.CloseAll();
            if (triggerExplorer != null)
            {
                var parent = (Grid)triggerExplorer.Parent;
                parent.Children.Remove(triggerExplorer);
                triggerExplorer.treeViewTriggerExplorer.SelectedItemChanged -= TreeViewTriggerExplorer_SelectedItemChanged;
                triggerExplorer.OnOpenExplorerElement -= TriggerExplorer_OnOpenExplorerElement;
            }
            triggerExplorer = new TriggerExplorer();
            TriggerExplorer.Current = triggerExplorer;
            triggerExplorer.Margin = new Thickness(-1, 1, 4, -1);
            triggerExplorer.HorizontalAlignment = HorizontalAlignment.Stretch;
            triggerExplorer.Width = double.NaN;
            triggerExplorer.BorderBrush = new SolidColorBrush(Color.FromArgb(255, 32, 32, 32));
            mainGrid.Children.Add(triggerExplorer);
            Grid.SetRow(triggerExplorer, 3);
            Grid.SetRowSpan(triggerExplorer, 4);
            Grid.SetColumn(triggerExplorer, 0);

            triggerExplorer.treeViewTriggerExplorer.SelectedItemChanged += TreeViewTriggerExplorer_SelectedItemChanged;
            triggerExplorer.OnOpenExplorerElement += TriggerExplorer_OnOpenExplorerElement;

            EnableToolbar(true);

            VerifyTriggerData();
            OpenLastOpenedTabs();
        }

        private void OpenLastOpenedTabs()
        {
            Project project = Project.CurrentProject;
            if(project == null)
            {
                return;
            }
            var lastOpenedTabs = LastOpenedTabs.Load(project.GetRoot().GetName());
            var explorerElements = project.GetAllExplorerElements();
            if (lastOpenedTabs.Tabs != null)
            {
                foreach (string lastOpenedPath in lastOpenedTabs.Tabs)
                {
                    for (int i = 0; i < explorerElements.Count; i++)
                    {
                        var element = explorerElements[i];
                        if(element.path == lastOpenedPath)
                        {
                            OnSelectTab(element, tabViewModel, tabControl);
                            break;
                        }
                    }
                }
            }
        }


        private void TestMap()
        {
            Builder builder = new Builder();
            if (!Project.CurrentProject.War3MapDirExists())
            {
                SelectWar3MapWindow window = new SelectWar3MapWindow();
                window.ShowDialog();
                if (!window.OK)
                {
                    return;
                }
            }
            try
            {
                builder.TestMap();
            }
            catch (Exception ex)
            {
                Components.Dialogs.MessageBox dialogBox = new Components.Dialogs.MessageBox("Error", ex.Message);
                dialogBox.ShowDialog();
            }
        }

        private void BuildMap()
        {
            if (!Project.CurrentProject.War3MapDirExists())
            {
                SelectWar3MapWindow window = new SelectWar3MapWindow();
                window.ShowDialog();
                if (!window.OK)
                {
                    return;
                }
            }
            try
            {
                BuildMapWindow window = new BuildMapWindow();
                window.ShowDialog();
            }
            catch (Exception ex)
            {
                Components.Dialogs.MessageBox dialogBox = new Components.Dialogs.MessageBox("Error", ex.Message);
                dialogBox.ShowDialog();
            }
        }

        private void EnableToolbar(bool enable)
        {
            btnVariableMenu.IsEnabled = enable;
            btnCreateFolder.IsEnabled = enable;
            btnCreateTrigger.IsEnabled = enable;
            btnCreateScript.IsEnabled = enable;
            btnCreateVariable.IsEnabled = enable;
            btnCreateActionDef.IsEnabled = enable;
            btnCreateConditionDef.IsEnabled = enable;
            btnCreateFunctionDef.IsEnabled = enable;
            btnSaveScript.IsEnabled = enable;
            btnTestMap.IsEnabled = enable;
            btnBuildMap.IsEnabled = enable;
        }

        private void EnableECAButtons(bool enable)
        {
            btnCreateEvent.IsEnabled = enable;
            btnCreateCondition.IsEnabled = enable;
            btnCreateLocalVariable.IsEnabled = enable;
            btnCreateAction.IsEnabled = enable;
        }

        private void EnableParameterButton(bool enable)
        {
            btnCreateParameter.IsEnabled = enable;
        }

        private void Rectangle_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void Menu_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            bool doClose = DoCloseProject();
            EditorSettings.Save(EditorSettings.Load());
            Keybindings.Save(GetKeybindings());
            SaveLastOpenedTabs();

            if (globalBuildMap != null)
                globalBuildMap.Dispose();
            if (globalTestMap != null)
                globalTestMap.Dispose();
            if (globalValidateTriggers != null)
                globalValidateTriggers.Dispose();

            e.Cancel = !doClose;
        }

        private bool DoCloseProject()
        {
            if (Project.CurrentProject == null)
                return true;

            if (Project.CurrentProject.CommandManager.HasUnsavedChanges == false)
                return true;

            OnCloseWindow onCloseWindow = new OnCloseWindow();
            onCloseWindow.ShowDialog();
            if (onCloseWindow.Yes)
            {
                Project.CurrentProject.EnableFileEvents(false);
                Project.CurrentProject.Save();
                return true;
            }
            else if (!onCloseWindow.Yes && !onCloseWindow.No)
                return false;

            return true;
        }


        private void CommandBinding_CanExecute_NewProject(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void CommandBinding_Executed_NewProject(object sender, ExecutedRoutedEventArgs e)
        {
            NewProject();
        }

        private void CommandBinding_CanExecute_OpenProject(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void CommandBinding_Executed_OpenProject(object sender, ExecutedRoutedEventArgs e)
        {
            OpenMap();
        }

        private void CommandBinding_CanExecute_IsProjectActive(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = IsProjectActive();
        }

        private void CommandBinding_Executed_Save(object sender, ExecutedRoutedEventArgs e)
        {
            Project.CurrentProject.Save();
        }


        private void CommandBinding_Executed_CloseProject(object sender, ExecutedRoutedEventArgs e)
        {
            CloseProject();
        }

        private void CloseProject(bool forceClose = false)
        {
            if (!forceClose)
            {
                if (!DoCloseProject())
                    return;
            }

            SaveLastOpenedTabs();
            tabViewModel.CloseAll();
            mainGrid.Children.Remove(triggerExplorer);
            triggerExplorer = null;
            EnableToolbar(false);
            EnableECAButtons(false);
            EnableParameterButton(false);

            Project.Close();
        }

        private void SaveLastOpenedTabs()
        {
            if (Project.CurrentProject == null)
                return;

            int tabIndex = 0;
            string[] tabs = new string[tabViewModel.Tabs.Count];
            var enumerator = tabViewModel.Tabs.GetEnumerator();
            while (enumerator.MoveNext())
            {
                tabs[tabIndex] = enumerator.Current.explorerElement.GetPath();
                tabIndex++;
            }
            LastOpenedTabs.Save(Project.CurrentProject.GetRoot().GetName(), tabs);
        }

        private void CommandBinding_Executed_ImportTriggers(object sender, ExecutedRoutedEventArgs e)
        {
            ImportTriggersWindow window = new ImportTriggersWindow();
            window.ShowDialog();
        }

        private void CommandBinding_Executed_OpenProjectSettings(object sender, ExecutedRoutedEventArgs e)
        {
            ProjectSettingsWindow projectSettings = new ProjectSettingsWindow();
            projectSettings.ShowDialog();
        }

        private void CommandBinding_CanExecute_Undo(object sender, CanExecuteRoutedEventArgs e)
        {
            if (Project.CurrentProject != null)
                e.CanExecute = Project.CurrentProject.CommandManager.CanUndo();
        }

        private void CommandBinding_Executed_Undo(object sender, ExecutedRoutedEventArgs e)
        {
            Project.CurrentProject.CommandManager.Undo();
        }

        private void CommandBinding_CanExecute_Redo(object sender, CanExecuteRoutedEventArgs e)
        {
            if (Project.CurrentProject != null)
                e.CanExecute = Project.CurrentProject.CommandManager.CanRedo();
        }

        private void CommandBinding_Executed_Redo(object sender, ExecutedRoutedEventArgs e)
        {
            Project.CurrentProject.CommandManager.Redo();
        }

        private void btnVariableMenu_Click(object sender, RoutedEventArgs e)
        {
            VariableListWindow window = new VariableListWindow();
            window.ShowDialog();
        }

        private void CommandBinding_Executed_NewCategory(object sender, ExecutedRoutedEventArgs e)
        {
            Project.CurrentProject.Folders.Create();
        }

        private void CommandBinding_Executed_NewTrigger(object sender, ExecutedRoutedEventArgs e)
        {
            Project.CurrentProject.Triggers.Create();
        }

        private void CommandBinding_Executed_NewScript(object sender, ExecutedRoutedEventArgs e)
        {
            Project.CurrentProject.Scripts.Create();
        }

        private void CommandBinding_Executed_NewGlobalVariable(object sender, ExecutedRoutedEventArgs e)
        {
            Project.CurrentProject.Variables.Create();
        }

        private void CommandBinding_CanExecute_IsControlTrigger(object sender, CanExecuteRoutedEventArgs e)
        {
            if(triggerExplorer == null)
            {
                e.CanExecute = false;
                return;
            }

            var selected = tabControl.SelectedItem as TabItemBT;
            if (selected != null)
            {
                var triggerControl = selected.explorerElement.editor as TriggerControl;
                e.CanExecute = triggerControl != null;
            }
        }

        private void CommandBinding_Executed_NewEvent(object sender, ExecutedRoutedEventArgs e)
        {
            var selected = tabControl.SelectedItem as TabItemBT;
            var triggerControl = selected.explorerElement.editor as TriggerControl;
            triggerControl.CreateEvent();
        }

        private void CommandBinding_Executed_NewCondition(object sender, ExecutedRoutedEventArgs e)
        {
            var selected = tabControl.SelectedItem as TabItemBT;
            var triggerControl = selected.explorerElement.editor as TriggerControl;
            triggerControl.CreateCondition();
        }

        private void CommandBinding_Executed_NewLocalVariable(object sender, ExecutedRoutedEventArgs e)
        {
            var selected = tabControl.SelectedItem as TabItemBT;
            var triggerControl = selected.explorerElement.editor as TriggerControl;
            triggerControl.CreateLocalVariable();
        }

        private void CommandBinding_Executed_NewAction(object sender, ExecutedRoutedEventArgs e)
        {
            var selected = tabControl.SelectedItem as TabItemBT;
            var triggerControl = selected.explorerElement.editor as TriggerControl;
            triggerControl.CreateAction();
        }

        private void btnCreateLocalVariable_Click(object sender, RoutedEventArgs e)
        {
            var selected = tabControl.SelectedItem as TabItemBT;
            var triggerControl = selected.explorerElement.editor as TriggerControl;
            triggerControl.CreateLocalVariable();
        }

        private void btnCreateParameter_Click(object sender, RoutedEventArgs e)
        {
            var selected = tabControl.SelectedItem as TabItemBT;
            var triggerControl = selected.explorerElement.editor as TriggerControl;
            triggerControl.CreateParameter();
        }

        private void CommandBinding_Executed_ValidateTriggers(object sender, ExecutedRoutedEventArgs e)
        {
            Builder builder = new Builder();
            builder.GenerateScript();
        }

        private void CommandBinding_Executed_TestMap(object sender, ExecutedRoutedEventArgs e)
        {
            Builder builder = new Builder();
            try
            {
                builder.TestMap();
            }
            catch (Exception ex)
            {
                Components.Dialogs.MessageBox dialogBox = new Components.Dialogs.MessageBox("Error", ex.Message);
                dialogBox.ShowDialog();
            }
        }

        private void CommandBinding_Executed_BuildMap(object sender, ExecutedRoutedEventArgs e)
        {
            BuildMap();
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            EditorSettings settings = EditorSettings.Load();
            settings.mainWindowWidth = (int)this.Width;
            settings.mainWindowHeight = (int)this.Height;
        }

        private void Window_LocationChanged(object sender, EventArgs e)
        {
            EditorSettings settings = EditorSettings.Load();
            settings.mainWindowX = (int)this.Left;
            settings.mainWindowY = (int)this.Top;
        }

        private void Window_StateChanged(object sender, EventArgs e)
        {
            EditorSettings settings = EditorSettings.Load();
            settings.mainWindowFullscreen = this.WindowState.HasFlag(WindowState.Maximized);
        }

        private void GridSplitter_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            EditorSettings settings = EditorSettings.Load();
            settings.triggerExplorerWidth = (int)rowTriggerExplorer.Width.Value;
        }

        private void menuItemHelp_Click(object sender, RoutedEventArgs e)
        {
            var uri = "https://thelazzoro.github.io/BetterTriggersGuide/#/Guide";
            var psi = new System.Diagnostics.ProcessStartInfo();
            psi.UseShellExecute = true;
            psi.FileName = uri;
            System.Diagnostics.Process.Start(psi);
        }

        private void menuItemAbout_Click(object sender, RoutedEventArgs e)
        {
            AboutBox aboutBox = new AboutBox();
            aboutBox.ShowDialog();
        }

        private void menuItemKeybindings_Click(object sender, RoutedEventArgs e)
        {
            KeybindingWindow window = new KeybindingWindow(GetKeybindings());
            window.ShowDialog();
            if (window.OK)
                SetKeybindings(window.keybindings);
        }

        private void btnGameVersion_Click(object sender, RoutedEventArgs e)
        {
            DialogBox dialog = new DialogBox("Change Game Version", "Changing game version requires you to point to a different Warcraft III installation.\n\nThis action closes the current project.\nDo you wish to continue?");
            dialog.ShowDialog();
            if (dialog.OK)
            {
                if (!DoCloseProject())
                    return;
            }
            else
                return;

            SaveLastOpenedTabs();
            SetupWindow window = new SetupWindow();
            window.Show();
            this.Close();
        }


        TabItem tabItem_rightClicked;
        private void tabControl_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            var item = e.OriginalSource as DependencyObject;
            while (item != null)
            {
                if (item is TextElement)
                    break;

                if (item is TabItem)
                    break;

                item = VisualTreeHelper.GetParent(item);
            }

            if (item == null || item is not TabItem)
            {
                // TODO: We need to find a way to prevent the opening of this context menu
                // while still not preventing the opening of the context menu in the trigger editor.
                // e.Handled = true will just prevent all other consecutive events from firing.
                //e.Handled = true;
                return;
            }

            // Gets here if valid tabitem found
            tabItem_rightClicked = (TabItem)item;
        }

        private void tabitem_Menu_CloseAll_Click(object sender, RoutedEventArgs e)
        {
            tabViewModel.CloseAll();
        }

        private void tabitem_Menu_Close_Click(object sender, RoutedEventArgs e)
        {
            if (tabItem_rightClicked != null)
            {
                var header = tabItem_rightClicked.Header as TabItemBT;
                int index = tabControl.Items.IndexOf(header);
                tabViewModel.Tabs.RemoveAt(index);
            }
        }

        private void tabitem_Menu_NavigateToElement_Click(object sender, RoutedEventArgs e)
        {
            if (tabItem_rightClicked != null)
            {
                var header = tabItem_rightClicked.Header as TabItemBT;
                var explorerElement = header.explorerElement;
                triggerExplorer.NavigateToExplorerElement(explorerElement);
            }
        }

        private void tabitem_Menu_OpenInExplorer_Click(object sender, RoutedEventArgs e)
        {
            if (tabItem_rightClicked != null)
            {
                var header = tabItem_rightClicked.Header as TabItemBT;
                var explorerElement = header.explorerElement;
                FileSystemUtil.OpenInExplorer(explorerElement.GetPath());
            }
        }

        private void menuItemCheckVersion_Click(object sender, RoutedEventArgs e)
        {
            var window = new NewVersionWindow();
            window.ShowDialog();
        }

        private void menuItemReportIssue_Click(object sender, RoutedEventArgs e)
        {
            var window = new ReportIssueWindow();
            window.ShowDialog();
        }
    }
}
