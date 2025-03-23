using BetterTriggers;
using BetterTriggers.Containers;
using BetterTriggers.Models.EditorData;
using GUI.Components.Shared;
using GUI.Extensions;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;

namespace GUI.Components.OpenMap
{
    public partial class OpenWar3MapWindow : Window
    {
        public string SelectedPath;
        public bool OK;
        private string currentDir;
        private bool useRelativeMapDirectory;

        private OpenWar3MapViewModel _viewModel;

        public OpenWar3MapWindow()
        {
            InitializeComponent();

            _viewModel = new OpenWar3MapViewModel();
            DataContext = _viewModel;

            EditorSettings settings = EditorSettings.Load();
            this.Width = settings.selectMapWindowWidth;
            this.Height = settings.selectMapWindowHeight;
            this.Left = settings.selectMapWindowX;
            this.Top = settings.selectMapWindowY;
            this.ResetPositionWhenOutOfScreenBounds();

            string path;
            var project = Project.CurrentProject.war3project;
            useRelativeMapDirectory = project.UseRelativeMapDirectory;
            if (useRelativeMapDirectory)
            {
                var root = Project.CurrentProject.projectFiles[0];
                string rootDir = Path.GetDirectoryName(root.GetPath());
                path = Path.Combine(rootDir, "map");
                btnBrowseFiles.Visibility = Visibility.Hidden;
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
            }
            else
            {
                path = settings.lastOpenedFileLocation;
                if (!Directory.Exists(path))
                    path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            }


            RefreshFileList(path);
        }

        private void OnOk()
        {
            EditorSettings settings = EditorSettings.Load();
            settings.lastOpenedFileLocation = currentDir;
            OK = true;
            this.Close();
        }


        class ListItemData
        {
            public string path;
            public bool isMap;
            public ListItemData(string path, bool isMap)
            {
                this.path = path;
                this.isMap = isMap;
            }
        }

        private void RefreshFileList(string dir)
        {
            btnOK.IsEnabled = false;
            try
            {
                var options = new EnumerationOptions();
                options.IgnoreInaccessible = true;
                string[] entries = Directory.GetFileSystemEntries(dir, "*", options);
                _viewModel.Maps.Clear();
                currentDir = dir;
                textBox.Text = dir;
                for (int i = 0; i < entries.Length; i++)
                {
                    var entry = entries[i];
                    string ext = Path.GetExtension(entry);
                    bool isMap = ext == ".w3x" || ext == ".w3m";
                    if (!isMap)
                    {
                        continue;
                    }

                    _viewModel.Maps.Add(new MapFile(entry));
                }

                txtNoMapsFound.Visibility = _viewModel.Maps.Count == 0 ? Visibility.Visible : Visibility.Hidden;
                lblFound.Content = "Maps found: " + _viewModel.Maps.Count;
            }
            catch (Exception ex)
            {
                Dialogs.MessageBox dialog = new Dialogs.MessageBox("Error", ex.Message);
                dialog.ShowDialog();
            }

        }

        private void Window_LocationChanged(object sender, EventArgs e)
        {
            EditorSettings settings = EditorSettings.Load();
            settings.selectMapWindowX = (int)this.Left;
            settings.selectMapWindowY = (int)this.Top;
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            EditorSettings settings = EditorSettings.Load();
            settings.selectMapWindowWidth = (int)this.Width;
            settings.selectMapWindowHeight = (int)this.Height;
        }

        private void treeViewFiles_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            var treeItem = treeViewFiles.SelectedItem as MapFile;
            if (treeItem == null)
            {
                btnOK.IsEnabled = false;
                return;
            }

            SelectedPath = treeItem.FullPath;
            if(!Project.VerifyMapPath(SelectedPath))
            {
                btnOK.IsEnabled = false;
                return;
            }
            if(useRelativeMapDirectory)
            {
                SelectedPath = Path.GetFileName(treeItem.FullPath);
            }

            btnOK.IsEnabled = true;
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            OnOk();
        }

        private void Window_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Escape)
            {
                this.Close();
                e.Handled = true;
            }
        }

        private void btnBrowseFiles_Click(object sender, RoutedEventArgs e)
        {
            using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
            {
                System.Windows.Forms.DialogResult result = dialog.ShowDialog();
                if (dialog.SelectedPath != "")
                {
                    RefreshFileList(dialog.SelectedPath);
                }
            }
        }
    }
}
