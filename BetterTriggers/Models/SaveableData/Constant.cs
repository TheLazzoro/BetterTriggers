namespace BetterTriggers.Models.SaveableData
{
    /// <summary>
    /// Things like 'Player00' or 'DestructableNull'
    /// </summary>
    public class Constant : Parameter
    {
        public readonly int ParamType = 2; // DO NOT CHANGE

        public override Constant Clone()
        {
            string value = null;
            if (this.value != null)
                value = new string(this.value);

            return new Constant()
            {
                value = value,
            };
        }
    }
}
