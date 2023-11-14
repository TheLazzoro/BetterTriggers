using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace BetterTriggers.Containers
{
    public static class RecentFiles
    {
        public static bool isTest = true;
        private static List<string> recentFiles = new List<string>();
        private static string pathRecentFiles = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Better Triggers/recent.txt");

        public static List<string> GetRecentFiles()
        {
            if (recentFiles.Count == 0)
            {
                recentFiles = new List<string>();

                string[] recentFilesLoaded;
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

        public static void AddProjectToRecent(string projectFilePath)
        {
            if (isTest)
                return;

            if (recentFiles.Count == 0)
            {
                GetRecentFiles(); // Loads there are no recent files.
            }

            if (recentFiles.Contains(projectFilePath))
            {
                recentFiles.Remove(projectFilePath);
            }

            recentFiles.Insert(0, projectFilePath);
            Save();
        }

        private static void Save()
        {
            string saveable = string.Empty;

            int i = 0;
            int max = 10;
            while (i < recentFiles.Count && i < max)
            {
                saveable += recentFiles[i] + "\n";
                i++;
            }

            if (!Directory.Exists(Path.GetDirectoryName(pathRecentFiles)))
                Directory.CreateDirectory(Path.GetDirectoryName(pathRecentFiles));

            File.WriteAllText(pathRecentFiles, saveable);
        }
    }
}
