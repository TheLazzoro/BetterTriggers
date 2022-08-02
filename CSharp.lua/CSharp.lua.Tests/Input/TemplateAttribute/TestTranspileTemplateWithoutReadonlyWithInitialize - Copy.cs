using System;
using System.Collections.Generic;
using System.Text;

namespace Test {
  class Api
    {
        /// <summary>
        /// @CSharpLua.Template = "math.pi"
        /// </summary>
        public static float PI = 3.14f;

        /// <summary>
        /// @CSharpLua.Template = "print({0})"
        /// </summary>
        public static extern void Print( object a );
    }
  class TestTranspileTemplate {

    private readonly float _pi;

    public TestTranspileTemplate() {
      _pi = Api.PI;

      Api.Print(_pi);
    }
  }
}
