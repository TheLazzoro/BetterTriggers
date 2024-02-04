using BetterTriggers.JsonBaseConverter;
using Newtonsoft.Json;

namespace BetterTriggers.Models.SaveableData
{
    [JsonConverter(typeof(BaseConverterParameter))]
    public class Parameter_Saveable
    {
        public string value;

        public virtual Parameter_Saveable Clone()
        {
            string value = null;
            if (this.value != null)
                value = new string(this.value);

            return new Parameter_Saveable()
            {
                value = value,
            };
        }
    }
}
