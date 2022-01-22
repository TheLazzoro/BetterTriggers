
using Model.Templates;
using System;
using System.Collections.Generic;

namespace Model.Containers
{
    public static class ContainerEvents
    {
        private static List<FunctionTemplate> container = new List<FunctionTemplate>();

        public static void AddEvent(FunctionTemplate _event)
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

        public static List<FunctionTemplate> GetAllTypes()
        {
            return container;
        }

        public static void SetList(List<FunctionTemplate> list)
        {
            if (list != null)
                container = list;
            else
                container = new List<FunctionTemplate>();
        }
    }
}
