using BetterTriggers.Utility;
using System;
using System.Windows.Controls;

namespace GUI.Components.Shared
{
    public partial class SearchableListControl : UserControl, ISearchablesObserverList
    {
        public EventHandler ListViewChanged;
        private Searchables searchObjects;

       
        public SearchableListControl()
        {
            InitializeComponent();
        }

        public void SetSearchableList(Searchables searchObjects)
        {
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
    }
}
