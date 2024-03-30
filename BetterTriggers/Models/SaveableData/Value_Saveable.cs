namespace BetterTriggers.Models.SaveableData
{
    public class Value_Saveable : Parameter_Saveable
    {
        public readonly int ParamType = 5; // DO NOT CHANGE

        public override Value_Saveable Clone()
        {
            string value = null;
            if (this.value != null)
                value = new string(this.value);
            return new Value_Saveable()
            {
                value = value,
            };
        }
    }
}
