namespace BetterTriggers.Models.SaveableData
{
    /// <summary>
    /// Things like 'Player00' or 'DestructableNull'
    /// </summary>
    public class Constant_Saveable : Parameter_Saveable
    {
        public readonly int ParamType = 2; // DO NOT CHANGE

        public override Constant_Saveable Clone()
        {
            string value = null;
            if (this.value != null)
                value = new string(this.value);

            return new Constant_Saveable()
            {
                value = value,
            };
        }
    }
}
