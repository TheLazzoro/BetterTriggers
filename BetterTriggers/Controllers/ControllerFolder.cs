using BetterTriggers.Containers;
using System.IO;

namespace BetterTriggers.Controllers
{
    public class ControllerFolder
    {
        /// <summary>
        /// Creates a folder at the destination folder.
        /// </summary>
        public static string Create()
        {
            string directory = ContainerProject.currentSelectedElement;
            if (!Directory.Exists(directory))
                directory = Path.GetDirectoryName(directory);

            string name = "Untitled Category";
            bool ok = false;
            int i = 0;
            while (!ok)
            {
                if(!Directory.Exists(directory + @"\" + name))
                    ok = true;
                else
                {
                    name = "Untitled Category " + i;
                }

                i++;
            }

            string path = Path.Combine(directory, name);
            Directory.CreateDirectory(path);

            return path;
        }
    }
}
