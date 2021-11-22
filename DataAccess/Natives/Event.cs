using DataAccess.Containers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Natives
{
    public class Event
    {
        public string identifier;
        public List<Type> parameters = new List<Type>();
        public string name;
        public string eventText;
        public string description;

        public Event(string identifier, List<Type> parameters, string name, string eventText, string description)
        {
            this.identifier = identifier;
            this.parameters = parameters;
            this.name = name;
            this.eventText = eventText;
            this.description = description;

            ContainerEvents.AddEvent(this);
        }
    }
}
