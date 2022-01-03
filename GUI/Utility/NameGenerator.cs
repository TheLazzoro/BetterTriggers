using GUI.Containers;
using System;
using System.Collections.Generic;
using System.Text;

namespace GUI.Utility
{
    public static class NameGenerator
    {
        public static string GenerateCategoryName()
        {
            string nameTemplate = "Untitled Category";
            string name = nameTemplate;
            int suffix = 0;
            for (int i = 0; i < ContainerFolders.Count(); i++)
            {
                var element = ContainerFolders.Get(i);
                if (element.Name == name)
                {
                    suffix++;
                    name = nameTemplate + " " + suffix;
                }
            }

            return name;
        }

        public static string GenerateTriggerName()
        {
            string nameTemplate = "Untitled Trigger";
            string name = nameTemplate;
            int i = 0;
            bool isValid = false;
            while (!isValid)
            {
                if (ContainerTriggers.Contains(name))
                {
                    name = nameTemplate + " " + i;
                } else
                {
                    isValid = true;
                }

                i++;
            }

            return name;
        }

        public static string GenerateScriptName()
        {
            string nameTemplate = "Untitled Script";
            string name = nameTemplate;
            int suffix = 0;
            for (int i = 0; i < ContainerScripts.Count(); i++)
            {
                var element = ContainerScripts.Get(i);
                if (element.Name == name)
                {
                    suffix++;
                    name = nameTemplate + " " + suffix;
                }
            }

            return name;
        }

        public static string GenerateVariableName()
        {
            string nameTemplate = "UntitledVariable";
            string name = nameTemplate;
            int suffix = 0;
            for (int i = 0; i < ContainerVariables.Count(); i++)
            {
                var element = ContainerVariables.Get(i);
                if (element.Name == name)
                {
                    suffix++;
                    name = nameTemplate + suffix;
                }
            }

            return name;
        }
    }
}
