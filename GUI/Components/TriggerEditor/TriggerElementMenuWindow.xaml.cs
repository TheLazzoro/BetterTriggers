using BetterTriggers;
using BetterTriggers.Controllers;
using BetterTriggers.Models.EditorData;
using BetterTriggers.Models.SaveableData;
using BetterTriggers.Models.Templates;
using BetterTriggers.Utility;
using GUI.Components.Shared;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace GUI.Components.TriggerEditor
{
    public partial class TriggerElementMenuWindow : Window
    {
        public TriggerElement createdTriggerElement;
        TriggerElementType triggerElementType;
        TriggerElement selected;
        TriggerElement previous;

        ListViewItem defaultSelected;

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
                Category category = Category.Get(templates[i].category);
                string categoryStr = Locale.Translate(category.Name);
                if(categoryStr != "")
                    categoryStr += " - ";
                string name = templates[i].name != "" ? templates[i].name : templates[i].identifier;
                string content = categoryStr + name;
                ListViewItem listItem = new ListViewItem();
                listItem.Content = content;
                listItem.Tag = templates[i].ToTriggerElement();
                objects.Add(new Searchable()
                {
                    Object = listItem,
                    Category = Locale.Translate(category.Name),
                    Words = new List<string>()
                    {
                        content.ToLower(),
                        templates[i].identifier.ToLower()
                    },
                });

                // default selection
                if (previous != null && previous.function.identifier == templates[i].identifier)
                {
                    defaultSelected = listItem;
                    selected = (TriggerElement)listItem.Tag;
                    listItem.IsSelected = true;
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
                if (item == null)
                    return;

                selected = (TriggerElement)item.Tag;
                textBoxDescription.Text = Locale.Translate(selected.function.identifier);
            };

            if (selected == null)
            {
                defaultSelected = (ListViewItem)listControl.listView.Items[0];
                selected = (TriggerElement)defaultSelected.Tag;
            }

            listControl.listView.ScrollIntoView(defaultSelected);
            defaultSelected.Focus();

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

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && selected != null)
            {
                createdTriggerElement = selected;
                this.Close();
            }
        }
    }
}
