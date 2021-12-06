
using Model.Natives;
using System;
using System.Collections.Generic;

namespace Model.Containers
{
    public static class ContainerFunctions
    {
        private static List<Function> container = new List<Function>();

        public static void AddParameter(Function parameter)
        {
            bool alreadyExists = false;
            string whichType = string.Empty;
            int index = 0;

            for (int i = 0; i < container.Count; i++)
            {
                if(container[i].identifier == parameter.identifier || container[i].name == parameter.name)
                {
                    alreadyExists = true;
                    whichType = container[i].identifier;
                    index = i;
                    break;
                }
            }

            if (!alreadyExists)
                container.Add(parameter);
            else
                Console.WriteLine($"At {index}: Type '{parameter.identifier}' already exists as '{whichType}' in the container.");
        }

        public static List<Function> GetAllTypes()
        {
            return container;
        }

        public static void SetList(List<Function> list)
        {
            if (list != null)
                container = list;
            else
                container = new List<Function>();
        }
    }
}
