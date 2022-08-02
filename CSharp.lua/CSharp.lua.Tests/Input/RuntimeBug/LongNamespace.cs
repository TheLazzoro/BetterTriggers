using System;
using System.Collections.Generic;
using System.Text;

using This.Namespace.Is.Waaaaaaaaaaay.Too.Long;
using This.Namespace.Is.Also.Way.Too.Long;
using This.Long;

namespace This.Namespace.Is.Waaaaaaaaaaay.Too.Long {
  class Something {
    public static void Foo() { }
  }
}

namespace This.Namespace.Is.Also.Way.Too.Long {
  class Otherthing {
    public static void Bar() { }
  }
}

namespace This.Long {
  class NotSoLong {
    public static void HelloWorld() { }
  }
}

namespace Test {
  static class Test {
    static void TestMethod() {
      Something.Foo();
      Otherthing.Bar();
      NotSoLong.HelloWorld();
    }
  }
}
