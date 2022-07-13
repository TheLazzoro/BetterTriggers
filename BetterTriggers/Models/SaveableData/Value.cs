namespace BetterTriggers.Models.SaveableData
{
    public class Value : Parameter
    {
        public readonly int ParamType = 5; // DO NOT CHANGE

        public override Value Clone()
        {
            string value = null;
            if (this.value != null)
                value = new string(this.value);
            return new Value()
            {
                value = value,
            };
        }
    }
}
