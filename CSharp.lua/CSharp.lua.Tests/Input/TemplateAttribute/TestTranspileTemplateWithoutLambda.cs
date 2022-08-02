using System;
using System.Collections.Generic;
using System.Text;

namespace Test {
  class Api
    {
        /// <summary>
        /// @CSharpLua.Template = "math.pi"
        /// </summary>
        public static readonly float PI = 3.14f;

        /// <summary>
        /// @CSharpLua.Template = "print({0})"
        /// </summary>
        public static extern void Print( object a );

        /// <summary>
        /// @CSharpLua.Template = "pcall({0})"
        /// </summary>
        public static extern bool PInvoke( Action f, out string errorMessage );
    }
  class TestTranspileTemplate {

    private float _pi;

        public TestTranspileTemplate()
        {
            Action action = SetPi;
            if ( !Api.PInvoke( action, out var error ) )
            {
                Api.Print( error );
            }
        }

        private void SetPi()
        {
            _pi = Api.PI;
        }
    }
}
