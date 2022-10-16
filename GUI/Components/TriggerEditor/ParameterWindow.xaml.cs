using BetterTriggers;
using BetterTriggers.Containers;
using BetterTriggers.Controllers;
using BetterTriggers.Models.SaveableData;
using GUI.Components.TriggerEditor.ParameterControls;
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
    public partial class ParameterWindow : Window
    {
        public bool isOK = false;
        public Parameter selectedParameter;

        ParameterFunctionControl functionControl;
        ParameterConstantControl constantControl;
        ParameterVariableControl variableControl;
        ParameterValueControl valueControl;
        ParameterTriggerControl triggerRefControl;
        ParameterImportedControl importControl;
        IParameterControl selectedControl;


        /// <summary>
        /// </summary>
        /// <param name="function">null indicates we're editing a variable initial value.</param>
        public ParameterWindow(Parameter parameter, string returnType, Function function = null)
        {
            InitializeComponent();
            this.Owner = MainWindow.GetMainWindow();

            Settings settings = Settings.Load();
            this.Width = settings.parameterWindowWidth;
            this.Height = settings.parameterWindowHeight;
            this.Left = settings.parameterWindowX;
            this.Top = settings.parameterWindowY;

            this.Title = returnType;


            if (function != null && function.value == "SetVariable")
            {
                ControllerVariable controller = new ControllerVariable();
                VariableRef variableRef = function.parameters[0] as VariableRef;
                if (parameter == function.parameters[0])
                    returnType = "AnyGlobal";
                else if (parameter == function.parameters[1])
                    returnType = controller.GetById(variableRef.VariableId).Type;
            }

            this.functionControl = new ParameterFunctionControl(returnType);
            grid.Children.Add(functionControl);
            Grid.SetRow(functionControl, 1);
            Grid.SetColumnSpan(functionControl, 2);

            this.constantControl = new ParameterConstantControl(returnType);
            grid.Children.Add(constantControl);
            Grid.SetRow(constantControl, 1);
            Grid.SetColumnSpan(constantControl, 2);

            this.variableControl = new ParameterVariableControl(returnType);
            grid.Children.Add(variableControl);
            Grid.SetRow(variableControl, 1);
            Grid.SetColumnSpan(variableControl, 2);

            this.valueControl = new ParameterValueControl(returnType);
            grid.Children.Add(valueControl);
            Grid.SetRow(valueControl, 1);
            Grid.SetColumnSpan(valueControl, 2);

            this.triggerRefControl = new ParameterTriggerControl();
            grid.Children.Add(triggerRefControl);
            Grid.SetRow(triggerRefControl, 1);
            Grid.SetColumnSpan(triggerRefControl, 2);

            this.importControl = new ParameterImportedControl(returnType);
            grid.Children.Add(importControl);
            Grid.SetRow(importControl, 1);
            Grid.SetColumnSpan(importControl, 2);

            this.functionControl.listControl.listView.SelectionChanged += delegate { SetSelectedItem(functionControl); };
            this.constantControl.listControl.listView.SelectionChanged += delegate { SetSelectedItem(constantControl); };
            this.variableControl.listControl.listView.SelectionChanged += delegate { SetSelectedItem(variableControl); };
            this.triggerRefControl.listControl.listView.SelectionChanged += delegate { SetSelectedItem(triggerRefControl); };
            this.importControl.listControl.listView.SelectionChanged += delegate { SetSelectedItem(importControl); };
            this.valueControl.SelectionChanged += delegate { SetSelectedItem(valueControl); };

            functionControl.listControl.listView.MouseDoubleClick += btnOK_Click;
            constantControl.listControl.listView.MouseDoubleClick += btnOK_Click;
            variableControl.listControl.listView.MouseDoubleClick += btnOK_Click;
            triggerRefControl.listControl.listView.MouseDoubleClick += btnOK_Click;
            importControl.listControl.listView.MouseDoubleClick += btnOK_Click;
            valueControl.OK += delegate { OK(); };


            IParameterControl parameterControl = constantControl; // default
            if (parameter is Function)
            {
                radioBtnFunction.IsChecked = true;
                parameterControl = functionControl;
            }
            else if (parameter is Constant)
            {
                radioBtnPreset.IsChecked = true;
                parameterControl = constantControl;
            }
            else if (parameter is VariableRef)
            {
                radioBtnVariable.IsChecked = true;
                parameterControl = variableControl;
            }
            else if (parameter is Value && returnType != "skymodelstring" && returnType != "musictheme")
            {
                radioBtnValue.IsChecked = true;
                parameterControl = valueControl;
            }
            else if (parameter is TriggerRef)
            {
                radioBtnTrigger.IsChecked = true;
                parameterControl = triggerRefControl;
            }
            else if (parameter is Value && (returnType == "skymodelstring" || returnType == "musictheme"))
            {
                radioBtnImported.IsChecked = true;
                parameterControl = importControl;
            }
            ShowHideTabs(parameterControl);
            parameterControl.SetDefaultSelection(parameter);

            if(function == null)
            {
                if(parameter is not Constant && parameter is not Value)
                {
                    ShowHideTabs(constantControl);
                    radioBtnPreset.IsChecked = true;
                }

                radioButtonList.Items.Remove(radioBtnFunction);
                radioButtonList.Items.Remove(radioBtnVariable);
                radioButtonList.Items.Remove(radioBtnTrigger);
            }
            else if (returnType == "AnyGlobal")
            {
                functionControl.Visibility = Visibility.Hidden;
                constantControl.Visibility = Visibility.Hidden;
                variableControl.Visibility = Visibility.Visible;
                valueControl.Visibility = Visibility.Hidden;
                triggerRefControl.Visibility = Visibility.Hidden;

                radioButtonList.Items.Remove(radioBtnFunction);
                radioButtonList.Items.Remove(radioBtnPreset);
                radioButtonList.Items.Remove(radioBtnValue);
                radioButtonList.Items.Remove(radioBtnTrigger);

                radioBtnVariable.IsChecked = true;
            }
            else if (returnType == "trigger")
                radioButtonList.Items.Remove(radioBtnValue);
            else
                radioButtonList.Items.Remove(radioBtnTrigger);


            if (!valueControl.ValueControlExists())
                radioButtonList.Items.Remove(radioBtnValue);
            if (returnType != "modelfile" && returnType != "skymodelstring" && returnType != "musictheme")
                radioButtonList.Items.Remove(radioBtnImported);

        }


        /// <summary>
        /// </summary>
        /// <param name="control">Control to show.</param>
        private void ShowHideTabs(IParameterControl control)
        {
            this.selectedControl = control;
            functionControl.Visibility = Visibility.Hidden;
            constantControl.Visibility = Visibility.Hidden;
            variableControl.Visibility = Visibility.Hidden;
            valueControl.Visibility = Visibility.Hidden;
            triggerRefControl.Visibility = Visibility.Hidden;
            importControl.Visibility = Visibility.Hidden;

            control.SetVisibility(Visibility.Visible);
            if (control.GetSelectedItem() != null)
                btnOK.IsEnabled = true;
            else
                btnOK.IsEnabled = false;
        }

        private void SetSelectedItem(IParameterControl control)
        {
            selectedParameter = control.GetSelectedItem();
            if (selectedParameter != null)
            {
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

        private void radioBtnTrigger_Checked(object sender, RoutedEventArgs e)
        {
            ShowHideTabs(triggerRefControl);
        }


        private void radioBtnValue_Checked(object sender, RoutedEventArgs e)
        {
            ShowHideTabs(valueControl);
        }

        private void radioBtnImported_Checked(object sender, RoutedEventArgs e)
        {
            ShowHideTabs(importControl);
        }

        private void OK()
        {
            this.selectedParameter = selectedControl.GetSelectedItem();
            if (this.selectedParameter == null)
                return;

            this.isOK = true;
            this.Close();
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            OK();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && selectedControl.GetSelectedItem() != null)
            {
                this.selectedParameter = selectedControl.GetSelectedItem();
                this.isOK = true;
                this.Close();
            }
        }
        
        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Settings settings = Settings.Load();
            settings.parameterWindowWidth = (int)this.Width;
            settings.parameterWindowHeight = (int)this.Height;
        }

        private void Window_LocationChanged(object sender, EventArgs e)
        {
            Settings settings = Settings.Load();
            settings.parameterWindowX = (int)this.Left;
            settings.parameterWindowY = (int)this.Top;
        }

    }
}
