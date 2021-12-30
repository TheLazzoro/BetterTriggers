using GUI.Components.TriggerExplorer;
using GUI.Utility;
using Model.Data;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;

namespace GUI.Controllers
{
    public class ControllerFolder
    {
        public void CreateFolder(TriggerExplorer triggerExplorer)
        {
            string name = NameGenerator.GenerateCategoryName();

            TreeViewItem item = new TreeViewItem();
            TriggerFolder script = new TriggerFolder();

            triggerExplorer.CreateTreeViewItem(item, name, EnumCategory.Folder);
        }
    }
}
