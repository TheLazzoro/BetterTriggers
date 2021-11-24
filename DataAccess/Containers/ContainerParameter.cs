
using DataAccess.Natives;
using System;
using System.Collections.Generic;

namespace DataAccess.Containers
{
    public static class ContainerParameter
    {
        private static List<Parameter> container = new List<Parameter>();

        public static void AddParameter(Parameter parameter)
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

        public static List<Parameter> GetAllTypes()
        {
            return container;
        }

        public static void SetList(List<Parameter> list)
        {
            if (list != null)
                container = list;
            else
                container = new List<Parameter>();
        }
    }
}
