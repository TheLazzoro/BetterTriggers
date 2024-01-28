using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.IO;
using GUI.Components.Shared;
using System.Windows.Controls;
using BetterTriggers;
using System.Runtime.InteropServices;
using GUI.Utility;
using BetterTriggers.Utility;
using System.Windows.Media;
using BetterTriggers.Containers;

namespace GUI.Components.OpenMap
{
    public partial class OpenWar3MapWindowHotfix : Window
    {
        public string SelectedPath;
        public bool OK;
        private string currentDir;

        public OpenWar3MapWindowHotfix()
        {
            InitializeComponent();
            EditorSettings settings = EditorSettings.Load();
            this.Width = settings.selectMapWindowWidth;
            this.Height = settings.selectMapWindowHeight;
            this.Left = settings.selectMapWindowX;
            this.Top = settings.selectMapWindowY;

            string path = settings.lastOpenedFileLocation;
            if (!Directory.Exists(path))
                path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);


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
                treeViewFiles.Items.Clear();
                currentDir = dir;
                textBox.Text = dir;
                for (int i = 0; i < entries.Length; i++)
                {
                    var entry = entries[i];
                    string name = Path.GetFileName(entry);
                    string category = TriggerCategory.TC_MAP;
                    string ext = Path.GetExtension(entry);
                    bool isMap = ext == ".w3x" || ext == ".w3m";
                    if (!isMap)
                    {
                        continue;
                    }

                    TreeItemHeader header = new TreeItemHeader(name, category);
                    TreeViewItem treeItem = new TreeViewItem();
                    ListItemData listItemData = new ListItemData(entry, isMap);
                    treeItem.Tag = listItemData;
                    treeItem.Header = header;
                    treeViewFiles.Items.Add(treeItem);
                }

                txtNoMapsFound.Visibility = treeViewFiles.Items.Count == 0 ? Visibility.Visible : Visibility.Hidden;
                lblFound.Content = "Maps found: " + treeViewFiles.Items.Count;
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
            TreeViewItem treeItem = treeViewFiles.SelectedItem as TreeViewItem;
            if (treeItem == null)
            {
                btnOK.IsEnabled = false;
                return;
            }

            ListItemData data = (ListItemData)treeItem.Tag;
            SelectedPath = data.path;
            if (!Project.VerifyMapPath(SelectedPath))
            {
                btnOK.IsEnabled = false;
                return;
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