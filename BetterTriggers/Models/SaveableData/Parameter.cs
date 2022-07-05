using BetterTriggers.JsonBaseConverter;
using Newtonsoft.Json;

namespace BetterTriggers.Models.SaveableData
{
    [JsonConverter(typeof(BaseConverter))]
    public class Parameter
    {
        public string identifier; // For convenience, values also use this field.

        public Parameter Clone()
        {
            string identifier = null;
            if (this.identifier != null)
                identifier = new string(this.identifier);

            return new Parameter()
            {
                identifier = identifier,
            };
        }
    }
}
