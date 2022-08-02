using System;
using System.Collections.Generic;
using System.Text;

namespace CSharp.lua.Tests.Input.RuntimeBug
{
  class CompoundBitShift
  {
    static void test()
    {
      var x = 1 >> 1;
      x >>= 1;

      var y = 1 << 1;
      y <<= 1;
    }
  }
}
