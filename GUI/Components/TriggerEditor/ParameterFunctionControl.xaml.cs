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
using Facades.Controllers;
using GUI.Controllers;
using Model;
using Model.SaveableData;
using Model.Templates;

namespace GUI
{
    /// <summary>
    /// Interaction logic for ParameterFunctionControl.xaml
    /// </summary>
    public partial class ParameterFunctionControl : UserControl, IParameterControl
    {
        private ListViewItem selectedItem;

        public ParameterFunctionControl(string returnType)
        {
            InitializeComponent();

            ControllerTriggerData controller = new ControllerTriggerData();
            List<FunctionTemplate> functions = controller.LoadAllFunctions(@"C:\Users\Lasse Dam\Desktop\JSON\calls.json");

            for (int i = 0; i < functions.Count; i++)
            {
                if (functions[i].returnType == returnType)
                {
                    Function function = new Function()
                    {
                        identifier = functions[i].identifier,
                        parameters = new List<Parameter>(),
                        returnType = functions[i].returnType,
                    };
                    ListViewItem item = new ListViewItem();
                    item.Content = functions[i].name;
                    item.Tag = functions[i];

                    listViewFunctions.Items.Add(item);
                    this.selectedItem = listViewFunctions.Items.GetItemAt(0) as ListViewItem;
                }
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
    }
}
