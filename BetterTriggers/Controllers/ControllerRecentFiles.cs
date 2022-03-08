using BetterTriggers.Containers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace BetterTriggers.Controllers
{
    public class ControllerRecentFiles
    {
        private static List<string> recentFiles = new List<string>();
        private static string pathRecentFiles = System.IO.Directory.GetCurrentDirectory() + @"\" + @"Resources\" + "recent";

        public List<string> GetRecentFiles()
        {
            if (recentFiles.Count == 0)
            {
                recentFiles = new List<string>();

                string[] recentFilesLoaded = null;
                if (File.Exists(pathRecentFiles))
                {
                    recentFilesLoaded = File.ReadAllLines(pathRecentFiles);
                    for (int i = 0; i < recentFilesLoaded.Length; i++)
                    {
                        recentFiles.Insert(i, recentFilesLoaded[i]);
                    }
                }
            }

            List<string> lastTenFiles = new List<string>();
            int f = 0;
            int max = 10;
            while (f < recentFiles.Count && f < max)
            {
                lastTenFiles.Insert(f, recentFiles[f]);
                f++;
            }

            return lastTenFiles;
        }

        public void AddProjectToRecent(string projectFilePath)
        {
            if (recentFiles.Contains(projectFilePath))
            {
                recentFiles.Remove(projectFilePath);
            }

            recentFiles.Insert(0, projectFilePath);
            Save();
        }

        public void RemoveRecentByPath(string filePath)
        {
            if (recentFiles.Contains(filePath))
                recentFiles.Remove(filePath);

            Save();
        }

        private void Save()
        {
            string saveable = string.Empty;

            int i = 0;
            int max = 10;
            while (i < recentFiles.Count && i < max)
            {
                saveable += recentFiles[i] + "\n";
                i++;
            }

            File.WriteAllText(pathRecentFiles, saveable);
        }
    }
}
