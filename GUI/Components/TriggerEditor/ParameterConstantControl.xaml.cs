﻿using DataAccess.Natives;
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

            List<Constant> constants = DataAccess.LoadData.LoadAllConstants(@"C:\Users\Lasse Dam\Desktop\JSON\constants.json");

            for (int i = 0; i < constants.Count; i++)
            {
                if(constants[i].returnType.type == returnType)
                {
                    ListViewItem item = new ListViewItem();
                    item.Content = constants[i].name;
                    item.Tag = constants[i];

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
            var parameter = (Constant) selectedItem.Tag;
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