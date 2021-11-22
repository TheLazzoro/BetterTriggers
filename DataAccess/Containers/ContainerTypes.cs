
using DataAccess.Natives;
using System;
using System.Collections.Generic;

namespace DataAccess.Containers
{
    public static class ContainerTypes
    {
        private static List<Natives.Type> container = new List<Natives.Type>();

        public static int Size()
        {
            return container.Count;
        }
        
        public static void AddType(Natives.Type type)
        {
            bool alreadyExists = false;
            string whichType = string.Empty;
            int index = 0;

            for(int i = 0; i < container.Count; i++)
            {
                if(container[i].type == type.type || container[i].name == type.name)
                {
                    alreadyExists = true;
                    whichType = container[i].type;
                    index = i;
                    break;
                }
            }

            if (!alreadyExists)
                container.Add(type);
            else
                Console.WriteLine($"At {index}: Type '{type.type}' already exists as '{whichType}' in the container.");
        }

        public static List<Natives.Type> GetAllTypes()
        {
            return container;
        }

        public static void SetList(List<Natives.Type> list)
        {
            if (list != null)
                container = list;
            else
                container = new List<Natives.Type>();
        }
    }
}
