
using DataAccess.Natives;
using System;
using System.Collections.Generic;

namespace DataAccess.Containers
{
    public static class ContainerConstants
    {
        private static List<Natives.Constant> container = new List<Natives.Constant>();

        public static int Size()
        {
            return container.Count;
        }
        
        public static void AddConstant(Natives.Constant constant)
        {
            bool alreadyExists = false;
            string whichType = string.Empty;
            int index = 0;

            for(int i = 0; i < container.Count; i++)
            {
                if(container[i].identifier == constant.identifier || container[i].name == constant.name)
                {
                    alreadyExists = true;
                    whichType = container[i].identifier;
                    index = i;
                    break;
                }
            }

            if (!alreadyExists)
                container.Add(constant);
            else
                Console.WriteLine($"At {index}: Type '{constant.identifier}' already exists as '{whichType}' in the container.");
        }

        public static List<Natives.Constant> GetAllTypes()
        {
            return container;
        }

        public static void SetList(List<Natives.Constant> list)
        {
            if (list != null)
                container = list;
            else
                container = new List<Natives.Constant>();
        }
    }
}
