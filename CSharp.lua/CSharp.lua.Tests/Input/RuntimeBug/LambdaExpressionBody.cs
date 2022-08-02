using System;
using System.Collections.Generic;

namespace Test
{
  internal class InputHandler
  {
    public readonly Dictionary<int, EventHandler> OnKeyUp;
    public readonly Dictionary<int, EventHandler> OnKeyDown;

    private void BugTest()
    {
      int action = 0;
      var isKeyDown = true;
      if (!PInvoke(() => { (isKeyDown ? OnKeyDown : OnKeyUp)[action]?.Invoke(this, EventArgs.Empty); }, out var err1))
      {
        // works
      }
      
      if (!PInvoke(() => (isKeyDown ? OnKeyDown : OnKeyUp)[action]?.Invoke(this, EventArgs.Empty), out var err2))
      {
        // works (fixed in #274)
      }
    }

    /// @CSharpLua.Template = "pcall({0})"
    private static extern bool PInvoke( Action func, out string err );
  }
}
