using BetterTriggers.Containers;
using BetterTriggers.Models.EditorData;
using BetterTriggers.Utility;
using GUI.Components.VariableEditor;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace GUI.Components.TriggerEditor.ParameterControls
{
    public class VariableItem
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public VariableRef VariableRef { get; set; }
        public bool IsSelected { get; set; }

        public VariableItem(string name, bool isLocal, VariableRef variableRef)
        {
            this.Name = name;
            this.Type = isLocal ? "Local" : "Global";
            this.VariableRef = variableRef;
        }
    }


    public partial class ParameterVariableControl : UserControl, IParameterControl, ISearchablesObserverList
    {
        private VariableItem selectedItem;
        private Searchables searchables;
        private ParameterVariableControlViewModel viewModel;
        private ExplorerElement? explorerElement;

        /// <summary>
        /// </summary>
        /// <param name="returnType"></param>
        /// <param name="localVariables"></param>
        public ParameterVariableControl(string returnType, ExplorerElement? explorerElement = null)
        {
            InitializeComponent();

            viewModel = new ParameterVariableControlViewModel();
            DataContext = viewModel;

            if (returnType == "VarAsString_Real")
                returnType = "real";
            else if (returnType == "StringExt")
                returnType = "string";


            this.explorerElement = explorerElement;
            TriggerElementCollection? localVariables = null;
            if(explorerElement != null)
            {
                localVariables = explorerElement.GetLocalVariables();
            }
            var project = Project.CurrentProject;
            List<Variable> variables = project.Variables.GetVariables(returnType, Variables.includeLocals, localVariables);
            List<Searchable> objects = new List<Searchable>();

            for (int i = 0; i < variables.Count; i++)
            {
                var variable = variables[i];
                var variableRef = project.Variables.GetVariableRef(variable);
                var variableItem = new VariableItem(variable.Name, variable._isLocal, variableRef);

                objects.Add(new Searchable()
                {
                    Object = variableItem,
                    Words = new List<string>()
                    {
                        variable.Name.ToLower(),
                    },
                });
            }
            listView.SelectionChanged += ListView_SelectionChanged;

            searchables = new Searchables(objects);
            searchables.AttachList(this);
            searchables.Search("");
        }

        public void SetDefaultSelection(Parameter parameter)
        {
            var project = Project.CurrentProject;
            int i = 0;
            bool found = false;
            Variable selected = null;
            if (parameter is VariableRef)
                selected = project.Variables.GetByReference(parameter as VariableRef, explorerElement);

            if (selected == null)
                return;

            var items = searchables.GetAllObject();
            VariableItem defaultSelected = null;
            while (!found && i < items.Count)
            {
                var variableItem = (VariableItem)items[i].Object;
                var variable = project.Variables.GetByReference(variableItem.VariableRef, explorerElement);
                if (variable == selected)
                {
                    variableItem.IsSelected = true;
                    defaultSelected = variableItem;
                    listView.ScrollIntoView(defaultSelected);
                    found = true;
                }
                else
                    i++;
            }
        }


        public int GetElementCount()
        {
            return searchables.GetAllObject().Count;
        }

        public Parameter GetSelectedItem()
        {
            if (selectedItem == null)
                return null;

            var variables = selectedItem.VariableRef;
            return variables;
        }

        public void SetVisibility(Visibility visibility)
        {
            this.Visibility = visibility;
        }

        public void Update()
        {
            viewModel.Variables.Clear();
            var variables = searchables.GetObjects();
            foreach (var searchItem in variables)
            {
                var variableItem = (VariableItem)searchItem.Object;
                viewModel.Variables.Add(variableItem);
            }
        }

        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selectedItem = listView.SelectedItem as VariableItem;
        }

        private void textBoxSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            searchables.Search(textBoxSearch.Text);
        }
    }
}
