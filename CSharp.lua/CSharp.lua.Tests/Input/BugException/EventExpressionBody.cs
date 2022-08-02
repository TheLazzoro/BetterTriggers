using System;
namespace EventTest
{
  public static class TestClass
  {
    public static event EventHandler<EventArgs> PlayerLeave
    {
// works
      // add { Add(value); }
      // remove { Remove(value); }

// does not work
      add => Add(value);
      remove => Remove(value);
    }

    private static void Add( EventHandler<EventArgs> handler )
    {
    }

    private static void Remove( EventHandler<EventArgs> handler )
    {
    }
  }
}
