using GUI.Components.TriggerExplorer;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;

namespace GUI.SaveLoad
{
    public static class SaveProject
    {
        public static void Save(TriggerExplorer triggerExplorer)
        {
            var root = triggerExplorer.map;

            string json = SaveRecurse(root);
        }

        private static string SaveRecurse(TreeViewItem node)
        {
            string json = string.Empty;
            
            for (int i = 0; i < node.Items.Count; i++)
            {
                /*
                if(node is TriggerFolder)
                {
                    
                } else if(node is Trigger)
                {

                }
                */
            }

            return json;
        }
    }
}
