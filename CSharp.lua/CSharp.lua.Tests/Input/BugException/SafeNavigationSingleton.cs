namespace TestBugException
{
    internal class A
    {
        private static A _a;
        private B _b;

        public static B B => _a?._b;
    }

    internal class B
    {
    }
}
