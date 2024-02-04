namespace BetterTriggers.Models.EditorData
{
    public class SetVariable : ECA
    {
        public SetVariable()
        {
            function.value = "SetVariable";
        }

        public override SetVariable Clone()
        {
            SetVariable setVariable = new SetVariable();
            setVariable.function = this.function.Clone();

            return setVariable;
        }
    }
}
