namespace Test.VerbatimString {
  class TestInterpolatedVerbatimString {
    public TestInterpolatedVerbatimString() {
      var expect = $"{100f}\\{200f}";
      var test1 = $@"{100f}\{200f}";
      var test2 = @$"{100f}\{200f}";
    }
  }
}
