using BetterTriggers;
using BetterTriggers.Containers;
using BetterTriggers.Models.EditorData;
using BetterTriggers.Models.EditorData.TriggerEditor;
using BetterTriggers.Models.SaveableData;
using BetterTriggers.Models.Templates;
using BetterTriggers.Utility;
using BetterTriggers.WorldEdit;
using GUI.Components.Shared;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GUI.Components.TriggerEditor.ParameterControls
{
    public partial class ParameterParameterDefinitionControl : UserControl, IParameterControl
    {
        private ListViewItem selectedItem;

        public ParameterParameterDefinitionControl(string returnType, ParameterDefinitionCollection? parameters = null)
        {
            InitializeComponent();

            List<Searchable> objects = new List<Searchable>();
            if (parameters != null)
            {
                for (int i = 0; i < parameters.Count(); i++)
                {
                    var parameterDef = (ParameterDefinition)parameters.Elements[i];
                    if (parameterDef.ReturnType.Type != returnType)
                        continue;

                    ListViewItem listItem = new ListViewItem();
                    listItem.Content = parameterDef.Name;
                    listItem.Tag = parameterDef;
                    objects.Add(new Searchable()
                    {
                        Object = listItem,
                        Words = new List<string>()
                    {
                        parameterDef.Name.ToLower(),
                    },
                    });
                }
            }

            var searchables = new Searchables(objects);
            listControl.SetSearchableList(searchables);

            var categoryControl = new GenericCategoryControl(searchables);
            grid.Children.Add(categoryControl);
            Grid.SetRow(categoryControl, 1);
            Grid.SetRowSpan(categoryControl, 3);

            listControl.listView.SelectionChanged += ListView_SelectionChanged;
        }

        public void SetDefaultSelection(Parameter parameter)
        {
            var parameterDefRef = (ParameterDefinitionRef)parameter;
            int i = 0;
            bool found = false;
            while (!found && i < listControl.listView.Items.Count)
            {
                var item = listControl.listView.Items[i] as ListViewItem;
                var constant = item.Tag as ParameterDefinition;
                if (constant.Id == parameterDefRef.ParameterDefinitionId)
                    found = true;
                else
                    i++;
            }
            if (found == false)
                return;

            var defaultSelected = listControl.listView.Items[i] as ListViewItem;
            defaultSelected.IsSelected = true;
            listControl.listView.ScrollIntoView(defaultSelected);
        }

        public int GetElementCount()
        {
            return listControl.listView.Items.Count;
        }

        public Parameter GetSelectedItem()
        {
            if (selectedItem == null)
                return null;

            var parameterDef = (ParameterDefinition)selectedItem.Tag;
            var parameterDefRef = new ParameterDefinitionRef()
            {
                ParameterDefinitionId = parameterDef.Id,
            };
            return parameterDefRef;
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
