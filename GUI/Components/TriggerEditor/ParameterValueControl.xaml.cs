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
using GUI.Controllers;
using Model;
using Model.SaveableData;
using Model.Templates;

namespace GUI
{
    /// <summary>
    /// Interaction logic for ParameterFunctionControl.xaml
    /// </summary>
    public partial class ParameterValueControl : UserControl, IParameterControl
    {
        private ListViewItem selectedItem;

        public ParameterValueControl(string returnType)
        {
            InitializeComponent();
        }

        public int GetElementCount()
        {
            throw new NotImplementedException();
        }

        public Parameter GetSelectedItem()
        {
            throw new NotImplementedException();
        }

        public void SetVisibility(Visibility visibility)
        {
            this.Visibility = visibility;
        }
    }
}
