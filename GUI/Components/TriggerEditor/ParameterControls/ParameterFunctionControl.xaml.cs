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
using BetterTriggers.Controllers;
using BetterTriggers.Utility;
using GUI.Controllers;
using Model;
using Model.SaveableData;
using Model.Templates;

namespace GUI.Components.TriggerEditor.ParameterControls
{
    /// <summary>
    /// Interaction logic for ParameterFunctionControl.xaml
    /// </summary>
    public partial class ParameterFunctionControl : UserControl, IParameterControl
    {
        private ListViewItem selectedItem;
        private SearchObjects searchObjects;

        public ParameterFunctionControl(string returnType)
        {
            InitializeComponent();

            ControllerTriggerData controller = new ControllerTriggerData();
            List<FunctionTemplate> functions = controller.LoadAllCalls();

            List<SearchObject> objects = new List<SearchObject>();
            for (int i = 0; i < functions.Count; i++)
            {
                if (functions[i].returnType != returnType)
                    continue;

                ListViewItem listItem = new ListViewItem();
                listItem.Content = functions[i].name;
                listItem.Tag = functions[i];
                objects.Add(new SearchObject()
                {
                    Object = listItem,
                    Words = new List<string>()
                    {
                        functions[i].name.ToLower(),
                        functions[i].identifier.ToLower(),
                        string.Empty
                    },
                });
            }
            searchObjects = new SearchObjects(objects);
            PopulateList("");
        }

        public void PopulateList(string searchWord)
        {
            listViewFunctions.Items.Clear();
            var list = searchObjects.Search(searchWord);
            for (int i = 0; i < list.Count; i++)
            {
                listViewFunctions.Items.Add(list[i].Object);
            }
        }

        public int GetElementCount()
        {
            return listViewFunctions.Items.Count;
        }

        public Parameter GetSelectedItem()
        {
            var template = (FunctionTemplate)selectedItem.Tag;
            var parameter = new Function()
            {
                identifier = template.identifier,
                parameters = template.parameters,
                returnType = template.returnType,
            };
            return parameter;
        }

        public void SetVisibility(Visibility visibility)
        {
            this.Visibility = visibility;
        }

        private void listViewFunction_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selectedItem = listViewFunctions.SelectedItem as ListViewItem;
        }

        private void textBoxSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            PopulateList(textBoxSearch.Text);
        }
    }
}
