using System;
using System.Collections.Generic;
using System.IO;
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
using GUI.Containers;
using Model;
using Model.Data;
using Model.SavableTriggerData;
using Newtonsoft.Json;

namespace GUI
{
    /// <summary>
    /// Interaction logic for ParameterFunctionControl.xaml
    /// </summary>
    public partial class ParameterVariableControl : UserControl, IParameterControl
    {
        private ListViewItem selectedItem;

        public ParameterVariableControl(string returnType)
        {
            InitializeComponent();

            List<Model.Data.Variable> variables = new List<Model.Data.Variable>();

            for (int i = 0; i < ContainerVariables.Count(); i++)
            {
                var explorerElement = ContainerVariables.Get(i);
                string json = File.ReadAllText(explorerElement.FilePath);
                Model.Data.Variable variable = JsonConvert.DeserializeObject<Model.Data.Variable>(json);
                variable.Name = System.IO.Path.GetFileNameWithoutExtension(explorerElement.FilePath); // hack

                if (variable.Type == returnType)
                    variables.Add(variable);
            }

            // Create a list of saveable variables
            for (int i = 0; i < variables.Count; i++)
            {
                ListViewItem item = new ListViewItem();
                item.Content = variables[i].Name;
                item.Tag = new Model.SavableTriggerData.Variable()
                {
                    returnType = variables[i].Type,
                    VariableId = variables[i].Id,
                };

                listViewVariables.Items.Add(item);
                this.selectedItem = listViewVariables.Items.GetItemAt(0) as ListViewItem;
            }
        }

        public int GetElementCount()
        {
            return listViewVariables.Items.Count;
        }

        public Parameter GetSelectedItem()
        {
            var variables = (Model.SavableTriggerData.Variable)selectedItem.Tag;
            return variables;
        }

        public void SetVisibility(Visibility visibility)
        {
            this.Visibility = visibility;
        }

        private void listViewFunction_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selectedItem = listViewVariables.SelectedItem as ListViewItem;
        }
    }
}
