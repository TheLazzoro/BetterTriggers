using BetterTriggers.Controllers;
using BetterTriggers.Utility;
using Model.EditorData;
using Model.SaveableData;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace GUI.Components.TriggerEditor.ParameterControls
{
    public partial class ParameterImportedControl : UserControl, IParameterControl
    {
        private ListViewItem selectedItem;

        public ParameterImportedControl(string returnType)
        {
            InitializeComponent();

            var imports = ControllerImports.GetImportsAll(returnType);
            List<Searchable> objects = new List<Searchable>();

            for (int i = 0; i < imports.Count; i++)
            {
                ListViewItem listItem = new ListViewItem();
                listItem.Content = imports[i].identifier;
                listItem.Tag = imports[i];

                objects.Add(new Searchable()
                {
                    Object = listItem,
                    Words = new List<string>()
                    {
                        imports[i].identifier.ToLower()
                    },
                });
                var searchables = new Searchables(objects);
                listControl.SetSearchableList(searchables);

                listControl.listView.SelectionChanged += ListView_SelectionChanged;
            }
        }

        public int GetElementCount()
        {
            return listControl.listView.Items.Count;
        }

        public Parameter GetSelectedItem()
        {
            var variables = (Value)selectedItem.Tag;
            return variables;
        }

        public void SetVisibility(Visibility visibility)
        {
            this.Visibility = visibility;
        }

        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selectedItem = listControl.listView.SelectedItem as ListViewItem;
        }
    }
}
