using BetterTriggers.JsonBaseConverter;
using Newtonsoft.Json;

namespace BetterTriggers.Models.SaveableData
{
    [JsonConverter(typeof(BaseConverter))]
    public class Parameter
    {
        public string value;

        public virtual Parameter Clone()
        {
            string value = null;
            if (this.value != null)
                value = new string(this.value);

            return new Parameter()
            {
                value = value,
            };
        }
    }
}
