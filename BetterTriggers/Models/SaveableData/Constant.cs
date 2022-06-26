namespace BetterTriggers.Models.SaveableData
{
    /// <summary>
    /// Things like 'Player00' or 'DestructableNull'
    /// </summary>
    public class Constant : Parameter
    {
        public readonly int ParamType = 2; // DO NOT CHANGE

        public new Constant Clone()
        {
            string identifier = null;
            if (this.identifier != null)
                identifier = new string(this.identifier);

            return new Constant()
            {
                identifier = identifier,
                returnType = new string(returnType),
            };
        }
    }
}
