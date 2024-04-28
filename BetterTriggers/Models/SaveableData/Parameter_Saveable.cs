using BetterTriggers.JsonBaseConverter;
using Newtonsoft.Json;

namespace BetterTriggers.Models.SaveableData
{
    [JsonConverter(typeof(BaseConverterParameter))]
    public class Parameter_Saveable
    {
        public string value;
    }
}
