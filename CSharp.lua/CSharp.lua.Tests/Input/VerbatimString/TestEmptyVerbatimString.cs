namespace Test.VerbatimString {
  class TestEmptyVerbatimString {
    private static readonly string Test = @"";
    public TestEmptyVerbatimString() {
      var test = Test;
    }
  }
}
