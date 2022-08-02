using System;

class NullCoalescingCondition {
  static bool Test(Func<bool> condition) {
    return condition?.Invoke() ?? true;
  }
}
