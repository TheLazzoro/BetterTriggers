using GUI.Components.TriggerExplorer;
using GUI.Containers;
using GUI.Utility;
using Model.Data;
using Model.Enums;
using Model.War3Project;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Controls;

namespace GUI.Controllers
{
    public class ControllerProject
    {
        public static ExplorerElement currentExplorerElement;

        public War3Project CreateProject(string language, string name, string destinationFolder)
        {
            War3Project project = new War3Project()
            {
                Name = name,
                Language = language,
                Header = "",
                Root = destinationFolder + @"\" + name,
                Files = new List<string>(),
            };

            string json = JsonConvert.SerializeObject(project);

            string filepath = destinationFolder + @"\" + name + ".json";
            File.WriteAllText(filepath, json);
            Directory.CreateDirectory(destinationFolder + @"\" + name);

            return project;
        }

        public War3Project LoadProject(Grid mainGrid, string filepath)
        {
            string json = File.ReadAllText(filepath);
            War3Project project = JsonConvert.DeserializeObject<War3Project>(json);

            ContainerTriggerExplorer.CreateNewTriggerExplorer(mainGrid, project);

            LoadProjectShared(mainGrid, project);

            return project;
        }

        public void LoadProject(Grid mainGrid, War3Project project)
        {
            LoadProjectShared(mainGrid, project);
        }

        private void LoadProjectShared(Grid mainGrid, War3Project project)
        {
            ContainerTriggerExplorer.CreateNewTriggerExplorer(mainGrid, project);
            LoadFiles(project.Root, ContainerTriggerExplorer.triggerExplorer.map);

        }

        private void LoadFiles(string folder, TreeViewItem parentNode)
        {
            ControllerTrigger controllerTrigger = new ControllerTrigger();

            string[] entries = Directory.GetFileSystemEntries(folder);
            foreach (var entry in entries)
            {

                if (Directory.Exists(entry))
                {
                    ExplorerElement item = new ExplorerElement(entry);
                    ContainerFolders.AddTriggerElement(item);
                    parentNode.Items.Add(item);
                    TreeViewManipulator.SetTreeViewItemAppearance(item, Reader.GetFileNameAndExtension(entry), Category.Folder);
                    LoadFiles(entry, item);
                }
                else if (File.Exists(entry))
                {
                    ExplorerElement item = new ExplorerElement(entry);
                    parentNode.Items.Add(item);

                    switch (Reader.GetFileExtension(entry))
                    {
                        case ".trg":
                            ContainerTriggers.AddTriggerElement(item);
                            break;
                        case ".j":
                            ContainerScripts.AddTriggerElement(item);
                            break;
                        case ".var":
                            ContainerVariables.AddTriggerElement(item);
                            break;
                        default:
                            break;
                    }
                    /*
                    var file = File.ReadAllText(entry);
                    Model.Trigger trigger = JsonConvert.DeserializeObject<Model.Trigger>(file);

                    controllerTrigger.CreateTriggerWithElements(triggerExplorer, mainGrid, entry, trigger);
                    */
                }
            }
        }

        public void OnClick_ExplorerElement(ExplorerElement selectedElement, TabControl tabControl)
        {
            if (selectedElement != null && selectedElement.Ielement == null) // Load file data if the element is null
            {
                switch (Reader.GetFileExtension(selectedElement.FilePath))
                {
                    case ".trg":
                        ControllerTrigger triggerController = new ControllerTrigger();
                        Model.Trigger trigger = triggerController.LoadTriggerFromFile(selectedElement.FilePath);
                        var triggerControl = triggerController.CreateTriggerWithElements(tabControl, trigger);
                        TabItemBT tabItemTrigger = new TabItemBT(triggerControl, Reader.GetFileName(selectedElement.FilePath));
                        tabControl.Items.Add(tabItemTrigger);
                        //tabControl.ItemsSource.
                        selectedElement.tabItem = tabItemTrigger;
                        selectedElement.Ielement = triggerControl;
                        break;
                    case ".var":
                        ControllerVariable controllerVariable = new ControllerVariable();
                        Model.Data.Variable variable = controllerVariable.LoadVariableFromFile(selectedElement.FilePath);
                        variable.Name = Path.GetFileNameWithoutExtension(selectedElement.FilePath); // hack
                        var variableControl = controllerVariable.CreateVariableWithElements(tabControl, variable);
                        TabItemBT tabItemVariable = new TabItemBT(variableControl, Reader.GetFileName(selectedElement.FilePath));
                        tabControl.Items.Add(tabItemVariable);
                        //tabControl.ItemsSource.
                        selectedElement.tabItem = tabItemVariable;
                        selectedElement.Ielement = variableControl;
                        break;
                    case ".j":
                        ControllerScript scriptController = new ControllerScript();
                        var scripControl = scriptController.CreateScriptControlWithScript(tabControl, selectedElement.FilePath);
                        TabItemBT tabItemScript = new TabItemBT(scripControl, Reader.GetFileName(selectedElement.FilePath));
                        tabControl.Items.Add(tabItemScript);
                        selectedElement.tabItem = tabItemScript;
                        selectedElement.Ielement = scripControl;
                        break;
                    default:
                        break;
                }
            }
            if (selectedElement != null && selectedElement.Ielement != null)
            {
                currentExplorerElement = selectedElement;
                selectedElement.Ielement.OnElementClick();
                tabControl.SelectedItem = selectedElement.tabItem;
            }
        }

        public string GetDirectoryFromSelection(TreeView treeViewTriggerExplorer)
        {
            ExplorerElement selectedItem = treeViewTriggerExplorer.SelectedItem as ExplorerElement;

            if (selectedItem == null)
                return null;

            if (Directory.Exists(selectedItem.FilePath))
                return selectedItem.FilePath;

            else if (File.Exists(selectedItem.FilePath))
                return Path.GetDirectoryName(selectedItem.FilePath);

            return null;
        }

    }
}
