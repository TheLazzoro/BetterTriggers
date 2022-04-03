using GUI.Components.TriggerEditor.ParameterControls;
using Model.SaveableData;
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
using System.Windows.Shapes;

namespace GUI
{
    /// <summary>
    /// Interaction logic for ParameterWindow.xaml
    /// </summary>
    public partial class ParameterWindow : Window
    {
        public bool isOK = false;
        public Parameter selectedParameter;
        ParameterFunctionControl functionControl;
        ParameterConstantControl constantControl;
        ParameterVariableControl variableControl;
        ParameterValueControl valueControl;

        public ParameterWindow(string returnType)
        {
            InitializeComponent();

            this.functionControl = new ParameterFunctionControl(returnType);
            grid.Children.Add(functionControl);
            Grid.SetRow(functionControl, 1);
            functionControl.Visibility = Visibility.Hidden;

            this.variableControl = new ParameterVariableControl(returnType);
            grid.Children.Add(variableControl);
            Grid.SetRow(variableControl, 1);
            variableControl.Visibility = Visibility.Hidden;

            this.valueControl = new ParameterValueControl(returnType);
            grid.Children.Add(valueControl);
            Grid.SetRow(valueControl, 1);
            valueControl.Visibility = Visibility.Hidden;

            this.constantControl = new ParameterConstantControl(returnType);
            grid.Children.Add(constantControl);
            Grid.SetRow(constantControl, 1);
            constantControl.Visibility = Visibility.Visible;
            radioBtnPreset.IsChecked = true;


            this.functionControl.listViewFunctions.SelectionChanged += SelectionChanged;
            this.constantControl.listViewConstant.SelectionChanged += SelectionChanged;
            this.variableControl.listViewVariables.SelectionChanged += SelectionChanged;
        }

        private void SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var listView = (ListView)sender;
            var grid = (Grid) listView.Parent; // parent of listview
            var control = (IParameterControl)grid.Parent; // parent of grid
            // I consider this a 'working' hack for now

            this.selectedParameter = control.GetSelectedItem();
        }

        private void ShowHideTabs(IParameterControl control)
        {
            functionControl.Visibility = Visibility.Hidden;
            constantControl.Visibility = Visibility.Hidden;
            variableControl.Visibility = Visibility.Hidden;
            valueControl.Visibility = Visibility.Hidden;

            control.SetVisibility(Visibility.Visible);
            if (control.GetElementCount() > 0)
            {
                this.selectedParameter = control.GetSelectedItem();
                btnOK.IsEnabled = true;
            }
            else
                btnOK.IsEnabled = false;
        }

        private void radioBtnFunction_Checked(object sender, RoutedEventArgs e)
        {
            ShowHideTabs(functionControl);
        }

        private void radioBtnPreset_Checked(object sender, RoutedEventArgs e)
        {
            ShowHideTabs(constantControl);
        }

        private void radioBtnVariable_Checked(object sender, RoutedEventArgs e)
        {
            ShowHideTabs(variableControl);
        }

        private void radioBtnValue_Checked(object sender, RoutedEventArgs e)
        {
            ShowHideTabs(valueControl);
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            this.isOK = true;
            this.Close();
        }
    }
}
