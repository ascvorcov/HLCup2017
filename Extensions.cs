using System;
using System.Collections.Generic;

public static class Extensions
{

  public static TValue GetValueSafe<TKey, TValue>(this IDictionary<TKey,TValue> map, TKey key, TValue deflt = default(TValue))
  {
    return map.TryGetValue(key, out var ret) ? ret : deflt;
  }
}