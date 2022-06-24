using BetterTriggers.Controllers;
using BetterTriggers.Utility;
using GUI.Components.Shared;
using Model.SaveableData;
using Model.Templates;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace GUI.Components.TriggerEditor
{
    public partial class TriggerElementMenuWindow : Window
    {
        public TriggerElement createdTriggerElement;
        TriggerElementType triggerElementType;
        TriggerElement selected;
        TriggerElement previous;

        public TriggerElementMenuWindow(TriggerElementType triggerElementType, TriggerElement previous = null)
        {
            InitializeComponent();
            this.Owner = MainWindow.GetMainWindow();

            this.triggerElementType = triggerElementType;
            this.previous = previous;

            Window parentWindow = Application.Current.MainWindow;
            this.Top = parentWindow.Top + parentWindow.Height / 2 - this.Height / 2;
            this.Left = parentWindow.Left + parentWindow.Width / 2 - this.Width / 2;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Window parentWindow = Application.Current.MainWindow;
            this.Top = parentWindow.Top + parentWindow.Height / 2 - this.Height / 2;
            this.Left = parentWindow.Left + parentWindow.Width / 2 - this.Width / 2;

            var controllerTriggerData = new ControllerTriggerData();
            var templates = new List<FunctionTemplate>();
            if (triggerElementType == TriggerElementType.Event)
            {
                templates = controllerTriggerData.LoadAllEvents();
            }
            else if (triggerElementType == TriggerElementType.Condition)
            {
                templates = controllerTriggerData.LoadAllConditions();
            }
            else if (triggerElementType == TriggerElementType.Action)
            {
                templates = controllerTriggerData.LoadAllActions();
            }

            List<Searchable> objects = new List<Searchable>();
            for (int i = 0; i < templates.Count; i++)
            {
                ListViewItem listItem = new ListViewItem();
                listItem.Content = templates[i].name;
                listItem.Tag = templates[i].ToTriggerElement();
                objects.Add(new Searchable()
                {
                    Object = listItem,
                    Category = templates[i].category.ToString(),
                    Words = new List<string>()
                    {
                        templates[i].name.ToLower(),
                        templates[i].identifier.ToLower()
                    },
                });

                // default selection
                if (previous != null && previous.function.identifier == templates[i].identifier)
                {
                    selected = (TriggerElement)listItem.Tag;
                    listItem.IsSelected = true;
                    listControl.listView.ScrollIntoView(listItem);
                }
            }


            var searchables = new Searchables(objects);
            listControl.SetSearchableList(searchables);
            listControl.ListViewChanged += delegate
            {
                btnOK.IsEnabled = selected != null && listControl.GetItemsCount() > 0;
            };
            listControl.listView.SelectionChanged += delegate
            {
                ListViewItem item = (ListViewItem)listControl.listView.SelectedItem;
                selected = (TriggerElement)item.Tag;
            };

            if (selected == null)
            {
                ListViewItem item = (ListViewItem)listControl.listView.Items[0];
                selected = (TriggerElement)item.Tag;
            }

            var categoryControl = new GenericCategoryControl(searchables);
            categoryControl.Margin = new Thickness(0, 0, 4, 0);
            grid.Children.Add(categoryControl);
            Grid.SetRow(categoryControl, 1);
            Grid.SetRowSpan(categoryControl, 3);
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            createdTriggerElement = selected;
            this.Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
