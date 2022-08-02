using System;
using System.Collections.Generic;
using System.Text;

namespace Test {
  class Api {
    /// @CSharpLua.Template = "math.pi"
    public static readonly float PI = 3.14f;

    /// @CSharpLua.Template = "print({0})"
    public static extern void Print( object a );

    /// @CSharpLua.Template = "pcall({0})"
    public static extern bool PInvoke( Action f, out string errorMessage );
  }

  class TestTranspileTemplate {
    private float _pi;

    public TestTranspileTemplate() {
      if ( !Api.PInvoke( () => { _pi = Api.PI; }, out var error ) ) {
        Api.Print( error );
      }
    }
  }
}
