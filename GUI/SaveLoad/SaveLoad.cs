using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;

namespace GUI.SaveLoad
{
    public static class SaveLoad
    {
        public static string projectPath;
        
        public static void Save()
        {
            if(projectPath == null)
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                if (saveFileDialog.ShowDialog() == true)
                    File.WriteAllText(saveFileDialog.FileName, "");

                return;
            } else
                File.WriteAllText(projectPath, "");
        }

        public static void SaveStringAs(string json)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            if (saveFileDialog.ShowDialog() == true)
                File.WriteAllText(saveFileDialog.FileName, json);
        }
    }
}
