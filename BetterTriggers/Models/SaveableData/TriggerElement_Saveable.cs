using BetterTriggers.Containers;
using BetterTriggers.JsonBaseConverter;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetterTriggers.Models.SaveableData
{
    [JsonConverter(typeof(BaseConverterTriggerElement))]
    public class TriggerElement_Saveable
    {
        public virtual TriggerElement_Saveable Clone()
        {
            return new TriggerElement_Saveable();
        }
    }
}
