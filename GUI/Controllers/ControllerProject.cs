using GUI.Components.TriggerExplorer;
using GUI.Containers;
using GUI.Utility;
using Model.Data;
using Model.War3Project;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Controls;

namespace GUI.Controllers
{
    public class ControllerProject
    {
        public static IExplorerElement currentTriggerExplorerElement;

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

        public void LoadProject(TriggerExplorer triggerExplorer, Grid mainGrid, string filepath)
        {
            string json = File.ReadAllText(filepath);
            War3Project project = JsonConvert.DeserializeObject<War3Project>(json);

            LoadProjectShared(triggerExplorer, mainGrid, project);
        }

        public void LoadProject(TriggerExplorer triggerExplorer, Grid mainGrid, War3Project project)
        {
            LoadProjectShared(triggerExplorer, mainGrid, project);
        }

        private void LoadProjectShared(TriggerExplorer triggerExplorer, Grid mainGrid, War3Project project)
        {
            triggerExplorer.CreateTreeViewItem(new TreeViewItem(), project.Name, EnumCategory.Map);
            LoadFiles(project.Root, triggerExplorer.map);
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
                    TreeViewManipulator.SetTreeViewItemAppearance(item, Reader.GetFileNameAndExtension(entry), EnumCategory.Folder);
                    LoadFiles(entry, item);
                }
                else if (File.Exists(entry))
                {
                    ExplorerElement item = new ExplorerElement(entry);
                    parentNode.Items.Add(item);

                    switch (Reader.GetFileExtension(entry))
                    {
                        case ".json":
                            TreeViewManipulator.SetTreeViewItemAppearance(item, Reader.GetFileName(entry), EnumCategory.Trigger);
                            ContainerTriggers.AddTriggerElement(item);
                            break;
                        case ".j":
                            TreeViewManipulator.SetTreeViewItemAppearance(item, Reader.GetFileName(entry), EnumCategory.AI);
                            ContainerScripts.AddTriggerElement(item);
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
                    case ".json":
                        ControllerTrigger controller = new ControllerTrigger();
                        Model.Trigger trigger = controller.LoadTriggerFromFile(selectedElement.FilePath);
                        var triggerControl = controller.CreateTriggerWithElements(tabControl, trigger);
                        tabControl.Items.Add(TabItemManipulator.SetTabItemApperance(triggerControl, "testname"));
                        selectedElement.Ielement = triggerControl;
                        break;
                    case ".j":
                        break;
                    default:
                        break;
                }

                if(selectedElement.Ielement != null)
                {
                    currentTriggerExplorerElement = selectedElement.Ielement;
                    selectedElement.Ielement.OnElementClick();
                }
            }
        }
    }
}
