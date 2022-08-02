using System;
using System.Collections.Generic;
using System.Text;

namespace Test.TemplateAttribute {
  class TestTranspileTemplate {
    /// @CSharpLua.Template = "math.pi"
    private static readonly float PI = 3.14f;

    private float _pi;

    public TestTranspileTemplate() {
      if (!PInvoke(() => { _pi = PI; }, out var error ) )
            {
                Print( error );
            }
    }

    /// @CSharpLua.Template = "print({0})"
    public static extern void Print(object a);
        
        /// @CSharpLua.Template = "pcall({0})"
    public static extern bool PInvoke(Action f, out string errorMessage);
  }
}
