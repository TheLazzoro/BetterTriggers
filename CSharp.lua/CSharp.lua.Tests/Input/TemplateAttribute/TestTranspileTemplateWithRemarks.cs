using System;
using System.Collections.Generic;
using System.Text;

namespace Test {
  class Api
    {
        /// <summary>
        /// The number Pi.
        /// </summary>
        /// <remarks>
        /// @CSharpLua.Template = "math.pi"
        /// </remarks>
        public static readonly float PI = 3.14f;

        /// <summary>
        /// Prints something...
        /// </summary>
        /// <remarks>
        /// @CSharpLua.Template = "print({0})"
        /// </remarks>
        public static extern void Print( object a );

        /// <summary>
        /// Run a function in protected mode.
        /// </summary>
        /// <remarks>
        /// @CSharpLua.Template = "pcall({0})"
        /// </remarks>
        public static extern bool PInvoke( Action f, out string errorMessage );
    }
  class TestTranspileTemplate {

    private float _pi;

        public TestTranspileTemplate()
        {
            if ( !Api.PInvoke( () => { _pi = Api.PI; }, out var error ) )
            {
                Api.Print( error );
            }
        }
    }
}
