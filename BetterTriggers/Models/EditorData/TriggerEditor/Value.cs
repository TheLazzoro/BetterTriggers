namespace BetterTriggers.Models.EditorData
{
    public class Value : Parameter
    {
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
