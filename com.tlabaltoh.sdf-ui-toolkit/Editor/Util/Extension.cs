using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;

namespace TLab.UI.SDF.Editor
{
    public static class Extension
    {
        // https://baba-s.hatenablog.com/entry/2023/08/18/085339

        public static Vector2 Average(this IEnumerable<Vector2> self)
        {
            var enumerable = self as Vector2[] ?? self.ToArray();
            return enumerable.Aggregate(Vector2.zero, (x, y) => x + y) / enumerable.Count();
        }

        public static Vector2 Average<T>(this IEnumerable<T> self, Func<T, Vector2> func)
        {
            var enumerable = self as T[] ?? self.ToArray();
            return enumerable.Aggregate(Vector2.zero, (x, y) => x + func(y)) / enumerable.Count();
        }
    }
}
