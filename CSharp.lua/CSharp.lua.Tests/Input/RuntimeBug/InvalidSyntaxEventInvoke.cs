using System;
using System.Collections.Generic;
using System.Text;

namespace CSharp.lua.Tests.Input.RuntimeBug
{
  class InvalidSyntaxEventInvoke
  {
    public static event System.EventHandler Initialized;

    public static bool Init()
    {
      Initialized?.Invoke( null, null );

      return true;
    }
  }
}
