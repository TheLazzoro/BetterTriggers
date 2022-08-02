using System.Collections.Generic;
using System.IO;

namespace CSharpLua.CoreSystem {
  public static class CoreSystemProvider {
    private const string CoreSystemDirectory = @".\CoreSystem";

    public static IEnumerable<string> GetCoreSystemFiles() {
      yield return Path.Combine(CoreSystemDirectory, @"Natives.lua");
      yield return Path.Combine(CoreSystemDirectory, @"Core.lua");
      yield return Path.Combine(CoreSystemDirectory, @"Interfaces.lua");
      yield return Path.Combine(CoreSystemDirectory, @"Exception.lua");
      yield return Path.Combine(CoreSystemDirectory, @"Number.lua");
      yield return Path.Combine(CoreSystemDirectory, @"Char.lua");
      yield return Path.Combine(CoreSystemDirectory, @"String.lua");
      yield return Path.Combine(CoreSystemDirectory, @"Boolean.lua");
      yield return Path.Combine(CoreSystemDirectory, @"Delegate.lua");
      yield return Path.Combine(CoreSystemDirectory, @"Enum.lua");
      yield return Path.Combine(CoreSystemDirectory, @"TimeSpan.lua");
      yield return Path.Combine(CoreSystemDirectory, @"DateTime.lua");
      yield return Path.Combine(CoreSystemDirectory, @"Common.lua");
      yield return Path.Combine(CoreSystemDirectory, @"Collections\EqualityComparer.lua");
      yield return Path.Combine(CoreSystemDirectory, @"Array.lua");
      yield return Path.Combine(CoreSystemDirectory, @"Type.lua");
      yield return Path.Combine(CoreSystemDirectory, @"Collections\List.lua");
      yield return Path.Combine(CoreSystemDirectory, @"Collections\Dictionary.lua");
      yield return Path.Combine(CoreSystemDirectory, @"Collections\Queue.lua");
      yield return Path.Combine(CoreSystemDirectory, @"Collections\Stack.lua");
      yield return Path.Combine(CoreSystemDirectory, @"Collections\HashSet.lua");
      yield return Path.Combine(CoreSystemDirectory, @"Collections\LinkedList.lua");
      yield return Path.Combine(CoreSystemDirectory, @"Collections\Linq.lua");
      yield return Path.Combine(CoreSystemDirectory, @"Collections\SortedSet.lua");
      yield return Path.Combine(CoreSystemDirectory, @"Convert.lua");
      yield return Path.Combine(CoreSystemDirectory, @"Math.lua");
      yield return Path.Combine(CoreSystemDirectory, @"Random.lua");
      yield return Path.Combine(CoreSystemDirectory, @"Text\StringBuilder.lua");
      yield return Path.Combine(CoreSystemDirectory, @"Console.lua");
      yield return Path.Combine(CoreSystemDirectory, @"IO\File.lua");
      yield return Path.Combine(CoreSystemDirectory, @"Reflection\Assembly.lua");
      yield return Path.Combine(CoreSystemDirectory, @"Threading\Timer.lua");
      yield return Path.Combine(CoreSystemDirectory, @"Threading\Thread.lua");
      yield return Path.Combine(CoreSystemDirectory, @"Threading\Task.lua");
      yield return Path.Combine(CoreSystemDirectory, @"Utilities.lua");
      yield return Path.Combine(CoreSystemDirectory, @"Globalization\Globalization.lua");
      yield return Path.Combine(CoreSystemDirectory, @"Numerics\HashCodeHelper.lua");
      yield return Path.Combine(CoreSystemDirectory, @"Numerics\Complex.lua");
      yield return Path.Combine(CoreSystemDirectory, @"Numerics\Vector2.lua");
      yield return Path.Combine(CoreSystemDirectory, @"Numerics\Vector3.lua");
      yield return Path.Combine(CoreSystemDirectory, @"Numerics\Vector4.lua");
      yield return Path.Combine(CoreSystemDirectory, @"Numerics\Matrix3x2.lua");
      yield return Path.Combine(CoreSystemDirectory, @"Numerics\Matrix4x4.lua");
      yield return Path.Combine(CoreSystemDirectory, @"Numerics\Plane.lua");
      yield return Path.Combine(CoreSystemDirectory, @"Numerics\Quaternion.lua");
    }
  }
}
