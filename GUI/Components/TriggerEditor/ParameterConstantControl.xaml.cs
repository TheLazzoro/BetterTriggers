using BetterTriggers.Containers;
using GUI.Controllers;
using Model.SaveableData;
using Model.Templates;
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

namespace GUI
{
    /// <summary>
    /// Interaction logic for ParameterFunctionControl.xaml
    /// </summary>
    public partial class ParameterConstantControl : UserControl, IParameterControl
    {
        private ListViewItem selectedItem;

        public ParameterConstantControl(string returnType)
        {
            InitializeComponent();

            for (int i = 0; i < ContainerTriggerData.ConstantTemplates.Count; i++)
            {
                var constant = ContainerTriggerData.ConstantTemplates[i];
                if (constant.returnType == returnType)
                {
                    ListViewItem item = new ListViewItem();
                    item.Content = constant.name;
                    item.Tag = constant;

                    listViewConstant.Items.Add(item);
                    this.selectedItem = listViewConstant.Items.GetItemAt(0) as ListViewItem;
                }
            }
        }

        public int GetElementCount()
        {
            return listViewConstant.Items.Count;
        }

        public Parameter GetSelectedItem()
        {
            var template = (ConstantTemplate)selectedItem.Tag;
            var parameter = new Constant()
            {
                identifier = template.identifier,
                returnType = template.returnType,
            };
            return parameter;
        }

        public void SetVisibility(Visibility visibility)
        {
            this.Visibility = visibility;
        }

        private void listViewConstant_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selectedItem = listViewConstant.SelectedItem as ListViewItem;
        }
    }
}
