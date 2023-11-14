using BetterTriggers.Containers;

namespace BetterTriggers.Utility
{
    public static class NameGenerator
    {
        public static string GenerateCategoryName()
        {
            string nameTemplate = "Untitled Category";
            string name = nameTemplate;
            int i = 0;
            bool isValid = false;
            var folders = Project.CurrentProject.Folders;
            while (!isValid)
            {
                if (folders.Contains(name))
                {
                    name = nameTemplate + " " + i;
                }
                else
                {
                    isValid = true;
                }

                i++;
            }

            return name;
        }

        public static string GenerateTriggerName()
        {
            string nameTemplate = "Untitled Trigger";
            string name = nameTemplate;
            int i = 0;
            bool isValid = false;
            var triggers = Project.CurrentProject.Triggers;
            while (!isValid)
            {
                if (triggers.Contains(name))
                {
                    name = nameTemplate + " " + i;
                }
                else
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
            int i = 0;
            bool isValid = false;
            var scripts = Project.CurrentProject.Scripts;
            while (!isValid)
            {
                if (scripts.Contains(name))
                {
                    name = nameTemplate + " " + i;
                }
                else
                {
                    isValid = true;
                }

                i++;
            }

            return name;
        }

        public static string GenerateVariableName()
        {
            string nameTemplate = "UntitledVariable";
            string name = nameTemplate;
            int i = 0;
            bool isValid = false;
            var variables = Project.CurrentProject.Variables;
            while (!isValid)
            {
                if (variables.Contains(name))
                {
                    name = nameTemplate + i;
                }
                else
                {
                    isValid = true;
                }

                i++;
            }

            return name;
        }
    }
}
