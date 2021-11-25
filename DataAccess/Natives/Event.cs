using DataAccess.Containers;
using DataAccess.Data;
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
        public List<Parameter> parameters = new List<Parameter>();
        public string name;
        public string eventText;
        public string description;
        public EnumCategory category;

        public Event(string identifier, List<Parameter> parameters, string name, string eventText, string description, EnumCategory category)
        {
            this.identifier = identifier;
            this.parameters = parameters;
            this.name = name;
            this.eventText = eventText;
            this.description = description;
            this.category = category;

            ContainerEvents.AddEvent(this);
        }
    }
}
