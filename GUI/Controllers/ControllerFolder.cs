using GUI.Components.TriggerExplorer;
using GUI.Utility;
using Model.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Controls;

namespace GUI.Controllers
{
    public class ControllerFolder
    {
        public void CreateFolder(TriggerExplorer triggerExplorer)
        {
            ControllerProject controllerProject = new ControllerProject();
            string directory = controllerProject.GetDirectoryFromSelection(triggerExplorer.treeViewTriggerExplorer);
            string name = NameGenerator.GenerateCategoryName();

            Directory.CreateDirectory(directory + "/" + name);
        }
    }
}
