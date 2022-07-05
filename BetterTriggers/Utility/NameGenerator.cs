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
            while (!isValid)
            {
                if (Folders.Contains(name))
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
            while (!isValid)
            {
                if (Triggers.Contains(name))
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
            while (!isValid)
            {
                if (Scripts.Contains(name))
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
            while (!isValid)
            {
                if (Variables.Contains(name))
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
