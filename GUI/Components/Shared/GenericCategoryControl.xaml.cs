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
using System.Windows.Navigation;
using System.Windows.Shapes;
using BetterTriggers.Utility;

namespace GUI.Components.Shared
{
    /// <summary>
    /// Interaction logic for GenericCategoryControl.xaml
    /// </summary>
    public partial class GenericCategoryControl : UserControl, ISearchablesObserverCategories
    {
        Searchables searchObjects;
        List<ListViewItem> listItems = new List<ListViewItem>();
        ListViewItem categoryAll;
        ListViewItem selected;
        bool doListenSelect = true;

        public GenericCategoryControl(Searchables searchObjects)
        {
            InitializeComponent();
            this.searchObjects = searchObjects;
            searchObjects.AttachCategories(this);

            var categories = searchObjects.GetCurrentCategories();

            categoryAll = new ListViewItem();
            categoryAll.Content = "- All";

            listViewCategory.Items.Add(categoryAll);
            for (int i = 0; i < categories.Count; i++)
            {
                ListViewItem item = new ListViewItem();
                item.Content = categories[i];
                item.Tag = categories[i];

                listItems.Add(item);
                listViewCategory.Items.Add(item);
            }

            selected = categoryAll;
        }

        public void Update()
        {
            doListenSelect = false;
            listViewCategory.Items.Clear();
            listViewCategory.Items.Add(categoryAll); // hack
            var categories = searchObjects.GetCurrentCategories();
            for (int i = 0; i < listItems.Count; i++)
            {
                if (categories.Contains((string)listItems[i].Tag))
                    listViewCategory.Items.Add(listItems[i]);
            }

            selected.IsSelected = true;
            doListenSelect = true;

            if (!listViewCategory.Items.Contains(selected))
            {
                selected = categoryAll;
                selected.IsSelected = true;
            }
        }

        private void listViewCategory_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(!doListenSelect)
                return;

            selected = listViewCategory.SelectedItem as ListViewItem;
            string categoryId;
            if (selected == null || selected == categoryAll)
                categoryId = "-1";
            else
                categoryId = (string)selected.Tag;

            searchObjects.SetCurrentCategory(categoryId);
        }
    }
}
