using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.IO;
using GUI.Components.Shared;
using System.Windows.Controls;
using BetterTriggers;

namespace GUI
{
    public partial class OpenWar3MapWindow : Window
    {
        public string SelectedPath;
        public bool OK;
        private Stack<ExplorerBackForth> Back = new Stack<ExplorerBackForth>();
        private Stack<ExplorerBackForth> Forward = new Stack<ExplorerBackForth>();
        private string currentDir;

        class ExplorerBackForth
        {
            public string BackDir;
            public string ForwardDir;
            public ExplorerBackForth(string backDir, string forwardDir)
            {
                this.BackDir = backDir;
                this.ForwardDir = forwardDir;
            }
        }


        public OpenWar3MapWindow()
        {
            InitializeComponent();
            Settings settings = Settings.Load();
            this.Width = settings.selectMapWindowWidth;
            this.Height = settings.selectMapWindowHeight;
            this.Left = settings.selectMapWindowX;
            this.Top = settings.selectMapWindowY;

            foreach (var drive in DriveInfo.GetDrives())
            {
                TreeItemHeader header = new TreeItemHeader(drive.Name, TriggerCategory.TC_DRIVE);
                ListViewItem listItem = new ListViewItem();
                listItem.Content = header;
                listItem.Selected += delegate
                {
                    GoToDirectory(drive.RootDirectory.FullName);
                };
                listViewDrives.Items.Add(listItem);
            }

            string path = settings.lastOpenedFileLocation;
            if (!Directory.Exists(path))
                path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);


            RefreshFileList(path);
        }

        private void GoBack()
        {
            if (Back.Count > 0)
            {
                btnForward.IsEnabled = true;
                var backForth = Back.Pop();
                Forward.Push(backForth);
                RefreshFileList(backForth.BackDir);
                CanLevelUp();
                for (int i = 0; i < treeViewFiles.Items.Count; i++)
                {
                    TreeViewItem treeItem = (TreeViewItem)treeViewFiles.Items[i];
                    ListItemData data = (ListItemData)treeItem.Tag;
                    if (data.path == backForth.ForwardDir)
                    {
                        treeItem.IsSelected = true;
                        treeItem.Focus();
                    }
                }
            }
            if (Back.Count == 0)
                btnBack.IsEnabled = false;
        }

        private void GoForward()
        {
            if (Forward.Count > 0)
            {
                btnBack.IsEnabled = true;
                var backForth = Forward.Pop();
                Back.Push(backForth);
                RefreshFileList(backForth.ForwardDir);
                CanLevelUp();
            }
            if (Forward.Count == 0)
                btnForward.IsEnabled = false;
        }

        private void OneLevelUp()
        {
            string upDir = Path.GetDirectoryName(currentDir);
            var backForth = new ExplorerBackForth(currentDir, upDir);
            Back.Push(backForth);
            Forward.Clear();
            btnBack.IsEnabled = true;
            btnForward.IsEnabled = false;
            RefreshFileList(upDir);
        }

        private void CanLevelUp()
        {
            if (Path.GetDirectoryName(currentDir) == null)
                btnLevelUp.IsEnabled = false;
            else
                btnLevelUp.IsEnabled = true;
        }

        private void OnOk()
        {
            Settings settings = Settings.Load();
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
            try
            {
                var options = new EnumerationOptions();
                options.IgnoreInaccessible = true;
                string[] entries = Directory.GetFileSystemEntries(dir, "*", options);
                treeViewFiles.Items.Clear();
                currentDir = dir;
                textBox.Text = dir;
                CanLevelUp();
                for (int i = 0; i < entries.Length; i++)
                {
                    var entry = entries[i];
                    string name = Path.GetFileName(entry);
                    string category = TriggerCategory.TC_DIRECTORY;
                    string ext = Path.GetExtension(entry);
                    bool isMap = ext == ".w3x" || ext == ".w3m";
                    if (isMap)
                        category = TriggerCategory.TC_MAP;
                    if (File.Exists(entry))
                    {
                        if (!isMap)
                            continue;
                    }

                    TreeItemHeader header = new TreeItemHeader(name, category);
                    TreeViewItem treeItem = new TreeViewItem();
                    ListItemData listItemData = new ListItemData(entry, isMap);
                    treeItem.Tag = listItemData;
                    treeItem.Header = header;
                    treeItem.MouseDoubleClick += TreeItem_MouseDoubleClick;
                    treeItem.KeyDown += TreeItem_KeyDown;
                    treeViewFiles.Items.Add(treeItem);
                }
            }
            catch (Exception ex)
            {
                MessageBox dialog = new MessageBox("Error", ex.Message);
                dialog.ShowDialog();
            }

        }

        private void TreeItem_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            TreeViewItem treeItem = (TreeViewItem)sender;
            ListItemData data = (ListItemData)treeItem.Tag;
            if (data.isMap)
                OnOk();
            else
                GoToDirectory(data.path);
        }

        private void TreeItem_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key != System.Windows.Input.Key.Enter)
                return;

            TreeViewItem treeItem = (TreeViewItem)sender;
            ListItemData data = (ListItemData)treeItem.Tag;
            GoToDirectory(data.path);
        }

        private void GoToDirectory(string dir)
        {
            if (!Directory.Exists(dir))
            {
                MessageBox messageBox = new MessageBox("Error", "Directory does not exist.");
                messageBox.ShowDialog();
                return;
            }

            string previousDir = currentDir;
            var backForth = new ExplorerBackForth(previousDir, dir);
            Back.Push(backForth);
            Forward.Clear();
            btnBack.IsEnabled = true;
            btnForward.IsEnabled = false;
            RefreshFileList(dir);
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            GoBack();
        }

        private void btnForward_Click(object sender, RoutedEventArgs e)
        {
            GoForward();
        }

        private void btnLevelUp_Click(object sender, RoutedEventArgs e)
        {
            OneLevelUp();
        }



        private void textBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter && textBox.Text != currentDir)
                GoToDirectory(textBox.Text);
        }

        private void Window_LocationChanged(object sender, EventArgs e)
        {
            Settings settings = Settings.Load();
            settings.selectMapWindowX = (int)this.Left;
            settings.selectMapWindowY = (int)this.Top;
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Settings settings = Settings.Load();
            settings.selectMapWindowWidth = (int)this.Width;
            settings.selectMapWindowHeight = (int)this.Height;
        }

        private void treeViewFiles_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            TreeViewItem treeItem = treeViewFiles.SelectedItem as TreeViewItem;
            if (treeItem == null)
            {
                btnOK.IsEnabled = false;
                return;
            }

            ListItemData data = (ListItemData)treeItem.Tag;
            SelectedPath = data.path;
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
    }
}
