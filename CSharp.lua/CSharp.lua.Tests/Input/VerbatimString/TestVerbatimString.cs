namespace Test.VerbatimString {
  class TestEmptyVerbatimString {
    private static readonly string Test = @"hello world";
    public TestEmptyVerbatimString() {
      var test = Test;
    }
  }
}
