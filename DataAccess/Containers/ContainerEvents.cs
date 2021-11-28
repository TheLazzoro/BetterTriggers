
using DataAccess.Natives;
using System;
using System.Collections.Generic;

namespace DataAccess.Containers
{
    public static class ContainerEvents
    {
        private static List<Function> container = new List<Function>();

        public static void AddEvent(Function _event)
        {
            bool alreadyExists = false;
            string whichType = string.Empty;
            int index = 0;

            for (int i = 0; i < container.Count; i++)
            {
                if(container[i].identifier == _event.identifier || container[i].name == _event.name)
                {
                    alreadyExists = true;
                    whichType = container[i].identifier;
                    index = i;
                    break;
                }
            }

            if (!alreadyExists)
                container.Add(_event);
            else
                Console.WriteLine($"At {index}: Type '{_event.identifier}' already exists as '{whichType}' in the container.");
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
