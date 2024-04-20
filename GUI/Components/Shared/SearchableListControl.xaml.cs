using BetterTriggers;
using BetterTriggers.Utility;
using GUI.Components.TriggerEditor;
using System;
using System.Windows;
using System.Windows.Controls;

namespace GUI.Components.Shared
{
    public partial class SearchableListControl : UserControl, ISearchablesObserverList
    {
        public EventHandler ListViewChanged;
        public event Action ShowIconsChanged;
        private Searchables searchObjects;

        public SearchableListControl()
        {
            InitializeComponent();

            EditorSettings settings = EditorSettings.Load();
            checkBoxShowIcons.IsChecked = settings.GUINewElementIcon;
            checkBoxShowIcons.Click += checkBoxShowIcons_Click;
            checkBoxShowIcons.IsVisibleChanged += CheckBoxShowIcons_IsVisibleChanged;
        }

        public void SetSearchableList(Searchables searchObjects)
        {
            this.listView.Items.Clear();
            this.searchObjects = searchObjects;
            searchObjects.AttachList(this);
            Search("");
        }

        public void Search(string searchWord)
        {
            searchObjects.Search(searchWord);
        }

        public int GetItemsCount()
        {
            return listView.Items.Count;
        }

        private void textBoxSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            Search(textBoxSearch.Text);
            InvokeListViewChanged(sender, e);
        }

        private void InvokeListViewChanged(object sender, EventArgs e)
        {
            // bubble up event
            if (ListViewChanged != null)
                ListViewChanged(this, e);
        }

        public void Update()
        {
            listView.Items.Clear();
            var objects = searchObjects.GetObjects();
            for (int i = 0; i < objects.Count; i++)
            {
                listView.Items.Add(objects[i].Object);
            }
        }

        private void checkBoxShowIcons_Click(object sender, RoutedEventArgs e)
        {
            EditorSettings settings = EditorSettings.Load();
            settings.GUINewElementIcon = (bool)checkBoxShowIcons.IsChecked;
            ShowIconsChanged?.Invoke();
            var items = searchObjects.GetAllObject();
            foreach (var item in items)
            {
                var function = item.Object as ListItemFunctionTemplate;
                function.IsIconVisible = settings.GUINewElementIcon ? Visibility.Visible : Visibility.Hidden;
            }
        }

        private void CheckBoxShowIcons_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (checkBoxShowIcons.IsVisible)
                Grid.SetColumnSpan(textBoxSearch, 1);
            else
                Grid.SetColumnSpan(textBoxSearch, 2);
        }

    }
}
