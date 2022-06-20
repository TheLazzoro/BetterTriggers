namespace Model.SaveableData
{
    public class SetVariable : Function
    {
        public readonly int ParamType = 13; // DO NOT CHANGE
        
        public new SetVariable Clone()
        {
            SetVariable setVariable = new SetVariable();

            Function f = base.Clone();
            setVariable.identifier = f.identifier;
            setVariable.returnType = f.returnType;
            setVariable.parameters = f.parameters;

            return setVariable;
        }
    }
}
