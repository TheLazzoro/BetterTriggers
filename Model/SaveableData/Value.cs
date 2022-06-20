namespace Model.SaveableData
{
    public class Value : Parameter
    {
        public readonly int ParamType = 4; // DO NOT CHANGE

        public new Value Clone()
        {
            string identifier = null;
            if (this.identifier != null)
                identifier = new string(this.identifier);
            return new Value()
            {
                identifier = identifier,
                returnType = new string(returnType),
            };
        }
    }
}
